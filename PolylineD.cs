using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// ポリライン クラス
    /// 
    /// コンストラクタ
    /// PolylineD()
    /// PolylineD(List<PointD> polyline)
    /// PolylineD(PolylineD polyline)
    /// 
    /// void Add(PointD p)                              座標の追加
    /// void Insert(int index, PointD p)                座標の挿入
    /// override string ToString()
    /// string ToString(string form)                    座標を書式付きで文字列に変換
    /// PolylineD toCopy()                              コピーを作成
    /// List<PointD> toPointList()                      座標点リストに変換
    /// List<LineD> toLineList()                        線分リストに変換
    /// double length()                                 全体の長さを求める
    /// Box getBox()                                    Box領域を求める
    /// LineD getLine(int n)                            指定位置のポリラインの線分を取り出す
    /// LineD getLine(PointD p)                         指定座標に近い線分を取り出す
    /// void offset(double d)                           オフセットする
    /// void offset(PointD sp, PointD ep)               垂直方向に平行移動させる
    /// void translate(PointD vec)                      全体を移動する
    /// void rotate(double ang)                         原点を中心に全体を回転する
    /// void rotate(PointD cp, double ang)              指定点を中心に回転する
    /// void rotate(PointD cp, PointD mp)               指定点を中心に回転する
    /// void mirror(PointD sp, PointD ep)               指定線分でミラーする
    /// void mirror(LineD l)                            指定線分でミラーする
    /// void scale(PointD cp, double scale)             原点を指定して拡大縮小
    /// void trim(PointD sp, PointD ep)                 指定点でトリミングする
    /// void stretch(PointD vec, PointD nearPos)        要素の指定位置に近い座標を移動させる
    /// List<PolylineD> divide(PointD dp)               指定位置で分割する
    /// List<PointD> intersection(PointD p)             交点(垂点)の座標リストを求める
    /// List<PointD> intersection(LineD l)              交点の座標リストを求める
    /// List<PointD> intersection(ArcD arc)             交点の座標リストを求める
    /// List<PointD> intersection(PolylineD polyline)   交点の座標リストを求める
    /// List<PointD> intersection(PolygonD polygon)     交点の座標リストを求める
    /// LineD nearLine(PointD p, bool on = false)       最も近い線分を求める
    /// PointD nearPoint(PointD p)                      交点の中で最も近い点を求める
    /// int nearPos(PointD p)                           交点の中で最も近い点の線分位置を求める
    /// PointD nearPeackPoint(PointD p)                 頂点の中で最も近い点の座標
    /// int nearPeackPos(PointD p)                      頂点の中で最も近い点の位置
    /// 
    /// 
    /// </summary>
    public class PolylineD
    {
        public List<PointD> mPolyline;

        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PolylineD()
        {
            mPolyline = new List<PointD>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標リスト</param>
        public PolylineD(List<PointD> polyline)
        {
            mPolyline = polyline.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">PolylineD</param>
        public PolylineD(PolylineD polyline)
        {
            mPolyline = polyline.mPolyline.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// 座標の追加
        /// </summary>
        /// <param name="p"></param>
        public void Add(PointD p)
        {
            mPolyline.Add(p);
        }

        /// <summary>
        /// 座標の挿入
        /// </summary>
        /// <param name="index">挿入位置</param>
        /// <param name="p">座標</param>
        public void Insert(int index, PointD p)
        {
            mPolyline.Insert(index, p);
        }

        /// <summary>
        /// 座標を文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string buf = "";
            mPolyline.ForEach(p => buf += p.ToString());
            return buf;
        }

        /// <summary>
        /// 座標を書式付きで文字列に変換
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string ToString(string form)
        {
            string buf = "";
            mPolyline.ForEach(p => buf += p.ToString(form));
            return buf;
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>PolylineD</returns>
        public PolylineD toCopy()
        {
            return new PolylineD(mPolyline);
        }

        /// <summary>
        /// 座標点リストに変換
        /// </summary>
        /// <returns></returns>
        public List<PointD> toPointList()
        {
            return mPolyline.ConvertAll(p => new PointD(p));
        }

        /// <summary>
        /// 線分リストに変換
        /// </summary>
        /// <returns>線分リスト</returns>
        public List<LineD> toLineList()
        {
            List<LineD> llist = new List<LineD>();
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
                llist.Add(line);
            }
            return llist;
        }

        /// <summary>
        /// 水平垂直線分の座標点リストに変換
        /// </summary>
        /// <returns></returns>
        public List<PointD> toHVLine()
        {
            List<PointD> hvplist = new List<PointD>();
            bool horizontal = false;
            for (int i = 0; i < mPolyline.Count; i++) {
                if (i == 0) {
                    hvplist.Add(mPolyline[0]);
                    continue;
                } else if (i == 1) {
                    LineD line = new LineD(mPolyline[0], mPolyline[1]);
                    horizontal = line.directHorizontal();
                } else if (i == 2) {
                    hvplist.RemoveAt(hvplist.Count - 1);
                    horizontal = !horizontal;
                }
                if (horizontal)
                    hvplist.Add(new PointD(mPolyline[i].x, hvplist[^1].y));
                else
                    hvplist.Add(new PointD(hvplist[^1].x, mPolyline[i].y));
                horizontal = !horizontal;
            }
            //  重複削除
            for (int i = hvplist.Count - 1; 0 < i; i--) {
                if (hvplist[i].isEqual(hvplist[i - 1]))
                    hvplist.RemoveAt(i);
            }
            return hvplist;
        }

        /// <summary>
        /// 重複データ削除
        /// </summary>
        public void squeeze()
        {
            for (int i = mPolyline.Count - 1; 0 < i; i--) {
                if (mPolyline[i].isEqual(mPolyline[i - 1]))
                    mPolyline.RemoveAt(i);
            }
        }

        /// <summary>
        /// 全体の長さを求める
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            double length = 0;
            for (int i = 0; i < mPolyline.Count - 1; i++)
                length += mPolyline[i].length(mPolyline[i + 1]);
            return length;
        }

        /// <summary>
        /// Box領域を求める
        /// </summary>
        /// <returns>Box</returns>
        public Box getBox()
        {
            if (mPolyline == null || mPolyline.Count == 0)
                return null;
            Box b = new Box(mPolyline[0]);
            mPolyline.ForEach(p => b.extension(p));
            return b;
        }

        /// <summary>
        /// 指定位置のポリラインの線分を取り出す
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>線分</returns>
        public LineD getLine(int n)
        {
            if (mPolyline != null && 1 < mPolyline.Count && 0 <= n && n < mPolyline.Count - 1)
                return new LineD(mPolyline[n], mPolyline[n + 1]);
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
            int np = nearCrossLinePos(p);
            return getLine(np);
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
                mPolyline.Clear();
                mPolyline.Add(llist[0].ps);
                for (int i = 0; i < llist.Count - 1; i++) {
                    PointD ip = llist[i].intersection(llist[i + 1]);
                    mPolyline.Add(ip);
                }
                mPolyline.Add(llist[llist.Count - 1].pe);
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
        /// 全体を移動する
        /// </summary>
        /// <param name="vec">移動量</param>
        public void translate(PointD vec)
        {
            mPolyline.ForEach(p => p.offset(vec));
        }

        /// <summary>
        /// 原点を中心に全体を回転する
        /// </summary>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(double ang)
        {
            mPolyline.ForEach(p => p.rotate(ang));
        }

        /// <summary>
        /// 指定点を中心に回転する
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(PointD cp, double ang)
        {
            mPolyline.ForEach(p => p.rotate(cp, ang));
        }

        /// <summary>
        /// 指定点を中心に回転する
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="mp">回転位置</param>
        public void rotate(PointD cp, PointD mp)
        {
            mPolyline.ForEach(p => p.rotate(cp, mp));
        }

        /// <summary>
        /// 指定線分でミラーする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(PointD sp, PointD ep)
        {
            mPolyline.ForEach(p => p.mirror(sp, ep));
        }

        /// <summary>
        /// 指定線分でミラーする
        /// </summary>
        /// <param name="l">線分</param>
        public void mirror(LineD l)
        {
            mPolyline.ForEach(p => p.mirror(l.ps, l.pe));
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点</param>
        /// <param name="scale">拡大率</param>
        public void scale(PointD cp, double scale)
        {
            mPolyline.ForEach(p => p.scale(cp, scale));
        }

        /// <summary>
        /// 指定点でトリミングする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(PointD sp, PointD ep)
        {
            int sn = nearCrossLinePos(sp, true);
            int en = nearCrossLinePos(ep, true);
            PointD ps = getLine(sn).intersection(sp);
            PointD pe = getLine(en).intersection(ep);
            if (sn <= en && 0 <= sn && en <= mPolyline.Count - 2) {
                mPolyline.RemoveRange(en + 2, mPolyline.Count - en - 2);
                mPolyline.RemoveRange(0, sn);
                mPolyline[0] = ps;
                mPolyline[mPolyline.Count - 1] = pe;
            } else if (en < sn && 0 <= en && sn <= mPolyline.Count - 2){
                mPolyline.RemoveRange(sn + 2, mPolyline.Count - sn - 2);
                mPolyline.RemoveRange(0, en);
                mPolyline[0] = pe;
                mPolyline[mPolyline.Count - 1] = ps;
            }
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
                mPolyline[pos].translate(vec);
        }

        /// <summary>
        /// 指定位置で分割する
        /// </summary>
        /// <param name="dp">分割参照点</param>
        /// <returns>分割したポリラインリスト</returns>
        public List<PolylineD> divide(PointD dp)
        {
            List<PolylineD> polylineList = new();
            PointD mp = nearCrossPoint(dp);
            if (mp == null)
                return polylineList;
            int pos = nearCrossLinePos(mp, true);

            PolylineD polyline0 = toCopy();
            int last = polyline0.mPolyline.Count - 1;
            polyline0.mPolyline.RemoveRange(pos + 1, last - pos);
            polyline0.mPolyline.Add(mp);
            polylineList.Add(polyline0);

            PolylineD polyline1 = toCopy();
            polyline1.mPolyline.RemoveRange(0, pos + 1);
            polyline1.mPolyline.Insert(0, mp);
            polylineList.Add(polyline1);
            return polylineList;
        }

        /// <summary>
        /// 交点(垂点)の座標リストを求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>座標点リスト</returns>
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
        /// 交点の座標リストを求める
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns></returns>
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
        /// <param name="on">線上の交点</param>
        /// <returns>線分</returns>
        public LineD nearLine(PointD p, bool on = false)
        {
            int np = nearCrossLinePos(p, on);
            return getLine(np);
        }

        /// <summary>
        /// 交点の中で最も近い点を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>交点</returns>
        public PointD nearCrossPoint(PointD p)
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
        /// <param name="on">線上の交点</param>
        /// <returns>線分位置</returns>
        public int nearCrossLinePos(PointD p, bool on = false)
        {
            List<LineD> llist = toLineList();
            double length = double.MaxValue;
            int nearNo = 0;
            for (int i = 0; i < llist.Count; i++) {
                PointD ip = llist[i].intersection(p);
                double len = ip.length(p);
                if ((!on || llist[i].onPoint(ip)) && len <= length + mEps && ip != llist[i].pe) {
                    if (len < length - mEps) {
                        length = len;
                        nearNo = i;
                    } else {
                        //  距離が等しい時
                        double il = Math.Min(ip.length(llist[i].ps), ip.length(llist[i].pe));
                        double nl = Math.Min(ip.length(llist[nearNo].ps), ip.length(llist[nearNo].pe));
                        if (il < nl) {
                            length = len;
                            nearNo = i;
                        }
                    }
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
            if (mPolyline != null && 0 < mPolyline.Count) {
                double l = double.MaxValue;
                PointD np = mPolyline[0];
                foreach (PointD ip in mPolyline) {
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
            if (mPolyline != null && 0 < mPolyline.Count) {
                double l = double.MaxValue;
                for (int i = 0; i < mPolyline.Count; i++) {
                    double lt = mPolyline[i].length(p);
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
