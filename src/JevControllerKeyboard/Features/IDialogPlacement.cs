using UnityEngine;

namespace JevControllerKeyboard.Features
{
    public interface IDialogPlacement
    {
        void Place(RectTransform field, IKeyboardView keyboard);

        void Restore();
    }
}
