using HarmonyLib;
using JevOnScreenKeyboard.Infrastructure;

namespace JevOnScreenKeyboard.Patches
{
    [HarmonyPatch(typeof(ZInput), nameof(ZInput.TryGetButtonState))]
    internal static class ZInputTryGetButtonStatePatch
    {
        private static bool Prefix(string name, ref bool __result)
        {
            if (PadInput.Instance == null || !PadInput.Instance.Blocks(name))
            {
                return true;
            }

            __result = false;
            return false;
        }
    }
}
