using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Models;
using TachePro.Services;

namespace TachePro.ViewModels;

[QueryProperty(nameof(TaskId), "id")]
public partial class TaskDetailViewModel : BaseViewModel
{
    private readonly DataService _data;
    private TaskItem? _task;

    public TaskDetailViewModel(DataService data) => _data = data;

    [ObservableProperty] private string? _taskId;
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _assigneeName = "Non assignée";
    [ObservableProperty] private string _dueText = "";
    [ObservableProperty] private bool _isOverdue;
    [ObservableProperty] private string _statusLabel = "";
    [ObservableProperty] private Color _badgeBackground = Colors.Transparent;
    [ObservableProperty] private Color _badgeText = Colors.Black;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTodo), nameof(IsDoing), nameof(IsDone), nameof(PrimaryActionLabel))]
    private Statut _status = Statut.AFaire;

    public bool IsTodo => Status == Statut.AFaire;
    public bool IsDoing => Status == Statut.EnCours;
    public bool IsDone => Status == Statut.Termine;
    public string PrimaryActionLabel => IsDone ? "Rouvrir la tâche" : "Marquer comme terminée";

    partial void OnTaskIdChanged(string? value) => _ = LoadAsync();

    public async Task LoadAsync()
    {
        if (!Guid.TryParse(TaskId, out var id)) return;

        _task = await _data.GetTaskAsync(id);
        if (_task == null) return;

        var members = await _data.GetMembersAsync();
        Title = _task.Title;
        Description = string.IsNullOrWhiteSpace(_task.Description) ? "Aucune description." : _task.Description;
        AssigneeName = members.FirstOrDefault(m => m.Id == _task.AssigneeId)?.Name ?? "Non assignée";
        DueText = Fmt.DueText(_task.DueDate);
        IsOverdue = Fmt.IsOverdue(_task);
        Status = _task.Status;
        StatusLabel = Fmt.StatusLabel(_task.Status);
        BadgeBackground = Fmt.BadgeBackground(_task.Status);
        BadgeText = Fmt.BadgeText(_task.Status);
    }

    [RelayCommand]
    private async Task SetStatusAsync(string value)
    {
        if (_task == null) return;
        _task.Status = Enum.Parse<Statut>(value);
        await _data.SaveTaskAsync(_task);
        await LoadAsync();
    }

    [RelayCommand]
    private Task TogglePrimaryAsync() =>
        SetStatusAsync(IsDone ? nameof(Statut.EnCours) : nameof(Statut.Termine));

    [RelayCommand]
    private async Task ReassignAsync()
    {
        if (_task == null) return;

        var members = await _data.GetMembersAsync();
        var names = members.Select(m => m.Name).ToArray();
        var choice = await Shell.Current.DisplayActionSheet("Réassigner à", "Annuler", "Non assignée", names);
        if (choice is null or "Annuler") return;

        _task.AssigneeId = choice == "Non assignée" ? null : members.FirstOrDefault(m => m.Name == choice)?.Id;
        await _data.SaveTaskAsync(_task);
        await LoadAsync();
    }

    [RelayCommand]
    private Task EditAsync() => Shell.Current.GoToAsync($"taskedit?id={TaskId}");

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (_task == null) return;
        var ok = await Shell.Current.DisplayAlert("Supprimer la tâche", "Cette action est définitive.", "Supprimer", "Annuler");
        if (!ok) return;

        await _data.DeleteTaskAsync(_task.Id);
        await Shell.Current.GoToAsync("..");
    }
}
