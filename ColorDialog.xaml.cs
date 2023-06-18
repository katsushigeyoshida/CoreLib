using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CoreLib
{
    /// <summary>
    /// ColorDialog.xaml の相互作用ロジック
    /// カラー選択ダイヤログ
    /// 参考
    /// https://resanaplaza.com/2021/03/29/【wpf】色（カラー）一覧を取得してcomboboxやlistで選択す/
    /// </summary>
    public partial class ColorDialog : Window
    {
        /// <summary>
        /// 色と色名を保持するクラス
        /// </summary>
        public class MyColor
        {
            public Color Color { get; set; }
            public string Name { get; set; }
        }

        //  ウィンドウの位置
        public Window mMainWindow = null;                   //  親ウィンドウの設定
        public int mHorizontalAliment = -1;                 //  0:Left 1:Center 2:Right
        public int mVerticalAliment = -1;                   //  0:Top 1:Center 2:Bottom
        public bool mOneClick = false;                      //  OneClickで選択

        public Color mColor;
        public string mColorName;

        public ColorDialog()
        {
            InitializeComponent();

            DataContext = GetColorList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Height = Math.Min(mMenuList.Count * 28 + 25, Height);

            if (mMainWindow != null) {
                //  親ウィンドウに対しての表示位置
                //  水平方向
                if (mHorizontalAliment == 0)
                    Left = mMainWindow.Left;                                    //  LEFT
                else if (mHorizontalAliment == 2)
                    Left = mMainWindow.Left + (mMainWindow.Width - Width);      //  RIGHT
                else
                    Left = mMainWindow.Left + (mMainWindow.Width - Width) / 2;  //  CENTER
                //  垂直方向
                if (mVerticalAliment == 0)
                    Top = mMainWindow.Top;                                      //  TOP
                else if (mVerticalAliment == 2)
                    Top = mMainWindow.Top + (mMainWindow.Height - Height);      //  BOTTOM
                else
                    Top = mMainWindow.Top + (mMainWindow.Height - Height) / 2;  //  CENTER
            }

            //  メニューリストにフォーカスを設定
            lbColotList.Focus();
        }


        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mycolor = (MyColor)((ListBox)sender).SelectedItem;
            selectColor(mycolor);
        }

        private void lbColotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mOneClick) {
                var mycolor = (MyColor)((ListBox)sender).SelectedItem;
                selectColor(mycolor);
            }
        }

        /// <summary>
        /// すべての色を取得するメソッド
        /// </summary>
        /// <returns></returns>
        private MyColor[] GetColorList()
        {
            return typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(i => new MyColor() { Color = (Color)i.GetValue(null), Name = i.Name }).ToArray();
        }

        /// <summary>
        /// 選択した色を設定して終了する
        /// </summary>
        /// <param name="mycolor"></param>
        private void selectColor(MyColor mycolor)
        {
            mColor = mycolor.Color;
            mColorName = mycolor.Name;

            Close();
        }
    }
}
