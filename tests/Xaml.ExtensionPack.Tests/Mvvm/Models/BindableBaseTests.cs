using System.ComponentModel;
using System.Linq.Expressions;

namespace Xaml.ExtensionPack;

public class BindableBaseTests
{
    private class TestBindable : BindableBase
    {
        private string _name = "";
        private int _value;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public bool SetValue<T>(ref T storage, T value, string propertyName = "")
            => SetProperty(ref storage, value, propertyName);

        public void SetValueWithCallback<T>(ref T storage, T value, Action? onChanged, string propertyName = "")
            => SetProperty(ref storage, value, onChanged, propertyName);

        public void RaisePropertyChangedByExpression<T>(Expression<Func<T>> propertyExpression)
            => OnPropertyChanged(propertyExpression);

        public void RaisePropertyChangedByName(string propertyName)
            => OnPropertyChanged(propertyName);
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ReturnsTrue()
    {
        // Arrange
        var bindable = new TestBindable();
        var storage = "";

        // Act
        var result = bindable.SetValue(ref storage, "new value");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void SetProperty_WhenValueDoesNotChange_ReturnsFalse()
    {
        // Arrange
        var bindable = new TestBindable();
        var storage = "same";

        // Act
        var result = bindable.SetValue(ref storage, "same");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SetProperty_WhenValueChanges_RaisesPropertyChangedWithCorrectName()
    {
        // Arrange
        var bindable = new TestBindable();
        PropertyChangedEventArgs? eventArgs = null;
        bindable.PropertyChanged += (s, e) => eventArgs = e;

        // Act
        bindable.Name = "new value";

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(nameof(TestBindable.Name), eventArgs!.PropertyName);
    }

    [Fact]
    public void SetProperty_WhenValueDoesNotChange_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var bindable = new TestBindable();
        bindable.Name = "initial";
        var eventCount = 0;
        bindable.PropertyChanged += (s, e) => eventCount++;

        // Act
        bindable.Name = "initial";

        // Assert
        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void SetProperty_WithOnChangedAction_WhenValueChanges_InvokesAction()
    {
        // Arrange
        var bindable = new TestBindable();
        var actionCalled = false;
        var name = "";

        // Act
        bindable.SetValueWithCallback(ref name, "new value", () => actionCalled = true, nameof(TestBindable.Name));

        // Assert
        Assert.True(actionCalled);
    }

    [Fact]
    public void SetProperty_WithOnChangedAction_WhenValueDoesNotChange_DoesNotInvokeAction()
    {
        // Arrange
        var bindable = new TestBindable();
        var actionCalled = false;
        var name = "same";

        // Act
        bindable.SetValueWithCallback(ref name, "same", () => actionCalled = true, nameof(TestBindable.Name));

        // Assert
        Assert.False(actionCalled);
    }

    [Fact]
    public void OnPropertyChanged_WithExpression_RaisesPropertyChangedWithCorrectName()
    {
        // Arrange
        var bindable = new TestBindable();
        PropertyChangedEventArgs? eventArgs = null;
        bindable.PropertyChanged += (s, e) => eventArgs = e;

        // Act
        bindable.RaisePropertyChangedByExpression(() => bindable.Name);

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(nameof(TestBindable.Name), eventArgs!.PropertyName);
    }

    [Fact]
    public void OnPropertyChanged_WithPropertyName_RaisesPropertyChangedWithCorrectName()
    {
        // Arrange
        var bindable = new TestBindable();
        PropertyChangedEventArgs? eventArgs = null;
        bindable.PropertyChanged += (s, e) => eventArgs = e;

        // Act
        bindable.RaisePropertyChangedByName(nameof(TestBindable.Name));

        // Assert
        Assert.NotNull(eventArgs);
        Assert.Equal(nameof(TestBindable.Name), eventArgs!.PropertyName);
    }
}
