using System;
using System.Collections.Generic;
using System.Linq;
using JevOnScreenKeyboard.Features;
using JevOnScreenKeyboard.Infrastructure;
using static JevOnScreenKeyboard.Tests.Check;

namespace JevOnScreenKeyboard.Tests
{
    public static class LayoutTests
    {
        public static void Run()
        {
            Layouts();
        }

        private static void Layouts()
        {
            var catalog = new LanguageLayouts(EmbeddedText.Read);
            var cases = new Dictionary<string, string>
            {
                { "Bulgarian", "абвгдежзийклмнопрстуфхцчшщъьюя" },
                { "Russian", "абвгдеёжзийклмнопрстуфхцчшщъыьэюя" },
                { "Ukrainian", "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя’" },
                { "Georgian", "აბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰ" },
                { "Korean", "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎㅏㅐㅑㅒㅓㅔㅕㅖㅗㅛㅜㅠㅡㅣ" },
                { "Japanese", "あいうえおかきくけこさしすせそたちつてとなにぬねの" },
                { "Thai", "กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรฤลฦวศษสหฬอฮ" },
                { "Swedish", "åäöé" },
                { "Danish", "æøåé" },
                { "Norwegian", "æøåé" },
                { "German", "äöüß" },
                { "Polish", "ąćęłńóśźż" },
                { "Czech", "ěščřžýáíéúůťďň" },
                { "Slovak", "äľščťžýáíéúôňď" },
                { "Hungarian", "áéíóöőúüű" },
                { "Latvian", "āčēģīķļņšūž" },
                { "Lithuanian", "ąčęėįšųūž" },
                { "Turkish", "çğıöşü" },
                { "Icelandic", "áðéíóúýþæö" },
                { "Romanian", "ăâîșț" },
                { "Croatian", "čćđšž" },
                { "French", "éèêàâçùûôîëïœ" },
                { "Spanish", "ñáéíóúü" },
                { "Portuguese_Brazilian", "çáàâãéêíóôõú" },
            };
            foreach (var pair in cases)
            {
                var layout = new KeyLayout();
                layout.Reset(catalog.For(pair.Key));
                var reachable = Reachable(layout);
                foreach (var letter in pair.Value)
                {
                    That(
                        pair.Key + " reaches " + letter,
                        true,
                        reachable.Contains(letter) || Typeable(reachable, letter));
                }
            }

            var japanese = new KeyLayout();
            japanese.Reset(catalog.For("Japanese"));
            That("japanese rows", 7, japanese.Rows.Count);
            That("japanese max rows", 7, japanese.MaxRows);
            japanese.Shifted = true;
            var hira = japanese.Rows[1][0];
            That("katakana shift", 'ア', japanese.CharacterOf(hira));

            var thai = new KeyLayout();
            thai.Reset(catalog.For("Thai"));
            thai.Shifted = true;
            That("thai shift layer", true, Reachable(thai).Contains('ณ'));

            var english = new KeyLayout();
            That("english rows", 5, english.Rows.Count);
            That("english max rows", 5, english.MaxRows);

            var chinese = new KeyLayout();
            chinese.Reset(catalog.For("Chinese"));
            That("chinese can switch", true, chinese.CanSwitchLanguage);
            That("chinese max rows", 5, chinese.MaxRows);

            ChoiceRow(catalog);
            CandidateTables(catalog);
        }

        private static bool Typeable(HashSet<char> reachable, char letter)
        {
            var lower = char.ToLowerInvariant(letter);
            foreach (var key in reachable)
            {
                if (LetterVariants.Of(key).IndexOf(lower) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static HashSet<char> Reachable(KeyLayout layout)
        {
            var result = new HashSet<char>();
            foreach (var row in layout.Rows)
            {
                foreach (var key in row)
                {
                    if (key.Action == KeyAction.Character)
                    {
                        result.Add(key.Character);
                        result.Add(key.ShiftCharacter);
                    }
                }
            }

            return result;
        }

        private static void ChoiceRow(LanguageLayouts catalog)
        {
            var layout = new KeyLayout();
            layout.Reset(catalog.For("Chinese"));
            var digits = new string(layout.Rows[0].Select(key => key.Character).ToArray());
            layout.ShowChoices("你尼泥".ToCharArray());
            That("choices shown", true, layout.ChoicesShown);
            That("choices keep slot count", digits.Length, layout.Rows[0].Count);
            That("first choice", '你', layout.Rows[0][0].Character);
            That("blank choice", '\0', layout.Rows[0][5].Character);
            layout.FocusChoices();
            That("focus row", 0, layout.Row);
            layout.LeaveChoices(true);
            layout.ClearChoices();
            var restored = new string(layout.Rows[0].Select(k => k.Character).ToArray());
            That("digits back", digits, restored);

            var many = new string('一', 25).ToCharArray();
            layout.ShowChoices(many);
            That("paged key", KeyAction.NextPage, layout.Rows[0][9].Action);
            That("page one count", 9, layout.Rows[0].Count(key => key.Character == '一'));
            layout.NextChoicePage();
            layout.NextChoicePage();
            That("last page count", 7, layout.Rows[0].Count(key => key.Character == '一'));
            layout.NextChoicePage();
            That("wraps to first", 9, layout.Rows[0].Count(key => key.Character == '一'));
        }

        private static void CandidateTables(LanguageLayouts catalog)
        {
            var simplified = catalog.For("Chinese").CreateInput();
            var traditional = catalog.For("Chinese_Trad").CreateInput();
            var surface = new Surface();
            simplified.Type('g', surface);
            simplified.Type('u', surface);
            simplified.Type('o', surface);
            That("simplified guo", true, simplified.Candidates.Contains('国'));
            That("simplified has no traditional", false, simplified.Candidates.Contains('國'));
            surface = new Surface();
            traditional.Type('g', surface);
            traditional.Type('u', surface);
            traditional.Type('o', surface);
            That("traditional guo", true, traditional.Candidates.Contains('國'));
            That("traditional has no simplified", false, traditional.Candidates.Contains('国'));
            simplified = catalog.For("Chinese").CreateInput();
            surface = new Surface();
            simplified.Type('n', surface);
            simplified.Type('i', surface);
            That("ni first", '你', simplified.Candidates[0]);
            simplified = catalog.For("Chinese").CreateInput();
            surface = new Surface();
            simplified.Type('l', surface);
            simplified.Type('v', surface);
            That("lv has lü", true, simplified.Candidates.Contains('绿'));
        }
    }
}
