namespace Films.Domain.Entities;

/// <summary>
/// Represents a sex/gender entity in the domain
/// </summary>
public class Sex
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();
    public virtual ICollection<Director> Directors { get; set; } = new List<Director>();
}
