﻿using RimeTyrant.Tools;

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

        private async void LogBtn_Clicked(object sender, EventArgs e)
            => await Navigation.PushAsync(logPage);

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
                       && encoder.Encode(ui.WordToAdd, out var codes)
                       ? codes : [];
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
            var success = Simp.Try("添加词条", () =>
            {
                Item newItem = new(ui.WordToAdd, code, priority);
                Dict.Add(newItem);
            });
            if (success)
            {
                ui.CodeToSearch = string.Empty; // 清空编码框
                ui.CodeToSearch = code; // 重新搜索，相当于刷新
                ui.AllowAdd = false; // 避免重复添加
                ui.AllowMod = true;
            }
        }

        #endregion

        #region 选中编码

        private void ResultArray_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ui.AllowDel = e.SelectedItem is Item;
            ui.AllowCut = e.SelectedItem is Item && e.SelectedItemIndex > 0;
        }

        #endregion
    }
}
