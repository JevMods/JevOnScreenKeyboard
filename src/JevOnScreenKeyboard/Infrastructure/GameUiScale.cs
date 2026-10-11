using UnityEngine;

namespace JevOnScreenKeyboard.Infrastructure
{
    public sealed class GameUiScale : IUiScale
    {
        public float Of(Transform element)
        {
            var canvas = element.GetComponentInParent<Canvas>();
            return canvas != null ? canvas.rootCanvas.scaleFactor : 1f;
        }
    }
}
