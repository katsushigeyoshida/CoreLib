using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// InputSelect2.xaml の相互作用ロジック
    /// </summary>
    public partial class InputSelect2 : Window
    {
        public List<string> mSelectList;
        public string mSelectText = "";
        public string mEditText = "";
        public string mTitle1 = "";
        public string mTitle2 = "";

        private TextD mTextD = new TextD();
        private YLib YLib = new YLib();
        public InputSelect2()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbTitle1.Content = mTitle1;
            lbTitle2.Content = mTitle2;
            cbSelectList.ItemsSource = mSelectList;
            tbEditText.Text = mEditText;
            double t1Width = mTextD.measureText(mTitle1, lbTitle1.FontSize).Width;
            double t2Width = mTextD.measureText(mTitle2, lbTitle1.FontSize).Width;
            double tWidth = Math.Max(t1Width, t2Width);
            cbSelectList.Margin = new Thickness(tWidth + 20, 5, 10, 0);
            tbEditText.Margin = new Thickness(tWidth + 20, 35, 10, 0);
            cbSelectList.SelectedIndex = 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void cbSelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= cbSelectList.SelectedIndex) {
                tbEditText.Text = cbSelectList.SelectedItem.ToString();
            }
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            mEditText = tbEditText.Text;
            mSelectText = cbSelectList.Text;
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
