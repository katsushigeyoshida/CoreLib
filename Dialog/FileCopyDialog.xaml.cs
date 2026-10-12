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
        public string mSrcFolder;
        public string mDestFolder;                  //  コピー先フォルダー
        private int mIndex = 0;                     //  コピー位置
        public bool mSrcRemove = false;             //  削除処理
        private bool mReadOnlyContinue = false;     //  ReadOnlyファイルの確認無視
        private bool mReadOnlyRemove = false;       //  ReadOnlyファイル削除
        public bool mCallBackOn = false;            //  コールバック有り
        public Action callback;                     //  コールバック関数
        public string mFileDiffTool = "";           //  ファイル比較アプリ


        private YLib ylib = new YLib();
        private YFile yfile = new YFile();
        
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
            mSrcFolder = Path.GetDirectoryName(mSrcFiles[0]);
            mSrcFiles = getCopyFiles(mSrcFiles);
            if (mSrcFiles != null && 0 < mSrcFiles.Count && mDestFolder.Length > 0) {
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
                pbCopyCount.Minimum = 0;
                pbCopyCount.Maximum = mSrcFiles.Count;
                pbCopyCount.Value = 0;
            }
            if (mFileDiffTool == "")
                btCompare.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// OKボタン(コピーを許可して次に行く)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            if (mIndex < mSrcFiles.Count) {
                fileCopy(mSrcFiles[mIndex], mSrcFolder, mDestFolder, cbUpdate.IsChecked == true ? 0 : 2);
                mIndex++;
                pbCopyCount.Value++;
                while (cbCheckedMessage.IsChecked == false && mIndex < mSrcFiles.Count) {
                    setFileInfo(mSrcFiles[mIndex], mDestFolder);
                    fileCopy(mSrcFiles[mIndex],mSrcFolder, mDestFolder, cbUpdate.IsChecked == true ? 0 : 2);
                    mIndex++;
                    pbCopyCount.Value++;
                    if (mCallBackOn && (mIndex % 200) == 0)
                        callback();
                }
            }
            if (mIndex < mSrcFiles.Count)
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
            else {
                if (mCallBackOn)
                    callback();
                Close();
            }
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
            if (mIndex < mSrcFiles.Count)
                setFileInfo(mSrcFiles[mIndex], mDestFolder);
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
            fileCompre(mSrcFiles[mIndex], mDestFolder);
        }

        /// <summary>
        /// ファイルをアプリで比較
        /// </summary>
        /// <param name="srcPath">コピー元ファイル</param>
        /// <param name="destPath">コピー先ファイル</param>
        private void fileCompre(string srcPath, string destPath)
        {
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
        private void fileCopy(string srcPath, string srcFolder, string destFolder, int copyType)
        {
            string destPath = srcPath.Replace(srcFolder, destFolder);
            yfile.fileCopy(srcPath, destPath, copyType);     //  ディレクトリ自動作成でコピー
            if (mSrcRemove) {
                removeFile(srcPath);
            }
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
                } else if (File.Exists(path)) {
                    copyFiles.Add(path);
                }

            }
            return copyFiles;
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
        /// コピーファイルの情報表示
        /// </summary>
        /// <param name="srcpath">コピー元ファイルパス</param>
        /// <param name="destFolder">コピー先フォルダ</param>
        private void setFileInfo(string srcpath, string destFolder)
        {
            FileInfo srcFileInfo = new FileInfo(srcpath);
            tbFileName.Text = srcFileInfo.Name;
            tbSrcFolder.Text = srcFileInfo.DirectoryName;
            tbSrcDateTime.Text = srcFileInfo.CreationTime.ToString();
            tbSrcSize.Text = srcFileInfo.Length.ToString("N0");
            FileInfo destFileInfo = new FileInfo(Path.Combine(destFolder, srcFileInfo.Name));
            if (destFileInfo.Exists) {
                tbDestFolder.Text = destFileInfo.DirectoryName;
                tbDestDateTime.Text = destFileInfo.CreationTime.ToString();
                tbDestSize.Text = destFileInfo.Length.ToString("N0");
            } else {
                tbDestFolder.Text = mDestFolder;
                tbDestDateTime.Text = "";
                tbDestSize.Text = "";
            }
            tbProgress.Text = $"[ {mIndex} / {mSrcFiles.Count} ]";

            if (mFileDiffTool != "") {
                //  比較ボタン表示
                if (destFileInfo.Exists)
                    btCompare.Visibility = Visibility.Visible;
                else
                    btCompare.Visibility = Visibility.Collapsed;
            }
        }
    }
}
