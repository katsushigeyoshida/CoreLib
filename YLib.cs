using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace CoreLib
{
    /// <summary>
    /// 汎用ライブラリ
    /// 
    /// ---  システム関連  ------
    ///  void DoEvents()
    ///  string getAppFolderPath()
    ///  string getAppName()
    ///  void fileExecute(string path)
    ///  static void Swap<T>(ref T lhs, ref T rhs)
    ///  
    ///  ---  ネットワーク関連  ---
    ///  bool webFileDownload(string url, string filePath)
    ///  string getWebText(string url)
    ///  string getWebText(string url, int encordType = 0)
    ///  
    ///  ---  文字列処理関連  ----
    ///  bool wcMatch(string srcstr, string pattern)
    ///  int indexOf(string text, string val, int count = 1)
    ///  int lastIndexOf(string text, string val, int count = 1)
    ///  stripBrackets(string text, char sb = '[', char eb = ']')
    ///  string trimControllCode(string buf)
    ///  bool IsNumberString(string num, bool allNum = false)
    ///  bool boolParse(string str, bool val = true)
    ///  int intParse(string str, int val = 0)
    ///  double doubleParse(string str, double val = 0.0)
    ///  int string2int(string num)
    ///  double string2double(string num)
    ///  List<string> string2StringNumbers(string num)
    ///  string string2StringNum(string num)
    ///  string[] seperateString(string str)
    ///  
    ///  ---  ファイル・ディレクトリ関連  ------
    ///  bool makeDir(string path)
    ///  string fileOpenSelectDlg(string title, string initDir, List<string[]> filters)
    ///  string fileSaveSelectDlg(string title, string initDir, List<string[]> filters)
    ///  string[] getFiles(string path)
    ///  List<string[]> loadCsvData(string filePath, string[] title, bool firstTitle = false)
    ///  List<string[]> loadCsvData(string filePath, bool tabSep = false)
    ///  void saveCsvData(string path, string[] format, List<string[]> data)
    ///  void saveCsvData(string path, List<string[]> csvData)
    ///  List<string[]> loadJsonData(string filePath)
    ///  string loadTextFile(string path)
    ///  saveTextFile(string path, string buffer)
    ///  byte[] loadBinData(string path, int size = 0)
    ///  void saveBinData(string path, byte[] buffer)
    ///  bool gzipDecompress(string ipath, string opath)
    ///  
    ///  --- データ処理関係  ------
    ///  List<string[]> splitJson(string jsonData, string baseTitle = "")
    ///  string getJsonDataString(string jsonData)
    ///  
    ///  ---  日付・時間関連  ------
    ///  double getJD(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)
    ///  int julianDay(int y, int m, int d)
    ///  double getMJD(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)
    ///  (int year, int month, int day) JulianDay2Date(double jd)
    ///  (int hour, int min, int sec) JulianDay2Time(double jd)
    ///  double getGreenwichSiderealTime(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)
    ///  double getLocalSiderealTime(double dSiderealTime, double dLongitude)
    ///  
    ///  ---  グラフィック処理関連  ------
    ///  Point rotateOrg(Point po, double rotate)
    ///  Point rotatePoint(Point ctr, Point po, double rotate)
    ///  double angleOrg(Point po)
    ///  double anglePoint(Point ctr, Point po)
    ///  int angleQuadrant(double ang)
    ///  List<Point> circlePeakPoint(Point c, double r)
    ///  List<PointD> arcPeakPoint(PointD c, double r, double sa, double ea)
    ///  List<Point> arcPeakPoint(Point c, double r, double sa, double ea)
    ///  List<PointD> pointSort(List<PointD> plist)
    ///  List<Point> pointSort(List<Point> plist)
    ///  List<PointD> pointListSqueeze(List<PointD> plist)
    ///  List<PointD> devideArcList(PointD c, double r, double sa, double ea, int div = 32)
    ///  List<PointD> divideCircleList(PointD c, double r, int div = 32)
    ///  List<Point> divideCircleList(Point c, double r, int div = 32)
    ///  Point averagePoint(List<Point> pList)
    ///  
    ///  ---  数値処理関連  ------
    ///  double mod(double a, double b)
    ///  int mod(int a, int b)
    ///  
    ///  ---  行列計算  ---
    ///  double[,] unitMatrix(int unit)                     単位行列
    ///  double[,] matrixTranspose(double[,] A)             転置行列  行列Aの転置A^T
    ///  double[,] matrixMulti(double[,] A, double[,] B)    行列の積  AxB
    ///  double[,] matrixAdd(double[,] A, double[,] B)      行列の和 A+B
    ///  double[,] matrixInverse(double[,] A)               逆行列 A^-1
    ///  double[,] copyMatrix(double[,] A)                  行列のコピー
    /// 
    ///  ---  単位変換関連  ------
    ///  string getCoordinatePattern(string coordinate)
    ///  Point cnvCoordinate(string coordinate)
    ///  Point cnvCoordinate2(string coordinate)
    ///  double R2D(double rad)
    ///  double D2R(double deg)
    ///  double H2D(double hour)
    ///  double D2H(double deg)
    ///  double H2R(double hour)
    ///  double R2H(double rad)
    ///  string deg2DMS(double deg)
    ///  double DMS2deg(string dms)
    ///  string hour2HMS(double hour)
    ///  double HMS2hour(string hms)
    ///  double DMS2deg(int d, int m, double s)
    ///  double HMS2hour(int h, int m, double s)
    ///  double HM2hour(string hm)
    ///  double DM2deg(string dm)
    ///  double dm2deg(string dm)
    ///  double hm2hour(string hm)
    /// 
    /// </summary>
    public class YLib
    {
        private Encoding[] mEncoding;
        public int mEncordingType = 0;

        public bool mError = false;
        public string mErrorMessage = "";
        private double mEps = 1E-8;

        public YLib()
        {
            //  エンコーディングのための設定
            //  .Net CoreではEncoding.RegisterProviderのプロバイダー登録が必要、ないとshift_jisでエラーとなる
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            mEncoding = new Encoding[] {
                Encoding.UTF8, Encoding.GetEncoding("shift_jis"), Encoding.GetEncoding("euc-jp") 
            };
        }

        public void test()
        {

        }

        //  ---  システム関連  ------

        /// <summary>
        /// コントロールを明示的に更新するコード
        /// 参考: https://www.ipentec.com/document/csharp-wpf-implement-application-doevents
        /// </summary>
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(ExitFrames);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// DoEventsからのコールバック
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private object ExitFrames(object arg)
        {
            ((DispatcherFrame)arg).Continue = false;
            return null;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 実行ファイルのフォルダを取得(アプリケーションが存在するのディレクトリ)
        /// </summary>
        /// <returns>ディレクトリ</returns>
        public string getAppFolderPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 実行プログラム名をフルパスで取得
        /// プログラム名は exe でなく dll になる
        /// </summary>
        /// <returns></returns>
        public string getAppName()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// ファイルを実行する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public void fileExecute(string path)
        {
            try {
                if (File.Exists(path))
                    System.Diagnostics.Process.Start(path);
                else
                    System.Windows.MessageBox.Show(path + " がありません");
            } catch (Exception e) {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// URLを標準ブラウザで開く
        /// Process.Start()でエラー(.NET COre)になる時に使用
        /// https://oita.oika.me/2017/09/17/dotnet-core-process-start-with-url/
        /// https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        /// </summary>
        /// <param name="targetUrl"></param>
        public void openUrl(string url)
        {
            try {
                Process.Start(url);
            } catch {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    //Windowsのとき  
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    //Linuxのとき  
                    Process.Start("xdg-open", url);
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    //Macのとき  
                    Process.Start("open", url);
                } else {
                    throw;
                }
            }
        }


        /// <summary>
        /// ジェネリックによるswap関数
        /// 例: Swap<int>(ref a, ref b); Swap(ref a, ref b);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        //  ---  ネットワーク関連  ---

        /// <summary>
        /// Web上のファイルをダウンロードする
        /// HttpClienttが推奨されている(https://posnum.hatenablog.com/entry/2014/10/13/160230)
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public bool webFileDownload(string url, string filePath)
        {
            WebClient wc = null;
            try {
                wc = new WebClient();       //  HttpClienttが推奨されている(https://qiita.com/aosho235/items/1ad9ee05b1be62ca2c59)
                //  Microsoft Edgeの要求ヘッダーを模擬(403エラー対応)
                wc.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Mobile Safari/537.36 Edg/92.0.902.67");    //  403対応
                //  データのダウンロード
                wc.DownloadFile(url, filePath);
                wc.Dispose();
            } catch (Exception e) {
                wc.Dispose();
                mError = true;
                mErrorMessage = e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// URLのWebデータの読込
        /// EndodingのデフォルトはUTF8(mEncordingType == 0)
        /// </summary>
        /// <param name="url">URL</param>
        public string getWebText(string url)
        {
            return getWebText(url, mEncordingType);
        }

        /// <summary>
        /// エンコードタイプを指定してURLのWebデータの読込
        /// https://vdlz.xyz/Csharp/Porpose/WebTool/parse/parse.html
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="encordType">エンコードタイプ(default=URF8)</param>
        /// <returns></returns>
        public string getWebText(string url, int encordType = 0)
        {
            Stream st;
            StreamReader sr;
            string text;
            try {
                WebClient wc = new WebClient();
                //  Microsoft Edgeの要求ヘッダーを模擬(403エラー対応)
                wc.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Mobile Safari/537.36 Edg/92.0.902.67");    //  403対応
                //  データのダウンロード
                st = wc.OpenRead(url);
                sr = new StreamReader(st, mEncoding[encordType]);
                text = sr.ReadToEnd();

            } catch (Exception e) {
                mError = true;
                mErrorMessage = e.Message;
                return null;
            }
            sr.Close();
            st.Close();

            return text;
        }

        //  ---  文字列処理関連  -----

        /// <summary>
        /// ワイルドカードによる文字検索
        /// </summary>
        /// <param name="srcstr">文字列</param>
        /// <param name="pattern">検索パターン</param>
        /// <returns>有無</returns>
        public bool wcMatch(string srcstr, string pattern)
        {
            if (pattern == null || pattern.Length == 0)
                return true;
            int si = 0, si2 = -1, fi = 0, fi2 = -1;
            string ss = srcstr.ToLower();
            string fs = pattern.ToLower();
            do {
                if (fs[fi] == '*') {
                    fi2 = fi;
                    fi++;
                    if (fs.Length <= fi)
                        return true;
                    for (; (si < ss.Length) && (ss[si] != fs[fi]); si++)
                        ;
                    si2 = si;
                    if (ss.Length <= si)
                        return false;
                }
                if (fs[fi] != '?') {
                    if (ss.Length <= si)
                        return false;
                    if (ss[si] != fs[fi]) {
                        if (si2 < 0)
                            return false;
                        si = si2 + 1;
                        fi = fi2;
                        continue;
                    }
                }
                si++;
                fi++;
                if (fs.Length <= fi && si < ss.Length)
                    return false;
                if (ss.Length <= si && fi < fs.Length && fs[fi] != '*' && fs[fi] != '?')
                    return false;
            } while (si < ss.Length && fi < fs.Length);
            return true;
        }

        /// <summary>
        /// 文字列を前から指定位置を検索する
        /// </summary>
        /// <param name="text">検索される文字列</param>
        /// <param name="val">検索する文字列</param>
        /// <param name="count">検索回数</param>
        /// <returns>検索位置</returns>
        public int indexOf(string text, string val, int count = 1)
        {
            var n = 0;
            for (int i = 0; i < count; i++) {
                n = text.IndexOf(val, n + 1);
            }
            return n;
        }

        /// <summary>
        /// 文字列を後から指定位置を検索する
        /// </summary>
        /// <param name="text">検索される文字列</param>
        /// <param name="val">検索する文字列</param>
        /// <param name="count">検索回数</param>
        /// <returns>検索位置</returns>
        public int lastIndexOf(string text, string val, int count = 1)
        {
            var n = text.Length;
            for (int i = 0; i < count; i++) {
                n = text.LastIndexOf(val, n - 1);
            }
            return n;
        }

        /// <summary>
        /// 文字列から括弧で囲まれた領域を取り除く
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="sb">開始括弧文字(省略時'[')</param>
        /// <param name="eb">終了括弧文字(省略時']')</param>
        /// <returns>文字列</returns>
        public string stripBrackets(string text, char sb = '[', char eb = ']')
        {
            int n = text.LastIndexOf(sb);
            if (n < 0)
                return text;
            int m = text.IndexOf(eb, n);
            if (0 <= n && 0 <= m) {
                text = text.Substring(0, n) + text.Substring(m + 1);
                text = stripBrackets(text, sb, eb);
            } else if (0 <= n && m < 0) {
                text = text.Substring(0, n);
            } else if (n < 0 && 0 <= m) {
                text = text.Substring(m + 1);
            }
            return text;
        }


        /// <summary>
        /// 文字列内のコントロールコードを除去する
        /// 0x20 <= (半角文字) <0xf0 and 0x100<= (全角文字)を通過させる
        /// UNICODEのHeader(0xFEFF)も除く
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public string trimControllCode(string buf)
        {
            if (buf == null)
                return "";
            string obuf = "";
            for (int i = 0; i < buf.Length; i++)
                if ((0x20 <= (int)buf[i] && (int)buf[i] < 0xf0) ||
                    ((0xff < (int)buf[i] && (int)buf[i] != 0xFEFF && !(0xFFFE <= (int)buf[i]))))
                    obuf += buf[i];
            return obuf;
        }

        /// <summary>
        /// 数値文字列かを判定する
        /// 数値以外の文字があっても数値にできるものは数値として判定
        /// 数値文字列が複数ある時は数値文字列とはしない
        /// </summary>
        /// <param name="num">数値文字列</param>
        /// <param name="allNum">すべて数値のみ</param>
        /// <returns>判定</returns>
        public bool IsNumberString(string num, bool allNum = false)
        {
            if (num == null)
                return false;
            if (num.Length == 1 && num.CompareTo("0") == 0)
                return true;
            List<string> numbers = string2StringNumbers(num);
            if (1 < numbers.Count)
                return false;
            string nbuf = allNum ? num : string2StringNum(num);
            double val;
            return double.TryParse(nbuf, out val) ? true : false;
        }

        /// <summary>
        /// 文字列を論理値に変換
        /// 変換できない場合はdefaultを使用
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="val">default値(省略時はtrue)</param>
        /// <returns></returns>
        public bool boolParse(string str, bool val = true)
        {
            bool b;
            if (bool.TryParse(str, out b))
                return b;
            else
                return val;
        }

        /// <summary>
        /// 文字列を整数に変換
        /// 変換できない場合はdefaultを使用
        /// </summary>
        /// <param name="str">数値文字列</param>
        /// <param name="val">default値(省略時は0)</param>
        /// <returns>整数値</returns>
        public int intParse(string str, int val = 0)
        {
            int d;
            if (int.TryParse(str, out d))
                return d;
            else
                return val;
        }

        /// <summary>
        /// 文字列を実数に変換
        /// 変換できない場合はdefaultを使用
        /// </summary>
        /// <param name="str">数値文字列</param>
        /// <param name="val">default値(省略時は0)</param>
        /// <returns>実数値</returns>
        public double doubleParse(string str, double val = 0.0)
        {
            double d;
            if (double.TryParse(str, out d))
                return d;
            else
                return val;
        }

        /// <summary>
        /// 文字列の先頭が数値の場合、数値に変換する
        /// 変換できない時は0を返す
        /// 先頭の空白や0、後尾の数値以外の文字は除外して変換する
        /// </summary>
        /// <param name="num">数値入り文字列</param>
        /// <returns>整数値</returns>
        public int string2int(string num)
        {
            if (num == null)
                return 0;

            string nbuf = string2StringNum(num);
            int val;
            val = int.TryParse(nbuf, out val) ? val : 0;
            return val;
        }

        /// <summary>
        /// 文字列の先頭が数値の場合、数値に変換する
        /// 変換できない時は0を返す
        /// 先頭の空白や0、後尾の数値以外の文字は除外して変換する
        /// </summary>
        /// <param name="num">数値入り文字列</param>
        /// <returns>実数値</returns>
        public double string2double(string num)
        {
            if (num == null)
                return 0;

            string nbuf = string2StringNum(num);
            double val;
            val = double.TryParse(nbuf, out val) ? val : 0;
            return val;
        }

        /// <summary>
        /// 文字列の中から複数の数値文字列を抽出する
        /// </summary>
        /// <param name="num">文字列</param>
        /// <returns>数値文字列リスト</returns>
        public List<string> string2StringNumbers(string num)
        {
            List<string> data = new List<string>();
            char[] startChar = startNum.ToCharArray();
            int sp = 0;
            while (sp < num.Length) {
                sp = num.IndexOfAny(startChar, sp);
                if (sp < 0)
                    break;
                string buf = string2StringNum(num.Substring(sp));
                if (0 < buf.Length) {
                    data.Add(buf);
                    sp += buf.Length;
                } else {
                    sp++;
                }
            }
            return data;
        }

        private string startNum = "+-123456789";
        private string numChar = ".0123456789";

        /// <summary>
        /// 文字列から数値に関係ない文字を除去し、実数に変換できる文字列にする
        /// </summary>
        /// <param name="num">文字列</param>
        /// <returns>数値文字列</returns>
        public string string2StringNum(string num)
        {
            string buf = "";
            num = num.Replace(",", "");             //  桁区切りのカンマを除く
            num = num.TrimStart(' ');               //  先頭の空白と0を除く
            for (int i = 0; i < num.Length; i++) {
                if (buf.Length == 0 || num[i] == 'E' || num[i] == 'e') {
                    if (0 <= startNum.IndexOf(num[i])) {
                        buf += num[i];
                    } else if (buf.Length == 0 && (i + 1 < num.Length) && num[i] == '0' && num[i + 1] != '.') {
                        //  先頭の不要な0を除く
                    } else if (i + 1 < num.Length && num[i] == '0' && num[i + 1] == '.') {
                        buf += num[i++];
                        buf += num[i];
                    } else if (i + 1 < num.Length && (num[i] == 'E' || num[i] == 'e') && 0 <= startNum.IndexOf(num[i + 1])) {
                        buf += num[i++];
                        buf += num[i];
                    } else {
                        break;
                    }
                } else {
                    if (0 <= numChar.IndexOf(num[i])) {
                        buf += num[i];
                    } else {
                        break;
                    }
                }
            }
            if (buf.Length == 1 && (buf[0] == '+' || buf[0] == '-' || buf[0] == '.'))
                return "";
            for (int i = buf.Length - 1; 0 <= i; i--) {
                if (0 <= numChar.IndexOf(buf[i])) {
                    buf = buf.Substring(0, i + 1);
                    break;
                }
            }

            return buf;
        }

        private string ZenCode =
            "　！”＃＄％＆’（）＊＋，－．／" +
             "０１２３４５６７８９：；＜＝＞？" +
             "＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯ" +
             "ＰＱＲＳＴＵＶＷＸＹＺ［￥］＾＿" +
             "‘ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏ" +
             "ｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～";
        private string HanCode =
            " !\"#$%&'()*+,-./" +
            "0123456789:;<=>?" +
            "@ABCDEFGHIJKLMNO" +
            "PQRSTUVWXYZ[\\]^_" +
            "`abcdefghijklmno" +
            "pqrstuvwxyz{|}~";

        /// <summary>
        /// 文字列内の全角英数字を半角に変換する
        /// </summary>
        /// <param name="zenStr"></param>
        /// <returns></returns>
        public string strZne2Han(string zenStr)
        {
            string buf = "";
            for (int i = 0; i < zenStr.Length; i++) {
                int n = ZenCode.IndexOf(zenStr[i]);
                if (0 <= n && n < HanCode.Length) {
                    buf += HanCode[n];
                } else {
                    buf += zenStr[i];
                }
            }
            return buf;
        }

        private string ZenNumCode = "０１２３４５６７８９．＋－";
        private string HanNumCode = "0123456789.+-";

        /// <summary>
        /// 文字列内の全角数値を半角に変換する
        /// </summary>
        /// <param name="zenStr"></param>
        /// <returns></returns>
        public string strNumZne2Han(string zenStr)
        {
            string buf = "";
            for (int i = 0; i < zenStr.Length; i++) {
                int n = ZenNumCode.IndexOf(zenStr[i]);
                if (0 <= n && n < HanCode.Length) {
                    buf += HanNumCode[n];
                } else {
                    buf += zenStr[i];
                }
            }
            return buf;
        }

        /// <summary>
        /// 文字列の中の改行コード、','、'"'を'\'付きコードに置き換える
        /// '"'はCSVの仕様に合わせて'""'にする
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>'\'付き文字列</returns>
        public string strControlCodeCnv(string str)
        {
            string buffer;
            buffer = str.Replace("\\", "\\\\");         //  \→\\
            buffer = buffer.Replace("\r\n", "\\\\n");   //  \r\n→\\\n
            buffer = buffer.Replace("\n", "\\\\n");     //  \n→\\\n
            buffer = buffer.Replace("\r", "\\\\n");     //  \r→\\\n
            buffer = buffer.Replace(",", "\\,");        //  ,→\,
            buffer = buffer.Replace("\"", "\"\"");      //  "→""
            return buffer;
        }

        /// <summary>
        /// 文字列の中の'\'付きコードを通常のコードに戻す
        /// </summary>
        /// <param name="str">'\'付き文字列</param>
        /// <returns>文字列</returns>
        public string strControlCodeRev(string str)
        {
            string buffer;
            buffer = str.Replace("\\\\n", "\r\n");      //  \\\n→\n
            buffer = buffer.Replace("\\\\", "\\");      //  \\→\
            buffer = buffer.Replace("\\,", ",");        //  \,→,
            buffer = buffer.Replace("\"\"", "\"");      //  ""→"
            buffer = buffer.Replace("\\\"", "\"");      //  \"→"
            return buffer;
        }

        /// <summary>
        /// 文字列をカンマセパレータで分解して配列に格納する
        /// ""で囲まれた中で""→"、\"→"、\,→,、\\n→\nにする
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns></returns>
        public string[] seperateString(string str)
        {
            List<string> data = new List<string>();
            string buf = "";
            int i = 0;
            while (i < str.Length) {
                if (str[i] == '"' && i < str.Length - 1) {
                    //  ダブルクォーテーションで囲まれている場合
                    i++;
                    while (str[i] != '"' || (i + 1 < str.Length && str[i] == '"' && str[i + 1] == '"')) {
                        if (str[i] == '\\' && i + 1 < str.Length) {
                            //  コントロールコードの処理('\\"','\\,','\\\n','\\\r')
                            if (str[i + 1] == '"' || str[i + 1] == ',' || str[i + 1] == '\n') {
                                buf += str[i + 1];
                                i += 2;
                            } else if (str[i + 1] == '\r') {
                                buf += '\n';
                                i += 2;
                            } else {
                                buf += str[i++];
                            }
                        } else if (str[i] == '"' && i + 1 < str.Length) {
                            if (str[i + 1] == '"') {
                                buf += str[i + 1];
                                i += 2;
                            } else {
                                buf += str[i++];
                            }
                        } else {
                            buf += str[i++];
                        }
                        if (i == str.Length)
                            break;
                    }
                    data.Add(buf);
                    i++;
                } else if (str[i] == ',' && i < str.Length - 1) {
                    //  区切りカンマ
                    if (i == 0 || (0 < i && str[i - 1] == ','))
                        data.Add(buf);
                    i++;
                } else {
                    //  カンマ区切りの場合
                    if (str[i] == ',' && i < str.Length - 1) {
                        i++;
                    } else {
                        while (str[i] != ',') {
                            buf += str[i++];
                            if (i == str.Length)
                                break;
                        }
                    }
                    data.Add(buf);
                    i++;
                }
                buf = "";
            }
            return data.ToArray();
        }

        /// <summary>
        /// 正規表現を使ったHTMLデータからパターン抽出
        /// 例: pattern = "<title>(?<title>.*?)</title>"
        ///     group = "title"
        /// </summary>
        /// <param name="html">HTMLソースデータ</param>
        /// <param name="pattern">抽出パターン</param>
        /// <param name="group">抽出グループ</param>
        /// <returns>抽出データリスト</returns>
        public List<string> getPattern(string html, string pattern, string group)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = reg.Match(html);
            List<string> listData = new List<string>();
            while (m.Success) {
                listData.Add(m.Groups[group].Value);
                m = m.NextMatch();
            }
            return listData;
        }

        //  HTMLパターン抽出のオプション
        public RegexOptions mRegexOption = RegexOptions.IgnoreCase; //  | Singleline/Multiline/RightToLeft

        /// <summary>
        /// 正規表現を使ったHTMLからのパターン抽出
        /// 例:  pattern = "<a href=\"(.*?)\".*?title=\"(.*?)\">(.*?)</a>(.*?)</li>"
        /// </summary>
        /// <param name="html">HTMLソースデータ</param>
        /// <param name="pattern">抽出パターン</param>
        /// <returns>抽出データリスト</returns>
        public List<string[]> getPattern(string html, string pattern)
        {
            Regex reg = new Regex(pattern, mRegexOption);
            Match m = reg.Match(html);
            List<string[]> listData = new List<string[]>();
            while (m.Success) {
                System.Diagnostics.Debug.WriteLine($"{html.Length} {m.Index} {m.Length}");
                string[] data = new string[m.Groups.Count];
                for (int i = 0; i < m.Groups.Count; i++) {
                    data[i] = m.Groups[i].ToString();
                }
                listData.Add(data);
                m = m.NextMatch();
            }
            return listData;
        }

        /// <summary>
        /// 正規表現を使ったHTMLからのパターン抽出
        /// getPattern()と同じだか抽出方法にMatches()(一括検索)を使用
        /// 例:  pattern = "<a href=\"(.*?)\".*?title=\"(.*?)\">(.*?)</a>(.*?)</li>"
        /// </summary>
        /// <param name="html">HTMLソースデータ</param>
        /// <param name="pattern">抽出パターン</param>
        /// <returns>抽出データリスト</returns>
        public List<string[]> getPattern2(string html, string pattern)
        {
            Regex reg = new Regex(pattern, mRegexOption);
            MatchCollection mc = reg.Matches(html);
            List<string[]> listData = new List<string[]>();
            foreach (Match m in mc) {
                string[] data = new string[m.Groups.Count];
                for (int i = 0; i < m.Groups.Count; i++) {
                    data[i] = m.Groups[i].ToString();
                }
                listData.Add(data);
            }
            return listData;
        }



        //  ---  ファイル・ディレクトリ関連  ------

        /// <summary>
        /// ファイルパスからディレクトリを作成
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>成否</returns>
        public bool makeDir(string path)
        {
            try {
                string folder = Path.GetDirectoryName(path);
                if (0 < folder.Length && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

            } catch (Exception e) {
                return false;
            }
            return true;
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
        public string fileSaveSelectDlg(string title, string initDir, List<string[]> filters)
        {
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
        /// 指定されたパスからファイルリストを作成
        /// 指定されたpathからフォルダ名とワイルドカードのファイル名に分けて
        /// ファイルリストを取得する
        /// 例 path = D:\folder\*.flac
        /// </summary>
        /// <param name="path">パス名</param>
        /// <returns>ファイルリスト</returns>
        public string[] getFiles(string path)
        {
            try {
                string folder = Path.GetDirectoryName(path);
                string ext = Path.GetFileName(path);
                return Directory.GetFiles(folder, ext);
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// CSV形式のファイルを読み込みList<String[]>形式で出力する
        /// title[]が指定されて一行目にタイトルが入っていればタイトルの順番に合わせて取り込む
        /// titleがnullであればそのまま配列に置き換える
        /// </summary>
        /// <param name="filePath">ファイル名パス</param>
        /// <param name="title">タイトルの配列</param>
        /// <param name="firstTitle">1列目の整合性確認</param>
        /// <returns>取得データ(タイトル行なし)</returns>
        public List<string[]> loadCsvData(string filePath, string[] title, bool firstTitle = false)
        {
            //	ファイルデータの取り込み
            List<string[]> fileData = loadCsvData(filePath);
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
        /// <returns>データリスト</returns>
        public List<string[]> loadCsvData(string filePath, bool tabSep = false)
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
                            string[] buf;
                            if (tabSep) {
                                buf = line.Split('\t');
                            } else {
                                buf = seperateString(line);
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
        /// </summary>
        /// <param name="path">ファイル名パス</param>
        /// <param name="format">タイトル列</param>
        /// <param name="data">Listデータ</param>
        public void saveCsvData(string path, string[] format, List<string[]> data)
        {
            List<string[]> dataList = new List<string[]>();
            dataList.Add(format);
            foreach (string[] v in data)
                dataList.Add(v);

            saveCsvData(path, dataList);
        }

        /// <summary>
        /// データをCSV形式でファイルに書き込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="csvData">データリスト</param>
        public void saveCsvData(string path, List<string[]> csvData)
        {
            if (0 < csvData.Count) {
                string folder = Path.GetDirectoryName(path);
                if (0 < folder.Length && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (StreamWriter dataFile = new StreamWriter(path, false, mEncoding[mEncordingType])) {
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
                            List<string[]> jsonData = splitJson(getJsonDataString(line));
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

        /// <summary>
        /// gzipファイルを解凍する
        /// 拡張子が gz 出ない場合はリネームしてから解凍
        /// </summary>
        /// <param name="ipath"></param>
        /// <param name="opath"></param>
        /// <returns></returns>
        public bool gzipDecompress(string ipath, string opath)
        {
            if (!File.Exists(ipath))
                return false;
            //  gzipファイル化の確認
            byte[] header = loadBinData(ipath, 2);
            if (header[0] != 0x1f || header[1] != 0x8b)
                return false;
            string inpath = "";
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

        //  --- データ処理関係  ------

        /// <summary>
        /// JSON形式の文字列から[名前:値]の対データをリストデータとして取得する
        /// </summary>
        /// <param name="jsonData">JSON形式の文字列</param>
        /// <param name="baseTitle">オブジェクト名(省略可)</param>
        /// <returns>対データのリストデータ</returns>
        public List<string[]> splitJson(string jsonData, string baseTitle = "")
        {
            List<string[]> jsonList = new List<string[]>();
            string[] data = new string[2];
            int i = 0;
            int j = 0;
            while (i < jsonData.Length) {
                if (jsonData[i] == '\"') {
                    data[j] = "";
                    i++;
                    while (jsonData[i] != '\"' && i < jsonData.Length) {
                        data[j] += jsonData[i];
                        i++;
                    }
                }
                if (jsonData[i] == ',' || i == jsonData.Length - 1) {
                    if (i == jsonData.Length - 1)
                        data[j] += jsonData[i];
                    data[0] = (0 < baseTitle.Length ? baseTitle + " " : "") + data[0].Trim();
                    data[1] = data[1].Trim();
                    if (0 < data[0].Length)
                        jsonList.Add(data);
                    j = 0;
                    data = new string[2];
                } else if (jsonData[i] == ':') {
                    j = 1;
                } else if (jsonData[i] == '\"') {

                } else if (jsonData[i] == '{') {
                    string jsonData2 = getJsonDataString(jsonData.Substring(i));
                    List<string[]> jsonList2 = splitJson(jsonData2, data[0]);
                    foreach (string[] data2 in jsonList2)
                        jsonList.Add(data2);
                    data[0] = "";
                    i += jsonData2.Length + 1;
                    while (jsonData[i] != '}' && i < jsonData.Length)
                        i++;
                } else {
                    data[j] += jsonData[i];
                }
                i++;
            }
            return jsonList;
        }

        /// <summary>
        /// JSON形式の文字列から{}内の文字列を取得する
        /// 入れ子構造に対応
        /// </summary>
        /// <param name="jsonData">JSON形式文字列</param>
        /// <returns>抽出したJSON形式文字列</returns>
        public string getJsonDataString(string jsonData)
        {
            string buffer = "";
            int brCount = 0;
            int i = 0;
            while (i < jsonData.Length) {
                if (jsonData[i] == '{') {
                    brCount++;
                    if (brCount <= 1) {
                        i++;
                        continue;
                    }
                } else if (jsonData[i] == '}') {
                    brCount--;
                    if (brCount == 0)
                        break;
                }
                buffer += jsonData[i];
                i++;
            }
            return buffer.Trim();
        }


        //  ---  日付・時間関連  ------

        /// <summary>
        /// 歴日からユリウス日に変換
        /// </summary>
        /// <param name="year">西暦年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns></returns>
        public int date2JulianDay(int year, int month, int day)
        {
            if (month <= 2) {
                month += 12;
                year--;
            }
            if ((year * 12 + month) * 31 + day >= (1582 * 12 + 10) * 31 + 15) {
                //  1582/10/15以降はグレゴリオ暦
                day += 2 - year / 100 + year / 400;
            }
            return (int)Math.Floor(365.25 * (year + 4716)) + (int)(30.6001 * (month + 1)) + day - 1524;
        }

        /// <summary>
        /// ユリウス日(紀元前4713年(-4712年)1月1日が0日)の取得 (https://www.dinop.com/vc/getjd.html)
        /// 年月日は西暦、時間はUTC
        /// </summary>
        /// <param name="nYear">年</param>
        /// <param name="nMonth">月</param>
        /// <param name="nDay">日</param>
        /// <param name="nHour">時</param>
        /// <param name="nMin">分</param>
        /// <param name="nSec">秒</param>
        /// <returns>ユリウス日</returns>
        public double getJD(int nYear, int nMonth, int nDay, int nHour = 0, int nMin = 0, int nSec = 0)
        {
            //  ユリウス日の計算
            if (nMonth == 1 || nMonth == 2) {
                nMonth += 12;
                nYear--;
            }
            double dJD = (double)((int)(nYear * 365.25) + (int)(nYear / 400) -
                (int)(nYear / 100) + (int)(30.59 * (nMonth - 2)) + nDay - 678912 + 2400000.5 +
                (double)nHour / 24 + (double)nMin / (24 * 60) + (double)nSec / (24 * 60 * 60));
            return dJD;
        }

        /// <summary>
        /// 天文年鑑の数式でユリウス日を求める
        /// </summary>
        /// <param name="y">年</param>
        /// <param name="m">月</param>
        /// <param name="d">日</param>
        /// <returns></returns>
        public int julianDay(int y, int m, int d)
        {
            return (y + 4800 - (14 - m) / 12) * 1461 / 4
                + (m + (14 - m) / 12 * 12 - 2) * 367 / 12
                - (y + 4900 - (14 - m) / 12) / 100 * 3 / 4 + d - 32075;
        }

        /// <summary>
        /// 準ユリウス日(1582年10月15日が0日)の取得
        /// </summary>
        /// <param name="nYear">年</param>
        /// <param name="nMonth">月</param>
        /// <param name="nDay">日</param>
        /// <param name="nHour">時</param>
        /// <param name="nMin">分</param>
        /// <param name="nSec">秒</param>
        /// <returns>準ユリウス日</returns>
        public double getMJD(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)
        {
            double dJD = getJD(nYear, nMonth, nDay, nHour, nMin, nSec);
            if (dJD == 0.0)
                return 0.0;
            else
                return dJD - 2400000.5;
        }

        /// <summary>
        /// ユリウス日から西暦(年月日)を求める
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns> (年,月,日)</returns>
        public (int year, int month, int day) JulianDay2Date(double jd)
        {
            int jdc = (int)(jd + 0.5);  //  ユリウス日の半日のずれを調整
            if (jdc >= 2299161) {
                //  1582/10+15以降はグレゴリオ暦
                int t = (int)((jdc - 1867216.25) / 365.25);
                jdc += 1 + t / 100 - t / 400;
            }
            jdc += 1524;
            int y = (int)Math.Floor(((jdc - 122.1) / 365.25));
            jdc -= (int)Math.Floor(365.25 * y);
            int m = (int)(jdc / 30.6001);
            jdc -= (int)(30.6001 * m);
            int day = jdc;
            int month = m - 1;
            int year = y - 4716;
            if (month > 12) {
                month -= 12;
                year++;
            }
            return (year, month, day);
        }

        /// <summary>
        /// ユリウス日から時間(時分秒)の取得
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>(時,分,秒)</returns>
        public (int hour, int min, int sec) JulianDay2Time(double jd)
        {
            double t = (jd - 0.5 + 0.5 / 24.0 / 3600.0) % 1.0;  //ユリウス日の半日ずれとsecの誤差調整
            int hour = (int)(t * 24.0);
            int min = (int)(((t * 24.0) % 1.0) * 60.0);
            int sec = (int)(((t * 24.0 * 60) % 1) * 60.0);
            return (hour, min, sec);
        }

        /// <summary>
        /// ユリウス日から歴日文字列に変換
        /// 0: yyyy/mm/dd 1: yyyymmdd 2:yyyy-mm-dd  3: mm/dd/yyyy 
        /// 4: yyyy年mm月dd日 5: yyyy年mm月 6: yyyy年
        /// 7: yyyy年mm月ww週 8: yyyy年ww週 9: yyyy年ww週(年変わりを同じ年にする)
        /// </summary>
        /// <param name="jdc">ユリウス日</param>
        /// <param name="type">歴日文字列タイプ</param>
        /// <returns>歴日文字列</returns>
        public string JulianDay2DateYear(double jd, int type = 0)
        {
            string date = "";
            double jdc = jd;
            if (type == 9) {
                //  年変わりを同じ週にするためユリウス日を週頭に変更
                jd = jdc - (jdc + 1) % 7;
            }
            (int year, int month, int day) = JulianDay2Date(jd);
            int weekNo = 0;
            switch (type) {
                case 0:     //  yyyy/mm/dd
                    date = string.Format("{0:0000}/{1:00}/{2:00}", year, month, day);
                    break;
                case 1:     //  yyyymmdd
                    date = string.Format("{0:0000}{1:00}{2:00}", year, month, day);
                    break;
                case 2:     //  yyyy-mm-dd
                    date = string.Format("{0:0000}-{1:00}-{2:00}", year, month, day);
                    break;
                case 3:     //  mm/dd/yyyy
                    date = string.Format("{0:00}/{1:00}/{2:0000}", month, day, year);
                    break;
                case 4:     //  yyyy年mm月dd日
                    date = string.Format("{0}年{1}月{2}日", year, month, day);
                    break;
                case 5:     //  yyyy年mm月
                    date = string.Format("{0}年{1}月", year, month);
                    break;
                case 6:   //    yyyy年  
                    date = string.Format("{0}年", year);
                    break;
                case 7:     //  yyyy年mm月ww週
                    int jdt = date2JulianDay(year, month, 1) + 1;
                    weekNo = ((int)jd + 1) / 7 - jdt / 7 + 1;
                    date = string.Format("{0}年{1}月{2}週", year, month, weekNo);
                    break;
                case 8:     //  yyyy年ww週
                case 9:     //  yyyy年ww週(年変わり同じ週)
                    weekNo = ((int)jd + 1) / 7 - (date2JulianDay(year, 1, 1) + 1) / 7 + 1;
                    date = string.Format("{0}年{1}週", year, weekNo);
                    break;
                default:    //  yyyy/mm/dd
                    date = string.Format("{0:0000}/{1:00}/{2:00}", year, month, day);
                    break;
            }
            return date;
        }

        /// <summary>
        /// 恒星時(Wikipedia https://ja.wikipedia.org/wiki/恒星時)
        /// 1平均太陽日 = 1.0027379094 (2020.5 天文年鑑2020)
        /// </summary>
        /// <param name="nYear">年</param>
        /// <param name="nMonth">月</param>
        /// <param name="nDay">日</param>
        /// <param name="nHour">時</param>
        /// <param name="nMin">分</param>
        /// <param name="nSec">秒</param>
        /// <returns>恒星時(hh.hhhh)</returns>
        public double getGreenwichSiderealTime(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)
        {
            double JD = getJD(nYear, nMonth, nDay, nHour, nMin, nSec);      //  ユリウス日
            double TJD = JD - 2440000.5;    //  準ユリウス日
            return 24.0 * ((0.671262 + 1.0027379094 * TJD) % 1.0);
        }

        /// <summary>
        /// 地方恒星時の取得
        /// </summary>
        /// <param name="dSiderealTime">グリニッジ恒星時(hh.hhhh)</param>
        /// <param name="dLongitude">観測地の緯度(dd.dddd)</param>
        /// <returns>地方恒星時(hh.hhhh)</returns>
        public double getLocalSiderealTime(double dSiderealTime, double dLongitude)
        {
            return dSiderealTime + dLongitude / 15.0;
        }


        //  ---  グラフィック処理関連  ------


        /// <summary>
        /// 原点を中心に回転
        /// </summary>
        /// <param name="po">点座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        /// <returns>回転後の座標</returns>
        public Point rotateOrg(Point po, double rotate)
        {
            Point p = new Point();
            p.X = po.X * Math.Cos(rotate) - po.Y * Math.Sin(rotate);
            p.Y = po.X * Math.Sin(rotate) + po.Y * Math.Cos(rotate);
            return p;
        }

        /// <summary>
        /// 回転中心を指定して回転
        /// </summary>
        /// <param name="ctr">回転の中心座標</param>
        /// <param name="po">点座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        /// <returns>回転後の座標</returns>
        public Point rotatePoint(Point ctr, Point po, double rotate)
        {
            Point p = new Point(po.X - ctr.X, po.Y - ctr.Y);
            p = rotateOrg(p, rotate);
            return new Point(p.X + ctr.X, p.Y + ctr.Y);
        }

        /// <summary>
        /// 原点に対する座標点の水平線との角度(rad)
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public double angleOrg(Point po)
        {
            return Math.Atan2(po.Y, po.X);
        }

        /// <summary>
        /// 中心座標を指定して回転角度(rad)
        /// </summary>
        /// <param name="ctr">中心点</param>
        /// <param name="po">点座標</param>
        /// <returns>回転角度</returns>
        public double anglePoint(Point ctr, Point po)
        {
            Point p = new Point(po.X - ctr.X, po.Y - ctr.Y);
            return angleOrg(p);
        }

        /// <summary>
        /// 角度の象限を求める(0-3)
        /// </summary>
        /// <param name="ang">角度(rad)</param>
        /// <returns>象限(0-3)</returns>
        public int angleQuadrant(double ang)
        {
            ang = (ang + Math.PI * 2.0) % (Math.PI * 2.0);
            return (int)(ang / (Math.PI / 2.0));
        }

        /// <summary>
        /// 円の4頂点?を求める
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <returns>頂点リスト</returns>
        public List<Point> circlePeakPoint(Point c, double r)
        {
            List<Point> cpList = new List<Point>();
            cpList.Add(new Point(c.X + r, c.Y));
            cpList.Add(new Point(c.X, c.Y + r));
            cpList.Add(new Point(c.X - r, c.Y));
            cpList.Add(new Point(c.X, c.Y - r));
            return cpList;
        }

        /// <summary>
        /// 円弧の頂点リストを求める
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <returns>頂点リスト</returns>
        public List<PointD> arcPeakPoint(PointD c, double r, double sa, double ea)
        {
            List<PointD> pointList = new List<PointD>();
            List<Point> plist = arcPeakPoint(c.toPoint(), r, sa, ea);
            foreach (Point p in plist)
                pointList.Add(new PointD(p));
            return pointList;
        }

        /// <summary>
        /// 円弧の頂点リストを求める
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <returns>頂点リスト</returns>
        public List<Point> arcPeakPoint(Point c, double r, double sa, double ea)
        {
            Point sp = rotatePoint(c, new Point(c.X + r, c.Y), sa);
            Point ep = rotatePoint(c, new Point(c.X + r, c.Y), ea);
            List<Point> cpList = circlePeakPoint(c, r);     //  円としての頂点座標
            List<Point> plist = new List<Point>();
            int saq = angleQuadrant(sa);                    //  端点の象限
            int eaq = angleQuadrant(ea);
            if (eaq < saq || (saq == eaq && ea < sa)) {
                eaq += 4;
            }
            plist.Add(sp);
            for (int i = saq + 1; i <= eaq; i++) {
                plist.Add(cpList[i % 4]);
            }
            if (plist[plist.Count - 1].X != ep.X || plist[plist.Count - 1].Y != ep.Y)
                plist.Add(ep);
            return plist;
        }

        /// <summary>
        /// 点リストを中心点の角度でソート
        /// </summary>
        /// <param name="plist">点リスト</param>
        /// <returns>ソートリスト</returns>
        public List<PointD> pointSort(List<PointD> plist)
        {
            List<Point> pointList = new List<Point>();
            foreach (PointD p in plist)
                pointList.Add(p.toPoint());
            List<Point> splist = pointSort(pointList);
            plist.Clear();
            foreach (Point p in splist)
                plist.Add(new PointD(p));
            return plist;
        }

        /// <summary>
        /// 点リストを中心点の角度でソート
        /// </summary>
        /// <param name="plist">点リスト</param>
        /// <returns>ソートリスト</returns>
        public List<Point> pointSort(List<Point> plist)
        {
            if (plist.Count < 2)
                return plist;
            Point ap = averagePoint(plist);
            List<(double, Point)> angList = new List<(double, Point)>();
            foreach (Point p in plist) {
                angList.Add((anglePoint(ap, p), p));
            }
            angList.Sort((a, b) => Math.Sign(a.Item1 - b.Item1));
            List<Point> spList = new List<Point>();
            for (int i = 0; i < angList.Count; i++)
                spList.Add(angList[i].Item2);
            return spList;
        }

        /// <summary>
        /// 点座標リストで重複点を削除
        /// 隣同士の点の距離がmEps(10-8)以下の場合にその座標点を削除
        /// </summary>
        /// <param name="plist">座標点リスト</param>
        /// <returns>スクイーズ後の座標点リスト</returns>
        public List<PointD> pointListSqueeze(List<PointD> plist)
        {
            List<PointD> sqeezeList = new List<PointD>();
            if (0 < plist.Count) {
                sqeezeList.Add(plist[0]);
                for (int i = 1; i < plist.Count; i++) {
                    double l = plist[i - 1].length(plist[i]);
                    if (mEps < l)
                        sqeezeList.Add(plist[i]);
                }
            }
            return sqeezeList;
        }

        /// <summary>
        /// 円弧を分割した点座標リストを作成
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <param name="div">真円とした場合の分割数</param>
        /// <returns>座標点リスト</returns>
        public List<PointD> devideArcList(PointD c, double r, double sa, double ea, int div = 32)
        {
            List<PointD> plist = new List<PointD>();
            double da = Math.PI * 2.0 / div;
            if (ea < sa)
                ea += Math.PI * 2.0;
            for (double ang = sa; ang < ea; ang += da) {
                PointD pa = new PointD(c.x + r, c.y);
                pa.rotatePoint(c, ang);
                plist.Add(pa);
            }
            PointD p = new PointD(c.x + r, c.y);
            p.rotatePoint(c, ea);
            plist.Add(p);
            return plist;
        }

        /// <summary>
        /// 円を分割した点座標リストを作成
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="div">分割数</param>
        /// <returns>点座標リスト</returns>
        public List<PointD> divideCircleList(PointD c, double r, int div = 32)
        {
            List<Point> pList = divideCircleList(c.toPoint(), r, div);
            List<PointD> pointList = new List<PointD>();
            foreach (Point p in pList)
                pointList.Add(new PointD(p));
            return pointList;
        }

        /// <summary>
        /// 円を分割した点座標リストを作成
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="div">分割数</param>
        /// <returns>点座標リスト</returns>
        public List<Point> divideCircleList(Point c, double r, int div = 32)
        {
            List<Point> plist = new List<Point>();
            if (4 < div) {
                for (double a = 0.0; a < Math.PI * 2.0; a += Math.PI * 2.0 / div) {
                    double dx = r * Math.Cos(a);
                    double dy = r * Math.Sin(a);
                    Point p = new Point(c.X + dx, c.Y + dy);
                    plist.Add(p);
                }
            }
            return plist;
        }


        /// <summary>
        /// 一種の中心点(点リストの平均)
        /// </summary>
        /// <param name="pList">点リスト</param>
        /// <returns>点座標</returns>
        public Point averagePoint(List<Point> pList)
        {
            Point ap = new Point();
            foreach (Point lp in pList) {
                ap.X += lp.X;
                ap.Y += lp.Y;
            }
            ap.X /= pList.Count;
            ap.Y /= pList.Count;
            return ap;
        }

        //  ---  数値処理関連  ------

        /// <summary>
        /// 剰余関数 (負数の剰余を正数で返す)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>剰余</returns>
        public double mod(double a, double b)
        {
            double c = (a % b);
            c += c < 0.0 ? b : 0.0;
            return c;
        }

        /// <summary>
        /// 剰余関数 (負数の剰余を正数で返す)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>剰余</returns>
        public int mod(int a, int b)
        {
            int c = (a % b);
            c += c < 0 ? b : 0;
            return c;
        }

        //  ---  行列計算  ---
        //  https://qiita.com/sekky0816/items/8c73a7ec32fd9b040127

        /// <summary>
        /// 単位行列の作成(正方行列のみ)
        /// </summary>
        /// <param name="unit">行列のサイズ</param>
        /// <returns>単位行列</returns>
        public double[,] unitMatrix(int unit)
        {
            double[,] A = new double[unit, unit];
            for (int i = 0; i < unit; i++)
                for (int j = 0; j < unit; j++)
                    if (i == j)
                        A[i, j] = 1;
                    else
                        A[i, j] = 0;
            return A;
        }

        /// <summary>
        /// 転置行列  行列Aの転置A^T
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>転置行列 AT</returns>
        public double[,] matrixTranspose(double[,] A)
        {
            double[,] AT = new double[A.GetLength(1), A.GetLength(0)];
            for (int i = 0; i < A.GetLength(1); i++) {
                for (int j = 0; j < A.GetLength(0); j++) {
                    AT[i, j] = A[j, i];
                }
            }
            return AT;
        }

        /// <summary>
        /// 行列の積  AxB
        /// 行列の積では 結合の法則  (AxB)xC = Ax(BxC) , 分配の法則 (A+B)xC = AxC+BxC , Cx(A+B) = CxA + CxB　が可
        /// 交換の法則は成立しない  AxB ≠ BxA
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <param name="B">行列 B</param>
        /// <returns>行列の積</returns>
        public double[,] matrixMulti(double[,] A, double[,] B)
        {
            double[,] product = new double[A.GetLength(0), B.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++) {
                for (int j = 0; j < B.GetLength(1); j++) {
                    for (int k = 0; k < A.GetLength(1); k++) {
                        product[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return product;
        }

        /// <summary>
        /// 行列の和 A+B
        /// 異なるサイズの行列はゼロ行列にする
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <param name="B">行列 B</param>
        /// <returns>行列の和</returns>
        public double[,] matrixAdd(double[,] A, double[,] B)
        {
            double[,] sum = new double[A.GetLength(0), A.GetLength(1)];
            if (A.GetLength(0) == B.GetLength(0) && A.GetLength(1) == B.GetLength(1)) {
                for (int i = 0; i < A.GetLength(0); i++) {
                    for (int j = 0; j < A.GetLength(1); j++) {
                        sum[i, j] = A[i, j] + B[i, j];
                    }
                }
            }
            return sum;
        }


        /// <summary>
        /// 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列)
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>逆行列</returns>
        public double[,] matrixInverse(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            double[,] invA = new double[n, m];
            double[,] AA = copyMatrix(A);
            if (n == m) {
                //  正方行列
                int max;
                double tmp;
                for (int j = 0; j < n; j++) {
                    for (int i = 0; i < n; i++) {
                        invA[j, i] = (i == j) ? 1 : 0;
                    }
                }
                for (int k = 0; k < n; k++) {
                    max = k;
                    for (int j = k + 1; j < n; j++) {
                        if (Math.Abs(AA[j, k]) > Math.Abs(AA[max, k])) {
                            max = j;
                        }
                    }
                    if (max != k) {
                        for (int i = 0; i < n; i++) {
                            //  入力行列側
                            tmp = AA[max, i];
                            AA[max, i] = AA[k, i];
                            AA[k, i] = tmp;
                            //  単位行列側
                            tmp = invA[max, i];
                            invA[max, i] = invA[k, i];
                            invA[k, i] = tmp;
                        }
                    }
                    tmp = AA[k, k];
                    for (int i = 0; i < n; i++) {
                        AA[k, i] /= tmp;
                        invA[k, i] /= tmp;
                    }
                    for (int j = 0; j < n; j++) {
                        if (j != k) {
                            tmp = AA[j, k] / AA[k, k];
                            for (int i = 0; i < n; i++) {
                                AA[j, i] = AA[j, i] - AA[k, i] * tmp;
                                invA[j, i] = invA[j, i] - invA[k, i] * tmp;
                            }
                        }
                    }
                }
                //  逆行列が計算できなかった場合
                for (int j = 0; j < n; j++) {
                    for (int i = 0; i < n; i++) {
                        if (double.IsNaN(invA[j, i])) {
                            Debug.WriteLine("Error: Unable to compute inverse matrix");
                            invA[j, i] = 0;     //  とりあえず0に置き換える
                        }
                    }
                }
                return invA;
            } else {
                Debug.WriteLine("Error: It is not a squre matrix");
                return invA;
            }
        }

        /// <summary>
        /// 行列のコピー
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>コピー行列</returns>
        public double[,] copyMatrix(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            double[,] AA = new double[n, m];
            for (int i = 0; i < A.GetLength(0); i++) {
                for (int j = 0; j < A.GetLength(1); j++) {
                    AA[i, j] = A[i, j];
                }
            }
            return AA;
        }

        //  ---  座標変換関連  ------


        //  ---  単位変換関連  ------

        /// <summary>
        /// 度分秒表示の抽出するための正規表現パターン
        /// </summary>
        private string[] mCoordinatePattern = {
                    "北緯(.*?)度(.*?)分(.*?)秒.*?東経(.*?)度(.*?)分(.*?)秒",
                    "北緯(.*?)度(.*?)分.*?東経(.*?)度(.*?)分",
                    "北緯(.*?)度.*?東経(.*?)度",
                };

        /// <summary>
        /// 度分秒表示の座標があればその文字列を返す
        /// </summary>
        /// <param name="coordinate">対象文字列</param>
        /// <returns>検索文字列</returns>
        public string getCoordinatePattern(string coordinate)
        {
            foreach (string pattern in mCoordinatePattern) {
                List<string[]> datas = getPattern(coordinate.Trim(), pattern);
                if (0 < datas.Count) {
                    return datas[0][0];
                }
            }
            return "";
        }

        /// <summary>
        /// 度分秒表示の座標を度に変換する(IndexOf使うことで正規表現より早い)
        /// 北緯45度22分21秒 東経141度00分57秒 → 緯度 45.3725 経度 141.015833333333 
        /// </summary>
        /// <param name="coordinate">度分秒文字列</param>
        /// <returns>座標値</returns>
        public Point cnvCoordinate(string coordinate)
        {
            char[] stripChars = new char[] { ' ', '.' };
            double latitude = 0.0;
            double longitude = 0.0;
            int a1 = coordinate.IndexOf("北緯");
            int b1 = coordinate.IndexOf("東経");
            if (a1 < 0 || b1 < 0)
                return new Point(0, 0);
            int n = coordinate.IndexOf("北緯", b1);
            if (0 <= n)
                coordinate = coordinate.Substring(0, n);
            int a2 = coordinate.IndexOf("度", a1);
            if (0 <= a2 && a2 < b1) {
                string buf = coordinate.Substring(a1 + 2, a2 - a1 - 2);
                latitude = buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars));
                int a3 = coordinate.IndexOf("分", a1);
                if (0 <= a3 && a3 < b1) {
                    buf = coordinate.Substring(a2 + 1, a3 - a2 - 1);
                    latitude += buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars)) / 60.0;
                    int a4 = coordinate.IndexOf("秒", a1);
                    if (0 <= a4 && a4 < b1) {
                        buf = coordinate.Substring(a3 + 1, a4 - a3 - 1);
                        latitude += buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars)) / 3600.0;
                    }
                }
            }

            int b2 = coordinate.IndexOf("度", b1);
            if (0 <= b2) {
                string buf = coordinate.Substring(b1 + 2, b2 - b1 - 2);
                longitude = buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars));
                int b3 = coordinate.IndexOf("分", b1);
                if (0 <= b3) {
                    buf = coordinate.Substring(b2 + 1, b3 - b2 - 1);
                    longitude += buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars)) / 60.0;
                    int b4 = coordinate.IndexOf("秒", b1);
                    if (0 <= b4) {
                        buf = coordinate.Substring(b3 + 1, b4 - b3 - 1);
                        longitude += buf.Length == 0 ? 0 : double.Parse(buf.Trim(stripChars)) / 3600.0;
                    }
                }
            }

            return new Point(longitude, latitude);
        }

        /// <summary>
        /// 度分秒表示の座標を度に変換する(正規表現を使う)
        /// 北緯45度22分21秒 東経141度00分57秒 → 緯度 45.3725 経度 141.015833333333 
        /// </summary>
        /// <param name="coordinate">座標</param>
        /// <returns></returns>
        public Point cnvCoordinate2(string coordinate)
        {
            char[] stripChars = new char[] { ' ', '.' };
            List<string[]> datas = getPattern(coordinate.Trim(), mCoordinatePattern[0]);
            if (0 < datas.Count) {
                double latitude = 0.0;
                double longitude = 0.0;
                foreach (string[] data in datas) {
                    if (6 < data.Length) {
                        latitude = data[1].Length == 0 ? 0 : double.Parse(data[1].TrimEnd(stripChars));
                        latitude += data[2].Length == 0 ? 0 : double.Parse(data[2].TrimEnd(stripChars)) / 60.0;
                        latitude += data[3].Length == 0 ? 0 : double.Parse(data[3].TrimEnd(stripChars)) / 3600.0;
                        longitude = data[4].Length == 0 ? 0 : double.Parse(data[4].TrimEnd(stripChars));
                        longitude += data[5].Length == 0 ? 0 : double.Parse(data[5].TrimEnd(stripChars)) / 60.0;
                        longitude += data[6].Length == 0 ? 0 : double.Parse(data[6].TrimEnd(stripChars)) / 3600.0;
                        break;
                    }
                }

                return new Point(longitude, latitude);
            }
            //  秒なし
            datas = getPattern(coordinate.Trim(), mCoordinatePattern[1]);
            if (0 < datas.Count) {
                double latitude = 0.0;
                double longitude = 0.0;
                foreach (string[] data in datas) {
                    if (4 < data.Length) {
                        latitude = data[1].Length == 0 ? 0 : double.Parse(data[1].TrimEnd(stripChars));
                        latitude += data[2].Length == 0 ? 0 : double.Parse(data[2].TrimEnd(stripChars)) / 60.0;
                        longitude = data[3].Length == 0 ? 0 : double.Parse(data[3].TrimEnd(stripChars));
                        longitude += data[4].Length == 0 ? 0 : double.Parse(data[4].TrimEnd(stripChars)) / 60.0;
                        break;
                    }
                }

                return new Point(longitude, latitude);
            }
            //  分・秒なし
            datas = getPattern(coordinate.Trim(), mCoordinatePattern[2]);
            if (0 < datas.Count) {
                double latitude = 0.0;
                double longitude = 0.0;
                foreach (string[] data in datas) {
                    if (2 < data.Length) {
                        latitude = data[1].Length == 0 ? 0 : double.Parse(data[1].TrimEnd(stripChars));
                        longitude = data[2].Length == 0 ? 0 : double.Parse(data[2].TrimEnd(stripChars));
                        break;
                    }
                }

                return new Point(longitude, latitude);
            }

            return new Point(0.0, 0.0);
        }

        /// <summary>
        /// ラジアンから度に変換
        /// </summary>
        /// <param name="rad">ラジアン(rad)</param>
        /// <returns>度(deg)</returns>
        public double R2D(double rad)
        {
            return rad * 180.0 / Math.PI;
        }

        /// <summary>
        /// 度からラジアンに変換
        /// </summary>
        /// <param name="deg">度(deg)</param>
        /// <returns>ラジアン(rad)</returns>
        public double D2R(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        /// <summary>
        /// 時(hh.hhhh)から度(ddd.dddd)
        /// </summary>
        /// <param name="hour">時(hh.hhhh)</param>
        /// <returns>度(ddd.dddd)</returns>
        public double H2D(double hour)
        {
            // return Math.Floor(hour) * 360 / 24 + hour % 1.0;
            return hour * 360.0 / 24.0;
        }

        /// <summary>
        /// 度()ddd.dddd)から時(hh.hhhh)に変換
        /// </summary>
        /// <param name="deg">度(ddd.dddd)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double D2H(double deg)
        {
            return deg * 24.0 / 360.0;
        }

        /// <summary>
        /// 時(hh.hhhh)からラジアンに変換
        /// </summary>
        /// <param name="hour">時(hh.hhhh)</param>
        /// <returns>ラジアン(rad)</returns>
        public double H2R(double hour)
        {
            return hour * Math.PI / 12.0;
        }

        //  radian → hour(h.hhhh)
        /// <summary>
        /// ラジアンから時(hh.hhhh)に変換
        /// </summary>
        /// <param name="rad">ラジアン(rad)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double R2H(double rad)
        {
            return rad * 12.0 / Math.PI;
        }

        private char[] mDMSChar = { '°', '′', '″' };

        /// <summary>
        /// 度(ddd.dddd)を度分秒(dd°mm′ss″)文字列に変換
        /// </summary>
        /// <param name="deg">度(ddd.ddddd)</param>
        /// <returns>度分秒(dd°mm′ss″)</returns>
        public string deg2DMS(double deg)
        {
            double deg2 = Math.Abs(deg);
            string dms = (deg < 0 ? "-" : "+") + Math.Floor(deg2).ToString("00") + mDMSChar[0];
            dms += Math.Floor((deg2 % 1.0) * 60).ToString("00") + mDMSChar[1];
            dms += (((deg2 * 60.0) % 1.0) * 60).ToString("00.##") + mDMSChar[2];
            return dms;
        }

        /// <summary>
        /// 度分秒(dd°mm′ss″)文字列を度(ddd.dddd)に変換
        /// </summary>
        /// <param name="dms">度分秒(dd°mm′ss″)</param>
        /// <returns>度(ddd.dddd)</returns>
        public double DMS2deg(string dms)
        {
            string dms2 = dms[0] == '-' ? dms.Substring(1) : dms;
            double d = doubleParse(dms2.Substring(0, dms2.IndexOf(mDMSChar[0])));
            double m = doubleParse(dms2.Substring(dms2.IndexOf(mDMSChar[0]) + 1, dms2.IndexOf(mDMSChar[1]) - dms2.IndexOf(mDMSChar[0]) - 1));
            double s = doubleParse(dms2.Substring(dms2.IndexOf(mDMSChar[1]) + 1, dms2.IndexOf(mDMSChar[2]) - dms2.IndexOf(mDMSChar[1]) - 1));
            return (d + m / 60.0 + s / 3600.0) * (dms[0] == '-' ? -1.0 : 1.0);
        }

        /// <summary>
        /// 時(hh.hhhh)を時分秒(HHhMMmSSs)文字列に変換
        /// </summary>
        /// <param name="hour">時(hh.hhhh)</param>
        /// <returns>時分秒(HHhMMmSSs)</returns>
        public string hour2HMS(double hour)
        {
            double hour2 = hour % 24.0;
            hour2 = hour2 < 0 ? hour2 + 24 : hour2;
            string hms = (hour < 0 ? "-" : "") + Math.Floor(hour2).ToString("00") + "h";
            hms += Math.Floor((hour2 % 1.0) * 60).ToString("00") + "m";
            hms += (((hour2 * 60.0) % 1.0) * 60).ToString("00.##") + "s";
            return hms;
        }

        /// <summary>
        /// 時分秒(HHhMMmSSs)文字列を時(hh.hhhh)に変換
        /// </summary>
        /// <param name="dms">時分秒(HHhMMmSSs)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double HMS2hour(string hms)
        {
            string hms2 = hms[0] == '-' ? hms.Substring(1) : hms;
            double h = doubleParse(hms2.Substring(0, hms2.IndexOf("h")));
            double m = doubleParse(hms2.Substring(hms2.IndexOf("h") + 1, hms2.IndexOf("m") - hms2.IndexOf("h") - 1));
            double s = doubleParse(hms2.Substring(hms2.IndexOf("m") + 1, hms2.IndexOf("s") - hms2.IndexOf("m") - 1));
            return (h + m / 60.0 + s / 3600.0) * (hms[0] == '-' ? -1.0 : 1.0);
        }

        /// <summary>
        /// 時分秒(hhmmss)文字列を時(hh.hhhh)に変換
        /// </summary>
        /// <param name="hhmmss">時分秒(hhmmss)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double hhmmss2hour(string hhmmss)
        {
            double h = doubleParse(hhmmss.Substring(0, 2));
            double m = doubleParse(hhmmss.Substring(2, 2));
            double s = doubleParse(hhmmss.Substring(4, 2));
            return h + m / 60.0 + s / 3600.0;
        }

        /// <summary>
        /// 度分秒(dd,mm,ss)を度(ddd.dddd)
        /// </summary>
        /// <param name="d">度</param>
        /// <param name="m">分</param>
        /// <param name="s">秒</param>
        /// <returns>度(ddd.dddd)</returns>
        public double DMS2deg(int d, int m, double s)
        {
            return d + m / 60.0 + s / 3600.0;
        }

        //  hms → hour
        /// <summary>
        /// 時分秒(hh,mm,ss)を時(hh.hhhh)に変換
        /// </summary>
        /// <param name="h">時</param>
        /// <param name="m">分</param>
        /// <param name="s">秒</param>
        /// <returns>時(hh.hhhh)</returns>
        public double HMS2hour(int h, int m, double s)
        {
            return h + m / 60.0 + s / 3600.0;
        }

        /// <summary>
        /// 時分(hhmm.m)を時(hh.hhhh)に変換
        /// </summary>
        /// <param name="hm">時分(hhmm.m)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double HM2hour(string hm)
        {
            return DM2deg(hm);
        }

        /// <summary>
        /// 度分(±ddmm.m)を度(ddd.dddd)に変換
        /// </summary>
        /// <param name="hm">度分(±ddmm.m)</param>
        /// <returns>度(ddd.dddd)</returns>
        public double DM2deg(string dm)
        {
            int n = 0;
            int sign = 1;
            if (dm[0] == '+') {
                n = 1;
            } else if (dm[0] == '-') {
                n = 1;
                sign = -1;
            }
            double d = doubleParse(dm.Substring(n, 2));
            double m = 0.0;
            if (n + 2 < dm.Length)
                m = doubleParse(dm.Substring(n + 2));
            return (d + m / 60.0) * sign;
        }

        /// <summary>
        /// 度分秒(±dd:mm:ss)を度(dd.ddddに変換
        /// </summary>
        /// <param name="dm">度分秒(dd:mm:ss</param>
        /// <returns>度(dd.dddd)</returns>
        public double dm2deg(string dm)
        {
            return hm2hour(dm);
        }

        /// <summary>
        /// 時分(hh:mm)を時(hh.hhhh)に変換
        /// </summary>
        /// <param name="hm">時分(hh:mm)</param>
        /// <returns>時(hh.hhhh)</returns>
        public double hm2hour(string hm)
        {
            string[] text = hm.Split(':');
            if (0 < text.Length) {
                int n = 0;
                double sign = 1.0;
                if (text[0][0] == '+') {
                    n = 1;
                } else if (text[0][0] == '-') {
                    n = 1;
                    sign = -1.0;
                }
                double hour = string2double(text[0].Substring(n));
                for (int i = 1; i < text.Length; i++) {
                    hour += string2double(text[i]) / Math.Pow(60.0, i);
                }
                return hour *sign;
            }
            return 0.0;
        }
    }
}
