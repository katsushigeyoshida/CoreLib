using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreLib
{
    /// <summary>
    /// DXFデータ単位(グループコードとデータ)
    /// </summary>
    public class DxfData
    {
        public int mGroupCode;
        public string mData;

        public DxfData(int code, string data) {
            mGroupCode = code;
            mData = data;
        }

        public bool compare(int code,string data)
        {
            if ( mGroupCode == code && mData == data) 
                return true;
            return false;
        }
    }

    /// <summary>
    /// DXFデータの要素単位データ
    /// </summary>
    public class DxfEntity
    {
        public string mEntityName;
        public int mColor = 256;
        public string mLineType = "CONTINUOUS";
        public double mThickness = 1.0;
        public List<(int code, int val)> mIntList = new List<(int code, int val)>();
        public List<(int code, double val)> mDoubleList = new List<(int code, double val)>();
        public List<(int code, string val)> mStringList = new List<(int code, string val)>();
        public List<PointD> mPoints = new List<PointD>();

        private YLib ylib = new YLib();

        public DxfEntity(string name)
        {
            mEntityName = name;
        }
        public string ToString()
        {
            string buf = $"{mEntityName},{mColor},{mLineType},{mThickness},";
            foreach ((int code, int val) data in mIntList) {
                buf += $"({data.code},{data.val}),";
            }
            foreach ((int code, double val) data in mDoubleList) {
                buf += $"({data.code},{data.val}),";
            }
            foreach ((int code, string val) data in mStringList) {
                buf += $"({data.code},{data.val}),";
            }
            foreach (var point in mPoints) {
                buf += point.ToString() + ",";
            }
            buf.TrimEnd(new char[] { ',' });
            return buf;
        }

        /// <summary>
        /// 線分データに変換
        /// </summary>
        /// <returns></returns>
        public LineD getLine()
        {
            if (1 < mPoints.Count)
                return new LineD(mPoints[0], mPoints[1]);
            else
                return null;
        }

        /// <summary>
        /// 円弧データに変換
        /// </summary>
        /// <returns></returns>
        public ArcD getArc()
        {
            ArcD arc = new ArcD();
            arc.mCp = mPoints[0];
            arc.mR = mDoubleList.Find(p => p.code == 40).val;
            if (mEntityName == "ARC" && 1 < mDoubleList.Count) {
                arc.mSa = ylib.D2R(mDoubleList.Find(p => p.code == 50).val);
                arc.mEa = ylib.D2R(mDoubleList.Find(p => p.code == 51).val);
            }
            return arc;
        }

        /// <summary>
        /// ポリラインデータに変換
        /// </summary>
        /// <returns></returns>
        public PolylineD getPolyline()
        {
            PolylineD polyline = new PolylineD();
            polyline.mPolyline = mPoints;
            if (mIntList.Find(p => p.code == 70).val == 1)
                polyline.mPolyline.Add(polyline.mPolyline[0]);
            return polyline;
        }

        /// <summary>
        /// テキストデータに変換
        /// </summary>
        /// <returns></returns>
        public TextD getText()
        {
            TextD text = new TextD();
            text.mText = textCnv(string.Join("", mStringList));
            text.mPos = mPoints[0];
            text.mTextSize = mDoubleList.Find(p => p.code == 40).val;
            text.mRotate = mDoubleList.Find(p => p.code == 50).val;
            if (mEntityName == "MTEXT") {
                int aliment = mIntList.Find(p => p.code == 71).val;
                int ha = aliment % 3;
                text.mHa = ha == 1 ? System.Windows.HorizontalAlignment.Left : ha == 2 ? System.Windows.HorizontalAlignment.Center : System.Windows.HorizontalAlignment.Right;
                int va = (aliment - 1) / 3;
                text.mVa = va == 0 ? System.Windows.VerticalAlignment.Top : va == 1 ? System.Windows.VerticalAlignment.Center : System.Windows.VerticalAlignment.Bottom;
            } else if (mEntityName == "TEXT") {
                int aliment = mIntList.Find(p => p.code == 72).val;
                int ha = aliment > 2 ? 0 : aliment;
                text.mHa = ha == 0 ? System.Windows.HorizontalAlignment.Left : ha == 1 ? System.Windows.HorizontalAlignment.Center : System.Windows.HorizontalAlignment.Right;
                int va = mIntList.Find(p => p.code == 73).val;
                text.mVa = va == 3 ? System.Windows.VerticalAlignment.Top : va == 2 ? System.Windows.VerticalAlignment.Center : System.Windows.VerticalAlignment.Bottom;
            }

            return text;
        }
        
        /// <summary>
        /// DXFデータのテキストで改行コード以外を除く(改行コードは変換 \P → \n)
        /// </summary>
        /// <param name="text">コントロールコード入り文字列</param>
        /// <returns>コントロールコード除外文字列</returns>
        private string textCnv(string text)
        {
            string buf = "";
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '{' || text[i] == '}') {

                } else if (text[i] == '\\') {
                    i++;
                    if (text[i] == '\\' || text[i] == '{' || text[i] == '}') {
                        buf += text[i];
                    } else if (text[i] == 'C' || text[i] == 'f' || text[i] == 'H' || text[i] == 'S' || text[i] == 'T' || text[i] == 'Q' || text[i] == 'W' || text[i] == 'A') {
                        while (text[i++] != ';' && i < text.Length) ;
                        buf += text[i];
                    } else if (text[i] == 'P') {
                        buf += '\n';
                    }
                } else {
                    buf += text[i];
                }
            }
            return buf;
        }
    }

    /// <summary>
    /// DXFデータ読込変換クラス
    /// </summary>
    public class DxfReader
    {
        private Encoding[] mEncoding = { Encoding.UTF8, Encoding.GetEncoding("shift_jis"), Encoding.GetEncoding("euc-jp") };
        private int mEncordingType = 1;     //  UTF8

        private List<DxfData> mDxfList = new List<DxfData>();       //  DXFのデータリスト
        public List<DxfEntity> mEntityList = new List<DxfEntity>(); //  要素データリスト

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">DXFファイルパス</param>
        public DxfReader(string path)
        {
            if (loadData(path)) {
                int i = 0;
                while (i < mDxfList.Count && !mDxfList[i].compare(0, "EOF")) {
                    if (mDxfList[i++].compare(0, "SECTION")) {
                        if (mDxfList[i].mGroupCode == 2) {
                            if (mDxfList[i].mData == "HEADER") {
                                i++;
                            } else if (mDxfList[i].mData == "TABLES") {
                                i++;
                            } else if (mDxfList[i].mData == "BLOCKS") {
                                i++;
                            } else if (mDxfList[i].mData == "ENTITIES") {
                                   readEntities(++i);
                            } else {
                                i++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ENTITIES ブロックの読込
        /// </summary>
        /// <param name="i">データ位置</param>
        private void readEntities(int i)
        {
            while (i < mDxfList.Count && !mDxfList[i].compare(0, "ENDSEC")) {
                if (mDxfList[i].mGroupCode == 0) {
                    System.Diagnostics.Debug.WriteLine($"{i} {mDxfList[i].mData}");
                    DxfEntity dxfEntity = null;
                    dxfEntity = getEntity(i, mDxfList[i].mData);
                    if (dxfEntity != null)
                        mEntityList.Add(dxfEntity);
                }
                i++;
            }
        }

        /// <summary>
        /// 要素パラメータの取得
        /// </summary>
        /// <param name="i"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private DxfEntity getEntity(int i, string name)
        {
            DxfEntity dxfEntity = new DxfEntity(name);
            i++;
            PointD point = new PointD();
            while (i < mDxfList.Count && mDxfList[i].mGroupCode != 0) {
                switch (mDxfList[i].mGroupCode) {
                    case 8:             //  画層名 LAYER
                        break;
                    case 6:             //  線種名 FONT
                        dxfEntity.mLineType = mDxfList[i].mData;
                        break;
                    case 62:            //  色番号
                        dxfEntity.mColor = ylib.string2int(mDxfList[i].mData);
                        break;
                    case 10:            //  X座標
                        point = new PointD();
                        point.x = ylib.string2double(mDxfList[i].mData);
                        break;
                    case 20:            //  Y座標
                        point.y = ylib.string2double(mDxfList[i].mData);
                        dxfEntity.mPoints.Add(point);
                        break;
                    case 30:            //  Y座標
                        //point.z = ylib.string2double(mDxfList[i].mData);
                        //dxfEntity.mPoints.Add(point);
                        break;
                    case 11:            //  X座標
                        point = new PointD();
                        point.x = ylib.string2double(mDxfList[i].mData);
                        break;
                    case 21:            //  Y座標
                        point.y = ylib.string2double(mDxfList[i].mData);
                        dxfEntity.mPoints.Add(point);
                        break;
                    case 31:            //  Z座標
                        //point.z = ylib.string2double(mDxfList[i].mData);
                        //dxfEntity.mPoints.Add(point);
                        break;
                    case 39:
                    case 40:            //  円弧の半径/文字高さ
                    case 41:            //  
                    case 50:            //  開始角度
                    case 51:            //  終了角度
                        dxfEntity.mDoubleList.Add((mDxfList[i].mGroupCode, ylib.string2double(mDxfList[i].mData)));
                        break;
                    case 70:
                    case 71:            //  アライメント
                    case 72:            //  水平アライメント
                    case 73:            //  垂直アライメント
                    case 74:
                    case 75:
                    case 76:
                        dxfEntity.mIntList.Add((mDxfList[i].mGroupCode, ylib.string2int(mDxfList[i].mData)));
                        break;
                    case 1:             //  文字列
                    case 2:
                    case 3:             //  追加文字列
                        dxfEntity.mStringList.Add((mDxfList[i].mGroupCode, mDxfList[i].mData));
                        break;
                }
                i++;
            }
            return dxfEntity;
        }


        /// <summary>
        /// データをファイルから取り込む
        /// </summary>
        /// <param name="filePath">GPXファイル名</param>
        /// <returns>データ文字列</returns>
        private bool loadData(string filePath)
        {
            if (File.Exists(filePath)) {
                using (System.IO.StreamReader dataFile = new StreamReader(filePath, mEncoding[mEncordingType])) {
                    string buf;
                    while ((buf = dataFile.ReadLine()) != null) {  //  行単位読込
                        int code = Convert.ToInt32(buf);
                        if ((buf = dataFile.ReadLine()) != null) {
                            DxfData data = new DxfData(code, buf);
                            mDxfList.Add(data);
                        } else
                            break;
                    }
                    //      dataFile.Close(); //  usingの場合は不要 Disposeを含んでいる
                }
                return true;
            }
            return false;
        }
    }
}
