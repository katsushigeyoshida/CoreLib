using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CoreLib
{
    /// <summary>
    /// ImageView.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageView : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        public string mImagePath;
        public List<string> mImageList = new List<string>();
        private Point mMousePosition = new Point(0, 0);         //  マウス位置
        private bool mImagMove = false;                         //  イメージ移動フラグ


        private YLib ylib = new YLib();


        public ImageView()
        {
            InitializeComponent();

            WindowFormLoad();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mImageList == null || mImageList.Count == 0) {
                string folder = Path.GetDirectoryName(mImagePath);
                mImageList = ylib.getFiles(Path.Combine(folder, "*.jpg")).ToList();
            }
            ImImage.Source = ylib.getBitmapImage(mImagePath);
            FileInfo fi = new FileInfo(mImagePath);
            Title =$"画像データ [ {fi.FullName} ][{fi.LastWriteTime.ToString()}][{fi.Length.ToString("N")}";
            setPhotoInfo(mImagePath);
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            if (WindowState != mWindowState &&
                WindowState == WindowState.Maximized) {
                //  ウィンドウの最大化時
                mWindowWidth = SystemParameters.WorkArea.Width;
                mWindowHeight = SystemParameters.WorkArea.Height;
            } else if (WindowState != mWindowState ||
                mWindowWidth != Width ||
                mWindowHeight != Height) {
                //  ウィンドウサイズが変わった時
                mWindowWidth = Width;
                mWindowHeight = Height;
            } else {
                //  ウィンドウサイズが変わらない時は何もしない
                mWindowState = WindowState;
                return;
            }
            mWindowState = WindowState;
            //  ウィンドウの大きさに合わせてコントロールの幅を変更する
            double dx = mWindowWidth - mPrevWindowWidth;
            mPrevWindowWidth = mWindowWidth;
            //  表示の更新
            //sampleGraphInit();
            //drawSampleGraph(mStartPosition, mEndPosition);
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.ImageViewWidth < 100 ||
                Properties.Settings.Default.ImageViewHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.ImageViewHeight) {
                Properties.Settings.Default.ImageViewWidth = mWindowWidth;
                Properties.Settings.Default.ImageViewHeight = mWindowHeight;
            } else {
                Top = Properties.Settings.Default.ImageViewTop;
                Left = Properties.Settings.Default.ImageViewLeft;
                Width = Properties.Settings.Default.ImageViewWidth;
                Height = Properties.Settings.Default.ImageViewHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.ImageViewTop = Top;
            Properties.Settings.Default.ImageViewLeft = Left;
            Properties.Settings.Default.ImageViewWidth = Width;
            Properties.Settings.Default.ImageViewHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// キーコマンド処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            double cx = ImImage.ActualWidth / 2.0;
            double cy = ImImage.ActualHeight / 2.0;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control) {
                if (e.Key == Key.Left) {                //  左に移動
                    moveImage(-50, 0);
                } else if (e.Key == Key.Right) {        //  右に移動
                    moveImage(50, 0);
                } else if (e.Key == Key.Up) {           //  上に移動
                    moveImage(0, -50);
                } else if (e.Key == Key.Down) {         //  下に移動
                    moveImage(0, 50);
                } else if (e.Key == Key.C) {            //  画面コピー
                    ylib.image2Clipbord(ImImage.Source);
                } else if (e.Key == Key.E) {            //  コメント登録
                    setComment(mImagePath);
                } else if (e.Key == Key.G) {            //  緯度経度編集
                    editCoordinate(mImagePath);
                } else if (e.Key == Key.I) {            //  イメージのプロパティ
                    infoImage(mImagePath);
                }
            } else {
                if (e.Key == Key.Left) {                //  左に移動
                    //  前のデータファイルを表示
                    nextImage(-1);
                } else if (e.Key == Key.Right) {        //  右に移動
                    //  次のデータファイルを表示
                    nextImage(1);
                } else if (e.Key == Key.PageUp) {       //  拡大
                    imageZoom(1.25, cx, cy);
                } else if (e.Key == Key.PageDown) {     //  縮小
                    imageZoom(1 / 1.25, cx, cy);
                } else if (e.Key == Key.F5) {           //  再表示

                } else if (e.Key == Key.Escape) {       //  終了
                    Close();
                } else if (e.Key == Key.Home) {         //  初期状態
                                                        //  イメージを初期状態にする
                    ImImage.RenderTransform = new MatrixTransform(new Matrix());
                } else if (e.Key == Key.R) {            //  回転
                    rotateImage(90);
                }
            }
        }

        /// <summary>
        /// ステータスバーのボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            double cx = ImImage.ActualWidth / 2.0;
            double cy = ImImage.ActualHeight / 2.0;
            if (button.Name == "BtGInfo") {
                //  イメージのプロパティ表示
                infoImage(mImagePath);
            } else if (button.Name == "BtGComment") {
                //  コメント追加・７編集
                setComment(mImagePath);
            } else if (button.Name == "BtGImageCopy") {
                //  画像をクリップボードにコピー
                ylib.image2Clipbord(ImImage.Source);
            } else if (button.Name == "BtGZoomReset") {
                //  全体表示(イメージを初期状態にする)
                Matrix matrix = new Matrix();
                ImImage.RenderTransform = new MatrixTransform(matrix);
            } else if (button.Name == "BtGZoomUp") {
                //  拡大
                imageZoom(1.25, cx, cy);
            } else if (button.Name == "BtGZoomDown") {
                //  縮小
                imageZoom(1 / 1.25, cx, cy);
            } else if (button.Name == "BtRotate") {
                //  回転
                rotateImage(90);
            } else if (button.Name == "BtPrevImage") {
                //  前のデータファイルを表示
                nextImage(-1);
            } else if (button.Name == "BtNextImage") {
                //  次のデータファイルを表示
                nextImage(1);
            }
        }

        /// <summary>
        /// [MouseWheel]マウスホイールによる拡大縮小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point pos = e.GetPosition(this);
            var scale = 1.25;
            if (e.Delta < 0)
                scale = 1 / scale;
            imageZoom(scale, pos.X, pos.Y);
        }

        /// <summary>
        /// [MouseLeftButtonDown]イメージ移動の開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mMousePosition = e.GetPosition(this);
            mImagMove = true;
        }

        /// <summary>
        /// [MouseLeftButtonUp]イメージ移動の終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mImagMove = false;
        }

        /// <summary>
        /// [MouseMove]イメージの移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (mImagMove) {
                Point pos = e.GetPosition(this);
                Matrix matrix = ((MatrixTransform)ImImage.RenderTransform).Matrix;
                matrix.Translate(pos.X - mMousePosition.X, pos.Y - mMousePosition.Y);
                ImImage.RenderTransform = new MatrixTransform(matrix);
                mMousePosition = pos;
            }
        }

        /// <summary>
        /// コンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            double cx = ImImage.ActualWidth / 2.0;
            double cy = ImImage.ActualHeight / 2.0;
            if (menuItem.Name.CompareTo("imImageInfoMenu") == 0) {
                //  イメージのプロパティ表示
                infoImage(mImagePath);
            } else if (menuItem.Name.CompareTo("imCommenMenu") == 0) {
                //  コメント追加・７編集
                setComment(mImagePath);
            } else if (menuItem.Name.CompareTo("imClipCopyMenu") == 0) {
                //  画像をクリップボードにコピー
                ylib.image2Clipbord(ImImage.Source);
            } else if (menuItem.Name.CompareTo("imCoordinateMenu") == 0) {
                //  緯度経度座標編集
                editCoordinate(mImagePath);
            } else if (menuItem.Name.CompareTo("imZoomFitMenu") == 0) {
                //  全体表示(イメージを初期状態にする)
                Matrix matrix = new Matrix();
                ImImage.RenderTransform = new MatrixTransform(matrix);
            } else if (menuItem.Name.CompareTo("imZoomUpMenu") == 0) {
                //  拡大
                imageZoom(1.25, cx, cy);
            } else if (menuItem.Name.CompareTo("imZoomDownMenu") == 0) {
                //  縮小
                imageZoom(1 / 1.25, cx, cy);
            } else if (menuItem.Name.CompareTo("imRotateMenu") == 0) {
                //  回転
                rotateImage(90);
            } else if (menuItem.Name.CompareTo("imPrevMenu") == 0) {
                //  前のデータファイルを表示
                nextImage(-1);
            } else if (menuItem.Name.CompareTo("imNextMenu") == 0) {
                //  次のデータファイルを表示
                nextImage(1);
            }
        }

        /// <summary>
        /// [コピー]コンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imClipCopyMenu_Click(object sender, RoutedEventArgs e)
        {
            ylib.image2Clipbord(ImImage.Source);
        }

        /// <summary>
        /// [コメント追加]コンテキストメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imCommenMenu_Click(object sender, RoutedEventArgs e)
        {
            setComment(mImagePath);
        }

        /// <summary>
        /// コメントデータを設定する
        /// </summary>
        private void setComment(string path)
        {
            DateTime lastDateTime = ylib.getFileDateTime(path);
            ExifInfo exifInfo = new ExifInfo(path);
            string comment = exifInfo.getUserComment();
            if (comment.Length <= 0)
                comment += ylib.getIPTC(path)[4];
            InputBox dlg = new InputBox();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Title = "コメント登録";
            dlg.mEditText = comment;
            if (dlg.ShowDialog() == true) {
                if (exifInfo.setUserComment(dlg.mEditText))
                    if (!exifInfo.save()) {
                        MessageBox.Show(exifInfo.mErrorMsg);
                    } else {
                        setPhotoInfo(path);
                        ylib.setFileDateTime(path, lastDateTime);
                    }
            }
        }

        /// <summary>
        /// 座標データの追加・編集
        /// </summary>
        /// <param name="path"></param>
        private void editCoordinate(string path)
        {
            ExifInfo exifInfo = new ExifInfo(path);
            Point coord = exifInfo.getExifGpsCoordinate();
            InputBox dlg = new InputBox();
            dlg.Title = "座標編集(緯度,軽度)";
            dlg.mEditText = coord.Y + "," + coord.X;
            if (dlg.ShowDialog() == true) {
                string[] data = dlg.mEditText.Split(',');
                if (1 <= data.Length) {
                    coord.X = ylib.string2double(data[1]);
                    coord.Y = ylib.string2double(data[0]);
                    if (exifInfo.setExifGpsCoordinate(coord))
                        exifInfo.save();
                }
            }
        }

        /// <summary>
        /// 画像ファイルの移動
        /// </summary>
        /// <param name="next"></param>
        private void nextImage(int next)
        {
            int n = mImageList.IndexOf(mImagePath);
            if (0 <= (n + next) && n < mImageList.Count - next) {
                mImagePath = mImageList[n + next];
                ImImage.Source = ylib.getBitmapImage(mImagePath);
                FileInfo fi = new FileInfo(mImagePath);
                Title = $"画像データ [ {fi.FullName} ][{fi.LastWriteTime.ToString()}][{fi.Length.ToString("N")}";
                setPhotoInfo(mImagePath);
            }
        }

        /// <summary>
        /// イメージの拡大縮小
        /// </summary>
        /// <param name="scale">拡大率</param>
        /// <param name="cx">拡大中心座標X</param>
        /// <param name="cy">拡大中心座標Y</param>
        private void imageZoom(double scale, double cx, double cy)
        {
            Matrix matrix = ((MatrixTransform)ImImage.RenderTransform).Matrix;
            matrix.ScaleAt(scale, scale, cx, cy);
            ImImage.RenderTransform = new MatrixTransform(matrix);
        }

        /// <summary>
        /// イメージの移動
        /// </summary>
        /// <param name="dx">X方向の移動量</param>
        /// <param name="dy">Y方向の移動量</param>
        private void moveImage(double dx, double dy)
        {
            Matrix matrix = ((MatrixTransform)ImImage.RenderTransform).Matrix;
            matrix.Translate(dx, dy);
            ImImage.RenderTransform = new MatrixTransform(matrix);
        }

        /// <summary>
        /// イメージの回転
        /// </summary>
        /// <param name="angle">回転角(dig)</param>
        private void rotateImage(double angle)
        {
            double cx = ImImage.ActualWidth / 2.0;
            double cy = ImImage.ActualHeight / 2.0;
            Matrix matrix = ((MatrixTransform)ImImage.RenderTransform).Matrix;
            matrix.RotateAt(angle, cx, cy);
            ImImage.RenderTransform = new MatrixTransform(matrix);
        }

        /// <summary>
        /// イメージのプロパティ表示
        /// </summary>
        private void infoImage(string path)
        {
            string buf = ylib.getIPTCall(path);
            ExifInfo exifInfo = new ExifInfo(path);
            buf += "\n" + exifInfo.getExifInfoAll();
            messageBox(buf, "属性表示[" + Path.GetFileName(path) + "]");
        }

        /// <summary>
        /// ステータスバーに画像情報を表示する
        /// </summary>
        /// <param name="path"></param>
        private void setPhotoInfo(string path)
        {
            //  ファイルプロパティ表示
            System.Windows.Media.Imaging.BitmapImage bmpImage = ylib.getBitmapImage(path);
            ExifInfo exifInfo = new ExifInfo(path);
            Point coodinate = exifInfo.getExifGpsCoordinate();
            string[] datetime = exifInfo.getDateTime().Split(':');
            TbPhotoInfo.Text = datetime.Length == 5 ?
                $"{datetime[0]}/{datetime[1]}/{datetime[2]}:{datetime[3]}:{datetime[4]}" : exifInfo.getDateTime();
            if (coodinate.X < 0 || coodinate.Y < 0)
                TbPhotoInfo.Text += " (座標なし)";
            List<string> iptc = ylib.getIPTC(path);
            TbPhotoInfo.Text += " " + (iptc.Count > 3 ? iptc[4] : "") + exifInfo.getUserComment();
            TbPhotoInfo.Text += " [" + bmpImage.PixelWidth + "x" + bmpImage.PixelHeight + "]";
            TbPhotoInfo.Text += " " + exifInfo.getCamera("カメラ {0} {1}");
            TbPhotoInfo.Text += " " + exifInfo.getCameraSetting(" 1/{0} s F{1} ISO {2} 焦点距離 {3} mm");
            exifInfo.close();
        }

        /// <summary>
        /// メッセージ表示ダイヤログ
        /// </summary>
        /// <param name="buf">メッセージ</param>
        /// <param name="title">タイトル</param>
        private void messageBox(string buf, string title)
        {
            InputBox dlg = new InputBox();
            //dlg.mMainWindow = this;           //  親Windowの中心に表示
            dlg.Title = title;
            dlg.mWindowSizeOutSet = true;
            dlg.mWindowWidth = 500.0;
            dlg.mWindowHeight = 400.0;
            dlg.mMultiLine = true;
            dlg.mReadOnly = true;
            dlg.mEditText = buf;
            dlg.ShowDialog();
        }
    }
}
