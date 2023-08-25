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

namespace CoreLib
{
    /// <summary>
    /// InputSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class InputSelect : Window
    {
        public string mTitle = "";
        public string mText = "";
        public List<string> mTextList = new List<string>();

        public InputSelect()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = mTitle;
            cbText.Text = mText;
            cbText.ItemsSource = mTextList;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            mText = cbText.Text;
            DialogResult = true;
            Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
