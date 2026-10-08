using System.Collections.Generic;

namespace JevControllerKeyboard.Features
{
    public sealed class CandidateTable
    {
        private readonly Dictionary<string, char[]> _entries = new Dictionary<string, char[]>();

        public CandidateTable(string content)
        {
            foreach (var line in content.Split('\n'))
            {
                var separator = line.IndexOf(' ');
                if (separator > 0)
                {
                    _entries[line.Substring(0, separator)] =
                        line.Substring(separator + 1).Trim().ToCharArray();
                }
            }
        }

        public char[] Lookup(string reading)
        {
            return _entries.TryGetValue(reading, out var characters)
                ? characters
                : System.Array.Empty<char>();
        }
    }
}
