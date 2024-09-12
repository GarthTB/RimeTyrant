using RimeTyrant.Codes;

namespace RimeTyrant.Tools
{
    internal class Encoder
    {
        private Code? _code;

        private string _name = string.Empty;

        private void Reset()
        {
            _code = null;
            _name = string.Empty;
        }

        public bool Ready(string name)
            => _name == name
               && _code is not null
               && _code.AllowAutoCode;

        /// <summary>
        /// 编码方案类的构造函数封装
        /// </summary>
        /// <returns>
        /// 所有有效的码长, 默认码长的索引
        /// </returns>
        public async Task<(int[], int)> SetCode(string codeName)
        {
            try
            {
                switch (codeName)
                {
                    case "键道6":
                        static Code Initialize(string filePath) => new JD(filePath);
                        _code = await FileLoader.LoadSingle("xkjd6.danzi.dict.yaml", codeName, Initialize);
                        _name = codeName;
                        return ([3, 4, 5, 6], 1);
                    default:
                        await Simp.Show("还未支持该编码方案哦");
                        return ([], -1);
                }
            }
            catch
            {
                Reset();
                return ([], -1);
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

        public bool IsValidWord(string? word)
            => _code is not null
               && _code.AllowAutoCode
               && Code.IsValidWord(word);

        public bool IsValidCode(string? code)
            => _code is not null
               && _code.AllowAutoCode
               && Code.IsValidCode(code);

        public bool IsValidPriority(string? priority)
            => _code is not null
               && _code.AllowAutoCode
               && Code.IsValidPriority(priority);

        /// <summary>
        /// 获取一个词组的所有全码
        /// </summary>
        public bool Encode(string word, out string[] fullCodes)
        {
            fullCodes = [];
            return _code is not null
                   && _code.AllowAutoCode
                   && _code.Encode(word, out fullCodes);
        }

        /// <summary>
        /// 用全码推导出指定长度的短码
        /// </summary>
        public bool CutCode(string fullCode, int length, out string shortCode)
        {
            shortCode = string.Empty;
            return _code is not null
                   && _code.AllowAutoCode
                   && _code.CutCode(fullCode, length, out shortCode);
        }

        /// <summary>
        /// 用全码推导出指定长度的短码
        /// </summary>
        public bool CutCodes(string[] fullCodes, int length, out string[] shortCodes)
        {
            shortCodes = [];
            return _code is not null
                   && _code.AllowAutoCode
                   && _code.CutCodes(fullCodes, length, out shortCodes);
        }
    }
}
