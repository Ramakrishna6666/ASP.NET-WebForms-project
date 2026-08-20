namespace Films.Domain.Entities;

/// <summary>
/// Represents the relationship between actors and films
/// </summary>
public class RefAF
{
    public int Id { get; set; }
    public int ActorId { get; set; }
    public int FilmId { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Actor? Actor { get; set; }
    public Film? Film { get; set; }
}
