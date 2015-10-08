using System.Threading;
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
            new Thread(() =>
            {
                Circuit circuit = new Circuit(new string[] { "push-ups", "burpees" });
                Session session = new Session(circuit, new Level(2));
                session.Run(new TraineeAdapter(presentation));
            }).Start();
        }
    }

    class TraineeAdapter: Trainee
    {
        private readonly MainWindow presenter;
        
        public TraineeAdapter(MainWindow presenter)
        {
            this.presenter = presenter;
        }

        public override void Break()
        {
            presenter.ShowText("BREAK!");
            Countdown();
        }

        private void Countdown()
        {
            Timer timer = new Timer(5, tickHandler);
            timer.DoWork();
        }

        private void tickHandler(int now)
        {
            presenter.ShowTime(now);
        }

        public override void GoFor(string move)
        {
            presenter.ShowText(move);
            Countdown();
        }

        public override void SwitchTo()
        {
            presenter.ShowText("SWITCH");
            Countdown();
        }
    }

}
