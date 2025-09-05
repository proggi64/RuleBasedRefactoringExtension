using System.Text;
using Webkasi.Refactoring.Rules;

namespace RuleBaseWebkasi.Refactoring.Rules;

[TestClass]
public sealed class RuleTest
{
    public TestContext TestContext { get; set; }
    private RuleSet? ruleSet = null;

    [TestInitialize]
    public void Initialize()
    {
        bool first = true;
        var rulesFiles = Directory.GetFiles(TestContext.DeploymentDirectory! + "../../../../Rules/", "*.json");
        foreach (var rulesFile in rulesFiles)
        {
            string json = File.ReadAllText(rulesFile);
            if (first)
            {
                ruleSet = RuleSetLoader.LoadFromJson(json);
                first = false;
            }
            else
            {
                foreach (var rule in RuleSetLoader.LoadFromJson(json))
                {
                    ruleSet!.Add(rule.Key, rule.Value);
                }
            }
        }
    }

    #region ReplaceBOOL
    [TestMethod]
    [DataRow("BOOL x = TRUE;", "bool x = TRUE;")]
    [DataRow("\tBOOL x = TRUE;", "\tbool x = TRUE;")]
    [DataRow(" BOOL x = TRUE;", " bool x = TRUE;")]
    [DataRow("void Func(BOOL xy) {", "void Func(bool xy) {")]
    [DataRow("void Func(BOOL xy, BOOL wert) {", "void Func(bool xy, bool wert) {")]
    public void ReplaceBOOL(string toChange, string expected)
    {
        string result = Apply("ReplaceBOOL", toChange);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("BOOL x = TRUE;", "bool x = true;")]
    [DataRow("\tBOOL x = FALSE;", "\tbool x = false;")]
    [DataRow(" BOOL x = TRUE;", " bool x = true;")]
    [DataRow("void Func(BOOL xy) {", "void Func(bool xy) {")]
    [DataRow("void Func(BOOL xy, BOOL wert) {", "void Func(bool xy, bool wert) {")]
    public void ReplaceBOOLApplyAll(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region RemoveMethodClassName
    [TestMethod]
    [DataRow("void CDsClass::DoSomething()", "void DoSomething()")]
    [DataRow("void CDsClass::DoSomething(int x, CDsClass& refClass)", "void DoSomething(int x, CDsClass& refClass)")]
    [DataRow("void CDsClass::DoSomething(int x,\r\n", "void DoSomething(int x,\r\n")]
    [DataRow("\t void CDsClass::DoSomething(int x,\r\n", "\t void DoSomething(int x,\r\n")]
    public void RemoveMethodClassName(string toChange, string expected)
    {
        string result = Apply("RemoveMethodClassName", toChange);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("void CDsClass::DoSomething()", "void DoSomething()")]
    [DataRow("void CDsClass::DoSomething(BOOL x = FALSE, CDsClass& refClass)", "void DoSomething(bool x = false, CDsClass refClass)")]
    [DataRow("void CDsClass::DoSomething(DWORD x,\r\n", "void DoSomething(uint x,\r\n")]
    [DataRow("\t void CDsClass::DoSomething(INT x,\r\n", "\t void DoSomething(int x,\r\n")]
    public void RemoveMethodClassNameApplyAll(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceRUNTIME_CLASS
    [TestMethod]
    [DataRow("\tASSERT(psto->IsKindOf(RUNTIME_CLASS(CDsStorage)));\r\n", "\tDebug.Assert(psto.IsKindOf(Afx.RUNTIME_CLASS(typeof(CDsStorage))));\r\n")]
    public void ReplaceRUNTIME_CLASSApplyAll(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceStatic_cast
    [TestMethod]
    [DataRow("\tx = static_cast<LPCTSTR>(s);\r\n", "\tx = (string)(s);\r\n")]
    [DataRow("\tx = Test(static_cast<LPCTSTR>(s) + abc);\r\n", "\tx = Test((string)(s) + abc);\r\n")]
    public void ReplaceStatic_cast(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceConst_cast
    [TestMethod]
    [DataRow("\tint nKeyIndex = const_cast<CDsDataInfo*>(this)->GetIndex(strKey);\r\n", "\tint nKeyIndex = (this).GetIndex(strKey);\r\n")]
    [DataRow("\tint nKeyIndex = const_cast<CDsDataInfo?>(this).GetIndex(strKey);\r\n", "\tint nKeyIndex = (this).GetIndex(strKey);\r\n")]
    public void ReplaceConst_cast(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceConstMethod
    [TestMethod]
    [DataRow("\tCString Method(INT x, double y) const;\r\n", "\tCString Method(int x, double y);\r\n")]
    [DataRow("\tCString Method(INT x, double y)  const  ;\r\n", "\tCString Method(int x, double y)   ;\r\n")]
    public void ReplaceConstMethod(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region Remove_FUNC_
    [TestMethod]
    [DataRow("\tstatic LPCTSTR _FUNC_ = _T(\"CDsDataInfo::SwitchCurrency()\");", "")]
    [DataRow("\tconst LPCTSTR _FUNC_ = _T(\"CDsDataInfo::SwitchCurrency()\");", "")]
    [DataRow("\tconstexpr LPCTSTR _FUNC_ = _T(\"CDsDataInfo::SwitchCurrency\");", "")]
    public void Remove_FUNC_(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region Helper
    private string Apply(string ruleName, string toChange)
    {
        Assert.IsNotNull(ruleSet);
        var buffer = new StringBuilder(toChange);
        ruleSet!.ApplyRule(ruleName, buffer);
        return buffer.ToString();
    }

    private string Apply(string toChange)
    {
        Assert.IsNotNull(ruleSet);
        return ruleSet!.ApplyAll(toChange);
    }
    #endregion
}
