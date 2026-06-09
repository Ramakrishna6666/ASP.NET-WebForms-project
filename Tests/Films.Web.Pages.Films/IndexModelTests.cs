using Films.Application.DTOs;
using Films.Application.Services;
using Films.Web.Pages.Films;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests.Pages.Films;

public class IndexModelTests
{
    private readonly Mock<IFilmService> _mockFilmService;
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _indexModel;

    public IndexModelTests()
    {
        _mockFilmService = new Mock<IFilmService>();
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _indexModel = new IndexModel(_mockFilmService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var model = new IndexModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullFilmService_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IndexModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IndexModel(_mockFilmService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithNoSearchTerm_RetrievesAllFilms()
    {
        // Arrange
        var expectedFilms = new List<FilmDto>
        {
            new FilmDto { Id = 1, Title = "Film 1", Year = 2020 },
            new FilmDto { Id = 2, Title = "Film 2", Year = 2021 }
        };
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = null;

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.Films);
        Assert.Equal(2, _indexModel.Films.Count());
        _mockFilmService.Verify(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithSearchTerm_SearchesFilms()
    {
        // Arrange
        var searchTerm = "Action";
        var expectedFilms = new List<FilmDto>
        {
            new FilmDto { Id = 1, Title = "Action Film", Year = 2020 }
        };
        _mockFilmService.Setup(s => s.SearchFilmsAsync(searchTerm)).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = searchTerm;

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.Films);
        Assert.Single(_indexModel.Films);
        _mockFilmService.Verify(s => s.SearchFilmsAsync(searchTerm), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithEmptySearchTerm_RetrievesAllFilms()
    {
        // Arrange
        var expectedFilms = new List<FilmDto>
        {
            new FilmDto { Id = 1, Title = "Film 1", Year = 2020 }
        };
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = "";

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.Films);
        _mockFilmService.Verify(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithWhitespaceSearchTerm_RetrievesAllFilms()
    {
        // Arrange
        var expectedFilms = new List<FilmDto>
        {
            new FilmDto { Id = 1, Title = "Film 1", Year = 2020 }
        };
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = "   ";

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.Films);
        _mockFilmService.Verify(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_SetsEmptyFilmsList()
    {
        // Arrange
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
        _indexModel.SearchTerm = null;

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        Assert.NotNull(_indexModel.Films);
        Assert.Empty(_indexModel.Films);
    }

    [Fact]
    public async Task OnGetAsync_WhenServiceThrowsException_LogsError()
    {
        // Arrange
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));
        _indexModel.SearchTerm = null;

        // Act
        await _indexModel.OnGetAsync();

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
    public async Task OnGetAsync_WithSearchTerm_LogsInformation()
    {
        // Arrange
        var searchTerm = "Action";
        var expectedFilms = new List<FilmDto>();
        _mockFilmService.Setup(s => s.SearchFilmsAsync(searchTerm)).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = searchTerm;

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Searched films")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithoutSearchTerm_LogsInformation()
    {
        // Arrange
        var expectedFilms = new List<FilmDto>();
        _mockFilmService.Setup(s => s.GetAllFilmsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilms);
        _indexModel.SearchTerm = null;

        // Act
        await _indexModel.OnGetAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved all films")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Films_DefaultValue_IsEmptyList()
    {
        // Arrange & Act
        var model = new IndexModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model.Films);
        Assert.Empty(model.Films);
    }

    [Fact]
    public void SearchTerm_CanBeSet()
    {
        // Arrange
        var searchTerm = "Test";

        // Act
        _indexModel.SearchTerm = searchTerm;

        // Assert
        Assert.Equal(searchTerm, _indexModel.SearchTerm);
    }

    [Fact]
    public void SearchTerm_CanBeNull()
    {
        // Arrange & Act
        _indexModel.SearchTerm = null;

        // Assert
        Assert.Null(_indexModel.SearchTerm);
    }
}
