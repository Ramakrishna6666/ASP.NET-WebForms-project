namespace Films.Application.DTOs;

public class SexDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}

public class SexCreateDto
{
    public string Name { get; set; } = string.Empty;
}

public class SexUpdateDto
{
    public string Name { get; set; } = string.Empty;
}
