using Films.Application.DTOs;
using Films.Application.Interfaces;
using Films.Application.Services;
using Films.Domain.Entities;
using Films.Domain.Interfaces.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.UnitTests.Services;

public class FilmServiceTests
{
    private readonly Mock<IFilmRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<FilmService>> _mockLogger;
    private readonly IFilmService _service;

    public FilmServiceTests()
    {
        _mockRepository = new Mock<IFilmRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<FilmService>>();
        _service = new FilmService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFilms()
    {
        // Arrange
        var films = new List<Film>
        {
            new Film { Id = 1, Title = "Film 1", IsActive = true },
            new Film { Id = 2, Title = "Film 2", IsActive = true }
        };
        var filmDtos = new List<FilmDto>
        {
            new FilmDto { Id = 1, Title = "Film 1" },
            new FilmDto { Id = 2, Title = "Film 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(films);
        _mockMapper.Setup(m => m.Map<IEnumerable<FilmDto>>(films))
            .Returns(filmDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(filmDtos);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFilm_WhenFilmExists()
    {
        // Arrange
        var film = new Film { Id = 1, Title = "Film 1", IsActive = true };
        var filmDto = new FilmDto { Id = 1, Title = "Film 1" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(film);
        _mockMapper.Setup(m => m.Map<FilmDto>(film))
            .Returns(filmDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(filmDto);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateFilm()
    {
        // Arrange
        var createDto = new FilmCreateDto { Title = "New Film" };
        var film = new Film { Id = 1, Title = "New Film", IsActive = true };
        var filmDto = new FilmDto { Id = 1, Title = "New Film" };

        _mockMapper.Setup(m => m.Map<Film>(createDto))
            .Returns(film);
        _mockRepository.Setup(r => r.AddAsync(film, It.IsAny<CancellationToken>()))
            .ReturnsAsync(film);
        _mockMapper.Setup(m => m.Map<FilmDto>(film))
            .Returns(filmDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Film");
    }
}
