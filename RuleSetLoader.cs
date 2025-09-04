using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Webkasi.Refactoring.Rules
{
    public static class RuleSetLoader
    {
        public static RuleSet LoadFromJson(string json)
        {
            var ruleDtos = JsonConvert.DeserializeObject<List<RuleDto>>(json);

            var ruleSet = new RuleSet();

            // Ersetze das switch-Statement mit einer klassischen if-else-Kette, da rekursive Muster erst ab C# 8.0 unterstützt werden.
            foreach (var dto in ruleDtos)
            {
                Rule rule;
                if (dto.Type == "SimpleReplace")
                {
                    rule = new SimpleReplaceRule
                    {
                        AutoApply = dto.AutoApply,
                        Name = dto.Name,
                        MenuText = dto.MenuText,
                        OldValue = dto.OldValue,
                        NewValue = dto.NewValue
                    };
                }
                else if (dto.Type == "RegEx")
                {
                    rule = new RegExRule
                    {
                        AutoApply = dto.AutoApply,
                        Name = dto.Name,
                        MenuText = dto.MenuText,
                        OldValue = dto.OldValue,
                        NewValue = dto.NewValue
                    };
                }
                else if (dto.Type == "DebugBlock")
                {
                    rule = new DebugBlockRule
                    {
                        AutoApply = dto.AutoApply,
                        Name = dto.Name,
                        MenuText = dto.MenuText
                    };
                }
                else if (dto.Type == "Visibility")
                {
                    rule = new VisibilityRule
                    {
                        AutoApply = dto.AutoApply,
                        Name = dto.Name,
                        MenuText = dto.MenuText
                    };
                }
                else
                {
                    throw new NotSupportedException($"Unbekannter Regeltyp: {dto.Type}");
                }

                ruleSet.Add(rule.Name, rule);
            }
            return ruleSet;
        }

        internal class RuleDto
        {
            public string Type { get; set; }
            public bool? AutoApply { get; set; } = true;
            public string Name { get; set; }
            public string MenuText { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }
    }
}