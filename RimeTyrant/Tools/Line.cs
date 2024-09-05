﻿namespace RimeTyrant.Tools
{
    internal class Line
    {
        // 这里不能用字段，因为字段不能显示在ItemList里
        public string Word { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Priority { get; set; } = 0;

        public Line Clone() => new(Word, Code, Priority);

        public bool Equals(Line other)
            => Word == other.Word
               && Code == other.Code
               && Priority == other.Priority;

        public Line(string word, string code, int priority = 0)
        {
            Word = word;
            Code = code;
            Priority = priority;
        }

        public Line(string word, string code, string priority)
        {
            Word = word;
            Code = code;
            if (!int.TryParse(priority, out int p))
                throw new ArgumentException("遇到了无法识别的优先级！");
            Priority = p;
        }
    }
}
