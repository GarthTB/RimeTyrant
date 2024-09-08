using System.Text;

namespace RimeTyrant.Tools
{
    internal static class Logger
    {
        private static readonly List<string> _log = [];

        public static string ReadAll()
        {
            var sb = new StringBuilder();
            foreach (var log in _log)
                _ = sb.AppendLine(log);
            return sb.ToString();
        }

        public static void Add(string message) => _log.Add(message);

        public static void Add(string message, Item item)
        {
            var timestamp = $"{DateTime.Now:HH:mm:ss}";
            var log = item.Priority == 0
                ? $"{message}\t{item.Word}\t{item.Code}"
                : $"{message}\t{item.Word}\t{item.Code}\t{item.Priority}";

            _log.Add($"{timestamp}\t{log}");
        }

        public static void Save()
        {
            if (_log.Count == 0)
                throw new InvalidOperationException("没有记录到日志");

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, $"RimeTyrant_{DateTime.Now:yyyyMMdd}.log");
            if (File.Exists(path))
                Simp.Show("同天的日志文件已存在，将续写");

            using StreamWriter sw = new(path, true, Encoding.UTF8);
            sw.Write(ReadAll());
            Simp.Show($"日志已保存至：\n{path}");
        }
    }
}
