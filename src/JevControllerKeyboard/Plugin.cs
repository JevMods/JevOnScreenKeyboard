using System;
using BepInEx;
using HarmonyLib;
using JevControllerKeyboard.Config;
using JevControllerKeyboard.Features;
using JevControllerKeyboard.Infrastructure;

namespace JevControllerKeyboard
{
    [BepInPlugin(PluginGuid, PluginName, BuildInfo.Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "JevMods.JevControllerKeyboard";
        public const string PluginName = "JevControllerKeyboard";

        private IFeature[] _features;
        private Harmony _harmony;

        private void Awake()
        {
            var gamepad = new PadInput();
            PadInput.Instance = gamepad;

            var assets = new GameAssets();
            var language = new GameLanguage();
            var focus = new FocusedField();
            var hint = new FocusHint(assets, language);
            var noHint = new NoFocusHint();
            _features = new IFeature[]
            {
                new OnscreenKeyboardFeature(
                    gamepad,
                    focus,
                    new KeyboardView(transform, assets, language),
                    new ModSettings(Config),
                    language,
                    new LanguageLayouts(EmbeddedText.Read),
                    new DialogPlacement(),
                    new GameUiScale()),
                new FieldFocusFeature(
                    new MainMenu(),
                    PadButton.RightStick,
                    focus,
                    gamepad,
                    hint),
                new FieldFocusFeature(
                    new ServerPassword(),
                    PadButton.RightStick,
                    focus,
                    gamepad,
                    noHint),
                new FieldFocusFeature(
                    new ServerFilter(),
                    PadButton.LeftStick,
                    focus,
                    gamepad,
                    noHint),
                new FieldFocusFeature(
                    new TextDialog(),
                    PadButton.LeftStick,
                    focus,
                    gamepad,
                    hint),
                new FieldFocusFeature(
                    new MapPin(),
                    PadButton.LeftStick,
                    focus,
                    gamepad,
                    hint),
                new FieldFocusFeature(
                    new PopupEntry(),
                    PadButton.LeftStick,
                    focus,
                    gamepad,
                    hint),
            };
            foreach (var feature in _features)
            {
                feature.Enable();
            }

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Logger.LogInfo("Loaded");
        }

        private void Update()
        {
            foreach (var feature in _features)
            {
                feature.Update();
            }
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            foreach (var feature in _features ?? Array.Empty<IFeature>())
            {
                feature.Disable();
            }

            PadInput.Instance = null;
        }
    }
}
