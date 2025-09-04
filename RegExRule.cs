using System.Text;
using System.Text.RegularExpressions;

namespace Webkasi.Refactoring.Rules
{
    /// <summary>
    /// Ersetzt alle über reguläre Ausdrücke gefundenen Stellen durch den angegebenen Ersatz-Ausdruck.
    /// </summary>
    public class RegExRule : Rule
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override void Apply(StringBuilder result)
        {
            var replaced = Regex.Replace(result.ToString(), OldValue, NewValue, RegexOptions.None);
            result.Clear();
            result.Append(replaced);
        }
    }
}