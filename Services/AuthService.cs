using System.Security.Cryptography;
using TachePro.Models;

namespace TachePro.Services;

public enum LoginResult { Ok, UnknownEmail, WrongPassword }

public class AuthService
{
    private const string SessionKey = "current_user";
    private readonly DataService _data;

    public AuthService(DataService data) => _data = data;

    public User? CurrentUser { get; private set; }
    public Guid? CurrentMemberId { get; private set; }

    /// <summary>Retrouve la session enregistrée (si l'utilisateur ne s'est pas déconnecté).</summary>
    public async Task<bool> RestoreAsync()
    {
        if (CurrentUser != null) return true;
        if (!Guid.TryParse(Preferences.Default.Get(SessionKey, ""), out var id)) return false;

        var data = await _data.GetAsync();
        CurrentUser = data.Users.FirstOrDefault(u => u.Id == id);
        CurrentMemberId = data.Members.FirstOrDefault(m => m.UserId == id)?.Id;
        return CurrentUser != null;
    }

    public async Task<(bool Ok, string? Error)> RegisterAsync(string name, string email, string password)
    {
        var data = await _data.GetAsync();
        email = email.Trim();

        if (data.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            return (false, "Cet e-mail est déjà utilisé");

        var salt = RandomNumberGenerator.GetBytes(16);
        var user = new User
        {
            Name = name.Trim(),
            Email = email,
            Salt = Convert.ToBase64String(salt),
            PasswordHash = Hash(password, salt)
        };
        var member = new Member { Name = user.Name, Email = user.Email, UserId = user.Id };

        data.Users.Add(user);
        data.Members.Add(member);
        await _data.SaveAsync();

        StartSession(user, member.Id);
        return (true, null);
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var data = await _data.GetAsync();
        var user = data.Users.FirstOrDefault(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        if (user == null) return LoginResult.UnknownEmail;

        var expected = Convert.FromBase64String(user.PasswordHash);
        var actual = Convert.FromBase64String(Hash(password, Convert.FromBase64String(user.Salt)));
        if (!CryptographicOperations.FixedTimeEquals(expected, actual)) return LoginResult.WrongPassword;

        StartSession(user, data.Members.FirstOrDefault(m => m.UserId == user.Id)?.Id);
        return LoginResult.Ok;
    }

    public void Logout()
    {
        CurrentUser = null;
        CurrentMemberId = null;
        Preferences.Default.Remove(SessionKey);
    }

    private void StartSession(User user, Guid? memberId)
    {
        CurrentUser = user;
        CurrentMemberId = memberId;
        Preferences.Default.Set(SessionKey, user.Id.ToString());
    }

    // Mot de passe jamais stocké en clair : PBKDF2 + sel aléatoire
    private static string Hash(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32));
}
