using System;
using System.Windows;
using System.Speech.Synthesis;
using System.IO;

namespace Heat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, UserInterface
    {
        private readonly Engine engine;
        private readonly SpeechSynthesizer synthesizer;


        public MainWindow(Engine engine)
        {
            this.engine = engine;
            this.synthesizer = new SpeechSynthesizer();

            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            engine.OnGo(this);  
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

        private void LoadWorkout_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) { 
                Circuit circuit = Circuit.fromYAML(new StreamReader(fileDialog.FileName));
                engine.LoadCircuit(circuit);
            }
        }
    }
}
