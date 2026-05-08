using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// 入力項目数可変のダイヤログ
    /// 
    /// InputBoxMulti.xaml の相互作用ロジック
    /// </summary>
    public partial class InputBoxMulti : Window
    {
        private double mWindowWidth;
        private double mWindowHeight;
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        private double mWithoutStacPanelHeight = 0;                     //  スタックパネル以外の高さ
        private double mStackPanelHeight = 0;                           //  スタックパネルの高さ
        private List<StackPanel> mStackPanel = new List<StackPanel>();  //  スタックパネルの高さ取得用
        private List<TextBox> mTextBoxList = new List<TextBox>();       //  入力データ取り出し用

        public List<string[]> mDataList = new List<string[]>();         //  入力項目データ(タイトルと初期データ)

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InputBoxMulti()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初期値設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //  前回のWindowの位置を復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            Top = Properties.Settings.Default.InputBoxMultiTop;
            Left = Properties.Settings.Default.InputBoxMultiLeft;
            //  項目の設定
            setForm();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //  Windowの位置を保存
            Properties.Settings.Default.InputBoxMultiTop = Top;
            Properties.Settings.Default.InputBoxMultiLeft = Left;
            Properties.Settings.Default.Save();

        }

        /// <summary>
        /// レイアウト変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_LayoutUpdated(object sender, System.EventArgs e)
        {
            //  StackPanel(引数入力)の初期の高さを保存
            if (mStackPanelHeight == 0 && 0 < stackPanel.ActualHeight) {
                mStackPanelHeight = stackPanel.ActualHeight;
                mWithoutStacPanelHeight = Height - mStackPanelHeight;
            }
            //  引数入力が追加された時
            if (0 < mStackPanelHeight && 0 < mStackPanel.Count) {
                mStackPanelHeight = mStackPanel[0].ActualHeight * mStackPanel.Count;
                Height = mWithoutStacPanelHeight + mStackPanelHeight;
            }

        }

        /// <summary>
        /// 入力項目の設定
        /// </summary>
        private void setForm()
        {
            stackPanel.Children.Clear();
            foreach (var str in mDataList) {
                Label label = new Label();
                label.Content = str[0];
                label.Margin = new Thickness(10, 0, 0, 0);
                label.Width = 150;
                TextBox textBox = new TextBox();
                textBox.Text = str[1];
                textBox.Width = 100;
                StackPanel childStackPanel = new StackPanel();
                childStackPanel.Orientation = Orientation.Horizontal;
                childStackPanel.Children.Add(label);
                childStackPanel.Children.Add(textBox);
                mTextBoxList.Add(textBox);
                mStackPanel.Add(childStackPanel);
                int n = stackPanel.Children.Count;
                stackPanel.Children.Insert(n, childStackPanel);
            }
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < mTextBoxList.Count; i++)
                mDataList[i][1] = mTextBoxList[i].Text;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancelボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
