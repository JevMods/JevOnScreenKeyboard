using System;
using System.Collections.Generic;
using UnityEngine;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class PadInput : IPadInput
    {
        private const float RepeatDelay = 0.4f;
        private const float RepeatRate = 0.05f;

        private readonly Dictionary<string, float> _nextRepeat = new Dictionary<string, float>();
        private bool _reading;
        private bool _blocking;
        private int _releasedFrame = -1;

        internal static PadInput Instance { get; set; }

        public bool IsActive => ZInput.IsGamepadActive();

        public bool Blocking
        {
            get => _blocking;
            set
            {
                if (_blocking && !value)
                {
                    _releasedFrame = Time.frameCount;
                }

                _blocking = value;
            }
        }

        public bool Pressed(string button) => Read(() => ZInput.GetButtonDown(button));

        public bool Repeated(string button)
        {
            var now = Time.unscaledTime;
            if (Pressed(button))
            {
                _nextRepeat[button] = now + RepeatDelay;
                return true;
            }

            var held = Read(() => ZInput.GetButton(button));
            if (!held || !_nextRepeat.TryGetValue(button, out var next))
            {
                _nextRepeat.Remove(button);
                return false;
            }

            if (now < next)
            {
                return false;
            }

            _nextRepeat[button] = now + RepeatRate;
            return true;
        }

        internal bool Blocks(string button)
        {
            var blocking = _blocking || _releasedFrame == Time.frameCount;
            return blocking && !_reading && button.StartsWith(PadButton.Prefix);
        }

        private bool Read(Func<bool> read)
        {
            _reading = true;
            try
            {
                return read();
            }
            finally
            {
                _reading = false;
            }
        }
    }
}
