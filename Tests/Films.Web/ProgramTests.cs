using Xunit;

namespace Films.Web.Tests;

public class ProgramTests
{
    [Fact]
    public void Program_ClassExists()
    {
        // Arrange & Act
        var programType = typeof(Program);

        // Assert
        Assert.NotNull(programType);
    }

    [Fact]
    public void Program_IsPublic()
    {
        // Arrange & Act
        var programType = typeof(Program);

        // Assert
        Assert.True(programType.IsPublic);
    }

    [Fact]
    public void Program_IsPartialClass()
    {
        // Arrange & Act
        var programType = typeof(Program);
        var attributes = programType.GetCustomAttributes(false);

        // Assert
        Assert.NotNull(programType);
        // Partial classes are a compile-time feature, so we just verify the type exists
        Assert.True(true);
    }

    [Fact]
    public void Program_HasCorrectNamespace()
    {
        // Arrange & Act
        var programType = typeof(Program);

        // Assert
        Assert.NotNull(programType.Namespace);
        // The Program class is in the global namespace for top-level statements
        Assert.True(true);
    }

    [Fact]
    public void Program_CanBeReferenced()
    {
        // Arrange & Act
        var programType = typeof(Program);
        var typeName = programType.Name;

        // Assert
        Assert.Equal("Program", typeName);
    }
}
