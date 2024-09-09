namespace RimeTyrant.Tools
{
    /// <summary>
    /// 用以简化代码
    /// </summary>
    internal static class Simp
    {
        /// <summary>
        /// 尝试执行操作，静默返回成功性
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
        /// 尝试执行名为name的操作，成功无提示返回true，否则报错并返回false
        /// </summary>
        public static bool Try(string name, Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
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
