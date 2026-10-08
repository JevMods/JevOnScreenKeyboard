using System;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class GameLanguage : IGameLanguage
    {
        public event Action Changed
        {
            add => Localization.OnLanguageChange += value;
            remove => Localization.OnLanguageChange -= value;
        }

        public string Current => Localization.instance?.GetSelectedLanguage();

        public string Localize(string text)
        {
            return Localization.instance != null ? Localization.instance.Localize(text) : text;
        }
    }
}
