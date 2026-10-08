using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JevControllerKeyboard.Infrastructure
{
    public sealed class GameAssets : IGameAssets
    {
        private readonly Dictionary<string, Object> _cache = new Dictionary<string, Object>();

        public Sprite Sprite(string name)
        {
            return Find<Sprite>(name);
        }

        public TMP_FontAsset Font(string name)
        {
            return Find<TMP_FontAsset>(name);
        }

        public Material Material(string name)
        {
            return Find<Material>(name);
        }

        private T Find<T>(string name)
            where T : Object
        {
            if (_cache.TryGetValue(name, out var cached) && cached != null)
            {
                return (T)cached;
            }

            foreach (var asset in Resources.FindObjectsOfTypeAll<T>())
            {
                if (asset.name == name)
                {
                    _cache[name] = asset;
                    return asset;
                }
            }

            return null;
        }
    }
}
