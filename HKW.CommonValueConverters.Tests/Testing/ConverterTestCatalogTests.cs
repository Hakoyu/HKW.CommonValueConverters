using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest.Testing;

[TestClass]
public sealed class ConverterTestCatalogTests
{
    [TestMethod]
    public void MissingLayersAndInactiveEntriesAreRejected()
    {
        var type = typeof(BoolInverterConverter);
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [type],
                [new(type, ConverterTestLayer.Behavior, "ignored", false)],
                [ConverterTestLayer.Behavior, ConverterTestLayer.Binding]
            )
        );
        StringAssert.Contains(error.Message, "Inactive entry:");
        StringAssert.Contains(error.Message, "/Behavior");
        StringAssert.Contains(error.Message, "/Binding");
    }

    [TestMethod]
    public void MultipleCasesAreAllowedButDuplicateIdsAreRejected()
    {
        var type = typeof(BoolInverterConverter);
        ConverterTestDescriptor[] cases =
        [
            new(type, ConverterTestLayer.Behavior, "first"),
            new(type, ConverterTestLayer.Behavior, "second"),
        ];
        ConverterTestCatalog.Validate([type], cases, [ConverterTestLayer.Behavior]);
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [type],
                [.. cases, cases[0]],
                [ConverterTestLayer.Behavior]
            )
        );
        StringAssert.Contains(error.Message, "Duplicate case:");
    }

    [TestMethod]
    public void ClosedGenericsCoverTheirDefinitionNotTheNonGenericIdentity()
    {
        ConverterTestDescriptor[] cases =
        [
            new(typeof(GenericFixture<string>), ConverterTestLayer.Behavior, "string"),
        ];
        ConverterTestCatalog.Validate(
            [typeof(GenericFixture<>)],
            cases,
            [ConverterTestLayer.Behavior]
        );
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [typeof(GenericFixture<>), typeof(GenericFixture)],
                cases,
                [ConverterTestLayer.Behavior]
            )
        );
        StringAssert.Contains(error.Message, typeof(GenericFixture).FullName!);
    }

    [TestMethod]
    public void ExceptionsRequireReasonsAndCannotOutliveTheirTypes()
    {
        var type = typeof(BoolInverterConverter);
        ConverterTestDescriptor[] cases = [new(type, ConverterTestLayer.Behavior, "valid")];
        ConverterTestCatalog.Validate(
            [type],
            cases,
            [ConverterTestLayer.Behavior, ConverterTestLayer.Binding],
            [
                new(
                    type,
                    ConverterTestLayer.Binding,
                    "Compatibility type has no platform binding interface."
                ),
            ]
        );
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [type],
                cases,
                [ConverterTestLayer.Behavior],
                [new(type, ConverterTestLayer.Binding, "")]
            )
        );
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [type],
                cases,
                [ConverterTestLayer.Behavior],
                [new(typeof(GenericFixture), ConverterTestLayer.Binding, "Removed type.")]
            )
        );
        StringAssert.Contains(error.Message, "Stale exception:");
    }

    [TestMethod]
    public void PlatformParityRequiresMatchingPlatformConvertersOrDocumentedExceptions()
    {
        ConverterTestCatalog.ValidatePlatformParity(
            [typeof(BoolInverterConverter), typeof(BoolToValueConverter<int>)],
            [typeof(BoolInverterConverter), typeof(BoolToValueConverter<string>)],
            "HKW.CommonValueConverters.WPF"
        );
        var missing = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ValidatePlatformParity(
                [typeof(BoolInverterConverter), typeof(BoolToValueConverter<>)],
                [typeof(BoolInverterConverter)],
                "HKW.CommonValueConverters.WPF"
            )
        );
        StringAssert.Contains(missing.Message, "Missing platform converter:");
        StringAssert.Contains(
            missing.Message,
            "HKW.CommonValueConverters.WPF.BoolToValueConverter<>"
        );
        ConverterTestCatalog.ValidatePlatformParity(
            [typeof(BoolInverterConverter), typeof(BoolToValueConverter<>)],
            [typeof(BoolInverterConverter)],
            "HKW.CommonValueConverters.WPF",
            [new(typeof(BoolToValueConverter<>), "Handled by specialized platform converter.")]
        );
        var emptyReason = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ValidatePlatformParity(
                [typeof(BoolInverterConverter)],
                [typeof(BoolInverterConverter)],
                "HKW.CommonValueConverters.WPF",
                [new(typeof(BoolInverterConverter), "")]
            )
        );
        StringAssert.Contains(emptyReason.Message, "Platform exception requires a reason:");
        var stale = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ValidatePlatformParity(
                [typeof(BoolInverterConverter)],
                [typeof(BoolInverterConverter)],
                "HKW.CommonValueConverters.WPF",
                [new(typeof(GenericFixture), "Removed core converter.")]
            )
        );
        StringAssert.Contains(stale.Message, "Stale platform exception:");
    }

    [TestMethod]
    public void ActualDynamicDataIsReadAndEmptyOrMissingProvidersFail()
    {
        var entries = ConverterTestCatalog.ReadCases(
            typeof(ProviderFixture).GetMethod(nameof(ProviderFixture.Valid))!,
            true
        );
        CollectionAssert.AreEqual(
            new[] { "actual-first", "actual-second" },
            entries.Select(entry => entry.CaseId).ToArray()
        );
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ReadCases(
                typeof(ProviderFixture).GetMethod(nameof(ProviderFixture.Empty))!,
                true
            )
        );
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ReadCases(
                typeof(ProviderFixture).GetMethod(nameof(ProviderFixture.Missing))!,
                true
            )
        );
        var ignored = ConverterTestCatalog.ReadCases(
            typeof(ProviderFixture).GetMethod(nameof(ProviderFixture.Ignored))!,
            true
        );
        Assert.IsTrue(ignored.All(entry => entry.Active is false));
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ReadCases(
                typeof(ProviderFixture).GetMethod(nameof(ProviderFixture.Throwing))!,
                true
            )
        );
        StringAssert.Contains(error.Message, nameof(ProviderFixture.Throwing));
        Assert.IsNotNull(error.InnerException);
    }

    [TestMethod]
    public void EmptyDiscoveryAndEmptyCaseIdsFail()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate([], [], [ConverterTestLayer.Behavior])
        );
        var type = typeof(BoolInverterConverter);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.Validate(
                [type],
                [new(type, ConverterTestLayer.Behavior, "")],
                [ConverterTestLayer.Behavior]
            )
        );
    }

    [TestMethod]
    [DataRow(nameof(ProviderFixture.ReturnsInt))]
    [DataRow(nameof(ProviderFixture.ReturnsTaskOfInt))]
    [DataRow(nameof(ProviderFixture.ReturnsAsyncVoid))]
    public void UnsupportedReturnTypesCannotContributeCoverage(string methodName)
    {
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ReadCases(typeof(ProviderFixture).GetMethod(methodName)!, true)
        );
        StringAssert.Contains(error.Message, "Unsupported test return type:");
        StringAssert.Contains(error.Message, methodName);
    }

    [TestMethod]
    [DataRow(nameof(ProviderFixture.TooFewArguments), "count mismatch")]
    [DataRow(nameof(ProviderFixture.TooManyArguments), "count mismatch")]
    [DataRow(nameof(ProviderFixture.IncompatibleArgument), "type mismatch")]
    [DataRow(nameof(ProviderFixture.NullValueTypeArgument), "type mismatch")]
    public void NonExecutableDataRowsFailWithTheirEntryName(string methodName, string expectedError)
    {
        var error = Assert.ThrowsExactly<InvalidOperationException>(() =>
            ConverterTestCatalog.ReadCases(typeof(ProviderFixture).GetMethod(methodName)!, true)
        );
        StringAssert.Contains(error.Message, expectedError);
        StringAssert.Contains(error.Message, methodName);
    }

    [TestMethod]
    [DataRow(nameof(ProviderFixture.ReturnsTask))]
    [DataRow(nameof(ProviderFixture.ReturnsValueTask))]
    [DataRow(nameof(ProviderFixture.NullableArgument))]
    [DataRow(nameof(ProviderFixture.ReferenceArgument))]
    public void SupportedAsyncAndNullableSignaturesKeepTheirCases(string methodName)
    {
        var entries = ConverterTestCatalog.ReadCases(
            typeof(ProviderFixture).GetMethod(methodName)!,
            true
        );
        Assert.IsNotEmpty(entries);
        Assert.IsTrue(entries.All(entry => entry.Active));
    }

    private sealed class GenericFixture;

    private sealed class GenericFixture<T>;

    private sealed record Case(string Id) : IConverterCase;

    // 普通提供者样例不声明 TestClass, 避免负向场景进入真实测试发现.
