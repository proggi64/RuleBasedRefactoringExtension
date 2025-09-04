using System;
using System.Collections.Generic;
using System.Text;

namespace Webkasi.Refactoring.Rules
{
    /// <summary>
    /// Set of <see cref="Rule"/>s identified by their names.
    /// </summary>
    public class RuleSet : Dictionary<string, Rule>
    {
        /// <summary>
        /// Adds the rule to the set.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <exception cref="ArgumentException">A rule with this name ahs already been added.</exception>
        public void AddRule(Rule rule)
        {
            if (ContainsKey(rule.Name))
            {
                throw new ArgumentException($"A rule with the name '{rule.Name}' already exists.");
            }
            Add(rule.Name, rule);
        }

        /// <summary>
        /// Applies all rules that have <see cref="Rule.AutoApply"/> set to <see langword="true"/>.
        /// </summary>
        /// <param name="input">Text where the rule is applied to.</param>
        /// <returns>Text where the rule has been applied to.</returns>
        public string ApplyAll(string input)
        {
            var result = new StringBuilder(input);
            foreach (var rule in Values)
            {
                if (rule.AutoApply ?? false)
                    rule.Apply(result);
            }
            return result.ToString();
        }

        /// <summary>
        /// Applies a specific rule by its name.
        /// </summary>
        /// <param name="ruleName">name of the rule to apply.</param>
        /// <param name="result">Text where the rule is to be applied and has been applied to
        /// after the call.</param>
        /// <exception cref="KeyNotFoundException"><paramref name="ruleName"/> has not been found.</exception>
        public void ApplyRule(string ruleName, StringBuilder result)
        {
            if (TryGetValue(ruleName, out var rule))
            {
                rule.Apply(result);
            }
            else
            {
                throw new KeyNotFoundException($"No rule found with the name '{ruleName}'.");
            }
        }
    }
}