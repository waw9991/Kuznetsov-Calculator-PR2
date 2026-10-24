namespace KuznetsovCalculator.Core;

/// <summary>Пять арифметических операций над действительными числами.</summary>
public class Calculator
{
    public double Add(double a, double b) => Check(a, b, a + b);
    public double Subtract(double a, double b) => Check(a, b, a - b);
    public double Multiply(double a, double b) => Check(a, b, a * b);

    public double Divide(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException("Деление на ноль невозможно.");
        return Check(a, b, a / b);
    }

    public double Power(double a, double b) => Check(a, b, Math.Pow(a, b));

    public double Calculate(double a, char operation, double b) => operation switch
    {
        '+' => Add(a, b),
        '-' => Subtract(a, b),
        '*' => Multiply(a, b),
        '/' => Divide(a, b),
        '^' => Power(a, b),
        _ => throw new ArgumentException("Неизвестная операция.", nameof(operation))
    };

    private static double Check(double a, double b, double result)
    {
        if (!double.IsFinite(a) || !double.IsFinite(b) || !double.IsFinite(result))
            throw new ArithmeticException(
                "Результат не является конечным действительным числом.");
        return result;
    }
}
