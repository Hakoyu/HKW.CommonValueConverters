# HKW.CommonValueConverters

面向 **.NET 10** 的单值/多值转换器库，包含平台无关核心及独立的 WPF、Avalonia 包装项目。不支持 .NET Framework。

| 项目 / 包 / 命名空间 | 目标框架 | 用途 |
|---|---|---|
| `HKW.CommonValueConverters` | `net10.0` | 平台无关转换逻辑，不引用 UI 框架 |
| `HKW.CommonValueConverters.WPF` | `net10.0-windows` | WPF `DependencyObject`、依赖属性及转换器接口 |
| `HKW.CommonValueConverters.Avalonia` | `net10.0` | Avalonia 12.1.5 `AvaloniaObject`、StyledProperty 及转换器接口 |

平台库引用核心，每个包装器独占核心实例，复用通用转换算法。两个平台共有 28 个单值、10 个多值包装器，以及 6 个泛型家族的同名非泛型 `object?` 版本：`BoolToValueConverter`、`EqualsToValueConverter`、`AllBoolToValueMultiConverter`、`AllEqualsMultiConverter`、`AnyEqualsMultiConverter`、`EqualsCountMultiConverter`。WPF 另外提供 17 个平台专用转换器及便捷预设（14 个单值、3 个多值），共 61 个公开转换器类型；Avalonia 保持原有 44 个类型。WPF 专用算法的内部实现位于 WPF 程序集，不向平台无关核心引入 WPF 依赖。

仓库内应用可通过 `ProjectReference` 引用相应平台项目；也可在本地构建包后从本地 NuGet 源引用。新增平台包未接入自动发布流程，不应假设 NuGet.org 已存在这些版本。

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

## WPF 用法

引用 `HKW.CommonValueConverters.WPF`，在资源中直接使用非泛型包装器。下面的布尔映射无需额外的 `BoolToVisibilityConverter` 类型：

```xml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:c="clr-namespace:HKW.CommonValueConverters.WPF;assembly=HKW.CommonValueConverters.WPF">
    <Window.Resources>
        <c:BoolToValueConverter x:Key="ShowWhenTrue"
            TrueValue="{x:Static Visibility.Visible}"
            FalseValue="{x:Static Visibility.Collapsed}" />
    </Window.Resources>
    <TextBlock Text="Ready"
        Visibility="{Binding IsReady, Converter={StaticResource ShowWhenTrue}, Mode=OneWay}" />
</Window>
```

需要强类型配置时可使用 C# 泛型版本：

```csharp
var converter = new HKW.CommonValueConverters.WPF.BoolToValueConverter<System.Windows.Visibility>
{
    TrueValue = System.Windows.Visibility.Visible,
    FalseValue = System.Windows.Visibility.Collapsed,
};
```

### WPF 专用转换器与便捷预设

| 类型 | 默认配置或转换方向 |
|---|---|
| `BoolToVisibilityConverter` | 真为 `Visible`，假为 `Collapsed` |
| `BoolToFontWeightConverter` | 真为 `Bold`，假为 `Normal` |
| `BoolToSolidColorBrushConverter` | 真为白色笔刷，假为黑色笔刷 |
| `BoolToThicknessConverter` | 可配置真假边距，失败默认零边距 |
| `BoolToStyleConverter` | 可配置真假 `Style` |
| `BoolToStringConverter` | 可配置真假字符串，失败默认空字符串；不自动输出 `True`/`False` |
| `BoolToDoubleConverter` / `BoolToInt32Converter` | 可配置真假数值，初始均为零，失败也为零；不是默认的 1/0 映射 |
| `NullToTrueConverter` | 空值为真，其余有效输入为假；保留可配置映射属性 |
| `EqualsToVisibilityConverter` | 相等为 `Visible`，不等或空输入为 `Collapsed` |
| `AllBoolToVisibilityMultiConverter` | 全真为 `Visible`，全假、全空、混合和失败为 `Collapsed`；空集合为 `Visible` |
| `AllIsTrueMultiConverter` | 默认检查所有值是否等于 `true`，空集合为 `true`；保留比较值、参数和反转配置 |
| `BoolInverterConverter` | **仅接受 `bool`**：真变假，假变真；`null`、字符串、数字及其他对象返回 `DefaultResult` |
| `BoolToSplitThicknessConverter` | 按布尔条件从分隔参数选数值，生成四边相同的 `Thickness` |
| `MediaColorToSolidColorBrushConverter` | 正向 `Color → SolidColorBrush`，反向取颜色 |
| `SolidColorBrushToMediaColorConverter` | 正向 `SolidColorBrush → Color`，反向创建笔刷 |
| `MarginMultiConverter` | 多值依次设置左、上、右、下，可用 `HasRatio` 指定首值为比例 |

