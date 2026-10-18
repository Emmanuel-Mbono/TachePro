using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Models;

namespace TachePro.ViewModels;

/// <summary>Ligne de tâche affichée dans les listes (tableau d'avancement et liste des tâches).</summary>
public class TaskRow
{
    public Guid Id { get; init; }
    public string Title { get; init; } = "";
    public string Meta { get; init; } = "";
    public string StatusLabel { get; init; } = "";
    public Color BadgeBackground { get; init; } = Colors.Transparent;
    public Color BadgeText { get; init; } = Colors.Black;
    public bool IsOverdue { get; init; }
    public IRelayCommand OpenCommand { get; init; } = null!;

    public static TaskRow From(TaskItem t, IEnumerable<Member> members)
    {
        var who = members.FirstOrDefault(m => m.Id == t.AssigneeId)?.Name ?? "Non assignée";
        var id = t.Id;
        return new TaskRow
        {
            Id = id,
            Title = t.Title,
            Meta = $"{who} · {Fmt.DueText(t.DueDate)}",
            StatusLabel = Fmt.StatusLabel(t.Status),
            BadgeBackground = Fmt.BadgeBackground(t.Status),
            BadgeText = Fmt.BadgeText(t.Status),
            IsOverdue = Fmt.IsOverdue(t),
            OpenCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync($"taskdetail?id={id}"))
        };
    }
}

/// <summary>Bouton de filtre par membre (écran « Mes tâches »).</summary>
public partial class FilterChip : ObservableObject
{
    public Guid? MemberId { get; }
    public string Name { get; }
    public IRelayCommand SelectCommand { get; }

    [ObservableProperty]
    private bool _isSelected;

    public FilterChip(string name, Guid? memberId, bool selected, Action<Guid?> onSelect)
    {
        Name = name;
        MemberId = memberId;
        _isSelected = selected;
        SelectCommand = new RelayCommand(() => onSelect(memberId));
    }
}

/// <summary>Ligne de membre (écran « Équipe »).</summary>
public class MemberRow
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Initial { get; init; } = "?";
    public string Summary { get; init; } = "";
    public string PercentText { get; init; } = "";
    public bool IsCurrentUser { get; init; }
    public IRelayCommand ManageCommand { get; init; } = null!;
}
