using TachePro.Services;
using TachePro.ViewModels;
using TachePro.Views;

namespace TachePro;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Services (une seule instance pour toute l'application)
        builder.Services.AddSingleton<DataService>();
        builder.Services.AddSingleton<AuthService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<TaskEditViewModel>();
        builder.Services.AddTransient<TaskDetailViewModel>();
        builder.Services.AddTransient<TeamViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<TaskEditPage>();
        builder.Services.AddTransient<TaskDetailPage>();
        builder.Services.AddTransient<TeamPage>();
        builder.Services.AddTransient<ProfilePage>();

        return builder.Build();
    }
}
