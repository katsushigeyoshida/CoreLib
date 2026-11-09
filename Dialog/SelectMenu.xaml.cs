using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// SelectMenu.xaml の相互作用ロジック
    /// 
    /// メニュー選択ダイヤログ
    /// ComboBoxによるメニュー表示
    /// 使用例
    ///     SelectMenu popupMenu = new SelectMenu();
    ///     popupMenu.Title = "PopUpMenuのテスト";
    ///     string[] testMenu = { "djjk", "hadjlkjlkj", "lkjdlkjas" };
    ///     popupMenu.mMenuList = testMenu;
    ///     if (popupMenu.ShowDialog() == true) {
    ///         LbResult.Content = popupMenu.mSelectItem;
    ///     } else {
    ///         LbResult.Content = "";
    ///     }
    /// </summary>
    public partial class SelectMenu : Window
    {
        public string[] mMenuList = { "aaa", "bbb", "CCC" };
        public string mSelectItem = "";
        public int mSelectIndex = 0;
        public bool mReadOnly = true;

        public SelectMenu()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 表示前処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbMenu.ItemsSource = mMenuList;
            CbMenu.SelectedIndex = mSelectIndex < mMenuList.Length ? mSelectIndex : 0;
            if (!mReadOnly)
                CbMenu.IsEditable = true;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Window_Closing");
        }

        /// <summary>
        /// メニュー選択処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex && CbMenu.SelectedIndex != mSelectIndex) {
                mSelectIndex = CbMenu.SelectedIndex;
                mSelectItem = CbMenu.Items[CbMenu.SelectedIndex].ToString();
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// マウスダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbMenu_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex) {
                mSelectIndex = CbMenu.SelectedIndex;
                mSelectItem = CbMenu.Items[CbMenu.SelectedIndex].ToString();
                DialogResult = true;
                Close();
            } else if (!mReadOnly) {
                mSelectItem = CbMenu.Text;
                DialogResult = true;
                Close();
            }
        }

        /// <summary>
        /// キー入力処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!mReadOnly && e.Key == System.Windows.Input.Key.Enter) {
                mSelectItem = CbMenu.Text;
                DialogResult = true;
                System.Diagnostics.Debug.WriteLine($"Window_PreviewKeyDown {mSelectItem}");
                Close();
            }
        }
    }
}
