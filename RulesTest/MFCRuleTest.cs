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

    #region ReplaceASSERT_AfxIsValidAddress
    [TestMethod]
    [DataRow("\tASSERT(AfxIsValidAddress(p, sizeof (CClass)));\r\n", "\tDebug.Assert(p != null && p is CClass);\r\n")]
    public void ReplaceASSERT_AfxIsValidAddress(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion
}
