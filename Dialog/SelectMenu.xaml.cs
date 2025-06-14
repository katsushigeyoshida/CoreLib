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

        public SelectMenu()
        {
            InitializeComponent();
        }

        private void CbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex && CbMenu.SelectedIndex != mSelectIndex) {
                mSelectIndex = CbMenu.SelectedIndex;
                mSelectItem = CbMenu.Items[CbMenu.SelectedIndex].ToString();
                DialogResult = true;
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbMenu.ItemsSource = mMenuList;
            CbMenu.SelectedIndex = mSelectIndex < mMenuList.Length ? mSelectIndex : 0;
        }

        private void CbMenu_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (0 <= CbMenu.SelectedIndex) {
                mSelectIndex = CbMenu.SelectedIndex;
                mSelectItem = CbMenu.Items[CbMenu.SelectedIndex].ToString();
                DialogResult = true;
                Close();
            }
        }
    }
}
