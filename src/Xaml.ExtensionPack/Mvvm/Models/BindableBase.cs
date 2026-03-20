using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xaml.ExtensionPack;

/// <summary>
/// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
/// モデルを簡略化するための <see cref="INotifyPropertyChanged"/> の実装を提供します。
/// </summary>
public abstract class BindableBase : INotifyPropertyChanged
{
    /// <summary>
    /// Multicast event for property change notifications.
    /// プロパティ値の変更通知用のマルチキャストイベントです。
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets the property and notifies listeners only when necessary.
    /// 必要な場合のみプロパティ値を設定し、リスナーに通知します。
    /// </summary>
    /// <typeparam name="T">Type of the property. プロパティの型。</typeparam>
    /// <param name="storage">Reference to a property with both getter and setter. プロパティの実体への参照。</param>
    /// <param name="value">Desired value for the property. 設定する値。</param>
    /// <param name="propertyName">Name of the property used to notify listeners. 通知に使用するプロパティ名。</param>
    /// <returns>True if the value was changed, false if the existing value matched the desired value. 値が変更された場合は true、既存の値と同じ場合は false。</returns>
    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Sets the property and notifies listeners, then executes the specified action.
    /// 必要な場合のみプロパティ値を設定して通知を行い、指定されたアクションを実行します。
    /// </summary>
    /// <typeparam name="T">Type of the property. プロパティの型。</typeparam>
    /// <param name="storage">Reference to a property with both getter and setter. プロパティの実体への参照。</param>
    /// <param name="value">Desired value for the property. 設定する値。</param>
    /// <param name="onChanged">Action called after the property value has been changed. 変更後に実行されるアクション。</param>
    /// <param name="propertyName">Name of the property used to notify listeners. 通知に使用するプロパティ名。</param>
    protected void SetProperty<T>(ref T storage, T value, Action? onChanged, [CallerMemberName] string propertyName = "")
    {
        if (SetProperty(ref storage, value, propertyName))
        {
            onChanged?.Invoke();
        }
    }

    /// <summary>
    /// Notifies listeners that a property value has changed using an expression.
    /// ラムダ式を使用してプロパティ値の変更を通知します（リファクタリングに強い）。
    /// </summary>
    /// <typeparam name="T">The type of the property. プロパティの型。</typeparam>
    /// <param name="propertyExpression">The expression representing the property. プロパティを表す式。</param>
    protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
        {
            OnPropertyChanged(propertyInfo.Name);
        }
    }

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// プロパティ値が変更されたことをリスナーに通知します。
    /// </summary>
    /// <param name="propertyName">Name of the property used to notify listeners. 通知に使用するプロパティ名。</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}