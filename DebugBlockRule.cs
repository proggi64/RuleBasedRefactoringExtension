using System;
using System.Text;

namespace Webkasi.Refactoring.Rules
{

    /// <summary>
    /// Entfernt alle #ifdef _DEBUG oder #if defined _DEBUG-Blöcke aus dem Quelltext.
    /// </summary>
    internal class DebugBlockRule : Rule
    {
        public override void Apply(StringBuilder result)
        {
            var lines = result.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            int ifdefLevel = 0;

            int count = lines.Length;

            foreach (var line in lines)
            {
                count--;
                if (line.Contains("#ifdef _DEBUG") || line.Contains("defined _DEBUG") ||
                    line.Contains("#ifdef DEBUG") || line.Contains("defined DEBUG"))
                    ifdefLevel++;
                else if (ifdefLevel > 0 && (line.Contains("#ifdef") || line.Contains("#if defined")))
                    ifdefLevel++;
                else if (line.Contains("#endif") && ifdefLevel > 0)
                    ifdefLevel--;
                else if (ifdefLevel == 0)
                    if (count > 0)
                        sb.AppendLine(line);
                    else
                        sb.Append(line);
            }
            result.Clear();
            result.Append(sb);
        }
    }
}