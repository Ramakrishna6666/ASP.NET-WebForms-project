using Films.Web.Pages.Actors;
using Xunit;

namespace Films.Web.Tests.Pages.Actors;

public class IndexModelTests
{
    private readonly IndexModel _indexModel;

    public IndexModelTests()
    {
        _indexModel = new IndexModel();
    }

    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Arrange & Act
        var model = new IndexModel();

        // Assert
        Assert.NotNull(model);
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

    [Fact]
    public void OnGet_DoesNotThrowException()
    {
        // Arrange & Act
        var exception = Record.Exception(() => _indexModel.OnGet());

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IndexModel_InheritsFromPageModel()
    {
        // Arrange & Act
        var model = new IndexModel();

        // Assert
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Mvc.RazorPages.PageModel>(model);
    }

    [Fact]
    public void OnGet_CanBeCalledMultipleTimes()
    {
        // Arrange
        // Act
        _indexModel.OnGet();
        _indexModel.OnGet();
        _indexModel.OnGet();

        // Assert - Method completes without exception
        Assert.True(true);
    }
}
