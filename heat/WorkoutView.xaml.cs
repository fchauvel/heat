using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Heat
{
    /// <summary>
    /// Interaction logic for WorkoutView.xaml
    /// </summary>
    public partial class WorkoutView : Page, UserInterface
    {
        private readonly MainWindow mainWindow; 
        private readonly SpeechSynthesizer synthesizer;

        public WorkoutView(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.synthesizer = new SpeechSynthesizer();
            InitializeComponent();  
        }


        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Engine().OnGo(this);
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

      
    }
}
