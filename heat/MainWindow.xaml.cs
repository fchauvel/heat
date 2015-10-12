using System;
using System.IO;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;

namespace Heat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, UserInterface
    {
        private const int DEFAULT_DURATION = 30;
        private const int DURATION_STEP = 5;
        private const int DEFAULT_EFFORT = 85;
        private const int EFFORT_STEP = 5;

        private readonly Engine engine;

        private readonly SpeechSynthesizer synthesizer;

        private int durationValue;
        private decimal effortValue;

        public MainWindow(Engine engine)
        {
            this.engine = engine;
            this.synthesizer = new SpeechSynthesizer();
            InitializeComponent();
            durationValue = DEFAULT_DURATION;
            effortValue = DEFAULT_EFFORT;
            update();
        }

        public void ShowAction(String text)
        {
            this.action.Dispatcher.BeginInvoke(
               (Action)(() =>
               {
                   this.action.Text = text;
                   PromptBuilder pb = new PromptBuilder();
                   pb.AppendText("Now, " + text + "!");
                   synthesizer.Speak(pb);
               }
            ));
        }

        public void ShowTime(int time)
        {
            string text = String.Format("{0} s.", time);
            this.clock.Dispatcher.BeginInvoke((Action)(() => this.clock.Text = text));
        }

        private void Longer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            durationValue += DURATION_STEP;
            update();
        }

        private void update()
        {
            var newDuration = string.Format("{0} min.", durationValue);
            duration.Text = newDuration; 
            var newEffort = string.Format("{0} %", effortValue);
            effort.Text = newEffort;
        }

        private void Shorter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            durationValue -= DURATION_STEP;
            update();
        }

        private void Harder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            effortValue += EFFORT_STEP;
            update();
        }

        private void Easier_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            effortValue -= EFFORT_STEP;
            update();
        }

        private void ChangeWorkout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Circuit circuit = Circuit.fromYAML(new StreamReader(fileDialog.FileName));
                engine.LoadCircuit(circuit);
            }
        }

        private void Go_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            engine.OnGo(this);
        }

    }
}
