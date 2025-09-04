using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;

using Webkasi.Refactoring.Rules;

namespace Webkasi.Refactoring.Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RulesCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1deb8201-3e37-4922-b2cc-3b79800545a1");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly RuleSet ruleSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private RulesCommand(AsyncPackage package, OleMenuCommandService commandService, RuleSet ruleSet)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            this.ruleSet = ruleSet ?? throw new ArgumentNullException(nameof(ruleSet));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            int i = 0;
            const int DynamicRuleCommandId = 0x0110; // Start-ID für dynamische Regeln

            foreach (var rule in ruleSet)
            {
                if (rule.Value.AutoApply != false)
                    continue;
                menuCommandID = new CommandID(CommandSet, DynamicRuleCommandId + i);
                menuItem = new OleMenuCommand((s, e) => ApplyRule(rule.Key), menuCommandID);
                menuItem.BeforeQueryStatus += (s, e) =>
                {
                    ((OleMenuCommand)s).Visible = true;
                    ((OleMenuCommand)s).Text = string.IsNullOrEmpty(rule.Value.MenuText) ? rule.Value.Name : rule.Value.MenuText;
                };
                commandService.AddCommand(menuItem);
                i++;
            }
            for (int j = i; j < 10; j++)
            {
                menuCommandID = new CommandID(CommandSet, DynamicRuleCommandId + j);
                menuItem = new OleMenuCommand((s, e) => { }, menuCommandID)
                {
                    Visible = false
                };
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RulesCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RulesCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            // Pfad zum Erweiterungsordner
#if DEBUG
            string extensionDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#else
            string extensionDir = AppDomain.CurrentDomain.BaseDirectory;
#endif
            string rulesDir = Path.Combine(extensionDir, "Rules");

            // Alle JSON-Dateien im Rules-Ordner laden
            RuleSet ruleSet = null;
            bool first = true;
            var jsonFiles = Directory.GetFiles(rulesDir, "*.json");
            foreach (var rulesFile in jsonFiles)
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
                        ruleSet.Add(rule.Key, rule.Value);
                    }
                }
            }

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RulesCommand(package, commandService, ruleSet);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string clipboardText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboardText))
                return;

            // Hier Deine Refactoring-Komponente aufrufen:
            string transformed = Refactor(clipboardText);

            // In den Editor einfügen:
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            var doc = dte.ActiveDocument;
            var selection = (TextSelection)doc.Selection;
            selection.Insert(transformed);
        }

        /// <summary>
        /// Applies a set of transformation rules to the specified input string.
        /// </summary>
        /// <param name="input">The input string to which the transformation rules will be applied. Cannot be null.</param>
        /// <returns>A string resulting from applying all transformation rules to the input. The returned string may be the same
        /// as the input if no rules modify it.</returns>
        private string Refactor(string input)
        {
            return ruleSet.ApplyAll(input);
        }

        /// <summary>
        /// Führt die durch das Command aufgerufene Regel aus.
        /// </summary>
        /// <remarks>Es wird der aktuelle Text aus der Zwischenablage geholt, die Regel darauf angewendet,
        /// und das Ergebnis an der Cursorposition des Editors eingefügt,</remarks>
        /// <param name="ruleName">Name der auszuführenden Regel.</param>
        private void ApplyRule(string ruleName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string clipboardText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipboardText))
                return;

            var buffer = new System.Text.StringBuilder(clipboardText);
            ruleSet.ApplyRule(ruleName, buffer);

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            var doc = dte.ActiveDocument;
            var selection = (TextSelection)doc.Selection;
            selection.Insert(buffer.ToString());
        }
    }
}
