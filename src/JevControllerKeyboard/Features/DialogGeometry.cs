using UnityEngine;
using UnityEngine.UI;

namespace JevControllerKeyboard.Features
{
    public sealed class DialogGeometry
    {
        private const float FullScreenShare = 0.9f;

        private readonly Vector3[] _corners = new Vector3[4];

        public RectTransform FindDialog(RectTransform field)
        {
            var dialog = field;
            var parent = field.parent as RectTransform;
            while (parent != null && !IsRoot(parent))
            {
                dialog = parent;
                parent = parent.parent as RectTransform;
            }

            return dialog;
        }

        public Rect VisualBounds(RectTransform dialog)
        {
            var bounds = ScreenBounds(dialog);
            foreach (RectTransform child in dialog)
            {
                if (IsBackground(child))
                {
                    var b = ScreenBounds(child);
                    bounds = Rect.MinMaxRect(
                        Mathf.Min(bounds.xMin, b.xMin),
                        Mathf.Min(bounds.yMin, b.yMin),
                        Mathf.Max(bounds.xMax, b.xMax),
                        Mathf.Max(bounds.yMax, b.yMax));
                }
            }

            return bounds;
        }

        public Rect ScreenBounds(RectTransform rect)
        {
            rect.GetWorldCorners(_corners);
            var camera = CanvasCamera(rect);
            var min = RectTransformUtility.WorldToScreenPoint(camera, _corners[0]);
            var max = RectTransformUtility.WorldToScreenPoint(camera, _corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        public Vector3 WorldOffset(RectTransform rect, Vector2 screenOffset)
        {
            var camera = CanvasCamera(rect);
            var center = ScreenBounds(rect).center;
            var parent = rect.parent as RectTransform ?? rect;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent,
                center,
                camera,
                out var from);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent,
                center + screenOffset,
                camera,
                out var to);
            return to - from;
        }

        private static Camera CanvasCamera(Transform transform)
        {
            var canvas = transform.GetComponentInParent<Canvas>();
            var overlay = canvas == null
                || canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
            return overlay ? null : canvas.rootCanvas.worldCamera;
        }

        private static bool IsBackground(RectTransform child)
        {
            var stretched = child.anchorMin == Vector2.zero && child.anchorMax == Vector2.one;
            return stretched && child.gameObject.activeSelf && child.GetComponent<Image>() != null;
        }

        private bool IsRoot(RectTransform rect)
        {
            return rect.GetComponent<Canvas>() != null || IsFullScreen(rect);
        }

        private bool IsFullScreen(RectTransform rect)
        {
            var bounds = ScreenBounds(rect);
            return bounds.width >= Screen.width * FullScreenShare
                && bounds.height >= Screen.height * FullScreenShare;
        }
    }
}
