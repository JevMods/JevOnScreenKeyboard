using JevOnScreenKeyboard.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JevOnScreenKeyboard.Features
{
    public sealed class FocusHint : IFocusHint
    {
        private const string HintName = "JevFocusHint";
        private const float HintSize = 30f;
        private const float HintOffset = 4f;
        private const float GlyphFontSize = 20f;

        private readonly IGameAssets _assets;
        private readonly IGameLanguage _language;

        public FocusHint(IGameAssets assets, IGameLanguage language)
        {
            _assets = assets;
            _language = language;
        }

        public void Show(RectTransform field, string button, bool visible)
        {
            var hint = field.Find(HintName);
            if (hint == null)
            {
                hint = Create(field);
            }

            if (hint.gameObject.activeSelf != visible)
            {
                hint.gameObject.SetActive(visible);
            }

            var glyph = hint.GetComponentInChildren<TextMeshProUGUI>(true);
            var text = _language.Localize("$KEY_" + button);
            if (glyph.text != text)
            {
                glyph.text = text;
            }
        }

        private Transform Create(RectTransform field)
        {
            var root = new GameObject(HintName, typeof(RectTransform), typeof(LayoutElement));
            root.SetActive(false);
            root.GetComponent<LayoutElement>().ignoreLayout = true;
            var rect = root.GetComponent<RectTransform>();
            rect.SetParent(field, false);
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(HintOffset, 0f);
            rect.sizeDelta = new Vector2(HintSize, HintSize);

            var gamepad = new GameObject("Gamepad", typeof(RectTransform));
            var gamepadRect = gamepad.GetComponent<RectTransform>();
            gamepadRect.SetParent(rect, false);
            gamepadRect.anchorMin = Vector2.zero;
            gamepadRect.anchorMax = Vector2.one;
            gamepadRect.sizeDelta = Vector2.zero;
            GameText.Create(
                gamepadRect,
                _assets,
                GameStyle.SansFont,
                GameStyle.SansOutlineMaterial,
                GlyphFontSize,
                GlyphFontSize,
                Color.white,
                TextAlignmentOptions.Midline);
            return rect;
        }
    }
}
