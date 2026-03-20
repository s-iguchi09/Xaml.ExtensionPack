namespace Xaml.ExtensionPack;

/// <summary>
/// Defines a command that allows asynchronous execution.
/// 非同期実行を可能にするコマンドを定義します。
/// </summary>
public interface IAsyncCommand : IRaiseCanExecuteChanged
{
    /// <summary>
    /// Executes the command asynchronously.
    /// コマンドを非同期に実行します。
    /// </summary>
    /// <param name="parameter">Data used by the command. コマンドで使用されるデータ。</param>
    /// <returns>A task that represents the asynchronous operation. 非同期操作を表すタスク。</returns>
    Task ExecuteAsync(object? parameter);

    /// <summary>
    /// Gets a value indicating whether the command is currently executing.
    /// コマンドが現在実行中かどうかを示す値を取得します。
    /// </summary>
    bool IsExecuting { get; }
}