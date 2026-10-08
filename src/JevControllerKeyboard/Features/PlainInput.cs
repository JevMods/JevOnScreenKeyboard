namespace JevControllerKeyboard.Features
{
    public sealed class PlainInput : InputMethod
    {
        public override void Type(char character, ITextSurface text)
        {
            text.Insert(character.ToString());
        }
    }
}
