namespace CSharpLint
{
    public class Violation
    {
        public Violation(string id, string message, Severity severity)
        {
            this.Id = id;
            this.Message = message;
            this.Severity = severity;
        }

        public string Id { get; }

        public string Message { get; }

        public Severity Severity { get; }
    }
}
