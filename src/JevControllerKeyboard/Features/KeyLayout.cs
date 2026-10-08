using System;
using System.Collections.Generic;

namespace JevControllerKeyboard.Features
{
    public sealed class KeyLayout
    {
        public const int DefaultRows = 6;

        private readonly List<List<Key>> _rows = new List<List<Key>>();
        private readonly KeyChoices _choices = new KeyChoices();

        private LanguageLayout _native = LanguageLayouts.English;
        private LanguageLayout _language = LanguageLayouts.English;
        private List<Key> _digitRow = new List<Key>();
        private bool _showSymbols;
        private float _anchor;

        public KeyLayout()
        {
            Reset(LanguageLayouts.English);
        }

        public IReadOnlyList<IReadOnlyList<Key>> Rows => _rows;

        public IInputMethod Input { get; private set; }

        public int Width { get; private set; }

        public int Version { get; private set; }

        public int Revision { get; private set; }

        public int Row { get; private set; }

        public int Index { get; private set; }

        public Key Selected => _rows[Row][Index];

        public bool Shifted { get; set; }

        public bool ChoicesShown => _choices.Shown;

        public bool ChoicesFocused => _choices.Focused;

        public bool ShowSymbols
        {
            get => _showSymbols;
            set
            {
                if (_showSymbols != value)
                {
                    _showSymbols = value;
                    Build();
                }
            }
        }

        public int MaxRows =>
            Math.Max(KeyRowBuilder.RowsOf(_native), KeyRowBuilder.RowsOf(LanguageLayouts.English));

        public bool CanSwitchLanguage => _native != LanguageLayouts.English;

        private float ChoiceOffset => (Width - KeyChoices.Slots) / 2f;

        public void Reset(LanguageLayout language)
        {
            if (_native != language)
            {
                _native = language;
                _language = language;
            }

            Input = _language.CreateInput();
            _showSymbols = false;
            Shifted = false;
            Row = 0;
            Index = 0;
            Build();
        }

        public void SwitchLanguage()
        {
            _language = _language == _native ? LanguageLayouts.English : _native;
            Input = _language.CreateInput();
            _showSymbols = false;
            Build();
        }

        public char CharacterOf(Key key)
        {
            return Shifted ? key.ShiftCharacter : key.Character;
        }

        public string Label(Key key)
        {
            switch (key.Action)
            {
                case KeyAction.Character:
                    return CharacterOf(key).ToString();
                case KeyAction.Choice:
                case KeyAction.NextPage:
                    return key.Character == '\0' ? string.Empty : key.Character.ToString();
                case KeyAction.Shift:
                    return "Shift";
                case KeyAction.Symbols:
                    return _showSymbols ? _language.Label : "#+=";
                case KeyAction.Language:
                    return (_language == _native ? LanguageLayouts.English : _native).Label;
                case KeyAction.Space:
                    return "$button_space";
                case KeyAction.Backspace:
                    return "$menu_remove";
                default:
                    return "$menu_ok";
            }
        }

        public void MoveHorizontal(int step)
        {
            var count = _rows[Row].Count;
            Index = (Index + step + count) % count;
            _anchor = Selected.Center;
        }

        public void MoveVertical(int step)
        {
            Row = (Row + step + _rows.Count) % _rows.Count;
            Index = NearestIndex(_rows[Row], _anchor);
        }

        public void ShowChoices(IReadOnlyList<char> items)
        {
            if (!_showSymbols && _choices.Show(items))
            {
                ReplaceTopRow(_choices.Row(ChoiceOffset));
            }
        }

        public void ClearChoices()
        {
            if (_choices.Shown)
            {
                _choices.Clear();
                ReplaceTopRow(_digitRow);
            }
        }

        public void NextChoicePage()
        {
            _choices.NextPage();
            ReplaceTopRow(_choices.Row(ChoiceOffset));
        }

        public void FocusChoices()
        {
            if (!_choices.Shown || _choices.Focused)
            {
                return;
            }

            _choices.Focus(Row, Index);
            Row = 0;
            Index = 0;
            _anchor = Selected.Center;
        }

        public void LeaveChoices(bool restore)
        {
            if (!_choices.Focused)
            {
                return;
            }

            _choices.Release();
            if (restore)
            {
                Row = Math.Min(_choices.ReturnRow, _rows.Count - 1);
                Index = Math.Min(_choices.ReturnIndex, _rows[Row].Count - 1);
                _anchor = Selected.Center;
            }
        }

        private static int NearestIndex(List<Key> row, float x)
        {
            var best = 0;
            for (var index = 1; index < row.Count; index++)
            {
                if (Math.Abs(row[index].Center - x) < Math.Abs(row[best].Center - x))
                {
                    best = index;
                }
            }

            return best;
        }

        private void Build()
        {
            var wasOnSpecialRow = _rows.Count > 0 && Row == _rows.Count - 1;
            var rows = KeyRowBuilder.Build(
                _language,
                _showSymbols,
                CanSwitchLanguage,
                out var width);
            Width = width;
            _rows.Clear();
            _rows.AddRange(rows);
            _digitRow = _showSymbols ? new List<Key>() : _rows[0];
            _choices.Clear();
            _choices.Release();
            Row = wasOnSpecialRow ? _rows.Count - 1 : Math.Min(Row, _rows.Count - 1);
            Index = NearestIndex(_rows[Row], _anchor);
            _anchor = Selected.Center;
            Version++;
        }

        private void ReplaceTopRow(List<Key> row)
        {
            _rows[0] = row;
            Revision++;
        }
    }
}
