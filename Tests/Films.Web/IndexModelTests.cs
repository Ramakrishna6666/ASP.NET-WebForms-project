using Films.Web.Pages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Films.Web.Tests;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _indexModel;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _indexModel = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange & Act
        var model = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IndexModel(null!));
    }

    [Fact]
    public void OnGet_LogsInformation()
    {
        // Arrange
        // Act
        _indexModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OnGet_DoesNotThrowException()
    {
        // Arrange & Act
        var exception = Record.Exception(() => _indexModel.OnGet());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        // Arrange
        // Act
        _indexModel.OnGet();

        // Assert - Method completes without exception
        Assert.True(true);
    }
}
