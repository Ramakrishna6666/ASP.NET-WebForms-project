namespace Films.Domain.Entities;

/// <summary>
/// Represents a director-film relationship entity in the domain
/// </summary>
public class RefDAF
{
    public int Id { get; set; }
    public int DirectorId { get; set; }
    public int FilmId { get; set; }
    public DateTime CreatedDate { get; set; }

    // Navigation properties
    public virtual Director Director { get; set; } = null!;
    public virtual Film Film { get; set; } = null!;
}
