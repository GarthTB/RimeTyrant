namespace RimeTyrant
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Width = 640;
            window.Height = 448;
            window.MinimumWidth = 280;
            window.MinimumHeight = 60;
            return window;
        }
    }
}
