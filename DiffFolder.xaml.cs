﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoreLib
{
    public class DiffFile
    {
        public string mFileName { get; set; }
        public string mRelPath { get; set; }
        public DateTime mSrcLastDate { get; set; }
        public long mSrcSize { get; set; }
        public ulong mSrcCrc { get; set; }
        public DateTime mDstLastDate { get; set; }
        public long mDstSize { get; set; }
        public ulong mDstCrc { get; set; }

        public DiffFile(string fileName, string relPath, DateTime srcLastDate, int srcSize, ulong srcCrc, DateTime dstLastDate, int dstSize, ulong dstCrc)
        {
            mFileName = fileName;
            mRelPath = relPath;
            mSrcLastDate = srcLastDate;
            mSrcSize = srcSize;
            mSrcCrc = srcCrc;
            mDstLastDate = dstLastDate;
            mDstSize = dstSize;
            mDstCrc = dstCrc;
        }

        public DiffFile(FilesData fileData)
        {
            mFileName = Path.GetFileName(fileData.mRelPath);
            mRelPath = Path.GetDirectoryName(fileData.mRelPath);
            mSrcLastDate = fileData.mSrcFile == null ? new DateTime() : fileData.mSrcFile.LastWriteTime;
            mSrcSize = fileData.mSrcFile == null ? 0 : fileData.mSrcFile.Length;
            mSrcCrc = fileData.mSrcCrc;
            mDstLastDate = fileData.mDstFile == null ? new DateTime() : fileData.mDstFile.LastWriteTime;
            mDstSize = fileData.mDstFile == null ? 0 : fileData.mDstFile.Length;
            mDstCrc = fileData.mDstCrc;
        }

        public string getPath()
        {
            return Path.Combine(mRelPath, mFileName);
        }

        public string getPath(string folder)
        {
            return Path.Combine(folder, Path.Combine(mRelPath, mFileName));
        }
    }

    /// <summary>
    /// DiffFolder.xaml の相互作用ロジック
    /// </summary>
    public partial class DiffFolder : Window
    {
        private double mWindowWidth;                            //  ウィンドウの高さ
        private double mWindowHeight;                           //  ウィンドウ幅
        private double mPrevWindowWidth;                        //  変更前のウィンドウ幅
        private WindowState mWindowState = WindowState.Normal;  //  ウィンドウの状態(最大化/最小化)

        public string mSrcFolder = ".";         //  比較元フォルダ
        public string mDestFolder = ".";        //  比較先フォルダ
        public string mSrcTitle = "";           //  比較元タイトル
        public string mDestTitle = "";          //  比較先タイトル

        public bool mDiffOnly = true;           //  差異ファイル表示と全ファイル表示の切替
        public bool mHashChk = true;            //  CRCによる差異表示/サイズと日付による再表示の切替

        public string mDiffTool = "";           //  ファイル比較ツール(WinMergeなど)

        private DirectoryDiff mDiffFolder;      //  フォルダ比較クラス
        private List<DiffFile> mDiffFileList;   //  比較結果リスト

        private YLib ylib = new YLib();

        public DiffFolder()
        {
            InitializeComponent();

            dgRemoveMenu.Visibility = Visibility.Visible;
            WindowFormLoad();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mHashChk) {
                //  HashCodeを使わない時
                dgColSrcCrc.Visibility = Visibility.Collapsed;  //  空白も非表示
                dgColDstCrc.Visibility = Visibility.Collapsed;
            }
            if (0 < mSrcTitle.Length)  tbSrcTitle.Text  = mSrcTitle;
            if (0 < mDestTitle.Length) tbDestTitle.Text = mDestTitle;
            tbSrcFolder.Text  = mSrcFolder;
            tbDestFolder.Text = mDestFolder;
            rbDiffFile.IsChecked = mDiffOnly;
            rbAllFile.IsChecked  = !mDiffOnly;

            setDiffFolder(mSrcFolder, mDestFolder);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowFormSave();
        }

        /// <summary>
        /// Windowの状態を前回の状態にする
        /// </summary>
        private void WindowFormLoad()
        {
            //  前回のWindowの位置とサイズを復元する(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.DiffFolderWindowWidth < 100 ||
                Properties.Settings.Default.DiffFolderWindowHeight < 100 ||
                SystemParameters.WorkArea.Height < Properties.Settings.Default.DiffFolderWindowHeight) {
                Properties.Settings.Default.DiffFolderWindowWidth = mWindowWidth;
                Properties.Settings.Default.DiffFolderWindowHeight = mWindowHeight;
            } else {
                this.Top = Properties.Settings.Default.DiffFolderWindowTop;
                this.Left = Properties.Settings.Default.DiffFolderWindowLeft;
                this.Width = Properties.Settings.Default.DiffFolderWindowWidth;
                this.Height = Properties.Settings.Default.DiffFolderWindowHeight;
            }
        }

        /// <summary>
        /// Window状態を保存する
        /// </summary>
        private void WindowFormSave()
        {
            //  Windowの位置とサイズを保存(登録項目をPropeties.settingsに登録して使用する)
            Properties.Settings.Default.DiffFolderWindowTop = Top;
            Properties.Settings.Default.DiffFolderWindowLeft = Left;
            Properties.Settings.Default.DiffFolderWindowWidth = Width;
            Properties.Settings.Default.DiffFolderWindowHeight = Height;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// [比較]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCompar_Click(object sender, RoutedEventArgs e)
        {
            setDiffFolder(mSrcFolder, mDestFolder);
        }

        /// <summary>
        /// [右へ更新]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRightUpdate_Click(object sender, RoutedEventArgs e)
        {
            selectCopy(mSrcFolder, mDestFolder);
            setDiffFolder(mSrcFolder, mDestFolder);
        }

        /// <summary>
        /// [左へ更新]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLeftUpdate_Click(object sender, RoutedEventArgs e)
        {
            selectCopy(mDestFolder, mSrcFolder);
            setDiffFolder(mSrcFolder, mDestFolder);
        }

        /// <summary>
        /// [終了]ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// [差異/全ファイル]ラジオボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDiffFile_Click(object sender, RoutedEventArgs e)
        {
            setDiffFolder(mSrcFolder, mDestFolder);
            //ColorRow(dgDiffFolder);
        }

        /// <summary>
        /// [比較ファイルリスト]ダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDiffFolder_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = dgDiffFolder.SelectedIndex;
            if (0 <= index && 0 < mDiffTool.Length) {
                DiffFile fileData = (DiffFile)dgDiffFolder.Items[index];
                string srcPath = fileData.getPath(mSrcFolder);
                string destPath = fileData.getPath(mDestFolder);
                if (File.Exists(srcPath) && File.Exists(destPath))
                    ylib.processStart(mDiffTool, $"\"{srcPath}\" \"{destPath}\"");
            }
        }

        /// <summary>
        /// 選択したファイルのみを更新(コピー)
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="destFolder"></param>
        private void selectCopy(string srcFolder, string destFolder)
        {
            IList selItems = dgDiffFolder.SelectedItems;
            int copyType = cbOverWriteForce.IsChecked == true ? 2 : 0;
            if (0 < selItems.Count) {
                pbCopyCount.Minimum = 0;
                pbCopyCount.Maximum = selItems.Count;
                pbCopyCount.Value = 0;
                if (ylib.messageBox(this.Owner, $"{srcFolder} から\n{destFolder} に\n{selItems.Count} ファイル コピーします",
                    "", "確認", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                    foreach (DiffFile fileData in selItems) {
                        string srcPath = fileData.getPath(srcFolder);
                        string destPath = fileData.getPath(destFolder);
                        if (File.Exists(srcPath))
                            ylib.fileCopy(srcPath, destPath, copyType);
                        pbCopyCount.Value++;
                    }
                }
            }
        }

        /// <summary>
        /// フォルダの比較と結果の表示
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="dstFolder"></param>
        private void setDiffFolder(string srcFolder, string dstFolder)
        {
            mDiffFolder = new DirectoryDiff(srcFolder, dstFolder, mHashChk);

            if (rbDiffFile.IsChecked == true) {
                //  同一ファイルを除く
                mDiffFolder.stripSameFile();
            }

            if (mDiffFileList == null)
                mDiffFileList = new List<DiffFile>();
            mDiffFileList.Clear();
            //dgDiffFolder.Items.Clear();
            foreach (FilesData filesData in mDiffFolder.mFiles) {
                mDiffFileList.Add(new DiffFile(filesData));
                //dgDiffFolder.Items.Add(new DiffFile(filesData));
            }
            dgDiffFolder.ItemsSource = new ReadOnlyCollection<DiffFile>(mDiffFileList);
        }

        /// <summary>
        /// 行ごとに色設定(あまりうまくいかない)
        /// </summary>
        /// <param name="dg"></param>
        private void ColorRow(DataGrid dg)
        {
            for (int i = 0; i < dg.Items.Count; i++) {
                DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(i);
                if (row != null) {
                    int index = row.GetIndex();
                    if (index % 2 == 0) {
                        SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(100, 255, 104, 0));
                        row.Background = brush;
                    } else {
                        SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(100, 255, 232, 0));
                        row.Background = brush;
                    }
                }
            }
        }

        /// <summary>
        /// コンテキストメニューからの選択ファイル操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgOpeMune_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            IList selItems = dgDiffFolder.SelectedItems;
            if (selItems.Count == 0)
                return;
            DiffFile selFileData = (DiffFile)dgDiffFolder.Items[dgDiffFolder.SelectedIndex];
            if (menuItem.Name.CompareTo("dgSrcRemoveMune") == 0) {
                if (ylib.messageBox(this, $"比較元 {selItems.Count}ファイルを削除します", "", "確認",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    foreach (DiffFile fileData in selItems) {
                        string srcPath = fileData.getPath(mSrcFolder);
                        if (File.Exists(srcPath))
                            File.Delete(srcPath);
                    }
                }
            } else if (menuItem.Name.CompareTo("dgDestRemoveMenu") == 0) {
                if (ylib.messageBox(this, $"比較先 {selItems.Count}ファイルを削除します", "", "確認",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    foreach (DiffFile fileData in selItems) {
                        string destPath = fileData.getPath(mDestFolder);
                        if (File.Exists(destPath))
                            File.Delete(destPath);
                    }
                }
            } else if (menuItem.Name.CompareTo("dgBothRemoveMenu") == 0) {
                if (ylib.messageBox(this, $"両方の {selItems.Count}ファイルを削除します", "", "確認",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    foreach (DiffFile fileData in selItems) {
                        string srcPath = fileData.getPath(mSrcFolder);
                        string destPath = fileData.getPath(mDestFolder);
                        if (File.Exists(srcPath))
                            File.Delete(srcPath);
                        if (File.Exists(destPath))
                            File.Delete(destPath);
                    }
                }
            } else if (menuItem.Name.CompareTo("dgSrcOpenMenu") == 0) {
                string srcPath = selFileData.getPath(mSrcFolder);
                if (File.Exists(srcPath))
                    ylib.openUrl(srcPath);
            } else if (menuItem.Name.CompareTo("dgDestOpenMenu") == 0) {
                string destPath = selFileData.getPath(mDestFolder);
                if (File.Exists(destPath))
                    ylib.openUrl(destPath);
            } else if (menuItem.Name.CompareTo("dgBothOpenMenu") == 0) {
                string srcPath = selFileData.getPath(mSrcFolder);
                string destPath = selFileData.getPath(mDestFolder);
                if (File.Exists(srcPath))
                    ylib.openUrl(srcPath);
                if (File.Exists(destPath))
                    ylib.openUrl(destPath);
            }
            setDiffFolder(mSrcFolder, mDestFolder);
        }
    }
}
