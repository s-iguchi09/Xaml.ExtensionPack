namespace Xaml.ExtensionPack;

public class TaskExtensionsTests
{
    [Fact]
    public void FireAndForget_WhenTaskSucceeds_DoesNotCallHandler()
    {
        // Arrange
        var handlerCalled = false;
        var task = Task.CompletedTask;

        // Act
        task.FireAndForget(ex => handlerCalled = true);

        // Assert
        Assert.False(handlerCalled);
    }

    [Fact]
    public async Task FireAndForget_WhenTaskFails_CallsOnException()
    {
        // Arrange
        Exception? capturedException = null;
        var completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var task = Task.FromException(new InvalidOperationException("test error"));

        // Act
        task.FireAndForget(ex =>
        {
            capturedException = ex;
            completionSource.SetResult(true);
        });
        await Task.WhenAny(completionSource.Task, Task.Delay(5000));

        // Assert
        Assert.IsType<InvalidOperationException>(capturedException);
        Assert.Equal("test error", capturedException!.Message);
    }

    [Fact]
    public void FireAndForget_WhenTaskFailsAndNoHandler_DoesNotThrow()
    {
        // Arrange
        var task = Task.FromException(new InvalidOperationException("unhandled"));

        // Act
        task.FireAndForget();

        // Assert: test passes if no exception is thrown
    }
}
