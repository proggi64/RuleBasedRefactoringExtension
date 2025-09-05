# RuleBasedRefactoringExtension
Erweiterung für Visual Studio 2022, um beim C++ nach C#-Refactoring ARbeitsabläufe zu automatisieren.

## Refactoring beim Einfügen aus Zwischenablage
Aktuell wird das Refactoring beim Einfügen von Quelltext aus der Zwischenablage durchgeführt. Der Arbeitsablauf ist folgender:
- C++-Codesequenz aus dem C++-Editor in die Zwischenablage kopieren
- In den C#-Zielquelltext wechseln
- Control und + drücken
Dadurch werden auf den C++-Quelltext vor dem Einfügen die als JSON-Dateien im Rules-Unterverzeichnis definierten Regeln angewandt. s werden alls Regeln in der Reihenfolge ihrer Definition in den JSON-Dateien angewandt. Ausnahme sind Regeln, die das Attribut AutoApply = false haben. Dazu unten mehr.

## Regeln definieren
Die Regeln sind in C# geschrieben und sind alle von der abstrkten Basisklasse Webkasi.Refactoring.Rules.Rule abgeleitet. Es gibt bisher folgende Rule-Klassen:
- SimpleReplaceRule
- RegExRule
- VisibilityRule
- DebugBlockRule
Die Regeln werden in den JSON-Dateien definiert und im Rules-Unterverzerzeichnis abgelegt. Neue JSON-Dateien müssen im Projekt die Eigenschaften "Inhalt/Content" und "Immer kopieren/Copy Always" haben. Jeder JSON-Eintrag einer Regel muss einen eindeutigen Namen im Attribut Name bekommen.

### SimpleReplaceRule
Beispiel:
```JSON
  {
    "Type": "SimpleReplace",
    "Name": "ReplaceNotNot",
    "OldValue": "!!",
    "NewValue": "0 != "
  },
```
Beim "Type" wird der Suffix "Rule" weggelassen. "Name" muss über alle Regeln aller JSON-Dateien eindeutig sein. SimpleReplace ersetzt den Text in "OldValue" durch den Text in "NewValue".

Hintergrund: In C++ wird !! oft verwendet, um numerische Ausdrücke in bool zu konvertieren. Ein numerischer Wert ungleich 0 ist true. In C# ist durch das "0 !=" derselbe Effekt zu erreichen. Eine erweiterte Prüfung durch einen regulären Ausdruck ist in diesem Fall praktisch nie notwendig.

### RegExRule
Beispiel:
```JSON
  {
    "Type": "RegEx",
    "Name": "_TCharLiteral",
    "OldValue": "_T\\s*\\(\\s*(\"([^'\\\\]|\\\\.)*')\\s*\\)",
    "NewValue": "$1"
  },
```
Beim "Type" wird der Suffix "Rule" weggelassen. "Name" muss über alle Regeln aller JSON-Dateien eindeutig sein. RegExRule wendet die C#-Methode Regex.Replace an. "OldValue" ist der reguläre Ausdruck, nach dem gesucht wird, "NewValue" der Ausdruck, der als Ersatztext eingesetzt wird. Die Dokumentation der Syntax findet man in der C#-Dokumentation. Eine gute Idee ist der Einsatz einer KI, um die jeweiligen regulären Ausdrücke zu erstellen.

Hintergund: In C++-Quellen wird das _T-Makro eingetzt, um String-Konstanten bei Übersetzung von ANSI- und Unicode-Versionen jeweils an die Enkodierung anzupassen. Diese Makros (_T("Text")) müssen in C# entfernt werden.

### VisibilityRule
Anwendung:
```JSON
  {
    "Type": "Visibility",
    "AutoApply": false,
    "Name": "ConvertVisibility",
    "MenuText": "Konvertiere public/protected/private"
  }
```
Beim "Type" wird der Suffix "Rule" weggelassen. "Name" muss über alle Regeln aller JSON-Dateien eindeutig sein. "AutoApply" gibt an, ob die Regel beim Einfügen immer angewandt wird (Control +), oder im Extras-Menü des Visual Studio nur durch Auslösen eines Menüpunkts. Falls "false" wird die Regel in die Liste dieser Menüpunkte aufgenommen und kann  ur durch den Menüpunkt manuell ausgelöst werden. Falls "MenuText" angegeben wird, wird dieser Text als Menüpunkt verwendet, ansonsten der unter "Name".

Die VisibilityRule ist nicht parametrisierbar und muss nur einmal in eine JSON-Datei eingetragen werden. Sie analysiert den einzufügenden Text, ob es sich um eine Header-Sequenz mit Methoden- und Member-Deklarationen handelt. Falls ja, werden die Sichtbarkeitsattribute verwednet, die oberhalb der Deklarationen stehen und jeweils vor die Einzeldeklaration gestellt, wie es in C# notwendig ist. Dabei werden auch mehrzeilige Deklarationen korrekt berücksichtigt. Weil sich Header signifikant von sonstigem C++-Code unterscheiden, ist die Option "AutoApply = false" sinnvoll, um die Regel bur bei Bedarf anzuwenden.

### DebugBlockRule
Anwendung:
```JSON
  {
    "Type": "DebugBlock",
    "Name": "RemoveDebugBlock"
  },
```
Beim "Type" wird der Suffix "Rule" weggelassen. "Name" muss über alle Regeln aller JSON-Dateien eindeutig sein. Ansonsten benötigt die Regel keine Parameter, kann aber, wie alle anderen Regelen auch mit AUtoApply = false versehen werden.

Die Regel entfernt #ifdef _DEBUG bzw. #if defined _DEBUG - Blöcke, auch wenn in ihnen noch verschachtelte weitere #ifdef-Blöcke stehen. Für das C#-Refactoring sind diese spezifischen C++-Blöcke oft nicht relevant.

## Menü Extras
Die Anwendung der Regeln steht im Visual Studio Menü Extras unter dem Menüpunkt "Einfügen mit Regeln". Unter diesem Punkt werden die bis zu 9 zusätzlichen Menüpunkte für die Regeln mit "AutoApply = false" angehängt. Die Platzhalter für diese zusätzlichen Regeln stehen in der vsct-Datei. Wenn mehr benötigt werden, müssen diese hier angehängt und in der Quelldatei RulesCommand.cs die Konstante MaxManualRules angepasst werden.
