using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// Web個別データの取得
    /// </summary>
    public class WikiData
    {
        public string mTitle;           //  タイトル名
        public string mUrl;             //  URL
        public string mComment;         //  コメント
        public string mData;            //  詳細データ
        public string mListTitle;       //  親リストのタイトル
        public string mListUrl;         //  親リストのURL
        public string mSearchForm;      //  一覧の抽出方法

        private YLib ylib = new YLib();
        private HtmlLib hlib = new HtmlLib();

        public WikiData()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="url">URL</param>
        /// <param name="comment">コメント</param>
        /// <param name="data">詳細データ</param>
        /// <param name="listTitle">親リストのタイトル</param>
        /// <param name="listUrl">親リストのURL</param>
        /// <param name="searchForm">一覧の抽出方法</param>
        public WikiData(string title, string url, string comment, string data = "", string listTitle = "", string listUrl = "", string searchForm = "")
        {
            mTitle = title;
            mUrl = url;
            mComment = comment;
            mData = data;
            mListTitle = listTitle;
            mListUrl = listUrl;
            mSearchForm = searchForm;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data"></param>
        public WikiData(string[] data)
        {
            //  コントロールコードを元に戻す
            string[] cdata = new string[data.Length];
            for (int i = 0; i < data.Length; i++) {
                cdata[i] = ylib.strControlCodeRev(data[i]);
            }
            //  配列データをクラスデータに移動
            mTitle      = 0 < data.Length ? cdata[0] : "";
            mUrl        = 1 < data.Length ? cdata[1] : "";
            mComment    = 2 < data.Length ? cdata[2] : "";
            mData       = 3 < data.Length ? cdata[3] : "";
            mListTitle  = 4 < data.Length ? cdata[4] : "";
            mListUrl    = 5 < data.Length ? cdata[5] : "";
            mSearchForm = 6 < data.Length ? cdata[6] : "";
        }

        public override string ToString()
        {
            return mTitle + "," + mUrl + "," +mComment;
        }

        /// <summary>
        /// 基本データのみ配列に変換
        /// </summary>
        /// <returns>配列データ</returns>
        public string[] ToBaseArray()
        {
            string[] buf = { mTitle, mUrl, mComment };
            return buf;
        }

        /// <summary>
        /// 全情報を配列に変換
        /// </summary>
        /// <returns>配列データ</returns>
        public string[] ToArray()
        {
            string[] buf = { mTitle, mUrl, mComment, mData, mListTitle, mListUrl, mSearchForm };
            return buf;
        }

        /// <summary>
        /// 全情報のコントロールコードを変換した配列に変換
        /// </summary>
        /// <returns></returns>
        public string[] convControlCode()
        {
            string[] buf = ToArray();
            string[] convData = new string[buf.Length];
            for (int i = 0; i < buf.Length; i++) {
                convData[i] = ylib.strControlCodeCnv(buf[i]);
            }
            return convData;
        }

        /// <summary>
        /// Webからの詳細データ取得
        /// </summary>
        /// <param name="baseUrl">ベースURL</param>
        public void getTagSetData(string baseUrl)
        {
            //  Webデータの取得
            string url = baseUrl + mUrl;
            string html= ylib.getWebText(url);
            if (html == null || 0 == html.Length)
                return;
            List<string[]> data = getInfoData(html);
            mData = "";
            for (int i = 0; i < data.Count; i++) {
                mData += data[i][0] + (0 < data[i][0].Length ? ": " : "") + data[i][1] + "\n";
            }
        }

        /// <summary>
        /// 個別Webデータをリストにまとめる
        /// </summary>
        /// <param name="html"><HTMLソース/param>
        /// <returns>リストデータ</returns>
        public List<string[]> getInfoData(string html)
        {
            List<string[]> dataList = new List<string[]>();
            if (html == null || html.Length <= 0)
                return null;
            //  タイトル
            string title = getTitle(html);
            string[] buf = new string[] { "タイトル", title };
            dataList.Add(buf);
            //  イントロダクション
            buf = new string[] { "序文", getIntroduction(html) };
            dataList.Add(buf);
            //  座標
            string coordinate = getCoordinateString(html);
            if(0 < coordinate.Length) {
                buf = new string[] { "", coordinate };
                dataList.Add(buf);
            }
            //  基本情報
            buf = new string[] { "基本情報", "" };
            dataList.Add(buf);
            List<string[]> listBuf = getBaseInfoData(html);
            dataList.AddRange(listBuf);

            return dataList;
        }

        /// <summary>
        /// データからタイトル(タグ)の取得
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>タイトル</returns>
        private string getTitle(string html)
        {
            (string para, string data, string nextSrc) = hlib.getHtmlTagData(html, "title");
            return data;
        }

        /// <summary>
        /// 序文の取得
        /// 最初の段落(<p> ・・・ </p>p>)のデータを抽出
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>抽出データ</returns>
        private string getIntroduction(string html)
        {
            //  段落の検出
            int bs = html.IndexOf("<p>");
            int be = bs;
            (bs, be) = hlib.getHtmlTagDataPos(html, "p", be);
            if (bs >= be)
                return "";
            string tmp = html.Substring(be, be - bs);

            //  段落データの抽出
            string introData = html.Substring(bs, be - bs + 1);
            if (0 < introData.IndexOf("<span id=\"coordinates\"")) {
                //  最初の段落に座標データの場合があるので、その時は次の段落を使う段落の検出
                (bs, be) = hlib.getHtmlTagDataPos(html, "p", be);
                if (bs >= be)
                    return "";
                introData = html.Substring(bs, be - bs + 1);
            }
            //introData = ylib.stripHtmlTagData(introData, "span");

            //  データを抽出してリスト化
            List<string> tagList = hlib.getHtmlTagDataAll(introData);

            string data = string.Join(" ", tagList);
            data = hlib.cnvHtmlSpecialCode(data);   //  HTML特殊コード変換
            data = data.Replace("\n", " ");
            return ylib.stripBrackets(data);        //  大括弧内の文字列を括弧ごと除く
        }

        /// <summary>
        /// tableデータから基本情報のデータを取得する
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>抽出データ</returns>
        private List<string[]> getBaseInfoData(string html)
        {
            List<string[]> tagDataList = new List<string[]>();          //  Wiki抽出データ

            //  <table> </table>情報の抽出
            string tagPara, tagData, nextSrc;
            do {
                (tagPara, tagData, nextSrc) = hlib.getHtmlTagData(html, "table");
                html = nextSrc;
            } while (0 < tagPara.Length && tagPara.IndexOf("infobox") < 0 && 0 < html.Length);

            //  表内データの抽出
            html = tagData;
            //  不要データの除去
            html = hlib.stripHtmlTagData(html, "style");
            while (0 < html.Length) {
                //  表の行単位のデータ抽出
                (tagPara, tagData, nextSrc) = hlib.getHtmlTagData(html, "tr");
                if (tagData.Length < 1)
                    break;
                (string title, string data) = getTableRowData(tagData);
                if (0 < title.Length && 0 < data.Length) {
                    data = ylib.stripBrackets(data);
                    tagDataList.Add(new string[] { title, data.Replace("\n", " ") });
                }
                html = nextSrc;
            }
            return tagDataList;
        }

        /// <summary>
        /// HTMLソースから表の1行分のデータ(<tr></tr>)から
        /// <th></th>と<td></td>をタイトルとデータして取得
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>(タイトル、データ)</returns>
        private (string title, string data) getTableRowData(string html)
        {
            (string tagPara, string tagData, string nextSrc) = hlib.getHtmlTagData(html, "th");
            List<string> titleList = hlib.getHtmlTagDataAll(tagData);
            (tagPara, tagData, nextSrc) = hlib.getHtmlTagData(nextSrc, "td");
            List<string> dataList = hlib.getHtmlTagDataAll(tagData);

            string title = "";
            foreach (string data in titleList)
                title += (title.Length > 0 ? "," : "") + data;
            string para = "";
            foreach (string data in dataList)
                para += data;
            return (title, para);
        }

        /// <summary>
        /// 基本情報とは別に座標情報の取得
        /// ページ上部の段落の中に<span > ～ </span> で記述されている場合
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>座標文字列</returns>
        private string getCoordinateString(string html)
        {
            int bs = html.IndexOf("<span id=\"coordinates");
            if (bs <0)
                bs = html.IndexOf("<span class=\"geo-dms");
            int be = 0;
            if (bs < 0)
                return "";

            (bs, be) = hlib.getHtmlTagDataPos(html, "span", bs);
            string coordData = html.Substring(bs, be - bs + 1);
            coordData = hlib.stripHtmlTagData(coordData, "style");
            List<string> tagList = hlib.getHtmlTagDataAll(coordData);
            return string.Join(" ", tagList);
        }

    }
}
