namespace RimeTyrant.Tools
{
    /// <summary>
    /// Item类，用于存储一个词条（Entry这个名字被用了）
    /// </summary>
    internal class Item
    {
        // 字段不能显示在ItemList里，所以用属性
        public string Word { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        // 因为要用于写入时的排序，所以用int，不用string
        public int Priority { get; set; } = 0;

        public Item Clone() => new(Word, Code, Priority);

        public bool Equals(Item other)
            => Word == other.Word
               && Code == other.Code
               && Priority == other.Priority;

        public Item(string word, string code, int priority = 0)
        {
            Word = word;
            Code = code;
            Priority = priority;
        }

        public Item(string word, string code, string priority)
        {
            Word = word;
            Code = code;
            if (!int.TryParse(priority, out int p))
                throw new ArgumentException("遇到了无法识别的优先级！");
            Priority = p;
        }
    }
}
