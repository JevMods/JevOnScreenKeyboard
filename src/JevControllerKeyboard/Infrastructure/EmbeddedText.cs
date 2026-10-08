using System.IO;
using System.Text;

namespace JevControllerKeyboard.Infrastructure
{
    public static class EmbeddedText
    {
        public static string Read(string name)
        {
            var assembly = typeof(EmbeddedText).Assembly;
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
