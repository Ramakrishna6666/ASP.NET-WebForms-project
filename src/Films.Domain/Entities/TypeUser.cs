namespace Films.Domain.Entities;

/// <summary>
/// Represents a type of user (e.g., Admin, Regular User)
/// </summary>
public class TypeUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}
