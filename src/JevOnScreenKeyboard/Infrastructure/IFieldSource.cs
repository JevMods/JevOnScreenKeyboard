using System.Collections.Generic;
using GUIFramework;

namespace JevOnScreenKeyboard.Infrastructure
{
    public interface IFieldSource
    {
        IReadOnlyList<GuiInputField> Fields { get; }
    }
}
