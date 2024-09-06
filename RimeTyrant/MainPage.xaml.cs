﻿using RimeTyrant.Tools;

namespace RimeTyrant
{
    public partial class MainPage : ContentPage
    {
        #region 初始化

        private readonly UI ui = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = ui;
        }

        #endregion

        #region 加载、保存、日志

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            _ = DictLoader.AutoLoadDict(out string message)
                ? DisplayAlert("提示", message, "好的")
                : DisplayAlert("提示", message, "好的");
        }

        private async void ReloadBtn_Clicked(object sender, EventArgs e)
        {
            var dict = await Loader.PickYaml();
            if (dict is not null)
            {
                if (DictLoader.LoadDict(dict.FullPath))
                {
                    _ = DisplayAlert("提示", "载入成功！", "好的");
                    ui.WordToAdd = string.Empty;
                    ui.ManualCode = string.Empty;
                    ui.Priority = string.Empty;
                    ui.CodeToSearch = string.Empty;
                }
                else await DisplayAlert("提示", "载入失败！", "好的");
            }
            else await DisplayAlert("提示", "未选择有效文件！", "好的");
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

        private void WordToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
