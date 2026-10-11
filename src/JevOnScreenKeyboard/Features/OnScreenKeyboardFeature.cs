using JevOnScreenKeyboard.Config;
using JevOnScreenKeyboard.Infrastructure;
using TMPro;
using UnityEngine;

namespace JevOnScreenKeyboard.Features
{
    public sealed class OnScreenKeyboardFeature : IFeature
    {
        private const float BaseScale = 0.5f;

        private readonly IPadInput _gamepad;
        private readonly IFocusedField _focus;
        private readonly IKeyboardView _view;
        private readonly IModSettings _settings;
        private readonly IGameLanguage _language;
        private readonly ILayoutCatalog _layouts;
        private readonly IDialogPlacement _placement;
        private readonly IUiScale _uiScale;
        private readonly KeyLayout _layout = new KeyLayout();
        private TMP_InputField _field;
        private TMP_InputField _dismissed;
        private KeyboardSession _session;
        private bool _savedNavigation;
        private bool _languageChanged;

        public OnScreenKeyboardFeature(
            IPadInput gamepad,
            IFocusedField focus,
            IKeyboardView view,
            IModSettings settings,
            IGameLanguage language,
            ILayoutCatalog layouts,
            IDialogPlacement placement,
            IUiScale uiScale)
        {
            _gamepad = gamepad;
            _focus = focus;
            _view = view;
            _settings = settings;
            _language = language;
            _layouts = layouts;
            _placement = placement;
            _uiScale = uiScale;
        }

        public void Enable()
        {
            _language.Changed += OnLanguageChanged;
        }

        public void Update()
        {
            var field = _focus.Current;
            if (field != _dismissed || _gamepad.Pressed(PadButton.A))
            {
                _dismissed = null;
            }

            if (field == null || field == _dismissed || !_gamepad.IsActive)
            {
                Close();
                return;
            }

            if (field != _field || _languageChanged)
            {
                Open(field);
            }

            _focus.Navigation = false;
            HandleInput();
        }

        public void Disable()
        {
            _language.Changed -= OnLanguageChanged;
            Close();
        }

        private void OnLanguageChanged()
        {
            _languageChanged = true;
        }

        private void Open(TMP_InputField field)
        {
            if (_field == null)
            {
                _savedNavigation = _focus.Navigation;
            }

            _field = field;
            _languageChanged = false;
            _session = new KeyboardSession(_layout, new InputFieldEditor(field));
            _gamepad.Blocking = true;
            _layout.Reset(_layouts.For(_language.Current));
            _view.Show(_settings.Scale * BaseScale * _uiScale.Of(field.transform));
            _view.Refresh(_layout);
            _placement.Place((RectTransform)field.transform, _view);
        }

        private void Close()
        {
            if (_field == null)
            {
                return;
            }

            _focus.Navigation = _savedNavigation;
            _field = null;
            _session = null;
            _gamepad.Blocking = false;
            _placement.Restore();
            _view.Hide();
        }

        private void HandleInput()
        {
            if (_gamepad.Pressed(PadButton.B))
            {
                Cancel();
                return;
            }

            var version = _layout.Version;
            var before = State();
            Navigate();
            Edit();
            if (before != State())
            {
                _view.Refresh(_layout);
            }

            if (version != _layout.Version)
            {
                _placement.Place((RectTransform)_field.transform, _view);
            }
        }

        private void Cancel()
        {
            if (_session.AccentsOpen)
            {
                _session.CloseAccents();
                _view.Refresh(_layout);
                return;
            }

            _dismissed = _field;
            _field.DeactivateInputField();
            Close();
        }

        private (int, int, bool, int, int) State()
        {
            return (_layout.Row, _layout.Index, _layout.Shifted, _layout.Version, _layout.Revision);
        }

        private void Navigate()
        {
            if (Moved(PadButton.DPadLeft, PadButton.LeftStickLeft))
            {
                _layout.MoveHorizontal(-1);
            }

            if (Moved(PadButton.DPadRight, PadButton.LeftStickRight))
            {
                _layout.MoveHorizontal(1);
            }

            if (Moved(PadButton.DPadUp, PadButton.LeftStickUp))
            {
                _layout.MoveVertical(-1);
            }

            if (Moved(PadButton.DPadDown, PadButton.LeftStickDown))
            {
                _layout.MoveVertical(1);
            }

            _session.DismissAccentsOnNavigation();
        }

        private bool Moved(string dpad, string stick)
        {
            return _gamepad.Repeated(dpad) || _gamepad.Repeated(stick);
        }

        private void Edit()
        {
            if (_gamepad.Pressed(PadButton.LeftTrigger))
            {
                _layout.Shifted = !_layout.Shifted;
            }

            if (_gamepad.Pressed(PadButton.RightTrigger))
            {
                _session.ToggleChoices();
            }

            if (_gamepad.Repeated(PadButton.X))
            {
                _session.Backspace();
            }

            if (_gamepad.Repeated(PadButton.Y))
            {
                _session.Space();
            }

            if (_gamepad.Repeated(PadButton.LeftBumper))
            {
                _session.MoveCaret(-1);
            }

            if (_gamepad.Repeated(PadButton.RightBumper))
            {
                _session.MoveCaret(1);
            }

            if (_gamepad.Repeated(PadButton.A))
            {
                PressSelected();
            }

            if (_gamepad.Pressed(PadButton.Start))
            {
                _session.Submit();
            }
        }

        private void PressSelected()
        {
            var key = _layout.Selected;
            if (KeyboardSession.Repeats(key) || _gamepad.Pressed(PadButton.A))
            {
                _session.Press(key);
            }
        }
    }
}
