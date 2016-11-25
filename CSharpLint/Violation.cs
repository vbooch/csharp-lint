namespace CSharpLint
{
    public class Violation
    {
        public Violation(string id, string message, Severity severity)
        {
            Id = id;
            Message = message;
            Severity = severity;
        }

        public string Id { get; }

        public string Message { get; }

        public Severity Severity { get; }
    }
}
