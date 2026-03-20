using System.Windows.Input;

namespace Xaml.ExtensionPack;

/// <summary>
/// A command whose sole purpose is to relay its functionality to other objects.
/// デリゲートを呼び出すことで機能を他オブジェクトへリレーするコマンドです。
/// </summary>
public class RelayCommand : IRaiseCanExecuteChanged
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// 実行状態に影響を及ぼす変更が発生したときに発生します。
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// <see cref="RelayCommand"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="execute">The execution logic. 実行ロジック。</param>
    /// <param name="canExecute">The execution status logic. 実行状態のロジック。</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Notifies the system that the command execution status has changed.
    /// 実行状態が変更されたことを通知します。
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Determines whether the command can execute.
    /// コマンドが実行可能かどうかを決定します。
    /// </summary>
    /// <param name="parameter">Data used by the command. コマンドで使用されるデータ。</param>
    /// <returns>True if the command can be executed. 実行可能な場合は true。</returns>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    /// <summary>
    /// Executes the command.
    /// コマンドを実行します。
    /// </summary>
    /// <param name="parameter">Data used by the command. コマンドで使用されるデータ。</param>
    public void Execute(object? parameter) => _execute();
}

/// <summary>
/// A generic command that relays its functionality to other objects.
/// 他のオブジェクトに機能をリレーするジェネリックなコマンドです。
/// </summary>
/// <typeparam name="T">The type of the command parameter. パラメータの型。</typeparam>
public class RelayCommand<T> : IRaiseCanExecuteChanged
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The execution logic. 実行ロジック。</param>
    /// <param name="canExecute">The execution status logic. 実行状態のロジック。</param>
    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Notifies the system that the command execution status has changed.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Determines whether the command can execute.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    /// <returns>True if the command can be executed.</returns>
    public bool CanExecute(object? parameter)
    {
        if (_canExecute == null) return true;
        if (parameter == null) return typeof(T).IsValueType ? _canExecute(default!) : _canExecute(default);
        return parameter is T t && _canExecute(t);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public void Execute(object? parameter)
    {
        if (parameter is T t) _execute(t);
        else if (parameter == null) _execute(default);
    }
}