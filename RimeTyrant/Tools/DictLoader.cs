namespace RimeTyrant.Tools
{
    internal static class DictLoader
    {
        public static bool AutoLoadDict(out string meassage)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android
                && AutoLoadDictAndroid(out meassage))
                return true;

            if (DeviceInfo.Platform == DevicePlatform.WinUI
                && AutoLoadDictWinUI(out meassage))
                return true;

            meassage = "程序上级目录和Rime默认目录中都找不到词库，或存在多个词库，请手动载入。";
            return false;
        }

        private static bool AutoLoadDictAndroid(out string message)
        {
            var userPath = @"storage/emulated/0/rime";
            if (Directory.Exists(userPath))
            {
                var dicts = FindYamls(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                {
                    message = "已自动载入Rime默认目录中的唯一词库。";
                    return true;
                }
            }

            message = string.Empty;
            return false;
        }

        private static bool AutoLoadDictWinUI(out string message)
        {
            //var parentPath = Directory.GetParent(Directory.GetCurrentDirectory());
            //if (parentPath != null && parentPath.Exists)
            //{
            //    var dicts = FindYamls(parentPath.FullName);
            //    if (dicts.Length == 1 && LoadDict(dicts[0]))
            //    {
            //        message = "已自动载入程序上级目录中的唯一词库。";
            //        return true;
            //    }
            //}

            var userPath = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime";
            if (Directory.Exists(userPath))
            {
                var dicts = FindYamls(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                {
                    message = "已自动载入Rime默认目录中的唯一词库。";
                    return true;
                }
            }

            message = string.Empty;
            return false;
        }

        private static string[] FindYamls(string directory)
            => Directory.GetFiles(directory, "*.yaml", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

        public static bool LoadDict(string path) => Try.Do(() => Dict.Load(path));
    }
}
