using System;
using System.Collections.Generic;
using System.Linq;

namespace JevControllerKeyboard.Features
{
    public static class KeyRowBuilder
    {
        public const int ServiceRows = 2;

        private const int MinWidth = 11;
        private const int FixedSpecialWidth = 9;
        private const int MinSpaceWidth = 2;
        private const string Digits = "1234567890";

        private static readonly string[] SymbolRows =
        {
            "!@#$%^&*()",
            "-_=+[]{}\\|",
            "/:;\"'<>?~`",
        };

        public static int RowsOf(LanguageLayout language)
        {
            return language.Rows.Length + ServiceRows;
        }

        public static List<List<Key>> Build(
            LanguageLayout language,
            bool symbols,
            bool canSwitchLanguage,
            out int width)
        {
            var texts = symbols ? SymbolTexts() : LetterTexts(language);
            var specialWidth = FixedSpecialWidth + (canSwitchLanguage ? 1 : 0);
            var rowWidth = Math.Max(MinWidth, specialWidth + MinSpaceWidth);
            foreach (var text in texts)
            {
                rowWidth = Math.Max(rowWidth, text.Characters.Length);
            }

            var rows = texts.Select(text => CharacterRow(text, rowWidth)).ToList();
            rows.Add(SpecialRow(language, symbols, canSwitchLanguage, rowWidth, specialWidth));
            width = rowWidth;
            return rows;
        }

        private static List<(string Characters, string Shifted)> SymbolTexts()
        {
            return SymbolRows.Select(row => (row, row)).ToList();
        }

        private static List<(string Characters, string Shifted)> LetterTexts(
            LanguageLayout language)
        {
            var texts = new List<(string Characters, string Shifted)> { (Digits, Digits) };
            for (var row = 0; row < language.Rows.Length; row++)
            {
                texts.Add((language.Rows[row], ShiftedRow(language, row)));
            }

            return texts;
        }

        private static string ShiftedRow(LanguageLayout language, int row)
        {
            var characters = language.Rows[row];
            var shifted = new char[characters.Length];
            for (var index = 0; index < shifted.Length; index++)
            {
                shifted[index] = characters[index] == LanguageLayout.Gap
                    ? LanguageLayout.Gap
                    : language.ShiftedOf(row, index);
            }

            return new string(shifted);
        }

        private static List<Key> CharacterRow((string Characters, string Shifted) text, int width)
        {
            var offset = (width - text.Characters.Length) / 2f;
            var keys = new List<Key>();
            for (var index = 0; index < text.Characters.Length; index++)
            {
                if (text.Characters[index] != LanguageLayout.Gap)
                {
                    keys.Add(new Key(
                        KeyAction.Character,
                        text.Characters[index],
                        text.Shifted[index],
                        offset + index,
                        1f));
                }
            }

            return keys;
        }

        private static List<Key> SpecialRow(
            LanguageLayout language,
            bool symbols,
            bool canSwitchLanguage,
            int width,
            int specialWidth)
        {
            var keys = new List<Key>();
            Append(keys, KeyAction.Shift, '\0', 2f);
            Append(keys, KeyAction.Symbols, '\0', 1f);
            if (canSwitchLanguage)
            {
                Append(keys, KeyAction.Language, '\0', 1f);
            }

            Append(keys, KeyAction.Character, symbols ? ',' : language.Comma, 1f);
            Append(keys, KeyAction.Space, ' ', width - specialWidth);
            Append(keys, KeyAction.Character, symbols ? '.' : language.Period, 1f);
            Append(keys, KeyAction.Backspace, '\0', 2f);
            Append(keys, KeyAction.Enter, '\0', 2f);
            return keys;
        }

        private static void Append(List<Key> keys, KeyAction action, char character, float width)
        {
            var x = keys.Count == 0 ? 0f : keys[keys.Count - 1].X + keys[keys.Count - 1].Width;
            keys.Add(new Key(action, character, x, width));
        }
    }
}
