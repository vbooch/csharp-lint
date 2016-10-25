namespace CSharpLint
{
    using System;
    using System.IO;
    using CSharpLint.Model;
    using Newtonsoft.Json;
    using Vstack.Extensions;

    public static class Program
    {
        public static void Main(string[] args)
        {
            args.ValidateNotNullParameter(nameof(args));

            if (args.Length != 1)
            {
                Console.Write("Must specify exactly one argument which is the path to the file to analyze.");
                Environment.Exit(1);
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.Write("The specified file could not be found.");
                Environment.Exit(1);
            }

            var csharpSource = File.ReadAllText(filePath);

            var analyzer = new Analyzer();
            var issues = analyzer.Analyze(filePath, csharpSource);
            var result = new LintResults(issues, filePath);

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
