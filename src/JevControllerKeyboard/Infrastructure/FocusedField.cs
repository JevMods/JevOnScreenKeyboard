using GUIFramework;
using TMPro;
using UnityEngine.EventSystems;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class FocusedField : IFocusedField
    {
        public TMP_InputField Current
        {
            get
            {
                var eventSystem = EventSystem.current;
                if (eventSystem == null || eventSystem.currentSelectedGameObject == null)
                {
                    return null;
                }

                var selected = eventSystem.currentSelectedGameObject;
                var field = selected.GetComponent<GuiInputField>();
                return field != null && field.isFocused && field.isActiveAndEnabled ? field : null;
            }
        }

        public bool Navigation
        {
            get => EventSystem.current == null || EventSystem.current.sendNavigationEvents;
            set
            {
                if (EventSystem.current != null)
                {
                    EventSystem.current.sendNavigationEvents = value;
                }
            }
        }
    }
}
