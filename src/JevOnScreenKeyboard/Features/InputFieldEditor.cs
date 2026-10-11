using TMPro;
using UnityEngine;

namespace JevOnScreenKeyboard.Features
{
    public sealed class InputFieldEditor : ITextEditor
    {
        private readonly TMP_InputField _field;

        public InputFieldEditor(TMP_InputField field)
        {
            _field = field;
        }

        public string Text => _field.text;

        public int Caret =>
            Mathf.Clamp(_field.selectionStringFocusPosition, 0, _field.text.Length);

        public bool HasSelection =>
            _field.selectionStringAnchorPosition != _field.selectionStringFocusPosition;

        public bool Insert(string typed)
        {
            var text = RemoveSelection(out var position);
            var inserted = false;
            foreach (var character in typed)
            {
                var accepted = Validate(text, position, character);
                if (accepted == '\0' || IsFull(text))
                {
                    continue;
                }

                text = text.Insert(position, accepted.ToString());
                position++;
                inserted = true;
            }

            SetText(text, position);
            return inserted;
        }

        public bool Replace(int start, int length, string replacement)
        {
            var current = _field.text;
            if (start < 0 || length < 0 || start + length > current.Length)
            {
                return false;
            }

            var text = current.Remove(start, length);
            var position = start;
            foreach (var character in replacement)
            {
                var accepted = Validate(text, position, character);
                if (accepted == '\0' || IsFull(text))
                {
                    return false;
                }

                text = text.Insert(position, accepted.ToString());
                position++;
            }

            SetText(text, position);
            return true;
        }

        public void Backspace()
        {
            var hadSelection = HasSelection;
            var text = RemoveSelection(out var position);
            if (!hadSelection && position > 0)
            {
                position--;
                text = text.Remove(position, 1);
            }

            SetText(text, position);
        }

        public void MoveCaret(int step)
        {
            var position = _field.selectionStringFocusPosition + step;
            _field.stringPosition = Mathf.Clamp(position, 0, _field.text.Length);
            ShowCaret();
        }

        public void Submit()
        {
            _field.SendOnSubmit();
            _field.DeactivateInputField();
        }

        private bool IsFull(string text)
        {
            return _field.characterLimit > 0 && text.Length >= _field.characterLimit;
        }

        private string RemoveSelection(out int position)
        {
            var text = _field.text;
            var anchor = Mathf.Clamp(_field.selectionStringAnchorPosition, 0, text.Length);
            var focus = Mathf.Clamp(_field.selectionStringFocusPosition, 0, text.Length);
            position = Mathf.Min(anchor, focus);
            return text.Remove(position, Mathf.Abs(focus - anchor));
        }

        private char Validate(string text, int position, char character)
        {
            if (_field.onValidateInput != null)
            {
                return _field.onValidateInput(text, position, character);
            }

            if (_field.characterValidation != TMP_InputField.CharacterValidation.None)
            {
                return _field.Validate(text, position, character);
            }

            return character;
        }

        private void SetText(string text, int position)
        {
            _field.text = text;
            _field.stringPosition = position;
            ShowCaret();
        }

        private void ShowCaret()
        {
            _field.UpdateLabel();
            _field.SetCaretVisible();
        }
    }
}
