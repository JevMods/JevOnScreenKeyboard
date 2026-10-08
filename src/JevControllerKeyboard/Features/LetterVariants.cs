using System.Collections.Generic;

namespace JevControllerKeyboard.Features
{
    public static class LetterVariants
    {
        private static readonly Dictionary<char, string> Variants =
            new Dictionary<char, string>
            {
                { 'a', "áåäâàæãąāăǎª" },
                { 'c', "čçćċ" },
                { 'd', "ďðđ" },
                { 'e', "éêèęēëėěĕẽ" },
                { 'g', "ğģǧǥ" },
                { 'h', "ȟħ" },
                { 'i', "íîīìįïĭĩ" },
                { 'k', "ķǩ" },
                { 'l', "łľļĺ" },
                { 'n', "ňñńņŋ" },
                { 'o', "öóôøõòœőōŏº" },
                { 'r', "řŕŗ" },
                { 's', "šşßșśŝ" },
                { 't', "ťțþţŧ" },
                { 'u', "úüùūûųűůŭǔũ" },
                { 'y', "ýÿ" },
                { 'z', "žźżʒ" },
                { 'е', "ёѐ" },
                { 'и', "йѝ" },
                { 'і', "ї" },
            };

        public static string Of(char letter)
        {
            var lower = char.ToLowerInvariant(letter);
            if (!Variants.TryGetValue(lower, out var variants))
            {
                return string.Empty;
            }

            return lower == letter ? variants : variants.ToUpperInvariant();
        }
    }
}
