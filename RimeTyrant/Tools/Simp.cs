﻿namespace RimeTyrant.Tools
{
    /// <summary>
    /// 用以简化代码
    /// </summary>
    internal static class Simp
    {
        /// <summary>
        /// 尝试执行一个方法，如果发生异常则返回false
        /// </summary>
        public static bool Try(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 显示一个提示框
        /// </summary>
        public static void Show(Page page, string message)
            => MainThread.BeginInvokeOnMainThread(
                async () => await page.DisplayAlert("提示", message, "好的"));
    }
}
