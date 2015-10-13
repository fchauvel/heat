using System.Windows;

namespace Heat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Engine engine;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            engine = new AsynchronousEngine(); 
            MainWindow presenter = new MainWindow(engine);
            engine.RegisterListener(presenter);
            presenter.Show();
        }

    }

    

}
