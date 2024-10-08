﻿using RimeTyrant.Codes;

namespace RimeTyrant.Tools
{
    internal static class FileLoader
    {
        public delegate Code Initializer(string filePath);

        public static async Task<string> PickYaml(string title)
        {
            try
            {
                var option = new PickOptions() { PickerTitle = title };
                var result = await FilePicker.PickAsync(option);
                return result?.FullPath ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #region 加载词库

        public static string AutoLoadDict()
        {
#if ANDROID
            if (AutoLoadDictAndroid())
                return "已自动载入程序Rime默认目录中的词库";
#elif WINDOWS
            if (AutoLoadDictWinUI(out var path))
                return $"已自动载入{path}中的词库";
#endif
            return "未能自动载入词库，请手动载入";
        }

#if ANDROID
        private static bool AutoLoadDictAndroid()
        {
            var userPath = @"storage/emulated/0/rime";
            if (Directory.Exists(userPath))
            {
                var dicts = AutoFindDicts(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                    return true;
            }
            return false;
        }
#endif

#if WINDOWS
        private static bool AutoLoadDictWinUI(out string path)
        {
            path = "程序上级";
            var parentPath = Directory.GetParent(Directory.GetCurrentDirectory());
            if (parentPath is not null && parentPath.Exists)
            {
                var dicts = AutoFindDicts(parentPath.FullName);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                    return true;
            }

            path = "Rime默认用户目录";
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userPath = Path.Combine(appData, "Rime");
            if (Directory.Exists(userPath))
            {
                var dicts = AutoFindDicts(userPath);
                if (dicts.Length == 1 && LoadDict(dicts[0]))
                    return true;
            }

            path = string.Empty;
            return false;
        }
#endif

        private static string[] AutoFindDicts(string directory)
        {
            string[] dicts = [];
            var success = Simp.Try("自动寻找dict.yaml文件", ()
                => dicts = Directory.GetFiles(directory, "*.yaml", SearchOption.AllDirectories)
                                    .Where(f => f.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase))
                                    .ToArray(), false);
            return success ? dicts : [];
        }

        public static bool LoadDict(string path)
            => Simp.Try($"载入位于{path}的词库", () => Dict.Load(path));

        #endregion

        #region 加载单字

        public static async Task<Code?> LoadSingle(string fileName, string codeName, Initializer Initialize)
        {
            if (AutoLoadSingle(fileName, Initialize, out Code? code))
            {
                await Simp.Show($"已自动载入词库同目录中的{codeName}单字库");
                return code;
            }
            await Simp.Show($"未能自动找到{codeName}单字库，请手动选择");
            return await ManualLoadSingle(Initialize);
        }

        private static bool AutoLoadSingle(string fileName, Initializer Initialize, out Code? code)
        {
            code = null;
            var dir = Directory.GetParent(Dict.Path);
            if (!Dict.Loaded || !File.Exists(Dict.Path) || dir is null)
                return false;
            var filePath = Path.Combine(dir.FullName, fileName);
            return File.Exists(filePath)
                   && TryLoadCode(filePath, Initialize, out code);
        }

        private static async Task<Code?> ManualLoadSingle(Initializer Initialize)
        {
            var filePath = await PickYaml("选择一个以dict.yaml结尾的单字文件");
            if (TryLoadCode(filePath, Initialize, out Code? code))
            {
                await Simp.Show("成功载入指定的单字库");
                return code;
            }
            await Simp.Show("载入指定的单字库失败，自动编码将不可用");
            return null;
        }

        private static bool TryLoadCode(string filePath, Initializer Initialize, out Code? code)
        {
            Code? tempCode = null;
            code = Simp.Try(null, () => tempCode = Initialize(filePath), false)
                ? tempCode : null;
            return code?.AllowAutoCode ?? false;
        }

        #endregion
    }
}
