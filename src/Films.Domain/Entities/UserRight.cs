namespace Films.Domain.Entities;

/// <summary>
/// Represents a user-right relationship entity in the domain
/// </summary>
public class UserRight
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RightId { get; set; }
    public DateTime GrantedDate { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Right Right { get; set; } = null!;
}
