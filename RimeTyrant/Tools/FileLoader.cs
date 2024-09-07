using RimeTyrant.Codes;

namespace RimeTyrant.Tools
{
    internal static class FileLoader
    {
        public delegate Code Initializer(string filePath);

        public static string PickYaml(string title)
        {
            try
            {
                var option = new PickOptions() { PickerTitle = title };

                var result = Task.Run(async () =>
                {
                    var file = await FilePicker.Default.PickAsync(option);
                    return file?.FullPath;
                });
                var path = result.Result;

                return path is not null && path.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase)
                    ? path
                    : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void AutoLoadDict()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android && AutoLoadDictAndroid())
                Simp.Show("已自动载入程序Rime默认目录中的词库");
            else if (DeviceInfo.Platform == DevicePlatform.WinUI && AutoLoadDictWinUI(out var path))
                Simp.Show($"已自动载入{path}中的词库");
            else Simp.Show("未能自动载入词库，请手动载入");
        }

        private static bool AutoLoadDictAndroid()
        {
            var userPath = @"storage/emulated/0/rime";
            if (Directory.Exists(userPath))
            {
                var dicts = AutoFindYamls(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                    return true;
            }
            return false;
        }

        private static bool AutoLoadDictWinUI(out string path)
        {
            //var parentPath = Directory.GetParent(Directory.GetCurrentDirectory());
            //if (parentPath != null && parentPath.Exists)
            //{
            //    var dicts = AutoFindYamls(parentPath.FullName);
            //    if (dicts.Length == 1 && LoadDict(dicts[0]))
            //    {
            //        path = "程序上级";
            //        return true;
            //    }
            //}

            var userPath = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime";
            if (Directory.Exists(userPath))
            {
                var dicts = AutoFindYamls(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                {
                    path = "Rime默认用户目录";
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }

        private static string[] AutoFindYamls(string directory)
            => Directory.GetFiles(directory, "*.yaml", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

        public static bool LoadDict(string path) => Simp.Try(() => Dict.Load(path));

        public static Code? LoadSingle(string fileName, string codeName, Initializer Initialize)
        {
            if (AutoLoadSamePath(fileName, out var filePath)
                && TryLoadCode(filePath, Initialize, out Code? code))
            {
                Simp.Show($"已自动载入词库同目录中的{codeName}单字库");
                return code;
            }
            // 不知道为什么，这行提示总是在手动选择之后才显示
            Simp.Show($"未能自动找到{codeName}单字库，请手动选择");
            return ManualLoadSingle(Initialize);
        }

        private static Code? ManualLoadSingle(Initializer Initialize)
        {
            var filePath = PickYaml("选择一个以dict.yaml结尾的单字文件");
            if (TryLoadCode(filePath, Initialize, out Code? code))
            {
                Simp.Show("成功载入指定的单字库");
                return code;
            }
            Simp.Show("载入指定的单字库失败，自动编码将不可用");
            return null;
        }

        /// <summary>
        /// 查找词库相同路径中的单字文件
        /// </summary>
        private static bool AutoLoadSamePath(string fileName, out string filePath)
        {
            filePath = string.Empty;
            var dir = Directory.GetParent(Dict.Path);
            if (!Dict.Loaded || !File.Exists(Dict.Path) || dir is null)
                return false;
            filePath = Path.Combine(dir.FullName, fileName);
            return File.Exists(filePath);
        }

        private static bool TryLoadCode(string filePath, Initializer Initialize, out Code? code)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();

                code = Initialize(filePath);
            }
            catch (Exception)
            {
                code = null;
            }
            return code?.AllowAutoCode ?? false;
        }
    }
}
