namespace Xaml.ExtensionPack;

public class AsyncRelayCommandTests
{
    [Fact]
    public void Constructor_WhenExecuteIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Task>? execute = null;

        // Act
        var act = () => new AsyncRelayCommand(execute!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void CanExecute_WithNoCanExecuteDelegate_WhenNotExecuting_ReturnsTrue()
    {
        // Arrange
        var command = new AsyncRelayCommand(() => Task.CompletedTask);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var command = new AsyncRelayCommand(() => Task.CompletedTask, () => false);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanExecute_WhenIsExecuting_ReturnsFalse()
    {
        // Arrange
        var executionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var command = new AsyncRelayCommand(async () =>
        {
            executionStarted.SetResult(true);
            await allowCompletion.Task;
        });

        // Act
        var executeTask = command.ExecuteAsync(null);
        await executionStarted.Task;
        var result = command.CanExecute(null);
        allowCompletion.SetResult(true);
        await executeTask;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCalled_InvokesExecuteDelegate()
    {
        // Arrange
        var executed = false;
        var command = new AsyncRelayCommand(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCannotExecute_DoesNotInvokeExecuteDelegate()
    {
        // Arrange
        var executed = false;
        var command = new AsyncRelayCommand(
            () => { executed = true; return Task.CompletedTask; },
            () => false);

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.False(executed);
    }

    [Fact]
    public async Task ExecuteAsync_AfterExecution_IsExecutingReturnsFalse()
    {
        // Arrange
        var command = new AsyncRelayCommand(() => Task.CompletedTask);

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.False(command.IsExecuting);
    }

    [Fact]
    public async Task ExecuteAsync_DuringExecution_IsExecutingReturnsTrue()
    {
        // Arrange
        var executionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        bool isExecutingDuringExecution = false;
        var command = new AsyncRelayCommand(async () =>
        {
            executionStarted.SetResult(true);
            await allowCompletion.Task;
        });

        // Act
        var executeTask = command.ExecuteAsync(null);
        await executionStarted.Task;
        isExecutingDuringExecution = command.IsExecuting;
        allowCompletion.SetResult(true);
        await executeTask;

        // Assert
        Assert.True(isExecutingDuringExecution);
        Assert.False(command.IsExecuting);
    }

    [Fact]
    public void RaiseCanExecuteChanged_WhenCalled_RaisesCanExecuteChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var command = new AsyncRelayCommand(() => Task.CompletedTask);
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.True(eventRaised);
    }
}

public class AsyncRelayCommandGenericTests
{
    [Fact]
    public void Constructor_WhenExecuteIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<string?, Task>? execute = null;

        // Act
        var act = () => new AsyncRelayCommand<string>(execute!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void CanExecute_WithNoCanExecuteDelegate_WhenNotExecuting_ReturnsTrue()
    {
        // Arrange
        var command = new AsyncRelayCommand<string>(_ => Task.CompletedTask);

        // Act
        var result = command.CanExecute("value");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanExecute_WhenCanExecuteReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var command = new AsyncRelayCommand<string>(_ => Task.CompletedTask, _ => false);

        // Act
        var result = command.CanExecute("value");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanExecute_WhenIsExecuting_ReturnsFalse()
    {
        // Arrange
        var executionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var command = new AsyncRelayCommand<string>(async _ =>
        {
            executionStarted.SetResult(true);
            await allowCompletion.Task;
        });

        // Act
        var executeTask = command.ExecuteAsync("value");
        await executionStarted.Task;
        var result = command.CanExecute("value");
        allowCompletion.SetResult(true);
        await executeTask;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenParameterMatchesType_InvokesExecuteWithParameter()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new AsyncRelayCommand<string>(p =>
        {
            receivedParameter = p;
            return Task.CompletedTask;
        });

        // Act
        await command.ExecuteAsync("hello");

        // Assert
        Assert.Equal("hello", receivedParameter);
    }

    [Fact]
    public async Task ExecuteAsync_WhenParameterIsNull_InvokesExecuteWithDefault()
    {
        // Arrange
        var executeCalled = false;
        var command = new AsyncRelayCommand<string>(_ =>
        {
            executeCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await command.ExecuteAsync(null);

        // Assert
        Assert.True(executeCalled);
    }

    [Fact]
    public void RaiseCanExecuteChanged_WhenCalled_RaisesCanExecuteChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var command = new AsyncRelayCommand<string>(_ => Task.CompletedTask);
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.True(eventRaised);
    }
}
