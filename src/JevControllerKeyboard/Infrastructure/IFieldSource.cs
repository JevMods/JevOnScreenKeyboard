using System.Collections.Generic;
using GUIFramework;

namespace JevControllerKeyboard.Infrastructure
{
    public interface IFieldSource
    {
        IReadOnlyList<GuiInputField> Fields { get; }
    }
}