预设继承对应泛型家族的配置能力和空值规则。例如，`BoolToVisibilityConverter` 默认将空输入视为假分支；只有开启 `IsNullable` 才使用 `NullValue`。两个颜色转换器支持双向转换，创建独立且未冻结的笔刷；其他新增转换器不支持反向转换，调用时抛出 `NotSupportedException`。

在上述 WPF 命名空间下可以直接配置：

```xml
<c:BoolToVisibilityConverter x:Key="ShowWhenTrue" />
<c:BoolToSplitThicknessConverter x:Key="SelectedPadding" Separator=";" />
<c:MarginMultiConverter x:Key="ScaledMargin" HasRatio="True" />
```

```xml
<Border Padding="{Binding IsSelected, Converter={StaticResource SelectedPadding}, ConverterParameter='8;2;0', Mode=OneWay}">
    <Border.Margin>
        <MultiBinding Converter="{StaticResource ScaledMargin}" Mode="OneWay">
            <Binding Path="Scale" />
            <Binding Path="Left" />
            <Binding Path="Top" />
            <Binding Path="Right" />
            <Binding Path="Bottom" />
        </MultiBinding>
    </Border.Margin>
</Border>
```

**边距与兼容性约定：**

- 分段边距的参数分别对应真、假、空输入，默认分隔符为逗号；选中一个数值后四边统一设置。参数缺失、格式错误或非有限数值返回 `DefaultResult`。不提供旧初始化钩子或自定义字符串转换委托入口。
- 多值边距不是 XAML 的统一值/水平垂直简写：`[2] → (2,0,0,0)`，`[2,3] → (2,3,0,0)`。比例模式 `[2,1,3] → (2,6,0,0)`。空输入或只有比例时返回零边距，数值槽的 `null` 按零处理。
- 多值边距的哨兵、超长输入、普通转换错误、非有限值和比例溢出返回 `DefaultResult`，默认是零边距。显式设置为 `null`、`Binding.DoNothing` 或其他对象均有效；清除该依赖属性后恢复零边距。意外编程异常不被统一吞掉。
- 两种边距转换的数值解析遵循 `PreferredCulture`，不固定使用当前文化。逗号作为小数点时建议将分段分隔符设置为分号。
- 修复旧 `MediaColorToSolidColorBrushConverter` 的方向错误，不保留其原先与反向转换器重复的行为；错误类型或空值改为返回 `DefaultResult`，不再抛出旧的参数异常。
- 不新增旧 `BoolInverteConverter` / `BoolInverter` 名称。严格取反使用 `BoolInverterConverter`；旧“与 false 比较”的需求可使用 `EqualsConverter` 并设置 `Other=false`。本次不是旧 API 的完全兼容替换，原 HKW.WPF 项目及调用方不自动修改。

## Avalonia 用法

引用 `HKW.CommonValueConverters.Avalonia`。XAML 中使用 `using:HKW.CommonValueConverters.Avalonia` 命名空间，例如：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:c="using:HKW.CommonValueConverters.Avalonia">
    <UserControl.Resources>
        <c:BoolToValueConverter x:Key="StatusText"
            TrueValue="Ready" FalseValue="Waiting" />
    </UserControl.Resources>
    <TextBlock Text="{Binding IsReady, Converter={StaticResource StatusText}, Mode=OneWay}" />
