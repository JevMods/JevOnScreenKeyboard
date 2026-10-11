using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevOnScreenKeyboard.Infrastructure
{
    public sealed class ServerFilter : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var list = ServerListGui.s_instance;
                return list != null && list.m_filterInputField.isActiveAndEnabled
                    ? new[] { list.m_filterInputField }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