#pragma warning disable MSTEST0018, MSTEST0007
    public sealed class ProviderFixture
    {
        public static IEnumerable<object[]> Cases =>
            [
                [new Case("actual-first")],
                [new Case("actual-second")],
            ];
        public static IEnumerable<object[]> EmptyCases => [];

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void Valid(IConverterCase scenario) => Assert.IsNotNull(scenario);

        [DynamicData(nameof(EmptyCases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void Empty(IConverterCase scenario) => Assert.IsNotNull(scenario);

        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void Missing() { }

        [Ignore]
        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void Ignored(IConverterCase scenario) => Assert.IsNotNull(scenario);

        public static IEnumerable<object[]> ThrowingCases =>
            throw new InvalidOperationException("Provider failure.");

        [DynamicData(nameof(ThrowingCases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void Throwing(IConverterCase scenario) => Assert.IsNotNull(scenario);

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public int ReturnsInt(IConverterCase scenario) => 1;

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public Task<int> ReturnsTaskOfInt(IConverterCase scenario) => Task.FromResult(1);

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public async void ReturnsAsyncVoid(IConverterCase scenario) => await Task.CompletedTask;

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public Task ReturnsTask(IConverterCase scenario) => Task.CompletedTask;

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public ValueTask ReturnsValueTask(IConverterCase scenario) => ValueTask.CompletedTask;

        public static IEnumerable<object[]> NoArguments => [Array.Empty<object>()];
        public static IEnumerable<object?[]> NullArgument =>
            [
                [new Case("null"), null],
            ];

        [DynamicData(nameof(NoArguments))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void TooFewArguments(IConverterCase scenario) { }

        [DynamicData(nameof(NullArgument))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void TooManyArguments(IConverterCase scenario) { }

        [DynamicData(nameof(Cases))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void IncompatibleArgument(string scenario) { }

        [DynamicData(nameof(NullArgument))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void NullValueTypeArgument(IConverterCase scenario, int value) { }

        [DynamicData(nameof(NullArgument))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void NullableArgument(IConverterCase scenario, int? value) { }

        [DynamicData(nameof(NullArgument))]
        [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
        public void ReferenceArgument(IConverterCase scenario, string? value) { }
    }
#pragma warning restore MSTEST0018, MSTEST0007
}
