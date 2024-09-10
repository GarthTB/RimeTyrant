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
            ui.EncodeMethods = ["键道6", "<待续>"];
        }

        #endregion

        #region 加载、日志

        private async void MainPage_Loaded(object sender, EventArgs e)
        {
            if (!Dict.Loaded)
                await DisplayAlert("提示", FileLoader.AutoLoadDict(), "好的");
        }

        private async void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            var dict = await FileLoader.PickYaml("选择一个以dict.yaml结尾的词库文件");
            if (string.IsNullOrEmpty(dict))
                await DisplayAlert("提示", "未选择文件！", "好的");
            else if (FileLoader.LoadDict(dict))
            {
                await DisplayAlert("提示", "载入成功！", "好的");
                UseAutoEncode.IsChecked = false;
                WordToAdd.Text = string.Empty;
                ManualEncode.Text = string.Empty;
                Priority.Text = string.Empty;
                CodeToSearch.Text = string.Empty;
            }
        }

        private void LogBtn_Clicked(object sender, EventArgs e)
            => Navigation.PushAsync(logPage);

        #endregion

        #region 涉及多个控件的逻辑

        /// <summary>
        /// 检查是否允许添加词，由左边8个控件触发
        /// </summary>
        private void CheckAddBtn()
        {
            var codeValid = (UseAutoEncode.IsChecked && encoder.IsValidCode(ui.AutoCode))
                            || (!UseAutoEncode.IsChecked && encoder.IsValidCode(ManualEncode.Text));
            var priorityValid = !UsePriority.IsChecked
                                || (UsePriority.IsChecked && encoder.IsValidPriority(Priority.Text));
            AddBtn.IsEnabled = Dict.Loaded
                               && codeValid
                               && priorityValid;
        }

        /// <summary>
        /// 初始化自动编码器，仅由两个控件触发：勾选自动编码、编码方案选单
        /// </summary>
        private async Task InitializeEncoder()
        {
            if (Dict.Loaded
                && UseAutoEncode.IsChecked
                && !string.IsNullOrEmpty(ui.EncodeMethod)
                && !encoder.Ready(ui.EncodeMethod))
            {
                (ui.ValidCodeLengths, CodeLength.SelectedIndex) = await encoder.SetCode(ui.EncodeMethod);
                UseAutoEncode.IsChecked = encoder.Ready(ui.EncodeMethod);
            }
        }

        /// <summary>
        /// 加载自动编码，仅由三个控件触发：加词框、勾选自动编码、编码方案选单
        /// </summary>
        private void LoadAutoCodes()
        {
            var fc = Dict.Loaded
                     && UseAutoEncode.IsChecked
                     && !string.IsNullOrEmpty(ui.EncodeMethod)
                     && encoder.Ready(ui.EncodeMethod)
                     && encoder.Encode(WordToAdd.Text, out var fullCodes)
                     ? fullCodes : [];
            ui.FullCodes = fc;
            // 有多项则变红，但是不知道为什么鼠标悬停就会变回原来颜色
            AutoCode.TextColor = fc.Length > 1
                ? Color.FromRgb(214, 100, 0)
                : CodeToSearch.TextColor;
        }

        /// <summary>
        /// 自动搜索，仅由两个控件触发：自动编码选单、手动编码框
        /// </summary>
        private void AutoSearch()
        {
            if (Dict.Loaded)
                CodeToSearch.Text = UseAutoEncode.IsChecked && !string.IsNullOrEmpty(ui.AutoCode)
                    ? ui.AutoCode
                    : !UseAutoEncode.IsChecked && !string.IsNullOrEmpty(ManualEncode.Text)
                    ? ManualEncode.Text
                    : string.Empty;
        }

        #endregion

        #region 更新搜索结果

        private void RefreshAdd(Item newItem)
            => ui.OriginResults = [.. ui.OriginResults
                .Append(newItem)
                .OrderBy(x => x.Code)];

        private void RefreshDel(int index)
            => ui.OriginResults = [.. ui.OriginResults
                .Where((element, idx) => idx != index)
                .OrderBy(x => x.Code)];

        #endregion

        #region 加词框

        private void WordToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (encoder.IsValidWord(WordToAdd.Text))
                WordToAdd.TextColor = Dict.HasWord(WordToAdd.Text)
                    ? Color.FromRgb(214, 100, 0)
                    : CodeToSearch.TextColor;
            LoadAutoCodes();
            CheckAddBtn();
        }

        #endregion

        #region 自动编码

        private async void UseAutoEncode_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            await InitializeEncoder();
            LoadAutoCodes();
            CheckAddBtn();
        }

        private async void EncodeMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            await InitializeEncoder();
            LoadAutoCodes();
            CheckAddBtn();
        }

        private void CodeLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoSearch();
            CheckAddBtn();
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

        #region 优先级

        private void UsePriority_CheckedChanged(object sender, CheckedChangedEventArgs e)
            => CheckAddBtn();

        private void Priority_TextChanged(object sender, TextChangedEventArgs e)
            => CheckAddBtn();

        #endregion

        #region 添加按钮

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            var code = UseAutoEncode.IsChecked
                ? ui.AutoCode ?? string.Empty // 如果为空，不应该通过合法检查
                : ManualEncode.Text;
            var priority = UsePriority.IsChecked && Priority.Text.Length != 0
                ? Priority.Text
                : "0";
            var newItem = new Item(WordToAdd.Text, code, priority);

            var success = Simp.Try("添加词条", () => Dict.Add(newItem));

            if (success)
            {
                RefreshAdd(newItem);
                AddBtn.IsEnabled = false; // 避免重复添加
                ModBtn.IsEnabled = Unsaved = true;
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
                DelBtn.IsEnabled = true;
                CutBtn.IsEnabled = item.Code != CodeToSearch.Text;
                return;
            }
            SelectedIndex = -1;
            DelBtn.IsEnabled = false;
            CutBtn.IsEnabled = false;
        }

        #endregion

        #region 改动了搜索结果

        private void Result_Modified(object sender, TextChangedEventArgs e)
            => ModBtn.IsEnabled = Unsaved || Modified();

        private bool Modified()
        {
            for (int i = 0; i < ui.ViewResults.Length; i++)
                if (!ui.ViewResults[i].Equals(ui.OriginResults[i]))
                    return true;
            return false;
        }

        #endregion

        #region 删除按钮

        private void DelBtn_Clicked(object sender, EventArgs e)
        {
            // SelectedIndex不合格时这个按钮不应该激活
            var success = Simp.Try("删除词条", ()
                => Dict.Remove(ui.OriginResults[SelectedIndex]));

            if (success)
            {
                RefreshDel(SelectedIndex);
                DelBtn.IsEnabled = false;
                ModBtn.IsEnabled = Unsaved = true;
            }
        }

        #endregion

        #region 截短按钮

        private void CutBtn_Clicked(object sender, EventArgs e)
        {
            if (Simp.Try("自动截短", CutTheItem))
            {
                CutBtn.IsEnabled = false;
                ModBtn.IsEnabled = Unsaved = true;
            }
        }

        private void CutTheItem()
        {
            var sel_item = ui.OriginResults[SelectedIndex].Clone();
            var tar_leng = ui.CodeToSearch.Length;
            if (!encoder.CutCode(sel_item.Code, tar_leng, out var cut_code))
                throw new Exception("无法自动截短。未操作。");
            if (cut_code != ui.CodeToSearch)
                throw new Exception("选中词条的短码和搜索框内的编码不一致。未操作。");
            var cut_item = new Item(sel_item.Word, cut_code, sel_item.Priority);
            // 检查截短后的编码是否和现有的编码冲突，如果有，则要加长占位的码
            var plc_hldr = ui.OriginResults.FirstOrDefault(x => x.Code == cut_code)?.Clone();
            if (plc_hldr is null)
                OneWay(sel_item, cut_item); // 没有冲突，直接缩短
            else TwoWay(sel_item, cut_code, cut_item, plc_hldr); // 有冲突，处理占位的码

            void OneWay(Item sel_item, Item cut_item)
            {
                if (Simp.Try("删除选中的过长条目", () => Dict.Remove(sel_item, "删除：过长"))
                    && Simp.Try("添加截短后的新条目", () => Dict.Add(cut_item, "添加：截短")))
                {
                    RefreshDel(SelectedIndex);
                    RefreshAdd(cut_item);
                }
            }

            void TwoWay(Item sel_item, string cut_code, Item cut_item, Item plc_hldr)
            {
                var index = Array.FindIndex(ui.OriginResults, x => x.Code == cut_code);
                var moved = Move(sel_item, plc_hldr);
                if (Simp.Try("删除选中的过长条目", () => Dict.Remove(sel_item, "删除：过长"))
                    && Simp.Try("添加截短后的新条目", () => Dict.Add(cut_item, "添加：截短"))
                    && Simp.Try("删除占位的短码条目", () => Dict.Remove(plc_hldr, "删除：占位"))
                    && Simp.Try("添加加长后的新条目", () => Dict.Add(moved, "添加：加长")))
                {
                    RefreshDel(SelectedIndex);
                    RefreshAdd(cut_item);
                    RefreshDel(index);
                    RefreshAdd(moved);
                }
            }
        }

        private Item Move(Item sel_item, Item plc_hldr)
        {
            if (!encoder.Encode(plc_hldr.Word, out var fullCodes))
                throw new Exception("无法为占位的词编码。未操作。");
            var cut_codes = fullCodes.Select(c
                => !encoder.CutCode(c, sel_item.Code.Length, out var cut_c)
                    ? throw new Exception("无法自动截短占位词的全码。未操作。")
                    : cut_c);
            return cut_codes.Contains(sel_item.Code)
                ? Exchange(sel_item, plc_hldr) // 刚好可以对调
                : FindNew(plc_hldr, fullCodes); // 不能对调，另外加长

            static Item Exchange(Item sel_item, Item plc_hldr)
                => new(plc_hldr.Word, sel_item.Code, plc_hldr.Priority);

            Item FindNew(Item plc_hldr, string[] fullCodes)
            {
                var range = ui.ValidCodeLengths.Where(l => l > plc_hldr.Code.Length).Order();
                var new_code = IterFind(fullCodes, range);
                return new_code.Length > 0
                    ? new Item(plc_hldr.Word, new_code, plc_hldr.Priority)
                    : throw new Exception("没有为占位的词找到匹配且空余的长码。未操作。");

                string IterFind(string[] fullCodes, IOrderedEnumerable<int> range)
                {
                    foreach (var len in range)
                    {
                        if (!encoder.CutCodes(fullCodes, len, out var new_codes))
                            throw new Exception("无法为占位的词编码。未操作。");
                        if (new_codes.Length == 1 && !Dict.HasCode(new_codes[0]))
                            return new_codes[0];
                    }
                    return string.Empty;
                }
            }
        }

        #endregion

        #region 应用修改并保存

        private bool Unsaved { get; set; } = false;

        private async void ModBtn_Clicked(object sender, EventArgs e)
        {
            // 没有修改则恒为真，有修改则为修改的成败
            var modSuccess = !Modified()
                             || Simp.Try("应用修改", ApplyModify);

            if (modSuccess && await SaveDict())
            {
                await DisplayAlert("提示", "应用并保存成功！", "好的");
                Unsaved = ModBtn.IsEnabled = false;
            }
        }

        private async Task<bool> SaveDict()
        {
            if (Simp.Try("保存修改后的词库失败，将自动保存至默认位置。", () => Dict.Save()))
                return true;

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dict");
            if (!Directory.Exists(dir))
                _ = Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, $"RimeTyrant_Modified.dict.yaml");
            if (File.Exists(path))
                await DisplayAlert("提示", "默认位置已存在词库文件，将覆写", "好的");

            return Simp.Try("保存至默认位置也失败了。若想避免数据损失，请照着日志手动修改。",
                            () => Dict.Save(path));
        }

        private void ApplyModify()
        {
            var modified = ui.ViewResults
                .Where(item => !ui.OriginResults.Any(x => x.Equals(item)))
                .Select(item => item.Clone())
                .ToArray();

            if (modified.Length == 0)
                throw new Exception("没有任何修改！");

            var discards = ui.OriginResults
                .Where(item => !ui.ViewResults.Any(x => x.Equals(item)))
                .Select(item => item.Clone())
                .ToArray();

            if (modified.Length != discards.Length)
                throw new Exception("修改前后数量不一致！这是不该出现的错误，如果出现请联系开发者。");

            Logger.Add($"---以下是手动修改的内容，共{modified.Length}项。---");
            Dict.RemoveAll(discards);
            Dict.AddAll(modified);
            Logger.Add($"---成功完成{modified.Length}项手动修改。---");
        }

        #endregion
    }
}
