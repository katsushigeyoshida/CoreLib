using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CoreLib
{
    //  
    //  ------------  ダイヤログ  ---------------------------
    //	string folderSelect(string title, string initFolder)	                        フォルダの選択ダイヤログの表示
    //	string fileOpenSelectDlg(string title, string initDir, List<string[]> filters)	ファイル読込選択ダイヤログ表示
    //	string fileSaveSelectDlg(string title, string initDir, List<string[]> filters)	ファイル保存選択ダイヤログ表示
    //	string consoleFileSelect(string folder, string fname)	                        Console用ファイル選択
    //  
    //  --------------  ディレクトリ/ファイル属性処理   --------------------
    //	DateTime getFileDateTime(string path)	                                        ファイルの日付(最終変更日)の取得
    //	void setFileDateTime(string path, DateTime dateTime)	                        ファイル日付(最終変更日)の設定
    //	string FileSize2Str(long filesize)	                                            数値を○K,○M,○G の文字列に変換する
    //	string FileAttribute2Str(FileAttributes atr)	                                ファイル属性を文字列にする(ACDEHORST)
    //  
    //  -------------------  ファイル操作  ------------------------
    //	bool makeDir(string path, bool fileName2folder = false)	                        ファイルパスからディレクトリを作成
    //	void copyDrectory(string srcDir, string destDir)	                            ディレクトリをコピー
    //	void fileCopy(string srcFile, string destFile, int copyType)	                ファイルのコピー(renameも含む)
    //	bool gzipDecompress(string ipath, string opath)	                                gzipファイルを解凍する
    //  
    //  -----------------  ファイル・ディレクトリ検索  ----------------
    //	string[] getFiles(string path, bool recursive = false)	                        指定されたパスからファイルリストを作成
    //	List<FileInfo> getFilesInfo(string path, bool recursive = false)	            ファイルを検索してFileInfoリストを作成
    //	List<DirectoryInfo> getDirectoriesInfo(string path)	                            ディレクトリ情報リストの取得
    //	List<string> getDirectories(string path, bool all = true)	                    指定されたパスからディレクトリリストを作成
    //	List<string> getFilesDirectories(string folder, string fileName)	            フォルダとファイルの一覧を取得
    //	List<FileInfo> getDirectoriesInfo(string folder, string fileName = "*")	        ファイル情報検索(サブディレクトリを含む)
    //	List<DirectoryInfo> getDrivesInfo()	                                            ドライブの一覧取得
    //	List<string> getDrives()	                                                    ドライブの一覧を取得する
    //  
    //  ---------  ファイルの読書き操作  ----------------------------
    //	List<string[]> loadCsvData(string filePath, string[] title, bool firstTitle = false, bool comment = true)	 CSV形式のファイルを読み込みList<String[]>形式で出力する
    //	List<string[]> loadCsvData(string filePath, bool tabSep = false, bool comment = true)	                    CSV形式のファイルを読み込む
    //	void saveCsvData(string path, string[] format, List<string[]> data, List<string> comment = null)	        タイトルをつけてCSV形式でListデータをファイルに保存
    //	void saveCsvData(string path, List<string[]> csvData, List<string> comment = null)	 データをCSV形式でファイルに書き込む
    //	void saveListData(string path, List<string> data)	                            リストデータをファイルに書き込む
    //	string arrayStr2CsvData(string[] data)	                                        配列データをCSVデータに変換
    //	string[] csvData2ArrayStr(string line)	                                        CSVデータを配列データに戻す
    //	List<string[]> loadJsonData(string filePath)	                                JSON形式のファイルを読み込む
    //	List<string> loadListData(string filePath)	                                    ファイルを読み込んでリストデータにする
    //	string loadTextFile(string path)	                                            テキストファイルの読込
    //	void saveTextFile(string path, string buffer)	                                テキストファイルの保存
    //	byte[] loadBinData(string path, int size = 0)	                                バイナリファイルの読込
    //	void saveBinData(string path, byte[] buffer)	                                バイナリデータをファイルに書き込む

    public class YFile
    {
        private Encoding[] mEncoding;
        public int mEncordingType = 0;
        public bool mError = false;
        public string mErrorMessage = "";

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YFile()
        {
            //  エンコーディングのための設定
            //  .Net CoreではEncoding.RegisterProviderのプロバイダー登録が必要、ないとshift_jisでエラーとなる
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            mEncoding = new Encoding[] {
                Encoding.UTF8, Encoding.GetEncoding("shift_jis"), Encoding.GetEncoding("euc-jp")
            };
        }

        //  ------------  ダイヤログ  ---------------------------

        /// <summary>
        /// フォルダの選択ダイヤログの表示
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="initFolder">初期ディレクトリ</param>
        /// <returns>フォルダパス</returns>
        public string folderSelect(string title, string initFolder)
        {
            var dlg = new CommonOpenFileDialog {
                Title = title,
                IsFolderPicker = true,
                InitialDirectory = initFolder,
                DefaultDirectory = initFolder,
            };
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                return dlg.FileName;
            } else {
                return "";
            }
        }

        /// <summary>
        /// ファイル読込選択ダイヤログ表示
        /// フィルタ例  ("CSVファイル", "*.csv;*.csv")("すべてのファイル", "*.*")
        /// Nuget で WindowsAPICodePack をインストール
        ///  CommonOpenFileDialogの使い方
        ///  参照 https://kuttsun.blogspot.com/2017/11/blog-post.html
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="initDir">初期ディレクトリ</param>
        /// <param name="filters">フィルタリスト</param>
        /// <returns>ファイルパス</returns>
        public string fileOpenSelectDlg(string title, string initDir, List<string[]> filters)
        {
            var dlg = new CommonOpenFileDialog {
                Title = title,
                InitialDirectory = initDir,
                DefaultDirectory = initDir
            };
            foreach (string[] filter in filters)
                dlg.Filters.Add(new CommonFileDialogFilter(filter[0], filter[1]));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                return dlg.FileName;
            } else {
                return "";
            }
        }

        /// <summary>
        /// ファイル保存選択ダイヤログ表示
        /// フィルタ例  ("CSVファイル", "*.csv;*.csv")("すべてのファイル", "*.*")
        /// Nuget で WindowsAPICodePack をインストール
        ///  CommonOpenFileDialogの使い方
        ///  参照 https://kuttsun.blogspot.com/2017/11/blog-post.html
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="initDir">初期ディレクトリ</param>
        /// <param name="filters">フィルタリスト</param>
        /// <returns>ファイルパス</returns>
        public string fileSaveSelectDlg(string title, string initDir, List<string[]> filters = null)
        {
            if (filters == null) {
                filters = new List<string[]> { new string[] { "すべてのファイル", "*.*" } };
            }
            var dlg = new CommonSaveFileDialog {
                Title = title,
                InitialDirectory = initDir,
                DefaultDirectory = initDir
            };
            foreach (string[] filter in filters)
                dlg.Filters.Add(new CommonFileDialogFilter(filter[0], filter[1]));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                return dlg.FileName;
            } else {
                return "";
            }
        }

        /// <summary>
        /// Console用ファイル選択
        /// </summary>
        /// <param name="folder">検索先フォルダ</param>
        /// <param name="fname">検索ファイル名(*.fit)</param>
        /// <returns>選択ファイルパス</returns>
        public string consoleFileSelect(string folder, string fname)
        {
            List<string> files;
            if (folder == null) {
                files = getDrives();
            } else {
                folder = Path.GetFullPath(folder);
                files = getFilesDirectories(folder, fname);
                files.Insert(0, "..");
            }
            for (int i = 0; i < files.Count; i++) {
                if (Directory.Exists(files[i])) {
                    Console.WriteLine($"{i}: [{files[i]}]");
                } else {
                    Console.WriteLine($"{i}: {files[i]}");
                }
            }
            Console.Write("番号入力 >> ");
            string inp = Console.ReadLine();
            int n = 0;
            if (!int.TryParse(inp, out n))
                return "";
            if (files.Count <= n)
                return "";
            if (Directory.Exists(files[n])) {
                if (files[int.Parse(inp)].CompareTo("..") == 0) {
                    folder = Path.GetDirectoryName(folder);
                } else
                    folder = files[int.Parse(inp)];
                return consoleFileSelect(folder, fname);
            }
            return files[n];
        }


        //  --------------  ディレクトリ/ファイル属性処理   --------------------

        /// <summary>
        /// ファイルの日付(最終変更日)の取得
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>DateTime</returns>
        public DateTime getFileDateTime(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.LastWriteTime;
        }

        /// <summary>
        /// ファイル日付(最終変更日)の設定
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="dateTime">DateTime</param>
        public void setFileDateTime(string path, DateTime dateTime)
        {
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.LastWriteTime = dateTime;
        }

        /// <summary>
        /// 数値を○K,○M,○G の文字列に変換する
        /// </summary>
        /// <param name="filesize"></param>
        /// <returns></returns>
        public string FileSize2Str(long filesize)
        {
            string buf;
            if (0 < (filesize >> 40)) {
                if ((filesize >> 40) < 10) {
                    buf = string.Format("{0}.{1:d2}T",
                        (filesize >> 40), ((filesize & 0xffffffffff) * 100) >> 40);
                } else {
                    buf = string.Format("{0}.{1:d1}T",
                        (filesize >> 40), ((filesize & 0xffffffffff) * 10) >> 40);
                }
            } else if (0 < (filesize >> 30)) {
                if ((filesize >> 30) < 10) {
                    buf = string.Format("{0}.{1:d2}G",
                        (filesize >> 30), ((filesize & 0x3fffffff) * 100) >> 30);
                } else {
                    buf = string.Format("{0}.{1:d1}G",
                        (filesize >> 30), ((filesize & 0x3fffffff) * 10) >> 30);
                }
            } else if (0 < (filesize >> 20)) {
                if ((filesize >> 20) < 10) {
                    buf = string.Format("{0}.{1:d2}M",
                        (filesize >> 20), ((filesize & 0xfffff) * 100) >> 20);
                } else {
                    buf = string.Format("{0}.{1:d1}M",
                        (filesize >> 20), ((filesize & 0xfffff) * 10) >> 20);
                }
            } else if (0 < (filesize >> 10)) {
                if ((filesize >> 10) < 10) {
                    buf = string.Format("{0}.{1:d2}K",
                        (filesize >> 10), ((filesize & 0x3ff) * 100) >> 10);
                } else {
                    buf = string.Format("{0}.{1:d1}K",
                        (filesize >> 10), ((filesize & 0x3ff) * 10) >> 10);
                }
            } else
                buf = filesize.ToString();
            return buf;
        }

        /// <summary>
        /// ファイル属性を文字列にする(ACDEHORST)
        /// </summary>
        /// <param name="atr">属性</param>
        /// <returns></returns>
        public string FileAttribute2Str(FileAttributes atr)
        {
            string buf;
            buf = ((atr & FileAttributes.Archive) != 0) ? "A" : "_";
            buf += ((atr & FileAttributes.Compressed) != 0) ? "C" : "_";
            buf += ((atr & FileAttributes.Directory) != 0) ? "D" : "_";
            buf += ((atr & FileAttributes.Encrypted) != 0) ? "E" : "_";
            buf += ((atr & FileAttributes.Hidden) != 0) ? "H" : "_";
            buf += ((atr & FileAttributes.Offline) != 0) ? "O" : "_";
            buf += ((atr & FileAttributes.ReadOnly) != 0) ? "R" : "_";
            buf += ((atr & FileAttributes.System) != 0) ? "S" : "_";
            buf += ((atr & FileAttributes.Temporary) != 0) ? "T" : "_";
            return buf;
        }

        //  -------------------  ファイル操作  ------------------------

        /// <summary>
        /// ファイルパスからディレクトリを作成
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>成否</returns>
        public bool makeDir(string path, bool fileName2folder = false)
        {
            try {
                string folder = fileName2folder ? Path.GetDirectoryName(path) : path;
                if (0 < folder.Length && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

            } catch (Exception e) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ディレクトリをコピー
        /// srcDir以下のファイルとディレクトリをdestDir以下にコピーする
        /// </summary>
        /// <param name="srcDir">コピー元ディレクトリ</param>
        /// <param name="destDir">コピー先ディレクトリ</param>
        public void copyDrectory(string srcDir, string destDir)
        {
            if (!Directory.Exists(destDir)) {
                Directory.CreateDirectory(destDir);
            }
            File.SetAttributes(destDir, File.GetAttributes(srcDir));
            if (!destDir.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                destDir = destDir + Path.DirectorySeparatorChar;
            }

            string[] files = Directory.GetFiles(srcDir);
            foreach (string file in files) {
                File.Copy(file, destDir + Path.GetFileName(file), true);
            }

            string[] dirs = Directory.GetDirectories(srcDir);
            foreach (string dir in dirs) {
                copyDrectory(dir, destDir + Path.GetFileName(dir));
            }
        }

        /// <summary>
        /// ファイルのコピー(renameも含む)
        /// copyType: 0:update, 1: != date | != size, 2:force, 3:new(dest not existe)
        /// </summary>
        /// <param name="srcFile">コピー元ファイルパス</param>
        /// <param name="destFile">コピー先ファイルパス</param>
        /// <param name="copyType">コピー条件</param>
        public void fileCopy(string srcFile, string destFile, int copyType)
        {
            FileInfo sf = new FileInfo(srcFile);
            FileInfo df = new FileInfo(destFile);
            if (df.Exists && df.IsReadOnly)
                df.IsReadOnly = false;
            if ((copyType == 0 && sf.LastWriteTime > df.LastWriteTime) ||
                (copyType == 1 && (sf.LastWriteTime != df.LastWriteTime || sf.Length != df.Length)) ||
                (copyType == 2) ||
                (copyType == 3 && !df.Exists)
                ) {
                if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                if (File.Exists(srcFile))
                    File.Copy(srcFile, destFile, true);
            }
        }

        /// <summary>
        /// Zipファイルの解凍
        /// </summary>
        /// <param name="zipPath">Zipファイルパス</param>
        /// <param name="extractFolder">解凍先フォルダ</param>
        /// <param name="overWrite">上書きの有無</param>
        /// <returns>解凍結果</returns>
        public bool zipExtract(string zipPath, string extractFolder, bool overWrite = false)
        {
            try {
                ZipFile.ExtractToDirectory(zipPath, extractFolder, overWrite);
                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.Write($"zipExtract: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// フャルダまたはファイルをZipファイルに圧縮する
        /// </summary>
        /// <param name="srcPath">ファイル/フォルダ名</param>
        /// <param name="targetFolder">圧縮先フォルダ</param>
        public bool zipCompress(string srcPath, string targetFolder, bool overWrite = false)
        {
            mError = false;
            try {
                string zipFilePath = Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(srcPath) + ".zip");

                if (File.Exists(zipFilePath)) {
                    if (overWrite) {
                        File.Delete(zipFilePath);
                    } else
                        return false;
                }
                if (Directory.Exists(srcPath)) {
                    ZipFile.CreateFromDirectory(srcPath, zipFilePath);
                } else if (File.Exists(srcPath)) {
                    fileZipCompress(srcPath, zipFilePath);
                } else
                    return false;
                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"zipCompress: {e.Message}");
                mError = true;
                mErrorMessage = e.Message;
                return false;
            }
        }

        /// <summary>
        /// ファイルをZipファイルに圧縮する
        /// </summary>
        /// <param name="srcPath">ファイルパス</param>
        /// <param name="zipPath">圧縮ファイルパス</param>
        public void fileZipCompress(string srcPath, string zipPath)
        {
            using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Create)) {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update)) {
                    // 新しいファイルをZIPに追加
                    archive.CreateEntryFromFile(srcPath, Path.GetFileName(srcPath));
                }
            }
        }

        /// <summary>
        /// Zipファイルにファイルを追加する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="zipPath">圧縮ファイルパス</param>
        public void zipAddCompress(string filePath, string zipPath)
        {
            using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Open)) {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update)) {
                    // 新しいファイルをZIPに追加
                    archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                }
            }
        }

        /// <summary>
        /// gzipファイルを解凍する
        /// 拡張子が gz でない場合はリネームしてから解凍
        /// </summary>
        /// <param name="ipath">gzファイル名</param>
        /// <param name="opath">出力ファイル名</param>
        /// <returns>可否</returns>
        public bool gzipDecompress(string ipath, string opath)
        {
            if (!File.Exists(ipath))
                return false;
            //  gzipファイル化の確認
            byte[] header = loadBinData(ipath, 2);
            if (header[0] != 0x1f || header[1] != 0x8b)
                return false;
            string inpath = ipath;
            if (!ipath.ToLower().EndsWith(".gz")) {
                inpath = Path.Combine(Path.GetDirectoryName(ipath), Path.GetFileNameWithoutExtension(ipath) + ".gz");
                if (File.Exists(inpath))
                    File.Delete(inpath);
                File.Move(ipath, inpath);
            }

            int num;
            byte[] buf = new byte[1024];                // 1Kbytesずつ処理する

            FileStream inStream                         // 入力ストリーム
              = new FileStream(inpath, FileMode.Open, FileAccess.Read);

            GZipStream decompStream                     // 解凍ストリーム
              = new GZipStream(
                inStream,                               // 入力元となるストリームを指定
                CompressionMode.Decompress);            // 解凍（圧縮解除）を指定

            FileStream outStream                        // 出力ストリーム
              = new FileStream(opath, FileMode.Create);

            using (inStream)
            using (outStream)
            using (decompStream) {
                while ((num = decompStream.Read(buf, 0, buf.Length)) > 0) {
                    outStream.Write(buf, 0, num);
                }
            }
            return true;
        }

        //  -----------------  ファイル・ディレクトリ検索  ----------------

        /// <summary>
        /// 指定されたパスからファイルリストを作成
        /// 指定されたpathからフォルダ名とワイルドカードのファイル名に分けて
        /// ファイルリストを取得する
        /// 例 path = D:\folder\*.flac
        /// </summary>
        /// <param name="path">パス名</param>
        /// <param name="recursive">再帰検索</param>
        /// <returns>ファイルリスト</returns>
        public string[] getFiles(string path, bool recursive = false)
        {
            List<string> files = new List<string>();
            try {
                string folder = Path.GetDirectoryName(path);
                string ext = Path.GetFileName(path);
                if (recursive) {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string dir in folders) {
                        string[] filesList = getFiles(Path.Combine(dir, ext), recursive);
                        if (filesList != null)
                            files.AddRange(filesList);
                    }
                    string[] fileList = Directory.GetFiles(folder, ext);
                    if (fileList != null)
                        files.AddRange(fileList);
                    return files.ToArray();
                } else
                    return Directory.GetFiles(folder, ext);
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine("getFiles: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// ファイルを検索してFileInfoリストを作成
        /// </summary>
        /// <param name="path">パス(フォルダ+拡張子)</param>
        /// <param name="recursive">再帰検索</param>
        /// <returns></returns>
        public List<FileInfo> getFilesInfo(string path, bool recursive = false)
        {
            List<FileInfo> files = new List<FileInfo>();
            try {
                string folder = Path.GetDirectoryName(path);
                string ext = Path.GetFileName(path);
                if (recursive) {
                    //  再帰検索
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (string dir in folders) {
                        List<FileInfo> filesList = getFilesInfo(Path.Combine(dir, ext), recursive);
                        if (filesList != null)
                            files.AddRange(filesList);
                    }
                    string[] fileList = Directory.GetFiles(folder, ext);
                    if (fileList != null) {
                        foreach (string file in fileList)
                            files.Add(new FileInfo(file));
                    }
                } else {
                    //  ファイル検索
                    string[] fileList = Directory.GetFiles(folder, ext);
                    if (fileList != null) {
                        foreach (string file in fileList)
                            files.Add(new FileInfo(file));
                    }
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"getFilesInfo: {e.Message}");
            }
            return files;
        }

        /// <summary>
        /// ディレクトリ情報リストの取得
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>ディレクトリ情報リスト</returns>
        public List<DirectoryInfo> getDirectoriesInfo(string path)
        {
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            try {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (DirectoryInfo dirInfo in di.GetDirectories()) {
                    dirList.Add(dirInfo);
                }
                return dirList;
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// 指定されたパスからディレクトリリストを作成
        /// </summary>
        /// <param name="path">パス名</param>
        /// <param name="all">全属性/Normal属性</param>
        /// <returns>ディレクトリリスト</returns>
        public List<string> getDirectories(string path, bool all = true)
        {
            List<string> dirList = new List<string>();
            try {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (DirectoryInfo dir in di.GetDirectories()) {
                    if (all || (dir.Attributes & FileAttributes.Hidden) == 0)
                        dirList.Add(dir.FullName);
                }
                return dirList;
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// フォルダとファイルの一覧を取得
        /// </summary>
        /// <param name="folder">フォルダ</param>
        /// <param name="fileName">ファイル名(*.ext)</param>
        /// <returns></returns>
        public List<string> getFilesDirectories(string folder, string fileName)
        {
            List<string> fileDirList = new List<string>();
            try {
                DirectoryInfo di = new DirectoryInfo(folder);
                foreach (DirectoryInfo dir in di.GetDirectories()) {
                    fileDirList.Add(dir.FullName);
                }
                string[] files = Directory.GetFiles(folder, fileName);
                foreach (var file in files) {
                    fileDirList.Add(file);
                }
                return fileDirList;
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// ファイル情報検索(サブディレクトリを含む)
        /// </summary>
        /// <param name="folder">検索フォルダ</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> getDirectoriesInfo(string folder, string fileName = "*")
        {
            List<FileInfo> fileList = new List<FileInfo>();
            try {
                DirectoryInfo di = new DirectoryInfo(folder);
                foreach (DirectoryInfo dir in di.GetDirectories()) {
                    List<FileInfo> filesInfo = getDirectoriesInfo(dir.FullName, fileName);
                    if (filesInfo != null) {
                        fileList.AddRange(filesInfo);
                    }
                }
                string[] files = Directory.GetFiles(folder, fileName);
                foreach (var file in files) {
                    FileInfo fi = new FileInfo(file);
                    if (fi != null)
                        fileList.Add(fi);
                }
                return fileList;
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// ドライブの一覧取得
        /// </summary>
        /// <returns></returns>
        public List<DirectoryInfo> getDrivesInfo()
        {
            string[] drives = Directory.GetLogicalDrives();
            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            foreach (string drive in drives)
                dirs.Add(new DirectoryInfo(drive));
            return dirs;
        }

        /// <summary>
        /// ドライブの一覧を取得する
        /// </summary>
        /// <returns></returns>
        public List<string> getDrives()
        {
            string[] drives = Directory.GetLogicalDrives();
            return drives.ToList();
        }


        //  ---------  ファイルの読書き操作  ----------------------------

        /// <summary>
        /// CSV形式のファイルを読み込みList<String[]>形式で出力する
        /// title[]が指定されて一行目にタイトルが入っていればタイトルの順番に合わせて取り込む
        /// titleがnullであればそのまま配列に置き換える
        /// </summary>
        /// <param name="filePath">ファイル名パス</param>
        /// <param name="title">タイトルの配列</param>
        /// <param name="firstTitle">1列目の整合性確認</param>
        /// <param name="comment">false:文字列の先頭が'#'だと取得しない</param>
        /// <returns>取得データ(タイトル行なし)</returns>
        public List<string[]> loadCsvData(string filePath, string[] title, bool firstTitle = false, bool comment = true)
        {
            //	ファイルデータの取り込み
            List<string[]> fileData = loadCsvData(filePath, false, comment);
            if (fileData == null)
                return null;
            if (fileData.Count == 0)
                return fileData;

            //	フォーマットの確認(タイトル行の展開)
            int start = 1;
            int[] titleNo = new int[title.Length];
            if (title != null && 0 < fileData.Count) {
                if (fileData[0][0].CompareTo(title[0]) == 0 || firstTitle) {
                    //	データの順番を求める
                    for (int n = 0; n < title.Length; n++) {
                        titleNo[n] = -1;
                        for (int m = 0; m < fileData[0].Length; m++) {
                            if (title[n].CompareTo(fileData[0][m]) == 0) {
                                titleNo[n] = m;
                                break;
                            }
                        }
                    }
                    start = 1;
                } else {
                    //  タイトルがない場合そのまま順で追加
                    for (int n = 0; n < title.Length; n++)
                        titleNo[n] = n;
                    start = 0;
                }
            } else {
                return null;
            }

            //  CSVデータを配列にしてListに登録
            List<string[]> listData = new List<string[]>();
            for (int i = start; i < fileData.Count; i++) {
                //  指定のタイトル順に並べ替えて追加
                string[] buffer = new string[title.Length];
                for (int n = 0; n < title.Length; n++) {
                    if (0 <= titleNo[n] && titleNo[n] < fileData[i].Length) {
                        buffer[n] = fileData[i][titleNo[n]];
                    } else {
                        buffer[n] = "";
                    }
                }
                listData.Add(buffer);
            }
            return listData;
        }

        /// <summary>
        /// CSV形式のファイルを読み込む
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="tabSep">Tabセパレート</param>
        /// <param name="comment">false: 文字列の先頭が'#'だと取得しない</param>
        /// <returns>データリスト</returns>
        public List<string[]> loadCsvData(string filePath, bool tabSep = false, bool comment = true)
        {
            List<string[]> csvData = new List<string[]>();
            if (!File.Exists(filePath))
                return csvData;
            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (StreamReader dataFile = new StreamReader(filePath, mEncoding[mEncordingType])) {
                        csvData.Clear();
                        string line;
                        while ((line = dataFile.ReadLine()) != null) {
                            if (!comment && 0 < line.Length && line[0] == '#')
                                continue;
                            string[] buf;
                            if (tabSep) {
                                buf = line.Split('\t');
                            } else {
                                buf = ylib.seperateString(line);
                            }
                            if (0 < buf.Length)
                                csvData.Add(buf);
                        }
                        //dataFile.Close(); //  usingの場合は不要 Disposeを含んでいる
                    }
                    return csvData;
                }
            } catch (Exception e) {
                mError = true;
                mErrorMessage = e.Message;
                return null;
            }
        }

        /// <summary>
        /// タイトルをつけてCSV形式でListデータをファイルに保存
        /// コメントリストが設定されている場合、ファイルの先頭に# commentの形式で出力される
        /// </summary>
        /// <param name="path">ファイル名パス</param>
        /// <param name="format">タイトル列</param>
        /// <param name="data">Listデータ</param>
        /// <param name="comment">コメントリスト</param>
        public void saveCsvData(string path, string[] format, List<string[]> data, List<string> comment = null)
        {
            List<string[]> dataList = new List<string[]>();
            dataList.Add(format);
            foreach (string[] v in data)
                dataList.Add(v);

            saveCsvData(path, dataList, comment);
        }

        /// <summary>
        /// データをCSV形式でファイルに書き込む
        /// コメントリストが設定されている場合、ファイルの先頭に# commentの形式で出力される
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="csvData">データリスト</param>
        /// <param name="comment">コメントリスト</param>
        public void saveCsvData(string path, List<string[]> csvData, List<string> comment = null)
        {
            if (0 < csvData.Count) {
                string folder = Path.GetDirectoryName(path);
                if (0 < folder.Length && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (StreamWriter dataFile = new StreamWriter(path, false, mEncoding[mEncordingType])) {
                    if (comment != null) {
                        foreach (var buf in comment)
                            dataFile.WriteLine("# " + buf);
                    }
                    foreach (string[] data in csvData) {
                        string buf = "";
                        for (int i = 0; i < data.Length; i++) {
                            data[i] = data[i] == null ? "" : data[i].Replace("\"", "\\\"");
                            buf += "\"" + data[i] + "\"";
                            if (i != data.Length - 1)
                                buf += ",";
                        }
                        dataFile.WriteLine(buf);
                    }
                    //dataFile.Close(); //  usingの場合は不要 Disposeを含んでいる
                }
            }
        }

        /// <summary>
        /// リストデータをファイルに書き込む
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="data">リストデータ</param>
        public void saveListData(string path, List<string> data)
        {
            if (data == null || data.Count == 0)
                return;
            string folder = Path.GetDirectoryName(path);
            if (0 < folder.Length && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            using (StreamWriter dataFile = new StreamWriter(path, false, mEncoding[mEncordingType])) {
                foreach (string buf in data) {
                    dataFile.WriteLine(buf);
                }
                //dataFile.Close(); //  usingの場合は不要 Disposeを含んでいる
            }
        }

        /// <summary>
        /// JSON形式のファイルを読み込む
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>データリスト</returns>
        public List<string[]> loadJsonData(string filePath)
        {
            List<string[]> jsonList = new List<string[]>();
            if (!File.Exists(filePath)) {
                mError = true;
                mErrorMessage = filePath + " のファイルが存在しません";
                return null;
            }
            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (System.IO.StreamReader dataFile = new StreamReader(filePath,
                       mEncoding[mEncordingType])) {
                        jsonList.Clear();
                        string line;
                        while ((line = dataFile.ReadLine()) != null) {
                            List<string[]> jsonData = ylib.splitJson(ylib.getJsonDataString(line));
                            if (0 < jsonData.Count) {
                                if (jsonList.Count == 0) {
                                    string[] buf = new string[jsonData.Count];
                                    for (int i = 0; i < jsonData.Count; i++)
                                        buf[i] = jsonData[i][0];
                                    jsonList.Add(buf);
                                    buf = new string[jsonData.Count];
                                    for (int i = 0; i < jsonData.Count; i++)
                                        buf[i] = jsonData[i][1];
                                    jsonList.Add(buf);
                                } else {
                                    string[] buf = new string[jsonData.Count];
                                    for (int i = 0; i < jsonData.Count; i++)
                                        buf[i] = jsonData[i][1];
                                    jsonList.Add(buf);
                                }
                            }
                        }
                        //dataFile.Close(); //  usingの場合は不要 Disposeを含んでいる
                    }
                }
            } catch (Exception e) {
                mError = true;
                mErrorMessage = e.Message;
                return null;
            }

            return jsonList;
        }

        /// <summary>
        /// ファイルを読み込んでリストデータにする
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>リストデータ</returns>
        public List<string> loadListData(string filePath)
        {
            List<string> listData = new List<string>();
            if (!File.Exists(filePath))
                return listData;
            try {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (StreamReader dataFile = new StreamReader(filePath, mEncoding[mEncordingType])) {
                        string line;
                        while ((line = dataFile.ReadLine()) != null) {
                            if (0 < line.Length)
                                listData.Add(line);
                        }
                    }
                    return listData;
                }
            } catch (Exception e) {
                mError = true;
                mErrorMessage = e.Message;
                return null;
            }
        }


        /// <summary>
        /// テキストファイルの読込
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>ファイルデータ</returns>
        public string loadTextFile(string path)
        {
            string buffer = "";
            if (File.Exists(path)) {
                StreamReader sr = new StreamReader(path, mEncoding[mEncordingType]);
                buffer = sr.ReadToEnd();
                sr.Close();
            }
            return buffer;
        }

        /// <summary>
        /// テキストファイルの保存
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="buffer">ファイルデータ</param>
        public void saveTextFile(string path, string buffer)
        {
            string folder = Path.GetDirectoryName(path);
            if (0 < folder.Length && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            StreamWriter sw = new StreamWriter(path, false, mEncoding[mEncordingType]);
            sw.Write(buffer);
            sw.Close();
        }

        /// <summary>
        /// バイナリファイルの読込
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="size">読込サイズ(省略可)</param>
        /// <returns>読込データ</returns>
        public byte[] loadBinData(string path, int size = 0)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
                int ret;
                FileInfo fi = new FileInfo(path);
                size = size == 0 ? (int)fi.Length : Math.Min(size, (int)fi.Length);
                byte[] buf = new byte[size];
                ret = reader.Read(buf, 0, size);
                return buf;
            }
        }

        /// <summary>
        /// バイナリデータをファイルに書き込む
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="buffer">バイナリデータ</param>
        public void saveBinData(string path, byte[] buffer)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(path))) {
                // 書き込み
                writer.Write(buffer);
            }
        }
    }
}
