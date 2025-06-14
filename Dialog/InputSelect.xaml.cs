using System.Collections.Generic;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// InputSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class InputSelect : Window
    {
        public string mText = "";
        public List<string> mTextList = new List<string>();
        public int mListIndex = 0;


        public InputSelect()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbText.Text = mText;
            cbText.ItemsSource = mTextList;
            if (0 < mListIndex && mListIndex <  mTextList.Count)
                cbText.SelectedIndex = mListIndex;
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
