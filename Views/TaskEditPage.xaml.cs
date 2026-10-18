using TachePro.ViewModels;

namespace TachePro.Views;

public partial class TaskEditPage : ContentPage
{
    private readonly TaskEditViewModel _vm;

    public TaskEditPage(TaskEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
