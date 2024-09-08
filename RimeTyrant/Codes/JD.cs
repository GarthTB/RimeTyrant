namespace RimeTyrant.Codes
{
    /// <summary>
    /// 星空键道6，2024年的规则
    /// </summary>
    internal class JD(string path) : Code(path)
    {
        public override bool IsValidWord(string? word)
            => !string.IsNullOrWhiteSpace(word);

        public override bool IsValidCode(string? code)
            => !string.IsNullOrWhiteSpace(code);

        protected override bool FindBlank(string[] codes, string prefix, out string code)
        {
            code = string.Empty;
            var _codes = codes.Where(s => s.StartsWith(prefix))
                              .Distinct()
                              .ToArray();
            if (_codes.Length != 1)
                return false;

            var result = _codes[0];
            for (int i = prefix.Length + 1; i < 6; i++)
                if (!Tools.Dict.HasCode(result[..i]))
                    code = result[..i];
            code = result;
            return true;
        }

        protected override bool Shorten(string fullCode, int length, out string shortCode)
        {
            if (length < 0 || length > fullCode.Length)
            {
                shortCode = fullCode;
                return false;
            }
            shortCode = fullCode[..length];
            return true;
        }

        protected override bool GetKeyChars(string originWord, out char[] keyChars)
        {
            var vc = originWord.Where(Contains).ToArray();
            keyChars = vc.Length > 4
                    ? [vc[0], vc[1], vc[2], vc[^1]]
                    : vc;
            return vc.Length > 1;
        }

        protected override bool GetKeyElements(char[] keyChars, out char[][][] keyElements)
        {
            keyElements = new char[keyChars.Length][][];
            for (int i = 0; i < keyChars.Length; i++)
                keyElements[i] = Dict.Where(e => e.Code.Length > 3 && e.Word == keyChars[i].ToString())
                                     .Select(e => e.Code[..3])
                                     .Distinct()
                                     .Select(x => x.ToArray())
                                     .ToArray();
            return keyElements.All(x => x.Length > 0);
        }

        protected override bool CodesOf(char[][][] keyElements, out string[] codes)
        {
            IEnumerable<char> Choose(int i, int j) => keyElements[i].Select(x => x[j]);

            string[] Compose(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l)
            {
                HashSet<string> result = [];
                foreach (var c1 in Choose(a, b))
                    foreach (var c2 in Choose(c, d))
                        foreach (var c3 in Choose(e, f))
                            foreach (var c4 in Choose(g, h))
                                foreach (var c5 in Choose(i, j))
                                    foreach (var c6 in Choose(k, l))
                                        _ = result.Add($"{c1}{c2}{c3}{c4}{c5}{c6}");
                return [.. result.Order()];
            }
            codes = keyElements.Length switch
            {
                //                 这两位代表第2码：
                //                 ┌─────从第1个字的所有码里
                //                 │  ┌──取每个码的第2个码元
                2 => Compose(0, 0, 0, 1, 1, 0, 1, 1, 0, 2, 1, 2),
                3 => Compose(0, 0, 1, 0, 2, 0, 0, 2, 1, 2, 2, 2),
                _ => Compose(0, 0, 1, 0, 2, 0, 3, 0, 0, 2, 1, 2),
            };
            return codes.All(code => code.Length == 6);
        }
    }
}
