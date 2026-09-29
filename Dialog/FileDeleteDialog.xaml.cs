using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// FileDeleteDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class FileDeleteDialog : Window
    {
        public List<string> mSrcFiles;          //  ファイルリスト
        private int mFileCount = 0;             //  ファイルの総数


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileDeleteDialog()
        {
            InitializeComponent();

            cbCheckedMessage.IsChecked = true;
        }

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mSrcFiles != null && 0 < mSrcFiles.Count) {
                mFileCount = mSrcFiles.Count;
                setFileInfo(mSrcFiles[0]);
            }
        }

        /// <summary>
        /// OKボタン 削除処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (0 < mSrcFiles.Count) {
                fileDelete(mSrcFiles[0]);
                mSrcFiles.RemoveAt(0);
                while (cbCheckedMessage.IsChecked == false && 0 < mSrcFiles.Count) {
                    setFileInfo(mSrcFiles[0]);
                    fileDelete(mSrcFiles[0]);
                    mSrcFiles.RemoveAt(0);
                }
            }
            if (0 < mSrcFiles.Count)
                setFileInfo(mSrcFiles[0]);
            else
                Close();
        }

        /// <summary>
        /// NOボタン削除を除外
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNO_Click(object sender, RoutedEventArgs e)
        {
            if (0 < mSrcFiles.Count) {
                mSrcFiles.RemoveAt(0);
                if (0 < mSrcFiles.Count)
                    setFileInfo(mSrcFiles[0]);
                else
                    Close();
            } else
                Close();
        }

        /// <summary>
        /// Cancelボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// ファイル情報の表示
        /// </summary>
        /// <param name="srcpath"></param>
        private void setFileInfo(string srcpath)
        {
            FileInfo srcFileInfo = new FileInfo(srcpath);
            tbFileName.Text = srcFileInfo.Name;
            tbSrcFolder.Text = srcFileInfo.DirectoryName;
            tbSrcDateTime.Text = srcFileInfo.CreationTime.ToString();
            tbSrcSize.Text = srcFileInfo.Length.ToString("N");
            tbProgress.Text = $"[ {mFileCount - mSrcFiles.Count} / {mFileCount} ]";
        }

        /// <summary>
        /// ファイル削除処理
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private void fileDelete(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file.Exists) {
                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                    if (MessageBox.Show("読込専用ファイルを削除しますか","確認",MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                } else
                    file.Delete();
            }
        }
    }
}
