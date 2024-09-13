using RimeTyrant.Tools;
using static Java.Util.Jar.Attributes;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

namespace RimeTyrant
{
    internal class AndroidFile
    {
        public static bool Write(string dirName, string fileName, string content)
            => Simp.Try($"在{dirName}写入文件{fileName}失败。", () =>
            {
                var folderPath = GetDirOrCreate(dirName);
                var filePath = Path.Combine(folderPath, fileName);
                File.WriteAllText(filePath, content);
            });

        public static async Task<bool> CopyTo(string oriPath, string dirName)
        {
            try
            {
                var folderPath = GetDirOrCreate(dirName);
                var fileName = Path.GetFileName(oriPath);
                string destPath = Path.Combine(folderPath, fileName);
                if (File.Exists(destPath))
                    await Simp.Show($"文件夹：\n{folderPath}\n中已存在文件：\n{fileName}\n将直接覆写");
                File.Copy(oriPath, destPath, true);
                Logger.Add($"将词库拷贝到{destPath}");
                return true;
            }
            catch (Exception ex)
            {
                await Simp.Show($"将词库拷贝到外部路径失败。错误：\n{ex.Message}");
                return false;
            }
        }

        private static string GetDirOrCreate(string dirName)
        {
            var dir = (Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath)
                ?? throw new Exception("无法获取外部路径");

            var folderPath = Path.Combine(dir, dirName);
            if (!Directory.Exists(folderPath))
                _ = Directory.CreateDirectory(folderPath);

            return folderPath;
        }
    }
}

#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
