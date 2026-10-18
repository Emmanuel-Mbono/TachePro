using TachePro.ViewModels;

namespace TachePro.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;

    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Session déjà ouverte : on passe directement au tableau d'avancement
        if (await _vm.TryRestoreSessionAsync())
            await Shell.Current.GoToAsync("//dashboard");
        else
            _vm.Reset();
    }
}
