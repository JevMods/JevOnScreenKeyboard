namespace JevOnScreenKeyboard.Features
{
    public sealed class Key
    {
        public Key(KeyAction action, char character, float x, float width)
            : this(action, character, character, x, width)
        {
        }

        public Key(KeyAction action, char character, char shiftCharacter, float x, float width)
        {
            Action = action;
            Character = character;
            ShiftCharacter = shiftCharacter;
            X = x;
            Width = width;
        }

        public KeyAction Action { get; }

        public char Character { get; }

        public char ShiftCharacter { get; }

        public float X { get; }

        public float Width { get; }

        public float Center => X + Width / 2f;
    }
}
