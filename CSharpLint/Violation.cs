namespace CSharpLint
{
    public class Violation
    {
        public Violation(int startLine, int endLine, string id, string message, string desc, Severity severity)
        {
            this.StartLine = startLine;
            this.EndLine = endLine;
            this.Id = id;
            this.Message = message;
            this.Severity = severity;
            this.Description = desc;
        }

        public int StartLine { get; }

        public int EndLine { get; }

        public string Id { get; }

        public string Message { get; }

        public string Description { get; }

        public Severity Severity { get; }
    }
}
