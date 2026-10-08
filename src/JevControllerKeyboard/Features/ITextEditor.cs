namespace JevControllerKeyboard.Features
{
    public interface ITextEditor : ITextSurface
    {
        void Backspace();

        void MoveCaret(int step);

        void Submit();
    }
}
