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

            window.Width = window.MaximumWidth = window.MinimumWidth = 700;
            window.Height = window.MaximumHeight = window.MinimumHeight = 448;

            return window;
        }
    }
}
