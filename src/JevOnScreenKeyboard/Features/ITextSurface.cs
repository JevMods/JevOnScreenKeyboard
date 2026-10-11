namespace JevOnScreenKeyboard.Features
{
    public interface ITextSurface
    {
        string Text { get; }

        int Caret { get; }

        bool HasSelection { get; }

        bool Insert(string text);

        bool Replace(int start, int length, string text);
    }
}
