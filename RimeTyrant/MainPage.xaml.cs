using RimeTyrant.Tools;

namespace RimeTyrant
{
    public partial class MainPage : ContentPage
    {
        #region 初始化

        private readonly UI ui = new();

        private readonly Encoder encoder = new();

        private readonly LogPage logPage = new();

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
        {
            if (!Dict.Loaded)
                FileLoader.AutoLoadDict();
        }

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
        }

        private void LogBtn_Clicked(object sender, EventArgs e)
            => Navigation.PushAsync(logPage);

        private bool Unsaved { get; set; } = false;

        private void ModBtn_Clicked(object sender, EventArgs e)
        {
            // 应用修改
            // 如果保存到原文件失败，就保存到新位置
        }

        #endregion

        #region 涉及多个控件的逻辑

        /// <summary>
        /// 检查是否允许添加词，仅由五个控件触发：加词框、自动编码选单、手动编码框、勾选优先级、优先级框
        /// </summary>
        private void CheckAddBtn()
        {
            var codeValid = (ui.UseAutoEncode && encoder.IsValidCode(ui.AutoCode))
                            || (ui.UseManualEncode && encoder.IsValidCode(ui.ManualCode));
            var priorityValid = !ui.UsePriority
                                || (ui.UsePriority && encoder.IsValidPriority(ui.Priority));
            ui.AllowAdd = Dict.Loaded
                          && codeValid
                          && priorityValid;
        }

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
        /// 加载自动编码，仅由三个控件触发：加词框、勾选自动编码、编码方案选单
        /// </summary>
        private void LoadAutoCodes()
        {
            var load = Dict.Loaded
                       && ui.UseAutoEncode
                       && !string.IsNullOrEmpty(ui.EncodeMethod)
                       && encoder.Ready(ui.EncodeMethod)
                       && encoder.Encode(ui.WordToAdd, out var fullCodes)
                       ? fullCodes : [];
            ui.OriginAutoCodeArray = load;
            // 有多项则变红，但是不知道为什么鼠标悬停就会变回原来颜色
            ui.AutoCodeColor = load.Length > 1
                ? "IndianRed"
                : CodeToSearch.TextColor.ToHex();
        }

        /// <summary>
        /// 自动搜索，仅由两个控件触发：自动编码选单、手动编码框
        /// </summary>
        private void AutoSearch()
        {
            if (Dict.Loaded)
                ui.CodeToSearch = ui.UseAutoEncode && !string.IsNullOrEmpty(ui.AutoCode)
                    ? ui.AutoCode
                    : ui.UseManualEncode && !string.IsNullOrEmpty(ui.ManualCode)
                    ? ui.ManualCode
                    : string.Empty;
        }

        #endregion

        #region 加词框

        private void WordToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (encoder.IsValidWord(ui.WordToAdd))
                ui.WordColor = Dict.HasWord(ui.WordToAdd)
                    ? "IndianRed"
                    : CodeToSearch.TextColor.ToHex();
            LoadAutoCodes();
            CheckAddBtn();
        }

        #endregion

        #region 自动编码

        private async void UseAutoEncode_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            await InitializeEncoder();
            LoadAutoCodes();
        }

        private async void EncodeMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            await InitializeEncoder();
            LoadAutoCodes();
        }

        private void AutoCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoSearch();
            CheckAddBtn();
        }

        private void CodeLength_SelectedIndexChanged(object sender, EventArgs e)
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

        #region 优先级

        private void UsePriority_CheckedChanged(object sender, CheckedChangedEventArgs e)
            => CheckAddBtn();

        private void Priority_TextChanged(object sender, TextChangedEventArgs e)
            => CheckAddBtn();

        #endregion

        #region 添加按钮

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            var code = ui.UseAutoEncode
                ? ui.AutoCode ?? string.Empty // 如果为空，不应该通过合法检查
                : ui.ManualCode;
            var priority = ui.UsePriority && ui.Priority.Length != 0
                ? ui.Priority
                : "0";
            var newItem = new Item(ui.WordToAdd, code, priority);

            var success = Simp.Try("添加词条", () => Dict.Add(newItem));

            if (success)
            {
                ui.OriginResultArray = [.. ui.OriginResultArray.Append(newItem).OrderBy(x => x.Code)];
                ui.AllowAdd = false; // 避免重复添加
                ui.AllowMod = Unsaved = true;
            }
        }

        #endregion

        #region 选中编码

        private int SelectedIndex { get; set; }

        private void ResultArray_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Item item)
            {
                SelectedIndex = e.SelectedItemIndex;
                ui.AllowDel = true;
                ui.AllowCut = item.Code != ui.CodeToSearch;
            }
            else
            {
                SelectedIndex = -1;
                ui.AllowDel = ui.AllowCut = false;
            }
        }

        #endregion

        #region 改动了搜索结果

        private void Result_Modified(object sender, TextChangedEventArgs e)
            => ui.AllowMod = Unsaved || Modified();

        private bool Modified()
        {
            for (int i = 0; i < ui.ResultArray.Length; i++)
                if (!ui.ResultArray[i].Equals(ui.OriginResultArray[i]))
                    return true;
            return false;
        }

        #endregion

        #region 删除按钮

        private void DelBtn_Clicked(object sender, EventArgs e)
        {
            // SelectedIndex不合格时这个按钮不应该激活
            var success = Simp.Try("删除词条", ()
                => Dict.Remove(ui.OriginResultArray[SelectedIndex]));

            if (success)
            {
                ui.OriginResultArray = [.. ui.OriginResultArray
                    .Where((element, idx) => idx != SelectedIndex)
                    .OrderBy(x => x.Code)];
                ui.AllowDel = false;
                ui.AllowMod = Unsaved = true;
            }
        }

        #endregion

        #region 截短按钮

        private void CutBtn_Clicked(object sender, EventArgs e)
        {
            var oriItem = ui.OriginResultArray[SelectedIndex].Clone(); // 选中的词
            var sLen = ui.CodeToSearch.Length; // 要把选中的词缩到这么短
            if (encoder.CutCode(oriItem.Code, sLen, out var sCode)) // 缩短编码
            {
                var newItem = new Item(oriItem.Word, sCode, oriItem.Priority); // 缩短后的词
                var blocker = ui.OriginResultArray.FirstOrDefault(x => x.Code == sCode); // 占位的词
                var success = blocker is null
                    ? OneWay(oriItem, newItem, sCode) // 没有占位词，直接移动
                    : TwoWay(oriItem, newItem, sCode, blocker); // 有占位词，双向移动
            }
            else Simp.Show("无法自动截短！未操作。");
        }

        private static bool OneWay(Item oriItem, Item newItem, string sCode)
            => Simp.Try("除去选中的长词", () => Dict.Remove(oriItem))
               && Simp.Try($"截至{sCode.Length}码", () => Dict.Add(newItem));

        private bool TwoWay(Item oriItem, Item newItem, string sCode, Item blocker)
        {
            var lLen = oriItem.Code.Length; // 要把占位的词扩到这么长
            encoder.Lengthen(blocker.Word, sCode, out var lCode);
        }

        #endregion
    }
}
