using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// Wikipediaの一覧ページから一覧参照データを抽出する
    /// </summary>
    public class WikiDataList
    {
        public List<WikiData> mDataList = new List<WikiData>(); //  詳細データリスト
        public List<string> mFormatTitle = new List<string>();  //  固定データを除くデータのタイトル
        public enum SEARCHFORM {                                //  リスト検索(抽出)方式
            NON, LISTLIMIT, LISTUNLIMIT, TABLE, GROUP, REFERENCELIMIT, REFERENCE, TABLE_LIST
        };
        public SEARCHFORM mSearchForm = SEARCHFORM.NON;
        public string[] mSearchFormTitle = {
            "自動", "箇条書き,制限あり", "箇条書き", "表形式", "グループ形式",
            "参照,制限あり", "参照", "表・箇条書き" };
        private string[] mStopTagData = new string[] {          //  一覧ページの読込中断キーワード
            "脚注", "References", "関連項目", "参考文献", "外部リンク" };
        public string[] mTitle = {                              //  リストタイトル
            "タイトル", "URL", "コメント",　"基本情報", "リストタイトル", "親URL", "一覧検索方法"
        };

        private HtmlLib hlib = new HtmlLib();
        private YLib ylib = new YLib();

        /// <summary>
        /// WikiDataのリストを配列リストデータに変換
        /// </summary>
        /// <returns>配列リストデータ</returns>
        public List<string[]> getArrayList()
        {
            List<string[]> dataList = new List<string[]>();
            foreach (WikiData data in mDataList) {
                dataList.Add(data.ToArray());
            }
            return dataList;
        }

        /// <summary>
        /// タイトルでデータを検索する
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <returns>検索データ(null)</returns>
        public WikiData getTitleData(string title)
        {
            int index = mDataList.FindIndex(p => p.mTitle.CompareTo(title) == 0);
            if (0 <= index)
                return mDataList[index];
            else
                return null;
        }

        /// <summary>
        /// リストデータをファイルに保存
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public void saveData(string filePath)
        {
            //  コントロールコードの変換
            List<string[]> convDataList = new List<string[]>();
            foreach (WikiData data in mDataList) {
                convDataList.Add(data.convControlCode());
            }
            //  ファイル保存
            ylib.saveCsvData(filePath, mTitle, convDataList);
        }

        /// <summary>
        /// リストデータを読み込む
        /// </summary>
        /// <param name="filePath"></param>
        public void loadData(string filePath)
        {
            List<string[]> dataList = ylib.loadCsvData(filePath);
            if (1 < dataList.Count) {
                //mTitle = dataList[0];
                mDataList.Clear();
                for (int i = 1; i < dataList.Count; i++) {
                    mDataList.Add(new WikiData(dataList[i]));
                }
            }
        }

        /// <summary>
        /// ikipediaの一覧リストの取得
        /// </summary>
        /// <param name="html">リストのHTMLソース</param>
        public void getWikiDataList(string html)
        {
            SEARCHFORM searchForm = SEARCHFORM.NON;
            mDataList.Clear();
            mFormatTitle.Clear();
            if (html != null) {
                //  箇条書き,制限あり
                if (mSearchForm == SEARCHFORM.LISTLIMIT || mSearchForm == SEARCHFORM.NON) {
                    searchForm = SEARCHFORM.LISTLIMIT;
                    mDataList = getWikiDataList(html, "li");
                }
                //  表形式
                if (mSearchForm == SEARCHFORM.TABLE || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.TABLE;
                    mDataList = getWikiDataList(html, "tr", false);
                }
                //  グループ形式
                if (mSearchForm == SEARCHFORM.GROUP || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.GROUP;
                    mDataList = getWikiDataList(html, "span", false);
                }
                // 箇条書き,制限なし
                if (mSearchForm == SEARCHFORM.LISTUNLIMIT || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.LISTUNLIMIT;
                    mDataList = getWikiDataList(html, "li", false);
                }
                // 参照,制限あり
                if (mSearchForm == SEARCHFORM.REFERENCELIMIT || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.REFERENCELIMIT;
                    mDataList = getWikiDataList(html, "a", true);
                }
                // 参照,制限なし
                if (mSearchForm == SEARCHFORM.REFERENCE || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.REFERENCE;
                    mDataList = getWikiDataList(html, "a", false);
                }
                //  表＋箇条書き
                if (mSearchForm == SEARCHFORM.TABLE_LIST || (mSearchForm == SEARCHFORM.NON && mDataList.Count < 25)) {
                    searchForm = SEARCHFORM.TABLE_LIST;
                    mDataList = getWikiDataList(html, "tr");    //  表形式
                    mDataList.AddRange(getWikiDataList(html, "li"));   //  箇条書き,制限あり
                }
                mSearchForm = searchForm;
            }
        }

        /// <summary>
        /// 箇条書きや表形式などのHTMLデータからリストを抽出
        /// フィルタリングタグ li : 箇条書き, tr : 表形式, span : グループ形式, a : 参照のみ
        /// 検索制限　HTMLソース中に"脚注", "References", "関連項目", "参考文献"のキーワードが
        ///           検出されたところでリストの抽出を辞める
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="filterTag">フィルタリングタグ(抽出方法)</param>
        /// <returns></returns>
        private List<WikiData> getWikiDataList(string html, string filterTag, bool limitOn = true)
        {
            //  検索終了位置を求める
            int limitPos = limitOn ? limitDataPosition(html) : -1;
            //  Wikiデータの検索
            List<WikiData> dataList;
            if (filterTag.CompareTo("tr") == 0) {
                int pos = 0;
                int sp, ep;
                string tagPara, tagData;
                dataList = new List<WikiData>();
                do {
                    (tagPara, tagData, html, sp, ep) = hlib.getHtmlTagData(html, "tbody", pos);
                    if (0 < tagData.Length)
                        dataList.AddRange(getWikiDataItem(tagData, filterTag, limitPos));
                    else
                        break;
                } while (0 < html.Length);
            } else {
                dataList = getWikiDataItem(html, filterTag, limitPos);
            }
            return dataList;
        }

        /// <summary>
        /// Wikiデータのアイテムリストに登録
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="filterTag">抽出タグ名</param>
        /// <param name="secondTitle">表の抽出タイトルを2列目にする</param>
        /// <returns></returns>
        private List<WikiData> getWikiDataItem(string html, string filterTag, int limitPos)
        {
            List<WikiData> dataList = new List<WikiData> ();
            int pos = 0;
            int sp = 0, ep = 0;
            string tagPara, tagData, nextSrc;

            while ((limitPos < 0 || pos < limitPos) && pos < html.Length) {
                (tagPara, tagData, nextSrc, sp, ep) = hlib.getHtmlTagData(html, filterTag, pos);
                if (0 < tagData.Length && sp < ep) {
                    if ((filterTag.CompareTo("li") == 0 || filterTag.CompareTo("tr") == 0) &&
                            (0 <= tagData.IndexOf("<" + filterTag + ">") || 0 <= tagData.IndexOf("<" + filterTag + " "))) {
                        listSetTagData(dataList, tagData, tagPara, filterTag);
                        //  入れ子データを再帰処理
                        dataList.AddRange(getWikiDataItem(tagData, filterTag, limitPos));
                    } else {
                        listSetTagData(dataList, tagData, tagPara, filterTag);
                    }
                } else {

                }
                pos = ep + 1;
            }
            return dataList;
        }

        /// <summary>
        /// データをリストに登録
        /// </summary>
        /// <param name="dataList">登録リスト</param>
        /// <param name="tagData">対象タグデータ</param>
        /// <param name="tagPara">タグのパラメータ</param>
        /// <param name="filterTag">フィルタのタグ名</param>
        private void listSetTagData(List<WikiData> dataList, string tagData, string tagPara, string filterTag)
        {
            tagData = hlib.stripHtmlTagData(tagData, "style");
            tagData = hlib.stripHtmlTagData(tagData, "rb");             //  ルビもとを除外、ただしルビ(rt)は残す)
            if (filterTag.CompareTo("a") == 0) {
                //  参照のみに対応(日本の港湾一覧)
                if (0 < tagPara.Length) {
                    string title = hlib.stripHtmlParaData(tagPara, "title");
                    string urlAddress = Uri.UnescapeDataString(hlib.stripHtmlParaData(tagPara, "href"));
                    string comment = tagData;
                    if (0 < comment.Length)
                        comment = "[" + comment + "]";
                    if (0 < title.Length)
                        dataList.Add(new WikiData(title, urlAddress, comment, "", "", ""));
                }
            } else if (filterTag.CompareTo("tr") == 0) {
                //  表形式
                List<WikiData> wikiData = getTableWikiData(tagData);
                if (0 < wikiData.Count)
                    dataList.AddRange(wikiData);
            } else {
                //  箇条書き、その他
                string title = hlib.stripHtmlParaData(tagData, "title");    //  タイトルのあるタグを検出
                if (0 < title.Length) {
                    string paraData = hlib.getHtmlTagParaDataTitle(tagData, "title");   //  検出したタイトルのタグを取得
                    string urlAddress = Uri.UnescapeDataString(hlib.stripHtmlParaData(paraData, "href"));
                    string comment = string.Join(" ", hlib.getHtmlTagDataAll(tagData));  //  表の行全体のコメントを抽出
                    if (0 < comment.Length)
                        comment = "[" + comment + "]";
                    dataList.Add(new WikiData(title, urlAddress, comment, "", "", ""));
                }
            }
        }

        /// <summary>
        /// 表形式<tr>のタグの処理
        /// 一行に複数の参照があるとき参照分のデータを作る
        /// </summary>
        /// <param name="html">HTMLソース(1行分のデータ)</param>
        /// <returns>List<WikiData></WikiData></returns>
        private List<WikiData> getTableWikiData(string html)
        {
            int pos = 0;
            int sp = 0;
            int ep = -1;
            string tagPara, tagData, nextSrc;
            html = html.Replace("\n", "");
            List<WikiData> wikiList = new List<WikiData> ();

            while (0 <= sp && sp != ep) {
                (tagPara, tagData, nextSrc, sp, ep) = hlib.getHtmlTagData(html, "td", pos); //  列データ
                if (tagData.Length == 0)
                    break;
                string caption = hlib.stripHtmlParaData(tagData, "title");  //  参照データ(<a)の有無
                if (0 < caption.Length) {
                    string title = hlib.getHtmlTagData(tagData, tagData.IndexOf("<a "));
                    string urlAddress = Uri.UnescapeDataString(hlib.stripHtmlParaData(tagData, "href"));
                    if (0 < title.Length)
                        wikiList.Add(new WikiData(title, urlAddress, "", "", ""));
                }
                pos = ep + 1;
            }
            if (0 < wikiList.Count) {
                string comment = string.Join(" ", hlib.getHtmlTagDataAll(html));
                if (0 < comment.Length)
                    comment = "[" + comment + "]";
                for (int i = 0; i < wikiList.Count; i++) {
                    wikiList[i].mComment = comment;
                }
            }
            return wikiList;
        }

        /// <summary>
        /// 検索終了位置を決める
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>終了位置</returns>
        private int limitDataPosition(string html)
        {
            int endPos = -1;
            foreach (string data in mStopTagData) {
                int p = html.LastIndexOf("id=\"" + data);
                if (endPos < 0) {
                    endPos = p;
                } else {
                    if (p < 0)
                        p = html.LastIndexOf("#" + data);
                    if (0 <= p)
                        endPos = Math.Min(endPos, p);
                }
            }
            int footerPos = html.LastIndexOf("<div class=\"printfooter");
            int navboxPos = html.LastIndexOf("<div class=\"navbox");
            if (endPos < 0) {
                if (0 < footerPos)
                    endPos = footerPos;
                if (0 < navboxPos)
                    endPos = endPos < navboxPos ? endPos : navboxPos;
            } else {
                if (0 < footerPos)
                    endPos = endPos < footerPos ? endPos : footerPos;
                if (0 < navboxPos)
                    endPos = endPos < navboxPos ? endPos : navboxPos;
            }
            return endPos;
        }

        /// <summary>
        /// リスト属性の追加
        /// </summary>
        /// <param name="listTitle">親リストタイトル</param>
        /// <param name="listUrl">親リストURL</param>
        /// <param name="searchForm">抽出方法</param>
        public void addData(string listTitle, string listUrl, SEARCHFORM searchForm)
        {
            for (int i = 0; i < mDataList.Count; i++) {
                mDataList[i].mListTitle = listTitle;
                mDataList[i].mListUrl = listUrl;
                mDataList[i].mSearchForm = mSearchFormTitle[(int)searchForm];
            }
        }

        /// <summary>
        /// URLの重複しているデータを削除する
        /// </summary>
        /// <returns>削除後のデータ</returns>
        public List<WikiData> removeDulicateUrl()
        {
            return removeDulicateUrl(mDataList);
        }

        /// <summary>
        ///  URLの重複しているデータを削除する
        /// </summary>
        /// <param name="srcList">元データ</param>
        /// <returns>削除後のデータ</returns>
        public List<WikiData> removeDulicateUrl(List<WikiData> srcList)
        {
            List<WikiData> destList = new List<WikiData>();
            for (int i = 0; i < srcList.Count; i++) {
                if (!destList.Exists(p => p.mUrl.CompareTo(srcList[i].mUrl) == 0))
                    destList.Add(srcList[i]);
            }
            return destList;
        }


        /// <summary>
        /// 検索ファイルから用語を検索しListに保存
        /// 大文字小文字の区別なし
        /// </summary>
        /// <param name="searchText">検索文字列</param>
        /// <param name="filePath">検索ファイル名</param>
        public void getSerchWikiDataFile(string searchText, string filePath)
        {
            List<string[]> dataList = ylib.loadCsvData(filePath);
            if (dataList != null && 0 < dataList.Count) {
                for (int i = 1; i < dataList.Count; i++) {
                    for (int j = 0; j < dataList[i].Length; j++) {
                        if (0 <= dataList[i][j].IndexOf(searchText, StringComparison.OrdinalIgnoreCase)) {
                            mDataList.Add(new WikiData(dataList[i]));
                            break;
                        }
                    }
                }
            }
        }
    }
}