</UserControl>
```

在启用编译绑定的应用中，应按应用约定配置 `x:DataType`；示例省略了应用 ViewModel。

两平台的字符串格式化多绑定用法相同，`c` 指向各自平台命名空间：

```xml
<TextBlock>
    <TextBlock.Text>
        <MultiBinding ConverterParameter="{}{0}: {1}">
            <MultiBinding.Converter>
                <c:StringFormatMultiConverter />
            </MultiBinding.Converter>
            <Binding Path="Name" />
            <Binding Path="Count" />
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

## 平台属性与转换契约

配置属性沿用核心名称但去掉 `Get` 前缀，例如 `TrueValue`、`IsInverted`、`Format`、`NumberType`。两平台的 `XxxProperty` 字段统一使用核心的 `CommonDependencyProperty<T>`：其只读 `object Value` 保存原生属性，WPF 底层仍为 `DependencyProperty`，Avalonia 底层仍为 `StyledProperty<T>`。封装不是新的属性引擎，不改变平台属性的有效值来源。`ConvertSplitValue` 保留 CLR 委托属性。仅核心支持反转的类型提供 `IsInverted`，不恢复旧库的额外行为。

### 通用属性访问

两个平台各自提供非泛型 `CommonDependencyProperty` 注册工厂，返回同一个核心 `CommonDependencyProperty<T>`。例如 WPF 自定义包装器内的字段声明：

```csharp
public static readonly HKW.CommonValueConverters.CommonDependencyProperty<bool> IsEnabledProperty =
    HKW.CommonValueConverters.WPF.CommonDependencyProperty.Register<MyConverter, bool>(
        nameof(IsEnabled), false);

public bool IsEnabled
{
    get => GetValue(IsEnabledProperty);
    set => SetValue(IsEnabledProperty, value);
}
```

上述片段放在继承 WPF `ConverterBase` 的 `MyConverter` 中；Avalonia 对应使用其平台的工厂和基类。`Register` 可省略默认值，此时使用原生属性系统对该类型的默认值。句柄本身只检查 `Value` 非空，平台访问入口负责验证原生属性类型及属性值类型是否与 `T` 一致；空句柄抛出 `ArgumentNullException`，错误平台或错误值类型抛出 `ArgumentException`。

```csharp
using HKW.CommonValueConverters.WPF;

var converter = new BoolToValueConverter<string>();
converter.SetValue(BoolToValueConverter<string>.TrueValueProperty, "Ready");
string value = converter.GetValue(BoolToValueConverter<string>.TrueValueProperty);
converter.SetCurrentValue(BoolToValueConverter<string>.TrueValueProperty, "Updated");
converter.ClearValue(BoolToValueConverter<string>.TrueValueProperty);
```

自定义平台包装器的对象配置属性可使用受保护的 `GetMappedValue` / `SetMappedValue`：前者读取并映射核心哨兵，后者将平台哨兵映射为核心值后写入本地配置。普通值和 `null` 保持不变。原有 `GetValue` / `SetValue` 仍保留原生属性语义，不自动映射哨兵。

WPF 的 `SetCurrentValue` 保留原生值来源语义；Avalonia 的 `SetValue` 保留优先级及其返回的可释放句柄：

```csharp
using Avalonia.Data;
using HKW.CommonValueConverters.Avalonia;

var converter = new BoolToValueConverter<string>();
using var styleValue = converter.SetValue(
    BoolToValueConverter<string>.TrueValueProperty, "Styled", BindingPriority.Style);
converter.TrueValue = "Local";
converter.ClearValue(BoolToValueConverter<string>.TrueValueProperty); // 恢复 Styled
// 释放 styleValue 后移除该优先级来源.
```

需要元数据覆盖、绑定或其他原生 API 时显式解包，原生 API 仍然可用：

```csharp
System.Windows.DependencyProperty wpfProperty =
    (System.Windows.DependencyProperty)HKW.CommonValueConverters.WPF.ConverterBase.DefaultResultProperty.Value;

Avalonia.StyledProperty<object?> avaloniaProperty =
    (Avalonia.StyledProperty<object?>)HKW.CommonValueConverters.Avalonia.ConverterBase.DefaultResultProperty.Value;
```

