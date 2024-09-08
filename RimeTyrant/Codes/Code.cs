using RimeTyrant.Tools;
using System.Text;

namespace RimeTyrant.Codes
{
    /// <summary>
    /// 所有编码方案的基类，一共5个需要实现的方法
    /// </summary>
    internal abstract class Code
    {
        protected HashSet<Item> Dict { get; private set; } = [];

        public bool AllowAutoCode => Dict.Count > 0;

        protected Code(string path)
        {
            if (!File.Exists(path))
                throw new Exception("单字文件不存在！");
            using StreamReader sr = new(path, Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2)
                {
                    if (!Dict.Add(new Item(parts[0], parts[1])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]}");
                }
                else if (parts.Length == 3)
                {
                    if (!Dict.Add(new Item(parts[0], parts[1], parts[2])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]} {parts[2]}");
                }
            }
            if (Dict.Count == 0)
                throw new Exception("单字文件为空！");
        }

        #region 查字是否存在

        public bool Contains(char c) => Dict.Any(e => e.Word == c.ToString());

        public bool Contains(string c) => Dict.Any(e => e.Word == c);

        #endregion

        #region 词、码、优先级的有效性（暂且认为是通用的，要改就改此文件、派生类、Encoder）

        public static bool IsValidWord(string? word)
            => !string.IsNullOrWhiteSpace(word);

        public static bool IsValidCode(string? code)
            => !string.IsNullOrWhiteSpace(code);

        public static bool IsValidPriority(string? priority)
            => priority is not null
               && ((int.TryParse(priority, out int num) && num >= 0)
                   || priority.Length == 0);

        #endregion

        #region 加长编码

        public bool Lengthen(string word, string prefix, out string code)
        {
            code = string.Empty;
            return Encode(word, out string[] codes)
                   && FindBlank(codes, prefix, out code);
        }

        /// <summary>
        /// 加长编码到剩余的最短空位，用于截短功能
        /// </summary>
        protected abstract bool FindBlank(string[] codes, string prefix, out string code);

        #endregion

        #region 获取简码

        public bool Shorten(string[] fullCodes, int length, out string[] shortCodes)
        {
            shortCodes = new string[fullCodes.Length];
            for (int i = 0; i < fullCodes.Length; i++)
                if (!Shorten(fullCodes[i], length, out shortCodes[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// 用全码推导出指定长度的简码
        /// </summary>
        protected abstract bool Shorten(string fullCode, int length, out string shortCode);

        #endregion

        #region 自动编码

        public bool Encode(string word, out string[] codes)
        {
            codes = [];
            return GetKeyChars(word, out char[] keyChars)
                   && GetKeyElements(keyChars, out char[][][] keyElements)
                   && CodesOf(keyElements, out codes);
        }

        /// <summary>
        /// 提取一个词中所有参与编码的关键字符
        /// </summary>
        protected abstract bool GetKeyChars(string originWord, out char[] keyChars);

        /// <summary>
        /// 提取每个关键字符的编码中，参与编码的码元
        /// </summary>
        /// <param name="keyElements">
        /// 第一维是每个关键字，第二维是一个关键字的每个全码，第三维是一个全码中每个参与编码的码元
        /// </param>
        protected abstract bool GetKeyElements(char[] keyChars, out char[][][] keyElements);

        /// <summary>
        /// 根据提取的码元编码
        /// </summary>
        /// <param name="keyElements">
        /// 第一维是每个关键字，第二维是一个关键字的每个全码，第三维是一个全码中每个参与编码的码元
        /// </param>
        protected abstract bool CodesOf(char[][][] keyElements, out string[] codes);

        #endregion
    }
}
