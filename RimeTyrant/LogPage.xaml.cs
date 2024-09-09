using RimeTyrant.Tools;

namespace RimeTyrant;

public partial class LogPage : ContentPage
{
    public LogPage()
        => InitializeComponent();

    private void ContentPage_Loaded(object sender, EventArgs e)
        => LogLabel.Text = Logger.ReadAll();

    private async void ExportBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Logger.Save(this);
        }
        catch (Exception ex)
        {
            await DisplayAlert("提示", $"未导出日志：\n{ex.Message}", "好的");
        }
    }
}
