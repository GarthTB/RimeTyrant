using RimeTyrant.Tools;

namespace RimeTyrant;

public partial class LogPage : ContentPage
{
    public LogPage()
        => InitializeComponent();

    private void ContentPage_Loaded(object sender, EventArgs e)
        => LogLabel.Text = Logger.ReadAll();

    private void ExportBtn_Clicked(object sender, EventArgs e)
        => Simp.Try("导出日志", Logger.Save);
}
