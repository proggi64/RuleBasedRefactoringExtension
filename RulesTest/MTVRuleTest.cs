namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class MTVRuleTest : RuleTest
{
    #region MTVFunction
    [TestMethod]
    [DataRow("x = MTV_GET_STRING(MTV_VALUE);", "x = MTV.MTV_GET_STRING(MTV_VALUE);")]
    [DataRow("x = MTV_GET_LONG(MTV_VALUE);", "x = MTV.MTV_GET_LONG(MTV_VALUE);")]
    [DataRow("CDsVarValue? x = MTV_GET(MTV_VALUE);", "CDsVarValue? x = MTV.MTV_GET(MTV_VALUE);")]
    [DataRow("MTV_SET(MTV_VALUE, 123);", "MTV.MTV_SET(MTV_VALUE, 123);")]
    [DataRow("\tx = MTV_GET_LONG(MTV_VALUE);\r\n", "\tx = MTV.MTV_GET_LONG(MTV_VALUE);\r\n")]
    public void MTVFunction(string toChange, string expected)
    {
        string result = Apply("MTVFunction", toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion
}
