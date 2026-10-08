using System.Collections.Generic;
using JevControllerKeyboard.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JevControllerKeyboard.Features
{
    public sealed class KeyboardView : IKeyboardView
    {
        private const float KeyWidth = 76f;
        private const float KeyHeight = 56f;
        private const float Gap = 6f;
        private const float Padding = 30f;
        private const float HintHeight = 36f;
        private const float BottomOffset = 24f;
        private const float KeyFontSize = 28f;
        private const float HintFontSize = 20f;
        private const float MinFontSize = 6f;
        private const float KeyTextMargin = 14f;

        private const string Hint =
            "$KEY_ButtonX $menu_remove      $KEY_ButtonY $button_space      "
            + "$KEY_LTrigger Shift      $KEY_LBumper $KEY_RBumper $inventory_move      "
            + "$KEY_RTrigger $menu_more      $KEY_Start $menu_ok      $KEY_ButtonB $menu_close";

        private readonly IGameAssets _assets;
        private readonly KeyCaption _captions;
        private readonly GameObject _canvas;
        private readonly RectTransform _panel;
        private readonly List<List<Image>> _keys = new List<List<Image>>();
        private readonly List<List<TextMeshProUGUI>> _labels = new List<List<TextMeshProUGUI>>();
        private readonly Vector3[] _corners = new Vector3[4];
        private TextMeshProUGUI _hint;
        private int _builtVersion = -1;
        private int _maxRows = KeyLayout.DefaultRows;

        public KeyboardView(Transform parent, IGameAssets assets, IGameLanguage language)
        {
            _assets = assets;
            _captions = new KeyCaption(language);
            _canvas = new GameObject("Canvas", typeof(Canvas));
            _canvas.transform.SetParent(parent, false);
            var canvas = _canvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            _panel = CreateImage("Panel", _canvas.transform).rectTransform;
            _panel.anchorMin = new Vector2(0.5f, 0f);
            _panel.anchorMax = new Vector2(0.5f, 0f);
            _panel.pivot = new Vector2(0.5f, 0f);
            _panel.anchoredPosition = new Vector2(0f, BottomOffset);
            _canvas.SetActive(false);
        }

        public Rect ScreenRect
        {
            get
            {
                _panel.GetWorldCorners(_corners);
                return Rect.MinMaxRect(_corners[0].x, _corners[0].y, _corners[2].x, _corners[2].y);
            }
        }

        public float MaxHeight => HeightOf(_maxRows) * _panel.lossyScale.y;

        public void MoveBy(Vector2 offset)
        {
            _panel.position += (Vector3)offset;
        }

        public void ResetPosition()
        {
            _panel.anchoredPosition = new Vector2(0f, BottomOffset);
        }

        public void Show(float scale)
        {
            if (_hint == null)
            {
                _hint = CreateHint();
            }

            var panel = _panel.GetComponent<Image>();
            ApplySprite(panel, GameStyle.PanelSprite);
            panel.material = _assets.Material(GameStyle.PanelMaterial);
            _panel.localScale = new Vector3(scale, scale, 1f);
            ResetPosition();
            _builtVersion = -1;
            _canvas.SetActive(true);
        }

        public void Hide()
        {
            _canvas.SetActive(false);
        }

        public void Refresh(KeyLayout layout)
        {
            _maxRows = layout.MaxRows;
            if (_builtVersion != layout.Version)
            {
                Build(layout);
            }

            for (var row = 0; row < layout.Rows.Count; row++)
            {
                for (var index = 0; index < layout.Rows[row].Count; index++)
                {
                    var key = layout.Rows[row][index];
                    var selected = row == layout.Row && index == layout.Index;
                    ApplySprite(_keys[row][index], KeySprite(layout, key, selected));
                    _labels[row][index].text = _captions.Of(layout, key);
                }
            }
        }

        private static float HeightOf(int rows)
        {
            return Padding * 2f + rows * (KeyHeight + Gap) + HintHeight;
        }

        private static string KeySprite(KeyLayout layout, Key key, bool selected)
        {
            if (selected)
            {
                return GameStyle.ButtonHighlightSprite;
            }

            var toggled = (key.Action == KeyAction.Shift && layout.Shifted)
                || (key.Action == KeyAction.Symbols && layout.ShowSymbols);
            return toggled ? GameStyle.ButtonPressedSprite : GameStyle.ButtonSprite;
        }

        private static Image CreateImage(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
            return image;
        }

        private void ApplySprite(Image image, string name)
        {
            image.sprite = _assets.Sprite(name);
            image.type = Image.Type.Sliced;
        }

        private TextMeshProUGUI CreateHint()
        {
            var go = new GameObject("Hint", typeof(RectTransform));
            go.SetActive(false);
            var rect = (RectTransform)go.transform;
            rect.SetParent(_panel, false);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, Padding - Gap);
            rect.sizeDelta = new Vector2(-Padding * 2f, HintHeight);

            var text = GameText.Create(
                rect,
                _assets,
                GameStyle.SansFont,
                GameStyle.SansOutlineMaterial,
                HintFontSize,
                MinFontSize,
                Color.white,
                TextAlignmentOptions.Center);
            text.text = Hint;
            go.AddComponent<Localize>();
            go.SetActive(true);
            return text;
        }

        private void Build(KeyLayout layout)
        {
            foreach (var row in _keys)
            {
                foreach (var key in row)
                {
                    Object.Destroy(key.gameObject);
                }
            }

            _keys.Clear();
            _labels.Clear();
            _panel.sizeDelta = new Vector2(
                Padding * 2f + layout.Width * KeyWidth + (layout.Width - 1) * Gap,
                HeightOf(layout.Rows.Count));

            for (var row = 0; row < layout.Rows.Count; row++)
            {
                var keys = new List<Image>();
                var labels = new List<TextMeshProUGUI>();
                foreach (var key in layout.Rows[row])
                {
                    var image = CreateKey(row, key);
                    keys.Add(image);
                    labels.Add(image.GetComponentInChildren<TextMeshProUGUI>());
                }

                _keys.Add(keys);
                _labels.Add(labels);
            }

            _builtVersion = layout.Version;
        }

        private Image CreateKey(int row, Key key)
        {
            var image = CreateImage("Key", _panel);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(
                Padding + key.X * (KeyWidth + Gap),
                -(Padding + row * (KeyHeight + Gap)));
            rect.sizeDelta = new Vector2(
                key.Width * KeyWidth + (key.Width - 1f) * Gap,
                KeyHeight);

            var label = GameText.Create(
                rect,
                _assets,
                GameStyle.SerifFont,
                GameStyle.SerifOutlineMaterial,
                KeyFontSize,
                MinFontSize,
                GameStyle.ButtonText,
                TextAlignmentOptions.Center);
            label.fontStyle = FontStyles.Bold;
            label.margin = new Vector4(KeyTextMargin, 0f, KeyTextMargin, 0f);
            return image;
        }
    }
}
