using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Models;
using TachePro.Services;

namespace TachePro.ViewModels;

/// <summary>Création et modification d'une tâche (le paramètre « id » active le mode modification).</summary>
[QueryProperty(nameof(TaskId), "id")]
public partial class TaskEditViewModel : BaseViewModel
{
    private readonly DataService _data;
    private readonly AuthService _auth;
    private TaskItem? _editing;
    private bool _loaded;

    public TaskEditViewModel(DataService data, AuthService auth)
    {
        _data = data;
        _auth = auth;
    }

    [ObservableProperty] private string? _taskId;
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _errorMessage = "";
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _pageTitle = "Nouvelle tâche";
    [ObservableProperty] private string _saveLabel = "Créer la tâche";
    [ObservableProperty] private Member? _selectedMember;
    [ObservableProperty] private bool _hasDueDate = true;
    [ObservableProperty] private DateTime _dueDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTodo), nameof(IsDoing), nameof(IsDone))]
    private Statut _status = Statut.AFaire;

    public bool IsTodo => Status == Statut.AFaire;
    public bool IsDoing => Status == Statut.EnCours;
    public bool IsDone => Status == Statut.Termine;

    public ObservableCollection<Member> Members { get; } = new();

    public async Task LoadAsync()
    {
        if (_loaded) return;
        _loaded = true;

        Members.Clear();
        foreach (var m in await _data.GetMembersAsync()) Members.Add(m);

        if (Guid.TryParse(TaskId, out var id) && await _data.GetTaskAsync(id) is { } task)
        {
            _editing = task;
            PageTitle = "Modifier la tâche";
            SaveLabel = "Enregistrer";
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
            HasDueDate = task.DueDate != null;
            if (task.DueDate != null) DueDate = task.DueDate.Value;
            SelectedMember = Members.FirstOrDefault(m => m.Id == task.AssigneeId);
        }
        else
        {
            // Nouvelle tâche : assignée par défaut à l'utilisateur connecté
            SelectedMember = Members.FirstOrDefault(m => m.Id == _auth.CurrentMemberId);
        }
    }

    [RelayCommand]
    private void SetStatus(string value) => Status = Enum.Parse<Statut>(value);

    [RelayCommand]
    private async Task SaveAsync()
    {
        HasError = false;
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Le titre est obligatoire";
            HasError = true;
            return;
        }

        var task = _editing ?? new TaskItem();
        task.Title = Title.Trim();
        task.Description = Description?.Trim() ?? "";
        task.AssigneeId = SelectedMember?.Id;
        task.DueDate = HasDueDate ? DueDate.Date : null;
        task.Status = Status;

        await _data.SaveTaskAsync(task);
        await Shell.Current.GoToAsync("..");
    }
}
