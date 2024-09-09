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
                RefreshAdd(newItem);
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
                return;
            }
            SelectedIndex = -1;
            ui.AllowDel = false;
            ui.AllowCut = false;
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
                RefreshDel(SelectedIndex);
                ui.AllowDel = false;
                ui.AllowMod = Unsaved = true;
            }
        }

        #endregion

        #region 截短按钮

        private void CutBtn_Clicked(object sender, EventArgs e)
        {
            if (Simp.Try("自动截短", CutTheItem))
            {
                ui.AllowCut = false;
                ui.AllowMod = Unsaved = true;
            }
        }

        private void CutTheItem()
        {
            var sel_item = ui.OriginResultArray[SelectedIndex].Clone();
            var tar_leng = ui.CodeToSearch.Length;
            if (!encoder.CutCode(sel_item.Code, tar_leng, out var cut_code))
                throw new Exception("无法自动截短。未操作。");
            if (cut_code != ui.CodeToSearch)
                throw new Exception("选中词条的短码和搜索框内的编码不一致。未操作。");
            var cut_item = new Item(sel_item.Word, cut_code, sel_item.Priority);
            // 检查截短后的编码是否和现有的编码冲突，如果有，则要加长占位的码
            var plc_hldr = ui.OriginResultArray.FirstOrDefault(x => x.Code == cut_code)?.Clone();
            if (plc_hldr is null)
            {
                if (Simp.Try("删除选中的过长条目", () => Dict.Remove(sel_item, "删除选中的过长条目"))
                    && Simp.Try("添加截短后的新条目", () => Dict.Add(cut_item, "添加截短后的新条目")))
                {
                    RefreshDel(SelectedIndex);
                    RefreshAdd(cut_item);
                }
            }
            else
            {
                var index = Array.FindIndex(ui.OriginResultArray, x => x.Code == cut_code);
                var moved = TwoWays(sel_item, plc_hldr);
                if (Simp.Try("删除选中的过长条目", () => Dict.Remove(sel_item, "删除选中的过长条目"))
                    && Simp.Try("添加截短后的新条目", () => Dict.Add(cut_item, "添加截短后的新条目"))
                    && Simp.Try("删除占位的短码条目", () => Dict.Remove(plc_hldr, "删除占位的短码条目"))
                    && Simp.Try("添加加长后的新条目", () => Dict.Add(moved, "添加加长后的新条目")))
                {
                    RefreshDel(SelectedIndex);
                    RefreshAdd(cut_item);
                    RefreshDel(index);
                    RefreshAdd(moved);
                }
            }
        }

        private Item TwoWays(Item sel_item, Item plc_hldr)
        {
            if (!encoder.Encode(plc_hldr.Word, out var fullCodes))
                throw new Exception("无法为占位的词编码。未操作。");
            var cut_codes = fullCodes.Select(c
                => !encoder.CutCode(c, sel_item.Code.Length, out var cut_c)
                    ? throw new Exception("无法自动截短占位词的全码。未操作。")
                    : cut_c);
            var moved = cut_codes.Contains(sel_item.Code)
                ? Cross(sel_item, plc_hldr)
                : FindNew(plc_hldr, fullCodes);
            return moved;
        }

        private static Item Cross(Item sel_item, Item plc_hldr)
            => new(plc_hldr.Word, sel_item.Code, plc_hldr.Priority);

        private Item FindNew(Item plc_hldr, string[] fullCodes)
        {
            var range = ui.ValidCodeLengthArray.Where(l => l > plc_hldr.Code.Length).Order();
            var new_code = string.Empty;
            foreach (var len in range)
            {
                if (!encoder.CutCodes(fullCodes, len, out var new_codes))
                    throw new Exception("无法为占位的词编码。未操作。");
                if (new_codes.Length == 1 && !Dict.HasCode(new_codes[0]))
                {
                    new_code = new_codes[0];
                    break;
                }
            }
            return new_code.Length > 0
                ? new Item(plc_hldr.Word, new_code, plc_hldr.Priority)
                : throw new Exception("没有为占位的词找到匹配且空余的长码。未操作。");
        }

        #endregion

        #region 更新搜索结果

        private void RefreshAdd(Item newItem)
            => ui.OriginResultArray = [.. ui.OriginResultArray
                .Append(newItem)
                .OrderBy(x => x.Code)];

        private void RefreshDel(int index)
            => ui.OriginResultArray = [.. ui.OriginResultArray
                .Where((element, idx) => idx != index)
                .OrderBy(x => x.Code)];

        #endregion
    }
}
