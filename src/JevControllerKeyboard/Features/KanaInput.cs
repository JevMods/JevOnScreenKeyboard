using System;

namespace JevControllerKeyboard.Features
{
    public sealed class KanaInput : CandidateInput
    {
        private const int MaxReadingLength = 8;

        public KanaInput(Func<CandidateTable> kanji)
            : base(kanji, MaxReadingLength)
        {
        }

        public override void Type(char character, ITextSurface text)
        {
            if (Kana.IsMark(character))
            {
                ApplyMark(character, text);
                return;
            }

            base.Type(character, text);
        }

        protected override bool Accepts(char character)
        {
            return Kana.IsKana(character);
        }

        protected override string ReadingOf(string shown)
        {
            return Kana.Hiragana(shown);
        }

        private void ApplyMark(char mark, ITextSurface text)
        {
            Composition.Synchronize(text);
            var position = text.Caret - 1;
            if (text.HasSelection || position < 0)
            {
                return;
            }

            var previous = text.Text[position];
            var changed = Kana.Convert(previous, mark);
            if (changed == previous)
            {
                return;
            }

            if (Composition.Active)
            {
                var shown = Composition.Shown;
                Composition.Show(text, shown.Substring(0, shown.Length - 1) + changed);
            }
            else
            {
                text.Replace(position, 1, changed.ToString());
            }
        }
    }
}
