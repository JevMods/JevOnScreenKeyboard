using UnityEngine;

namespace JevOnScreenKeyboard.Features
{
    public interface IDialogPlacement
    {
        void Place(RectTransform field, IKeyboardView keyboard);

        void Restore();
    }
}
