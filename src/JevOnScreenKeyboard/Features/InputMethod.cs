using System;
using System.Collections.Generic;

namespace JevOnScreenKeyboard.Features
{
    public abstract class InputMethod : IInputMethod
    {
        public virtual bool Choosing => false;

        public virtual IReadOnlyList<char> Candidates => Array.Empty<char>();

        public abstract void Type(char character, ITextSurface text);

        public virtual bool Backspace(ITextSurface text)
        {
            return false;
        }

        public virtual bool Space(ITextSurface text)
        {
            return false;
        }

        public virtual void Choose(char candidate, ITextSurface text)
        {
            text.Insert(candidate.ToString());
        }

        public virtual void Cancel()
        {
        }
    }
}
