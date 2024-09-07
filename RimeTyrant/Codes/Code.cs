﻿using RimeTyrant.Tools;
using System.Text;

namespace RimeTyrant.Codes
{
    /// <summary>
    /// 所有编码的基类，一共4个需要实现的方法
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

        #region 加长编码

        public bool Lengthen(string word, string prefix, out string code)
        {
            code = string.Empty;
            return Encode(word, out char[][] codes)
                   && FindBlank(codes, prefix, out code);
        }

        /// <summary>
        /// 加长编码到剩余的最短空位，用于截短功能
        /// </summary>
        protected abstract bool FindBlank(char[][] codes, string prefix, out string code);

        #endregion

        #region 自动编码

        public bool Encode(string word, out char[][] codes)
        {
            codes = [];
            return GetKeyChars(word, out char[] keyChars)
                   && GetKeyElements(keyChars, out char[][][] keyElements)
                   && CodesOf(keyElements, out codes);
        }

        /// <summary>
        /// 提取一个词中所有用于编码的关键字符
        /// </summary>
        protected abstract bool GetKeyChars(string originWord, out char[] keyChars);

        /// <summary>
        /// 提取关键字符对应的编码中所有参与编码的码元
        /// </summary>
        /// <param name="keyElements">
        /// 第一维是每个关键字，第二维是一个关键字的每个全码，第三维是一个全码中每个参与编码的码元
        /// </param>
        protected abstract bool GetKeyElements(char[] keyChars, out char[][][] keyElements);

        /// <summary>
        /// 根据提取的码元编码
        /// </summary>
        protected abstract bool CodesOf(char[][][] keyElements, out char[][] codes);

        #endregion
    }
}
