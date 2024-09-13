using System.Text;

namespace RimeTyrant.Tools
{
    internal static class Logger
    {
        private static readonly List<string> _log = [];

        public static void Clear() => _log.Clear();

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

        public static async Task Save(Page page)
        {
            if (_log.Count == 0)
                throw new InvalidOperationException("没有记录到日志");

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
#if ANDROID
                await SaveAndroid(page);
#endif
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                await SaveWinUI(page);
            else throw new PlatformNotSupportedException("此平台暂未支持！");
        }

#if ANDROID
        private static async Task SaveAndroid(Page page)
        {
            var dirName = "Logs";
            var fileName = $"RimeTyrant_{DateTime.Now:yyyyMMdd}.log";
            var content = ReadAll();
            await page.DisplayAlert("提示",
                await AndroidFile.Write(dirName, fileName, content)
                ? "日志已保存成功" : "日志保存失败",
                "好的");
        }
#endif

        private static async Task SaveWinUI(Page page)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(dir))
                _ = Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, $"RimeTyrant_{DateTime.Now:yyyyMMdd}.log");
            if (File.Exists(path))
                await page.DisplayAlert("提示", "同天的日志文件已存在，将续写", "好的");

            using StreamWriter sw = new(path, true, Encoding.UTF8);
            sw.Write(ReadAll());
            await page.DisplayAlert("提示", $"日志已保存至：\n{path}", "好的");
        }
    }
}
