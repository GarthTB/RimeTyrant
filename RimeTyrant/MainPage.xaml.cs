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
            ui.EncodeMethodArray = ["键道6", "<待续>"];
        }

        #endregion

        #region 加载、保存、日志

        private void MainPage_Loaded(object sender, EventArgs e)
            => FileLoader.AutoLoadDict(this);

        private void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            var dict = FileLoader.PickYaml("选择一个以dict.yaml结尾的词库文件");
            if (!string.IsNullOrEmpty(dict) && FileLoader.LoadDict(dict))
            {
                _ = DisplayAlert("提示", "载入成功！", "好的");
                ui.WordToAdd = string.Empty;
                ui.ManualCode = string.Empty;
                ui.Priority = string.Empty;
                ui.CodeToSearch = string.Empty;
            }
            else _ = DisplayAlert("提示", "未载入词库文件！", "好的");
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
            if (!string.IsNullOrWhiteSpace(ui.WordToAdd)
                && Text.IsValidWord(ui.WordToAdd))
            {
                ui.WordColor = Dict.HasWord(ui.WordToAdd)
                    ? "IndianRed"
                    : string.Empty;

                if (ui.UseAutoEncode && ui.EncodeMethod is not null && encoder.Ready(ui.EncodeMethod))
                    LoadAutoCodes();
            }
            else ui.AutoCodeArray = [];
            CheckAddBtn();
        }

        #endregion

        #region 自动编码

        private void UseAutoEncode_CheckedChanged(object sender, CheckedChangedEventArgs e)
            => InitializeEncoder();

        #endregion

        #region 涉及多个控件的逻辑

        private void CheckAddBtn()
            => ui.AllowAdd = Dict.Loaded
                             && ((ui.UseAutoEncode && ui.AutoCode is not null && Text.IsValidCode(ui.AutoCode))
                             || (ui.UseManualEncode && Text.IsValidCode(ui.ManualCode)));

        private void InitializeEncoder()
        {
            if (ui.UseAutoEncode && ui.EncodeMethod is not null)
            {
                _ = encoder.SetCode(ui.EncodeMethod, this, out var array);
                ui.ValidCodeLengthArray = array;
            }
        }

        private void LoadAutoCodes()
        {

        }

        #endregion
    }
}
