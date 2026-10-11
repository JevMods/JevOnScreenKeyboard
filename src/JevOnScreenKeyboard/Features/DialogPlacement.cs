using UnityEngine;

namespace JevOnScreenKeyboard.Features
{
    public sealed class DialogPlacement : IDialogPlacement
    {
        private const float Margin = 8f;
        private const float ScreenMargin = 16f;
        private const float LargeShare = 0.5f;
        private const float SidePanelHeightShare = 0.5f;
        private const float SidePanelCenterShare = 0.4f;

        private static readonly Slot[] DialogSlots =
        {
            Slot.BelowDialog,
            Slot.AboveDialog,
            Slot.BelowField,
            Slot.AboveField,
            Slot.RightOfDialog,
            Slot.LeftOfDialog,
        };

        private static readonly Slot[] SideSlots =
        {
            Slot.RightOfDialog,
            Slot.BelowDialog,
            Slot.AboveDialog,
            Slot.BelowField,
            Slot.AboveField,
        };

        private static readonly Slot[] FieldSlots =
        {
            Slot.BelowField,
            Slot.AboveField,
        };

        private static readonly Slot[] CornerSlots =
        {
            Slot.BelowFieldCorner,
            Slot.AboveFieldCorner,
        };

        private readonly DialogGeometry _geometry = new DialogGeometry();
        private RectTransform _dialog;
        private Vector3 _originalPosition;

        public void Place(RectTransform field, IKeyboardView keyboard)
        {
            Restore();
            keyboard.ResetPosition();
            var dialog = _geometry.FindDialog(field);
            var bounds = _geometry.VisualBounds(dialog);
            var fieldBounds = _geometry.ScreenBounds(field);
            var rect = keyboard.ScreenRect;
            var size = new Vector2(rect.width, Mathf.Max(rect.height, keyboard.MaxHeight));
            var usable = IsUsable(bounds);
            var shift = usable
                ? ShiftFor(bounds, fieldBounds, size)
                : ShiftForField(bounds, fieldBounds, size);
            if (shift != Vector2.zero)
            {
                Shift(dialog, shift);
                bounds = Translate(bounds, shift);
                fieldBounds = Translate(fieldBounds, shift);
            }

            var slot = Choose(SlotsFor(bounds, usable), bounds, fieldBounds, size);

            var target = PositionOf(slot, bounds, fieldBounds, rect.size);
            var top = Screen.height - ScreenMargin - rect.height;
            target.y = Mathf.Clamp(target.y, ScreenMargin, top);
            keyboard.MoveBy(target - rect.min);
        }

        public void Restore()
        {
            if (_dialog != null)
            {
                _dialog.position = _originalPosition;
            }

            _dialog = null;
        }

        private static Vector2 ShiftFor(Rect dialog, Rect field, Vector2 size)
        {
            if (IsSidePanel(dialog))
            {
                var overflow = dialog.xMax + Margin + size.x - (Screen.width - ScreenMargin);
                var room = dialog.xMin - ScreenMargin;
                var fits = overflow > 0f && room >= overflow;
                return fits ? new Vector2(-overflow, 0f) : Vector2.zero;
            }

            var fitsBelow = Fits(Slot.BelowDialog, dialog, field, size);
            if (fitsBelow || Fits(Slot.AboveDialog, dialog, field, size))
            {
                return Vector2.zero;
            }

            var needed = dialog.height + Margin + size.y + ScreenMargin * 2f;
            if (needed > Screen.height)
            {
                return Vector2.zero;
            }

            var up = ScreenMargin - (dialog.yMin - Margin - size.y);
            var down = dialog.yMax + Margin + size.y - (Screen.height - ScreenMargin);
            return up <= down ? new Vector2(0f, up) : new Vector2(0f, -down);
        }

