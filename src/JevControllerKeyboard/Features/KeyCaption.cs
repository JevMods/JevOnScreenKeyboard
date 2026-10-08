using System.Globalization;
using JevControllerKeyboard.Infrastructure;

namespace JevControllerKeyboard.Features
{
    public sealed class KeyCaption
    {
        private const char DottedCircle = '\u25CC';

        private readonly IGameLanguage _language;

        public KeyCaption(IGameLanguage language)
        {
            _language = language;
        }

        public string Of(KeyLayout layout, Key key)
        {
            var label = layout.Label(key);
            switch (key.Action)
            {
                case KeyAction.Character:
                case KeyAction.Choice:
                    return Printable(label);
                case KeyAction.NextPage:
                    return label;
                default:
                    return _language.Localize(label);
            }
        }

        private static string Printable(string label)
        {
            if (label.Length != 1)
            {
                return label;
            }

            var category = CharUnicodeInfo.GetUnicodeCategory(label[0]);
            var combining = category == UnicodeCategory.NonSpacingMark
                || category == UnicodeCategory.SpacingCombiningMark
                || category == UnicodeCategory.EnclosingMark;
            return combining ? DottedCircle + label : label;
        }
    }
}
