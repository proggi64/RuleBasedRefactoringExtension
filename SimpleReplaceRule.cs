using System.Text;

namespace Webkasi.Refactoring.Rules
{
    public class SimpleReplaceRule : Rule
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override void Apply(StringBuilder result)
        {
            result.Replace(OldValue, NewValue);
        }
    }
}
