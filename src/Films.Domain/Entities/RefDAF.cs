namespace Films.Domain.Entities;

/// <summary>
/// Represents the relationship between directors and films
/// </summary>
public class RefDAF
{
    public int Id { get; set; }
    public int DirectorId { get; set; }
    public int FilmId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public DirectedBy? Director { get; set; }
    public Film? Film { get; set; }
}
