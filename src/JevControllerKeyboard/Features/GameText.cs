using JevControllerKeyboard.Infrastructure;
using TMPro;
using UnityEngine;

namespace JevControllerKeyboard.Features
{
    public static class GameText
    {
        public static TextMeshProUGUI Create(
            Transform parent,
            IGameAssets assets,
            string font,
            string material,
            float size,
            float minSize,
            Color color,
            TextAlignmentOptions alignment)
        {
            var go = new GameObject("Label", typeof(RectTransform));
            go.SetActive(false);
            go.AddComponent<TextMeshProUGUI>();
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = assets.Font(font) ?? text.font;
            go.SetActive(true);
            var shared = assets.Material(material);
            if (shared != null)
            {
                text.fontSharedMaterial = shared;
            }

            text.fontSize = size;
            text.enableAutoSizing = minSize < size;
            text.fontSizeMin = minSize;
            text.fontSizeMax = size;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.raycastTarget = false;
            var rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            return text;
        }
    }
}
