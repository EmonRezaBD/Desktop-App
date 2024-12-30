using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

           /* tbHello.Text = "Hello World 2";//We set the initial text then changing it in runtime from code
            btnRun.Content = "Stop";*/

        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //tbHello.Text = "Hello FSM";
        }
    }
}