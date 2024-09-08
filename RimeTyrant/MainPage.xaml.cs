using RimeTyrant.Tools;

namespace RimeTyrant
{
    public partial class MainPage : ContentPage
    {
        #region 初始化

        private readonly UI ui = new();

        private readonly Encoder encoder = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = ui;
            ui.encoder = encoder;
            ui.EncodeMethodArray = ["键道6", "<待续>"];
        }

        #endregion

        #region 加载、保存、日志

        private void MainPage_Loaded(object sender, EventArgs e)
            => FileLoader.AutoLoadDict();

        private async void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            var dict = await FileLoader.PickYaml("选择一个以dict.yaml结尾的词库文件");
            if (!string.IsNullOrEmpty(dict) && FileLoader.LoadDict(dict))
            {
                Simp.Show("载入成功！");
                ui.WordToAdd = string.Empty;
                ui.ManualCode = string.Empty;
                ui.Priority = string.Empty;
                ui.CodeToSearch = string.Empty;
            }
            else Simp.Show("未载入词库文件！");
        }

        private void LogBtn_Clicked(object sender, EventArgs e)
        {
            // 显示日志页
        }

        private void ModBtn_Clicked(object sender, EventArgs e)
        {
            // 应用修改
            // 如果保存到原文件失败，就保存到新位置
        }

        #endregion

        #region 加词框

        private void WordToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (encoder.IsValidWord(ui.WordToAdd))
            {
                ui.WordColor = Dict.HasWord(ui.WordToAdd)
                    ? "IndianRed"
                    : CodeToSearch.TextColor.ToHex();

                if (ui.UseAutoEncode)
                    LoadAutoCodes();
            }
            else ui.OriginAutoCodeArray = [];
            CheckAddBtn();
        }

        #endregion

        #region 自动编码

        private async void UseAutoEncode_CheckedChanged(object sender, CheckedChangedEventArgs e)
            => await InitializeEncoder();

        private async void EncodeMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            await InitializeEncoder();
            if (!string.IsNullOrEmpty(ui.EncodeMethod)&& encoder.Ready(ui.EncodeMethod))
                LoadAutoCodes();
        }

        private void AutoCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoSearch();
            CheckAddBtn();
        }

        #endregion

        #region 手动编码

        private void ManualEncode_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSearch();
            CheckAddBtn();
        }

        #endregion

        #region 涉及多个控件的逻辑

        /// <summary>
        /// 检查是否允许添加词，仅由三个控件触发：加词框、自动编码选单、手动编码框
        /// </summary>
        private void CheckAddBtn()
            => ui.AllowAdd = Dict.Loaded
                             && ((ui.UseAutoEncode && encoder.IsValidCode(ui.AutoCode))
                             || (ui.UseManualEncode && encoder.IsValidCode(ui.ManualCode)));

        /// <summary>
        /// 初始化自动编码器，仅由两个控件触发：勾选自动编码、编码方案选单
        /// </summary>
        private async Task InitializeEncoder()
        {
            if (Dict.Loaded
                && ui.UseAutoEncode
                && !string.IsNullOrEmpty(ui.EncodeMethod)
                && !encoder.Ready(ui.EncodeMethod))
                (ui.ValidCodeLengthArray, ui.CodeLengthIndex) = await encoder.SetCode(ui.EncodeMethod);
        }

        /// <summary>
        /// 加载自动编码，仅由四个控件触发：加词框、勾选自动编码、编码方案选单、码长选单
        /// </summary>
        private void LoadAutoCodes()
        {
            if (Dict.Loaded
                && ui.UseAutoEncode
                && !string.IsNullOrEmpty(ui.EncodeMethod)
                && encoder.Ready(ui.EncodeMethod)
                && encoder.Encode(ui.WordToAdd, out var codes))
                ui.OriginAutoCodeArray = codes;
            // 有多项则变红，但是不知道为什么鼠标悬停就会变回原来颜色
            ui.AutoCodeColor = ui.AutoCodeArray.Length > 1
                ? "IndianRed"
                : CodeToSearch.TextColor.ToHex();
        }

        /// <summary>
        /// 自动编码，仅由两个控件触发：自动编码选单、手动编码框
        /// </summary>
        private void AutoSearch()
            => ui.OriginResultArray = ui.UseAutoEncode && !string.IsNullOrEmpty(ui.AutoCode)
                ? [.. Dict.CodeStartsWith(ui.AutoCode)]
                : ui.UseManualEncode && !string.IsNullOrEmpty(ui.ManualCode)
                    ? [.. Dict.CodeStartsWith(ui.ManualCode)]
                    : [];

        #endregion
    }
}
