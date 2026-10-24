using KuznetsovCalculator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KuznetsovCalculator.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    [DataRow(8.5, '+', 1.5, 10.0)]
    [DataRow(3.0, '-', 8.0, -5.0)]
    [DataRow(-4.0, '*', 2.5, -10.0)]
    [DataRow(0.0, '*', 25.0, 0.0)]
    [DataRow(9.0, '/', 4.0, 2.25)]
    [DataRow(3.0, '^', 4.0, 81.0)]
    [DataRow(4.0, '^', -1.0, 0.25)]
    [DataRow(5.0, '^', 0.0, 1.0)]
    public void Calculate_ValidArguments_ReturnsExpected(double a, char operation, double b, double expected)
    {
        Assert.AreEqual(expected, new Calculator().Calculate(a, operation, b), 0.000000001);
    }

    [TestMethod]
    public void Divide_Zero_Throws() => Assert.ThrowsExactly<DivideByZeroException>(
        () => new Calculator().Divide(7, 0));

    [TestMethod]
    public void Calculate_UnknownOperator_Throws() => Assert.ThrowsExactly<ArgumentException>(
        () => new Calculator().Calculate(2, '%', 3));

    [TestMethod]
    public void Power_NonRealResult_Throws() => Assert.ThrowsExactly<ArithmeticException>(
        () => new Calculator().Power(-4, 0.5));

    [TestMethod]
    public void Multiply_Overflow_Throws() => Assert.ThrowsExactly<ArithmeticException>(
        () => new Calculator().Multiply(double.MaxValue, 2));
}
