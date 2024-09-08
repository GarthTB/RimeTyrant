namespace RimeTyrant.Tools
{
    /// <summary>
    /// 用以简化代码
    /// </summary>
    internal static class Simp
    {
        /// <summary>
        /// 尝试执行名为name的操作，如果发生异常则报错并返回false，否则什么都不显示并返回true
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

        /// <summary>
        /// 选一个文件，返回文件绝对路径，不可能为null
        /// </summary>
        public static async Task<string> GetFile(string title)
        {
            var option = new PickOptions() { PickerTitle = title };
            var result = await FilePicker.PickAsync(option);
            return result?.FullPath ?? string.Empty;
        }
    }
}
