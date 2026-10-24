using KuznetsovCalculator.Wpf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KuznetsovCalculator.Tests;

[TestClass]
public class MainViewModelTests
{
    [TestMethod]
    public void Evaluate_Success_AddsHistory()
    {
        var model = new MainViewModel { Expression = "8+2*5" };
        model.Evaluate();
        Assert.AreEqual("18", model.Result);
        Assert.AreEqual("8+2*5", model.History[0].Expression);
        Assert.AreEqual("", model.Error);
    }
    [TestMethod]
    public void Evaluate_Error_DoesNotAddHistory()
    {
        var model = new MainViewModel { Expression = "2+3" };
        model.Evaluate();
        Assert.AreEqual("5", model.Result);
        model.Expression = "5/0";
        model.Evaluate();
        Assert.HasCount(1, model.History);
        Assert.AreEqual("2+3", model.History[0].Expression);
        Assert.AreEqual("—", model.Result);
        Assert.IsFalse(model.HasResult);
        StringAssert.Contains(model.Error, "ноль");
    }
    [TestMethod]
    public void Input_DigitAfterResult_StartsNewExpression()
    {
        var model = new MainViewModel { Expression = "3+4" };
        model.Evaluate(); model.Input("9");
        Assert.AreEqual("9", model.Expression);
    }
    [TestMethod]
    public void Input_OperatorAfterResult_ContinuesCalculation()
    {
        var model = new MainViewModel { Expression = "3+4" };
        model.Evaluate(); model.Input("*"); model.Input("2"); model.Evaluate();
        Assert.AreEqual("14", model.Result);

        model.Expression = "2-4";
        model.Evaluate(); model.Input("^"); model.Input("2"); model.Evaluate();
        Assert.AreEqual("4", model.Result,
            "Continuation must preserve the negative result as a single operand.");
    }
    [TestMethod]
    public void Input_FractionalResult_ContinuesCalculation()
    {
        var model = new MainViewModel { Expression = "1/4" };
        model.Evaluate(); model.Input("+"); model.Input("1"); model.Evaluate();
        Assert.AreEqual("1,25", model.Result);
    }
    [TestMethod]
    public void Clear_PreservesHistory()
    {
        var model = new MainViewModel { Expression = "2+2" };
        model.Evaluate(); model.Clear();
        Assert.AreEqual("", model.Expression);
        Assert.AreEqual("0", model.Result);
        Assert.HasCount(1, model.History);
    }
    [TestMethod]
    public void History_LimitsToTwentyEntries()
    {
        var model = new MainViewModel();
        for (int i = 0; i < 25; i++) { model.Expression = i.ToString(); model.Evaluate(); }
        Assert.HasCount(20, model.History);
        Assert.AreEqual("24", model.History[0].Expression);
        Assert.AreEqual("5", model.History[^1].Expression);
    }
    [TestMethod]
    public void Restore_HistoryEntry_UsesOriginalExpression()
    {
        var model = new MainViewModel { Expression = "4^3" };
        model.Evaluate(); var entry = model.History[0]; model.Clear(); model.Restore(entry);
        Assert.AreEqual("4^3", model.Expression);
    }
    [TestMethod]
    public void Backspace_EmptyAndNonemptyInput_Works()
    {
        var model = new MainViewModel { Expression = "12" };
        model.Backspace(); Assert.AreEqual("1", model.Expression);
        model.Clear(); model.Backspace(); Assert.AreEqual("", model.Expression);
    }
    [TestMethod]
    public void ClearHistory_RemovesEntries()
    {
        var model = new MainViewModel { Expression = "1+1" };
        model.Evaluate(); model.ClearHistory();
        Assert.IsEmpty(model.History);
    }
    [TestMethod]
    public void EditingAfterError_ClearsMessage()
    {
        var model = new MainViewModel { Expression = "1/0" };
        model.Evaluate(); model.Expression = "1/2"; model.Evaluate();
        Assert.AreEqual("", model.Error); Assert.AreEqual("0,5", model.Result);
    }
}
