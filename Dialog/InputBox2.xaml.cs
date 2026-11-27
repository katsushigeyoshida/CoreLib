using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace CoreLib
{
    /// <summary>
    /// InputBox2.xaml の相互作用ロジック
    /// 
    /// 2行文字列編集ダイヤログ
    /// mEditText1  1行目入力文字列
    /// mEditText2  2行目入力文字列
    /// mTitle1     1行目タイトル
    /// mTitle2     2行目タイトル
    /// mMultiLine  複数行入力可否
    /// mEditText2Enabled   2行目編集可否
    /// </summary>
    public partial class InputBox2 : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅

        public string mEditText1 = "";
        public string mEditText2 = "";
        public string mTitle1 = "";                             //  第１カラムタイトル
        public string mTitle2 = "";                             //  第２カラムタイトル
        public bool mMultiLine = false;                         //  第２カラム複数行入力可否
        public bool mEditText1Enabled = true;                   //  第1カラム変更可否
        public bool mEditText2Enabled = true;                   //  第2カラム変更可否

        public enum ColumnType { TEXT, FILE, FOLDER }           //  カラムの種別
        public ColumnType mColumnType1 = ColumnType.TEXT;
        public ColumnType mColumnType2 = ColumnType.TEXT;
        public List<string[]> mFilter1 = new List<string[]>() {
                    new string[] { "プログラムファイル", "*.exe"},
                    new string[] { "すべてのファイル", "*.*"}
                };
        public List<string[]> mFilter2 = new List<string[]>() {
                    new string[] { "プログラムファイル", "*.exe"},
                    new string[] { "すべてのファイル", "*.*"}
                };


        private YFile yfile = new YFile();

        public InputBox2()
        {
            InitializeComponent();

            mWindowWidth = Width;
            mWindowHeight = Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LbTitle1.Content = mTitle1;
            LbTitle2.Content = mTitle2;
            TbTextBox1.Text = mEditText1;
            TbTextBox2.Text = mEditText2;
            TbTextBox1.IsEnabled = mEditText1Enabled;
            TbTextBox2.IsEnabled = mEditText2Enabled;

            if (mMultiLine) {
                //  複数行入力設定
                TbTextBox2.AcceptsReturn = true;
                TbTextBox2.TextWrapping = TextWrapping.Wrap;
                TbTextBox2.VerticalContentAlignment = VerticalAlignment.Top;
                WindowFormLoad();
            }
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
            if (Properties.Settings.Default.InputBox2Width < 100 ||
                Properties.Settings.Default.InputBox2Height < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.InputBox2Height) {
                Properties.Settings.Default.InputBox2Width = mWindowWidth;
                Properties.Settings.Default.InputBox2Height = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.InputBox2Top;
                Left = Properties.Settings.Default.InputBox2Left;
                Width = Properties.Settings.Default.InputBox2Width;
                Height = Properties.Settings.Default.InputBox2Height;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.InputBox2Top = Top;
            Properties.Settings.Default.InputBox2Left = Left;
            Properties.Settings.Default.InputBox2Width = Width;
            Properties.Settings.Default.InputBox2Height = Height;
            Properties.Settings.Default.Save();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            mEditText1 = TbTextBox1.Text;
            mEditText2 = TbTextBox2.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TbTextBox1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mColumnType1 == ColumnType.FILE) {
                string file = yfile.fileOpenSelectDlg("ファイル選択", "", mFilter1);
                if (0 < file.Length)
                    TbTextBox1.Text = file;
            } else if (mColumnType1 == ColumnType.FOLDER) {
                string folder = yfile.folderSelect("フォルダの選択", "");
                if (0 < folder.Length)
                    TbTextBox1.Text = folder;
            }

        }

        private void TbTextBox2_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mColumnType2 == ColumnType.FILE) {
                string file = yfile.fileOpenSelectDlg("ファイル選択", "", mFilter2);
                if (0 < file.Length)
                    TbTextBox2.Text = file;
            } else if (mColumnType2 == ColumnType.FOLDER) {
                string folder = yfile.folderSelect("フォルダの選択", "");
                if (0 < folder.Length)
                    TbTextBox2.Text = folder;
            }
        }
    }
}
