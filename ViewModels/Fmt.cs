using TachePro.Models;

namespace TachePro.ViewModels;

/// <summary>Petites fonctions d'affichage partagées par les écrans.</summary>
public static class Fmt
{
    public static string StatusLabel(Statut s) => s switch
    {
        Statut.AFaire => "À faire",
        Statut.EnCours => "En cours",
        _ => "Terminé"
    };

    public static Color BadgeBackground(Statut s) => s switch
    {
        Statut.AFaire => Color.FromArgb("#FBEAE8"),
        Statut.EnCours => Color.FromArgb("#FBF1DC"),
        _ => Color.FromArgb("#E3F1E4")
    };

    public static Color BadgeText(Statut s) => s switch
    {
        Statut.AFaire => Color.FromArgb("#C0392B"),
        Statut.EnCours => Color.FromArgb("#B7791F"),
        _ => Color.FromArgb("#2E7D32")
    };

    public static string DueText(DateTime? due)
    {
        if (due == null) return "Sans échéance";
        var days = (due.Value.Date - DateTime.Today).Days;
        if (days == 0) return "Aujourd'hui";
        if (days == 1) return "Demain";
        return due.Value.Year == DateTime.Today.Year
            ? due.Value.ToString("dd/MM")
            : due.Value.ToString("dd/MM/yyyy");
    }

    public static bool IsOverdue(TaskItem t) =>
        t.Status != Statut.Termine && t.DueDate != null && t.DueDate.Value.Date < DateTime.Today;

    public static string Initial(string name) =>
        string.IsNullOrWhiteSpace(name) ? "?" : name.Trim()[0].ToString().ToUpperInvariant();

    public static string Initials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "?";
        if (parts.Length == 1) return Initial(parts[0]);
        return (Initial(parts[0]) + Initial(parts[^1]));
    }

    public static string Plural(int n, string singular, string plural) => n > 1 ? plural : singular;
}
