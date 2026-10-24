using System.Globalization;

namespace KuznetsovCalculator.Core;

/// <summary>Разбор выражения по уровням приоритета операций.</summary>
public class ExpressionCalculator
{
    public double Calculate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new FormatException("Введите выражение.");
        if (expression.Length > 256)
            throw new FormatException("Выражение должно содержать не более 256 символов.");
        return new Parser(expression).Parse();
    }

    private sealed class Parser(string text)
    {
        private readonly Calculator calculator = new();
        private int position;

        public double Parse()
        {
            double result = Sum();
            SkipSpaces();
            if (position != text.Length)
                throw Error("Лишний символ или отсутствует оператор");
            return Math.Round(result, 10);
        }

        private double Sum()
        {
            double value = Product();
            while (true)
            {
                if (Take('+')) value = calculator.Add(value, Product());
                else if (Take('-')) value = calculator.Subtract(value, Product());
                else return value;
            }
        }

        private double Product()
        {
            double value = Unary();
            while (true)
            {
                if (Take('*')) value = calculator.Multiply(value, Unary());
                else if (Take('/')) value = calculator.Divide(value, Unary());
                else return value;
            }
        }

        private double Unary()
        {
            if (Take('+')) return Unary();
            if (Take('-')) return -Unary();
            return Power();
        }

        private double Power()
        {
            double value = Atom();
            // Правый операнд читает следующую степень: 2^3^2 = 2^(3^2).
            return Take('^') ? calculator.Power(value, Unary()) : value;
        }

        private double Atom()
        {
            if (Take('('))
            {
                double value = Sum();
                if (!Take(')')) throw Error("Ожидается закрывающая скобка");
                return value;
            }

            SkipSpaces();
            int start = position;
            while (position < text.Length && char.IsAsciiDigit(text[position])) position++;
            if (position < text.Length && text[position] is '.' or ',')
            {
                position++;
                while (position < text.Length && char.IsAsciiDigit(text[position]))
                    position++;
            }
            string number = text[start..position].Replace(',', '.');
            if (!double.TryParse(number, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double parsed)
                || !double.IsFinite(parsed))
                throw Error("Ожидается корректное число");
            return parsed;
        }

        private bool Take(char symbol)
        {
            SkipSpaces();
            if (position >= text.Length || text[position] != symbol) return false;
            position++;
            return true;
        }

        private void SkipSpaces()
        {
            while (position < text.Length && char.IsWhiteSpace(text[position])) position++;
        }

        private FormatException Error(string message) =>
            new($"{message} (позиция {position + 1}).");
    }
}
