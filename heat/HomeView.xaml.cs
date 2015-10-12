using System.IO;
using System.Windows.Controls;

namespace Heat
{
    /// <summary>
    /// Interaction logic for Page1.xaml 
    /// </summary>
    public partial class HomeView : Page
    {
        private const int DEFAULT_DURATION = 30;
        private const int DURATION_STEP = 5;
        private const int DEFAULT_EFFORT = 85;
        private const int EFFORT_STEP = 5;

        private readonly MainWindow window;
        private int duration;
        private decimal effort;

        public HomeView(MainWindow mainWindow)
        {
            window = mainWindow;
            InitializeComponent();
            duration = DEFAULT_DURATION;
            effort = DEFAULT_EFFORT;
            update();
        }

        private void Longer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            duration += DURATION_STEP;
            update();
        }

        private void update() {
            var newDuration = string.Format("{0} min.", duration);
            durationText.Text = newDuration;
            var newEffort = string.Format("{0} %", effort);
            effortText.Text = newEffort;
        }

        private void Shorter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            duration -= DURATION_STEP;
            update();
        }

        private void Harder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            effort += EFFORT_STEP;
            update();
        }

        private void Easier_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            effort -= EFFORT_STEP;
            update();
        }

        private void ChangeWorkout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Circuit circuit = Circuit.fromYAML(new StreamReader(fileDialog.FileName));
                window.Engine().LoadCircuit(circuit);
            }
        }

        private void Go_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            window.frame.Navigate(new WorkoutView(window));
        }
    }
}
