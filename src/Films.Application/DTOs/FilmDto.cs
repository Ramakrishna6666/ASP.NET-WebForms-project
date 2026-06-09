namespace Films.Application.DTOs;

/// <summary>
/// Data Transfer Object for Film entity
/// </summary>
public class FilmDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

/// <summary>
/// DTO for creating a new film
/// </summary>
public class CreateFilmDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
}

/// <summary>
/// DTO for updating an existing film
/// </summary>
public class UpdateFilmDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
}
