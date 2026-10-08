using System;

namespace JevControllerKeyboard.Tests
{
    public static class Check
    {
        public static int Total { get; private set; }

        public static int Failures { get; private set; }

        public static void That(string name, object expected, object actual)
        {
            Total++;
            if (!Equals(expected, actual))
            {
                Failures++;
                Console.WriteLine("FAIL {0}: expected '{1}' got '{2}'", name, expected, actual);
            }
        }
    }
}
