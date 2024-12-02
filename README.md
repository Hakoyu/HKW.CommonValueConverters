# HKW.CommonValueConverters

A common value or multi value converter base


## Use in WPF

[WPFConverter](https://github.com/Hakoyu/HKW.WPF/tree/master/HKW.WPF/Converters)

### ConverterBase

`ConverterBase` and `CommonDependencyProperty`

```csharp
public class CommonDependencyProperty
{
    public static CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName
    )
    {
        var dependencyProperty = DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TOwner)
        );
        return new(dependencyProperty);
    }

    public static CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName,
        TProperty defaultValue
    )
    {
        var dependencyProperty = DependencyProperty.Register(
            propertyName,
            typeof(TProperty),
            typeof(TOwner),
            new PropertyMetadata(defaultValue)
        );
        return new(dependencyProperty);
    }
}

public abstract class ConverterBase : DependencyObject, ICommonValueConverter
{
    protected ConverterBase()
    {
        CommonConverterBase.UnsetValue = DependencyProperty.UnsetValue;
    }

    public static readonly object UnsetValue = DependencyProperty.UnsetValue;

    public PreferredCulture PreferredCulture { get; set; } =
        ValueConvertersConfig.DefaultPreferredCulture;

    public T GetValue<T>(CommonDependencyProperty<T> commonDependencyProperty)
    {
        if (commonDependencyProperty.Value is not DependencyProperty dependency)
            throw new ArgumentNullException(nameof(commonDependencyProperty));
        return (T)GetValue(dependency);
    }

    public void SetValue<T>(CommonDependencyProperty<T> commonDependencyProperty, T value)
    {
        if (commonDependencyProperty.Value is not DependencyProperty dependency)
            throw new ArgumentNullException(nameof(commonDependencyProperty));
        SetValue(dependency, value);
    } 

    public static readonly CommonDependencyProperty<object> DefaultResultProperty =
        CommonDependencyProperty.Register<ConverterBase, object>(nameof(DefaultResult));

    public object DefaultResult
    {
        get => GetValue(DefaultResultProperty);
        set => SetValue(DefaultResultProperty, value);
    }

    public static readonly CommonDependencyProperty<PreferredCulture> PreferredCultureProperty =
        CommonDependencyProperty.Register<ConverterBase, PreferredCulture>(
            nameof(PreferredCulture)
        );

    public PreferredCulture PreferredCulture
    {
        get => GetValue(PreferredCultureProperty);
        set => SetValue(PreferredCultureProperty, value);
    }
}
```

### ValueConverterBase

```csharp
public abstract class ValueConverterBase : ConverterBase, IValueConverter
{
    private CommonValueConverters.ValueConverterBase? _commonValueConverter;

    public CommonValueConverters.ValueConverterBase? CommonValueConverter
    {
        get => _commonValueConverter;
        set => CommonValueConverterInitialize(_commonValueConverter = value!);
    }

    // Add GetDefaultResult action when setting CommonValueConverter
    public virtual void CommonValueConverterInitialize(
        CommonValueConverters.ValueConverterBase commonValueConverter
    )
    {
        commonValueConverter.GetDefaultResult = () => DefaultResult;
    }

    public virtual object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return CommonValueConverter?.Convert(value, targetType, parameter, culture);
    }

    public virtual object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return CommonValueConverter?.ConvertBack(value, targetType, parameter, culture);
    }

    object? IValueConverter.Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    )
    {
        return Convert(
            value,
            targetType,
            parameter,
            ValueConvertersConfig.SelectCulture(PreferredCulture, () => culture)
        );
    }

    object? IValueConverter.ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    )
    {
        return ConvertBack(
            value,
            targetType,
            parameter,
            ValueConvertersConfig.SelectCulture(PreferredCulture, () => culture)
        );
    }
}
```
### BoolToValue

```csharp
public class BoolToValueConverter<T> : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public BoolToValueConverter()
    {
        CommonValueConverter = new CommonValueConverters.BoolToValueConverter<T>()
        {
            GetTrueValue = () => TrueValue,
            GetFalseValue = () => FalseValue,
            GetNullValue = () => NullValue,
            GetIsNullable = () => IsNullable,
        };
    }

    public static readonly CommonDependencyProperty<T> TrueValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(nameof(TrueValue));

    public T TrueValue
    {
        get => GetValue(TrueValueProperty);
        set => SetValue(TrueValueProperty, value);
    }

    public static readonly CommonDependencyProperty<T> FalseValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(nameof(FalseValue));

    public T FalseValue
    {
        get => GetValue(FalseValueProperty);
        set => SetValue(FalseValueProperty, value);
    }

    public static readonly CommonDependencyProperty<bool> IsNullableProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, bool>(nameof(IsNullable));

    public bool IsNullable
    {
        get => GetValue(IsNullableProperty);
        set => SetValue(IsNullableProperty, value);
    }

    public static readonly CommonDependencyProperty<T> NullValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(nameof(NullValue));

    public T NullValue
    {
        get => GetValue(NullValueProperty);
        set => SetValue(NullValueProperty, value);
    }
}
```

### BoolToVisibilityConverter

```csharp
public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
{
    public BoolToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }
}
```