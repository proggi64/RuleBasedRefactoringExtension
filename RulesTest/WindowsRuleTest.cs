namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class WindowsRuleTest : RuleTest
{
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

    #region HINSTANCE
    [TestMethod]
    [DataRow("HINSTANCE x = NULL;", "Assembly? x = null;")]
    [DataRow("void Func(HINSTANCE hInst) {", "void Func(Assembly? hInst) {")]
    [DataRow("\tvoid Func(HINSTANCE hInst) {\r\n", "\tvoid Func(Assembly? hInst) {\r\n")]
    public void HINSTANCE(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region HMODULE
    [TestMethod]
    [DataRow("HMODULE x = NULL;", "Assembly? x = null;")]
    [DataRow("void Func(HMODULE hInst) {", "void Func(Assembly? hInst) {")]
    [DataRow("\tvoid Func(HMODULE hInst, int i) {\r\n", "\tvoid Func(Assembly? hInst, int i) {\r\n")]
    public void HMODULE(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region HANDLE
    [TestMethod]
    [DataRow("HANDLE x = NULL;", "uint? x = null;")]
    [DataRow("void Func(HANDLE handle) {", "void Func(uint? handle) {")]
    [DataRow("\tvoid Func(HANDLE handle, int i) {\r\n", "\tvoid Func(uint? handle, int i) {\r\n")]
    [DataRow("a = 10;\r\n\tvoid Func(HANDLE handle, int i) {\r\n", "a = 10;\r\n\tvoid Func(uint? handle, int i) {\r\n")]
    public void HANDLE(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion
}
