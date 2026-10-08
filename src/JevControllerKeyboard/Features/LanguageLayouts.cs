using System;
using System.Collections.Generic;
using System.Globalization;

namespace JevControllerKeyboard.Features
{
    public sealed class LanguageLayouts : ILayoutCatalog
    {
        public static readonly LanguageLayout English =
            new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm");

        private static readonly string[] Hiragana =
        {
            "あかさたなはまやらわん",
            "いきしちにひみ りをー",
            "うくすつぬふむゆる々゛",
            "えけせてねへめ れ ゜",
            "おこそとのほもよろ 小",
        };

        private readonly Dictionary<string, LanguageLayout> _layouts;

        public LanguageLayouts(Func<string, string> readResource)
        {
            var simplified = Table(readResource, "PinyinSimplified.txt");
            var traditional = Table(readResource, "PinyinTraditional.txt");
            var kanji = Table(readResource, "KanjiReadings.txt");
            _layouts = new Dictionary<string, LanguageLayout>
            {
                { "English", English },
                { "Russian", new LanguageLayout("йцукенгшщзхъ", "фывапролджэ", "ячсмитьбюё") },
                { "Ukrainian", new LanguageLayout("йцукенгшщзхї", "фівапролджє", "ячсмитьбюґ’") },
                { "Bulgarian", new LanguageLayout("уеишщксдзц", "ьяаожгтнвмч", "юйъфхпрлбѝ") },
                {
                    "French",
                    new LanguageLayout("azertyuiop", "qsdfghjklm", "wxcvbn", "éèêàâçùûôîëïœ")
                },
                { "German", new LanguageLayout("qwertzuiopü", "asdfghjklöä", "yxcvbnmß") },
                {
                    "Spanish",
                    new LanguageLayout("qwertyuiop", "asdfghjklñ", "zxcvbnm", "áéíóúü¿¡")
                },
                {
                    "Portuguese_Brazilian",
                    new LanguageLayout("qwertyuiop", "asdfghjklç", "zxcvbnm", "áàâãéêíóôõú")
                },
                {
                    "Portuguese_European",
                    new LanguageLayout("qwertyuiop", "asdfghjklç", "zxcvbnm", "áàâãéêíóôõú")
                },
                { "Italian", new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "àèéìòù") },
                { "Dutch", new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "éëïöü") },
                { "Swedish", new LanguageLayout("qwertyuiopå", "asdfghjklöä", "zxcvbnmé") },
                { "Finnish", new LanguageLayout("qwertyuiopå", "asdfghjklöä", "zxcvbnm") },
                { "Norwegian", new LanguageLayout("qwertyuiopå", "asdfghjkløæ", "zxcvbnmé") },
                { "Danish", new LanguageLayout("qwertyuiopå", "asdfghjklæø", "zxcvbnmé") },
                {
                    "Icelandic",
                    new LanguageLayout("qwertyuiopð", "asdfghjklæ", "zxcvbnmþ", "áéíóúýö")
                },
                { "Polish", new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "ąćęłńóśźż") },
                {
                    "Czech",
                    new LanguageLayout("qwertzuiopú", "asdfghjklů", "yxcvbnm", "ěščřžýáíéťďň")
                },
                {
                    "Slovak",
                    new LanguageLayout("qwertzuiopú", "asdfghjklô", "yxcvbnmä", "ľščťžýáíéňóď")
                },
                { "Hungarian", new LanguageLayout("qwertzuiopőú", "asdfghjkléáű", "íyxcvbnmöüó") },
                { "Romanian", new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "ăâîșț") },
                { "Croatian", new LanguageLayout("qwertzuiopš", "asdfghjklčćž", "yxcvbnmđ") },
                {
                    "Turkish",
                    new LanguageLayout(
                        CultureInfo.GetCultureInfo("tr-TR"),
                        "qwertyuıopğü",
                        "asdfghjklşi",
                        "zxcvbnmöç")
                },
                {
                    "Latvian",
                    new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "āčēģīķļņšūž")
                },
                {
                    "Lithuanian",
                    new LanguageLayout("ąčęėįšųūž", "qwertyuiop", "asdfghjkl", "zxcvbnm")
                },
                { "Georgian", new LanguageLayout("ქწერტყუიოპ", "ასდფგჰჯკლ", "ზხცვბნმ", "ღთშჟძჩჭ") },
                { "Abenaki", new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm", "àãèôòñ") },
                {
                    "Thai",
                    new LanguageLayout(
                        "ๆไำพะัีรนยบลฃ",
                        "ฟหกดเ้่าสวง",
                        "ผปแอิืทมใฝ",
                        "ๅภถุึคตจขช")
                        .WithShift(
                            "๐\"ฎฑธํ๊ณฯญฐ,ฅ",
                            "ฤฆฏโฌ็๋ษศซ.",
                            "()ฉฮฺ์?ฒฬฦ",
                            "+๓๔ู฿๕๖๗๘๙")
                        .WithLabel("ไทย")
                },
                {
                    "Korean",
                    new LanguageLayout("ㅂㅈㄷㄱㅅㅛㅕㅑㅐㅔ", "ㅁㄴㅇㄹㅎㅗㅓㅏㅣ", "ㅋㅌㅊㅍㅠㅜㅡ")
                        .WithShift("ㅃㅉㄸㄲㅆㅛㅕㅑㅒㅖ", "ㅁㄴㅇㄹㅎㅗㅓㅏㅣ", "ㅋㅌㅊㅍㅠㅜㅡ")
                        .WithLabel("한")
                        .WithInput(() => new HangulInput())
                },
                {
                    "Japanese",
                    new LanguageLayout(Hiragana)
                        .WithShift(Array.ConvertAll(Hiragana, Kana.Katakana))
                        .WithLabel("あ")
                        .WithPunctuation('、', '。')
                        .WithInput(() => new KanaInput(() => kanji.Value))
                },
                {
                    "Chinese",
                    new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm")
                        .WithLabel("拼音")
                        .WithPunctuation('，', '。')
                        .WithInput(() => new PinyinInput(() => simplified.Value))
                },
                {
                    "Chinese_Trad",
                    new LanguageLayout("qwertyuiop", "asdfghjkl", "zxcvbnm")
                        .WithLabel("拼音")
                        .WithPunctuation('，', '。')
                        .WithInput(() => new PinyinInput(() => traditional.Value))
                },
            };
        }

        public LanguageLayout For(string language)
        {
            return language != null && _layouts.TryGetValue(language, out var layout)
                ? layout
                : English;
        }

        private static Lazy<CandidateTable> Table(Func<string, string> readResource, string name)
        {
            return new Lazy<CandidateTable>(() => new CandidateTable(readResource(name)));
        }
    }
}
