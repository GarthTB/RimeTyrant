using Android.App;
using Android.Runtime;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

namespace RimeTyrant
{
    [Application]
    public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配
