using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class MainMenu : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var menu = FejdStartup.instance;
                return menu != null && menu.m_newWorldName.isActiveAndEnabled
                    ? new[] { menu.m_newWorldName, menu.m_newWorldSeed }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
