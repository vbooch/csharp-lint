namespace CSharpLint
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using CSharpLint.Model;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Newtonsoft.Json;

    public class Analyzer
    {
        private readonly Dictionary<string, Violation> styleViolations;

        public Analyzer()
        {
            styleViolations = ReadStyleViolations().ToDictionary(e => e.Id);
        }

        public IEnumerable<LintIssue> Analyze(string filePath, string csharpSource)
        {
            ImmutableArray<Diagnostic> diagnostics = GetDiagnostics(filePath, csharpSource);

            return diagnostics.Select(CreateIssue).Where(violation => violation.Severity < LintSeverity.Disabled);
        }

        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2001:AvoidCallingProblematicMethods",
            MessageId = "System.Reflection.Assembly.LoadFile",
            Justification = "No other option.")]
        private ImmutableArray<Diagnostic> GetDiagnostics(string filePath, string csharpSource)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csharpSource, path: filePath);

            CSharpCompilation compilation = CSharpCompilation.Create("target")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            ImmutableArray<Diagnostic> parseDiagnostics = compilation.GetParseDiagnostics();

            Assembly stylecopAnalyzersAssembly = Assembly.LoadFile(GetPathToFile(@"StyleCop.Analyzers.dll"));
            ImmutableArray<DiagnosticAnalyzer> analyzers = stylecopAnalyzersAssembly.GetTypes()
                .Where(t => t.IsAbstract == false && typeof(DiagnosticAnalyzer).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t) as DiagnosticAnalyzer)
                .ToImmutableArray();

            CompilationWithAnalyzersOptions options = new CompilationWithAnalyzersOptions(
                new AnalyzerOptions(default(ImmutableArray<AdditionalText>)), OnAnalyzerException, false, false);

            CompilationWithAnalyzers compilationWithAnalyzers = new CompilationWithAnalyzers(compilation, analyzers, options);

            Task<ImmutableArray<Diagnostic>> analyzerDiagnosticsTask = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
            analyzerDiagnosticsTask.Wait();
            ImmutableArray<Diagnostic> analyzerDiagnostics = analyzerDiagnosticsTask.Result;

            return parseDiagnostics.Concat(analyzerDiagnostics).ToImmutableArray();
        }

        private LintIssue CreateIssue(Diagnostic diagnostic)
        {
            var lineSpan = diagnostic.Location.GetLineSpan();
            var line = lineSpan.StartLinePosition.Line;
            var column = lineSpan.StartLinePosition.Character;

            var code = diagnostic.Id;
            var name = diagnostic.GetMessage();
            var desc = diagnostic.Descriptor.Description.ToString();

            var severity = LintSeverity.Advice;

            if (code.StartsWith("CS"))
            {
                severity = LintSeverity.Error;
            }

            if (code.StartsWith("SA") && styleViolations.ContainsKey(code))
            {
                var definedSeverity = styleViolations[code].Severity;
                switch (definedSeverity)
                {
                    case Severity.None:
                        severity = LintSeverity.Disabled;
                        break;
                    case Severity.Warning:
                        severity = LintSeverity.Warning;
                        break;
                    case Severity.Error:
                        severity = LintSeverity.Error;
                        break;
                    case Severity.Advice:
                        severity = LintSeverity.Advice;
                        break;
                    default:
                        severity = LintSeverity.Disabled;
                        break;
                }
            }

            return new LintIssue(code, name, desc, severity, line, column);
        }

        private Violation[] ReadStyleViolations()
        {
            string violationsJson = File.ReadAllText(GetPathToFile(@"violations.json"));
            return JsonConvert.DeserializeObject<Violation[]>(violationsJson);
        }

        private string GetPathToFile(string file)
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string exeFolder = Path.GetDirectoryName(exeFilePath);
            return Path.Combine(exeFolder, file);
        }

        private void OnAnalyzerException(Exception exception, DiagnosticAnalyzer analyzer, Diagnostic diagnostic)
        {
            // do nothing
        }
    }
}
