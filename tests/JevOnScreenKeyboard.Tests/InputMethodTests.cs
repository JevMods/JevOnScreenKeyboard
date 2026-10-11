using System;
using System.Collections.Generic;
using System.Linq;
using JevOnScreenKeyboard.Features;
using JevOnScreenKeyboard.Infrastructure;
using static JevOnScreenKeyboard.Tests.Check;

namespace JevOnScreenKeyboard.Tests
{
    public static class InputMethodTests
    {
        public static void Run()
        {
            HangulSyllables();
            HangulEditing();
            KanaMarks();
            KanjiCandidates();
            PinyinCandidates();
            Variants();
        }

        private static string Type(IInputMethod method, string keys)
        {
            var surface = new Surface();
            foreach (var key in keys)
            {
                method.Type(key, surface);
            }

            return surface.Text;
        }

        private static void HangulSyllables()
        {
            var cases = new Dictionary<string, string>
            {
                { "ㅎㅏㄴㄱㅡㄹ", "한글" },
                { "ㄷㅏㄹㄱ", "닭" },
                { "ㄷㅏㄹㄱㅣ", "달기" },
                { "ㄷㅏㄹㄱㅇㅣ", "닭이" },
                { "ㄱㅏㅂㅅ", "값" },
                { "ㄱㅏㅂㅅㅣ", "갑시" },
                { "ㅇㅗㅏ", "와" },
                { "ㅇㅜㅓㄴ", "원" },
                { "ㄲㅏ", "까" },
                { "ㅅㅏㄹㅏㅁ", "사람" },
                { "ㅎㅏㄴㅏ", "하나" },
                { "ㅇㅏㄴㅈㅏ", "안자" },
                { "ㅇㅏㄴㅈㅇㅏ", "앉아" },
                { "ㅇㅏㅣ", "아ㅣ" },
                { "ㅏ", "ㅏ" },
                { "ㄱㄱ", "ㄱㄱ" },
                { "ㅗㅏ", "ㅘ" },
                { "ㄱㅏ1", "가1" },
            };
            foreach (var pair in cases)
            {
                That("hangul " + pair.Key, pair.Value, Type(new HangulInput(), pair.Key));
            }
        }

        private static void HangulEditing()
        {
            var method = new HangulInput();
            var surface = new Surface();
            foreach (var key in "ㅎㅏㄴ")
            {
                method.Type(key, surface);
            }

            That("hangul backspace 1", true, method.Backspace(surface));
            That("hangul after final", "하", surface.Text);
            method.Backspace(surface);
            That("hangul after vowel", "ㅎ", surface.Text);
            method.Backspace(surface);
            That("hangul empty", string.Empty, surface.Text);
            That("hangul no composition left", false, method.Backspace(surface));

            surface = new Surface();
            method = new HangulInput();
            method.Type('ㄱ', surface);
            surface.Caret = 0;
            method.Type('ㅏ', surface);
            That("hangul caret move cancels", "ㅏㄱ", surface.Text);
        }

        private static void KanaMarks()
        {
            var cases = new Dictionary<string, string>
            {
                { "か゛", "が" },
                { "が゛", "か" },
                { "は゜", "ぱ" },
                { "ぱ゛", "ば" },
                { "ば゜", "ぱ" },
                { "ぱ゜", "は" },
                { "つ小", "っ" },
                { "っ小", "つ" },
                { "ヤ小", "ャ" },
                { "ウ゛", "ヴ" },
                { "ホ゜", "ポ" },
                { "あ゛", "あ" },
                { "゛", string.Empty },
            };
            foreach (var pair in cases)
            {
                var method = new KanaInput(() => new CandidateTable(string.Empty));
                That("kana " + pair.Key, pair.Value, Type(method, pair.Key));
            }

            That("katakana map", "アイウエオ", Kana.Katakana("あいうえお"));
            That("katakana keeps marks", "ー゛゜小", Kana.Katakana("ー゛゜小"));
        }

        private static void KanjiCandidates()
        {
            var table = new CandidateTable("やま 山病" + (char)10 + "だ 田打" + (char)10 + "た 田太");
            var method = new KanaInput(() => table);
            var surface = new Surface();
            method.Type('や', surface);
            method.Type('ま', surface);
            That("kanji candidates", "山病", new string(method.Candidates.ToArray()));
            That("kanji reading shown", "やま", surface.Text);
            That("kanji space commits", true, method.Space(surface));
            That("kanji committed", "山", surface.Text);

            method.Type('た', surface);
            method.Type('゛', surface);
            That("kanji voiced reading", "山だ", surface.Text);
            That("kanji voiced candidates", "田打", new string(method.Candidates.ToArray()));
            method.Choose('田', surface);
            That("kanji choose", "山田", surface.Text);

            method.Type('ヤ', surface);
            method.Type('マ', surface);
            That("kanji from katakana", "山病", new string(method.Candidates.ToArray()));
            That("kanji backspace", true, method.Backspace(surface));
            That("kanji backspace text", "山田ヤ", surface.Text);
            method.Cancel();
            That("kanji space without composing", false, method.Space(surface));
            method.Type('あ', surface);
            That("kanji no candidates space consumed", true, method.Space(surface));
            That("kanji raw kept", "山田ヤあ", surface.Text);
        }

        private static void PinyinCandidates()
        {
            var table = new CandidateTable(
                "ni 你尼泥\nhao 好号\nzhong 中种重");
            var method = new PinyinInput(() => table);
            var surface = new Surface();
            method.Type('n', surface);
            That("pinyin composing", true, method.Choosing);
            That("pinyin none yet", 0, method.Candidates.Count);
            method.Type('i', surface);
            That("pinyin candidates", "你尼泥", new string(method.Candidates.ToArray()));
            That("pinyin letters shown", "ni", surface.Text);
            That("pinyin space commits", true, method.Space(surface));
            That("pinyin committed", "你", surface.Text);
            That("pinyin idle", false, method.Choosing);

            method.Type('h', surface);
            method.Type('a', surface);
            method.Type('o', surface);
            method.Choose('号', surface);
            That("pinyin choose", "你号", surface.Text);

            method.Type('x', surface);
            That("pinyin backspace consumed", true, method.Backspace(surface));
            That("pinyin backspace result", "你号", surface.Text);
            That("pinyin idle backspace", false, method.Backspace(surface));

            method.Type('n', surface);
            surface.Caret = 0;
            method.Type('i', surface);
            That("pinyin caret move cancels", "i你号n", surface.Text);

            That("pinyin uppercase bypass", "A", Type(new PinyinInput(() => table), "A"));
        }

        private static void Variants()
        {
            That("variants a", true, LetterVariants.Of('a').Contains('á'));
            That("variants A", true, LetterVariants.Of('A').Contains('Á'));
            That("variants e has é", true, LetterVariants.Of('e').Contains('é'));
            That("variants Turkish â", true, LetterVariants.Of('a').Contains('â'));
            That("variants Czech ó", true, LetterVariants.Of('o').Contains('ó'));
            That("variants Slovak ŕ", true, LetterVariants.Of('r').Contains('ŕ'));
            That("variants Bulgarian ѝ", true, LetterVariants.Of('и').Contains('ѝ'));
            That("variants digit", 0, LetterVariants.Of('1').Length);
            That("variants hangul", 0, LetterVariants.Of('ㄱ').Length);
        }
    }
}
