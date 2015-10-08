using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Heat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow presentation;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            presentation = new MainWindow(this);
            presentation.Show();
        }

        public void OnGo()
        {
            Circuit circuit = new Circuit(new string[] { "push-ups", "burpees" });
            Session session = new Session(circuit, new Level(2));
            session.Run(new Trainee());
            presentation.ShowText("<b>Bonjour</b>");
        }
    }

}
