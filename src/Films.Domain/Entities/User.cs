namespace Films.Domain.Entities;

/// <summary>
/// Represents a user entity in the domain
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? TypeUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual TypeUser? TypeUser { get; set; }
    public virtual ICollection<UserRight> UserRights { get; set; } = new List<UserRight>();
}
