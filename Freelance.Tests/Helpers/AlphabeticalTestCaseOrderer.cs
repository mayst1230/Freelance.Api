using Xunit.Abstractions;
using Xunit.Sdk;

namespace Freelance.Tests.Helpers;

/// <summary>
/// Сортировка тестов по имени метода.
/// </summary>
/// <remarks>Использовать только когда важен порядок проведения тестов в рамках класса!</remarks>
public class AlphabeticalTestCaseOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        => testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
}
