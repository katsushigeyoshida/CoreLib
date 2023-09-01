using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// SelectMenu2.xaml の相互作用ロジック
    /// 
    /// コンボボックスによる分類選択とリストボックスによる項目選択
    /// 分類の変更で項目リストも書き換える
    /// </summary>
    public partial class SelectMenu2 : Window
    {
        public double mWindowWidth;                         //  ウィンドウの高さ
        public double mWindowHeight;                        //  ウィンドウ幅
        public bool mWindowSizeOutSet = false;              //  ウィンドウサイズの外部設定

        public List<string> mGenreList;                     //  分類リスト
        public List<List<string>> mItemList;                //  分類ごとのアイテムリスト
        public string mGenre;                               //  分類名
        public string mItem;                                //  アイテム名

        public SelectMenu2()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbGenre.ItemsSource = mGenreList;
            if (mGenreList.Contains(mGenre))
                cbGenre.SelectedIndex = mGenreList.IndexOf(mGenre);
            else
                cbGenre.SelectedIndex = 0;
            lbItem.ItemsSource = mItemList[0];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// 分類の選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= cbGenre.SelectedIndex) {
                lbItem.ItemsSource = mItemList[cbGenre.SelectedIndex];
            }
        }

        /// <summary>
        /// アイテムのダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mGenre = cbGenre.Text;
            mItem = lbItem.SelectedItem.ToString();

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// [OK]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            mGenre = cbGenre.Text;
            mItem = lbItem.SelectedItem.ToString();

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// [キャンセル]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
