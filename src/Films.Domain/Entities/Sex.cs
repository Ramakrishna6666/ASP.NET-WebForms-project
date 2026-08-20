namespace Films.Domain.Entities;

/// <summary>
/// Represents gender/sex
/// </summary>
public class Sex
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public ICollection<DirectedBy> Directors { get; set; } = new List<DirectedBy>();
}
