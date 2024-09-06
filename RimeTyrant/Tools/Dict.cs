﻿using System.Text;

namespace RimeTyrant.Tools
{
    internal static class Dict
    {
        private static readonly HashSet<Item> _dict = [];
        private static readonly List<string> _shit = [];
        private static string _path = string.Empty;

        public static void Load(string path)
        {
            _shit.Clear();
            _dict.Clear();
            using StreamReader sr = new(path, Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2)
                {
                    if (!_dict.Add(new Item(parts[0], parts[1])))
                        throw new Exception($"无法读取词组文件中的：{parts[0]} {parts[1]}");
                }
                else if (parts.Length == 3)
                {
                    if (!_dict.Add(new Item(parts[0], parts[1], parts[2])))
                        throw new Exception($"无法读取词组文件中的：{parts[0]} {parts[1]} {parts[2]}");
                }
                else _shit.Add(line);
            }
            if (_dict.Count == 0)
                throw new Exception("词库文件为空！");
            _path = path;
        }

        public static void Save(string? path = null)
        {
            if (_path.Length == 0)
                return;
            path ??= _path;
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            if (_shit.Count > 0)
                foreach (var shit in _shit)
                    sw.WriteLine(shit);
            var sortedDict = _dict.OrderBy(e => e.Code)
                                  .ThenByDescending(e => e.Priority);
            foreach (var sd in sortedDict)
                sw.WriteLine(sd.Priority == 0
                    ? $"{sd.Word}\t{sd.Code}"
                    : $"{sd.Word}\t{sd.Code}\t{sd.Priority}");
        }

        public static void Add(Item entry)
        {
            if (HasEntry(entry))
                throw new Exception("词库中已存在该词条！");
            if (!_dict.Add(entry.Clone()))
                throw new Exception($"无法添加词条：{entry.Word} {entry.Code} {entry.Priority}");
        }

        public static void AddAll(Item[] entries)
        {
            foreach (var entry in entries)
                Add(entry);
        }

        public static void Remove(Item entry)
        {
            if (_dict.RemoveWhere(e => e.Equals(entry)) < 1)
                throw new Exception($"找不到词条：{entry.Word} {entry.Code} {entry.Priority}");
        }

        public static void RemoveAll(Item[] entries)
        {
            foreach (var entry in entries)
                Remove(entry);
        }

        public static bool HasWord(string word)
            => _dict.Any(e => e.Word == word);

        public static bool HasCode(string code)
            => _dict.Any(e => e.Code == code);

        public static bool HasEntry(Item entry)
            => _dict.Any(e => e.Equals(entry));

        public static HashSet<Item> CodeStartsWith(string prefix)
            => _dict.AsParallel()
                    .Where(e => e.Code.StartsWith(prefix))
                    .Select(e => e.Clone())
                    .ToHashSet();
    }
}
