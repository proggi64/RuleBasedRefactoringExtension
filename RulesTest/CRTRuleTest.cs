namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class CRTRuleTest : RuleTest
{
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
}
