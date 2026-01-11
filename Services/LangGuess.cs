namespace EVETranslate.Services
{
    public static class LangGuess
    {
        public enum Lang { EnglishLike, RussianLike, ChineseLike, Unknown, Mixed }

        // Tune these
        private const int LatinWeight = 1;

        // "Huge amounts" as requested (pick 10, 25, 50, 100 depending on how aggressive you want)
        private const int CyrillicWeight = 25;
        private const int HanWeight = 50;

        // If script exists at all, add a one-time bonus to prevent "a few chars" being drowned out.
        // This makes "Hello 世界" strongly non-English even if only 2 Han chars exist.
        private const int CyrillicPresenceBonus = 100;
        private const int HanPresenceBonus = 200;

        // Mixed threshold applies to WEIGHTED totals (not raw letters)
        private const double MixedThreshold = 0.70;

        public static Lang GuessLangByScript(string s)
        {
            int latinRaw = 0, cyrRaw = 0, hanRaw = 0;

            foreach (var ch in s)
            {
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSymbol(ch) || char.IsDigit(ch))
                    continue;

                // Han (CJK Unified Ideographs + Extension A + Compatibility Ideographs)
                if ((ch >= '\u4E00' && ch <= '\u9FFF') ||
                    (ch >= '\u3400' && ch <= '\u4DBF') ||
                    (ch >= '\uF900' && ch <= '\uFAFF'))
                { hanRaw++; continue; }

                // Cyrillic
                if ((ch >= '\u0400' && ch <= '\u052F') ||
                    (ch >= '\u2DE0' && ch <= '\u2DFF') ||
                    (ch >= '\uA640' && ch <= '\uA69F'))
                { cyrRaw++; continue; }

                // Latin-ish
                if ((ch >= '\u0041' && ch <= '\u007A') ||
                    (ch >= '\u00C0' && ch <= '\u024F') ||
                    (ch >= '\u1E00' && ch <= '\u1EFF'))
                { latinRaw++; continue; }
            }

            int rawLetters = latinRaw + cyrRaw + hanRaw;
            if (rawLetters == 0) return Lang.Unknown;

            // Weighted totals
            int latin = latinRaw * LatinWeight;
            int cyrillic = cyrRaw * CyrillicWeight;
            int han = hanRaw * HanWeight;

            // One-time presence bonuses (prevents "a few chars" from being ignored)
            if (cyrRaw > 0) cyrillic += CyrillicPresenceBonus;
            if (hanRaw > 0) han += HanPresenceBonus;

            int weightedTotal = latin + cyrillic + han;
            if (weightedTotal == 0) return Lang.Unknown;

            int max = Math.Max(latin, Math.Max(cyrillic, han));
            double share = (double)max / weightedTotal;

            if (share < MixedThreshold) return Lang.Mixed;

            if (max == han) return Lang.ChineseLike;
            if (max == cyrillic) return Lang.RussianLike;
            if (max == latin) return Lang.EnglishLike;

            return Lang.Unknown;
        }
    }
}
