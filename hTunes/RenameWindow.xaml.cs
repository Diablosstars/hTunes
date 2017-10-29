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
using System.Windows.Shapes;

namespace hTunes
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        public string PlaylistName;
        public RenameWindow(string originalName)
        {
            PlaylistName = originalName;
            InitializeComponent();
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            PlaylistName = textBox1.Text;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
