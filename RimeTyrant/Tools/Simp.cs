namespace RimeTyrant.Tools
{
    /// <summary>
    /// 粗暴地简化代码
    /// </summary>
    internal static class Simp
    {
        /// <summary>
        /// TryCatch块的简化
        /// </summary>
        /// <param name="msg">显示在错误弹窗中的内容</param>
        /// <param name="action">要try的动作</param>
        /// <param name="showError">是否在catch时显示错误</param>
        /// <returns>无错则true，有错则false</returns>
        public static bool Try(string? msg, Action action, bool showError = true)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                if (showError)
                    _ = Show($"{msg}错误：\n{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 在MainPage上显示提示框
        /// </summary>
        public static async Task Show(string message)
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage is not null)
                await mainPage.DisplayAlert("提示", message, "好的");
        }
    }
}
