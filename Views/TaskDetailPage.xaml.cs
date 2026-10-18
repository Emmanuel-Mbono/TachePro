using TachePro.ViewModels;

namespace TachePro.Views;

public partial class TaskDetailPage : ContentPage
{
    private readonly TaskDetailViewModel _vm;

    public TaskDetailPage(TaskDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Recharge les informations au retour de l'écran de modification
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
