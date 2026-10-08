using System;
using System.Collections.Generic;
using System.Linq;

namespace JevControllerKeyboard.Features
{
    public sealed class KeyChoices
    {
        public const int Slots = 10;
        public const char NextPageLabel = '»';

        private const int PagedSlots = Slots - 1;

        private IReadOnlyList<char> _items = Array.Empty<char>();
        private int _page;

        public bool Shown { get; private set; }

        public bool Focused { get; private set; }

        public int ReturnRow { get; private set; }

        public int ReturnIndex { get; private set; }

        public bool Show(IReadOnlyList<char> items)
        {
            if (Shown && _items.SequenceEqual(items))
            {
                return false;
            }

            _items = items;
            _page = 0;
            Shown = true;
            return true;
        }

        public void Clear()
        {
            _items = Array.Empty<char>();
            _page = 0;
            Shown = false;
        }

        public void NextPage()
        {
            var pages = (_items.Count + PagedSlots - 1) / PagedSlots;
            _page = (_page + 1) % Math.Max(pages, 1);
        }

        public void Focus(int row, int index)
        {
            ReturnRow = row;
            ReturnIndex = index;
            Focused = true;
        }

        public void Release()
        {
            Focused = false;
        }

        public List<Key> Row(float offset)
        {
            var paged = _items.Count > Slots;
            var size = paged ? PagedSlots : Slots;
            var keys = new List<Key>();
            for (var slot = 0; slot < Slots; slot++)
            {
                var x = offset + slot;
                if (paged && slot == PagedSlots)
                {
                    keys.Add(new Key(KeyAction.NextPage, NextPageLabel, x, 1f));
                    continue;
                }

                var number = _page * size + slot;
                var item = number < _items.Count ? _items[number] : '\0';
                keys.Add(new Key(KeyAction.Choice, item, x, 1f));
            }

            return keys;
        }
    }
}
