using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class ServerPassword : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var menu = FejdStartup.instance;
                return menu != null && !menu.m_newWorldName.isActiveAndEnabled
                    ? new[] { menu.m_serverPassword }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
