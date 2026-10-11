using System.Collections.Generic;
using GUIFramework;
using JevOnScreenKeyboard.Infrastructure;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JevOnScreenKeyboard.Features
{
    public sealed class FieldFocusFeature : IFeature
    {
        private readonly IFieldSource _source;
        private readonly string _button;
        private readonly IFocusedField _focus;
        private readonly IPadInput _pad;
        private readonly IFocusHint _hint;

        public FieldFocusFeature(
            IFieldSource source,
            string button,
            IFocusedField focus,
            IPadInput pad,
            IFocusHint hint)
        {
            _source = source;
            _button = button;
            _focus = focus;
            _pad = pad;
            _hint = hint;
        }

        public void Enable()
        {
        }

        public void Update()
        {
            var fields = _source.Fields;
            if (fields.Count == 0)
            {
                return;
            }

            var target = Target(fields);
            var editing = _focus.Current != null;
            foreach (var field in fields)
            {
                var visible = field == target && !editing && field.interactable && _pad.IsActive;
                _hint.Show((RectTransform)field.transform, _button, visible);
            }

            var focusable = target.isActiveAndEnabled && target.interactable;
            if (!editing && focusable && _pad.Pressed(_button))
            {
                target.Select();
                target.ActivateInputField();
            }
        }

        public void Disable()
        {
        }

        private static GuiInputField Target(IReadOnlyList<GuiInputField> fields)
        {
            var events = EventSystem.current;
            var selected = events != null ? events.currentSelectedGameObject : null;
            foreach (var field in fields)
            {
                if (field.gameObject == selected)
                {
                    return field;
                }
            }

            return fields[0];
        }
    }
}
