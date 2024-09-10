using RimeTyrant.Tools;

namespace RimeTyrant;

public partial class LogPage : ContentPage
{
    public LogPage() => InitializeComponent();

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        LogLabel.Text = Logger.ReadAll();
        ExportBtn.IsEnabled = !string.IsNullOrEmpty(LogLabel.Text);
    }

    private async void ExportBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Logger.Save(this);
            ExportBtn.IsEnabled = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("��ʾ", $"δ������־��\n{ex.Message}", "�õ�");
        }
    }
}
