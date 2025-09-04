using System.Text;

namespace Webkasi.Refactoring.Rules
{
    public abstract class Rule
    {
        public bool? AutoApply { get; set; } = true;
        public string Name { get; set; }
        public string MenuText { get; set; }

        public abstract void Apply(StringBuilder result);
    }
}
