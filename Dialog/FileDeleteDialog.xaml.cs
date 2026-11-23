using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace CoreLib
{
    /// <summary>
    /// FileDeleteDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class FileDeleteDialog : Window
    {
        public List<string> mSrcFiles;                  //  ファイルリスト
        private List<string> mSrcFileList;
        private int mFileCount = 0;                     //  ファイルの総数
        private int mIndex = 0;                         //  削除位置位置
        private List<string> mDeleteFileName = new List<string>();  //  削除ファイル
        private List<string> mExceptFileName = new List<string>();  //  削除除外ファイル
        private List<string> mExceptDirectory = new List<string>(); //  削除除外ディレクトリ
        private bool mContinueDeleteMessage = false;    //  削除確認メッセージ表示
        public bool mFilter = false;                    //  対象/除外ファイル/ディレクトリの表示

        private YFile yfile = new YFile();
        private YLib ylib = new YLib();

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
            if (mFilter)
                spFileSelect.Visibility = Visibility.Visible;
            else {
                spFileSelect.Visibility = Visibility.Collapsed;
                Height -= 20;
            }

            //  削除ファイル検索
            getSrcFiles();
        }

        /// <summary>
        /// ファイル検索
        /// </summary>
        private void getSrcFiles()
        {
            mSrcFileList = getFiles(mSrcFiles);
            if (mSrcFileList != null && 0 < mSrcFileList.Count) {
                mFileCount = mSrcFileList.Count;
                setFileInfo(mSrcFileList[0]);
            }
            pbCopyCount.Minimum = 0;
            pbCopyCount.Maximum = mSrcFileList.Count;
            pbCopyCount.Value = 0;
        }

        /// <summary>
        /// [再検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReload_Click(object sender, RoutedEventArgs e)
        {
            //  対象/除外ファイル、除外ディレクトリの設定
            mDeleteFileName = tbTargetFile.Text.Split(' ').ToList();
            mDeleteFileName.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            mExceptFileName = tbExceptFile.Text.Split(' ').ToList();
            mExceptFileName.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            mExceptDirectory = tbExceptDirectory.Text.Split(' ').ToList();
            mExceptDirectory.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            getSrcFiles();
        }

        /// <summary>
        /// OKボタン 削除処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            //  ファイル削除
            if (mIndex < mSrcFileList.Count) {
                fileDelete(mSrcFileList[mIndex]);
                directoryDelete(mSrcFileList[mIndex++]);
                pbCopyCount.Value++;
                while (cbCheckedMessage.IsChecked == false && mIndex < mSrcFileList.Count) {
                    setFileInfo(mSrcFileList[mIndex]);
                    fileDelete(mSrcFileList[mIndex]);
                    directoryDelete(mSrcFileList[mIndex++]);
                    pbCopyCount.Value++;
                }
            }
            if (mIndex < mSrcFileList.Count)
                setFileInfo(mSrcFileList[mIndex]);
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
            if (mIndex < mSrcFileList.Count - 1) {
                mIndex++;
                pbCopyCount.Value++;
                setFileInfo(mSrcFileList[mIndex]);
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
                    if ((dir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                        if (MessageBox.Show("読込専用ディレクトリを削除しますか", "確認", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                            dir.Attributes = FileAttributes.Normal;
                            dir.Delete();
                            return true;
                        }
                    } else {
                        dir.Delete();
                        return true;
                    }
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
                if (!mContinueDeleteMessage) {
                    MessageBoxEx dlg = new MessageBoxEx();
                    dlg.mMessage = $"削除エラー\n{e.Message}";
                    dlg.mButton = MessageBoxButton.OK;
                    dlg.mContinueMessageDisp = true;
                    dlg.ShowDialog();
                    mContinueDeleteMessage = dlg.mContinueMessage;
                }
            }
            return false;
        }

        /// <summary>
        /// フォルダの削除
        /// </summary>
        /// <param name="path">フォルダパス</param>
        private void directoryDelete(string path)
        {
            if (Directory.Exists(path)) {
                deleteDirctories(new DirectoryInfo(path));
            }
        }

        /// <summary>
        /// 空のフォルダを削除する
        /// </summary>
        /// <param name="directorr">フォルダ</param>
        /// <returns>可否</returns>
        private bool deleteDirctories(DirectoryInfo directory)
        {
            try {
                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (DirectoryInfo di in directories) {
                    if (di.Exists) {
                        if (deleteDirctories(di))
                            di.Delete();
                        else
                            return false;
                    }
                }
                return true;
            } catch (IOException e) {
                System.Diagnostics.Debug.WriteLine($"deleteDirctories: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// フォルダからファイルを検索して追加
        /// </summary>
        /// <param name="srcFiles">コピー元リスト</param>
        /// <returns>ファイルリスト</returns>
        private List<string> getFiles(List<string> srcFiles)
        {
            List<string> files = new List<string>();
            foreach (string path in srcFiles) {
                if (Directory.Exists(path)) {
                    files.AddRange(getFiles(path));
                    files.Add(path);
                } else if (File.Exists(path)) {
                    files.Add(path);
                }
            }
            return files;
        }

        /// <summary>
        /// ファイルとフォルダの検索
        /// </summary>
        /// <param name="path">検索パス</param>
        /// <returns>ファイルとフォルダのリスト</returns>
        private List<string> getFiles(string path)
        {
            List<string> filesDirectories = new List<string>();
            if (Directory.Exists(path)) {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files) {
                    if (isMatchFile(Path.GetFileName(file)))
                        filesDirectories.Add(file);
                }
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories) {
                    if (isMatchDirectory(Path.GetFileName(directory))) {
                        filesDirectories.AddRange(getFiles(directory));
                        filesDirectories.Add(directory);
                    }
                }
            } else if (File.Exists(path)) {
                filesDirectories.Add(path);
            }
            return filesDirectories;
        }

        /// <summary>
        /// 対象ファイルか除外ファイルのチェック
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>可否</returns>
        private bool isMatchFile(string fileName)
        {
            if (mDeleteFileName.Count != 0) {
                foreach (string deleteFile in mDeleteFileName)
                    if (ylib.wcMatch(fileName, deleteFile))
                        return true;
                return false;
            }
            if (mExceptFileName.Count != 0) {
                foreach (string exceptFile in mExceptFileName)
                    if (ylib.wcMatch(fileName, exceptFile))
                        return false;
            }
            return true;
        }

        /// <summary>
        /// 対象フォルダかのチェック(除外フォルダに該当する)
        /// </summary>
        /// <param name="directoryName">ディレクトリ名</param>
        /// <returns>可否</returns>
        private bool isMatchDirectory(string directoryName)
        {
            if (mExceptDirectory.Count != 0) {
                foreach (string exceptDirectory in mExceptDirectory)
                    if (ylib.wcMatch(directoryName, exceptDirectory))
                        return false;
            }
            return true;
        }
    }
}