**兼容性注意**：`XxxProperty` 字段从原生类型改为共享封装类型，是源代码及二进制兼容性变化。依赖这些字段的代码需要重新编译，原生元数据/绑定 API、字段反射及 XAML 静态属性引用可能需要调整；不提供隐式平台转换或第二套原生字段。普通 CLR 配置属性名称和类型保持不变。

- **默认结果**：不设置 `DefaultResult` 时保留核心默认值，通常为动态的 `CommonConverterBase.GlobalDefaultResult`，`EnumEqualsConverter` 则为 `false`。显式设置 `DefaultResult = null` 表示合法空回退，不表示继承默认值。
- **文化**：不设置 `PreferredCulture` 时遵循动态全局偏好；设置实例属性后覆盖全局。两平台可用 `ClearValue(ConverterBase.PreferredCultureProperty)` 清除本地覆盖；默认结果同理用 `ClearValue(ConverterBase.DefaultResultProperty)`。若仍存在其他有效平台属性来源，清除本地值后以该来源为准。
- **线程与实例**：每个包装器独占核心，应在所属 UI 线程创建和使用，不跨线程共享捕获 UI 属性的实例；不提供全局单例。
- **刷新**：修改平台属性后，下一次转换读取新值，但属性变化**不自动保证重新触发绑定转换**；需要时由应用刷新绑定。
- **方向**：单值反向能力遵循核心，不支持时抛出 `NotSupportedException`。WPF 多值反向桥接按项映射结果，但内置多值核心目前不实现反向；Avalonia 多值接口仅提供正向。
- **类型**：接口调用与直接调用行为相同，不使用旧 `ResultToString`，不会把普通结果强制转字符串。
- **分段边距**: `BoolToSplitThicknessConverter` 使用本次转换的有效文化解析分量, 非法或非有限数值返回 `DefaultResult`. `MarginMultiConverter` 的两槽输入保留左/上两项, 缺失方向为零; 缩放结果溢出也返回默认结果.

包装器自动双向映射以下哨兵，调用平台包装器时无需手动转换：

| 核心 | WPF | Avalonia |
|---|---|---|
| `ConverterResults.UnsetValue` | `DependencyProperty.UnsetValue` | `AvaloniaProperty.UnsetValue` |
| `ConverterResults.DoNothing` | `Binding.DoNothing` | `BindingOperations.DoNothing`（`Avalonia.Data`） |

两个平台不会互相覆盖核心哨兵。映射覆盖输入、参数、多值槽位、可容纳对象的配置和结果；不会修改调用方多值集合，也不会递归改写业务对象内部的数据。对象类型配置的 CLR setter 会先将原生哨兵存为核心哨兵，getter 再映射回平台值，避免被属性系统消耗。直接使用 `SetValue`、样式或 XAML 设置哨兵时，应使用核心 `ConverterResults.UnsetValue` / `ConverterResults.DoNothing`：原生 `UnsetValue` 可能表示清除属性，Avalonia 的原生 `DoNothing` 会被视为忽略赋值。重置配置请显式调用 `ClearValue`。

测试范围包括核心转换器单元测试, 以及两个平台每个转换器的直接行为、真实 Binding/MultiBinding 和编译 XAML 集成用例. WPF 使用 STA Dispatcher, Avalonia 使用 Headless UI 会话, 验证初值、源通知更新及支持的双向转换. 泛型与非泛型类型分别覆盖; 泛型 XAML 使用不重写转换逻辑的测试专用封闭子类. 测试不弹出桌面窗口, 不验证截图、像素渲染或操作系统交互. 上面的 XAML 仍是使用示例, 实际集成验证以测试项目内的编译资源为准.

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
| 使用原生 `XxxProperty` 字段 | 改为核心 `CommonDependencyProperty<T>`；平台强类型访问可直接传入，原生 API 使用 `.Value` 解包 |
| 通过核心接口调用 `GetValue` / `SetValue` | 使用平台 `ConverterBase` 的强类型访问重载；核心 `ICommonValueConverter` 仍仅保留文化偏好契约 |
| 失败结果默认 `null` | 默认核心 `UnsetValue`；需要旧行为时显式设置 `GetDefaultResult = () => null` |
| `SelectCulture(() => culture)` | `SelectCulture(culture)`；公共转换入口会自动选择文化 |
| 依赖错误数量导致异常 | 普通输入失败返回默认结果；不支持的反向转换仍抛异常 |
| 可空反转改变空分支、相等多绑定认为两个 null 不相等 | 空分支独立，两个 null 相等 |
| 反向解析依赖进程文化 | 使用统一有效文化，先精确格式再宽松解析 |

