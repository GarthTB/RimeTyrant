using Android.App;
using Android.Runtime;
using RimeTyrant.Tools;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

namespace RimeTyrant
{
    [Application]
    public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

    internal class FileService : IAndroidFile
    {
        public async Task<bool> AndroidWriteFile(string dirName, string fileName, string content)
        {
            try
            {
                var folderPath = GetDirOrCreate(dirName);
                var filePath = Path.Combine(folderPath, fileName);
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                await Simp.Show($"在{dirName}写入文件{fileName}失败。错误：\n{ex.Message}");
                return false;
            }
        }

        public async Task<bool> AndroidCopyTo(string oriPath, string dirName)
        {
            try
            {
                var folderPath = GetDirOrCreate(dirName);
                var fileName = Path.GetFileName(oriPath);
                File.Copy(oriPath, Path.Combine(folderPath, fileName));
                return true;
            }
            catch (Exception ex)
            {
                await Simp.Show($"将文件拷贝到外部路径失败。错误：\n{ex.Message}");
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
