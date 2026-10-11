namespace JevOnScreenKeyboard.Infrastructure
{
    public interface IPadInput
    {
        bool IsActive { get; }

        bool Blocking { get; set; }

        bool Pressed(string button);

        bool Repeated(string button);
    }
}
