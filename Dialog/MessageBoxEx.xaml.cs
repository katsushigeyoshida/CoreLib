using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// MessageBoxの拡張版
    /// 使い方
    ///     MessageBoxEx dlg = new MessageBoxEx();
    ///     dlg.Owner = this;                                               //  親Window
    ///     dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;  //  センター表示
    ///     dlg.mButton = MessageBoxButton.OKCancel;                        //  ボタンタイプ
    ///     dlg.mTitle = "タイトル";
    ///     dlg.mMessage = "メッセージ\n2行目\n3行目\n4行目\n5行目";
    ///     dlg.ShowDialog();
    ///     MessageBox.Show(dlg.mResult.ToString());                        //  ボタン結果
    /// </summary>
    public partial class MessageBoxEx : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅

        public string mDlgTitle = "";
        public string mTitle = "";
        public string mMessage = "";
        public MessageBoxButton mButton = MessageBoxButton.OK;
        public MessageBoxResult mResult = MessageBoxResult.OK;
        public bool mContinueMessageDisp = false;                   //  継続表示チェックボタン表示
        public bool mContinueMessage = true;                        //  継続表示の有無

        public MessageBoxEx()
        {
            InitializeComponent();

            mWindowWidth = Width;
            mWindowHeight = Height;
        }

        /// <summary>
        /// 前処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double minWidth = 120;
            Title = mDlgTitle;
            if (mTitle.Length == 0)
                tbTitle.Visibility = Visibility.Collapsed;
            tbTitle.Text = mTitle;
            tbMessage.Text = mMessage;

            switch (mButton) {
                case MessageBoxButton.OK:
                    btCanecel.Visibility = Visibility.Collapsed;
                    btYes.Visibility = Visibility.Collapsed;
                    btNo.Visibility = Visibility.Collapsed;
                    btYNCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    btYes.Visibility = Visibility.Collapsed;
                    btNo.Visibility = Visibility.Collapsed;
                    btYNCancel.Visibility = Visibility.Collapsed;
                    minWidth = 180;
                    break;
                case MessageBoxButton.YesNo:
                    btOK.Visibility = Visibility.Collapsed;
                    btCanecel.Visibility = Visibility.Collapsed;
                    btYNCancel.Visibility = Visibility.Collapsed;
                    minWidth = 170;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btOK.Visibility = Visibility.Collapsed;
                    btCanecel.Visibility = Visibility.Collapsed;
                    minWidth = 250;
                    break;
            }
            Size textSize = measureText(mMessage, tbMessage.FontSize);
            int rowCount = mMessage.Count(p => p == '\n');
            tbMessage.Height = 30 + rowCount * tbMessage.FontSize * 1.2 + 10;
            Height = tbMessage.Height + 80 + (0 < mTitle.Length ? 20 : 0);
            Width = Math.Max(textSize.Width + 50, minWidth);

            if (mContinueMessageDisp) {
                cbContinueMessage.Visibility = Visibility.Visible;
                Height += 20;
            } else
                cbContinueMessage.Visibility = Visibility.Collapsed;
            cbContinueMessage.IsChecked = mContinueMessage;
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            mResult = MessageBoxResult.OK;
            mContinueMessage = cbContinueMessage.IsChecked == true;
            Close();
        }

        /// <summary>
        /// Cancelボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCanecel_Click(object sender, RoutedEventArgs e)
        {
            mResult = MessageBoxResult.Cancel;
            mContinueMessage = cbContinueMessage.IsChecked == true;
            Close();
        }

        /// <summary>
        /// Yesボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btYes_Click(object sender, RoutedEventArgs e)
        {
            mResult = MessageBoxResult.Yes;
            mContinueMessage = cbContinueMessage.IsChecked == true;
            Close();
        }

        /// <summary>
        /// Noボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNo_Click(object sender, RoutedEventArgs e)
        {
            mResult = MessageBoxResult.No;
            mContinueMessage = cbContinueMessage.IsChecked == true;
            Close();
        }

        /// <summary>
        /// YesNoのCancelボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btYNCancel_Click(object sender, RoutedEventArgs e)
        {
            mResult = MessageBoxResult.Cancel;
            mContinueMessage = cbContinueMessage.IsChecked == true;
            Close();
        }

        /// <summary>
        /// 文字列の大きさを求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>文字列の大きさ(Size.Width/Height)</returns>
        public Size measureText(string text, double textSize)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = textSize;
            //  auto sized (https://code.i-harness.com/ja-jp/q/8d5d0e)
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }
    }
}
