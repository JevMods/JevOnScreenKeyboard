using System;

namespace JevOnScreenKeyboard.Tests
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--coverage")
            {
                return Coverage.Run(args[1]);
            }

            InputMethodTests.Run();
            LayoutTests.Run();
            SessionTests.Run();
            Console.WriteLine("{0} checks, {1} failures", Check.Total, Check.Failures);
            return Check.Failures == 0 ? 0 : 1;
        }
    }
}
