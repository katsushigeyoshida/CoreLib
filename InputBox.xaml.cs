using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CoreLib
{
    /// <summary>
    /// InputBox.xaml の相互作用ロジック
    /// 
    /// 文字列編集ダイヤログ
    /// ・入力文字列   mEditText
    /// ・複数行対応   mMultiLine
    /// ・表示専用設定 mReadOnly
    /// ・文字サイズ   mFontSize
    /// ・文字ズームボタン mFontZoomButtonVisible
    /// </summary>
    public partial class InputBox : Window
    {
        public double mWindowWidth;                         //  ウィンドウの高さ
        public double mWindowHeight;                        //  ウィンドウ幅
        public bool mWindowSizeOutSet = false;              //  ウィンドウサイズの外部設定

        public string mEditText;
        public bool mMultiLine = false;                     //  複数行入力可否
        public bool mReadOnly = false;                      //  リードオンリー,OKボタン非表示
        public int mFontSize = 12;                          //  文字サイズ
        public bool mFontZoomButtomVisible = true;          //  文字ズームボタン表示/非表示
        public bool mCalcButtonVisible = true;              //  計算ボタン表示/非表示

        private YLib ylib = new YLib();

        public InputBox()
        {
            InitializeComponent();

            mWindowWidth = Width;
            mWindowHeight = Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  編集文字列
            if (mEditText == null)
                mEditText = "";
            EditText.Text = mEditText;

            //  複数行入力設定
            if (mMultiLine) {
                EditText.AcceptsReturn = true;
                EditText.TextWrapping = TextWrapping.Wrap;
                EditText.VerticalContentAlignment = VerticalAlignment.Top;
                if (!mWindowSizeOutSet)
                    WindowFormLoad();
                else {
                    Width = mWindowWidth;
                    int rowCount = mEditText.Count(p => p == '\n') + 1;
                    Height = rowCount * mFontSize * 1.2 + 100;
                    Height = Math.Max(Height, mWindowHeight);
                }
                OK.IsDefault = false;
            }
            //  表示専用(編集不可,OKボタン非表示)設定
            EditText.IsReadOnly = mReadOnly;
            OK.Visibility = mReadOnly ? Visibility.Hidden : Visibility.Visible;
            //  文字サイズ
            EditText.FontSize = mFontSize;
            //  文字サイズズームボタンの表示/非表示
            if (mFontZoomButtomVisible) {
                BtGZoomDown.Visibility = Visibility.Visible;
                BtGZoomUp.Visibility = Visibility.Visible;
            } else {
                BtGZoomDown.Visibility = Visibility.Hidden;
                BtGZoomUp.Visibility = Visibility.Hidden;
            }
            if (mCalcButtonVisible)
                BtCalc.Visibility = Visibility.Visible;
            else
                BtCalc.Visibility = Visibility.Hidden;

            EditText.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!mWindowSizeOutSet)
                WindowFormSave();
        }

        /// <summary>
        /// [OK] ＯＫボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            mEditText = EditText.Text;
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// [Cancel] キャンセルボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// [+][-] 文字サイズズームボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)e.Source;
            if (bt.Name.CompareTo("BtGZoomDown") == 0) {
                mFontSize--;
            } else if (bt.Name.CompareTo("BtGZoomUp") == 0) {
                mFontSize++;
            } else if (bt.Name.CompareTo("BtCalc") == 0) {
                textCalculate();
            }
            EditText.FontSize = mFontSize;
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.InputBoxWindowWidth < 100 ||
                Properties.Settings.Default.InputBoxWindowHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.InputBoxWindowHeight) {
                Properties.Settings.Default.InputBoxWindowWidth = mWindowWidth;
                Properties.Settings.Default.InputBoxWindowHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.InputBoxWindowTop;
                Left = Properties.Settings.Default.InputBoxWindowLeft;
                Width = Properties.Settings.Default.InputBoxWindowWidth;
                Height = Properties.Settings.Default.InputBoxWindowHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.InputBoxWindowTop = Top;
            Properties.Settings.Default.InputBoxWindowLeft = Left;
            Properties.Settings.Default.InputBoxWindowWidth = Width;
            Properties.Settings.Default.InputBoxWindowHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// キー入力操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            keyCommand(e.Key, e.KeyboardDevice.Modifiers == ModifierKeys.Control, e.KeyboardDevice.Modifiers == ModifierKeys.Shift);
        }

        /// <summary>
        /// マウスダブルクリック操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            textOpen(lineSelect());
        }

        /// <summary>
        /// 選択文字列を計算する
        /// </summary>
        private void textCalculate()
        {
            YCalc calc = new YCalc();
            string text = EditText.SelectedText;
            //  数式文字以外を除く
            string express = ylib.stripControlCode(text);
            express = calc.stripExpressData(express);
            //  計算結果を挿入
            int pos = EditText.SelectionStart + EditText.SelectionLength;
            EditText.Select(pos, 0);
            EditText.SelectedText = " = " + calc.expression(express).ToString();
        }

        /// <summary>
        /// 選択文字列を開く
        /// </summary>
        private void textOpen(string word = "")
        {
            //  選択文字列
            if (word.Length <= 0)
                word = EditText.SelectedText;
            if (0 < word.Length && 0 <= word.IndexOf("http")) {
                int ps = word.IndexOf("http");
                if (0 < ps) {
                    int pe = word.IndexOfAny(new char[] { ' ', '\t', '\n' }, ps);
                    if (0 < pe) {
                        word = word.Substring(ps, pe - ps);
                    } else {
                        word = word.Substring(ps);
                    }
                }
                ylib.openUrl(word);
            } else if (0 < word.Length && File.Exists(word)) {
                ylib.openUrl(word);
            }
        }

        /// <summary>
        /// 一行分を抽出
        /// </summary>
        /// <returns></returns>
        private string lineSelect()
        {
            int pos = EditText.SelectionStart;
            pos = pos >= EditText.Text.Length ? 0 : pos;
            int sp = EditText.Text.LastIndexOf("\n", pos);
            int ep = EditText.Text.IndexOf("\n", pos);
            if (0 <= sp && 0 <= ep && sp == ep) {
                pos++;
                ep = EditText.Text.IndexOf("\n", pos);
            }
            ep = ep < 0 ? EditText.Text.Length : ep;
            return EditText.Text.Substring(sp + 1, ep - sp - 1);
        }

        /// <summary>
        /// キー入力処理
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <param name="control">Ctrlキーの有無</param>
        /// <param name="shift">Shiftキーの有無</param>
        private void keyCommand(Key key, bool control, bool shift)
        {
            if (control) {
                switch (key) {
                    default:
                        break;
                }
            } else if (shift) {
                switch (key) {
                    default: break;
                }
            } else {
                switch (key) {
                    case Key.Escape: break;                 //  ESCキーでキャンセル
                    case Key.F11: textCalculate(); break;   //  選択テキストを計算
                    case Key.F12: textOpen(); break;        //  選択テキストで開く
                    case Key.Back:                          //  ロケイト点を一つ戻す
                        break;
                    default: break;
                }
            }
        }
    }
}
