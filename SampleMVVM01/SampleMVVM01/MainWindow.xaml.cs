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

namespace SampleMVVM01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //MVVM
            //step-01: first design the View : Design of App, no C# code, only XAML code
            //step-02: second design the Model : Connector using Binding & Commands
            //step-03: third design the ViewModel : Data, C# code, File, etc, Business login


        }
    }
}