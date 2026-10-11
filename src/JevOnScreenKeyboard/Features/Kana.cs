namespace JevOnScreenKeyboard.Features
{
    public static class Kana
    {
        public const char Dakuten = '゛';
        public const char Handakuten = '゜';
        public const char Small = '小';
        public const char LongVowel = 'ー';

        private const int KatakanaOffset = 0x60;
        private const char FirstHiragana = 'ぁ';
        private const char LastHiragana = 'ゖ';
        private const string Plain = "かきくけこさしすせそたちつてとはひふへほう";
        private const string Voiced = "がぎぐげござじずぜぞだぢづでどばびぶべぼゔ";
        private const string HalfBase = "はひふへほ";
        private const string HalfVoiced = "ばびぶべぼ";
        private const string HalfVoiceless = "ぱぴぷぺぽ";
        private const string Big = "あいうえおつやゆよわかけ";
        private const string Reduced = "ぁぃぅぇぉっゃゅょゎゕゖ";

        public static bool IsMark(char character)
        {
            return character == Dakuten || character == Handakuten || character == Small;
        }

        public static bool IsKana(char character)
        {
            return IsHiragana(character) || IsKatakana(character) || character == LongVowel;
        }

        public static string Katakana(string hiragana)
        {
            var result = hiragana.ToCharArray();
            for (var index = 0; index < result.Length; index++)
            {
                if (IsHiragana(result[index]))
                {
                    result[index] = (char)(result[index] + KatakanaOffset);
                }
            }

            return new string(result);
        }

        public static string Hiragana(string kana)
        {
            var result = kana.ToCharArray();
            for (var index = 0; index < result.Length; index++)
            {
                result[index] = Hiragana(result[index]);
            }

            return new string(result);
        }

        public static char Convert(char character, char mark)
        {
            var hiragana = Hiragana(character);
            var converted = ConvertHiragana(hiragana, mark);
            return hiragana == character ? converted : (char)(converted + KatakanaOffset);
        }

        private static bool IsHiragana(char character)
        {
            return character >= FirstHiragana && character <= LastHiragana;
        }

        private static bool IsKatakana(char character)
        {
            return character >= FirstHiragana + KatakanaOffset
                && character <= LastHiragana + KatakanaOffset;
        }

        private static char Hiragana(char character)
        {
            return IsKatakana(character) ? (char)(character - KatakanaOffset) : character;
        }

        private static char ConvertHiragana(char character, char mark)
        {
            switch (mark)
            {
                case Dakuten:
                    return Pair(character, Plain, Voiced)
                        ?? Map(character, HalfVoiceless, HalfVoiced)
                        ?? character;
                case Handakuten:
                    return Pair(character, HalfBase, HalfVoiceless)
                        ?? Map(character, HalfVoiced, HalfVoiceless)
                        ?? character;
                default:
                    return Pair(character, Big, Reduced) ?? character;
            }
        }

        private static char? Pair(char character, string first, string second)
        {
            return Map(character, first, second) ?? Map(character, second, first);
        }

        private static char? Map(char character, string from, string to)
        {
            var index = from.IndexOf(character);
            return index >= 0 ? to[index] : (char?)null;
        }
    }
}
