using Films.Application.DTOs;
using Films.Application.Services;
using Films.Domain.Entities;
using Films.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Tests.Unit.Services;

public class FilmServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<FilmService>> _loggerMock;
    private readonly FilmService _filmService;

    public FilmServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<FilmService>>();
        _filmService = new FilmService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllFilmsAsync_ShouldReturnAllFilms()
    {
        // Arrange
        var films = new List<Film>
        {
            new Film { Id = 1, Title = "Film 1", Year = 2020 },
            new Film { Id = 2, Title = "Film 2", Year = 2021 }
        };

        var filmRepositoryMock = new Mock<IRepository<Film>>();
        filmRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(films);

        _unitOfWorkMock.Setup(u => u.Films).Returns(filmRepositoryMock.Object);

        // Act
        var result = await _filmService.GetAllFilmsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Title == "Film 1");
        result.Should().Contain(f => f.Title == "Film 2");
    }

    [Fact]
    public async Task GetFilmByIdAsync_ShouldReturnFilm_WhenFilmExists()
    {
        // Arrange
        var film = new Film { Id = 1, Title = "Test Film", Year = 2020 };

        var filmRepositoryMock = new Mock<IRepository<Film>>();
        filmRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(film);

        _unitOfWorkMock.Setup(u => u.Films).Returns(filmRepositoryMock.Object);

        // Act
        var result = await _filmService.GetFilmByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Film");
    }

    [Fact]
    public async Task CreateFilmAsync_ShouldCreateFilm()
    {
        // Arrange
        var createDto = new CreateFilmDto
        {
            Title = "New Film",
            Year = 2024,
            Genre = "Action"
        };

        var filmRepositoryMock = new Mock<IRepository<Film>>();
        filmRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Film f, CancellationToken ct) => { f.Id = 1; return f; });

        _unitOfWorkMock.Setup(u => u.Films).Returns(filmRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _filmService.CreateFilmAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Film");
        filmRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Film>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
