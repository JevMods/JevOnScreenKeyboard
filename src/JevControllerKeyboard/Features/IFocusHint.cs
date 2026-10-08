using UnityEngine;

namespace JevControllerKeyboard.Features
{
    public interface IFocusHint
    {
        void Show(RectTransform field, string button, bool visible);
    }
}
