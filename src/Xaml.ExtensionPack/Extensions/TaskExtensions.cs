namespace Xaml.ExtensionPack;

/// <summary>
/// Extensions for Task to handle exceptions in async void methods.
/// async void メソッドでの例外を安全に扱うための Task 拡張メソッドです。
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Safely fires and forgets a task.
    /// タスクを実行し、例外が発生した場合は指定されたハンドラで処理します。
    /// </summary>
    /// <param name="task">The task to execute. 実行するタスク。</param>
    /// <param name="onException">Action to call when an exception occurs. 例外発生時に呼び出されるアクション。</param>
    public static async void FireAndForget(this Task task, Action<Exception>? onException = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
        }
    }
}