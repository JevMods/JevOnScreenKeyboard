using System.Collections.Generic;

namespace JevOnScreenKeyboard.Features
{
    public sealed class HangulInput : InputMethod
    {
        private const string Initials = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        private const string Vowels = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        private const string Finals = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
        private const int VowelCount = 21;
        private const int FinalCount = 28;
        private const char SyllableBase = '가';

        private static readonly Dictionary<string, char> CompoundVowels =
            new Dictionary<string, char>
            {
                { "ㅗㅏ", 'ㅘ' },
                { "ㅗㅐ", 'ㅙ' },
                { "ㅗㅣ", 'ㅚ' },
                { "ㅜㅓ", 'ㅝ' },
                { "ㅜㅔ", 'ㅞ' },
                { "ㅜㅣ", 'ㅟ' },
                { "ㅡㅣ", 'ㅢ' },
            };

        private static readonly Dictionary<string, char> CompoundFinals =
            new Dictionary<string, char>
            {
                { "ㄱㅅ", 'ㄳ' },
                { "ㄴㅈ", 'ㄵ' },
                { "ㄴㅎ", 'ㄶ' },
                { "ㄹㄱ", 'ㄺ' },
                { "ㄹㅁ", 'ㄻ' },
                { "ㄹㅂ", 'ㄼ' },
                { "ㄹㅅ", 'ㄽ' },
                { "ㄹㅌ", 'ㄾ' },
                { "ㄹㅍ", 'ㄿ' },
                { "ㄹㅎ", 'ㅀ' },
                { "ㅂㅅ", 'ㅄ' },
            };

        private readonly List<char> _jamo = new List<char>();
        private readonly Composition _composition = new Composition();

        public static bool TryCompose(IReadOnlyList<char> jamo, out char result, out bool hasFinal)
        {
            result = '\0';
            hasFinal = false;
            if (jamo.Count == 0)
            {
                return false;
            }

            var index = 0;
            var initial = Initials.IndexOf(jamo[0]);
            if (initial >= 0)
            {
                index = 1;
            }
            else if (!IsVowel(jamo[0]))
            {
                return false;
            }

            var medial = -1;
            var final = 0;
            if (index < jamo.Count && IsVowel(jamo[index]))
            {
                medial = ReadVowel(jamo, ref index);
                if (initial >= 0 && index < jamo.Count && IsFinal(jamo[index]))
                {
                    final = ReadFinal(jamo, ref index);
                }
            }

            if (index != jamo.Count)
            {
                return false;
            }

            hasFinal = final > 0;
            result = initial >= 0 && medial >= 0
                ? (char)(SyllableBase + (initial * VowelCount + medial) * FinalCount + final)
                : initial >= 0 ? Initials[initial] : Vowels[medial];
            return true;
        }

        public override void Type(char character, ITextSurface text)
        {
            if (!IsJamo(character))
            {
                Cancel();
                text.Insert(character.ToString());
                return;
            }

            Synchronize(text);
            if (_jamo.Count > 0 && Extend(character, text))
            {
                return;
            }

            Begin(character, text);
        }

        public override bool Backspace(ITextSurface text)
        {
            Synchronize(text);
            if (_jamo.Count == 0)
            {
                return false;
            }

            _jamo.RemoveAt(_jamo.Count - 1);
            if (_jamo.Count == 0)
            {
                _composition.Show(text, string.Empty);
                return true;
            }

            TryCompose(_jamo, out var rest, out _);
            _composition.Show(text, rest.ToString());
            return true;
        }

        public override bool Space(ITextSurface text)
        {
            Cancel();
            return false;
        }

        public override void Choose(char candidate, ITextSurface text)
        {
            Cancel();
            base.Choose(candidate, text);
        }

        public override void Cancel()
        {
            _jamo.Clear();
            _composition.Cancel();
        }

        private static bool IsVowel(char character)
        {
            return Vowels.IndexOf(character) >= 0;
        }

        private static bool IsFinal(char character)
        {
            return Finals.IndexOf(character) > 0;
        }

        private static bool IsJamo(char character)
        {
            return Initials.IndexOf(character) >= 0 || IsVowel(character);
        }

        private static int ReadVowel(IReadOnlyList<char> jamo, ref int index)
        {
            var vowel = jamo[index++];
            if (index < jamo.Count
                && CompoundVowels.TryGetValue(vowel.ToString() + jamo[index], out var joined))
            {
                vowel = joined;
                index++;
            }

            return Vowels.IndexOf(vowel);
        }

        private static int ReadFinal(IReadOnlyList<char> jamo, ref int index)
        {
            var last = jamo[index++];
            if (index < jamo.Count
                && CompoundFinals.TryGetValue(last.ToString() + jamo[index], out var joined))
            {
                last = joined;
                index++;
            }

            return Finals.IndexOf(last);
        }

        private void Synchronize(ITextSurface text)
        {
            _composition.Synchronize(text);
            if (!_composition.Active)
            {
                _jamo.Clear();
            }
        }

        private bool Extend(char character, ITextSurface text)
        {
            _jamo.Add(character);
            if (TryCompose(_jamo, out var joined, out _))
            {
                if (!_composition.Show(text, joined.ToString()))
                {
                    _jamo.RemoveAt(_jamo.Count - 1);
                }

                return true;
            }

            _jamo.RemoveAt(_jamo.Count - 1);
            TryCompose(_jamo, out _, out var hasFinal);
            if (!hasFinal || !IsVowel(character))
            {
                return false;
            }

            MoveFinal(character, text);
            return true;
        }

        private void Begin(char character, ITextSurface text)
        {
            Cancel();
            if (_composition.Show(text, character.ToString()))
            {
                _jamo.Add(character);
            }
        }

        private void MoveFinal(char vowel, ITextSurface text)
        {
            var kept = new List<char>(_jamo);
            var moved = kept[kept.Count - 1];
            kept.RemoveAt(kept.Count - 1);
            var next = new List<char> { moved, vowel };
            TryCompose(kept, out var rest, out _);
            TryCompose(next, out var syllable, out _);
            if (_composition.Split(text, rest.ToString(), syllable.ToString()))
            {
                _jamo.Clear();
                _jamo.AddRange(next);
            }
        }
    }
}
