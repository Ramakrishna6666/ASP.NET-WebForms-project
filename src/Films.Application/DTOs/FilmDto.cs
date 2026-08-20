namespace Films.Application.DTOs;

public class FilmDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public int? Duration { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}

public class FilmCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public int? Duration { get; set; }
    public string? Country { get; set; }
}

public class FilmUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public int? Duration { get; set; }
    public string? Country { get; set; }
}
