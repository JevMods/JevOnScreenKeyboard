using UnityEngine;

namespace JevOnScreenKeyboard.Features
{
    public interface IFocusHint
    {
        void Show(RectTransform field, string button, bool visible);
    }
}
