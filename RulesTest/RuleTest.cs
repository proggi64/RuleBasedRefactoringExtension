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

    #region ReplaceSimpleClassInstance
    [TestMethod]
    [DataRow("\tCMyClass myClass;", "\tCMyClass myClass = new CMyClass();")]
    [DataRow("\tMyClass myClass;", "\tMyClass myClass;")]
    [DataRow("\tCMyClass myClass; // Test", "\tCMyClass myClass = new CMyClass(); // Test")]
    public void ReplaceSimpleClassInstance(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeScalarTypeRef
    [TestMethod]
    [DataRow("void Test(int& x) {", "void Test(ref int x) {")]
    [DataRow("void Test(int& x, double& y) {", "void Test(ref int x, ref double y) {")]
    [DataRow("void Test(int & x, double & y) {", "void Test(ref int x, ref double y) {")]
    public void ChangeScalarTypeRef(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeDelete
    [TestMethod]
    [DataRow("delete[] ptr;", "ptr = null;")]
    [DataRow("delete ptr;", "ptr = null;")]
    [DataRow("delete ptr; // Test", "ptr = null; // Test")]
    [DataRow("\tdelete ptr; // Test", "\tptr = null; // Test")]
    [DataRow("\tx = 10;\r\n\tdelete ptr; // Test", "\tx = 10;\r\n\tptr = null; // Test")]
    public void ChangeDelete(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangePointerDecl
    [TestMethod]
    [DataRow("int* ptr;", "int? ptr;")]
    [DataRow("CString* ptr;", "CString? ptr;")]
    [DataRow("void Test(CString* ptr) // test", "void Test(CString? ptr) // test")]
    [DataRow("\ta = 10;\r\nvoid Test(CString* ptr) // test", "\ta = 10;\r\nvoid Test(CString? ptr) // test")]
    public void ChangePointerDecl(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeArrayDecl
    [TestMethod]
    [DataRow("int xy[12];", "int[] xy = new int[12];")]
    [DataRow("\tint xy [12]; // test", "\tint[] xy = new int[12]; // test")]
    [DataRow("\ta = 10;\r\n\tint xy [12]; // test", "\ta = 10;\r\n\tint[] xy = new int[12]; // test")]
    [DataRow("\ta = 10;\r\n\tint xy [12]; // test\r\n\ta = 10;\r\n", "\ta = 10;\r\n\tint[] xy = new int[12]; // test\r\n\ta = 10;\r\n")]
    public void ChangeArrayDecl(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeStrCmp
    [TestMethod]
    [DataRow("s = strcmp(a, b);", "s = Str.strcmp(a, b);")]
    [DataRow("\ta = 10;\r\n\ts = strcmp(a, b);\r\n\ta = 10;\r\n", "\ta = 10;\r\n\ts = Str.strcmp(a, b);\r\n\ta = 10;\r\n")]
    [DataRow("s = Str.strcmp(a, b);", "s = Str.strcmp(a, b);")]
    public void ChangeStrCmp(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeTcsCmp
    [TestMethod]
    [DataRow("s = _tcscmp(a, b);", "s = Str.strcmp(a, b);")]
    public void ChangeTcsCmp(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeStriCmp
    [TestMethod]
    [DataRow("s = stricmp(a, b);", "s = Str.stricmp(a, b);")]
    [DataRow("\r\nx = 10;\ts = stricmp(a, b);", "\r\nx = 10;\ts = Str.stricmp(a, b);")]
    [DataRow("s = Str.stricmp(a, b);", "s = Str.stricmp(a, b);")]
    public void ChangeStriCmp(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeTcsCmpi
    [TestMethod]
    [DataRow("s = _tcscmpi(a, b);", "s = Str.stricmp(a, b);")]
    public void ChangeTcsCmpi(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeStrnCmp
    [TestMethod]
    [DataRow("s = strncmp(a, x, b);", "s = Str.strncmp(a, x, b);")]
    [DataRow("\ta = 10;\r\n\ts = strncmp(a, x, b);\r\n\ta = 10;\r\n", "\ta = 10;\r\n\ts = Str.strncmp(a, x, b);\r\n\ta = 10;\r\n")]
    [DataRow("s = Str.strncmp(a, xyz, b);", "s = Str.strncmp(a, xyz, b);")]
    public void ChangeStrnCmp(string toChange, string expected)
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
