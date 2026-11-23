using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// CopyDialog.xaml の相互作用ロジック
    /// ファイルコピーのダイヤログ
    /// モードレスで使用するとき(dialog.Show())はコールバック関数の読み出しを指定できる
    /// ファイル比較ツール(WinSyncなど)を登録する「比較」ボタンでファイル比較ができる
    /// </summary>
    public partial class FileCopyDialog : Window
    {
        public List<string> mSrcFiles;              //  コピー元ファイルリスト
        private List<string> mSrcFileList;
        public string mSrcFolder;                   //  コピー元フォルダ
        public string mDestFolder;                  //  コピー先フォルダ
        private int mIndex = 0;                     //  コピー位置
        public bool mSrcRemove = false;             //  削除処理
        public bool mOverWrite = false;             //  強制上書き
        private bool mReadOnlyContinue = false;     //  ReadOnlyファイルの確認無視
        private bool mReadOnlyRemove = false;       //  ReadOnlyファイル削除
        private bool mDeleteMessage = true;         //  削除確認メッセージの有無
        private bool mDelete = false;               //  削除
        private bool mFolderMessage = true;         //  フォルダ作成確認メッセージ
        private bool mFolderCreate = true;          //  フォルダ作成
        private List<string> mDeleteFileName = new List<string>();  //  削除ファイル
        private List<string> mExceptFileName = new List<string>();  //  削除除外ファイル
        private List<string> mExceptDirectory = new List<string>(); //  削除除外ディレクトリ

        public bool mCallBackOn = false;            //  コールバック有り
        public Action callback;                     //  コールバック関数
        public string mFileDiffTool = "";           //  ファイル比較アプリ
        private string mErrorMessage = "";
        public bool mFilter = false;                //  対象/除外ファイル/ディレクトリ設定の表示/非表示

        private YLib ylib = new YLib();
        private YFile yfile = new YFile();
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileCopyDialog()
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
            if (mSrcRemove) {
                Title = "ファイルの移動";
            } else {
                Title = "ファイルのコピー";
            }

            if (mFileDiffTool == "")
                btCompare.Visibility = Visibility.Collapsed;
            else
                btCompare.Visibility = Visibility.Visible;

            if (mFilter)
                spFileSelect.Visibility = Visibility.Visible;
            else {
                spFileSelect.Visibility = Visibility.Collapsed;
                Height -= 20;
            }

            cbOverWrite.IsChecked = mOverWrite;

            //  コピー元の状態確認
            if (mSrcFiles == null || mSrcFiles.Count == 0)
                Close();
            if (mSrcFolder == null || mSrcFolder == "")
                mSrcFolder = Path.GetDirectoryName(mSrcFiles[0]);

            //  コピー先の状態確認
            if (mDestFolder == null || mDestFolder == "") {
                MessageBox.Show("転送先フォルダが設定されていません");
                Close();
            }
            mDestFolder = Path.GetFullPath(mDestFolder);
            if (!Directory.Exists(mDestFolder)) {
                MessageBoxResult result = ylib.messageBox(this, "転送先のフォルダがありません\n作成しますか?", mDestFolder, "",
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
                else if (result == MessageBoxResult.Yes)
                    Directory.CreateDirectory(mDestFolder);
            }

            //  コピー元ファイル検索
            getSrcFiles();
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Window_Closing");
        }

        /// <summary>
        /// ファイル検索
        /// </summary>
        private void getSrcFiles()
        {
            mSrcFileList = getFiles(mSrcFiles);
            if (mSrcFileList != null && 0 < mSrcFileList.Count && mDestFolder.Length > 0) {
                setFileInfo(mSrcFileList[mIndex], mSrcFolder, mDestFolder);
                pbCopyCount.Minimum = 0;
                pbCopyCount.Maximum = mSrcFileList.Count;
                pbCopyCount.Value = 0;
            }
        }

        /// <summary>
        /// [再検索]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReload_Click(object sender, RoutedEventArgs e)
        {
            //  対象/除外ファイル、除外ディレクトリの設定
            mDeleteFileName = tbFileName.Text.Split(' ').ToList();
            mDeleteFileName.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            mExceptFileName = tbExceptFile.Text.Split(' ').ToList();
            mExceptFileName.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            mExceptDirectory = tbExceptDirectory.Text.Split(' ').ToList();
            mExceptDirectory.RemoveAll(s => string.IsNullOrWhiteSpace(s));

            getSrcFiles();
        }

        /// <summary>
        /// OKボタン(コピーを許可して次に行く)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            int interval = Math.Max(1, mSrcFileList.Count / 100);      //  画面更新のタイミング
            if (mIndex < mSrcFileList.Count) {
                if (!fileSync(mSrcFileList[mIndex], mSrcFolder, mDestFolder, cbOverWrite.IsChecked == false ? 0 : 2))
                    return;
                mIndex++;
                pbCopyCount.Value++;
                while (cbCheckedMessage.IsChecked == false && mIndex < mSrcFileList.Count) {
                    setFileInfo(mSrcFileList[mIndex], mSrcFolder, mDestFolder);
                    if (!fileSync(mSrcFileList[mIndex], mSrcFolder, mDestFolder, cbOverWrite.IsChecked == false ? 0 : 2))
                        return;
                    mIndex++;
                    pbCopyCount.Value++;
                    if (mCallBackOn && (mIndex % interval) == 0)
                        callback();
                    if (mIndex % interval == 0)
                        ylib.DoEvents();
                }
            }
            if (mIndex < mSrcFileList.Count)
                setFileInfo(mSrcFileList[mIndex], mSrcFolder, mDestFolder);
            else {
                if (mCallBackOn)
                    callback();
                Close();
            }
        }

        /// <summary>
        /// ファイルの同期(基本コピーでファイル元がなければコピー先ファイルを作治よ)
        /// </summary>
        /// <param name="srcPath">コピー元ファイルパス</param>
        /// <param name="srcFolder">コピー元フォルダ</param>
        /// <param name="destFolder">コピー先フォルダ</param>
        /// <param name="copyType">コピーの種類(上書き=1/0)</param>
        /// <returns></returns>
        private bool fileSync(string srcPath, string srcFolder, string destFolder, int copyType)
        {
            string destPath = getDestPath(srcPath, srcFolder, destFolder);
            if (!File.Exists(srcPath) && File.Exists(destPath)) {
                if (mDeleteMessage) {
                    (MessageBoxResult result, bool cont) = ylib.messageBoxEx(this,
                        "コピー元にないファイルを削除しますか?",
                        Path.GetFileName(srcPath), "確認", MessageBoxButton.OKCancel, true);
                    mDelete = result == MessageBoxResult.OK;
                    mDeleteMessage = cont;
                }
                if (mDelete)
                    removeFile(destPath);
            } else {
                if (!Directory.Exists(Path.GetDirectoryName(destPath))) {
                    if (mFolderMessage) {
                        (MessageBoxResult result, bool cont) = ylib.messageBoxEx(this,
                            $"{Path.GetDirectoryName(destPath)} フォルダを作成しますか?",
                            Path.GetFileName(srcPath), "確認", MessageBoxButton.OKCancel, true);
                        mFolderCreate = result == MessageBoxResult.OK;
                        mFolderMessage = cont;
                    }
                }
                if (mFolderCreate) {
                    if (!fileCopy(mSrcFileList[mIndex], mSrcFolder, mDestFolder, cbOverWrite.IsChecked == false ? 0 : 2)) {
                        MessageBox.Show($"例外エラー {mErrorMessage}");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// NOボタン(コピーしないで次に行く)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNO_Click(object sender, RoutedEventArgs e)
        {
            mIndex++;
            pbCopyCount.Value++;
            if (mIndex < mSrcFileList.Count)
                setFileInfo(mSrcFileList[mIndex], mSrcFolder, mDestFolder);
            else {
                if (mCallBackOn)
                    callback();
                Close();
            }
        }

        /// <summary>
        /// キャンセル(終了)ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            if (mCallBackOn)
                callback();
            Close();
        }

        /// <summary>
        /// 比較ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCompare_Click(object sender, RoutedEventArgs e)
        {
            fileCompre(mSrcFileList[mIndex], mSrcFolder, mDestFolder);
        }

        /// <summary>
        /// フォルダからファイルを検索して追加
        /// </summary>
        /// <param name="srcFiles">コピー元リスト</param>
        /// <returns>ファイルリスト</returns>
        private List<string> getFiles(List<string> srcFiles)
        {
            List<string> fileList = new List<string>();
            foreach (string path in srcFiles) {
                if (Directory.Exists(path)) {
                    fileList.AddRange(getFiles(path));
                } else {
                    fileList.Add(path);
                }
            }
            return fileList;
        }

        /// <summary>
        /// ファイルの検索
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> getFiles(string path)
        {
            List<string> filesList = new List<string>();
            if (Directory.Exists(path)) {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files) {
                    if (isMatchFile(Path.GetFileName(file)))
                        filesList.Add(file);
                }
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories) {
                    if (isMatchDirectory(Path.GetFileName(directory)))
                        filesList.AddRange(getFiles(directory));
                }
            } else if (File.Exists(path)) {
                filesList.Add(path);
            }
            return filesList;
        }

        /// <summary>
        /// ファイルの削除処理
        /// </summary>
        /// <param name="path">削除ファイルパス</param>
        private void removeFile(string path)
        {
            FileInfo file = new FileInfo(path);
            if (file.Exists) {
                if (file.Attributes.HasFlag(FileAttributes.ReadOnly)) {
                    //  ReadOnlyファイルの確認
                    if (!mReadOnlyContinue) {
                        MessageBoxEx dlg = new MessageBoxEx();
                        dlg.mMessage = "読込専用ファイルを削除しますか";
                        dlg.mButton = MessageBoxButton.YesNo;
                        dlg.mContinueMessageDisp = true;
                        dlg.ShowDialog();
                        mReadOnlyContinue = dlg.mContinueMessage;
                        mReadOnlyRemove = dlg.mResult == MessageBoxResult.Yes;
                    }
                    //  ReadOnlyファイルの削除
                    if (mReadOnlyRemove) {
                        file.Attributes &= ~FileAttributes.ReadOnly;
                        file.Delete();
                    }
                } else
                    file.Delete();
            }
        }

        /// <summary>
        /// ファイルをアプリで比較
        /// </summary>
        /// <param name="srcPath">コピー元ファイル</param>
        /// <param name="srcFolder">コピー元フォルダ</param>
        /// <param name="destFolder">コピー先フォルダ</param>
        private void fileCompre(string srcPath, string srcFolder, string destFolder)
        {
            string destPath = getDestPath(srcPath, srcFolder, destFolder);
            if (File.Exists(srcPath) && File.Exists(destPath))
                ylib.processStart(mFileDiffTool, $"\"{srcPath}\" \"{destPath}\"");
        }

        /// <summary>
        /// ファイルのコピーまたは移動処理
        /// </summary>
        /// <param name="srcPath">コピー元ファイル</param>
        /// <param name="srcFolder">コピー元フォルダ</param>
        /// <param name="destFolder">コピー先フォルダー</param>
        /// <param name="copyType">更新方法(0:日付更新 1:日付またはサイズが異なる  2:上書き 3:新規のみ</param>
        private bool fileCopy(string srcPath, string srcFolder, string destFolder, int copyType)
        {
            try {
                string destPath = getDestPath(srcPath, srcFolder, destFolder);
                yfile.fileCopy(srcPath, destPath, copyType);     //  ディレクトリ自動作成でコピー
                if (mSrcRemove) {
                    removeFile(srcPath);
                }
                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"YFile.filecopy: {e.Message}");
                mErrorMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// コピーファイルの情報表示
        /// </summary>
        /// <param name="srcPath">コピー元ファイルパス</param>
        /// <param name="destFolder">コピー先フォルダ</param>
        private void setFileInfo(string srcPath, string srcFolder, string destFolder)
        {
            (string srcName, srcFolder) = getSrcPath(srcPath, srcFolder);
            string destPath = Path.Combine(destFolder, srcName);

            //  コピー元ファイル情報
            FileInfo srcFileInfo = new FileInfo(srcPath);
            tbFileName.Text = srcName.Trim('\\');
            tbSrcFolder.Text = srcFolder;
            if (srcFileInfo.Exists) {
                tbSrcDateTime.Text = srcFileInfo.LastWriteTime.ToString();
                tbSrcSize.Text = srcFileInfo.Length.ToString("N0");
            } else {
                tbSrcDateTime.Text = "---";
                tbSrcSize.Text = "---";
            }
            //  コピー先ファイル情報
            FileInfo destFileInfo = new FileInfo(destPath);
            if (destFileInfo.Exists) {
                tbDestFolder.Text = destFolder;
                tbDestDateTime.Text = destFileInfo.LastWriteTime.ToString();
                tbDestSize.Text = destFileInfo.Length.ToString("N0");
            } else {
                tbDestFolder.Text = destFolder;
                tbDestDateTime.Text = "---";
                tbDestSize.Text = "---";
            }

            tbProgress.Text = $"[ {mIndex} / {mSrcFileList.Count} ]";

            //  比較ボタン表示
            if (mFileDiffTool != "") {
                if (srcFileInfo.Exists && destFileInfo.Exists)
                    btCompare.Visibility = Visibility.Visible;
                else
                    btCompare.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 転送先ファイルパスの作成
        /// </summary>
        /// <param name="srcPath">転送元ファイルパス</param>
        /// <param name="srcFolder">転送元フォルダ</param>
        /// <param name="destFolder">転送先フォルダ</param>
        /// <returns></returns>
        private string getDestPath(string srcPath, string srcFolder, string destFolder)
        {
            if (srcFolder == "")
                return Path.Combine(destFolder, Path.GetFileName(srcPath));
            else
                return srcPath.Replace(srcFolder, destFolder);
        }

        /// <summary>
        /// 転送元ファイル名とフォルダ名の取得
        /// </summary>
        /// <param name="srcPath">転送元ファイルパス</param>
        /// <param name="srcFolder">転送元フォルダ</param>
        /// <returns>(ファイル名とフォルダ名)</returns>
        private (string name, string folder) getSrcPath(string srcPath, string srcFolder)
        {
            string srcName;
            if (srcFolder == "") {
                srcName = Path.GetFileName(srcPath);
                srcFolder = Path.GetDirectoryName(srcPath);
            } else
                srcName = srcPath.Replace(srcFolder, "").Trim('\\');
            return (srcName, srcFolder);
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
