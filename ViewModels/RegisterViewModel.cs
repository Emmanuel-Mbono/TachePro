using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private readonly AuthService _auth;

    public RegisterViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _confirm = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _isBusy;

    public bool ShowEmailOk => EmailRegex.IsMatch(Email.Trim());
    public bool ShowPasswordHint => Password.Length > 0 && Password.Length < 8;
    public bool ConfirmMismatch => Confirm.Length > 0 && Confirm != Password;

    private bool CanRegister() =>
        !IsBusy && Name.Trim().Length > 1 && ShowEmailOk && Password.Length >= 8 && Confirm.Length > 0 && !ConfirmMismatch;

    partial void OnNameChanged(string value) => Refresh();
    partial void OnEmailChanged(string value) { HasError = false; Refresh(); }
    partial void OnPasswordChanged(string value) => Refresh();
    partial void OnConfirmChanged(string value) => Refresh();
    partial void OnIsBusyChanged(bool value) => RegisterCommand.NotifyCanExecuteChanged();

    private void Refresh()
    {
        OnPropertyChanged(nameof(ShowEmailOk));
        OnPropertyChanged(nameof(ShowPasswordHint));
        OnPropertyChanged(nameof(ConfirmMismatch));
        RegisterCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        IsBusy = true;
        try
        {
            var (ok, error) = await _auth.RegisterAsync(Name, Email, Password);
            if (!ok)
            {
                ErrorMessage = error ?? "Impossible de créer le compte";
                HasError = true;
                return;
            }
            await Shell.Current.GoToAsync("//dashboard");
        }
        finally { IsBusy = false; }
    }
}
