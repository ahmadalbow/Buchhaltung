using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Buchhaltung
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Rect workArea = SystemParameters.WorkArea;
            Width = Math.Min(Width, Math.Max(0, workArea.Width - 20));
            Height = Math.Min(Height, Math.Max(0, workArea.Height - 20));
        }
        public void Drag(object sender,MouseButtonEventArgs e)
        {
           if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
