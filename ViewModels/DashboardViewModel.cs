using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TachePro.Models;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly DataService _data;

    public DashboardViewModel(DataService data) => _data = data;

    [ObservableProperty] private int _todoCount;
    [ObservableProperty] private int _doingCount;
    [ObservableProperty] private int _doneCount;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _progressText = "0 %";
    [ObservableProperty] private string _summary = "";
    [ObservableProperty] private bool _hasUpcoming;

    public ObservableCollection<TaskRow> Upcoming { get; } = new();

    public async Task LoadAsync()
    {
        var tasks = await _data.GetTasksAsync();
        var members = await _data.GetMembersAsync();

        TodoCount = tasks.Count(t => t.Status == Statut.AFaire);
        DoingCount = tasks.Count(t => t.Status == Statut.EnCours);
        DoneCount = tasks.Count(t => t.Status == Statut.Termine);

        var total = tasks.Count;
        Progress = total == 0 ? 0 : (double)DoneCount / total;
        ProgressText = $"{Math.Round(Progress * 100)} %";
        Summary = total == 0
            ? "Aucune tâche pour l'instant"
            : $"{DoneCount} {Fmt.Plural(DoneCount, "tâche", "tâches")} sur {total} {Fmt.Plural(total, "terminée", "terminées")}";

        Upcoming.Clear();
        foreach (var t in tasks
                     .Where(t => t.Status != Statut.Termine && t.DueDate != null)
                     .OrderBy(t => t.DueDate)
                     .Take(3))
            Upcoming.Add(TaskRow.From(t, members));

        HasUpcoming = Upcoming.Count > 0;
    }
}
