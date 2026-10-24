using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using KuznetsovCalculator.Core;

namespace KuznetsovCalculator.Wpf;

public record HistoryEntry(string Expression, string Result)
{
    public string Display => $"{Expression} = {Result}";
}

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ExpressionCalculator calculator = new();
    private string expression = "";
    private string result = "0";
    private string error = "";
    private bool evaluated;

    public event PropertyChangedEventHandler? PropertyChanged;
    public bool HasResult => evaluated;
    public ObservableCollection<HistoryEntry> History { get; } = [];
    public string Expression
    {
        get => expression;
        set { expression = value; evaluated = false; Error = ""; Changed(); }
    }
    public string Result { get => result; private set { result = value; Changed(); } }
    public string Error { get => error; private set { error = value; Changed(); } }

    public void Input(string token)
    {
        string next = Expression;
        if (evaluated)
            next = token is "+" or "-" or "*" or "/" or "^"
                ? (Result.StartsWith('-') ? $"({Result})" : Result)
                : "";
        if (next.Length + token.Length <= 256) Expression = next + token;
    }

    public void Evaluate()
    {
        try
        {
            double value = calculator.Calculate(Expression);
            Result = value.ToString("0.##########", CultureInfo.InvariantCulture).Replace('.', ',');
            History.Insert(0, new HistoryEntry(Expression, Result));
            if (History.Count > 20) History.RemoveAt(20);
            Error = "";
            evaluated = true;
        }
        catch (Exception exception) when (exception is FormatException or ArithmeticException)
        {
            Result = "—";
            Error = exception.Message;
            evaluated = false;
        }
    }

    public void Clear() { Expression = ""; Result = "0"; }
    public void Backspace()
    {
        if (Expression.Length > 0) Expression = Expression[..^1];
    }
    public void Restore(HistoryEntry entry) => Expression = entry.Expression;
    public void ClearHistory() => History.Clear();
    private void Changed([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
