using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly AuthService _auth;

    public ProfileViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private string _initials = "?";
    [ObservableProperty] private bool _notificationsEnabled = true;
    [ObservableProperty] private bool _darkMode;

    public string NotificationsLabel => NotificationsEnabled ? "Activées" : "Désactivées";
    public string DarkModeLabel => DarkMode ? "Activé" : "Désactivé";

    public async Task LoadAsync()
    {
        await _auth.RestoreAsync();
        Name = _auth.CurrentUser?.Name ?? "";
        Email = _auth.CurrentUser?.Email ?? "";
        Initials = Fmt.Initials(Name);

        // On affecte les champs directement pour ne pas relancer l'enregistrement des préférences
        _notificationsEnabled = Preferences.Default.Get("notifications", true);
        _darkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        OnPropertyChanged(nameof(NotificationsEnabled));
        OnPropertyChanged(nameof(NotificationsLabel));
        OnPropertyChanged(nameof(DarkMode));
        OnPropertyChanged(nameof(DarkModeLabel));
    }

    partial void OnNotificationsEnabledChanged(bool value)
    {
        Preferences.Default.Set("notifications", value);
        OnPropertyChanged(nameof(NotificationsLabel));
    }

    partial void OnDarkModeChanged(bool value)
    {
        Preferences.Default.Set("theme", value ? "dark" : "light");
        if (Application.Current != null)
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        OnPropertyChanged(nameof(DarkModeLabel));
    }

    [RelayCommand]
    private Task LanguageAsync() =>
        AlertAsync("Langue", "Seul le français est disponible dans cette version.");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var ok = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vous déconnecter ?", "Se déconnecter", "Annuler");
        if (!ok) return;

        _auth.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
