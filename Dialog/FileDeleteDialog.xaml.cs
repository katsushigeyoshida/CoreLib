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
        private int mIndex = 0;                 //  削除位置位置
        private bool mContinueDeleteMessage = false;        //  削除確認メッセージ表示

        private YFile yfile = new YFile();

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
            mSrcFiles = getCopyFiles(mSrcFiles);
            if (mSrcFiles != null && 0 < mSrcFiles.Count) {
                mFileCount = mSrcFiles.Count;
                setFileInfo(mSrcFiles[0]);
            }
            pbCopyCount.Minimum = 0;
            pbCopyCount.Maximum = mSrcFiles.Count;
            pbCopyCount.Value = 0;
        }

        /// <summary>
        /// OKボタン 削除処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (mIndex < mSrcFiles.Count) {
                fileDelete(mSrcFiles[mIndex++]);
                pbCopyCount.Value++;
                while (cbCheckedMessage.IsChecked == false && mIndex < mSrcFiles.Count) {
                    setFileInfo(mSrcFiles[mIndex]);
                    fileDelete(mSrcFiles[mIndex++]);
                    pbCopyCount.Value++;
                }
            }
            if (mIndex < mSrcFiles.Count)
                setFileInfo(mSrcFiles[mIndex]);
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
            if (mIndex < mSrcFiles.Count) {
                mIndex++;
                pbCopyCount.Value++;
                setFileInfo(mSrcFiles[mIndex]);
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
            if (!srcFileInfo.Attributes.HasFlag(FileAttributes.Directory))
                tbSrcSize.Text = srcFileInfo.Length.ToString("N0");
            tbProgress.Text = $"[ {mIndex} / {mFileCount} ]";
        }

        /// <summary>
        /// ファイル削除処理
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private bool fileDelete(string path)
        {
            try {
                if (Directory.Exists(path)) {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    dir.Delete(true);
                } else if (File.Exists(path)) {
                    FileInfo file = new FileInfo(path);
                    if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        if (MessageBox.Show("読込専用ファイルを削除しますか", "確認", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                            return true;
                        }
                    } else {
                        file.Delete();
                        return true;
                    }
                }
            } catch (IOException e) {
                System.Diagnostics.Debug.WriteLine($"fileDelete: {e.Message}");
                if (mContinueDeleteMessage) {
                    MessageBoxEx dlg = new MessageBoxEx();
                    dlg.mMessage = $"削除エラー\n{e.Message}";
                    dlg.mButton = MessageBoxButton.YesNo;
                    dlg.mContinueMessageDisp = true;
                    dlg.ShowDialog();
                    mContinueDeleteMessage = dlg.mContinueMessage;
                }
            }
            return false;
        }

        /// <summary>
        /// フォルダからファイルを検索して追加
        /// </summary>
        /// <param name="srcFiles">コピー元リスト</param>
        /// <returns>ファイルリスト</returns>
        private List<string> getCopyFiles(List<string> srcFiles)
        {
            List<string> copyFiles = new List<string>();
            foreach (string path in srcFiles) {
                if (Directory.Exists(path)) {
                    copyFiles.AddRange(yfile.getFiles(Path.Combine(path, "*.*"), true).ToList());
                    copyFiles.Add(path);
                } else if (File.Exists(path)) {
                    copyFiles.Add(path);
                }

            }
            return copyFiles;
        }
    }
}
