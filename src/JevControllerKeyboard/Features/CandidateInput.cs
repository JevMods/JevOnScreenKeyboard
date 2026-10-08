using System;
using System.Collections.Generic;

namespace JevControllerKeyboard.Features
{
    public abstract class CandidateInput : InputMethod
    {
        private readonly Func<CandidateTable> _table;
        private readonly int _maxLength;

        protected CandidateInput(Func<CandidateTable> table, int maxLength)
        {
            _table = table;
            _maxLength = maxLength;
        }

        public override bool Choosing => Composition.Active;

        public override IReadOnlyList<char> Candidates =>
            Composition.Active
                ? _table().Lookup(ReadingOf(Composition.Shown))
                : Array.Empty<char>();

        protected Composition Composition { get; } = new Composition();

        public override void Type(char character, ITextSurface text)
        {
            if (!Accepts(character))
            {
                Cancel();
                text.Insert(character.ToString());
                return;
            }

            Composition.Synchronize(text);
            if (Composition.Shown.Length >= _maxLength)
            {
                Composition.Cancel();
            }

            Composition.Show(text, Composition.Shown + character);
        }

        public override bool Backspace(ITextSurface text)
        {
            Composition.Synchronize(text);
            if (!Composition.Active)
            {
                return false;
            }

            var shown = Composition.Shown;
            Composition.Show(text, shown.Substring(0, shown.Length - 1));
            return true;
        }

        public override bool Space(ITextSurface text)
        {
            Composition.Synchronize(text);
            if (!Composition.Active)
            {
                return false;
            }

            var candidates = Candidates;
            if (candidates.Count > 0)
            {
                Composition.Complete(text, candidates[0].ToString());
            }
            else
            {
                Composition.Cancel();
            }

            return true;
        }

        public override void Choose(char candidate, ITextSurface text)
        {
            Composition.Complete(text, candidate.ToString());
        }

        public override void Cancel()
        {
            Composition.Cancel();
        }

        protected abstract bool Accepts(char character);

        protected virtual string ReadingOf(string shown)
        {
            return shown;
        }
    }
}
