using BepInEx.Configuration;
using UnityEngine;

namespace JevControllerKeyboard.Config
{
    public sealed class ModSettings : IModSettings
    {
        private readonly ConfigEntry<float> _scale;

        public ModSettings(ConfigFile file)
        {
            _scale = file.Bind(
                "Keyboard",
                "Scale",
                1f,
                "Size multiplier for the on-screen keyboard, from 0.5 to 2. "
                + "It already scales with the screen.");
        }

        public float Scale => Mathf.Clamp(_scale.Value, 0.5f, 2f);
    }
}
