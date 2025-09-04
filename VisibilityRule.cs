using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Webkasi.Refactoring.Rules
{
    internal class VisibilityRule : Rule
    {
        public override void Apply(StringBuilder result)
        {
            string headerSourcePart = result.ToString();
            result.Clear();
            result.Append(ConvertCppClassVisibility(headerSourcePart));
        }

        public static string ConvertCppClassVisibility(string headerSourcePart)
        {
            var lines = headerSourcePart.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var result = new List<string>();
            string currentVisibility = "private"; // Standard in C++

            var visibilityRegex = new Regex(@"^\s*(public|private|protected)\s*:\s*$");
            var block = new List<string>();
            bool inMemberOrMethod = false;

            foreach (var line in lines)
            {
                var visMatch = visibilityRegex.Match(line);
                if (visMatch.Success)
                {
                    currentVisibility = visMatch.Groups[1].Value;
                    continue;
                }

                // Start eines Members/Methodenblocks (endet mit ;)
                if (!inMemberOrMethod && !string.IsNullOrWhiteSpace(line) && !visibilityRegex.IsMatch(line))
                {
                    block.Clear();
                    block.Add(line);
                    inMemberOrMethod = !(line.Trim().EndsWith(";"));

                    // BUG der KI: Dieser Block fehlte, so dass nur mehrzeile Deklarationen funktioniert haben.
                    if (line.Trim().EndsWith(";"))
                    {
                        inMemberOrMethod = false;
                        // Sichtbarkeit vor den gesamten Block stellen
                        result.Add($"{currentVisibility} {string.Join(Environment.NewLine, ChangeThrowNotImplemented(block)).Trim()}");
                        block.Clear();
                        continue;
                    }
                }
                else if (inMemberOrMethod)
                {
                    block.Add(line);
                    if (line.Trim().EndsWith(";"))
                    {
                        inMemberOrMethod = false;
                        // Sichtbarkeit vor den gesamten Block stellen
                        result.Add($"{currentVisibility} {string.Join(Environment.NewLine, ChangeThrowNotImplemented(block)).Trim()}");
                        block.Clear();
                        continue;
                    }
                }
                // BUG der KI: Hier wurde sinnlos die letzte Zeile mit Visibility angehängt, auch wenn sie leer war
            }

            // Reste (z.B. letzte Zeile ohne Semikolon)
            if (block.Count > 0 && inMemberOrMethod)
            {
                result.Add($"{currentVisibility} {string.Join(Environment.NewLine, block).Trim()}");
            }

            return string.Join(Environment.NewLine, result);
        }

        private static List<string> ChangeThrowNotImplemented(List<string> block)
        {
            if (block.Count == 0)
                return block;
            int lastIndex = block.Count - 1;
            block[lastIndex] = block[lastIndex].Replace(");", ")" + Environment.NewLine + "{ throw new NotImplementedException(); }");
            return block;
        }
    }
}
