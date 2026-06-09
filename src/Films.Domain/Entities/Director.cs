namespace Films.Domain.Entities;

/// <summary>
/// Represents a director entity in the domain
/// </summary>
public class Director
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? SexId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public virtual Sex? Sex { get; set; }
    public virtual ICollection<RefDAF> DirectorFilms { get; set; } = new List<RefDAF>();
}
