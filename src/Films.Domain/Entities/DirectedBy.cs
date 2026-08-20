namespace Films.Domain.Entities;

/// <summary>
/// Represents a director in the database
/// </summary>
public class DirectedBy
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? SexId { get; set; }
    public string? Nationality { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public Sex? Sex { get; set; }
    public ICollection<RefDAF> RefDAFs { get; set; } = new List<RefDAF>();
}
