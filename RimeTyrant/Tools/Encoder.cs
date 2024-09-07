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

        public bool SetCode(string codeName, Page page, out int[] validCodeLengthArray)
        {
            switch (codeName)
            {
                case "键道6":
                    validCodeLengthArray = [3, 4, 5, 6];
                    Initializer Initialize = (filePath) => new JD(filePath);
                    return FileLoader.LoadSingle("xkjd6.danzi.dict.yaml", codeName, Initialize, page, out _code);
                default:
                    _code = null;
                    _name = string.Empty;
                    validCodeLengthArray = [];
                    return false;
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
