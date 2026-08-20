namespace Films.Application.DTOs;

public class DirectedByDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? SexId { get; set; }
    public string? SexName { get; set; }
    public string? Nationality { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}

public class DirectedByCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? SexId { get; set; }
    public string? Nationality { get; set; }
}

public class DirectedByUpdateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public int? SexId { get; set; }
    public string? Nationality { get; set; }
}
