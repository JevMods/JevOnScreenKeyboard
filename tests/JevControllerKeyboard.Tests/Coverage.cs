using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JevControllerKeyboard.Features;
using JevControllerKeyboard.Infrastructure;

namespace JevControllerKeyboard.Tests
{
    public static class Coverage
    {
        private const string Initials = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        private const string Vowels = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        private const string Finals = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

        private static readonly Dictionary<char, string> Split = new Dictionary<char, string>
        {
            { 'ㅘ', "ㅗㅏ" }, { 'ㅙ', "ㅗㅐ" }, { 'ㅚ', "ㅗㅣ" }, { 'ㅝ', "ㅜㅓ" },
            { 'ㅞ', "ㅜㅔ" }, { 'ㅟ', "ㅜㅣ" }, { 'ㅢ', "ㅡㅣ" }, { 'ㄳ', "ㄱㅅ" },
            { 'ㄵ', "ㄴㅈ" }, { 'ㄶ', "ㄴㅎ" }, { 'ㄺ', "ㄹㄱ" }, { 'ㄻ', "ㄹㅁ" },
            { 'ㄼ', "ㄹㅂ" }, { 'ㄽ', "ㄹㅅ" }, { 'ㄾ', "ㄹㅌ" }, { 'ㄿ', "ㄹㅍ" },
            { 'ㅀ', "ㄹㅎ" }, { 'ㅄ', "ㅂㅅ" },
        };

        public static int Run(string directory)
        {
            var catalog = new LanguageLayouts(EmbeddedText.Read);
            var gaps = 0;
            foreach (var file in Directory.GetFiles(directory, "*.txt").OrderBy(f => f))
            {
                var language = Path.GetFileNameWithoutExtension(file);
                var layout = catalog.For(language);
                var reachable = Reachable(layout);
                var missing = new List<string>();
                var total = 0;
                foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
                {
                    var parts = line.Split('\t');
                    if (parts.Length != 2 || parts[0].Length != 1)
                    {
                        continue;
                    }

                    total++;
                    var character = parts[0][0];
                    if (!CanType(language, layout, reachable, character))
                    {
                        missing.Add(character + "x" + parts[1]);
                    }
                }

                if (missing.Count > 0)
                {
                    gaps += missing.Count;
                    Console.WriteLine(
                        "{0}: {1}/{2} untypeable: {3}",
                        language,
                        missing.Count,
                        total,
                        string.Join(" ", missing.Take(40)));
                }
            }

            Console.WriteLine("total untypeable distinct characters: " + gaps);
            return 0;
        }

        private static HashSet<char> Reachable(LanguageLayout language)
        {
            var result = new HashSet<char>();
            foreach (var native in new[] { language, LanguageLayouts.English })
            {
                var layout = new KeyLayout();
                layout.Reset(native);
                foreach (var row in layout.Rows)
                {
                    foreach (var key in row.Where(k => k.Action == KeyAction.Character))
                    {
                        result.Add(key.Character);
                        result.Add(key.ShiftCharacter);
                    }
                }
            }

            foreach (var key in result.ToList())
            {
                foreach (var variant in LetterVariants.Of(key))
                {
                    result.Add(variant);
                }
            }

            return result;
        }

        private static bool CanType(
            string language,
            LanguageLayout layout,
            HashSet<char> reachable,
            char character)
        {
            if (reachable.Contains(character))
            {
                return true;
            }

            if (character >= '가' && character <= '힣')
            {
                return TypeHangul(character);
            }

            if (language == "Japanese" && IsKana(character))
            {
                return TypeKana(reachable, character);
            }

            if (language == "Japanese" && character >= '一' && character <= '鿿')
            {
                return Kanji(layout, character);
            }

            if (language.StartsWith("Chinese") && character >= '一' && character <= '鿿')
            {
                return Pinyin(language, layout, character);
            }

            return false;
        }

        private static bool IsKana(char character)
        {
            return character >= 'ぁ' && character <= 'ヿ';
        }

        private static bool TypeKana(HashSet<char> reachable, char target)
        {
            foreach (var key in reachable.Where(IsKana))
            {
                foreach (var mark in new[] { '゛', '゜', '小' })
                {
                    var surface = new Surface();
                    var input = new KanaInput(() => new CandidateTable(string.Empty));
                    input.Type(key, surface);
                    input.Type(mark, surface);
                    if (surface.Text == target.ToString())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TypeHangul(char syllable)
        {
            var offset = syllable - '가';
            var final = offset % 28;
            var medial = offset / 28 % 21;
            var initial = offset / 28 / 21;
            var keys = new StringBuilder();
            keys.Append(Initials[initial]);
            keys.Append(Expand(Vowels[medial]));
            if (final > 0)
            {
                keys.Append(Expand(Finals[final]));
            }

            var surface = new Surface();
            var input = new HangulInput();
            foreach (var key in keys.ToString())
            {
                input.Type(key, surface);
            }

            return surface.Text == syllable.ToString();
        }

        private static string Expand(char jamo)
        {
            return Split.TryGetValue(jamo, out var parts) ? parts : jamo.ToString();
        }

        private static bool Kanji(LanguageLayout layout, char kanji)
        {
            if (!PinyinSets.TryGetValue("Japanese", out var set))
            {
                set = new HashSet<char>();
                foreach (var line in EmbeddedText.Read("KanjiReadings.txt").Split((char)10))
                {
                    var parts = line.Split(' ');
                    if (parts.Length == 2)
                    {
                        set.UnionWith(parts[1].Trim());
                    }
                }

                PinyinSets["Japanese"] = set;
            }

            return set.Contains(kanji);
        }

        private static bool Pinyin(string language, LanguageLayout layout, char hanzi)
        {
            if (!PinyinSets.TryGetValue(language, out var set))
            {
                set = new HashSet<char>();
                var resource = language == "Chinese"
                    ? "PinyinSimplified.txt"
                    : "PinyinTraditional.txt";
                foreach (var line in EmbeddedText.Read(resource).Split((char)10))
                {
                    var syllable = line.Split(' ')[0];
                    var input = layout.CreateInput();
                    var surface = new Surface();
                    foreach (var letter in syllable)
                    {
                        input.Type(letter, surface);
                    }

                    set.UnionWith(input.Candidates);
                }

                PinyinSets[language] = set;
            }

            return set.Contains(hanzi);
        }

        private static readonly Dictionary<string, HashSet<char>> PinyinSets =
            new Dictionary<string, HashSet<char>>();
    }
}
