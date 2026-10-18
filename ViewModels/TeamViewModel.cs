using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TachePro.Models;
using TachePro.Services;

namespace TachePro.ViewModels;

public partial class TeamViewModel : BaseViewModel
{
    private readonly DataService _data;
    private readonly AuthService _auth;

    public TeamViewModel(DataService data, AuthService auth)
    {
        _data = data;
        _auth = auth;
    }

    public ObservableCollection<MemberRow> Members { get; } = new();

    [ObservableProperty] private bool _hasMembers;

    public async Task LoadAsync()
    {
        await _auth.RestoreAsync();
        var members = await _data.GetMembersAsync();
        var tasks = await _data.GetTasksAsync();

        Members.Clear();
        foreach (var m in members)
        {
            var mine = tasks.Where(t => t.AssigneeId == m.Id).ToList();
            var done = mine.Count(t => t.Status == Statut.Termine);
            var percent = mine.Count == 0 ? 0 : (int)Math.Round(100.0 * done / mine.Count);
            var member = m;
            var isCurrent = m.Id == _auth.CurrentMemberId;
            Members.Add(new MemberRow
            {
                Id = m.Id,
                Name = m.Name,
                Initial = Fmt.Initial(m.Name),
                Summary = $"{mine.Count} {Fmt.Plural(mine.Count, "tâche", "tâches")} · {done} {Fmt.Plural(done, "terminée", "terminées")}",
                PercentText = $"{percent} %",
                IsCurrentUser = isCurrent,
                ManageCommand = new AsyncRelayCommand(() => ManageAsync(member, isCurrent))
            });
        }
        HasMembers = Members.Count > 0;
    }

    private async Task ManageAsync(Member member, bool isCurrentUser)
    {
        if (isCurrentUser)
        {
            await AlertAsync("Votre profil", "Vous ne pouvez pas vous retirer de l'équipe.");
            return;
        }

        var ok = await Shell.Current.DisplayAlert(
            member.Name, "Retirer ce membre de l'équipe ? Ses tâches deviendront non assignées.", "Retirer", "Annuler");
        if (!ok) return;

        await _data.RemoveMemberAsync(member.Id);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task InviteAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "Inviter un membre", "Nom du membre", "Ajouter", "Annuler", "Prénom Nom");
        if (string.IsNullOrWhiteSpace(name)) return;

        var email = await Shell.Current.DisplayPromptAsync(
            "Inviter un membre", "Adresse e-mail (facultatif)", "Ajouter", "Annuler", "name@company.com",
            keyboard: Keyboard.Email);

        await _data.AddMemberAsync(new Member { Name = name.Trim(), Email = email?.Trim() ?? "" });
        await LoadAsync();
    }
}
