using System;

namespace JevOnScreenKeyboard.Features
{
    public sealed class PinyinInput : CandidateInput
    {
        private const int MaxSyllableLength = 6;

        public PinyinInput(Func<CandidateTable> table)
            : base(table, MaxSyllableLength)
        {
        }

        protected override bool Accepts(char character)
        {
            return character >= 'a' && character <= 'z';
        }
    }
}
