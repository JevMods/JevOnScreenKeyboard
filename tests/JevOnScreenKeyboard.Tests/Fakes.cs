using System;
using JevOnScreenKeyboard.Features;

namespace JevOnScreenKeyboard.Tests
{
    public sealed class Surface : ITextSurface
    {
        public string Text { get; private set; } = string.Empty;

        public int Caret { get; set; }

        public bool HasSelection => false;

        public bool Insert(string text)
        {
            Text = Text.Insert(Caret, text);
            Caret += text.Length;
            return true;
        }

        public bool Replace(int start, int length, string text)
        {
            Text = Text.Remove(start, length).Insert(start, text);
            Caret = start + text.Length;
            return true;
        }
    }

    public sealed class Editor : ITextEditor
    {
        private readonly Surface _surface = new Surface();

        public string Text => _surface.Text;

        public int Caret => _surface.Caret;

        public bool HasSelection => false;

        public bool Insert(string text)
        {
            return _surface.Insert(text);
        }

        public bool Replace(int start, int length, string text)
        {
            return _surface.Replace(start, length, text);
        }

        public void Backspace()
        {
            if (_surface.Caret > 0)
            {
                _surface.Replace(_surface.Caret - 1, 1, string.Empty);
            }
        }

        public void MoveCaret(int step)
        {
            _surface.Caret = Math.Max(0, Math.Min(_surface.Text.Length, _surface.Caret + step));
        }

        public void Submit()
        {
        }
    }
}
