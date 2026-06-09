namespace Films.Domain.Entities;

/// <summary>
/// Represents a right/permission entity in the domain
/// </summary>
public class Right
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<UserRight> UserRights { get; set; } = new List<UserRight>();
}
