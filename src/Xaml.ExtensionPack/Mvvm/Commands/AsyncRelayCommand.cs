namespace Xaml.ExtensionPack;

/// <summary>
/// A command that relays its functionality to an asynchronous delegate.
/// 機能を非同期デリゲートにリレーするコマンドです。
/// </summary>
public class AsyncRelayCommand : IAsyncCommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution logic as a Task. Taskとしての実行ロジック。</param>
    /// <param name="canExecute">The execution status logic. 実行状態のロジック。</param>
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Gets a value indicating whether the command is currently executing.
    /// コマンドが現在実行中かどうかを示す値を取得します。
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    /// <param name="parameter">Data used by the command. コマンドで使用されるデータ。</param>
    /// <returns>True if the command can be executed. 実行可能な場合は true。</returns>
    public bool CanExecute(object? parameter) => !IsExecuting && (_canExecute?.Invoke() ?? true);

    /// <summary>
    /// Executes the command. (Standard ICommand implementation)
    /// コマンドを実行します（ICommandの標準実装）。
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public void Execute(object? parameter) => ExecuteAsync(parameter).FireAndForget();

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        try
        {
            IsExecuting = true;
            await _execute();
        }
        finally
        {
            IsExecuting = false;
        }
    }

    /// <summary>
    /// Notifies the system that the command execution status has changed.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// A generic command that relays its functionality to an asynchronous delegate with a parameter.
/// パラメータを持つ非同期デリゲートに機能をリレーするジェネリックなコマンドです。
/// </summary>
/// <typeparam name="T">The type of the command parameter. パラメータの型。</typeparam>
public class AsyncRelayCommand<T> : IAsyncCommand
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private bool _isExecuting;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic as a Task. Taskとしての実行ロジック。</param>
    /// <param name="canExecute">The execution status logic. 実行状態のロジック。</param>
    public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Gets a value indicating whether the command is currently executing.
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting != value)
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>True if the command can be executed.</returns>
    public bool CanExecute(object? parameter)
    {
        if (IsExecuting) return false;
        if (_canExecute == null) return true;
        if (parameter == null) return typeof(T).IsValueType ? _canExecute(default!) : _canExecute(default);
        return parameter is T t && _canExecute(t);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public void Execute(object? parameter) => ExecuteAsync(parameter).FireAndForget();

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        try
        {
            IsExecuting = true;
            if (parameter is T t) await _execute(t);
            else await _execute(default);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    /// <summary>
    /// Notifies the system that the command execution status has changed.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}