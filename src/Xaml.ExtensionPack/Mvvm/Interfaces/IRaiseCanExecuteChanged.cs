using System.Windows.Input;

namespace Xaml.ExtensionPack;

/// <summary>
/// Defines a command that can notify that its execution status has changed.
/// 実行状態が変更されたことを通知できるコマンドを定義します。
/// </summary>
public interface IRaiseCanExecuteChanged : ICommand
{
    /// <summary>
    /// Notifies the system that the command execution status has changed.
    /// コマンドの実行状態が変更されたことを通知します。
    /// </summary>
    void RaiseCanExecuteChanged();
}