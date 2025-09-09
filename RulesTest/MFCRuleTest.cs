namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class MFCRuleTest : RuleTest
{
    #region ReplaceRUNTIME_CLASS
    [TestMethod]
    [DataRow("\tASSERT(psto->IsKindOf(RUNTIME_CLASS(CDsStorage)));\r\n", "\tDebug.Assert(psto.IsKindOf(Afx.RUNTIME_CLASS(typeof(CDsStorage))));\r\n")]
    public void ReplaceRUNTIME_CLASSApplyAll(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion
}
