namespace Films.Application.DTOs;

public class TypeUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}

public class TypeUserCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class TypeUserUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
