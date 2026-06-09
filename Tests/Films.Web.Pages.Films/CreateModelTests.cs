using Films.Application.DTOs;
using Films.Application.Services;
using Films.Web.Pages.Films;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests.Pages.Films;

public class CreateModelTests
{
    private readonly Mock<IFilmService> _mockFilmService;
    private readonly Mock<ILogger<CreateModel>> _mockLogger;
    private readonly CreateModel _createModel;

    public CreateModelTests()
    {
        _mockFilmService = new Mock<IFilmService>();
        _mockLogger = new Mock<ILogger<CreateModel>>();
        _createModel = new CreateModel(_mockFilmService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var model = new CreateModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullFilmService_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateModel(_mockFilmService.Object, null!));
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        // Arrange
        // Act
        _createModel.OnGet();

        // Assert - Method completes without exception
        Assert.True(true);
    }

    [Fact]
    public void OnGet_DoesNotThrowException()
    {
        // Arrange & Act
        var exception = Record.Exception(() => _createModel.OnGet());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task OnPostAsync_WithValidModel_CreatesFilmAndRedirects()
    {
        // Arrange
        _createModel.Film = new CreateFilmDto 
        { 
            Title = "New Film", 
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FilmDto());
        // Act
        var result = await _createModel.OnPostAsync();
        _mockFilmService.Verify(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()), Times.Once);
    [Fact]
    public async Task OnPostAsync_WithInvalidModel_ReturnsPage()
    {
        // Arrange
        _createModel.Film = new CreateFilmDto();
        _createModel.ModelState.AddModelError("Title", "Title is required");
        _mockFilmService.Verify(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WithValidModel_LogsInformation()
    {
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FilmDto());
        _createModel.ModelState.Clear();

        // Act
        await _createModel.OnPostAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Film created")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_ReturnsPageWithError()
    {
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _createModel.ModelState.Clear();

        // Act
        var result = await _createModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(_createModel.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_WhenServiceThrowsException_LogsError()
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()))
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>()))
            .ThrowsAsync(new Exception("Database error"));
        _createModel.ModelState.Clear();

        // Act
        await _createModel.OnPostAsync();

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
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        _createModel.ModelState.Clear();

        // Act
        await _createModel.OnPostAsync();

        // Assert
        Assert.True(_createModel.ModelState.ContainsKey(string.Empty));
        Assert.Contains(_createModel.ModelState[string.Empty]!.Errors, 
            e => e.ErrorMessage.Contains("error occurred"));
    }

    [Fact]
    public void Film_DefaultValue_IsNotNull()
    {
        // Arrange & Act
        var model = new CreateModel(_mockFilmService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(model.Film);
    }

    [Fact]
    public void Film_CanBeSet()
    {
        // Arrange
        var film = new CreateFilmDto 
        { 
            Title = "Test Film", 
            Year = 2023 
        };

        // Act
        _createModel.Film = film;

        // Assert
        Assert.Equal(film, _createModel.Film);
        Assert.Equal("Test Film", _createModel.Film.Title);
    }

    [Fact]
    public async Task OnPostAsync_WithEmptyTitle_DoesNotCallService()
    {
        // Arrange
        _mockFilmService.Verify(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()), Times.Never);
        // Act
        await _createModel.OnPostAsync();

        // Assert
        _mockFilmService.Verify(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WithValidData_PassesCorrectDataToService()
    {
        // Arrange
        var createDto = new CreateFilmDto 
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FilmDto());
        CreateFilmDto? capturedDto = null;
        _mockFilmService.Setup(s => s.CreateFilmAsync(It.IsAny<CreateFilmDto>()))
            .Callback<CreateFilmDto>(dto => capturedDto = dto)
            .Returns(Task.CompletedTask);

        // Act
        await _createModel.OnPostAsync();

        // Assert
        Assert.NotNull(capturedDto);
        Assert.Equal(createDto.Title, capturedDto.Title);
        Assert.Equal(createDto.Year, capturedDto.Year);
    }
}
