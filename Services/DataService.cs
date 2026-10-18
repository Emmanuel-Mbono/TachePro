using System.Text.Json;
using TachePro.Models;

namespace TachePro.Services;

/// <summary>
/// Stockage local au format JSON (léger, sans base de données ni réseau).
/// Toutes les données restent sur l'appareil.
/// </summary>
public class DataService
{
    private readonly string _path = Path.Combine(FileSystem.AppDataDirectory, "tachepro.json");
    private readonly SemaphoreSlim _gate = new(1, 1);
    private AppData? _data;

    public async Task<AppData> GetAsync()
    {
        if (_data != null) return _data;

        await _gate.WaitAsync();
        try
        {
            if (_data == null)
            {
                try
                {
                    _data = File.Exists(_path)
                        ? JsonSerializer.Deserialize<AppData>(await File.ReadAllTextAsync(_path)) ?? new AppData()
                        : new AppData();
                }
                catch
                {
                    _data = new AppData(); // fichier illisible : on repart d'une base vide
                }
            }
        }
        finally { _gate.Release(); }

        return _data;
    }

    public async Task SaveAsync()
    {
        var data = await GetAsync();
        await File.WriteAllTextAsync(_path, JsonSerializer.Serialize(data));
    }

    // ---------- Tâches ----------
    public async Task<List<TaskItem>> GetTasksAsync() => (await GetAsync()).Tasks.ToList();

    public async Task<TaskItem?> GetTaskAsync(Guid id) =>
        (await GetAsync()).Tasks.FirstOrDefault(t => t.Id == id);

    public async Task SaveTaskAsync(TaskItem task)
    {
        var data = await GetAsync();
        var index = data.Tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0) data.Tasks[index] = task; else data.Tasks.Add(task);
        await SaveAsync();
    }

    public async Task DeleteTaskAsync(Guid id)
    {
        var data = await GetAsync();
        data.Tasks.RemoveAll(t => t.Id == id);
        await SaveAsync();
    }

    // ---------- Membres ----------
    public async Task<List<Member>> GetMembersAsync() => (await GetAsync()).Members.ToList();

    public async Task AddMemberAsync(Member member)
    {
        (await GetAsync()).Members.Add(member);
        await SaveAsync();
    }

    public async Task RemoveMemberAsync(Guid memberId)
    {
        var data = await GetAsync();
        data.Members.RemoveAll(m => m.Id == memberId);
        foreach (var task in data.Tasks.Where(t => t.AssigneeId == memberId))
            task.AssigneeId = null; // les tâches du membre retiré deviennent non assignées
        await SaveAsync();
    }

    // ---------- Données d'exemple (démonstration) ----------
    public async Task SeedDemoAsync()
    {
        var data = await GetAsync();

        Member GetOrAdd(string name)
        {
            var member = data.Members.FirstOrDefault(m => m.Name == name);
            if (member == null)
            {
                member = new Member { Name = name };
                data.Members.Add(member);
            }
            return member;
        }

        var awa = GetOrAdd("Awa");
        var marc = GetOrAdd("Marc");
        var lea = GetOrAdd("Léa");
        var today = DateTime.Today;

        void Add(string title, string desc, Member who, int days, Statut status) =>
            data.Tasks.Add(new TaskItem
            {
                Title = title, Description = desc, AssigneeId = who.Id,
                DueDate = today.AddDays(days), Status = status
            });

        Add("Rédiger le cahier des charges", "Décrire les besoins et les fonctionnalités attendues.", awa, 3, Statut.EnCours);
        Add("Maquette API", "Concevoir les 8 écrans et les valider avec l'équipe avant vendredi.", marc, 1, Statut.AFaire);
        Add("Base de données", "Modéliser les tables et écrire le script de création.", awa, -2, Statut.Termine);
        Add("Page de connexion", "Intégrer la maquette et la validation des champs.", lea, 2, Statut.EnCours);
        Add("Rapport de test", "Lister les cas de test et consigner les résultats.", awa, 5, Statut.AFaire);
        Add("Revue de code", "Relire les derniers commits de l'équipe.", marc, 7, Statut.AFaire);
        Add("Documentation", "Rédiger le guide d'installation.", lea, -1, Statut.Termine);
        Add("Déploiement", "Préparer la version de démonstration.", lea, 10, Statut.AFaire);

        await SaveAsync();
    }
}
