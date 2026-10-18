namespace TachePro.Models;

public enum Statut { AFaire, EnCours, Termine }

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Salt { get; set; } = "";
}

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    /// <summary>Renseigné si le membre correspond à un compte utilisateur.</summary>
    public Guid? UserId { get; set; }

    // Utilisé par le Picker pour afficher le nom
    public override string ToString() => Name;
}

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public Statut Status { get; set; } = Statut.AFaire;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class AppData
{
    public List<User> Users { get; set; } = new();
    public List<Member> Members { get; set; } = new();
    public List<TaskItem> Tasks { get; set; } = new();
}
