using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TachePro.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private Task GoBackAsync() => Shell.Current.GoToAsync("..");

    protected static Task AlertAsync(string title, string message) =>
        Shell.Current.DisplayAlert(title, message, "OK");
}
