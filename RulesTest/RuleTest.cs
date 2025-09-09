using System.Text;

namespace Webkasi.Refactoring.Rules;

[TestClass]
public class RuleTest
{
    #region Initialize
    public required TestContext TestContext { get; set; }
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
    #endregion

    #region Helper
    protected string Apply(string ruleName, string toChange)
    {
        Assert.IsNotNull(ruleSet);
        var buffer = new StringBuilder(toChange);
        ruleSet!.ApplyRule(ruleName, buffer);
        return buffer.ToString();
    }

    protected string Apply(string toChange)
    {
        Assert.IsNotNull(ruleSet);
        return ruleSet!.ApplyAll(toChange);
    }
    #endregion
}

