using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CoreLib
{
    /// <summary>
    /// 汎用ライブラリ
    /// 
    /// ---  API関数  ------
    /// List<string> getSystemFontFamilyName(bool languageSelect = false)   フォント名リストの取得
    /// FontStyle convFontStyle(string style)           文字列をFontStyleに変換して変換
    /// FontWeight convFontWeight(string weight)        文字列をFontWeightに変換して変換
    /// int GetWindowRect(IntPtr hWnd, out iRect rect)  ウインドウの外側のサイズを取得
    /// IntPtr GetForegroundWindow()                    フォアグラウンドウィンドウ(ActiveWindow)の取得
    /// short GetKeyState(int nVirtkey)                 クリックされているか判定用
    /// bool IsClickDownLeft()                          マウス左ボタン(0x01)(VK_LBUTTON)の状態
    /// bool IsClickDownRight()                         マウス右ボタン(0x02)(VK_RBUTTON)の状態
    /// bool isGetKeyState(int nVirtKey)                マウスやキーボードの状態取得
    /// bool onControlKey()                             コントロールキーの確認
    /// bool onAltKey()                                 Altキーの確認
    /// bool onKey(int keyCode)                         キーの確認
    /// 
    //  ---  システム関連  ------
    //	List<string> getSystemFontFamilyName(bool languageSelect = false)	 フォント名リストの取得(en-us,ja-jp)
    //	System.Windows.FontStyle convFontStyle(string style)	 文字列をFontStyleに変換して変換
    //	System.Windows.FontWeight convFontWeight(string weight)	 文字列をFontWeightに変換して変換
    //	void DoEvents()	                                コントロールを明示的に更新するコード
    //	object ExitFrames(object arg)	                DoEventsからのコールバック
    //	string getLastFolder(string folder)	            フルパスのディレクトリから最後のフォルダ名を取り出す
    //	string getLastFolder(string folder, int n)	    フルパスのディレクトリから最後のフォルダ名を取り出す
    //	string getAppFolderPath()	                    実行ファイルのフォルダを取得(アプリケーションが存在するのディレクトリ)
    //	string getAppName(bool fullpath = true)	        実行プログラム名をフルパスで取得
    //	void fileExecute(string path)	                ファイルを実行する
    //	bool processStart(string prog, string arg)	    プログラムの実行
    //	void openUrl(string url)	                    URLを標準ブラウザで開く
    //	static void Swap<T>(ref T lhs, ref T rhs)	    ジェネリックによるswap関数
    //	MessageBoxResult messageBox(Window owner, string message, string title = "",	 メッセージダイヤログ(親Windowの中央に表示)
    //	(MessageBoxResult result, bool cont) messageBoxEx(Window owner, string message, string title = "",	 メッセージ表示ダイヤログ
    //	void setClipboard(List<string> fileList, int effect)	 クリップボードにDropEffectを設定
    //	(List<string> files, int copyType) getClipboardFiles()	 クリップボードからDropFileを取り出す
    ///
    // ---  ストップウォッチ  ---
    //	void stopWatchStartNew()	    ストップウォッチ機能初期化・計測開始
    //	TimeSpan stopWatchLapTime()	    ストップウォッチ機能ラップ時間取得
    //	TimeSpan stopWatchRestart()	    ストップウォッチ機能計測時間の取得と再スタート
    //	TimeSpan stopWatchStop()	    ストップウォッチ機能計測時間の取得と終了
    //	TimeSpan stopWatchTotalTime()	 ストップウォッチ機能累積時間の取得
    ///
    //  ---  ネットワーク関連  ---
    //	bool webFileDownload(string url, string filePath)	 Web上のファイルをダウンロードする
    //	string getWebText(string url)	                    URLのWebデータの読込
    //	string getWebText(string url, int encordType = 0)	 エンコードタイプを指定してURLのWebデータの読込
    ///
    //  ---  文字列処理関連  -----
    //	bool wcMatch(string srcstr, string pattern)	                ワイルドカードによる文字検索
    //	int indexOf(string text, string val, int count = 1)	        文字列を前から指定位置を検索する
    //	int lastIndexOf(string text, string val, int count = 1)	    文字列を後から指定位置を検索する
    //	string stripBrackets(string text, char sb = '[', char eb = ']')	 文字列から括弧で囲まれた領域を取り除く
    //	List<string> extractBrackets(string text, char sb = '{', char eb = '}', bool withBracket = false)	 括弧で囲まれた文字列を抽出する(ディフォルトは'{','}')
    //	string trimControllCode(string buf)	                        文字列内のコントロールコードを除去する
    //	string HanCode =	private string ZenCode =
    //	string strZne2Han(string zenStr)	                        文字列内の全角英数字を半角に変換する
    //	string HanNumCode = "0123456789.+-";	private string ZenNumCode = "０１２３４５６７８９．＋－";
    //	string strNumZne2Han(string zenStr)	                        文字列内の全角数値を半角に変換する
    //	string strControlCodeCnv(string str)	                    文字列の中の改行コード、','、'"'を'\'付きコードに置き換える
    //	string strControlCodeRev(string str)	                    文字列の中の'\'付きコードを通常のコードに戻す
    //	string stripControlCode(string str)	                        文字列からコントロールコードを除外する
    //	string[] seperateString(string str)	                        文字列をカンマセパレータで分解して配列に格納する
    //	List<string> splitString(string str, string[] sep)	        文字列を指定文字列で分ける
    //	string stripBracketString(string str, char bracket = '(')	括弧付き文字列で前後の括弧を除いた文字列
    //	string getBracketString(string str, int n, char bracket = '(', bool withBracket = true)	 括弧で囲まれた文字列の抽出(括弧含む)
    //	string insertLinefeed(string str, string word = ",", int lineSize = 80)	 一行が指定文字数を超えたら改行を入れる
    //	List<string> getPattern(string html, string pattern, string group)	 正規表現を使ったHTMLデータからパターン抽出
    //	List<string[]> getPattern(string html, string pattern)	    正規表現を使ったHTMLからのパターン抽出
    //	List<string[]> getPattern2(string html, string pattern)	    正規表現を使ったHTMLからのパターン抽出
    //	int getStrByteCount(string str)	                            文字列の長さをbyte数で求める
    //	string stringUncode2String(string text)	                    Unicode文字コードの文字列を文字列に変換
    //	string tab2space(string text, int tabsize = 4)	            タブをスペースに変換

    //  ---  数値処理  ----------
    //	bool IsNumberString(string num, bool allNum = false)	    数値文字列かを判定する
    //	string double2StrZeroSup(double val, string form = "f1")	数値をゼロサプレスして文字列に変換
    //	bool boolParse(string str, bool val = true)	                文字列を論理値に変換
    //	int intParse(string str, int val = 0)	                    文字列を整数に変換
    //	double doubleParse(string str, double val = 0.0)	        文字列を実数に変換
    //	int string2int(string num)	                                文字列の先頭が数値の場合、数値に変換する
    //	double string2double(string num)	                        文字列の先頭が数値の場合、数値に変換する
    //	List<string> string2StringNumbers(string num)	            文字列の中から複数の数値文字列を抽出する
    //	string numChar = ".0123456789";	private string startNum = "+-123456789";
    //	string string2StringNum(string num)	                        文字列から数値に関係ない文字を除去し、実数に変換できる文字列にする

    ///
    //  --- データ処理関係  ------
    //	string arrayStr2CsvData(string[] data)	                            配列データをCSVデータに変換
    //	string[] csvData2ArrayStr(string line)	                            CSVデータを配列データに戻す
    //	List<string[]> splitJson(string jsonData, string baseTitle = "")    JSON形式の文字列から[名前:値]の対データをリストデータとして取得する
    //	string getJsonDataString(string jsonData)	                        JSON形式の文字列から{}内の文字列を取得する
    ///
    //  ---  日付・時間関連  ------
    //	int date2JulianDay(int year, int month, int day)	        歴日からユリウス日に変換
    //	double getJD(int nYear, int nMonth, int nDay, int nHour = 0, int nMin = 0, int nSec = 0)	 ユリウス日(紀元前4713年(-4712年)1月1日が0日)の取得 (https://www.dinop.com/vc/getjd.html)
    //	int julianDay(int y, int m, int d)	                        天文年鑑の数式でユリウス日を求める
    //	double getMJD(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)	 準ユリウス日(1582年10月15日が0日)の取得
    //	(int year, int month, int day) JulianDay2Date(double jd)	ユリウス日から西暦(年月日)を求める
    //	(int hour, int min, int sec) JulianDay2Time(double jd)	    ユリウス日から時間(時分秒)の取得
    //	string JulianDay2DateYear(double jd, int type = 0)	        ユリウス日から歴日文字列に変換
    //	double getGreenwichSiderealTime(int nYear, int nMonth, int nDay, int nHour, int nMin, int nSec)	 恒星時(Wikipedia https://ja.wikipedia.org/wiki/恒星時)
    //	double getLocalSiderealTime(double dSiderealTime, double dLongitude)	 地方恒星時の取得
    ///
    //  ---  グラフィック処理関連  ------
    //	ColorTitle[] getColorList()	                                Media.ColorsよりColor Listを取得
    //	int getBrushNo(System.Windows.Media.Brush color)	        BrushListからListNoの取得
    //	string getBrushName(System.Windows.Media.Brush color)	    Brush を色名に変換
    //	System.Windows.Media.Brush getBrsh(string colorName)	    色名を Brush値に変換
    //	string getColorName(System.Windows.Media.Brush color)	    Brush を色名に変換
    //	string getColorName(int colorNo)	                        色番号から色名を取得
    //	System.Windows.Media.Brush getColor(string color)	        色名を Brush値に変換
    //	System.Windows.Point rotateOrg(System.Windows.Point po, double rotate)	 原点を中心に回転
    //	System.Windows.Point rotatePoint(System.Windows.Point ctr, System.Windows.Point po, double rotate)	 回転中心を指定して回転
    //	double angleOrg(System.Windows.Point po)	                原点に対する座標点の水平線との角度(rad)
    //	double anglePoint(System.Windows.Point ctr, System.Windows.Point po)	 中心座標を指定して回転角度(rad)
    //	int angleQuadrant(double ang)	                            角度の象限を求める(0-3)
    //	List<System.Windows.Point> circlePeakPoint(System.Windows.Point c, double r)	 円の4頂点?を求める
    //	List<PointD> arcPeakPoint(PointD c, double r, double sa, double ea)	 円弧の頂点リストを求める
    //	List<System.Windows.Point> arcPeakPoint(System.Windows.Point c, double r, double sa, double ea)	 円弧の頂点リストを求める
    //	List<PointD> pointSort(List<PointD> plist)	                点リストを中心点の角度でソート
    //	List<System.Windows.Point> pointSort(List<System.Windows.Point> plist)	 点リストを中心点の角度でソート
    //	List<PointD> pointListSqueeze(List<PointD> plist)	        点座標リストで重複点を削除
    //	List<PointD> devideArcList(PointD c, double r, double sa, double ea, int div = 32)	 円弧を分割した点座標リストを作成
    //	List<PointD> divideCircleList(PointD c, double r, int div = 32)	 円を分割した点座標リストを作成
    //	List<System.Windows.Point> divideCircleList(System.Windows.Point c, double r, int div = 32)	 円を分割した点座標リストを作成
    //	System.Windows.Point averagePoint(List<System.Windows.Point> pList)	 一種の中心点(点リストの平均位置)
    //	PointD nearPoint(List<PointD> plist, PointD pos)	        座標点リストで指定点に最も近い点
    ///
    //  ---  ビットマップ処理  ----
    //	Bitmap getScreen(System.Drawing.Point ps, System.Drawing.Point pe)	画面の指定領域をキャプチャする
    //	Bitmap getActiveWindowCapture()	                                    アクティブウィンドウの画面をキャプチャする
    //	System.Drawing.Bitmap getFullScreenCapture()	                    全画面をキャプチャする
    //	System.Drawing.Bitmap getBitmap(string path)	                    画像ファイルからBitmapを取得
    //	BitmapImage getBitmapImage(string path)	                            画像ファイルからBitmapImageを取得
    //	BitmapImage getThumbnailImage(string path, int width, int height, bool exif = true)	 JPEGのサムネイル画像を取得する
    //	BitmapImage cnvBitmap2BitmapImage(System.Drawing.Bitmap bitmap,	 BitmapをBitmapImageに変換
    //	BitmapImage getExifThumbnailImage(string path)	                    JPEGのExifサムネイル画像(0x501B)を取出す
    //	BitmapImage cnvBitmap2BitmapImage(System.Drawing.Bitmap bitmap)	    BitmapをBitmapImageに変換
    //	System.Drawing.Bitmap cnvBitmapImage2Bitmap(BitmapImage bitmapImage)	 BitmapImageをBitmapに変換
    //	System.Drawing.Bitmap cnvBitmapSource2Bitmap(BitmapSource bitmapSource)	 BitmapSourceをBitmapに変換
    //	BitmapSource bitmap2BitmapSource(System.Drawing.Bitmap bitmap)	    BitMap からBitmapSourceに変換
    //	BitmapImage bitmapSource2BitmapImage(BitmapSource bitmapSource)	    BitmapSourceからBitmapImageに変換(動作未確認)
    //	System.Drawing.Bitmap trimingBitmap(System.Drawing.Bitmap bitmap, System.Windows.Point sp, System.Windows.Point ep)	 Bitmapデータをトリミングする
    //	int setCanvasBitmapImage(System.Windows.Controls.Canvas canvas, BitmapImage bitmapImage, double ox, double oy, double width, double height)	 BitmapImageをCanvasに登録する
    //	System.Drawing.Bitmap verticalCombineImage(System.Drawing.Bitmap[] src)	 画像を縦方向に連結
    //	System.Drawing.Bitmap horizontalCombineImage(System.Drawing.Bitmap[] src)	 画像の水平方向に連結
    //	void saveBitmapImage(BitmapSource bitmapSource, string filePath)	 画像データをファイルに保存
    //	System.Windows.Media.Color Draw2MediaColor(System.Drawing.Color color)	 Drawing.Color から Media.Color に変換
    //	System.Drawing.Color Media2DrawColor(System.Windows.Media.Color color)	 Media.Color から Drawing.Colorに変換
    //	BitmapSource canvas2Bitmap(Canvas canvas, double offsetX = 0, double offsetY = 0)	 CanvasをBitmapに変換
    //	void moveImage(Canvas canvas, BitmapSource bitmapSource, double dx, double dy, int offset = 0)	 Bitmap 図形を移動させる
    //	void image2Clipbord(System.Windows.Media.ImageSource imageSource)   Imageコントロールの画像をクリップボードに貼り付ける
    //	List<string> getIPTC(string path)	                                JPEGファイルからIPTC情報の取得
    //	string getIPTCall(string path)	                                    JPEGファイルの全IPTC情報を文字列化
    ///
    //  ---  配列処理  ---
    //	double[,] doubleArray2Sort(double[,] data, int col = 0)	 2次元配列のソート(数値)
    //	string[,] stringArray2Sort(string[,] data, int col = 0)	 2次元配列のソート(文字列))
    ///
    //  ---  単位変換関連  ------
    //	string getCoordinatePattern(string coordinate)	        度分秒表示の座標があればその文字列を返す
    //	System.Windows.Point cnvCoordinate(string coordinate)	度分秒表示の座標を度に変換する(IndexOf使うことで正規表現より早い)
    //	System.Windows.Point cnvCoordinate2(string coordinate)	度分秒表示の座標を度に変換する(正規表現を使う)
    //	double R2D(double rad)	                                ラジアンから度に変換
    //	double D2R(double deg)	                                度からラジアンに変換
    //	double H2D(double hour)	                                時(hh.hhhh)から度(ddd.dddd)
    //	double D2H(double deg)	                                度()ddd.dddd)から時(hh.hhhh)に変換
    //	double H2R(double hour)	                                時(hh.hhhh)からラジアンに変換
    //	double R2H(double rad)	                                ラジアンから時(hh.hhhh)に変換
    //	string deg2DMS(double deg)	                            度(ddd.dddd)を度分秒(dd°mm′ss″)文字列に変換
    //	double DMS2deg(string dms)	                            度分秒(dd°mm′ss″)文字列を度(ddd.dddd)に変換
    //	string hour2HMS(double hour)	                        時(hh.hhhh)を時分秒(HHhMMmSSs)文字列に変換
    //	double HMS2hour(string hms)	                            時分秒(HHhMMmSSs)文字列を時(hh.hhhh)に変換
    //	double hhmmss2hour(string hhmmss)	                    時分秒(hhmmss)文字列を時(hh.hhhh)に変換
    //	double DMS2deg(int d, int m, double s)	                度分秒(dd,mm,ss)を度(ddd.dddd)
    //	double HMS2hour(int h, int m, double s)	                時分秒(hh,mm,ss)を時(hh.hhhh)に変換
    //	double HM2hour(string hm)	                            時分(hhmm.m)を時(hh.hhhh)に変換
    //	double DM2deg(string dm)	                            度分(±ddmm.m)を度(ddd.dddd)に変換
    //	double dm2deg(string dm)    	                        度分秒(±dd:mm:ss)を度(dd.ddddに変換
    //	double hm2hour(string hm)	                            時分(hh:mm)を時(hh.hhhh)に変換
    ///
    //  ---  bit演算関連  ------
    //	List<int> getBitOnNo(byte[] bytes)	                    OnBit位置の取得
    //	bool IsEmpty(byte[] bytes)	                            Bit配列がすべて 0 かの確認
    //	bool bitAnd(byte[] bytesA, byte[] bytesB)	            2つの　bitリストの and で同一のbitがあればtrue
    //	bool getBitOn(byte[] bytes, int n)	                    Bitデータで指定位置のBitがOnかの確認
    //	void bitOnAll(byte[] bytes, bool off = false)	        すべてのBitをOn/Offにする
    //	void bitOn(byte[] bytes, int n)	                        BitデータをBit位置指定でOnにする
    //	void bitOff(byte[] bytes, int n)	                    BitデータをBit位置指定でOffにする
    //	string binary2HexString(byte[] bytes, int start = 0, int size = 0)	 16進文字列に変換
    //	string binary2AsciiString(byte[] data, int start = 0, int size = 0)	 byte配列をASCii文字列に変換
    //	byte[] intList2ByteArray(List<Int32> intlist)	        intリストをbyte配列に変換
    //	byte[] ByteCopy(byte[] a, int start, int size)	        byteデータの指定範囲をコピーする
    //	byte[] ByteOverWrite(byte[] src, int start, byte[] dest)    byte配列にbyteデータを上書きする
    //	byte[] ByteCat(byte[] src1, byte[] src2)	            byte配列同士を連結する
    /// </summary>
    public class YLib
    {
        //  色名と色(Brush)のrecord
        public record ColorTitle(string colorTitle, System.Windows.Media.Brush brush);
        //  140色の色パターン
        public List<ColorTitle> mColorList = new List<ColorTitle>() {
            new ColorTitle("AliceBlue", System.Windows.Media.Brushes.AliceBlue),
            new ColorTitle("AntiqueWhite", System.Windows.Media.Brushes.AntiqueWhite),
            new ColorTitle("Aqua", System.Windows.Media.Brushes.Aqua),
            new ColorTitle("Aquamarine", System.Windows.Media.Brushes.Aquamarine),
            new ColorTitle("Azure", System.Windows.Media.Brushes.Azure),
            new ColorTitle("Beige", System.Windows.Media.Brushes.Beige),
            new ColorTitle("Bisque", System.Windows.Media.Brushes.Bisque),
            new ColorTitle("Black", System.Windows.Media.Brushes.Black),
            new ColorTitle("BlanchedAlmond", System.Windows.Media.Brushes.BlanchedAlmond),
            new ColorTitle("Blue", System.Windows.Media.Brushes.Blue),
            new ColorTitle("BlueViolet", System.Windows.Media.Brushes.BlueViolet),
            new ColorTitle("Brown", System.Windows.Media.Brushes.Brown),
            new ColorTitle("BurlyWood", System.Windows.Media.Brushes.BurlyWood),
            new ColorTitle("CadetBlue", System.Windows.Media.Brushes.CadetBlue),
            new ColorTitle("Chartreuse", System.Windows.Media.Brushes.Chartreuse),
            new ColorTitle("Chocolate", System.Windows.Media.Brushes.Chocolate),
            new ColorTitle("Coral", System.Windows.Media.Brushes.Coral),
            new ColorTitle("CornflowerBlue", System.Windows.Media.Brushes.CornflowerBlue),
            new ColorTitle("Cornsilk", System.Windows.Media.Brushes.Cornsilk),
            new ColorTitle("Crimson", System.Windows.Media.Brushes.Crimson),
            new ColorTitle("Cyan", System.Windows.Media.Brushes.Cyan),
            new ColorTitle("DarkBlue", System.Windows.Media.Brushes.DarkBlue),
            new ColorTitle("DarkCyan", System.Windows.Media.Brushes.DarkCyan),
            new ColorTitle("DarkGoldenrod", System.Windows.Media.Brushes.DarkGoldenrod),
            new ColorTitle("DarkGray", System.Windows.Media.Brushes.DarkGray),
            new ColorTitle("DarkGreen", System.Windows.Media.Brushes.DarkGreen),
            new ColorTitle("DarkKhaki", System.Windows.Media.Brushes.DarkKhaki),
            new ColorTitle("DarkMagenta", System.Windows.Media.Brushes.DarkMagenta),
            new ColorTitle("DarkOliveGreen", System.Windows.Media.Brushes.DarkOliveGreen),
            new ColorTitle("DarkOrange", System.Windows.Media.Brushes.DarkOrange),
            new ColorTitle("DarkOrchid", System.Windows.Media.Brushes.DarkOrchid),
            new ColorTitle("DarkRed", System.Windows.Media.Brushes.DarkRed),
            new ColorTitle("DarkSalmon", System.Windows.Media.Brushes.DarkSalmon),
            new ColorTitle("DarkSeaGreen", System.Windows.Media.Brushes.DarkSeaGreen),
            new ColorTitle("DarkSlateBlue", System.Windows.Media.Brushes.DarkSlateBlue),
            new ColorTitle("DarkSlateGray", System.Windows.Media.Brushes.DarkSlateGray),
            new ColorTitle("DarkTurquoise", System.Windows.Media.Brushes.DarkTurquoise),
            new ColorTitle("DarkViolet", System.Windows.Media.Brushes.DarkViolet),
            new ColorTitle("DeepPink", System.Windows.Media.Brushes.DeepPink),
            new ColorTitle("DeepSkyBlue", System.Windows.Media.Brushes.DeepSkyBlue),
            new ColorTitle("DimGray", System.Windows.Media.Brushes.DimGray),
            new ColorTitle("DodgerBlue", System.Windows.Media.Brushes.DodgerBlue),
            new ColorTitle("Firebrick", System.Windows.Media.Brushes.Firebrick),
            new ColorTitle("FloralWhite", System.Windows.Media.Brushes.FloralWhite),
            new ColorTitle("ForestGreen", System.Windows.Media.Brushes.ForestGreen),
            new ColorTitle("Fuchsia", System.Windows.Media.Brushes.Fuchsia),
            new ColorTitle("Gainsboro", System.Windows.Media.Brushes.Gainsboro),
            new ColorTitle("GhostWhite", System.Windows.Media.Brushes.GhostWhite),
            new ColorTitle("Gold", System.Windows.Media.Brushes.Gold),
            new ColorTitle("Goldenrod", System.Windows.Media.Brushes.Goldenrod),
            new ColorTitle("Gray", System.Windows.Media.Brushes.Gray),
            new ColorTitle("Green", System.Windows.Media.Brushes.Green),
            new ColorTitle("GreenYellow", System.Windows.Media.Brushes.GreenYellow),
            new ColorTitle("Honeydew", System.Windows.Media.Brushes.Honeydew),
            new ColorTitle("HotPink", System.Windows.Media.Brushes.HotPink),
            new ColorTitle("IndianRed", System.Windows.Media.Brushes.IndianRed),
            new ColorTitle("Indigo", System.Windows.Media.Brushes.Indigo),
            new ColorTitle("Ivory", System.Windows.Media.Brushes.Ivory),
            new ColorTitle("Khaki", System.Windows.Media.Brushes.Khaki),
            new ColorTitle("Lavender", System.Windows.Media.Brushes.Lavender),
            new ColorTitle("LavenderBlush", System.Windows.Media.Brushes.LavenderBlush),
            new ColorTitle("LawnGreen", System.Windows.Media.Brushes.LawnGreen),
            new ColorTitle("LemonChiffon", System.Windows.Media.Brushes.LemonChiffon),
            new ColorTitle("LightBlue", System.Windows.Media.Brushes.LightBlue),
            new ColorTitle("LightCoral", System.Windows.Media.Brushes.LightCoral),
            new ColorTitle("LightCyan", System.Windows.Media.Brushes.LightCyan),
            new ColorTitle("LightGoldenrodYellow", System.Windows.Media.Brushes.LightGoldenrodYellow),
            new ColorTitle("LightGray", System.Windows.Media.Brushes.LightGray),
            new ColorTitle("LightGreen", System.Windows.Media.Brushes.LightGreen),
            new ColorTitle("LightPink", System.Windows.Media.Brushes.LightPink),
            new ColorTitle("LightSalmon", System.Windows.Media.Brushes.LightSalmon),
            new ColorTitle("LightSeaGreen", System.Windows.Media.Brushes.LightSeaGreen),
            new ColorTitle("LightSkyBlue", System.Windows.Media.Brushes.LightSkyBlue),
            new ColorTitle("LightSlateGray", System.Windows.Media.Brushes.LightSlateGray),
            new ColorTitle("LightSteelBlue", System.Windows.Media.Brushes.LightSteelBlue),
            new ColorTitle("LightYellow", System.Windows.Media.Brushes.LightYellow),
            new ColorTitle("Lime", System.Windows.Media.Brushes.Lime),
            new ColorTitle("LimeGreen", System.Windows.Media.Brushes.LimeGreen),
            new ColorTitle("Linen", System.Windows.Media.Brushes.Linen),
            new ColorTitle("Magenta", System.Windows.Media.Brushes.Magenta),
            new ColorTitle("Maroon", System.Windows.Media.Brushes.Maroon),
            new ColorTitle("MediumAquamarine", System.Windows.Media.Brushes.MediumAquamarine),
            new ColorTitle("MediumBlue", System.Windows.Media.Brushes.MediumBlue),
            new ColorTitle("MediumOrchid", System.Windows.Media.Brushes.MediumOrchid),
            new ColorTitle("MediumPurple", System.Windows.Media.Brushes.MediumPurple),
            new ColorTitle("MediumSeaGreen", System.Windows.Media.Brushes.MediumSeaGreen),
            new ColorTitle("MediumSlateBlue", System.Windows.Media.Brushes.MediumSlateBlue),
            new ColorTitle("MediumSpringGreen", System.Windows.Media.Brushes.MediumSpringGreen),
            new ColorTitle("MediumTurquoise", System.Windows.Media.Brushes.MediumTurquoise),
            new ColorTitle("MediumVioletRed", System.Windows.Media.Brushes.MediumVioletRed),
            new ColorTitle("MidnightBlue", System.Windows.Media.Brushes.MidnightBlue),
            new ColorTitle("MintCream", System.Windows.Media.Brushes.MintCream),
            new ColorTitle("MistyRose", System.Windows.Media.Brushes.MistyRose),
            new ColorTitle("Moccasin", System.Windows.Media.Brushes.Moccasin),
            new ColorTitle("NavajoWhite", System.Windows.Media.Brushes.NavajoWhite),
            new ColorTitle("Navy", System.Windows.Media.Brushes.Navy),
            new ColorTitle("OldLace", System.Windows.Media.Brushes.OldLace),
            new ColorTitle("Olive", System.Windows.Media.Brushes.Olive),
            new ColorTitle("OliveDrab", System.Windows.Media.Brushes.OliveDrab),
            new ColorTitle("OliveDrab", System.Windows.Media.Brushes.Orange),
            new ColorTitle("OrangeRed", System.Windows.Media.Brushes.OrangeRed),
            new ColorTitle("Orchid", System.Windows.Media.Brushes.Orchid),
            new ColorTitle("PaleGoldenrod", System.Windows.Media.Brushes.PaleGoldenrod),
            new ColorTitle("PaleGreen", System.Windows.Media.Brushes.PaleGreen),
            new ColorTitle("PaleTurquoise", System.Windows.Media.Brushes.PaleTurquoise),
            new ColorTitle("PaleVioletRed", System.Windows.Media.Brushes.PaleVioletRed),
            new ColorTitle("PapayaWhip", System.Windows.Media.Brushes.PapayaWhip),
            new ColorTitle("PeachPuff", System.Windows.Media.Brushes.PeachPuff),
            new ColorTitle("Peru", System.Windows.Media.Brushes.Peru),
            new ColorTitle("Pink", System.Windows.Media.Brushes.Pink),
            new ColorTitle("Plum", System.Windows.Media.Brushes.Plum),
            new ColorTitle("PowderBlue", System.Windows.Media.Brushes.PowderBlue),
            new ColorTitle("Purple", System.Windows.Media.Brushes.Purple),
            new ColorTitle("Red", System.Windows.Media.Brushes.Red),
            new ColorTitle("RosyBrown", System.Windows.Media.Brushes.RosyBrown),
            new ColorTitle("RoyalBlue", System.Windows.Media.Brushes.RoyalBlue),
            new ColorTitle("SaddleBrown", System.Windows.Media.Brushes.SaddleBrown),
            new ColorTitle("Salmon", System.Windows.Media.Brushes.Salmon),
            new ColorTitle("SandyBrown", System.Windows.Media.Brushes.SandyBrown),
            new ColorTitle("SeaGreen", System.Windows.Media.Brushes.SeaGreen),
            new ColorTitle("SeaShell", System.Windows.Media.Brushes.SeaShell),
            new ColorTitle("Sienna", System.Windows.Media.Brushes.Sienna),
            new ColorTitle("Silver", System.Windows.Media.Brushes.Silver),
            new ColorTitle("SkyBlue", System.Windows.Media.Brushes.SkyBlue),
            new ColorTitle("SlateBlue", System.Windows.Media.Brushes.SlateBlue),
            new ColorTitle("SlateGray", System.Windows.Media.Brushes.SlateGray),
            new ColorTitle("Snow", System.Windows.Media.Brushes.Snow),
            new ColorTitle("SpringGreen", System.Windows.Media.Brushes.SpringGreen),
            new ColorTitle("SteelBlue", System.Windows.Media.Brushes.SteelBlue),
            new ColorTitle("Tan", System.Windows.Media.Brushes.Tan),
            new ColorTitle("Teal", System.Windows.Media.Brushes.Teal),
            new ColorTitle("Thistle", System.Windows.Media.Brushes.Thistle),
            new ColorTitle("Tomato", System.Windows.Media.Brushes.Tomato),
            new ColorTitle("Transparent", System.Windows.Media.Brushes.Transparent),
            new ColorTitle("Turquoise", System.Windows.Media.Brushes.Turquoise),
            new ColorTitle("Violet", System.Windows.Media.Brushes.Violet),
            new ColorTitle("Wheat", System.Windows.Media.Brushes.Wheat),
            new ColorTitle("White", System.Windows.Media.Brushes.White),
            new ColorTitle("WhiteSmoke", System.Windows.Media.Brushes.WhiteSmoke),
            new ColorTitle("Yellow", System.Windows.Media.Brushes.Yellow),
            new ColorTitle("YellowGreen", System.Windows.Media.Brushes.YellowGreen),
        };


        public int VK_BACK    = 0x08;
        public int VK_TAB     = 0x09;
        public int VK_RETURN  = 0x0D;
        public int VK_SHIFT   = 0x10;
        public int VK_CONTROL = 0x11;
        public int VK_ESCAPE  = 0x1B;

        System.Diagnostics.Stopwatch mSw;       //  ストップウォッチクラス
        private TimeSpan mStopWatchTotalTime;   //  mSwの経過時間
        public ColorTitle[] mBrushList;
        private char[] mBrackets = { '(', ')', '{', '}', '[', ']', '\"', '\"' };    //  括弧リスト
        private double[] mKaigen = {    //  改元の日付(yyyy.mmdd)
            2019.0501, 1989.0108, 1926.1225, 1912.0730, 1868.0908, 1865.0407, 1864,   1861,   1860,   1854,   1848,   1844,   1830,   1818
        };
        private string[] mGengou = {    //  元号名
            "令和",    "平成",     "昭和",   "大正",    "明治",    "慶応",    "元治", "文久", "万延", "安政", "嘉永", "弘化", "天保", "文政"
        };
        private string[] mMonth = {
            "january","february","march","april","may","june","july","august","september","october","novmber","december",
            "jan","feb","mar","apr","may","jun","jul","aug","sep","oct","nov","dec",
        };

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
            mBrushList = getColorList();
        }

        public void test()
        {

        }

        //  ---  API関数  ------

        [StructLayout(LayoutKind.Sequential)]
        public struct iRect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        //  ウインドウの外側のサイズを取得
        //  hWnd ; ウィンドウ・ハンドル
        //  rect : Rect構造体
        [DllImport("user32.Dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out iRect rect);

        //  フォアグラウンドウィンドウ(ActiveWindow)の取得
        //  Return : ウィンドウ・ハンドル
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        //  クリックされているか判定用
        //  nVirtkey : 状態を知りたいキーコード
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtkey);

        //クリック判定
        //  マウス左ボタン(0x01)(VK_LBUTTON)の状態
        //  押されていたらマイナス値(-127)、なかったら0
        public bool IsClickDownLeft()
        {
            return GetKeyState(0x01) < 0;
        }

        //  マウス右ボタン(0x02)(VK_RBUTTON)の状態
        //  押されていたらマイナス値(-127)、なかったら
        public bool IsClickDownRight()
        {
            return GetKeyState(0x02) < 0;
        }

        //  マウスやキーボードの状態取得
        //  nVirtkey : 状態を知りたいキーコード
        //  仮想キー コード VK_LBUTTON,VK_RBUTTON,VK_CANCEL,VK_BACK,VK_TAB,VK_RETURN,VK_SHIFT,VK_CONTROL,VK_MENU(ALT)...
        //  https://learn.microsoft.com/ja-jp/windows/win32/inputdev/virtual-key-codes
        public bool isGetKeyState(int nVirtKey)
        {
            return GetKeyState(nVirtKey) < 0;
        }

        /// <summary>
        /// コントロールキーの確認
        /// </summary>
        /// <returns>Ctrlキーの有無</returns>
        public bool onControlKey()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        /// <summary>
        /// Altキーの確認
        /// </summary>
        /// <returns>Altキーの有無</returns>
        public bool onAltKey()
        {
            return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        }

        /// <summary>
        /// キーの確認
        /// </summary>
        /// <param name="keyCode">キーのコード</param>
        /// <returns>指定キーの有無</returns>
        public bool onKey(int keyCode)
        {
            return isGetKeyState(keyCode);
        }


        //  ---  システム関連  ------

        /// <summary>
        /// フォント名リストの取得(en-us,ja-jp)
        /// </summary>
        /// <param name="languageSelect">日本語フォントのみ</param>
        /// <returns></returns>
        public List<string> getSystemFontFamilyName(bool languageSelect = false)
        {
            var language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);   //  カレントの言語
            var lang_en = XmlLanguage.GetLanguage("en-us");                                     //  英語
            var lang_jp = XmlLanguage.GetLanguage("ja-jp");                                     //  日本語
            List<string> fontFamiles = new List<string>();
            //  すべてのフォントからフォント名を抽出
            foreach (System.Windows.Media.FontFamily fontFamily in Fonts.SystemFontFamilies) {
                if (fontFamily.FamilyNames.FirstOrDefault(j => j.Key == lang_jp).Value != null ||
                    !languageSelect)
                    fontFamiles.Add(fontFamily.Source);
            }
            fontFamiles.Sort();
            return fontFamiles;
        }

        /// <summary>
        /// 文字列をFontStyleに変換して変換
        /// Normal/Italic/Oblique
        /// </summary>
        /// <param name="style">FontStyle文字列</param>
        public System.Windows.FontStyle convFontStyle(string style)
        {
            return style == "Italic" ? FontStyles.Italic :
                style == "Oblique" ? FontStyles.Oblique : FontStyles.Normal;
        }

        /// <summary>
        /// 文字列をFontWeightに変換して変換
        /// Normal/Thin/Bold
        /// </summary>
        /// <param name="weight">FontWeight文字列</param>
        public System.Windows.FontWeight convFontWeight(string weight)
        {
            return weight == "Bold" ? FontWeights.Bold :
                 weight == "Thin" ? FontWeights.Thin : FontWeights.Normal;
        }

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
        /// フルパスのディレクトリから最後のフォルダ名を取り出す
        /// </summary>
        /// <param name="folder">ディレクトリ</param>
        /// <returns>抽出フォルダ名</returns>
        public string getLastFolder(string folder)
        {
            return getLastFolder(folder, 1);
        }

        /// <summary>
        /// フルパスのディレクトリから最後のフォルダ名を取り出す
        /// </summary>
        /// <param name="folder">ディレクトリ</param>
        /// <param name="n">後ろから位置 n番目のフォルダ</param>
        /// <returns>抽出フォルダ名</returns>
        public string getLastFolder(string folder, int n)
        {
            string[] buf = folder.Split('\\');
            if (0 < buf.Length)
                return buf[buf.Length - n];
            else
                return "";
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
        /// この場合は CoreLib となるので Mainのアプリケーション名は直接コードを呼ぶ
        /// </summary>
        /// <returns></returns>
        public string getAppName(bool fullpath = true)
        {
            if (fullpath)
                return System.Reflection.Assembly.GetExecutingAssembly().Location;
            else
                return Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
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
        /// プログラムの実行
        /// ylib.processStart(mDiffTool, $"\"{srcPath}\" \"{destPath}\"");
        /// </summary>
        /// <param name="prog">プログラムのパス</param>
        /// <param name="arg">プログラムの引数</param>
        /// <returns>実行の可否</returns>
        public bool processStart(string prog, string arg)
        {
            try {
                //if (File.Exists(prog))
                    Process.Start(prog, arg);
                //else
                //    return false;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                return false;
            }
            return true;
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
                    Process.Start(new ProcessStartInfo("cmd", $"/c start　\"\" \"{url}\"") { CreateNoWindow = true });
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

        /// <summary>
        /// メッセージダイヤログ(親Windowの中央に表示)
        /// ボタンの種類 MessageBoxButton.OK/OKCancel/YesNo/YesNoCancel
        /// </summary>
        /// <param name="owner">オーナー</param>
        /// <param name="message">メッセージ</param>
        /// <param name="title">ダイヤログのタイトル</param>
        /// <param name="dlgTitle">メッセージ内タイトル</param>
        /// <param name="buttonType">ボタンの種類</param>
        /// <returns>MessageBoxResult</returns>
        public MessageBoxResult messageBox(Window owner, string message, string title = "",
            string dlgTitle = "",MessageBoxButton buttonType = MessageBoxButton.OK)
        {
            MessageBoxEx dlg = new MessageBoxEx();
            dlg.Owner = owner;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.mMessage = message;
            dlg.mDlgTitle = dlgTitle;
            dlg.mTitle = title;
            dlg.mButton = buttonType;
            dlg.ShowDialog();
            return dlg.mResult;
        }

        /// <summary>
        /// メッセージ表示ダイヤログ
        /// </summary>
        /// <param name="owner">オーナー</param>
        /// <param name="message">メッセージ</param>
        /// <param name="title">ダイヤログのタイトル</param>
        /// <param name="dlgTitle">メッセージ内タイトル</param>
        /// <param name="buttonType">ボタンの種類</param>
        /// <param name="continueMessage">(MessageBoxResult, MessageContinue)</param>
        /// <returns></returns>
        public (MessageBoxResult result, bool cont) messageBoxEx(Window owner, string message, string title = "",
            string dlgTitle = "", MessageBoxButton buttonType = MessageBoxButton.OK, bool continueMessage = false)
        {
            MessageBoxEx dlg = new MessageBoxEx();
            dlg.Owner = owner;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.mMessage = message;
            dlg.mDlgTitle = dlgTitle;
            dlg.mTitle = title;
            dlg.mButton = buttonType;
            dlg.mContinueMessageDisp = continueMessage;
            dlg.ShowDialog();
            return (dlg.mResult, dlg.mContinueMessage);
        }

        /// <summary>
        /// クリップボードにファイルリストを7設定(FileDrop + Preferred DropEffect + Text)
        /// </summary>
        /// <param name="fileList">ファイルリスト</param>
        /// <param name="effect">コピータイプ(0:Non 1:Copy 2:Mpve 4:Link)</param>
        public void setClipboard(List<string> fileList, int effect)
        {
            //  [FileDrop]
            //  FileName/FilNameWも合わせて作られる
            string[] files = fileList.ToArray();
            DataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.FileDrop, files);

            //  [Preferred DropEffect]
            // Set Preferred DropEffect to COPY
            // This is a DWORD (4 bytes) in little-endian format
            MemoryStream dropEffectStream = new MemoryStream();
            byte[] dropEffectBytes = BitConverter.GetBytes((uint)effect);
            dropEffectStream.Write(dropEffectBytes, 0, dropEffectBytes.Length);
            dataObject.SetData("Preferred DropEffect", dropEffectStream);

            //  [Text]
            dataObject.SetData(DataFormats.Text, string.Join('\n', fileList));

            Clipboard.SetDataObject(dataObject, true);
        }

        /// <summary>
        /// クリップボードからファイルリスト(FileDrop+ Preferred DropEffect)を取り出す
        /// </summary>
        /// <returns>(ファイルリスト,コピータイプ　0:NONE 1:COPY 2:MOVE 4:LINK)</returns>
        public (List<string> files, int copyType) getClipboardFiles()
        {
            List<string> files = new List<string>();
            uint effect = 0;
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.FileDrop)) {
                string[] fileArray = (string[])iData.GetData(DataFormats.FileDrop);
                files = fileArray.ToList();
            }
            if (iData.GetDataPresent("Preferred DropEffect")) {
                MemoryStream ms = iData.GetData("Preferred DropEffect") as MemoryStream;
                if (ms != null && ms.Length >= 4) {
                    byte[] bytes = new byte[4];
                    ms.Read(bytes, 0, 4);
                    effect = BitConverter.ToUInt32(bytes, 0);  //  0:NONE 1:COPY 2:MOVE 4:LINK
                }
            }
            return (files, (int)effect);
        }

        // ---  ストップウォッチ  ---

        /// <summary>
        /// ストップウォッチ機能初期化・計測開始
        /// </summary>
        public void stopWatchStartNew()
        {
            mSw = System.Diagnostics.Stopwatch.StartNew();
            mStopWatchTotalTime = new TimeSpan();
        }

        /// <summary>
        /// ストップウォッチ機能ラップ時間取得
        /// </summary>
        /// <returns>計測時間</returns>
        public TimeSpan stopWatchLapTime()
        {
            mSw.Stop();
            TimeSpan lap = mSw.Elapsed;
            mSw.Start();
            return lap;
        }

        /// <summary>
        /// ストップウォッチ機能計測時間の取得と再スタート
        /// 累積時間には追加
        /// </summary>
        /// <returns>計測時間</returns>
        public TimeSpan stopWatchRestart()
        {
            mSw.Stop();
            TimeSpan lap = mSw.Elapsed;
            mStopWatchTotalTime += lap;
            mSw.Restart();
            return lap;
        }

        /// <summary>
        /// ストップウォッチ機能計測時間の取得と終了
        /// </summary>
        /// <returns>計測時間</returns>
        public TimeSpan stopWatchStop()
        {
            mSw.Stop();
            TimeSpan lap = mSw.Elapsed;
            mStopWatchTotalTime += lap;
            return lap;
        }

        /// <summary>
        /// ストップウォッチ機能累積時間の取得
        /// </summary>
        /// <returns>計測時間</returns>
        public TimeSpan stopWatchTotalTime()
        {
            return mStopWatchTotalTime;
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
        /// 括弧で囲まれた文字列を抽出する(ディフォルトは'{','}')
        /// 抽出した文字列に括弧は含まれない
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="sb">開始括弧</param>
        /// <param name="eb">終了括弧</param>
        /// <returns>括弧内文字列</returns>
        public List<string> extractBrackets(string text, char sb = '{', char eb = '}', bool withBracket = false)
        {
            List<string> extractText = new List<string>();
            int bOffset = withBracket ? 1 : 0;
            int pos = 0;
            int sp = text.IndexOf(sb);
            int ep = text.IndexOf(eb, sp + 1);
            if ((0 <= sp && 0 <= ep && ep < sp) || (sp < 0 && 0 <= ep)) {
                string data = text.Substring(0, ep + bOffset);
                if (0 < data.Length)
                    extractText.Add(data);
                pos = ep + 1;
            }
            while (pos < text.Length) {
                int st = text.IndexOf(sb, pos);
                string data = "";
                if (pos <= st) {
                    int ct = text.IndexOf(eb, st + 1);
                    if (0 <= ct) {
                        data = text.Substring(st + 1 - bOffset, ct - st - 1 + 2 * bOffset);
                        pos = ct + 1;
                    } else {
                        data = text.Substring(st + 1 - bOffset);
                        pos = text.Length;
                    }
                } else {
                    pos = text.Length;
                }
                // string data = data.Trim(trimChar);
                if (0 < data.Length)
                    extractText.Add(data);
            }
            return extractText;
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
        /// 文字列からコントロールコードを除外する
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>除外した文字列</returns>
        public string stripControlCode(string str)
        {
            string buf = "";
            int pos = 0;
            while (pos < str.Length) {
                if (0x20 <= str[pos])
                    buf += str[pos];
                pos++;
            }

            return buf;
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
        /// 文字列を指定文字列で分ける
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="sep">分割文字列配列</param>
        /// <returns>分割リスト</returns>
        public List<string> splitString(string str, string[] sep)
        {
            List<string> strList = new List<string>();
            int i = 0;
            string buf = "";
            while (i < str.Length) {
                bool find = false;
                for (int j = 0; j < sep.Length; j++) {
                    int len = sep[j].Length;
                    if (len <= str.Length - i) {
                        if (sep[j].CompareTo(str.Substring(i, len)) == 0) {
                            if (0 < buf.Length)
                                strList.Add(buf);
                            strList.Add(str.Substring(i, len));
                            i += len;
                            buf = "";
                            find = true;
                            break;
                        }
                    }
                }
                if (find)
                    continue;
                buf += str[i++];
            }
            if (0 < buf.Length)
                strList.Add(buf);
            return strList;
        }


        /// <summary>
        /// 括弧付き文字列で前後の括弧を除いた文字列
        /// </summary>
        /// <param name="str">括弧付き文字列</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>文字列</returns>
        public string stripBracketString(string str, char bracket = '(')
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            str = str.Trim();
            int sp = str.IndexOf(mBrackets[offset]);
            int ep = str.LastIndexOf(mBrackets[offset + 1]);
            if (sp != 0 || ep < 0 || sp >= ep)
                return str;
            return str.Substring(sp + 1, ep - sp - 1);
        }

        /// <summary>
        /// 括弧で囲まれた文字列の抽出(括弧含む)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="n">開始位置</param>
        /// <param name="bracket">括弧の種類</param>
        /// <returns>抽出文字列</returns>
        public string getBracketString(string str, int n, char bracket = '(', bool withBracket = true)
        {
            int offset = Array.IndexOf(mBrackets, bracket);
            if (offset < 0)
                return str;
            int sp = str.IndexOf(mBrackets[offset], n) + (withBracket ? 0 : 1);
            int pos = sp;
            if (pos < 0) return str;
            int count = 1;
            pos++;
            while (pos < str.Length && 0 < count) {
                if (bracket != '"' && str[pos] == '"') {
                    while (pos + 1 < str.Length && str[++pos] != '"') ;
                }
                if (str[pos] == '\\') pos++;
                else if (str[pos] == mBrackets[offset + 1]) count--;
                else if (str[pos] == mBrackets[offset]) count++;
                pos++;
            }
            pos -= withBracket ? 0 : 1;
            return str.Substring(sp, pos - sp);
        }

        /// <summary>
        /// 一行が指定文字数を超えたら改行を入れる
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="word">改行対象文字列</param>
        /// <param name="lineSize">一行の文字数</param>
        /// <returns></returns>
        public string insertLinefeed(string str, string word = ",", int lineSize = 80)
        {
            string buf = "";
            int sp = 0, np = 0;
            int n = str.IndexOf(word, sp);
            int nn = str.IndexOf("\n", sp);
            while (sp < str.Length && 0 <= n) {
                n += word.Length;
                if (0 <= nn && nn < n) {
                    buf += str.Substring(np, nn - np);
                    n = nn;
                    np = n;
                } else if (lineSize < n - np) {
                    buf += str.Substring(np, n - np) + '\n';
                    np = n;
                }
                sp = n + 1;
                n = str.IndexOf(word, sp);
                nn = str.IndexOf("\n", sp);
            }
            buf += str.Substring(np);
            return buf;
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

        /// <summary>
        /// 文字列の長さをbyte数で求める
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>byte数</returns>
        public int getStrByteCount(string str)
        {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            return sjisEnc.GetByteCount(str);
        }

        /// <summary>
        /// Unicode文字コードの文字列を文字列に変換
        /// "\\U+30A2\\U+30CE\\U+30FC\\U+30C9" → "アノード"
        /// </summary>
        /// <param name="text">Unicode文字列</param>
        /// <returns>文字列</returns>
        public string stringUncode2String(string text)
        {
            string buf = "";
            for (int i = 0; i < text.Length; i++) {
                if (i < text.Length - 6 && text.Substring(i,3) == "\\U+") {
                    string str = text.Substring(i + 3, 4);
                    int charCode = Convert.ToInt32(str, 16);
                    char c = Convert.ToChar(charCode);
                    buf += c.ToString();
                    i += 6;
                } else
                    buf += text[i].ToString();
            }
            return buf;
        }

        /// <summary>
        /// タブをスペースに変換
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="tabsize">タブサイズ</param>
        /// <returns>変換文字列</returns>
        public string tab2space(string text, int tabsize = 4)
        {
            string buf = "";
            string cbuf = "";
            foreach (var c in text) {
                if (c == '\t') {
                    int pos = getStrByteCount(cbuf);
                    int tab = tabsize - pos % tabsize;
                    buf += new string(' ', tab);
                } else {
                    if (c == '\n' || c == '\r')
                        cbuf = "";
                    cbuf += c.ToString();
                    buf += c.ToString();
                }
            }
            return buf;
        }

        /// <summary>
        /// 文字列のハッシュ値(MD5)を求める
        /// 比較は SrcHash.SequenceEqual(DestHash)
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>ハッシュ値</returns>
        public byte[] getHashMD5(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            MD5 md5 = MD5.Create();
            byte [] hashCode = md5.ComputeHash(data);
            md5.Clear();
            return hashCode;
        }


        //  ---  数値変換  -----------------

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
        /// 数値をゼロサプレスして文字列に変換
        /// </summary>
        /// <param name="val">数値</param>
        /// <param name="form">書式</param>
        /// <returns>数値文字列</returns>
        public string double2StrZeroSup(double val, string form = "f1")
        {
            string valStr = val.ToString(form);
            if (0 <= valStr.IndexOf('.')) {
                for (int i = valStr.Length - 1; 0 < i; i--) {
                    if (valStr[i] != '0') {
                        if (valStr[i] == '.')
                            return valStr.Substring(0, i);
                        else
                            return valStr.Substring(0, i + 1);
                    }
                }
            }
            return valStr;
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
        /// 文字列を整数に変換(0xがつくと16進として変換)
        /// </summary>
        /// <param name="str">数値文字列</param>
        /// <param name="val">default値(省略時は0)</param>
        /// <returns></returns>
        public long longParse(string str, int val = 0)
        {
            long i;
            str = str.Replace(",", "");
            if (str.StartsWith("0x"))
                return Convert.ToInt64(str, 16);
            else if (long.TryParse(str.Trim(), out i))
                return i;
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
            int n = nbuf.IndexOf('.');
            if (0 <= n)
                nbuf = nbuf.Substring(0, n);
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

        //  ---  16進文字列処理  -----------

        /// <summary>
        /// 16進文字列をintに変換
        /// </summary>
        /// <param name="str">16進文字列</param>
        /// <param name="val">default値(省略時は0)</param>
        /// <returns></returns>
        public int intHexParse(string str, int val = 0)
        {
            str = str.Replace(" ", "");
            if (int.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out int number)) {
                return number;
            } else {
                return val;
            }
        }

        /// <summary>
        /// 16進文字列をlongに変換
        /// </summary>
        /// <param name="str">16進文字列</param>
        /// <param name="val">default値(省略時は0)</param>
        /// <returns></returns>
        public long longHexParse(string str, int val = 0)
        {
            str = str.Replace(" ", "");
            if (long.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out long number)) {
                return number;
            } else {
                return val;
            }
        }

        /// <summary>
        /// 文字列が16進数かの判定
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>16進数?</returns>
        public bool isHexNumber(string str)
        {
            foreach (char c in str) {
                if (!isHexNumber(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 文字が16進数かの判定
        /// </summary>
        /// <param name="c">文字</param>
        /// <returns>16進数?</returns>
        public bool isHexNumber(char c)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
                return false;
            return true;
        }

        /// <summary>
        /// 文字列が10進数かの判定(実数含む)
        /// </summary>
        /// <param name="str">文字列</param>
        /// <returns>10進数?</returns>
        public bool isDecNumber(string str)
        {
            for (int i = 0; i < str.Length; i++) {
                if (!(Char.IsNumber(str[i]) || str[i] == '.' ||
                    (i == 0 && str[i] == '-') ||
                    (0 < i && (str[i] == 'E' || str[i] == 'e') && Char.IsNumber(str[i - 1])) ||
                    (0 < i && (str[i - 1] == 'E' || str[i - 1] == 'e') && (str[i] == '-' || str[i] == '+'))))
                    return false;
            }
            return true;
        }


        //  --- データ処理関係  ------

        /// <summary>
        /// 配列データをCSVデータに変換
        /// </summary>
        /// <param name="data">配列データ</param>
        /// <returns>文字列</returns>
        public string arrayStr2CsvData(string[] data)
        {
            string buf = "";
            for (int i = 0; i < data.Length; i++) {
                data[i] = data[i] == null ? "" : data[i].Replace("\"", "\\\"");
                buf += "\"" + data[i] + "\"";
                if (i != data.Length - 1)
                    buf += ",";
            }
            return buf;
        }

        /// <summary>
        /// CSVデータを配列データに戻す
        /// </summary>
        /// <param name="line">文字列</param>
        /// <returns>配列データ</returns>
        public string[] csvData2ArrayStr(string line)
        {
            string[] buf = seperateString(line);
            return buf;
        }

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
        /// 日時を分単位で丸める
        /// </summary>
        /// <param name="et">日時</param>
        /// <param name="min">丸める分数</param>
        /// <returns>丸めた日時</returns>
        public DateTime roundDateTimeMin(DateTime et, int min)
        {
            DateTime st = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan dt = et - st;
            DateTime rt = st.Add(new TimeSpan(0, (int)(Math.Floor(dt.TotalMinutes / min) * min), 0));
            return rt;
        }

        /// <summary>
        /// 秒数を時分秒の文字列に変換 s → mm:ss or hh:mm:ss
        /// </summary>
        /// <param name="seconds">時間(秒)</param>
        /// <param name="auto">自動で選択(mm:ss or hh:mm:ss)</param>
        /// <returns>時刻文字列</returns>
        public string second2String(double seconds, bool auto)
        {
            long second = (long)seconds % 60;
            long minute = (long)(seconds / 60) % 60;
            long hour = (long)(seconds / 3600);
            if (0 < hour || !auto)
                return string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
            else
                return string.Format("{0:00}:{1:00}", minute, second);
        }

        /// <summary>
        /// 文字列の時刻タイプの判定
        /// hh:mm:ss,hh:mm,hh:mm AM/PM, h時m分s秒, h時m分
        /// </summary>
        /// <param name="time">文字列</param>
        /// <returns></returns>
        public bool IsTime(string time)
        {
            if (Regex.IsMatch(time, "(^[01][0-9]|^2[0123]):([0-5][0-9]):([0-5][0-9])") ||     //  24時:分:秒
                Regex.IsMatch(time, "(^[1-9]|^1[012]):([0-5][0-9]):([0-5][0-9])") ||          //  12時:分:秒
                Regex.IsMatch(time, "(^[01][0-9]|^2[0123]):([0-5][0-9])") ||                  //  時:分
                Regex.IsMatch(time, "(^[1-9]|^1[012]):([0-5][0-9]) (AM|PM)") ||               //  時:分 PM
                Regex.IsMatch(time, "(^[0-9]|^[01][0-9]|^2[0123])時([0-9]|[0-5][0-9])分([0-9]|[0-5][0-9])秒") ||
                Regex.IsMatch(time, "(^[0-9]|^[01][0-9]|^2[0123])時([0-9]|[0-5][0-9])分"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 文字列時刻を秒数に変換
        /// hh:mm:ss,hh:mm,hh:mm AM/PM,hh時mm分ss秒,hh時mm分
        /// </summary>
        /// <param name="time">時刻文字列</param>
        /// <returns>秒数</returns>

        public int time2Seconds(string time)
        {
            int h = 0;
            int m = 0;
            int s = 0;
            if (Regex.IsMatch(time, "([01][0-9]|2[0123]):([0-5][0-9]):([0-5][0-9])")) {     //  24時:分:秒
                h = int.Parse(time.Substring(0, 2));
                m = int.Parse(time.Substring(3, 2));
                s = int.Parse(time.Substring(6, 2));
            } else if (Regex.IsMatch(time, "([1-9]|1[012]):([0-5][0-9]):([0-5][0-9])")) {   //  12時:分:秒
                h = int.Parse(time.Substring(0, time.IndexOf(':')));
                m = int.Parse(time.Substring(time.IndexOf(':') + 1, time.IndexOf(':', time.IndexOf(':') + 1) - time.IndexOf(':') - 1));
                s = int.Parse(time.Substring(time.IndexOf(':', time.IndexOf(':') + 1) + 1));
            } else if (Regex.IsMatch(time, "([01][0-9]|2[0123]):([0-5][0-9])")) {           //  時:分
                h = int.Parse(time.Substring(0, time.IndexOf(':')));
                m = int.Parse(time.Substring(time.IndexOf(':') + 1));
            } else if (Regex.IsMatch(time, "([1-9]|1[012]):([0-5][0-9]) (AM|PM)")) {        //  時:分 PM
                h = int.Parse(time.Substring(0, time.IndexOf(':')));
                m = int.Parse(time.Substring(time.IndexOf(':') + 1, time.IndexOf(' ') - time.IndexOf(':')));
                if (0 < time.IndexOf("PM"))
                    h += 12;
            } else if (Regex.IsMatch(time, "([0-9]|[01][0-9]|2[0123])時([0-9]|[0-5][0-9])分([0-9]|[0-5][0-9])秒")) {
                h = int.Parse(time.Substring(0, time.IndexOf('時')));
                int hp = time.IndexOf('時') + 1;
                int mp = time.IndexOf('分') - time.IndexOf('時') - 1;
                m = int.Parse(time.Substring(time.IndexOf('時') + 1, time.IndexOf('分') - time.IndexOf('時') - 1));
                s = int.Parse(time.Substring(time.IndexOf('分') + 1, time.IndexOf('秒') - time.IndexOf('分') - 1));
            } else if (Regex.IsMatch(time, "([0-9]|[01][0-9]|2[0123])時([0-9]|[0-5][0-9])分")) {
                h = int.Parse(time.Substring(0, time.IndexOf('時')));
                m = int.Parse(time.Substring(time.IndexOf('時') + 1, time.IndexOf('分') - time.IndexOf('時') - 1));
            } else {
                return 0;
            }
            return ((h * 60) + m) * 60 + s;
        }

        /// <summary>
        /// 文字列が曜日かの判定
        /// </summary>
        /// <param name="weekday">曜日</param>
        /// <returns></returns>
        public bool IsWeekday(string weekday)
        {
            if (Regex.IsMatch(weekday, "^[日月火水木金土]|^[日月火水木金土]曜|^[日月火水木金土]曜日") ||
                Regex.IsMatch(weekday, "(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)") ||
                Regex.IsMatch(weekday, "(Sun|Mon|Tue|Wed|Thu|Fri|Sat)"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 文字列から曜日の番号を返す
        /// 日曜日を0とする
        /// </summary>
        /// <param name="weekday">曜日</param>
        /// <returns>曜日の位置</returns>
        public int WeekNo(string weekday)
        {
            if (weekday.Length < 1)
                return -1;
            int weekNo = "日月火水木金土".IndexOf(weekday.Substring(0, 1));
            if (weekNo < 0 && 2 < weekday.Length) {
                weekNo = "SunMonTueWedThuFriSat".IndexOf(weekday.Substring(0, 3));
                weekNo = weekNo < 0 ? weekNo : weekNo / 3;
            }
            return weekNo;
        }

        /// <summary>
        /// 曜日の位置から曜日文字列を返す
        /// </summary>
        /// <param name="weekNo">曜日の位置</param>
        /// <returns>曜日文字列</returns>
        public string getWeekday(int weekNo, int type)
        {
            string[,] weekday = {
                { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" },
                { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" },
                { "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日" },
                { "日", "月", "火", "水", "木", "金", "土" },
            };
            return weekday[type % weekday.GetLength(0), weekNo % weekday.GetLength(1)];
        }

        public string[] mDatePattern = {
            //  [month] dd, yyyy
            "(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec).*?([12][0-9]|3[01]|[1-9]),.*?(1[89][0-9][0-9]|2[01][0-9][0-9])",
            //  [元号]yy年mm月dd日
            "(令和|平成|昭和|大正|明治|慶応|元治|文久|万延|安政|嘉永|弘化|天保|文政)([1-9]|[0-9][0-9])年([1-9]|[01][0-2])月([0-2][0-9]|3[01]|[1-9])日",
            //  yyyy年ww周
            "(^1[89][0-9][0-9]|2[01][0-9][0-9])年([1-5][0-9]|[1-9])週",
            //  yyyy年mm月w週
            "(^1[89][0-9][0-9]|2[01][0-9][0-9])年([1-9]|1[0-2])月([1-5])週",
            //  yyyy年mm月dd日
            "(1[89][0-9][0-9]|2[01][0-9][0-9])年([1-9]|1[0-2])月([12][0-9]|3[01]|[1-9])日",
            //  yyyy年mm月
            "(1[89][0-9][0-9]|2[01][0-9][0-9])年(1[0-2]|[1-9])月",
            //  yyyy年
            "(1[89][0-9][0-9]|2[01][0-9][0-9])年",
            //  mm月dd日
            "([1-9]|1[0-2])月([1-9]|[12][0-9]|3[01])日",
            //  yyyy/mm/dd
            "(19[0-9][0-9]|2[01][0-9][0-9])/([1-9]|1[0-2]|0[1-9])/([12][0-9]|3[01]|0[1-9]|[1-9])",
            //  yyyy/mm
            "(1[89][0-9][0-9]|2[01][0-9][0-9])/([1-9]|1[0-2]|0[1-9])",
            //  yyyy-mm-dd
            "(1[89][0-9][0-9]|2[01][0-9][0-9])-([1-9]|1[0-2]|0[1-9])-([12][0-9]|3[01]|0[1-9]|[1-9])",
            //  dd/mm/yyyy
            "(1[0-2]|0[1-9]|[1-9])/([1-9]|[12][0-9]|3[01]|0[1-9])/(19[0-9][0-9]|2[01][0-9][0-9])",
            //  yyyymmdd
            "(1[89][0-9][0-9]|2[01][0-9][0-9])(1[0-2]|0[1-9])([12][0-9]|3[01]|0[1-9])",
            //  yyyy
            "(1[89][0-9][0-9]|2[01][0-9][0-9])",
            //  dd[month]yyyy
            "([0-9][0-9])(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)(1[89][0-9][0-9]|2[01][0-9][0-9])",
            //  dd-[month]-yy
            "([0-9][0-9]|[0-9])-(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)-([0-9][0-9])",
        };

        /// <summary>
        /// テキストから日付の部分を抽出する
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>日付</returns>
        public (int, string) getDateMatch(string text)
        {
            for (int i = 0; i < mDatePattern.Length; i++) {
                Match match = Regex.Match(text, mDatePattern[i]);
                if (0 < match.Value.Length) {
                    return (i, match.Value);
                }
            }
            return (-1, "");
        }

        /// <summary>
        /// 日付文字列から曜日を求める
        /// 曜日のタイプ  0:Sunday 1:SUN 2:日曜日 3:日
        /// </summary>
        /// <param name="type">曜日のタイプ</param>
        /// <param name="date">日付文字列</param>
        /// <returns>曜日</returns>
        public string cnvDateWeekday(int type, string date = "")
        {
            DateTime dt;
            if (date.Length == 0)
                dt = DateTime.Now;
            else
                dt = DateTime.Parse(date);
            int weekNo = int.Parse(dt.DayOfWeek.ToString("d"));
            return getWeekday(weekNo, type);
        }

        /// <summary>
        /// 各種の日付パターンをyyyy/mm/ddに変換する
        /// </summary>
        /// <param name="date">日付文字列</param>
        /// <returns></returns>
        public string cnvDateFormat(string date)
        {
            int year = 0;
            int month = 0;
            int day = 0;
            int week = 0;
            int jd = 0;
            for (int i = 0; i < mDatePattern.Length; i++) {
                if (Regex.IsMatch(date.ToLower(), mDatePattern[i])) {
                    List<string[]> dateList = getPattern(date.ToLower(), mDatePattern[i]);
                    switch (i) {
                        case 0: //  [manth] dd, yyyy
                            year = intParse(dateList[0][3]);
                            month = Array.IndexOf(mMonth, dateList[0][1]) % 12 + 1;
                            day = intParse(dateList[0][2]);
                            break;
                        case 1: //  [元号]yy年mm月dd日
                            year = intParse(dateList[0][2]) + (int)mKaigen[Array.IndexOf(mGengou, dateList[0][1])] - 1;
                            month = intParse(dateList[0][3]);
                            day = intParse(dateList[0][4]);
                            break;
                        case 2: //  yyyy年ww周
                            year = intParse(dateList[0][1]);
                            week = intParse(dateList[0][2]);
                            jd = (week - 1) * 7 + (date2JulianDay(year, 1, 1) / 7) * 7 - 1;
                            month = JulianDay2Month(jd);
                            day = JulianDay2Day(jd);
                            break;
                        case 3: //  yyyy年mm月w週
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            week = intParse(dateList[0][3]);
                            jd = (week - 1) * 7 + (date2JulianDay(year, month, 1) / 7) * 7 - 1;
                            day = JulianDay2Day(jd);
                            break;
                        case 4: //  yyyy年mm月dd日
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = intParse(dateList[0][3]);
                            break;
                        case 5: //  yyyy年mm月
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = 1;
                            break;
                        case 6: //  yyyy年
                            year = intParse(dateList[0][1]);
                            month = 1;
                            day = 1;
                            break;
                        case 7: //  mm月dd日
                            year = DateTime.Now.Year;
                            month = intParse(dateList[0][1]);
                            day = intParse(dateList[0][2]);
                            break;
                        case 8: //   yyyy/mm/dd
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = intParse(dateList[0][3]);
                            break;
                        case 9: //  yyyy/mm
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = 1;
                            break;
                        case 10: //  yyyy-mm-dd
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = intParse(dateList[0][3]);
                            break;
                        case 11: //  mm/dd/yyyy
                            year = intParse(dateList[0][3]);
                            month = intParse(dateList[0][1]);
                            day = intParse(dateList[0][2]);
                            break;
                        case 12: //  yyyymmdd
                            year = intParse(dateList[0][1]);
                            month = intParse(dateList[0][2]);
                            day = intParse(dateList[0][3]);
                            break;
                        case 13: //  yyyy
                            year = intParse(dateList[0][1]);
                            month = 1;
                            day = 1;
                            break;
                        case 14:    //  dd[month]yyyy
                            year = intParse(dateList[0][3]);
                            month = Array.IndexOf(mMonth, dateList[0][2].ToLower()) % 12 + 1;
                            day = intParse(dateList[0][1]);
                            break;
                        case 15:    //  dd-[month]-yy
                            year = 2000 + intParse(dateList[0][3]);
                            month = Array.IndexOf(mMonth, dateList[0][2].ToLower()) % 12 + 1;
                            day = intParse(dateList[0][1]);
                            break;
                    }

                    string buf = string.Format("{0:0000}/{1:00}/{2:00}", year, month, day);
                    //  デバッグ用
                    //buf = "(" + i + ") " + buf + " ";
                    //foreach (string[] datas in dateList) {
                    //    for (int j = 0; j < datas.Length; j++)
                    //        buf += j + ": " + datas[j] + " ";
                    //}
                    return buf;
                }
            }
            return "";      //  UnMatch!!
        }

        /// <summary>
        /// 文字列が日付かを判定する
        /// yyyy年mm月dd日,mm月dd日,yyyy/mm/dd,yyyy-mm-dd,yyyymmdd,mm/dd/yyyy,GGyy年mm月dd日
        /// yyyy年mm月,yyyy年は含めない
        /// </summary>
        /// <param name="date">文字列</param>
        /// <returns></returns>
        public bool IsDateString(string date)
        {
            if (Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])年([1-9]|1[0-2])月([1-9]|[12][0-9]|3[01])日") ||            //  yyyy年mm月dd日
                Regex.IsMatch(date, "(^[1-9]|^1[0-2])月([1-9]|[12][0-9]|3[01])日") ||                                             //  mm月dd日
                Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])/([1-9]|1[0-2]|0[1-9])/([1-9]|[12][0-9]|3[01]|0[1-9])") ||   //  yyyy/mm/dd
                Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])-([1-9]|1[0-2]|0[1-9])-([1-9]|[12][0-9]|3[01]|0[1-9])") ||   //  yyyy-mm-dd
                Regex.IsMatch(date, "(^[1-9]|1[0-2]|^0[1-9])/([1-9]|[12][0-9]|3[01]|0[1-9])/(19[0-9][0-9]|2[01][0-9][0-9])") ||   //  mm/dd/yyyy
                Regex.IsMatch(date.ToLower(), "(^[0-9][0-9])(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)(19[0-9][0-9]|2[01][0-9][0-9])") ||   //  dd[month]yyyy
                Regex.IsMatch(date.ToLower(), "(^[0-9][0-9]|^[0-9])-(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)-([0-9][0-9])") ||   //  dd-[month]-yy
                (Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])(1[0-2]|0[1-9])([12][0-9]|3[01]|0[1-9])") && (date.Length == 8)) ||//  yyyymmdd
                Regex.IsMatch(date, "(^令和|^平成|^昭和|^大正|^明治)([1-9]|[0-9][0-9])年([1-9]|[01][0-2])月([1-9]|[0-2][0-9]|3[01])日"))  //  [元号]yy年mm月dd日
                return true;
            return false;
        }

        /// <summary>
        /// 文字列が年を表しているかを判定する
        /// yyyy年
        /// </summary>
        /// <param name="date">文字列</param>
        /// <returns></returns>
        public bool IsDateYearString(string date)
        {
            if (Regex.IsMatch(date, "(19[0-9][0-9]|2[01][0-9][0-9])年"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 文字列が年月を表しているか判定する
        /// yyyy年mm月
        /// </summary>
        /// <param name="date">文字列</param>
        /// <returns></returns>
        public bool IsDateMonthString(string date)
        {
            if (Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])年([1-9]|1[0-2])月"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 文字列が週単位の日付を表しているか判定する
        /// yyyy年mm月ww週,yyyy年ww週
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsDateWeekString(string date)
        {
            if (Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])年([1-9]|1[0-2])月([1-5])週") ||
                Regex.IsMatch(date, "(^19[0-9][0-9]|^2[01][0-9][0-9])年([1-9]|[1-5][0-9])週"))
                return true;
            else
                return false;
        }

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
        /// ユリウス日から年を抽出
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>西洋年</returns>
        public int JulianDay2Year(int jd)
        {
            (int year, int month, int day) = JulianDay2Date(jd);
            return year;
        }


        /// <summary>
        /// ユリウス日から月を抽出
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>月</returns>
        public int JulianDay2Month(int jd)
        {
            (int year, int month, int day) = JulianDay2Date(jd);
            return month;
        }

        /// <summary>
        /// ユリウス日から日を抽出
        /// </summary>
        /// <param name="jd">ユリウス日</param>
        /// <returns>日</returns>

        public int JulianDay2Day(int jd)
        {
            (int year, int month, int day) = JulianDay2Date(jd);
            return day;
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
        /// 和暦をユリウス日に変換
        /// </summary>
        /// <param name="wareki"></param>
        /// <returns></returns>
        public int wareki2JulianDay(string wareki)
        {
            string era = wareki.Substring(0, 2);
            int yp = wareki.IndexOf("年");
            int mp = wareki.IndexOf("月");
            int dp = wareki.IndexOf("日");
            int year = int.Parse(wareki.Substring(2, yp - 2));
            int month = int.Parse(wareki.Substring(yp + 1, mp - yp - 1));
            int day = int.Parse(wareki.Substring(mp + 1, dp - mp - 1));
            if (era.CompareTo("令和") == 0) {
                year += (int)mKaigen[0] - 1;
            } else if (era.CompareTo("平成") == 0) {
                year += (int)mKaigen[1] - 1;
            } else if (era.CompareTo("昭和") == 0) {
                year += (int)mKaigen[2] - 1;
            } else if (era.CompareTo("大正") == 0) {
                year += (int)mKaigen[3] - 1;
            } else if (era.CompareTo("明治") == 0) {
                year += (int)mKaigen[4] - 1;
            }
            return date2JulianDay(year, month, day);
        }


        /// <summary>
        /// 令和年を西暦年に変換(yy.mmdd → yyyy.mmdd)
        /// </summary>
        /// <param name="reiwa">令和年</param>
        /// <returns>西暦年</returns>
        private double reiwa2Date(double reiwa)
        {
            double dyear = (int)mKaigen[0] + (int)reiwa - 1;
            return dyear + (reiwa % 1);
        }

        /// <summary>
        /// 平成年を西暦年に変換(yy.mmdd → yyyy.mmdd)
        /// </summary>
        /// <param name="heisei">平成年</param>
        /// <returns>西暦年</returns>
        private double heisei2Date(double heisei)
        {
            double dyear = (int)mKaigen[1] + (int)heisei - 1;
            return dyear + (heisei % 1);
        }

        /// <summary>
        /// 昭和年を西暦年に変換(yy.mmdd → yyyy.mmdd)
        /// </summary>
        /// <param name="shyouwa">昭和年</param>
        /// <returns>西暦年</returns>
        private double shyouwa2Date(double shyouwa)
        {
            double dyear = (int)mKaigen[2] + (int)shyouwa - 1;
            return dyear + (shyouwa % 1);
        }

        /// <summary>
        /// 大正年を西暦年に変換(yy.mmdd → yyyy.mmdd)
        /// </summary>
        /// <param name="taisyou">大正年</param>
        /// <returns>西暦年</returns>
        private double taisyou2Date(double taisyou)
        {
            double dyear = (int)mKaigen[3] + (int)taisyou - 1;
            return dyear + (taisyou % 1);
        }

        /// <summary>
        /// 明治年を西暦年に変換(yy.mmdd → yyyy.mmdd)
        /// </summary>
        /// <param name="meiji">明治年</param>
        /// <returns>西暦年</returns>
        private double meiji2Date(double meiji)
        {
            double dyear = (int)mKaigen[4] + (int)meiji - 1;
            return dyear + (meiji % 1);
        }

        /// <summary>
        /// 西暦(yyyy/mm/dd)を和暦に変換する
        /// </summary>
        /// <param name="date">西暦(yyyy/mm/dd)(省略時は現在日)</param>
        /// <returns>和暦</returns>
        public string toWareki(string date = "")
        {
            DateTime dt;
            DateTime calenderStart = DateTime.Parse("1868/09/08");   //  カレンダの範囲1868/09/18-9999/12/31)
            if (date.Length == 0)
                dt = DateTime.Now;
            else
                dt = DateTime.Parse(date);
            //  和暦の使用準備
            JapaneseCalendar jc = new JapaneseCalendar();
            CultureInfo ci = new CultureInfo("Ja-JP", true);
            ci.DateTimeFormat.Calendar = jc;
            //  DateTime日付を和暦で表示する
            if (calenderStart < dt)
                return dt.ToString("ggy年M月d日", ci);
            else
                return getWareki(date);
        }

        /// <summary>
        /// 西暦和暦変換(文政(1818年)以降令和まで
        /// </summary>
        /// <param name="date">yyyy/mm/dd</param>
        /// <returns>[元号]yy年mm月dd日</returns>
        public string getWareki(string date)
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(5, 2));
            int day = int.Parse(date.Substring(8, 2));
            for (int i = 0; i < mKaigen.Length; i++) {
                if ((int)mKaigen[i] <= year)
                    return $"{mGengou[i]}{year - (int)mKaigen[i] + 1}年{month}月{day}日";
            }
            return $"{year}年{month}月{day}日";
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
        /// Media.ColorsよりColor Listを取得
        /// ColorSet { Color Name }
        /// </summary>
        /// <returns></returns>
        public ColorTitle[] getColorList()
        {
            return typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(i => new ColorTitle (
                    i.Name,
                    new SolidColorBrush((System.Windows.Media.Color)i.GetValue(null))
                )).ToArray();
        }

        /// <summary>
        /// BrushListからListNoの取得
        /// </summary>
        /// <param name="color">Brush値</param>
        /// <returns>ColorListNo</returns>
        public int getBrushNo(System.Windows.Media.Brush color)
        {
            if (color == null)
                return -1;
            return mBrushList.FindIndex(x => x.brush.ToString() == color.ToString());
        }

        /// <summary>
        /// Brush を色名に変換
        /// </summary>
        /// <param name="color">Brush値</param>
        /// <returns>色名</returns>
        public string getBrushName(System.Windows.Media.Brush color)
        {
            if (color == null)
                return "null";
            int index = mBrushList.FindIndex(x => x.brush.ToString() == color.ToString());
            return mBrushList[index < 0 ? 0 : index].colorTitle;
        }

        /// <summary>
        /// 色名を Brush値に変換
        /// </summary>
        /// <param name="colorName">色名</param>
        /// <returns>Brush値</returns>
        public System.Windows.Media.Brush getBrsh(string colorName)
        {
            int index = mBrushList.FindIndex(x => x.colorTitle == colorName);
            return mBrushList[index < 0 ? 0 : index].brush;
        }

        /// <summary>
        /// Brush を色名に変換
        /// </summary>
        /// <param name="color">Brush値</param>
        /// <returns>色名</returns>
        public string getColorName(System.Windows.Media.Brush color)
        {
            int index = mColorList.FindIndex(x => x.brush == color);
            return mColorList[index < 0 ? 0 : index].colorTitle;
        }

        /// <summary>
        /// 色番号から色名を取得
        /// </summary>
        /// <param name="colorNo">色番号</param>
        /// <returns>色名</returns>
        public string getColorName(int colorNo)
        {
            return mColorList[colorNo % mColorList.Count].colorTitle;
        }

        /// <summary>
        /// 色名を Brush値に変換
        /// </summary>
        /// <param name="color">色名</param>
        /// <returns>Brush値</returns>
        public System.Windows.Media.Brush getColor(string color)
        {
            int index = mColorList.FindIndex(x => x.colorTitle == color);
            return mColorList[index < 0 ? 0 : index].brush;
        }

        /// <summary>
        /// 原点を中心に回転
        /// </summary>
        /// <param name="po">点座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        /// <returns>回転後の座標</returns>
        public System.Windows.Point rotateOrg(System.Windows.Point po, double rotate)
        {
            System.Windows.Point p = new System.Windows.Point();
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
        public System.Windows.Point rotatePoint(System.Windows.Point ctr, System.Windows.Point po, double rotate)
        {
            System.Windows.Point p = new System.Windows.Point(po.X - ctr.X, po.Y - ctr.Y);
            p = rotateOrg(p, rotate);
            return new System.Windows.Point(p.X + ctr.X, p.Y + ctr.Y);
        }

        /// <summary>
        /// 原点に対する座標点の水平線との角度(rad)
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        public double angleOrg(System.Windows.Point po)
        {
            return Math.Atan2(po.Y, po.X);
        }

        /// <summary>
        /// 中心座標を指定して回転角度(rad)
        /// </summary>
        /// <param name="ctr">中心点</param>
        /// <param name="po">点座標</param>
        /// <returns>回転角度</returns>
        public double anglePoint(System.Windows.Point ctr, System.Windows.Point po)
        {
            System.Windows.Point p = new System.Windows.Point(po.X - ctr.X, po.Y - ctr.Y);
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
        public List<System.Windows.Point> circlePeakPoint(System.Windows.Point c, double r)
        {
            List<System.Windows.Point> cpList = new List<System.Windows.Point>();
            cpList.Add(new System.Windows.Point(c.X + r, c.Y));
            cpList.Add(new System.Windows.Point(c.X, c.Y + r));
            cpList.Add(new System.Windows.Point(c.X - r, c.Y));
            cpList.Add(new System.Windows.Point(c.X, c.Y - r));
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
            List<System.Windows.Point> plist = arcPeakPoint(c.toPoint(), r, sa, ea);
            foreach (System.Windows.Point p in plist)
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
        public List<System.Windows.Point> arcPeakPoint(System.Windows.Point c, double r, double sa, double ea)
        {
            System.Windows.Point sp = rotatePoint(c, new System.Windows.Point(c.X + r, c.Y), sa);
            System.Windows.Point ep = rotatePoint(c, new System.Windows.Point(c.X + r, c.Y), ea);
            List<System.Windows.Point> cpList = circlePeakPoint(c, r);     //  円としての頂点座標
            List<System.Windows.Point> plist = new List<System.Windows.Point>();
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
            List<System.Windows.Point> pointList = new List<System.Windows.Point>();
            foreach (PointD p in plist)
                pointList.Add(p.toPoint());
            List<System.Windows.Point> splist = pointSort(pointList);
            plist.Clear();
            foreach (System.Windows.Point p in splist)
                plist.Add(new PointD(p));
            return plist;
        }

        /// <summary>
        /// 点リストを中心点の角度でソート
        /// </summary>
        /// <param name="plist">点リスト</param>
        /// <returns>ソートリスト</returns>
        public List<System.Windows.Point> pointSort(List<System.Windows.Point> plist)
        {
            if (plist.Count < 2)
                return plist;
            System.Windows.Point ap = averagePoint(plist);
            if (double.IsNaN(ap.X) || double.IsNaN(ap.Y))
                return plist;
            List<(double, System.Windows.Point)> angList = new List<(double, System.Windows.Point)>();
            foreach (System.Windows.Point p in plist) {
                angList.Add((anglePoint(ap, p), p));
            }
            angList.Sort((a, b) => Math.Sign(a.Item1 - b.Item1));
            List<System.Windows.Point> spList = new List<System.Windows.Point>();
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
                pa.rotate(c, ang);
                plist.Add(pa);
            }
            PointD p = new PointD(c.x + r, c.y);
            p.rotate(c, ea);
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
            List<System.Windows.Point> pList = divideCircleList(c.toPoint(), r, div);
            List<PointD> pointList = new List<PointD>();
            foreach (System.Windows.Point p in pList)
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
        public List<System.Windows.Point> divideCircleList(System.Windows.Point c, double r, int div = 32)
        {
            List<System.Windows.Point> plist = new List<System.Windows.Point>();
            if (4 < div) {
                for (double a = 0.0; a < Math.PI * 2.0; a += Math.PI * 2.0 / div) {
                    double dx = r * Math.Cos(a);
                    double dy = r * Math.Sin(a);
                    System.Windows.Point p = new System.Windows.Point(c.X + dx, c.Y + dy);
                    plist.Add(p);
                }
            }
            return plist;
        }


        /// <summary>
        /// 一種の中心点(点リストの平均位置)
        /// </summary>
        /// <param name="pList">点リスト</param>
        /// <returns>点座標</returns>
        public System.Windows.Point averagePoint(List<System.Windows.Point> pList)
        {
            System.Windows.Point ap = new System.Windows.Point();
            foreach (System.Windows.Point lp in pList) {
                ap.X += lp.X;
                ap.Y += lp.Y;
            }
            ap.X /= pList.Count;
            ap.Y /= pList.Count;
            return ap;
        }


        /// <summary>
        /// 座標点リストで指定点に最も近い点
        /// </summary>
        /// <param name="plist">座標点リスト</param>
        /// <param name="pos">細菌傍点</param>
        /// <returns></returns>
        public PointD nearPoint(List<PointD> plist, PointD pos)
        {
            double dis = double.MaxValue;
            PointD mp = null;
            for (int i = 0; i < plist.Count; i++) {
                if (dis > pos.length(plist[i])) {
                    dis = pos.length(plist[i]);
                    mp = plist[i];
                }
            }
            return mp;
        }

        //  ---  ビットマップ処理  ----

        /// <summary>
        /// 画面の指定領域をキャプチャする
        /// </summary>
        /// <param name="ps">左上座標</param>
        /// <param name="pe">右下座標</param>
        /// <returns>Bitmapデータ</returns>
        public Bitmap getScreen(System.Drawing.Point ps, System.Drawing.Point pe)
        {
            if (ps.X > pe.X) {
                int t = ps.X;
                ps.X = pe.X;
                pe.X = t;
            }
            if (ps.Y > pe.Y) {
                int t = ps.Y;
                ps.Y = pe.Y;
                pe.Y = t;
            }
            Rectangle rectangle = new Rectangle(ps.X, ps.Y, pe.X - ps.X, pe.Y - ps.Y);
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new System.Drawing.Point(rectangle.X, rectangle.Y),
                new System.Drawing.Point(0, 0), rectangle.Size);
            graphics.Dispose();
            return bitmap;
        }

        /// <summary>
        /// アクティブウィンドウの画面をキャプチャする
        /// </summary>
        /// <returns>Bitmapデータ</returns>
        public Bitmap getActiveWindowCapture()
        {
            YLib.iRect rect;
            IntPtr activeWindow = YLib.GetForegroundWindow();
            YLib.GetWindowRect(activeWindow, out rect);
            return getScreen(new System.Drawing.Point(rect.left + 7, rect.top),
                new System.Drawing.Point(rect.right - 7, rect.bottom - 7));
        }

        /// <summary>
        /// 全画面をキャプチャする
        /// </summary>
        /// <returns>Bitmapデータ</returns>
        public System.Drawing.Bitmap getFullScreenCapture()
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                (int)SystemParameters.VirtualScreenWidth,   //  すべての画面の合計サイズ
                (int)SystemParameters.VirtualScreenHeight
                //(int)SystemParameters.FullPrimaryScreenWidth, //  プライマリモニターのサイズ
                //(int)SystemParameters.FullPrimaryScreenHeight //  (タスクバーが含まれない?)
                );
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), bitmap.Size);
            graphics.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 画像ファイルからBitmapを取得
        /// ファイルから開放してBitmapを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public System.Drawing.Bitmap getBitmap(string path)
        {
            if (!File.Exists(path))
                return null;
            MemoryStream data = new MemoryStream(File.ReadAllBytes(path));
            return new System.Drawing.Bitmap(data);
        }

        /// <summary>
        /// 画像ファイルからBitmapImageを取得
        /// ファイルからデータをメモリに展開してファイルアクセスを開放しておく
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns></returns>
        public BitmapImage getBitmapImage(string path)
        {
            if (!File.Exists(path))
                return null;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (var drawingImage = System.Drawing.Image.FromStream(fs, false, false)) {
                    BitmapImage bmpImage = cnvBitmap2BitmapImage((System.Drawing.Bitmap)drawingImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                    drawingImage.Dispose();
                    return bmpImage;
                }
            }
        }

        /// <summary>
        /// JPEGのサムネイル画像を取得する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="exif">EXIFのサムネイルデータを使う</param>
        /// <returns>ビットマップイメージ</returns>
        public BitmapImage getThumbnailImage(string path, int width, int height, bool exif = true)
        {
            if (exif) {
                BitmapImage image = getExifThumbnailImage(path);
                if (image != null)
                    return image;
            }
            using (FileStream fs = File.OpenRead(path)) {
                System.Drawing.Image org = System.Drawing.Image.FromStream(fs, false, false);
                System.Drawing.Image thumbnail = org.GetThumbnailImage(width, height, delegate { return false; }, IntPtr.Zero);
                BitmapImage bmpImage = cnvBitmap2BitmapImage((System.Drawing.Bitmap)thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg);
                thumbnail.Dispose();
                return bmpImage;
            }
        }

        /// <summary>
        /// BitmapをBitmapImageに変換
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="imageFormat">ImageFormat(Png/Jpegなど)</param>
        /// <returns>BitmapImage</returns>
        public BitmapImage cnvBitmap2BitmapImage(System.Drawing.Bitmap bitmap,
            System.Drawing.Imaging.ImageFormat imageFormat)
        {
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, imageFormat);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                memory.Close();
                return bitmapImage;
            }
        }

        /// <summary>
        /// JPEGのExifサムネイル画像(0x501B)を取出す
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <returns>ビットマップイメージ</returns>
        public BitmapImage getExifThumbnailImage(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (var drawingImage = System.Drawing.Image.FromStream(fs, false, false)) {
                    //  サムネイル画像の有無
                    if (!drawingImage.PropertyIdList.Any(propertyId => propertyId == 0x501B))
                        return null;
                    //  サムネイル画像を取出す
                    var property = drawingImage.GetPropertyItem(0x501B);
                    //  ImageからBitmapImageに変換
                    using (var ms = new MemoryStream(property.Value)) {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                        return image;
                    }
                }
            }
        }

        /// <summary>
        /// BitmapをBitmapImageに変換
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="imageFormat">ImageFormat(Png/Jpegなど)</param>
        /// <returns>BitmapImage</returns>
        public BitmapImage cnvBitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                memory.Close();
                return bitmapImage;
            }
        }

        /// <summary>
        /// BitmapImageをBitmapに変換
        /// 参考: https://code-examples.net/ja/q/62f185
        /// </summary>
        /// <param name="bitmapImage">BitmapImage</param>
        /// <returns>Bitmap</returns>
        public System.Drawing.Bitmap cnvBitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            return new System.Drawing.Bitmap(bitmapImage.StreamSource);
        }

        /// <summary>
        /// BitmapSourceをBitmapに変換
        /// 参考: https://qiita.com/YSRKEN/items/a24bf2173f0129a5825c
        /// </summary>
        /// <param name="bitmapSource">BitmapSourceデータ</param>
        /// <returns>Bitmapデータ</returns>
        public System.Drawing.Bitmap cnvBitmapSource2Bitmap(BitmapSource bitmapSource)
        {
            var bitmap = new System.Drawing.Bitmap(
                bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb
            );
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb
            );
            bitmapSource.CopyPixels(
                System.Windows.Int32Rect.Empty,
                bitmapData.Scan0,
                bitmapData.Height * bitmapData.Stride,
                bitmapData.Stride
            );
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// BitMap からBitmapSourceに変換
        /// https://qiita.com/YSRKEN/items/a24bf2173f0129a5825c
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapSource bitmap2BitmapSource(System.Drawing.Bitmap bitmap)
        {
            // MemoryStreamを利用した変換処理
            using (var ms = new System.IO.MemoryStream()) {
                // MemoryStreamに書き出す
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                // MemoryStreamをシーク
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                // MemoryStreamからBitmapFrameを作成
                // (BitmapFrameはBitmapSourceを継承しているのでそのまま渡せばOK)
                System.Windows.Media.Imaging.BitmapSource bitmapSource =
                        System.Windows.Media.Imaging.BitmapFrame.Create(
                        ms,
                        System.Windows.Media.Imaging.BitmapCreateOptions.None,
                        System.Windows.Media.Imaging.BitmapCacheOption.OnLoad
                    );
                return bitmapSource;
            }
        }

        /// <summary>
        /// BitmapSourceからBitmapImageに変換(動作未確認)
        /// https://stackoverflow.com/questions/5338253/bitmapsource-to-bitmapimage
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        public BitmapImage bitmapSource2BitmapImage(BitmapSource bitmapSource)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();

            memoryStream.Close();

            return bImg;
        }

        /// <summary>
        /// Bitmapデータをトリミングする
        /// </summary>
        /// <param name="bitmap">Bitmapデータ</param>
        /// <param name="sp">領域の始点</param>
        /// <param name="ep">領域の終点</param>
        /// <returns>切り取ったBitmapデータ</returns>
        public System.Drawing.Bitmap trimingBitmap(System.Drawing.Bitmap bitmap, System.Windows.Point sp, System.Windows.Point ep)
        {
            System.Windows.Rect rect = new System.Windows.Rect(sp, ep);
            //  画像をトリミングする
            System.Drawing.Rectangle rectAngle = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            System.Drawing.Bitmap resultImg = bitmap.Clone(rectAngle, bitmap.PixelFormat);
            bitmap.Dispose();
            return resultImg;
        }

        /// <summary>
        /// BitmapImageをCanvasに登録する
        /// </summary>
        /// <param name="canvas">Canvas</param>
        /// <param name="bitmapImage">BitmapImage</param>
        /// <param name="ox">原点(左上)</param>
        /// <param name="oy">原点(左上)</param>
        /// <param name="width">イメージの幅</param>
        /// <param name="height">イメージの高さ</param>
        /// <returns>登録位置</returns>
        public int setCanvasBitmapImage(System.Windows.Controls.Canvas canvas, BitmapImage bitmapImage, double ox, double oy, double width, double height)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = bitmapImage;
            img.Width = width;
            img.Height = height;
            img.Margin = new System.Windows.Thickness(ox, oy, 0, 0);
            return canvas.Children.Add(img);
        }

        /// <summary>
        /// 画像を縦方向に連結
        /// https://imagingsolution.net/program/csharp/image_combine/
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public System.Drawing.Bitmap verticalCombineImage(System.Drawing.Bitmap[] src)
        {
            int dstWidth = 0;
            int dstHeight = 0;
            System.Drawing.Imaging.PixelFormat dstPixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;   //  256色
            for (int i = 0; i < src.Length; i++) {
                //  サイズの拡張
                if (dstWidth < src[i].Width)
                    dstWidth = src[i].Width;
                dstHeight += src[i].Height;
                //  最大ビット数の検索
                if (System.Drawing.Bitmap.GetPixelFormatSize(dstPixelFormat) <
                    System.Drawing.Bitmap.GetPixelFormatSize(src[i].PixelFormat)) {
                    dstPixelFormat = src[i].PixelFormat;
                }
            }

            var dst = new System.Drawing.Bitmap(dstWidth, dstHeight, dstPixelFormat);
            var dstRect = new System.Drawing.Rectangle();

            using (var g = System.Drawing.Graphics.FromImage(dst)) {
                for (int i = 0; i < src.Length; i++) {
                    dstRect.Width = src[i].Width;
                    dstRect.Height = src[i].Height;
                    //  描画
                    g.DrawImage(src[i], dstRect, 0, 0, src[i].Width, src[i].Height, System.Drawing.GraphicsUnit.Pixel);
                    //  次の描画先
                    dstRect.Y = dstRect.Bottom;
                }
            }
            return dst;
        }

        /// <summary>
        /// 画像の水平方向に連結
        /// https://imagingsolution.net/program/csharp/image_combine/
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public System.Drawing.Bitmap horizontalCombineImage(System.Drawing.Bitmap[] src)
        {
            int dstWidth = 0;
            int dstHeight = 0;
            System.Drawing.Imaging.PixelFormat dstPixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;   //  256色
            for (int i = 0; i < src.Length; i++) {
                //  サイズの拡張
                if (dstHeight < src[i].Height)
                    dstHeight = src[i].Height;
                dstWidth += src[i].Width;
                //  最大ビット数の検索
                if (System.Drawing.Bitmap.GetPixelFormatSize(dstPixelFormat) <
                    System.Drawing.Bitmap.GetPixelFormatSize(src[i].PixelFormat)) {
                    dstPixelFormat = src[i].PixelFormat;
                }
            }

            var dst = new System.Drawing.Bitmap(dstWidth, dstHeight, dstPixelFormat);
            var dstRect = new System.Drawing.Rectangle();

            using (var g = System.Drawing.Graphics.FromImage(dst)) {
                for (int i = 0; i < src.Length; i++) {
                    dstRect.Width = src[i].Width;
                    dstRect.Height = src[i].Height;
                    //  描画
                    g.DrawImage(src[i], dstRect, 0, 0, src[i].Width, src[i].Height, System.Drawing.GraphicsUnit.Pixel);
                    //  次の描画先
                    dstRect.X = dstRect.Right;
                }
            }
            return dst;
        }

        /// <summary>
        /// 画像データをファイルに保存
        /// </summary>
        /// <param name="bitmapSource">ビットマップデータ</param>
        /// <param name="filePath">ファイル名(png/jpg/bmp)</param>
        public void saveBitmapImage(BitmapSource bitmapSource, string filePath)
        {
            string ext = System.IO.Path.GetExtension(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                BitmapEncoder encoder = new PngBitmapEncoder();
                if (ext.ToLower().CompareTo(".png") == 0)
                    encoder = new PngBitmapEncoder();
                else if (ext.ToLower().CompareTo(".jpeg") == 0 || ext.ToLower().CompareTo(".jpg") == 0)
                    encoder = new JpegBitmapEncoder();
                else if (ext.ToLower().CompareTo(".bmp") == 0)
                    encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Drawing.Color から Media.Color に変換
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public System.Windows.Media.Color Draw2MediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Media.Color から Drawing.Colorに変換
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public System.Drawing.Color Media2DrawColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// CanvasをBitmapに変換
        /// 参照  https://qiita.com/tricogimmick/items/894914f6bbe224a45d49
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public BitmapSource canvas2Bitmap(Canvas canvas, double offsetX = 0, double offsetY = 0)
        {
            if (canvas.ActualWidth == 0 || canvas.ActualHeight == 0)
                return null;
            //  位置は CanvasのVisaulOffset値を設定したいが直接取れないので
            //Point preLoc = new Point(mMainWindow.lbCommand.ActualWidth + 10, 0);
            System.Windows.Point preLoc = new System.Windows.Point(offsetX, offsetY);
            // レイアウトを再計算させる
            var size = new System.Windows.Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);
            canvas.Arrange(new Rect(new System.Windows.Point(0, 0), size));

            // VisualObjectをBitmapに変換する
            var renderBitmap = new RenderTargetBitmap((int)size.Width,       // 画像の幅
                                                      (int)size.Height,      // 画像の高さ
                                                      96.0d,                 // 横96.0DPI
                                                      96.0d,                 // 縦96.0DPI
                                                      PixelFormats.Pbgra32); // 32bit(RGBA各8bit)
            renderBitmap.Render(canvas);
            //  Canvasの位置を元に戻す(Canvas.VisualOffset値)
            canvas.Arrange(new Rect(preLoc, size));
            return renderBitmap;
        }

        /// <summary>
        /// Bitmap 図形を移動させる
        /// Bitmapをdx,dy分移動させてCanvasに貼り付ける(dx,dyのマイナス方向も可)
        /// オフセットで指定した分Bitmapサイズをカットする(ポリゴンの塗潰しの境界線をカットするために必要)
        /// </summary>
        /// <param name="bitmapSource">Bitmap</param>
        /// <param name="dx">移動量</param>
        /// <param name="dy">移動量</param>
        /// <param name="offset">オフセット</param>
        public void moveImage(Canvas canvas, BitmapSource bitmapSource, double dx, double dy, int offset = 0)
        {
            System.Drawing.Bitmap bitmap = cnvBitmapSource2Bitmap(bitmapSource);
            double width = bitmap.Width - Math.Abs(dx);
            double height = bitmap.Height - Math.Abs(dy);
            System.Windows.Point sp = new System.Windows.Point(dx > 0 ? offset : -dx, dy > 0 ? offset : -dy);
            System.Windows.Point ep = new System.Windows.Point(sp.X + width - offset, sp.Y + height - offset);
            System.Drawing.Bitmap moveBitmap = trimingBitmap(bitmap, sp, ep);
            BitmapImage bitmapImage = cnvBitmap2BitmapImage(moveBitmap);
            setCanvasBitmapImage(canvas, bitmapImage, dx > 0 ? dx + offset : 0, dy > 0 ? dy + offset : 0,
                width - offset, height - offset);
        }

        /// <summary>
        /// Imageコントロールの画像をクリップボードに貼り付ける
        /// </summary>
        /// <param name="imageSource">Image.Source</param>
        public void image2Clipbord(System.Windows.Media.ImageSource imageSource)
        {
            System.Windows.Media.Imaging.BitmapSource bitmapSource = (System.Windows.Media.Imaging.BitmapSource)imageSource;
            System.Drawing.Bitmap bitmap = cnvBitmapSource2Bitmap(bitmapSource);
            //  クリップボードに張り付ける
            System.Windows.Clipboard.SetImage(bitmap2BitmapSource(bitmap));
        }

        /// <summary>
        /// JPEGファイルからIPTC情報の取得
        /// https://stackoverflow.com/questions/5597079/iptc-net-read-write-c-sharp-library
        /// http://msdn.microsoft.com/en-us/library/system.windows.media.imaging.aspx
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> getIPTC(string path)
        {
            List<string> iptcList = new List<string>();
            try {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.None);
                var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
                if (metadata != null) {
                    iptcList.Add(metadata.CameraManufacturer == null ? "" : metadata.CameraManufacturer.Trim());
                    iptcList.Add(metadata.CameraModel == null ? "" : metadata.CameraModel.Trim());
                    iptcList.Add(metadata.Copyright == null ? "" : metadata.Copyright.Trim());
                    iptcList.Add(metadata.DateTaken == null ? "" : metadata.DateTaken.Trim());
                    iptcList.Add(metadata.Title == null ? "" : metadata.Title.Trim());
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine("Excepsion Error getIPTC : " + e.Message);
            }
            return iptcList;
        }

        /// <summary>
        /// JPEGファイルの全IPTC情報を文字列化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string getIPTCall(string path)
        {
            List<string> iptcInf = getIPTC(path);
            string buf = "IPTC情報";
            buf += 0 < iptcInf[0].Length ? "\nメーカー : " + iptcInf[0] : "";
            buf += 0 < iptcInf[1].Length ? "\nモデル   : " + iptcInf[1] : "";
            buf += 0 < iptcInf[2].Length ? "\nCopyright: " + iptcInf[2] : "";
            buf += 0 < iptcInf[3].Length ? "\n日付     : " + iptcInf[3] : "";
            buf += 0 < iptcInf[4].Length ? "\nタイトル : " + iptcInf[4] : "";
            return buf;
        }


        //  ---  配列処理  ---

        /// <summary>
        /// 2次元配列のソート(数値)
        /// </summary>
        /// <param name="data">2D配列データ</param>
        /// <param name="col">ソートカラム</param>
        /// <returns>配列データ/returns>
        public double[,] doubleArray2Sort(double[,] data, int col = 0)
        {
            List<double[]> b = new();
            col = Math.Min(data.GetLength(0) - 1, col);
            for (int i = 0; i < data.GetLength(0); i++) {
                double[] buf = new double[data.GetLength(1)];
                for (int j = 0; j < data.GetLength(1); j++)
                    buf[j] = data[i, j];
                b.Add(buf);
            }
            b.Sort((a, b) => (int)(a[col] - b[col]));
            for (int i = 0; i < data.GetLength(0); i++) {
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = b[i][j];
            }
            return data;
        }

        /// <summary>
        /// 2次元配列のソート(文字列))
        /// </summary>
        /// <param name="data">2D配列データ</param>
        /// <param name="col">ソートカラム</param>
        /// <returns>配列データ</returns>
        public string[,] stringArray2Sort(string[,] data, int col = 0)
        {
            List<string[]> b = new();
            col = Math.Min(data.GetLength(0) - 1, col);
            for (int i = 0; i < data.GetLength(0); i++) {
                string[] buf = new string[data.GetLength(1)];
                for (int j = 0; j < data.GetLength(1); j++)
                    buf[j] = data[i, j];
                b.Add(buf);
            }
            b.Sort((a, b) => a[col].CompareTo(b[col]));
            for (int i = 0; i < data.GetLength(0); i++) {
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = b[i][j];
            }
            return data;
        }



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
        public System.Windows.Point cnvCoordinate(string coordinate)
        {
            char[] stripChars = new char[] { ' ', '.' };
            double latitude = 0.0;
            double longitude = 0.0;
            int a1 = coordinate.IndexOf("北緯");
            int b1 = coordinate.IndexOf("東経");
            if (a1 < 0 || b1 < 0)
                return new System.Windows.Point(0, 0);
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

            return new System.Windows.Point(longitude, latitude);
        }

        /// <summary>
        /// 度分秒表示の座標を度に変換する(正規表現を使う)
        /// 北緯45度22分21秒 東経141度00分57秒 → 緯度 45.3725 経度 141.015833333333 
        /// </summary>
        /// <param name="coordinate">座標</param>
        /// <returns></returns>
        public System.Windows.Point cnvCoordinate2(string coordinate)
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

                return new System.Windows.Point(longitude, latitude);
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

                return new System.Windows.Point(longitude, latitude);
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

                return new System.Windows.Point(longitude, latitude);
            }

            return new System.Windows.Point(0.0, 0.0);
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

        //  ---  bit演算関連  ------

        /// <summary>
        /// OnBit位置の取得
        /// </summary>
        /// <param name="bytes">bitリスト</param>
        /// <returns>On位置リスト</returns>
        public List<int> getBitOnNo(byte[] bytes)
        {
            List<int> bitNoList = new List<int>();
            for (int i = 0; i < bytes.Length * 8; i++) {
                if (getBitOn(bytes, i))
                    bitNoList.Add(i);
            }
            return bitNoList;
        }

        /// <summary>
        /// Bit配列がすべて 0 かの確認
        /// </summary>
        /// <param name="bytes">bitリスト</param>
        /// <returns></returns>
        public bool IsEmpty(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] != 0)
                    return false;
            return true;
        }

        /// <summary>
        /// 2つの　bitリストの and で同一のbitがあればtrue
        /// </summary>
        /// <param name="bytesA">bitリストA</param>
        /// <param name="bytesB">bitリストB</param>
        /// <returns>同一bitの有無</returns>
        public bool bitAnd(byte[] bytesA, byte[] bytesB)
        {
            for (int i = 0; i < bytesA.Length; i++) {
                byte b = (byte)(bytesA[i] & bytesB[i]);
                if (b != 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Bitデータで指定位置のBitがOnかの確認
        /// </summary>
        /// <param name="bytes">Bitデータ</param>
        /// <param name="n">Bit位置</param>
        /// <returns>On</returns>
        public bool getBitOn(byte[] bytes, int n)
        {
            byte lb = bytes[n / 8];
            byte b = 1;
            b <<= n % 8;
            return 0 != (lb & b);
        }

        /// <summary>
        /// すべてのBitをOn/Offにする
        /// </summary>
        /// <param name="bytes">Bitデータ</param>
        /// <param name="off">0クリア</param>
        public void bitOnAll(byte[] bytes, bool off = false)
        {
            for (int i = 0; i < bytes.Length; i++) {
                if (off)
                    bytes[i] = 0x00;
                else
                    bytes[i] = 0xff;
            }
        }

        /// <summary>
        /// BitデータをBit位置指定でOnにする
        /// </summary>
        /// <param name="bytes">Bitデータ</param>
        /// <param name="n">Bit位置</param>
        public void bitOn(byte[] bytes, int n)
        {
            byte b = 1;
            b <<= n % 8;
            bytes[n / 8] |= b;
        }

        /// <summary>
        /// BitデータをBit位置指定でOffにする
        /// </summary>
        /// <param name="bytes">Bitデータ</param>
        /// <param name="n">Bit位置</param>
        public void bitOff(byte[] bytes, int n)
        {
            byte b = 1;
            b <<= n % 8;
            bytes[n / 8] &= (byte)~b;
        }

        /// <summary>
        /// 16進文字列に変換
        /// </summary>
        /// <param name="bytes">Bitデータ</param>
        /// <param name="start">開始位置</param>
        /// <param name="size">サイズ</param>
        /// <returns>16進文字列</returns>
        public string binary2HexString(byte[] bytes, int start = 0, int size = 0)
        {
            string buf = "";
            if (size == 0) size = bytes.Length;
            for (int i = start; i < start + size && i < bytes.Length; i++) {
                buf += string.Format("{0:X2} ", bytes[i]);
            }
            return buf;
        }

        /// <summary>
        /// byte配列をASCii文字列に変換
        /// </summary>
        /// <param name="data">byte配列</param>
        /// <param name="start">開始位置</param>
        /// <param name="size">サイズ</param>
        /// <returns>文字列</returns>
        public string binary2AsciiString(byte[] data, int start = 0, int size = 0)
        {
            string buf = "";
            for (int i = start; i < start + size && i < data.Length; i++) {
                if (data[i] == 0)
                    break;
                buf += (char)data[i];
            }
            return buf;
        }

        /// <summary>
        /// intリストをbyte配列に変換
        /// </summary>
        /// <param name="intlist">intリスト</param>
        /// <returns>byte配列</returns>
        public byte[] intList2ByteArray(List<Int32> intlist)
        {
            byte[] byteArray = new byte[intlist.Count * sizeof(Int32)];
            for (int i = 0; i < intlist.Count; i++) {
                byte[] b = BitConverter.GetBytes(intlist[i]);
                Buffer.BlockCopy(b, 0, byteArray, sizeof(Int32) * i, sizeof(Int32));
            }
            return byteArray;
        }

        /// <summary>
        /// byteデータの指定範囲をコピーする
        /// </summary>
        /// <param name="a">byte配列</param>
        /// <param name="start">開始位置</param>
        /// <param name="size">サイズ</param>
        /// <returns>byte配列</returns>
        public byte[] ByteCopy(byte[] a, int start, int size)
        {
            byte[] b = new byte[size];
            for (int i = 0; i < size; i++)
                b[i] = a[i + start];
            return b;
        }

        /// <summary>
        /// byte配列にbyteデータを上書きする
        /// </summary>
        /// <param name="src">元データのbyte配列</param>
        /// <param name="start">上書き開始位置</param>
        /// <param name="dest">上書きデータのbyte配列</param>
        /// <returns>変換データ</returns>
        public byte[] ByteOverWrite(byte[] src, int start, byte[] dest)
        {
            int j = 0;
            for (int i = start; i < src.Length; i++) {
                if (j < dest.Length)
                    src[i] = dest[j++];
            }
            return src;
        }

        /// <summary>
        /// byte配列同士を連結する
        /// </summary>
        /// <param name="src1">byte配列データ1</param>
        /// <param name="src2">byte配列データ2</param>
        /// <returns>連結データ</returns>
        public byte[] ByteCat(byte[] src1, byte[] src2)
        {
            byte[] dest = new byte[src1.Length + src2.Length];
            dest = ByteOverWrite(dest, 0, src1);
            dest = ByteOverWrite(dest, src1.Length, src2);
            return dest;
        }
    }
}
