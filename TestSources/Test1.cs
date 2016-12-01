using System.Diagnostics.CodeAnalysis;

namespace MyNamespace
{
    public static class Test1
    {
        public static string Test;
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public static class Test2
    {
        public static string test;
    }

    public static class Test3
    {
        public static string Test;
    }
}
