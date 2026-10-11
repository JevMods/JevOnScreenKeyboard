using TMPro;
using UnityEngine;

namespace JevOnScreenKeyboard.Infrastructure
{
    public interface IGameAssets
    {
        Sprite Sprite(string name);

        TMP_FontAsset Font(string name);

        Material Material(string name);
    }
}
