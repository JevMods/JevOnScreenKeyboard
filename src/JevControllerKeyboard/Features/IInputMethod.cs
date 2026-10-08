using System.Collections.Generic;

namespace JevControllerKeyboard.Features
{
    public interface IInputMethod
    {
        bool Choosing { get; }

        IReadOnlyList<char> Candidates { get; }

        void Type(char character, ITextSurface text);

        bool Backspace(ITextSurface text);

        bool Space(ITextSurface text);

        void Choose(char candidate, ITextSurface text);

        void Cancel();
    }
}
