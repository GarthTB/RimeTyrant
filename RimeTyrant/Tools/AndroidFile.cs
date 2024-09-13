#if ANDROID

namespace RimeTyrant.Tools
{
    internal class AndroidFile
    {
        public static async Task<bool> Write(string dirName, string fileName, string content)
        {
            var dep = DependencyService.Get<IFileService>();
            return dep is not null
                   && await dep.AndroidWriteFile(dirName, fileName, content);
        }

        public static async Task<bool> CopyTo(string oriPath, string dirName)
        {
            var dep = DependencyService.Get<IFileService>();
            return dep is not null
                   && await dep.AndroidCopyTo(oriPath, dirName);
        }
    }
}

#endif
