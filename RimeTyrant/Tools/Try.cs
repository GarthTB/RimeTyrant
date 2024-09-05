namespace RimeTyrant.Tools
{
    internal static class Try
    {
        /// <summary>
        /// 用以简化代码
        /// </summary>
        public static bool Do(Action action)
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

        public static bool Do(Action action, Action catchAction)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception)
            {
                catchAction();
                return false;
            }
        }
    }
}
