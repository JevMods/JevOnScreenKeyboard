using UnityEngine;

namespace JevControllerKeyboard.Features
{
    public interface IKeyboardView
    {
        Rect ScreenRect { get; }

        float MaxHeight { get; }

        void Show(float scale);

        void Hide();

        void Refresh(KeyLayout layout);

        void MoveBy(Vector2 offset);

        void ResetPosition();
    }
}
