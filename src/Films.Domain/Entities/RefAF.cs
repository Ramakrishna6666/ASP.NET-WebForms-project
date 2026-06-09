namespace Films.Domain.Entities;

/// <summary>
/// Represents an actor-film relationship entity in the domain
/// </summary>
public class RefAF
{
    public int Id { get; set; }
    public int ActorId { get; set; }
    public int FilmId { get; set; }
    public string? Role { get; set; }

    // Navigation properties
    public virtual Actor Actor { get; set; } = null!;
    public virtual Film Film { get; set; } = null!;
}
