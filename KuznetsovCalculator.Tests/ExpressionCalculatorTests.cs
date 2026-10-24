using KuznetsovCalculator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KuznetsovCalculator.Tests;

[TestClass]
public class ExpressionCalculatorTests
{
    [TestMethod]
    [DataRow("8+2*5", 18.0)]
    [DataRow("(8+2)*5", 50.0)]
    [DataRow("2^3^2", 512.0)]
    [DataRow("-3^2", -9.0)]
    [DataRow("(-3)^2", 9.0)]
    [DataRow("2^-3", 0.125)]
    [DataRow("2^(-2)", 0.25)]
    [DataRow("0,1+0.2", 0.3)]
    [DataRow("18/3/2", 3.0)]
    [DataRow("9-4-2", 3.0)]
    [DataRow("(2+(3*4))", 14.0)]
    [DataRow(" 6 + 2 ", 8.0)]
    [DataRow("3*-2", -6.0)]
    [DataRow("--5", 5.0)]
    [DataRow(".5+0,25", 0.75)]
    [DataRow("4^0.5", 2.0)]
    public void Calculate_Expression_ReturnsExpected(string expression, double expected) =>
        Assert.AreEqual(expected, new ExpressionCalculator().Calculate(expression), 0.000000001);

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("3+")]
    [DataRow("(2+3")]
    [DataRow("2+3)")]
    [DataRow("2(3)")]
    [DataRow("1,,2")]
    [DataRow("abc")]
    [DataRow("()")]
    [DataRow("1 2")]
    public void Calculate_InvalidExpression_Throws(string expression) =>
        Assert.ThrowsExactly<FormatException>(() => new ExpressionCalculator().Calculate(expression));

    [TestMethod]
    public void Calculate_DivisionByExpressionZero_Throws() => Assert.ThrowsExactly<DivideByZeroException>(
        () => new ExpressionCalculator().Calculate("7/(3-3)"));

    [TestMethod]
    public void Calculate_TooLong_Throws() => Assert.ThrowsExactly<FormatException>(
        () => new ExpressionCalculator().Calculate(new string('1', 257)));
}
