namespace RimeTyrant.Tools
{
    internal static class Loader
    {
        public static async Task<FileResult?> PickYaml()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();

                return result is not null && result.FileName.EndsWith("dict.yaml", StringComparison.OrdinalIgnoreCase)
                    ? result
                    : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
