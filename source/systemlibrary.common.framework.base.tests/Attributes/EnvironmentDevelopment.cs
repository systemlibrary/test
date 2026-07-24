namespace SystemLibrary.Common.Framework.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class TestMethodInUnknownEnvironmentAttribute : TestMethodAttribute
{
    private readonly string _envVar;

    public TestMethodInUnknownEnvironmentAttribute(string envVar = "ASPNETCORE_ENVIRONMENT")
    {
        _envVar = envVar;
    }

    public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
    {
        var value = Environment.GetEnvironmentVariable(_envVar);

        var names = typeof(EnvironmentType).GetEnumNames();
        
        if (value.IsNot() || names.Contains(value, StringComparer.OrdinalIgnoreCase))
        {
            return
            [
                new TestResult
                    {
                        Outcome = UnitTestOutcome.Inconclusive,
                        TestFailureException = new AssertInconclusiveException(
                            $"Skipped: set {_envVar} to an unknown environment name. Value '{value}' is unset or matches a Key in EnvironmentType.")
                    }
            ];
        }

        return await base.ExecuteAsync(testMethod);
    }
}