        private static Vector2 ShiftForField(Rect dialog, Rect field, Vector2 size)
        {
            if (IsLarge(dialog) || Fits(Slot.BelowFieldCorner, dialog, field, size))
            {
                return Vector2.zero;
            }

            var up = ScreenMargin - (field.yMin - Margin - size.y);
            return dialog.yMax + up <= Screen.height ? new Vector2(0f, up) : Vector2.zero;
        }

        private static Slot[] SlotsFor(Rect dialog, bool usable)
        {
            if (!usable)
            {
                return IsLarge(dialog) ? FieldSlots : CornerSlots;
            }

            return IsSidePanel(dialog) ? SideSlots : DialogSlots;
        }

        private static Slot Choose(Slot[] slots, Rect dialog, Rect field, Vector2 size)
        {
            foreach (var slot in slots)
            {
                if (Fits(slot, dialog, field, size))
                {
                    return slot;
                }
            }

            return Slot.BelowField;
        }

        private static bool Fits(Slot slot, Rect dialog, Rect field, Vector2 size)
        {
            var position = PositionOf(slot, dialog, field, size);
            return position.x >= ScreenMargin
                && position.y >= ScreenMargin
                && position.x + size.x <= Screen.width - ScreenMargin
                && position.y + size.y <= Screen.height - ScreenMargin;
        }

        private static Vector2 PositionOf(Slot slot, Rect dialog, Rect field, Vector2 size)
        {
            switch (slot)
            {
                case Slot.BelowDialog:
                    return Below(dialog, size);
                case Slot.AboveDialog:
                    return Above(dialog, size);
                case Slot.BelowField:
                    return Below(field, size);
                case Slot.AboveField:
                    return Above(field, size);
                case Slot.BelowFieldCorner:
                    return new Vector2(CornerX(field, size), field.yMin - Margin - size.y);
                case Slot.AboveFieldCorner:
                    return new Vector2(CornerX(field, size), field.yMax + Margin);
                case Slot.RightOfDialog:
                    return new Vector2(dialog.xMax + Margin, dialog.yMin);
                default:
                    return new Vector2(dialog.xMin - Margin - size.x, dialog.yMin);
            }
        }

        private static Vector2 Below(Rect anchor, Vector2 size)
        {
            return new Vector2(CenterX(anchor, size), anchor.yMin - Margin - size.y);
        }

        private static Vector2 Above(Rect anchor, Vector2 size)
        {
            return new Vector2(CenterX(anchor, size), anchor.yMax + Margin);
        }

        private static float CornerX(Rect anchor, Vector2 size)
        {
            return Mathf.Clamp(anchor.xMin, ScreenMargin, Screen.width - ScreenMargin - size.x);
        }

        private static float CenterX(Rect anchor, Vector2 size)
        {
            return Mathf.Clamp(
                anchor.center.x - size.x / 2f,
                ScreenMargin,
                Screen.width - ScreenMargin - size.x);
        }

        private static Rect Translate(Rect rect, Vector2 offset)
        {
            return new Rect(rect.position + offset, rect.size);
        }

        private static bool IsUsable(Rect bounds)
        {
            var onScreen = bounds.xMin >= 0f
                && bounds.yMin >= 0f
                && bounds.xMax <= Screen.width
                && bounds.yMax <= Screen.height;
            return !IsLarge(bounds) && onScreen;
        }

        private static bool IsLarge(Rect bounds)
        {
            return bounds.width * bounds.height > Screen.width * Screen.height * LargeShare;
        }

        private static bool IsSidePanel(Rect bounds)
        {
            return bounds.height > Screen.height * SidePanelHeightShare
                && bounds.center.x < Screen.width * SidePanelCenterShare;
        }

        private void Shift(RectTransform dialog, Vector2 offset)
        {
            _dialog = dialog;
            _originalPosition = dialog.position;
            dialog.position += _geometry.WorldOffset(dialog, offset);
        }

        private enum Slot
        {
            BelowDialog,
            AboveDialog,
            BelowField,
            AboveField,
            BelowFieldCorner,
            AboveFieldCorner,
            RightOfDialog,
            LeftOfDialog,
        }
    }
}
