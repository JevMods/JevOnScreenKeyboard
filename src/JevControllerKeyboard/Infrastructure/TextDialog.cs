using System;
using System.Collections.Generic;
using GUIFramework;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class TextDialog : IFieldSource
    {
        public IReadOnlyList<GuiInputField> Fields
        {
            get
            {
                var dialog = TextInput.instance;
                return dialog != null && TextInput.IsVisible()
                    ? new[] { dialog.m_inputField }
                    : Array.Empty<GuiInputField>();
            }
        }
    }
}
