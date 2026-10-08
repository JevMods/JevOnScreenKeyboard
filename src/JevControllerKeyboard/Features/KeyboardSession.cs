namespace JevControllerKeyboard.Features
{
    public sealed class KeyboardSession
    {
        private readonly KeyLayout _layout;
        private readonly ITextEditor _editor;

        public KeyboardSession(KeyLayout layout, ITextEditor editor)
        {
            _layout = layout;
            _editor = editor;
        }

        public bool AccentsOpen { get; private set; }

        public static bool Repeats(Key key)
        {
            return key.Action != KeyAction.Choice && key.Action != KeyAction.NextPage;
        }

        public void Press(Key key)
        {
            switch (key.Action)
            {
                case KeyAction.Character:
                    _layout.Input.Type(_layout.CharacterOf(key), _editor);
                    Settle();
                    break;
                case KeyAction.Choice:
                    Choose(key.Character);
                    break;
                case KeyAction.NextPage:
                    _layout.NextChoicePage();
                    break;
                case KeyAction.Shift:
                    _layout.Shifted = !_layout.Shifted;
                    break;
                case KeyAction.Symbols:
                    Reset();
                    _layout.ShowSymbols = !_layout.ShowSymbols;
                    break;
                case KeyAction.Language:
                    Reset();
                    _layout.SwitchLanguage();
                    break;
                case KeyAction.Space:
                    Space();
                    break;
                case KeyAction.Backspace:
                    Backspace();
                    break;
                default:
                    Submit();
                    break;
            }
        }

        public void Backspace()
        {
            if (!_layout.Input.Backspace(_editor))
            {
                _editor.Backspace();
            }

            Settle();
        }

        public void Space()
        {
            if (!_layout.Input.Space(_editor))
            {
                _editor.Insert(" ");
            }

            Settle();
        }

        public void MoveCaret(int step)
        {
            _editor.MoveCaret(step);
            _layout.Input.Cancel();
            Settle();
        }

        public void Submit()
        {
            _editor.Submit();
        }

        public void ToggleChoices()
        {
            if (_layout.ChoicesFocused)
            {
                LeaveChoices();
            }
            else if (_layout.ChoicesShown)
            {
                _layout.FocusChoices();
            }
            else
            {
                OpenAccents();
            }
        }

        public void CloseAccents()
        {
            AccentsOpen = false;
            _layout.ClearChoices();
            _layout.LeaveChoices(true);
        }

        public void DismissAccentsOnNavigation()
        {
            if (AccentsOpen && _layout.Row != 0)
            {
                AccentsOpen = false;
                _layout.ClearChoices();
                _layout.LeaveChoices(false);
            }
        }

        private void LeaveChoices()
        {
            if (AccentsOpen)
            {
                CloseAccents();
            }
            else
            {
                _layout.LeaveChoices(true);
            }
        }

        private void OpenAccents()
        {
            var key = _layout.Selected;
            if (key.Action != KeyAction.Character || _layout.ShowSymbols)
            {
                return;
            }

            var variants = LetterVariants.Of(_layout.CharacterOf(key));
            if (variants.Length > 0)
            {
                AccentsOpen = true;
                _layout.ShowChoices(variants.ToCharArray());
                _layout.FocusChoices();
            }
        }

        private void Choose(char choice)
        {
            if (choice == '\0')
            {
                return;
            }

            if (_layout.Input.Choosing)
            {
                _layout.Input.Choose(choice, _editor);
            }
            else
            {
                _editor.Insert(choice.ToString());
            }

            AccentsOpen = false;
            Settle();
        }

        private void Reset()
        {
            _layout.Input.Cancel();
            AccentsOpen = false;
        }

        private void Settle()
        {
            if (_layout.Input.Choosing)
            {
                _layout.ShowChoices(_layout.Input.Candidates);
            }
            else if (!AccentsOpen)
            {
                _layout.ClearChoices();
                _layout.LeaveChoices(true);
            }
        }
    }
}
