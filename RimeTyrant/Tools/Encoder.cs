using RimeTyrant.Codes;

namespace RimeTyrant.Tools
{
    internal class Encoder
    {
        private Code? _code;

        public bool Ready => _code is not null;

        public bool SetCode(string codeName, out string message)
        {
            switch (codeName)
            {
                case "键道6":
                    _code = new JD(codeName);
                    message = "键道6自动编码已就绪";
                    return Ready;
                default:
                    message = "敬请期待！";
                    return false;
            }
        }

        public bool Encode(string text, out char[][] codes)
        {
            codes = [];
            return _code is not null
                   && _code.Encode(text, out codes);
        }
    }
}
