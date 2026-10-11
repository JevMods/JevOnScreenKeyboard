namespace JevOnScreenKeyboard.Features
{
    public sealed class Composition
    {
        private int _start;

        public string Shown { get; private set; } = string.Empty;

        public bool Active => Shown.Length > 0;

        public void Synchronize(ITextSurface text)
        {
            if (!Active)
            {
                return;
            }

            var end = _start + Shown.Length;
            var intact = !text.HasSelection
                && text.Caret == end
                && end <= text.Text.Length
                && string.CompareOrdinal(text.Text, _start, Shown, 0, Shown.Length) == 0;
            if (!intact)
            {
                Cancel();
            }
        }

        public bool Show(ITextSurface text, string shown)
        {
            Synchronize(text);
            if (shown.Length == 0)
            {
                Remove(text);
                return true;
            }

            if (!Active)
            {
                if (!text.Insert(shown))
                {
                    return false;
                }

                _start = text.Caret - shown.Length;
            }
            else if (!text.Replace(_start, Shown.Length, shown))
            {
                return false;
            }

            Shown = shown;
            return true;
        }

        public bool Split(ITextSurface text, string kept, string next)
        {
            Synchronize(text);
            if (!Active || !text.Replace(_start, Shown.Length, kept + next))
            {
                return false;
            }

            _start += kept.Length;
            Shown = next;
            return true;
        }

        public void Complete(ITextSurface text, string result)
        {
            Synchronize(text);
            if (Active)
            {
                text.Replace(_start, Shown.Length, result);
            }
            else
            {
                text.Insert(result);
            }

            Cancel();
        }

        public void Cancel()
        {
            Shown = string.Empty;
        }

        private void Remove(ITextSurface text)
        {
            if (Active)
            {
                text.Replace(_start, Shown.Length, string.Empty);
            }

            Cancel();
        }
    }
}
