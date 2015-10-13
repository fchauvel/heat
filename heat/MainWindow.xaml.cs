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
    public partial class MainWindow : Window, Listener
    {
        private readonly Engine engine;
        private readonly SpeechSynthesizer synthesizer;

        public MainWindow(Engine engine)
        {
            this.engine = engine;
            this.synthesizer = new SpeechSynthesizer();
            InitializeComponent();
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

        public void DurationChangedTo(int durationInMinutes)
        {
            duration.Dispatcher.BeginInvoke((Action)(() => {
                var newDuration = string.Format("{0} min.", durationInMinutes);
                duration.Text = newDuration;
            }));
        }

        public void EffortChangedTo(int newEffortValue)
        {
            effort.Dispatcher.BeginInvoke((Action)(() => {
                var newEffortText = string.Format("{0} %", newEffortValue);
                effort.Text = newEffortText;
            }));
        }

        private void Longer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            engine.Extend();
        }

        private void Shorter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            engine.Shorten();
        }

        private void Harder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            engine.AugmentEffort();
        }

        private void Easier_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            engine.ReduceEffort();
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
            engine.OnGo();
        }

    }
}
