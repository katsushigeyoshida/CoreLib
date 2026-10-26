using System.Collections.Generic;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// InputSelect.xaml の相互作用ロジック
    /// コンボボックスで文字列リストから項目を選択または入力
    /// 種別でファイルやフォルダを選択したときは項目のダブルクリックで
    /// ファイルまたはフォルダ選択ダイヤログを表示する
    /// 付加機能
    ///     チェックボックス
    /// </summary>
    public partial class InputSelect : Window
    {
        public double mWindowWidth;                         //  ウィンドウの高さ
        public double mWindowHeight;                        //  ウィンドウ幅

        //  選択の種別
        public enum InputType { TEXT, OPENFILEPATH, SAVEFILEPATH, FOLDER }
        public InputType mInputType = InputType.TEXT;
        //  ファイル/フォルダ選択時の初期ディレクトリ
        public string mInitDirectory = ".";
        //  ファイル選択時の選択ファイル種別
        public List<string[]> mFilters = new List<string[]>() {
            new string[]{ "CSVファイル", "*.csv;*.csv" },
            new string[]{ "すべてのファイル", "*.*" }
        };
        //  入力文字の初期値
        public string mText = "";
        //  選択文字列リスト
        public List<string> mTextList = new List<string>();
        //  選択項目
        public int mListIndex = 0;
        public string mCheckBoxTitle = "";
        public bool mCheckBocChecked = false;

        private YLib ylib = new YLib();
        private YFile yfile = new YFile();

        public InputSelect()
        {
            InitializeComponent();

            WindowFormLoad();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbText.Text = mText;
            cbText.ItemsSource = mTextList;
            if (0 < mListIndex && mListIndex <  mTextList.Count)
                cbText.SelectedIndex = mListIndex;
            if (mInputType == InputType.OPENFILEPATH)
                cbText.ToolTip = "ファイル名の入力、ダブルクリックで選択ダイヤログを表示する";
            else if (mInputType == InputType.SAVEFILEPATH)
                cbText.ToolTip = "ファイル名の入力、ダブルクリックで選択ダイヤログを表示する";
            else if (mInputType == InputType.FOLDER)
                cbText.ToolTip = "フォルダ名の入力、ダブルクリックで選択ダイヤログを表示する";
            else
                cbText.ToolTip = "ファイル名の入力";
            if (mCheckBoxTitle == "")
                cbCheck.Visibility = Visibility.Collapsed;
            else
                cbCheck.Content = mCheckBoxTitle;
            cbCheck.IsChecked = mCheckBocChecked;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.InputSelectWidth < 100 ||
                Properties.Settings.Default.InputSelectHeight < 50 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.InputSelectHeight) {
                Properties.Settings.Default.InputSelectWidth = Width;
                Properties.Settings.Default.InputSelectHeight = Height;
            } else {
                Top = Properties.Settings.Default.InputSelectTop;
                Left = Properties.Settings.Default.InputSelectLeft;
                Width = Properties.Settings.Default.InputSelectWidth;
                Height = Properties.Settings.Default.InputSelectHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.InputSelectTop = Top;
            Properties.Settings.Default.InputSelectLeft = Left;
            Properties.Settings.Default.InputSelectWidth = Width;
            Properties.Settings.Default.InputSelectHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// OKボタン(リストに登録)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            mText = cbText.Text;
            if (mTextList.Contains(mText))
                mTextList.Remove(mText);
            mTextList.Insert(0, mText);
            mCheckBocChecked = cbCheck.IsChecked == true;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// テキストボックスダブルクリックでファイルまたはフォルダ選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbText_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string path = "";
            if (mInputType == InputType.OPENFILEPATH) {
                path = yfile.fileOpenSelectDlg(Title, mInitDirectory, mFilters);
            } else if (mInputType == InputType.SAVEFILEPATH) {
                path = yfile.fileSaveSelectDlg(Title, mInitDirectory, mFilters);
            } else if (mInputType == InputType.FOLDER) {
                path = yfile.folderSelect(Title, mInitDirectory);
            }
            if (0 < path.Length)
                cbText.Text = path;
        }
    }
}
