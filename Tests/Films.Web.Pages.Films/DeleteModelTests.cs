using Films.Application.DTOs;
using Films.Application.Services;
using Films.Web.Pages.Films;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests.Pages.Films;

public class DeleteModelTests
{
    private readonly Mock<IFilmService> _mockFilmService;
    private readonly Mock<ILogger<DeleteModel>> _mockLogger;
    private readonly DeleteModel _deleteModel;

    public DeleteModelTests()
    {
        _mockFilmService = new Mock<IFilmService>();
        _mockLogger = new Mock<ILogger<DeleteModel>>();
        _deleteModel = new DeleteModel(_mockFilmService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var model = new DeleteModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullFilmService_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DeleteModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DeleteModel(_mockFilmService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_ReturnsPageResult()
    {
        // Arrange
        var filmId = 1;
        var expectedFilm = new FilmDto { Id = filmId, Title = "Test Film", Year = 2020 };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedFilm);

        // Act
        var result = await _deleteModel.OnGetAsync(filmId);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_deleteModel.Film);
        Assert.Equal(filmId, _deleteModel.Film.Id);
    }

    [Fact]
    public async Task OnGetAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var filmId = 999;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _deleteModel.OnGetAsync(filmId);

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
        var result = await _deleteModel.OnGetAsync(filmId);

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
        await _deleteModel.OnGetAsync(filmId);

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
    public async Task OnPostAsync_WithValidId_DeletesFilmAndRedirects()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.DeleteFilmAsync(filmId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _deleteModel.OnPostAsync(filmId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);
        _mockFilmService.Verify(s => s.DeleteFilmAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithValidId_LogsInformation()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.DeleteFilmAsync(filmId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _deleteModel.OnPostAsync(filmId);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Film deleted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_RedirectsToIndex()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.DeleteFilmAsync(filmId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _deleteModel.OnPostAsync(filmId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_LogsError()
    {
        // Arrange
        var filmId = 1;
        _mockFilmService.Setup(s => s.DeleteFilmAsync(filmId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act
        await _deleteModel.OnPostAsync(filmId);

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
    public async Task OnGetAsync_WithZeroId_CallsService()
    {
        // Arrange
        var filmId = 0;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _deleteModel.OnGetAsync(filmId);

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
        var result = await _deleteModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Film_DefaultValue_IsNull()
    {
        // Arrange & Act
        var model = new DeleteModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.Null(model.Film);
    }
}
