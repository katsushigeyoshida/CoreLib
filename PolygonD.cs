using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    /// <summary>
    /// ポリゴン クラス
    /// 
    /// コンストラクタ
    /// PolygonD()
    /// PolygonD(List<PointD> plist)
    /// PolygonD(PolygonD polygon)
    /// 
    /// void Add(PointD p)                              座標点の追加
    /// void Inset(int index, PointD p)                 座標点の挿入
    /// override string ToString()
    /// string ToString(string form)                    座標を書式付きで文字列に変換
    /// PolygonD toCopy()                               コピーを作成
    /// List<PointD> toPointList()                      座標点リストに変換
    /// List<LineD> toLineList()                        線分リストに変換
    /// double length()                                 全体の長さ
    /// Box getBox()                                    ポリゴンの領域を求める
    /// LineD getLine(int n)                            指定位置のポリラインの線分を取り出す
    /// void translate(PointD vec)                      全体を移動する
    /// void rotate(double ang)                         原点を中心に全体を回転する
    /// void rotate(PointD cp, double ang)              指定点を中心に全体を回転
    /// void rotate(PointD cp, PointD rp)               指定点を中心に全体を回転
    /// void mirror(PointD sp, PointD ep)               指定の線分に対してミラーする
    /// void mirror(LineD line)                         指定の線分に対してミラーする
    /// void offset(double d)                           オフセットする
    /// void offset(PointD sp, PointD ep)               垂直方向に平行移動させる
    /// void scale(PointD cp, double scale)             原点を指定して拡大縮小
    /// void stretch(PointD vec, PointD nearPos)        要素の指定位置に近い座標を移動させる
    /// PolylineD divide(PointD dp)                     要素を分割するしたポリラインを作成
    /// List<PointD> intersection(PointD p)             点との交点(垂点)リストを求める
    /// List<PointD> intersection(LineD l)              線分との交点リストを求める
    /// List<PointD> intersection(ArcD arc)             円弧との交点の座標リストを求める
    /// List<PointD> intersection(PolylineD polyline)   ポリラインとの交点の座標リストを求める
    /// List<PointD> intersection(PolygonD polygon)     ポリゴンとの交点の座標リストを求める
    /// LineD nearLine(PointD p)                        最も近い線分を求める
    /// PointD nearPoint(PointD p)                      交点の中で最も近い点を求める
    /// int nearPos(PointD p)                           交点の中で最も近い点の線分位置を求める
    /// ointD nearPeackPoint(PointD p)                  頂点の中で最も近い点の座標
    /// int nearPeackPos(PointD p)                      頂点の中で最も近い点の位置
    /// 
    /// </summary>
    public class PolygonD
    {
        public List<PointD> mPolygon;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PolygonD()
        {
            mPolygon = new List<PointD>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plist">座標リスト</param>
        public PolygonD(List<PointD> plist)
        {
            mPolygon = plist.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polygon">PolygonD</param>
        public PolygonD(PolygonD polygon)
        {
            mPolygon = polygon.mPolygon.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// 座標点の追加
        /// </summary>
        /// <param name="p">座標</param>
        public void Add(PointD p)
        {
            mPolygon.Add(p);
        }

        /// <summary>
        /// 座標点の挿入
        /// </summary>
        /// <param name="index">挿入位置</param>
        /// <param name="p">座標</param>
        public void Inset(int index, PointD p)
        {
            mPolygon.Insert(index, p);
        }

        /// <summary>
        /// 座標を文字列に変換
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            string buf = "";
            mPolygon.ForEach(p => buf += p.ToString());
            return buf;
        }

        /// <summary>
        /// 座標を書式付きで文字列に変換
        /// </summary>
        /// <param name="form">書式</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            string buf = "";
            mPolygon.ForEach(p => buf += p.ToString(form));
            return buf;
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>PolygonD</returns>
        public PolygonD toCopy()
        {
            return new PolygonD(mPolygon);
        }

        /// <summary>
        /// 座標点リストに変換
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<PointD> toPointList()
        {
            return mPolygon.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// 線分リストに変換
        /// </summary>
        /// <returns>線分リスト</returns>
        public List<LineD> toLineList()
        {
            List<LineD> llist = new List<LineD>();
            for (int i = 0; i < mPolygon.Count; i++) {
                LineD line = new LineD(mPolygon[i], mPolygon[i + 1 < mPolygon.Count ? i + 1 : 0]);
                llist.Add(line);
            }
            return llist;
        }

        /// <summary>
        /// 全体の長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            double length = mPolygon[0].length(mPolygon[mPolygon.Count - 1]);
            for (int i = 0; i < mPolygon.Count - 1; i++)
                length += mPolygon[i].length(mPolygon[i + 1]);
            return length;
        }

        /// <summary>
        /// ポリゴンの領域を求める
        /// </summary>
        /// <returns>Box領域</returns>
        public Box getBox()
        {
            if (mPolygon == null || mPolygon.Count == 0)
                return null;
            Box b = new Box(mPolygon[0]);
            mPolygon.ForEach(p => b.extension(p));
            return b;
        }

        /// <summary>
        /// 指定位置のポリラインの線分を取り出す
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>線分</returns>
        public LineD getLine(int n)
        {
            if (mPolygon != null && 1 < mPolygon.Count && 0 <= n && n < mPolygon.Count)
                return new LineD(mPolygon[n], mPolygon[n == mPolygon.Count - 1 ? 0 : n + 1]);
            else
                return null;
        }

        /// <summary>
        /// 指定位置に最も近い線分を取り出す
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>線分</returns>
        public LineD getLine(PointD p)
        {
            int np = nearPos(p);
            return getLine(np);
        }

        /// <summary>
        /// 全体を移動する
        /// </summary>
        /// <param name="vec">オフセット値(移動量)</param>
        public void translate(PointD vec)
        {
            mPolygon.ForEach(p => p.offset(vec));
        }

        /// <summary>
        /// 原点を中心に全体を回転する
        /// </summary>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(double ang)
        {
            mPolygon.ForEach(p => p.rotate(ang));
        }

        /// <summary>
        /// 指定点を中心に全体を回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(PointD cp, double ang)
        {
            mPolygon.ForEach(p => p.rotate(cp, ang));
        }

        /// <summary>
        /// 指定点を中心に全体を回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="rp">回転位置</param>
        public void rotate(PointD cp, PointD rp)
        {
            mPolygon.ForEach(p => p.rotate(cp, rp));
        }

        /// <summary>
        /// 指定の線分に対してミラーする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(PointD sp, PointD ep)
        {
            mPolygon.ForEach(p => p.mirror(sp, ep));
        }

        /// <summary>
        /// 指定の線分に対してミラーする
        /// </summary>
        /// <param name="line">線分</param>
        public void mirror(LineD line)
        {
            mPolygon.ForEach(p => p.mirror(line.ps, line.pe));
        }

        /// <summary>
        /// オフセットする
        /// </summary>
        /// <param name="d">オフセット距離</param>
        public void offset(double d)
        {
            List<LineD> llist = toLineList();
            llist.ForEach(l => l.offset(d));
            if (1 < llist.Count) {
                mPolygon.Clear();
                PointD ip = llist[llist.Count - 1].intersection(llist[0]);
                mPolygon.Add(ip);
                for (int i = 0; i < llist.Count - 1; i++) {
                    ip = llist[i].intersection(llist[i + 1]);
                    mPolygon.Add(ip);
                }
            }
        }

        /// <summary>
        /// 垂直方向に平行移動させる
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(PointD sp, PointD ep)
        {
            LineD line = getLine(sp);
            double dis = line.distance(ep) * Math.Sign(line.crossProduct(ep)) - line.distance(sp) * Math.Sign(line.crossProduct(sp));
            offset(dis);
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点</param>
        /// <param name="scale">拡大率</param>
        public void scale(PointD cp, double scale)
        {
            mPolygon.ForEach(p => p.scale(cp, scale));
        }

        /// <summary>
        /// 要素の指定位置に近い座標を移動させる
        /// </summary>
        /// <param name="vec">移動量</param>
        /// <param name="nearPos">指定位置</param>
        public void stretch(PointD vec, PointD nearPos)
        {
            int pos = nearPeackPos(nearPos);
            if (0 <= pos)
                mPolygon[pos].translate(vec);
        }

        /// <summary>
        /// 要素を分割するしたポリラインを作成
        /// </summary>
        /// <param name="dp">分割点</param>
        /// <returns>ポリライン</returns>
        public PolylineD divide(PointD dp)
        {
            PolylineD polyline = new PolylineD();
            PointD mp = nearPoint(dp);
            if (mp == null)
                return polyline;
            int pos = nearPos(mp);
            polyline.Add(mp);
            if (pos + 1 < mPolygon.Count)
                polyline.mPolyline.AddRange(mPolygon.Skip(pos + 1));
            polyline.mPolyline.AddRange(mPolygon.Take(pos + 1));
            polyline.Add(mp);
            return polyline;
        }

        /// <summary>
        /// 点との交点(垂点)リストを求める
        /// </summary>
        /// <param name="p"><点座標/param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PointD p)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = toLineList();
            foreach (LineD line in llist) {
                PointD ip = line.intersection(p);
                if (ip != null && line.onPoint(ip))
                    plist.Add(ip);
            }
            return plist;
        }

        /// <summary>
        /// 線分との交点リストを求める
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(LineD l)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = toLineList();
            foreach (LineD line in llist) {
                PointD ip = line.intersection(l);
                if (ip != null && line.onPoint(ip))
                    plist.Add(ip);
            }
            return plist;
        }

        /// <summary>
        /// 交点の座標リストを求める
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns></returns>
        public List<PointD> intersection(ArcD arc)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = toLineList();
            foreach (LineD line in llist) {
                plist.AddRange(arc.intersection(line));
            }
            return plist;
        }

        /// <summary>
        /// 交点の座標リストを求める
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <returns></returns>
        public List<PointD> intersection(PolylineD polyline)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = toLineList();
            foreach (LineD line in llist) {
                plist.AddRange(polyline.intersection(line));
            }
            return plist;
        }

        /// <summary>
        /// 交点の座標リストを求める
        /// </summary>
        /// <param name="polygon">ポリライン</param>
        /// <returns></returns>
        public List<PointD> intersection(PolygonD polygon)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = toLineList();
            foreach (LineD line in llist) {
                plist.AddRange(polygon.intersection(line));
            }
            return plist;
        }

        /// <summary>
        /// 最も近い線分を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>線分</returns>
        public LineD nearLine(PointD p)
        {
            int np = nearPos(p);
            return getLine(np);
        }

        /// <summary>
        /// 交点の中で最も近い点を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>交点</returns>
        public PointD nearPoint(PointD p)
        {
            List<PointD> plist = intersection(p);
            if (plist != null && 0 < plist.Count) {
                double l = double.MaxValue;
                PointD np = plist[0];
                foreach (PointD ip in plist) {
                    double lt = ip.length(p);
                    if (lt < l) {
                        l = lt;
                        np = ip;
                    }
                }
                return np;
            }
            return null;
        }

        /// <summary>
        /// 交点の中で最も近い点の線分位置を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>線分位置</returns>
        public int nearPos(PointD p)
        {
            List<LineD> llist = toLineList();
            double length = double.MaxValue;
            int nearNo = 0;
            for (int i = 0; i  < llist.Count; i++) {
                PointD ip = llist[i].intersection(p);
                double len = ip.length(p);
                if (len < length && ip != llist[i].pe) {
                    length = len;
                    nearNo = i;
                }
            }
            return nearNo;
        }

        /// <summary>
        /// 頂点の中で最も近い点の座標
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>座標</returns>
        public PointD nearPeackPoint(PointD p)
        {
            if (mPolygon != null && 0 < mPolygon.Count) {
                double l = double.MaxValue;
                PointD np = mPolygon[0];
                foreach (PointD ip in mPolygon) {
                    double lt = ip.length(p);
                    if (lt < l) {
                        l = lt;
                        np = ip;
                    }
                }
                return np;
            }
            return null;
        }

        /// <summary>
        /// 頂点の中で最も近い点の位置
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>頂点の位置</returns>
        public int nearPeackPos(PointD p)
        {
            int nearNo = -1;
            if (mPolygon != null && 0 < mPolygon.Count) {
                double l = double.MaxValue;
                for (int i = 0; i < mPolygon.Count; i++) {
                    double lt = mPolygon[i].length(p);
                    if (lt < l) {
                        l = lt;
                        nearNo = i;
                    }
                }
            }
            return nearNo;
        }
    }
}
