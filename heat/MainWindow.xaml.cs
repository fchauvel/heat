using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly App controller;

        public MainWindow(App controller)
        {
            this.controller = controller;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            controller.OnGo();  
        }

        public void ShowText(String text)
        {
            this.text.Text = text;
        }
    }
}
