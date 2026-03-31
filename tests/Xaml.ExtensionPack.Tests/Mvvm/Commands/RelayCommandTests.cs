namespace Xaml.ExtensionPack;

public class RelayCommandTests
{
    [Fact]
    public void Constructor_WhenExecuteIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Action? execute = null;

        // Act
        var act = () => new RelayCommand(execute!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void CanExecute_WithNoCanExecuteDelegate_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { });

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => false);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteReturnsTrue_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => true);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Execute_WhenCalled_InvokesExecuteDelegate()
    {
        // Arrange
        var executed = false;
        var command = new RelayCommand(() => executed = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void RaiseCanExecuteChanged_WhenCalled_RaisesCanExecuteChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var command = new RelayCommand(() => { });
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.True(eventRaised);
    }
}

public class RelayCommandGenericTests
{
    [Fact]
    public void Constructor_WhenExecuteIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Action<string?>? execute = null;

        // Act
        var act = () => new RelayCommand<string>(execute!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void CanExecute_WithNoCanExecuteDelegate_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });

        // Act
        var result = command.CanExecute("value");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenParameterMatchesType_InvokesCanExecuteWithParameter()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new RelayCommand<string>(_ => { }, p =>
        {
            receivedParameter = p;
            return true;
        });

        // Act
        command.CanExecute("hello");

        // Assert
        Assert.Equal("hello", receivedParameter);
    }

    [Fact]
    public void CanExecute_WhenParameterDoesNotMatchType_ReturnsFalse()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { }, _ => true);

        // Act
        var result = command.CanExecute(123);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Execute_WhenParameterMatchesType_InvokesExecuteWithParameter()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new RelayCommand<string>(p => receivedParameter = p);

        // Act
        command.Execute("hello");

        // Assert
        Assert.Equal("hello", receivedParameter);
    }

    [Fact]
    public void Execute_WhenParameterIsNull_InvokesExecuteWithDefault()
    {
        // Arrange
        var executeCalled = false;
        var command = new RelayCommand<string>(_ => executeCalled = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executeCalled);
    }

    [Fact]
    public void RaiseCanExecuteChanged_WhenCalled_RaisesCanExecuteChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var command = new RelayCommand<string>(_ => { });
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.True(eventRaised);
    }
}
