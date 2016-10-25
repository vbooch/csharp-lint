namespace CSharpLint.Model
{
    using System.Collections.Generic;

    public class LintResults
    {
        public IEnumerable<LintIssue> Issues { get; }

        public string FileName { get; }

        public LintResults(IEnumerable<LintIssue> issues, string fileName)
        {
            FileName = fileName;
            Issues = issues;
        }
    }
}