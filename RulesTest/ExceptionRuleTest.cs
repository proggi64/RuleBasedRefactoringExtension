namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class ExceptionRuleTest : RuleTest
{
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
}
