using Films.Application.DTOs;
using Films.Application.Services;
using Films.Web.Pages.Films;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests.Pages.Films;

public class EditModelTests
{
    private readonly Mock<IFilmService> _mockFilmService;
    private readonly Mock<ILogger<EditModel>> _mockLogger;
    private readonly EditModel _editModel;

    public EditModelTests()
    {
        _mockFilmService = new Mock<IFilmService>();
        _mockLogger = new Mock<ILogger<EditModel>>();
        _editModel = new EditModel(_mockFilmService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var model = new EditModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullFilmService_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EditModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EditModel(_mockFilmService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_ReturnsPageResult()
    {
        // Arrange
        var filmId = 1;
        var existingFilm = new FilmDto 
        { 
            Id = filmId, 
            Title = "Existing Film", 
            Year = 2020,
            Description = "Description",
            Genre = "Action"
        };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(existingFilm);

        // Act
        var result = await _editModel.OnGetAsync(filmId);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_editModel.Film);
        Assert.Equal(filmId, _editModel.Film.Id);
        Assert.Equal("Existing Film", _editModel.Film.Title);
    }

    [Fact]
    public async Task OnGetAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var filmId = 999;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _editModel.OnGetAsync(filmId);

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
        var result = await _editModel.OnGetAsync(filmId);

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
        await _editModel.OnGetAsync(filmId);

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
    public async Task OnGetAsync_MapsFilmDtoToUpdateFilmDto()
    {
        // Arrange
        var filmId = 1;
        var existingFilm = new FilmDto 
        { 
            Id = filmId, 
            Title = "Test Film", 
            Year = 2020,
            Description = "Test Description",
            Genre = "Drama"
        };
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync(existingFilm);

        // Act
        await _editModel.OnGetAsync(filmId);

        // Assert
        Assert.Equal(existingFilm.Id, _editModel.Film.Id);
        Assert.Equal(existingFilm.Title, _editModel.Film.Title);
        Assert.Equal(existingFilm.Year, _editModel.Film.Year);
        Assert.Equal(existingFilm.Description, _editModel.Film.Description);
        Assert.Equal(existingFilm.Genre, _editModel.Film.Genre);
    }

    [Fact]
    public async Task OnPostAsync_WithValidModel_UpdatesFilmAndRedirects()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023,
            Description = "Updated Description",
            Genre = "Action"
        };
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FilmDto());
        _editModel.ModelState.Clear();

        // Act
        var result = await _editModel.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirectResult.PageName);
        _mockFilmService.Verify(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModel_ReturnsPage()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto();
        _editModel.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _editModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        _mockFilmService.Verify(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WithValidModel_LogsInformation()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023 
        };
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FilmDto());
        _editModel.ModelState.Clear();

        // Act
        await _editModel.OnPostAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Film updated")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023 
        };
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _editModel.ModelState.Clear();

        // Act
        var result = await _editModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(_editModel.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_LogsError()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023 
        };
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _editModel.ModelState.Clear();

        // Act
        await _editModel.OnPostAsync();

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
    public async Task OnPostAsync_WhenServiceThrowsException_AddsModelError()
    {
        // Arrange
        _editModel.Film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023 
        };
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _editModel.ModelState.Clear();

        // Act
        await _editModel.OnPostAsync();

        // Assert
        Assert.True(_editModel.ModelState.ContainsKey(string.Empty));
        Assert.Contains(_editModel.ModelState[string.Empty]!.Errors, 
            e => e.ErrorMessage.Contains("error occurred"));
    }

    [Fact]
    public void Film_DefaultValue_IsNotNull()
    {
        // Arrange & Act
        var model = new EditModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model.Film);
    }

    [Fact]
    public void Film_CanBeSet()
    {
        // Arrange
        var film = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Test Film", 
            Year = 2023 
        };

        // Act
        _editModel.Film = film;

        // Assert
        Assert.Equal(film, _editModel.Film);
        Assert.Equal("Test Film", _editModel.Film.Title);
    }

    [Fact]
    public async Task OnPostAsync_WithValidData_PassesCorrectDataToService()
    {
        // Arrange
        var updateDto = new UpdateFilmDto 
        { 
            Id = 1,
            Title = "Updated Film", 
            Year = 2023,
            Description = "Updated Description",
            Genre = "Drama"
        };
        _editModel.Film = updateDto;
        _editModel.ModelState.Clear();

        UpdateFilmDto? capturedDto = null;
        _mockFilmService.Setup(s => s.UpdateFilmAsync(It.IsAny<UpdateFilmDto>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateFilmDto>(dto => capturedDto = dto)
            .ReturnsAsync(new FilmDto());

        // Act
        await _editModel.OnPostAsync();

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal(updateDto.Id, capturedDto.Id);
        Assert.Equal(updateDto.Title, capturedDto.Title);
        Assert.Equal(updateDto.Year, capturedDto.Year);
    }

    [Fact]
    public async Task OnGetAsync_WithZeroId_CallsService()
    {
        // Arrange
        var filmId = 0;
        _mockFilmService.Setup(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>())).ReturnsAsync((FilmDto?)null);

        // Act
        var result = await _editModel.OnGetAsync(filmId);

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
        var result = await _editModel.OnGetAsync(filmId);

        // Assert
        _mockFilmService.Verify(s => s.GetFilmByIdAsync(filmId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
