using TachePro.ViewModels;

namespace TachePro.Views;

public partial class TeamPage : ContentPage
{
    private readonly TeamViewModel _vm;

    public TeamPage(TeamViewModel vm)
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