源码中全注释的 `ValueConverterGroup`、`StringToObjectConverter` 不属于已提供的功能。

### 从旧平台库迁移

- 将旧 `HKW.WPF.Converters` / `HKW.HKWAvalonia.Converters` 命名空间替换为对应的新平台命名空间。
- 直接实例化转换器，不再使用静态 `Instance` 或可写 `CommonValueConverter`。通用属性句柄由核心统一定义，注册和类型验证由对应平台负责。
- 旧 Avalonia 布尔转换、旧库额外反向行为不保留，按当前核心契约选择绑定方向。
- 不迁入旧库的 Brush、Thickness、Visibility 专有转换器；简单条件选值可使用非泛型 `BoolToValueConverter` 配置实际平台值。

## 验证与本地打包

使用 .NET 10 SDK 和现有 MSTest/VSTest 配置。完整解决方案和 WPF 测试需要 Windows；Avalonia 项目本身不依赖 WPF。

核心测试中的 [ConverterTestCoverageTests](HKW.CommonValueConverters.Tests/ConverterTestCoverageTests.cs) 检查核心程序集, 两个平台测试项目则分别检查各自平台程序集的非抽象单值和多值转换器, 包含泛型及间接子类. 对应测试必须是当前测试程序集内未标记 `Ignore` 的 MSTest 测试类中的有效测试方法, 类和方法上的 `Ignore` 均不计入. 方法名区分大小写, 可以是 `<转换器名称>` 或 `<转换器名称>_<非空测试内容>`, 泛型名称不含类型参数, 每个转换器至少匹配一个测试方法即可. 缺失时按组列出所有缺失类型并抛出 `InvalidOperationException`, 使检查失败; 命名不符的已有测试也会被报告为缺失. 命名检查不等于代码覆盖率; 平台测试另以类型身份核对行为、Binding 和 XAML 用例矩阵, 防止同名泛型/非泛型类型互相掩盖遗漏.

平台 UI 测试必须通过各自的线程调度辅助执行, 不跨线程复用转换器或控件. WPF 测试需要 Windows Desktop runtime; Avalonia 测试依赖与生产库版本一致的 `Avalonia.Headless` (12.1.5), 保留 MSTest 运行入口. 每个用例独立创建状态并清理绑定, 全局文化或配置变更须恢复. 真实绑定及编译 XAML 用例随普通 `dotnet test` 自动执行, 无需单独启动桌面应用.

```powershell
dotnet restore HKW.CommonValueConverters.sln
dotnet test HKW.CommonValueConverters.Tests\HKW.CommonValueConverters.Tests.csproj --no-restore
dotnet test HKW.CommonValueConverters.WPF.Tests\HKW.CommonValueConverters.WPF.Tests.csproj --no-restore
dotnet test HKW.CommonValueConverters.Avalonia.Tests\HKW.CommonValueConverters.Avalonia.Tests.csproj --no-restore
dotnet build HKW.CommonValueConverters.sln --configuration Release --no-restore
```

单独构建平台库：

```powershell
dotnet build HKW.CommonValueConverters.WPF\HKW.CommonValueConverters.WPF.csproj --configuration Release
dotnet build HKW.CommonValueConverters.Avalonia\HKW.CommonValueConverters.Avalonia.csproj --configuration Release
```

三个库均按 `GeneratePackageOnBuild` 配置在各自 `bin\Release` 生成 NuGet 包，测试项目不打包。本地构建不会发布；现有标签发布工作流保持不变，仍只发布核心包。
