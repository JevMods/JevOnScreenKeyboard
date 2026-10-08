using TMPro;

namespace JevControllerKeyboard.Infrastructure
{
    public interface IFocusedField
    {
        TMP_InputField Current { get; }

        bool Navigation { get; set; }
    }
}
