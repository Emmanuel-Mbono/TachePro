using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly AuthService _auth;

    public LoginViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _hidePassword = true;
    [ObservableProperty] private bool _isBusy;

    public string ShowHideLabel => HidePassword ? "Voir" : "Masquer";

    partial void OnHidePasswordChanged(bool value) => OnPropertyChanged(nameof(ShowHideLabel));

    /// <summary>Appelé à l'ouverture de l'écran : saute la connexion si la session existe déjà.</summary>
    public async Task<bool> TryRestoreSessionAsync() => await _auth.RestoreAsync();

    public void Reset()
    {
        Email = "";
        Password = "";
        HasError = false;
        ErrorMessage = "";
        HidePassword = true;
    }

    [RelayCommand]
    private void TogglePassword() => HidePassword = !HidePassword;

    [RelayCommand]
    private async Task LoginAsync()
    {
        HasError = false;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Saisissez votre e-mail et votre mot de passe";
            HasError = true;
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _auth.LoginAsync(Email, Password);
            switch (result)
            {
                case LoginResult.Ok:
                    await Shell.Current.GoToAsync("//dashboard");
                    break;
                case LoginResult.UnknownEmail:
                    ErrorMessage = "Aucun compte avec cet e-mail";
                    HasError = true;
                    break;
                default:
                    ErrorMessage = "Mot de passe incorrect";
                    HasError = true;
                    break;
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task GoToRegisterAsync() => Shell.Current.GoToAsync("register");

    [RelayCommand]
    private Task ForgotPasswordAsync() =>
        AlertAsync("Mot de passe oublié", "L'application fonctionne hors ligne : la réinitialisation par e-mail n'est pas disponible. Créez un nouveau compte si nécessaire.");
}
