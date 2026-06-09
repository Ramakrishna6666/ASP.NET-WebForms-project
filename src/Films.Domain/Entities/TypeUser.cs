namespace Films.Domain.Entities;

/// <summary>
/// Represents a user type entity in the domain
/// </summary>
public class TypeUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
