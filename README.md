# HKW.CommonValueConverters

面向 **.NET 10** 的平台无关单值/多值转换逻辑库。核心不引用 WPF 或 Avalonia；平台包装器通过组合持有核心实例，负责 XAML、属性系统和绑定结果映射。不支持 .NET Framework。

## 核心用法

```csharp
using HKW.CommonValueConverters;

var converter = new BoolToValueConverter<string>
{
    GetTrueValue = () => "Yes",
    GetFalseValue = () => "No",
    GetIsNullable = () => true,
    GetNullValue = () => "Unknown",
};
var result = converter.Convert(true, typeof(string), null, null); // "Yes"
```

配置通过 `Func<T>` 在转换时读取，无需复制平台属性值。每个包装器应独占核心实例，不要共享捕获不同 UI 对象的委托。核心不负责 UI 调度，也不保证被捕获的平台对象可跨线程访问。

## 结果和输入契约

| 值 | 含义 | 包装层职责 |
|---|---|---|
| `null` | 合法空结果 | 原样传递 |
| `ConverterResults.UnsetValue` | 输入未就绪或转换失败 | 映射为平台未设置值 |
| `ConverterResults.DoNothing` | 不更新目标 | 映射为平台不更新值 |

两种哨兵是不可变的、身份唯一的对象；用 `ReferenceEquals` 判断。不要修改全局状态来“注册”平台哨兵。输入、参数及多绑定各个输入槽都要先映射，输出也要映射；多值反向数组按项映射。

- `GlobalDefaultResult` 初始为核心 `UnsetValue`；实例可用 `GetDefaultResult` 返回任意自定义值，包括 `null` 或 `DoNothing`。默认结果委托仅在失败路径调用。
- `EnumEqualsConverter` 保留自身默认 `false` 的约定，可通过 `GetDefaultResult` 覆盖。
- 普通无效数据、缺失必需输入和已知格式错误返回默认结果；用户委托、对象方法和意外编程异常不被统一吞掉。
- 未实现反向转换时抛出 `NotSupportedException`；请使用合适的绑定模式。
- 可空布尔映射先处理 `null`，反转只交换真假。相等谓词中两个 `null` 相等；有独立空值分支的映射转换器仍走该分支。
- 选值多绑定只要求条件及选中分支就绪；选中分支缺失返回默认值，未选中分支不会阻塞。
- 格式化多绑定的 `GetReplaceUnsetValue` 可显式替换未设置参数；`DoNothing` 不作为普通格式化参数，格式源本身必须就绪。
- 空集合聚合保持 `All=true`、`Any=false`、`Count=0`，`AllBoolToValueMultiConverter` 走真值分支；要求首元素的转换器返回默认结果。
- 计算器仍从左到右求值，不引入表达式运算优先级。范围参数仍为逗号分隔的两个数，不支持额外的范围语法。
- `DebugConverter` 是诊断透传转换器，保留输入及反向输入对象身份，包括核心哨兵。

## 文化与日期时间

公共 `Convert` / `ConvertBack` 统一选择有效文化。实例 `PreferredCulture` 优先于 `ValueConvertersConfig.DefaultPreferredCulture`；`ConverterCulture` 在调用文化为 `null` 时回退 `CultureInfo.CurrentCulture`。包装器无需再次调用文化选择器。

日期时间和 TimeSpan 的格式优先级是字符串参数 → `GetFormat()`。反向先尝试指定格式精确解析，再用同一文化宽松解析。日期正向仍转换到本地时区，反向仍转换到 UTC；不带时区的输入按系统本地时区解释。显示格式可能丢失秒、偏移等信息，不保证任意格式都可无损往返。数字正反向使用同一有效文化。

## WPF 包装示例

下面是一个可放入 WPF 应用的最小单向包装器。依赖属性完全属于平台层，不再使用核心的通用属性抽象。

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Core = HKW.CommonValueConverters;

namespace Example;

public sealed class BoolToVisibilityConverter : DependencyObject, IValueConverter
{
    private readonly Core.BoolToValueConverter<Visibility> _core;

    public BoolToVisibilityConverter()
    {
        _core = new()
        {
            GetTrueValue = () => Visibility.Visible,
            GetFalseValue = () => FalseValue,
            GetIsInverted = () => IsInverted,
            GetDefaultResult = () => ToCore(DefaultResult),
        };
    }

