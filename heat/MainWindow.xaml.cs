using System.Windows;
using System.Windows.Controls;

namespace Heat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Engine engine;

        public MainWindow(Engine engine)
        {
            this.engine = engine;
            InitializeComponent();
            frame.Navigate(new HomeView(this));
        }
       
        public Engine Engine()
        {
            return engine;
        }

        public void NavigatePage(Page page)
        {
            frame.Navigate(page);
        }
        

    }
}
