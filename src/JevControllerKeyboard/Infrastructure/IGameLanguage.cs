using System;

namespace JevControllerKeyboard.Infrastructure
{
    public interface IGameLanguage
    {
        event Action Changed;

        string Current { get; }

        string Localize(string text);
    }
}
