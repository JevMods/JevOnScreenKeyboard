using TMPro;
using UnityEngine;

namespace JevControllerKeyboard.Infrastructure
{
    public interface IGameAssets
    {
        Sprite Sprite(string name);

        TMP_FontAsset Font(string name);

        Material Material(string name);
    }
}
