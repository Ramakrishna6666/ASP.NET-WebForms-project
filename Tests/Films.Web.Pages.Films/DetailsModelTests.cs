using Films.Application.DTOs;
using Films.Application.Services;
using Films.Web.Pages.Films;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests.Pages.Films;

public class DetailsModelTests
{
    private readonly Mock<IFilmService> _mockFilmService;
    private readonly Mock<ILogger<DetailsModel>> _mockLogger;
    private readonly DetailsModel _detailsModel;

    public DetailsModelTests()
    {
        _mockFilmService = new Mock<IFilmService>();
        _mockLogger = new Mock<ILogger<DetailsModel>>();
        _detailsModel = new DetailsModel(_mockFilmService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var model = new DetailsModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullFilmService_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DetailsModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DetailsModel(_mockFilmService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_ReturnsPageResult()
    {
        // Arrange
        var filmId = 1;
        var expectedFilm = new FilmDto 
        { 
            Id = filmId, 
            Title = "Test Film", 
            Year = 2020,
            Description = "Test Description",
            Genre = "Action"
        };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilm);

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_detailsModel.Film);
        Assert.Equal(filmId, _detailsModel.Film.Id);
        Assert.Equal("Test Film", _detailsModel.Film.Title);
    }

    [Fact]
    public async Task OnGetAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var filmId = 999;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_RedirectsToIndex()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_LogsError()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        await _detailsModel.OnGetAsync(filmId);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_CallsServiceOnce()
    {
        // Arrange
        var filmId = 1;
        var expectedFilm = new FilmDto { Id = filmId, Title = "Test Film", Year = 2020 };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilm);

        // Act
        await _detailsModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithZeroId_CallsService()
    {
        // Arrange
        var filmId = 0;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithNegativeId_CallsService()
    {
        // Arrange
        var filmId = -1;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Film_DefaultValue_IsNull()
    {
        // Arrange & Act
        var model = new DetailsModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.Film);
    }

    [Fact]
    public async Task OnGetAsync_SetsFilmProperty()
    {
        // Arrange
        var filmId = 1;
        var expectedFilm = new FilmDto 
        { 
            Id = filmId, 
            Title = "Test Film", 
            Year = 2020,
            Description = "Description",
            Genre = "Drama"
        };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilm);

        // Act
        await _detailsModel.OnGetAsync(filmId);

        // Assert
        Assert.NotNull(_detailsModel.Film);
        Assert.Equal(expectedFilm.Id, _detailsModel.Film.Id);
        Assert.Equal(expectedFilm.Title, _detailsModel.Film.Title);
        Assert.Equal(expectedFilm.Year, _detailsModel.Film.Year);
        Assert.Equal(expectedFilm.Description, _detailsModel.Film.Description);
        Assert.Equal(expectedFilm.Genre, _detailsModel.Film.Genre);
    }

    [Fact]
    public async Task OnGetAsync_WithLargeId_CallsService()
    {
        // Arrange
        var filmId = int.MaxValue;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _detailsModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
