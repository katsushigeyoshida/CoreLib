using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// CopyDialog.xaml の相互作用ロジック
    /// ファイルコピーのダイヤログ
    /// </summary>
    public partial class FileCopyDialog : Window
    {
        public List<string> mSrcFiles;          //  コピー元ファイルリスト
        public string mDestFolder;              //  コピー先フォルダー
        private int mIndex = 0;                 //  コピー位置

        private YLib ylib = new YLib();
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileCopyDialog()
        {
            InitializeComponent();

            cbUpdate.IsChecked = true;
            cbCheckedMessage.IsChecked = true;
        }

        /// <summary>
        /// 初期表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mSrcFiles != null && 0 < mSrcFiles.Count && mDestFolder.Length > 0) {
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
            }
        }

        /// <summary>
        /// OKボタン(コピーを許可して次に行く)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (mIndex < mSrcFiles.Count) {
                ylib.fileCopy(mSrcFiles[mIndex], Path.Combine(mDestFolder, Path.GetFileName(mSrcFiles[mIndex])),cbUpdate.IsChecked==true?0:2);
                mIndex++;
                while (cbCheckedMessage.IsChecked == false && mIndex < mSrcFiles.Count) {
                    setFileInfo(mSrcFiles[mIndex], mDestFolder);
                    ylib.fileCopy(mSrcFiles[mIndex], Path.Combine(mDestFolder, Path.GetFileName(mSrcFiles[mIndex])), cbUpdate.IsChecked == true ? 0 : 2);
                    mIndex++;
                }
            }
            if (mIndex < mSrcFiles.Count)
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
            else
                Close();
        }

        /// <summary>
        /// NOボタン(コピーしないで次に行く)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNO_Click(object sender, RoutedEventArgs e)
        {
            mIndex++;
            if (mIndex < mSrcFiles.Count)
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
            else
                Close();
        }

        /// <summary>
        /// キャンセル(終了)ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// コピーファイルの情報表示
        /// </summary>
        /// <param name="srcpath">コピー元ファイルパス</param>
        /// <param name="destFolder">コピー先フォルダ</param>
        private void setFileInfo(string srcpath,string destFolder)
        {
            FileInfo srcFileInfo = new FileInfo(srcpath);
            tbFileName.Text    = srcFileInfo.Name;
            tbSrcFolder.Text   = srcFileInfo.DirectoryName;
            tbSrcDateTime.Text = srcFileInfo.CreationTime.ToString();
            tbSrcSize.Text     = srcFileInfo.Length.ToString("N");
            FileInfo destFileInfo = new FileInfo(Path.Combine(destFolder, srcFileInfo.Name));
            if (destFileInfo.Exists) {
                tbDestFolder.Text   = destFileInfo.DirectoryName;
                tbDestDateTime.Text = destFileInfo.CreationTime.ToString();
                tbDestSize.Text     = destFileInfo.Length.ToString("N");
            } else {
                tbDestFolder.Text  = "";
                tbDestDateTime.Text = "";
                tbDestSize.Text     = "";
            }
            tbProgress.Text = $"[ {mIndex} / {mSrcFiles.Count} ]";
        }
    }
}
