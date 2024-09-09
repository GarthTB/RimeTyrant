﻿namespace RimeTyrant.Tools
{
    /// <summary>
    /// 粗暴地简化代码
    /// </summary>
    internal static class Simp
    {
        /// <summary>
        /// TryCatch块的简化
        /// </summary>
        /// <param name="name">显示在错误弹窗中的内容</param>
        /// <param name="action">要try的动作</param>
        /// <param name="showError">是否在catch时显示错误</param>
        /// <returns>有catch则false，无catch则true</returns>
        public static bool Try(string name, Action action, bool showError = true)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                    Show($"{name}出错：\n{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 显示一个提示框
        /// </summary>
        public static void Show(string message)
            => MainThread.BeginInvokeOnMainThread(async () =>
            {
                var currentPage = Application.Current?.MainPage;
                if (currentPage is not null)
                    await currentPage.DisplayAlert("提示", message, "好的");
            });
    }
}