    public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
        nameof(FalseValue), typeof(Visibility), typeof(BoolToVisibilityConverter),
        new PropertyMetadata(Visibility.Collapsed));
    public Visibility FalseValue
    {
        get => (Visibility)GetValue(FalseValueProperty);
        set => SetValue(FalseValueProperty, value);
    }

    public static readonly DependencyProperty IsInvertedProperty = DependencyProperty.Register(
        nameof(IsInverted), typeof(bool), typeof(BoolToVisibilityConverter), new PropertyMetadata(false));
    public bool IsInverted
    {
        get => (bool)GetValue(IsInvertedProperty);
        set => SetValue(IsInvertedProperty, value);
    }

    public static readonly DependencyProperty DefaultResultProperty = DependencyProperty.Register(
        nameof(DefaultResult), typeof(object), typeof(BoolToVisibilityConverter),
        new PropertyMetadata(Core.ConverterResults.UnsetValue));
    public object? DefaultResult
    {
        get => GetValue(DefaultResultProperty);
        set => SetValue(DefaultResultProperty, value);
    }

    public Core.PreferredCulture PreferredCulture
    {
        get => _core.PreferredCulture;
        set => _core.PreferredCulture = value;
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        FromCore(_core.Convert(ToCore(value), targetType, ToCore(parameter), culture));

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        FromCore(_core.ConvertBack(ToCore(value), targetType, ToCore(parameter), culture));

    private static object? ToCore(object? value) =>
        ReferenceEquals(value, DependencyProperty.UnsetValue) ? Core.ConverterResults.UnsetValue :
        ReferenceEquals(value, Binding.DoNothing) ? Core.ConverterResults.DoNothing : value;

    private static object? FromCore(object? value) =>
        ReferenceEquals(value, Core.ConverterResults.UnsetValue) ? DependencyProperty.UnsetValue :
        ReferenceEquals(value, Core.ConverterResults.DoNothing) ? Binding.DoNothing : value;
}
```

在 XAML 中按应用命名空间注册该包装器，使用 `Mode=OneWay`。如需可空布尔映射，可同样为 `GetIsNullable`、`GetNullValue` 提供平台属性委托。修改包装器属性后，下一次转换会读取新值，但属性变化**不自动保证重新触发绑定转换**；必要时由应用或包装层刷新绑定。

## Avalonia 包装约定

沿用相同组合方式，使用 Avalonia 自己的属性系统和转换器接口：

| 核心 | WPF | Avalonia |
|---|---|---|
| `ConverterResults.UnsetValue` | `DependencyProperty.UnsetValue` | `AvaloniaProperty.UnsetValue` |
| `ConverterResults.DoNothing` | `Binding.DoNothing` | `BindingOperations.DoNothing`（`Avalonia.Data`） |

两种平台可以拥有独立包装器，不会互相覆盖核心哨兵。Avalonia 多绑定仅实现平台支持的方向；不要因为核心具有多值反向方法就假定平台支持反向多绑定。本仓库未包含平台项目或真实 XAML 集成测试。

## 自定义转换器

```csharp
using System;
using System.Globalization;
using HKW.CommonValueConverters;

public sealed class UpperCaseConverter : ValueConverterBase
{
    protected override object? ConvertCore(
        object? value, Type? targetType, object? parameter, CultureInfo culture)
    {
        return value is string text ? culture.TextInfo.ToUpper(text) : GetDefaultResult();
    }
}
```

`culture` 已解析且非空。需要反向时重写 `ConvertBackCore`；多值实现继承 `MultiValueConverterBase`。按转换器的实际输入语义处理哨兵，不要把所有可选输入一律过滤。

## 破坏性变更迁移

| 旧用法 | 新用法 |
|---|---|
| 重写公共 `Convert` / `ConvertBack` | 重写受保护的 `ConvertCore` / `ConvertBackCore`，文化参数非空 |
| 设置 `CommonConverterBase.UnsetValue` | 在包装层双向映射 `ConverterResults.UnsetValue` |
| 使用 `CommonDependencyProperty<T>` 或接口 `GetValue` / `SetValue` | 直接使用平台属性 API；核心 `ICommonValueConverter` 仅保留文化偏好契约 |
| 失败结果默认 `null` | 默认核心 `UnsetValue`；需要旧行为时显式设置 `GetDefaultResult = () => null` |
| `SelectCulture(() => culture)` | `SelectCulture(culture)`；公共转换入口会自动选择文化 |
| 依赖错误数量导致异常 | 普通输入失败返回默认结果；不支持的反向转换仍抛异常 |
| 可空反转改变空分支、相等多绑定认为两个 null 不相等 | 空分支独立，两个 null 相等 |
| 反向解析依赖进程文化 | 使用统一有效文化，先精确格式再宽松解析 |

源码中全注释的 `ValueConverterGroup`、`StringToObjectConverter` 不属于已提供的功能。

## 验证

使用 .NET 10 SDK 和现有 MSTest/VSTest 配置：

```powershell
dotnet test HKW.CommonValueConverters.Tests\HKW.CommonValueConverters.Tests.csproj
dotnet build HKW.CommonValueConverters\HKW.CommonValueConverters.csproj --configuration Release
```

构建会按项目的 `GeneratePackageOnBuild` 配置生成 NuGet 包，不会自动发布。
