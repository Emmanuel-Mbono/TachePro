using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Models;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class TasksViewModel : BaseViewModel
{
    private readonly DataService _data;
    private List<TaskItem> _all = new();
    private List<Member> _members = new();
    private Guid? _memberFilter;
    private Statut? _statusFilter;
    private bool _firstLoad = true;

    public TasksViewModel(DataService data) => _data = data;

    public ObservableCollection<TaskRow> Tasks { get; } = new();
    public ObservableCollection<FilterChip> Chips { get; } = new();

    // États de l'écran : chargement, liste, vide, aucun résultat
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private bool _showList;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private bool _hasNoResults;
    [ObservableProperty] private bool _showFab;
    [ObservableProperty] private string _statusFilterLabel = "Filtrer";

    public async Task LoadAsync()
    {
        // Le squelette de chargement ne s'affiche qu'à la première ouverture
        if (_firstLoad) IsLoading = true;
        UpdateState();

        _all = await _data.GetTasksAsync();
        _members = await _data.GetMembersAsync();

        if (_memberFilter != null && _members.All(m => m.Id != _memberFilter))
            _memberFilter = null;

        BuildChips();
        ApplyFilter();

        _firstLoad = false;
        IsLoading = false;
        UpdateState();
    }

    private void BuildChips()
    {
        Chips.Clear();
        Chips.Add(new FilterChip("Tous", null, _memberFilter == null, SelectMember));
        foreach (var m in _members)
            Chips.Add(new FilterChip(m.Name, m.Id, _memberFilter == m.Id, SelectMember));
    }

    private void SelectMember(Guid? id)
    {
        _memberFilter = id;
        foreach (var chip in Chips) chip.IsSelected = chip.MemberId == id;
        ApplyFilter();
        UpdateState();
    }

    private void ApplyFilter()
    {
        IEnumerable<TaskItem> query = _all;
        if (_memberFilter != null) query = query.Where(t => t.AssigneeId == _memberFilter);
        if (_statusFilter != null) query = query.Where(t => t.Status == _statusFilter);

        Tasks.Clear();
        foreach (var t in query
                     .OrderBy(t => t.Status == Statut.Termine)
                     .ThenBy(t => t.DueDate ?? DateTime.MaxValue))
            Tasks.Add(TaskRow.From(t, _members));
    }

    private void UpdateState()
    {
        IsEmpty = !IsLoading && _all.Count == 0;
        HasNoResults = !IsLoading && _all.Count > 0 && Tasks.Count == 0;
        ShowList = !IsLoading && Tasks.Count > 0;
        ShowFab = !IsLoading;
    }

    [RelayCommand]
    private async Task FilterStatusAsync()
    {
        var choice = await Shell.Current.DisplayActionSheet(
            "Filtrer par statut", "Annuler", null, "Tous les statuts", "À faire", "En cours", "Terminé");

        if (choice is null or "Annuler") return;

        _statusFilter = choice switch
        {
            "À faire" => Statut.AFaire,
            "En cours" => Statut.EnCours,
            "Terminé" => Statut.Termine,
            _ => null
        };
        StatusFilterLabel = _statusFilter == null ? "Filtrer" : choice;
        ApplyFilter();
        UpdateState();
    }

    [RelayCommand]
    private Task AddTaskAsync() => Shell.Current.GoToAsync("taskedit");

    [RelayCommand]
    private async Task LoadDemoAsync()
    {
        await _data.SeedDemoAsync();
        await LoadAsync();
    }
}
