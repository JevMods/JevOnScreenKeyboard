using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class MapPin : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var map = Minimap.instance;
                return map != null && map.m_nameInput.gameObject.activeInHierarchy
                    ? new[] { map.m_nameInput }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
