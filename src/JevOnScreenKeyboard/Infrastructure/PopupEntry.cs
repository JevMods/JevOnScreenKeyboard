using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevOnScreenKeyboard.Infrastructure
{
    public sealed class PopupEntry : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var field = UnifiedPopup.IsVisible() ? UnifiedPopup.instance.textEntryField : null;
                return field != null && field.gameObject.activeInHierarchy
                    ? new[] { field }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
