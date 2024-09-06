namespace RimeTyrant.Codes
{
    /// <summary>
    /// 键道6
    /// </summary>
    internal class JD : Code
    {
        public JD(string path) => Load(path);

        protected override bool FindBlank(char[][] codes, string prefix, out string code)
        {
            code = string.Empty;
            var _codes = codes.Select(x => new string(x))
                              .Where(s => s.StartsWith(prefix))
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
                                     .Select(e => e.Code.ToCharArray(0, 3))
                                     .Distinct()
                                     .ToArray();
            return keyElements.All(x => x.Length > 0);
        }

        protected override bool CodesOf(char[][][] keyElements, out char[][] codes)
        {
            var _codes = new List<char[]>();

            switch (keyElements.Length)
            {
                case 2:
                    foreach (var c1 in keyElements[0])
                        foreach (var c2 in keyElements[1])
                            _codes.Add([c1[0], c1[1], c2[0], c2[1], c1[2], c2[2]]);
                    break;

                case 3:
                    foreach (var c1 in keyElements[0])
                        foreach (var c2 in keyElements[1])
                            foreach (var c3 in keyElements[2])
                                _codes.Add([c1[0], c2[0], c3[0], c1[2], c2[2], c3[2]]);
                    break;

                default:
                    foreach (var c1 in keyElements[0])
                        foreach (var c2 in keyElements[1])
                            foreach (var c3 in keyElements[2])
                                foreach (var c4 in keyElements[3])
                                    _codes.Add([c1[0], c2[0], c3[0], c4[0], c1[2], c2[2]]);
                    break;
            }

            codes = _codes.Distinct().ToArray();
            return codes.Length > 0;
        }
    }
}
