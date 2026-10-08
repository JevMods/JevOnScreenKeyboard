using System;
using System.Globalization;

namespace JevControllerKeyboard.Features
{
    public sealed class LanguageLayout
    {
        public const char Gap = ' ';

        private const int LabelLength = 3;

        private Func<IInputMethod> _input = () => new PlainInput();

        public LanguageLayout(params string[] rows)
            : this(CultureInfo.InvariantCulture, rows)
        {
        }

        public LanguageLayout(CultureInfo culture, params string[] rows)
        {
            Culture = culture;
            Rows = rows;
            Label = rows[0].Substring(0, Math.Min(LabelLength, rows[0].Length)).ToUpper(culture);
        }

        public CultureInfo Culture { get; }

        public string[] Rows { get; }

        public string[] ShiftRows { get; private set; }

        public string Label { get; private set; }

        public char Comma { get; private set; } = ',';

        public char Period { get; private set; } = '.';

        public LanguageLayout WithShift(params string[] rows)
        {
            var copy = Copy();
            copy.ShiftRows = rows;
            return copy;
        }

        public LanguageLayout WithLabel(string label)
        {
            var copy = Copy();
            copy.Label = label;
            return copy;
        }

        public LanguageLayout WithPunctuation(char comma, char period)
        {
            var copy = Copy();
            copy.Comma = comma;
            copy.Period = period;
            return copy;
        }

        public LanguageLayout WithInput(Func<IInputMethod> input)
        {
            var copy = Copy();
            copy._input = input;
            return copy;
        }

        public IInputMethod CreateInput()
        {
            return _input();
        }

        public char ShiftedOf(int row, int index)
        {
            var character = Rows[row][index];
            return ShiftRows != null
                ? ShiftRows[row][index]
                : char.ToUpper(character, Culture);
        }

        private LanguageLayout Copy()
        {
            return (LanguageLayout)MemberwiseClone();
        }
    }
}
