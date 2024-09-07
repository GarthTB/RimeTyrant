using RimeTyrant.Codes;

namespace RimeTyrant.Tools
{
    internal class Encoder
    {
        private Code? _code;
        private string _name = string.Empty;

        public bool Ready(string name)
            => _name == name
               && _code is not null
               && _code.AllowAutoCode;

        public delegate Code Initializer(string filePath);

        public int[] SetCode(string codeName, Page page)
        {
            switch (codeName)
            {
                case "键道6":
                    Initializer Initialize = (filePath) => new JD(filePath);
                    _code = FileLoader.LoadSingle("xkjd6.danzi.dict.yaml", codeName, Initialize, page);
                    _name = codeName;
                    return [3, 4, 5, 6];
                default:
                    _code = null;
                    _name = string.Empty;
                    return [];
            }
        }

        public bool Contains(char c)
            => _code is not null
               && _code.AllowAutoCode
               && _code.Contains(c);

        public bool Contains(string s)
            => _code is not null
               && _code.AllowAutoCode
               && _code.Contains(s);

        public bool Lengthen(string word, string prefix, out string code)
        {
            code = string.Empty;
            return _code is not null
               && _code.AllowAutoCode
               && _code.Lengthen(word, prefix, out code);
        }

        public bool Encode(string text, out char[][] codes)
        {
            codes = [];
            return _code is not null
                   && _code.AllowAutoCode
                   && _code.Encode(text, out codes);
        }
    }
}
