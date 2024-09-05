using System.Text;

namespace RimeTyrant.Tools
{
    internal static class JD
    {
        private static readonly HashSet<Line> _dict = [];

        public static void Load(string path)
        {
            _dict.Clear();
            using StreamReader sr = new(path, Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2)
                {
                    if (!_dict.Add(new Line(parts[0], parts[1])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]}");
                }
                else if (parts.Length == 3)
                {
                    if (!_dict.Add(new Line(parts[0], parts[1], parts[2])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]} {parts[2]}");
                }
            }
            if (_dict.Count == 0)
                throw new Exception("单字文件为空！");
        }

        public static bool Contains(char c)
            => _dict.Any(e => e.Word == c.ToString());

        public static bool Contains(string c)
            => _dict.Any(e => e.Word == c);

        /// <summary>
        /// 某个字的前3码，第一维是字，第二维是读音，第三维是前3码
        /// </summary>
        public static char[][][] GetKeyCodes(string word)
        {
            var result = new char[word.Length][][];
            for (int i = 0; i < word.Length; i++)
            {
                var c = word[i];
                result[i] = _dict.Where(e => e.Word == c.ToString() && e.Code.Length > 3)
                                 .Select(e => e.Code.ToCharArray(0, 3))
                                 .Distinct()
                                 .ToArray();
                if (result[i].Length == 0)
                    throw new Exception($"单字中找不到“{c}”字！");
            }
            return result;
        }

        public static string Lengthen(string word, string prefix)
        {
            if (!Encode(word, out char[][] codes))
                throw new ArgumentException("无法为短码的词自动编码！");

            var _codes = codes.Select(c => new string(c))
                              .Where(s => s.StartsWith(prefix))
                              .Distinct()
                              .ToArray();

            if (_codes.Length == 0)
                throw new Exception("短码和自动编码不匹配！");
            if (_codes.Length > 1)
                throw new Exception("短码加长方式不唯一！");

            var result = _codes[0];

            for (int i = prefix.Length + 1; i < 6; i++)
                if (!Dict.HasCode(result[..i]))
                    return result[..i];

            return result;
        }

        public static bool Encode(string word, out char[][] codes)
        {
            codes = GetKeyChars(word, out string keyChars)
                ? CodesOf(keyChars)
                : [];
            return codes.Length > 0;
        }

        private static bool GetKeyChars(string originWord, out string keyChars)
        {
            var vc = originWord.Where(Contains)
                               .ToArray();
            keyChars = vc.Length < 2
                ? string.Empty
                : vc.Length > 4
                    ? $"{new string(vc[..3])}{vc[^1]}"
                    : new string(vc);
            return keyChars.Length > 0;
        }

        private static char[][] CodesOf(string word)
        {
            var codesOfChar = GetKeyCodes(word);
            var codes = new List<char[]>();

            switch (word.Length)
            {
                case 2:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            codes.Add([c1[0], c1[1], c2[0], c2[1], c1[2], c2[2]]);
                    break;

                case 3:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            foreach (var c3 in codesOfChar[2])
                                codes.Add([c1[0], c2[0], c3[0], c1[2], c2[2], c3[2]]);
                    break;

                default:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            foreach (var c3 in codesOfChar[2])
                                foreach (var c4 in codesOfChar[3])
                                    codes.Add([c1[0], c2[0], c3[0], c4[0], c1[2], c2[2]]);
                    break;
            }

            return [.. codes];
        }
    }
}
