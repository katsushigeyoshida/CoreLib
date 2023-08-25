using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// リストボックスにチェックマークのデータを反映させるためのクラス
    /// </summary>
    public class CheckBoxListItem
    {
        public bool Checked { get; set; }
        public string Text { get; set; }
        public CheckBoxListItem(bool ch, string text)
        {
            Checked = ch;
            Text = text;
        }
    }

    /// <summary>
    /// ChkListDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ChkListDialog : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private double mPrevWindowHeight;                       //  変更前のウィンドウ高さ
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        public string mTitle = "";
        public List<CheckBoxListItem> mChkList = new List<CheckBoxListItem>();

        public bool mAddMenuEnable = false;
        public bool mEditMenuEnable = false;
        public bool mDeleteMenuEnable = false;
        public bool mMoveMenuEnable = false;

        public Window mMainWindow;

        private YLib ylib = new YLib();

        public ChkListDialog()
        {
            InitializeComponent();

            mWindowWidth = Width;
            mWindowHeight = Height;
            WindowFormLoad();       //  Windowの位置とサイズを復元
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listDataSet();
            lbChkListMenuAdd.Visibility = mAddMenuEnable ? Visibility.Visible : Visibility.Collapsed;
            lbChkListMenuEdit.Visibility = mEditMenuEnable ? Visibility.Visible : Visibility.Collapsed;
            lbChkListMenuDelete.Visibility = mDeleteMenuEnable ? Visibility.Visible : Visibility.Collapsed;
            lbChkListMenuMove.Visibility = mMoveMenuEnable ? Visibility.Visible : Visibility.Collapsed;
            if (0 < mTitle.Length)
                Title = mTitle;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();       //  ウィンドの位置と大きさを保存
        }


        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.ChkListWindowWidth < 100 ||
                Properties.Settings.Default.ChkListWindowHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.ChkListWindowHeight) {
                Properties.Settings.Default.ChkListWindowWidth = mWindowWidth;
                Properties.Settings.Default.ChkListWindowHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.ChkListWindowTop;
                Left = Properties.Settings.Default.ChkListWindowLeft;
                Width = Properties.Settings.Default.ChkListWindowWidth;
                Height = Properties.Settings.Default.ChkListWindowHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.ChkListWindowTop = Top;
            Properties.Settings.Default.ChkListWindowLeft = Left;
            Properties.Settings.Default.ChkListWindowWidth = Width;
            Properties.Settings.Default.ChkListWindowHeight = Height;
            Properties.Settings.Default.Save();
        }

        private void cbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lbChkListMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            int index = lbChkList.SelectedIndex;
            if (menuItem.Name.CompareTo("lbChkListMenuAdd") == 0) {
                //  項目の追加
                InputBox dlg = new InputBox();
                dlg.Owner = mMainWindow;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.mEditText = "";
                if (dlg.ShowDialog() == true) {
                    if (0 < dlg.mEditText.Length) {
                        if (0 <= mChkList.FindIndex(p => p.Text == dlg.mEditText.ToString())) {
                            mChkList.Add(new CheckBoxListItem(false, dlg.mEditText.ToString()));
                        }
                    }
                }
            } else if (0 <= index && menuItem.Name.CompareTo("lbChkListMenuEdit") == 0) {
                //  編集
            } else if (0 <= index && menuItem.Name.CompareTo("lbChkListMenuDelete") == 0) {
                //  リストボックスから項目削除
                CheckBoxListItem item = (CheckBoxListItem)lbChkList.Items[index];
            } else if (0 <= index && menuItem.Name.CompareTo("lbChkListMenuMove") == 0) {
                //  移動
                CheckBoxListItem item = (CheckBoxListItem)lbChkList.Items[index];
            } else if (menuItem.Name.CompareTo("lbChkListMenuAllCheck") == 0) {
                //  すべてにチェックを入れる
                visibleDataAllSet(true);
                listDataSet();
            } else if (menuItem.Name.CompareTo("lbChkListMenuAllUnCheck") == 0) {
                //  すべてのチェックを外す
                visibleDataAllSet(false);
                listDataSet();
            } else {
                return;
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 表示しているリストにすべてのチェックの入り切り
        /// </summary>
        /// <param name="check"></param>
        private void visibleDataAllSet(bool check)
        {
            foreach (CheckBoxListItem item in mChkList) {
                item.Checked = check;
            }
        }

        /// <summary>
        /// リストボックスにデータを設定する
        /// </summary>
        private void listDataSet()
        {
            lbChkList.Items.Clear();
            foreach (var item in mChkList) {
                lbChkList.Items.Add(item);
            }
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
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
