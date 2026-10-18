namespace TachePro;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Thème choisi par l'utilisateur (sinon, celui du système)
        UserAppTheme = Preferences.Default.Get("theme", "system") switch
        {
            "dark" => AppTheme.Dark,
            "light" => AppTheme.Light,
            _ => AppTheme.Unspecified
        };
    }

    protected override Window CreateWindow(IActivationState? activationState) => new Window(new AppShell());
}
