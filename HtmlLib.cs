using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoreLib
{
    /// <summary>
    /// HTML関係のライブラリ
    /// 
    /// List<string> getPattern(string html, string pattern, string group)      正規表現を使ったHTMLデータからパターン抽出
    /// List<string[]> getPattern(string html, string pattern)                  正規表現を使ったHTMLからのパターン抽出
    /// List<string[]> getPattern2(string html, string pattern)                 正規表現を使ったHTMLからのパターン抽出
    /// List<string> getHtmlTagList(string html)                    HTMLソースからTAGデータごとに分解したリストを作る
    /// int getHtmlTagType(string tagData)                          タグデータの種類を判別する
    /// string getHtmlTagName(string tagData)                       '<','>'で囲まれたタグデータからタグ名を抽出する
    /// List<string> getHtmlTagDataAll(string html, int pos = 0)    HTMLソースからデータのみを抽出
    /// string getHtmlTagData(string html, int pos = 0)             HTMLソースから最初のデータ部を抽出(TAGは含まない)
    /// (string, string, string) getHtmlTagData(string html, string tag)    TAGデータの抽出(入れ子対応)
    /// (string, string, string, int, int) getHtmlTagData(string html, string tag, int pos)     TAGデータの抽出(入れ子対応)
    /// (int, int) getHtmlTagDataPos(string html, string tag, int pos = 0)      TAGの開始位置と終了位置の検索(TAGを含む)
    /// string stripHtmlTagData(string html, string tag)            HTMLソースからタグで囲まれた領域を除く
    /// string getHtmlTagPara(string tagData, string paraName)      '<','>'で囲まれたタグデータからパラメータを抽出する
    /// string getHtmlTagParaDataTitle(string tagData, string paraTitle, string paraData = "")      パラメータのタイトルでタグ全体を取得する
    /// string stripHtmlParaData(string para, string paraTitle)     HTMLのタグパラメータからデータだけを取り出す
    /// int findHtmlParaDataTagPos(string para, string paraTitle)   HTMLのデータからタグパラメータの存在するタグの開始位置を取得する
    /// (int, string) findHtmlTagPos(string html, string tagName, int pos = 0)      HTMLソースからタグを検索し開始位置を求める
    /// (int tagType, int start, int end) findHtmlTag(string html, string tagName, int pos = 0)     タグを検索して種別と位置を求める
    /// string cnvHtmlSpecialCode(string html)      HTMLで使われいる{&#??;]のコードを通常の文字に置換える
    /// 
    /// 
    /// </summary>
    public class HtmlLib
    {
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
        /// HTMLソースからTAGデータごとに分解したリストを作る
        /// <tag para....>data</tag>
        /// 1.<tag para....>
        /// 2.data
        /// 3.</tag>tag>
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>TAGリスト</returns>
        public List<string> getHtmlTagList(string html)
        {
            List<string> tagList = new List<string>();
            char[] trimChar = { '\n', '\a', '\r', ' ', '\t' };
            int pos = 0;
            string data = "";
            int ep = html.IndexOf('>');
            int sp = html.IndexOf('<');
            if ((0 <= ep && 0 <= sp && ep < sp) || (0 <= ep && sp < 0)) {
                data = html.Substring(0, ep + 1).Trim(trimChar);
                if (0 < data.Length)
                    tagList.Add(data);
                pos = ep + 1;
            }
            while (pos < html.Length) {
                int st = html.IndexOf('<', pos);
                if (pos < st) {
                    data = html.Substring(pos, st - pos);
                    pos = st;
                } else if (pos == st) {
                    int ct = html.IndexOf('>', st);
                    if (0 <= ct) {
                        data = html.Substring(st, ct - st + 1);
                        pos = ct + 1;
                    } else {
                        data = html.Substring(st);
                        pos = html.Length;
                    }
                } else {
                    data = html.Substring(pos);
                    pos = html.Length;
                }
                string tag = data.Trim(trimChar);
                if (0 < tag.Length)
                    tagList.Add(tag);
            }
            return tagList;
        }

        /// <summary>
        ///  タグデータの種類を判別する
        ///  1: <TAG ..../>
        ///  2: <TAG ....>
        ///  3: </TAG>
        ///  4: <!....>
        ///  5: <!-- ....
        ///  6: ...-->
        ///  7: ...DATA...
        ///  0: ?
        /// </summary>
        /// <param name="tagData">タグデータ</param>
        /// <returns>タグの種類</returns>
        public int getHtmlTagType(string tagData)
        {
            int st = tagData.IndexOf('<');
            int et = tagData.IndexOf('>');
            if (st < 0 && et < 0) {
                return 7;                       //  データ(タグがない)
            } else if (st < 0 && 0 <= et) {
                if (0 <= tagData.IndexOf("-->"))
                    return 6;                   //  コメント終端(-->)
                else
                    return 0;                   //  不明(...>)
            } else if (0 <= st && et < 0) {
                if (0 <= tagData.IndexOf("<!--"))
                    return 5;                   //  コメント開始(<!--)
                else
                    return 0;                   //  不明(<...)
            } else if (0 <= st && 0 <= et) {
                if (0 <= tagData.IndexOf("/>"))
                    return 1;                   //  タグ(</TAG .../>)
                if (0 <= tagData.IndexOf("</"))
                    return 3;                   //  終端タグ(</TAG>
                if (0 <= tagData.IndexOf("<!"))
                    return 4;                   //  コメント(<!...>)
                return 2;                       //  開始タグ(<TAG ...>)
            } else
                return 0;                       //  不明
        }

        /// <summary>
        /// '<','>'で囲まれたタグデータからタグ名を抽出する
        /// </summary>
        /// <param name="tagData">タグデータ</param>
        /// <returns>タグ名</returns>
        public string getHtmlTagName(string tagData)
        {
            int st = tagData.IndexOf('<');
            if (st < 0)
                return "";                          //  タグがない
            int et = tagData.IndexOf(' ', st);
            if (et < 0)
                et = tagData.IndexOf("/>", st);
            if (et < 0)
                et = tagData.IndexOf(">", st);
            if (et < 0)
                return tagData.Substring(st + 1);   //  終端がない時のタグ名
            else
                return tagData.Substring(st + 1, et - st - 1);  //  タグ名
        }

        /// <summary>
        /// HTMLソースからデータのみを抽出
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="pos">抽出データList</param>
        /// <returns>タグリスト</returns>
        public List<string> getHtmlTagDataAll(string html, int pos = 0)
        {
            List<string> tagDataList = getHtmlTagList(html.Substring(pos));
            List<string> dataList = new List<string>();
            foreach (string data in tagDataList) {
                if (data.IndexOf('<') < 0 && data.IndexOf('>') < 0)
                    dataList.Add(cnvHtmlSpecialCode(data));
            }
            return dataList;
        }

        /// <summary>
        /// HTMLソースから最初のデータ部を抽出(TAGは含まない)
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>データ文字列</returns>
        public string getHtmlTagData(string html, int pos = 0)
        {
            if (pos < 0)
                return "";
            int sp = html.IndexOf('>', pos);
            if (0 <= sp) {
                int ep = html.IndexOf('<', sp);
                if (0 <= ep)
                    return html.Substring(sp + 1, ep - sp - 1);
                else {
                    ep = html.IndexOf('<');
                    if (0 < ep)
                        return html.Substring(0, ep - 1);
                    else
                        return html.Substring(sp + 1);
                }
            } else {
                int ep = html.IndexOf('<');
                if (0 < ep)
                    return html.Substring(0, ep - 1);
                else
                    return html;
            }
        }

        /// <summary>
        /// TAGデータの抽出(入れ子対応)
        /// <TAG ...>data...</TAG>のdata部分を抽出、dataの中にタグが入れ子構造にも対応
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tag">タグ名</param>
        /// <returns>(タグパラメータ, 抽出データ, 残りHTML)</returns>
        public (string, string, string) getHtmlTagData(string html, string tag)
        {
            (string tagPara, string tagData, string nextSrc, int start, int end) = getHtmlTagData(html, tag, 0);
            return (tagPara, tagData, nextSrc);
        }

        /// <summary>
        /// TAGデータの抽出(入れ子対応)
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tag">対象タグ名</param>
        /// <param name="pos">検索開始位置</param>
        /// <returns>(タグパラメータ, 抽出データ, 残りHTML, タグ開始位置, タグ終了位置)</returns>
        public (string, string, string, int, int) getHtmlTagData(string html, string tag, int pos)
        {
            int startPos = pos;                 //  TAGの開始位置(TAGを含む)
            int endPos = pos;                   //  TAGの終了位置(TAGを含む)
            int count = 0;                      //  ネストの数
            int st, tagType, start, end;        //  st, et, 検索したタグの種別,開始位置,終了位置
            string tagPara = "";                //  タグのパラメータ(初回検索タグ)
            st = pos;                           //  タグデータの開始位置
            do {
                (tagType, start, end) = findHtmlTag(html, tag, pos);
                switch (tagType) {
                    case 1:
                        break;                 //  完結タグ
                    case 2:
                        count++;
                        break;        //  開始タグ
                    case 3:
                        count--;
                        break;        //  終了タグ
                    case 0:
                        return ("", "", html, startPos, endPos); //  不明タグでreturn
                }
                if (st == pos) {
                    //  初回設定
                    startPos = start;
                    st = end + 1;
                    if (html[start + tag.Length + 1] == ' ') {
                        tagPara = html.Substring(start + tag.Length + 2,
                                end - (tagType == 1 ? 2 : 1) - (start + tag.Length + 1));
                    }
                }
                pos = end;
            } while (0 < count);
            start = start <= st ? end + 1 : start;

            return (tagPara, html.Substring(st, start - st), html.Substring(end + 1), startPos, end);
        }

        /// <summary>
        /// TAGの開始位置と終了位置の検索(TAGを含む)
        /// TAGが見つからない場合 開始位置 >= 終了位置
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tag">タグ名(<>は含まない)</param>
        /// <param name="pos">検索開始位置</param>
        /// <returns>(開始位置,終了位置)</returns>
        public (int, int) getHtmlTagDataPos(string html, string tag, int pos = 0)
        {
            int startPos = pos;
            int endPos = pos;
            int count = 0;                      //  ネストの数
            int st, tagType, start, end;        //  st, et, 検索したタグの種別,開始位置,終了位置
            st = pos;                           //  タグデータの開始位置
            do {
                (tagType, start, end) = findHtmlTag(html, tag, pos);
                switch (tagType) {
                    case 1:
                        break;                 //  完結タグ
                    case 2:
                        count++;
                        break;        //  開始タグ
                    case 3:
                        count--;
                        break;        //  終了タグ
                    case 0:
                        return (startPos, endPos); //  不明タグでreturn  (検索開始位置を返す)
                }
                if (st == pos) {
                    startPos = start;
                    st = end + 1;
                }
                pos = end;
            } while (0 < count);

            return (startPos, end);
        }

        /// <summary>
        /// HTMLソースからタグで囲まれた領域を除く
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tag">タグ名(<>は含まない)</param>
        /// <returns>取り除かれたHTMLソース</returns>
        public string stripHtmlTagData(string html, string tag)
        {
            string buffer = "";
            int pos = html.IndexOf("<" + tag);
            if (pos < 0)
                return html;
            buffer = html.Substring(0, pos);
            (string tagPara, string tagData, string nextSrc) = getHtmlTagData(html.Substring(pos), tag);
            buffer += stripHtmlTagData(nextSrc, tag);
            return buffer;
        }

        /// <summary>
        /// '<','>'で囲まれたタグデータからパラメータを抽出する
        /// <a href="...." title="TITLE">
        /// data = getHtmlTagPara(tagData, "title");
        /// </summary>
        /// <param name="tagData">タグデータ</param>
        /// <param name="paraName">パラメータ名</param>
        /// <returns>パラメータデータ</returns>
        public string getHtmlTagPara(string tagData, string paraName)
        {
            int st = tagData.IndexOf('<');
            if (st < 0)
                return "";
            st = tagData.IndexOf(paraName, st);
            if (0 <= st) {
                st = tagData.IndexOf('\"', st);
                if (0 <= st) {
                    int et = tagData.IndexOf('\"', st + 1);
                    if (0 < et)
                        return tagData.Substring(st + 1, et - st - 1);
                }
            }
            return "";
        }

        /// <summary>
        /// パラメータのタイトルでタグ全体を取得する
        /// パラメータのタイトルのデータを指定したい場合には対象データのあるタグデータを検索する
        /// </summary>
        /// <param name="tagData">HTMLソース</param>
        /// <param name="paraTitle">パラメータタイトル</param>
        /// <param name="paraData">パラメータタイトルのデータ(省略可)</param>
        /// <returns>'<''>'で囲まれたタグデータ</returns>
        public string getHtmlTagParaDataTitle(string tagData, string paraTitle, string paraData = "")
        {
            int s = 0;
            int m, n, l;
            do {
                m = tagData.IndexOf(paraTitle + "=\"", s);
                if (m < 0)
                    return "";
                if (paraData.Length == 0)
                    break;
                s = m + 1;
                n = tagData.IndexOf("\"", m);
                l = tagData.IndexOf("\"", n + 1);
            } while (tagData.Substring(n + 1, l - n - 1).CompareTo(paraData) != 0);

            int sp = tagData.LastIndexOf('<', m);
            int ep = tagData.IndexOf('>', m);
            if (0 <= sp && 0 <= ep)
                return tagData.Substring(sp, ep - sp + 1);
            return "";
        }

        /// <summary>
        /// HTMLのタグパラメータからデータだけを取り出す
        /// 例: html = "<a herf="/wiki/para" title="タイトル"> </a>";
        ///     (string para, string tdata, string next) = ylib.getHtmlTagData(html, "a");
        ///         [para = "herf=\"/wiki/para\" title=\"タイトル\"";]
        ///     data = stripParaData(para, "title");
        /// </summary>
        /// <param name="para">タグパラメータ</param>
        /// <param name="paraTitle">パラメータの種別</param>
        /// <returns>パラメータのデータ</returns>
        public string stripHtmlParaData(string para, string paraTitle)
        {
            int m = para.IndexOf(paraTitle + "=\"");
            if (m < 0)
                return "";
            int n = para.IndexOf('\"', m);
            if (n < 0)
                return "";
            int o = para.IndexOf('\"', n + 1);
            if (o < 0)
                return para.Substring(o + 1);
            return para.Substring(n + 1, o - 1 - n);
        }

        /// <summary>
        /// HTMLのデータからタグパラメータの存在するタグの開始位置を取得する
        /// </summary>
        /// <param name="para">HTMLデータ</param>
        /// <param name="paraTitle">タグパラメータ名</param>
        /// <returns>タグ開始位置</returns>
        public int findHtmlParaDataTagPos(string para, string paraTitle)
        {
            int m = para.IndexOf(paraTitle + "=\"");
            if (m < 0)
                return 0;
            return para.LastIndexOf('<', m);
        }

        /// <summary>
        /// HTMLソースからタグを検索し開始位置を求める
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tagName">タグ名</param>
        /// <param name="pos">検索開始位置</param>
        /// <returns>(検索位置, パラメータ)</returns>
        public (int, string) findHtmlTagPos(string html, string tagName, int pos = 0)
        {
            pos = pos < 0 ? 0 : pos;
            string startTag = "<" + tagName + '>';
            string startTag2 = "<" + tagName + ' ';
            int sp = html.IndexOf(startTag, pos);
            if (0 <= sp)
                return (sp, "");
            int sp2 = html.IndexOf(startTag2, pos);
            if (0 <= sp2) {
                int ep = html.IndexOf("/>", sp2);
                if (0 <= ep)
                    return (sp2, html.Substring(sp2 + startTag2.Length, ep - (sp2 + startTag2.Length)));
                ep = html.IndexOf(">", sp2);
                if (0 <= ep)
                    return (sp2, html.Substring(sp2 + startTag2.Length, ep - (sp2 + startTag2.Length)));
            }
            return (-1, "");
        }

        /// <summary>
        /// タグを検索して種別と位置を求める
        /// 種別  1: <TAG ..../>  完結タグ
        ///       2: <TAG ...>    開始タグ
        ///       3: </TAG>       終了タグ
        ///       0: 不明         タグが見つからない
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <param name="tagName">タグ名(</>は含まない)</param>
        /// <param name="pos">検索開始位置</param>
        /// <returns>(種別, タグの起点, タグの終点)</returns>
        public (int tagType, int start, int end) findHtmlTag(string html, string tagName, int pos = 0)
        {
            pos = pos < 0 ? 0 : pos;
            string startTag = "<" + tagName + '>';
            string startTag2 = "<" + tagName + ' ';
            string endTag = "</" + tagName + ">";
            int sp = html.IndexOf(startTag, pos);
            int sp2 = html.IndexOf(startTag2, pos);
            sp = (0 <= sp && 0 <= sp2) ? Math.Min(sp, sp2) : (0 <= sp ? sp : (0 <= sp2 ? sp2 : -1));    //  タグの開始位置
            int sep = html.IndexOf('>', sp < 0 ? pos : sp);     //  タグの終了位置
            int seep = html.IndexOf("/>", sp < 0 ? pos : sp);   //  タグの終了位置
            int ep = html.IndexOf(endTag, pos);                 //  終了タグの開始位置
            if (0 <= sp) {
                if ((0 <= ep && sp < ep) || ep < 0) {
                    if (0 <= sep && seep == sep - 1)
                        return (1, sp, sep);
                    else if (0 <= sep && seep != sep - 1)
                        return (2, sp, sep < 0 ? ep : sep);
                }
                if (0 <= ep && ep < sp) {
                    sep = html.IndexOf('>', ep);
                    return (3, ep, sep < 0 ? ep : sep);
                }
            } else if (0 <= ep) {
                sep = html.IndexOf('>', ep);
                return (3, ep, sep < 0 ? ep : sep);
            }
            sp = sp < 0 ? pos : sp;
            ep = ep < 0 ? pos : ep;
            return (0, ep, sep < 0 ? ep : sep);
        }

        /// <summary>
        /// HTMLで使われいる{&#??;]のコードを通常の文字に置換える
        /// {&#???;] 文字を10進で表示  [&#x???;] 文字を16進で表示
        /// &xxx; の文字変換もおこなう
        /// </summary>
        /// <param name="html">HTMLソース</param>
        /// <returns>変換データ</returns>
        public string cnvHtmlSpecialCode(string html)
        {
            html = html.Replace("&lt;", "<");
            html = html.Replace("&gt;", ">");
            html = html.Replace("&amp;", "&");
            html = html.Replace("&quot;", "\"");
            html = html.Replace("&nbsp;", " ");

            int m = html.IndexOf("&#");
            int n = 0;
            if (0 <= m)
                n = html.IndexOf(";", m);
            char code;
            while (0 <= m && 0 <= n) {
                string specialCode = html.Substring(m, n - m);
                if (specialCode[2] == 'x')
                    code = (char)Convert.ToInt32(specialCode.Substring(3), 16);
                else
                    code = (char)int.Parse(specialCode.Substring(2));
                html = html.Replace(specialCode + ";", code.ToString());
                m = html.IndexOf("&#");
                if (0 <= m)
                    n = html.IndexOf(";", m);
            }
            return html;
        }
    }
}
