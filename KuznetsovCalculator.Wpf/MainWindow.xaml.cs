using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KuznetsovCalculator.Wpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel model = new();
    public MainWindow()
    {
        InitializeComponent();
        DataContext = model;
        Loaded += (_, _) => ExpressionBox.Focus();
    }
    private void Input_Click(object sender, RoutedEventArgs e) =>
        model.Input(((Button)sender).Content.ToString()!);
    private void Evaluate_Click(object sender, RoutedEventArgs e) => model.Evaluate();
    private void Clear_Click(object sender, RoutedEventArgs e) => model.Clear();
    private void Backspace_Click(object sender, RoutedEventArgs e) => model.Backspace();
    private void ClearHistory_Click(object sender, RoutedEventArgs e) => model.ClearHistory();
    private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HistoryList.SelectedItem is HistoryEntry entry)
        {
            model.Restore(entry);
            HistoryList.SelectedIndex = -1;
            ExpressionBox.Focus();
            ExpressionBox.CaretIndex = ExpressionBox.Text.Length;
        }
    }
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { model.Evaluate(); e.Handled = true; }
        else if (e.Key == Key.Escape) { model.Clear(); e.Handled = true; }
        else if (e.Key == Key.Back && !ExpressionBox.IsKeyboardFocused)
        { model.Backspace(); e.Handled = true; }
    }
    private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (e.Text == "=") { model.Evaluate(); e.Handled = true; return; }
        if (ExpressionBox.IsKeyboardFocused && !model.HasResult) return;
        if (e.Text.All(c => "0123456789+-*/^().,".Contains(c)))
        {
            model.Input(e.Text);
            e.Handled = true;
            ExpressionBox.Focus();
            ExpressionBox.CaretIndex = ExpressionBox.Text.Length;
        }
    }
}
