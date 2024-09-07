﻿using RimeTyrant.Codes;
using static RimeTyrant.Tools.Encoder;

namespace RimeTyrant.Tools
{
    internal static class FileLoader
    {
        public static string PickYaml(string title)
        {
            try
            {
                var option = new PickOptions() { PickerTitle = title };
                var result = FilePicker.Default.PickAsync(option).Result?.FullPath;

                return result is not null && result.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase)
                    ? result
                    : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void AutoLoadDict(Page page)
            => _ = DeviceInfo.Platform == DevicePlatform.Android && AutoLoadDictAndroid()
                ? page.DisplayAlert("提示", "已自动载入程序Rime默认目录中的词库", "好的")
                : DeviceInfo.Platform == DevicePlatform.WinUI && AutoLoadDictWinUI(out var path)
                    ? page.DisplayAlert("提示", $"已自动载入{path}中的词库", "好的")
                    : page.DisplayAlert("提示", "未能自动载入词库", "好的");

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

        public static bool LoadDict(string path) => Try.Do(() => Dict.Load(path));

        public static bool LoadSingle(string fileName, string codeName, Initializer Initialize, Page page, out Code? code)
        {
            if (!AutoLoadSamePath(fileName, out var filePath)
                || !TryLoadCode(filePath, Initialize, out code))
            {
                _ = page.DisplayAlert("提示", $"未能自动找到{codeName}词库，请手动选择", "好的");
                filePath = PickYaml("选择一个以dict.yaml结尾的单字文件");
                return TryLoadCode(filePath, Initialize, out code);
            }
            else return true;
        }

        /// <summary>
        /// 查找词库相同路径中的单字文件
        /// </summary>
        private static bool AutoLoadSamePath(string fileName, out string filePath)
        {
            filePath = string.Empty;
            var dir = Directory.GetParent(Dict.Path);
            if (Dict.Loaded || !File.Exists(Dict.Path) || dir is null)
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
