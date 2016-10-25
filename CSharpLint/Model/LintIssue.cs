namespace CSharpLint.Model
{
    public class LintIssue
    {
        public int LineNumber { get; }

        public int Column { get; }

        public string Code { get; }

        public string Name { get; }

        public string Message { get; }

        public LintSeverity Severity { get; }

        public LintIssue(string code, string name, string message, LintSeverity severity, int line, int column)
        {
            Code = code;
            Name = name;
            Message = message;
            Severity = severity;
            LineNumber = line;
            Column = column;
        }
    }
}
