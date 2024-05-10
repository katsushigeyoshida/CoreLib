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
    /// void insert(int index, PointD p)                座標点の挿入
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
        private double mEps = 1E-8;

        private YLib ylib = new YLib();

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
        public void insert(int index, PointD p)
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
        /// <param name="withoutArc">円弧データを除く</param>
        /// <returns>線分リスト</returns>
        public List<LineD> toLineList(bool withoutArc = false)
        {
            List<LineD> llist = new List<LineD>();
            for (int i = 0; i < mPolygon.Count; i++) {
                if (withoutArc && mPolygon[i + 1].type == 1) {
                    i++;
                } else {
                    LineD line = new LineD(mPolygon[i], mPolygon[(i + 1) % mPolygon.Count]);
                    llist.Add(line);
                }
            }
            return llist;
        }


        /// <summary>
        /// 円弧データの取得
        /// </summary>
        /// <returns>円弧リスト</returns>
        public List<ArcD> toArcList()
        {
            List<ArcD> arclist = new List<ArcD>();
            for (int i = 0; i < mPolygon.Count; i++) {
                if (mPolygon[i + 1].type == 1) {
                    ArcD arc = new ArcD(mPolygon[i], mPolygon[(i + 1) % mPolygon.Count], mPolygon[(i + 2) % mPolygon.Count]);
                    arclist.Add(arc);
                    i++;
                }
            }
            return arclist;
        }

        /// <summary>
        /// 座標の挿入
        /// </summary>
        /// <param name="index">挿入位置</param>
        /// <param name="p">座標</param>
        public void Insert(int index, PointD p)
        {
            mPolygon.Insert(index, p);
        }

        /// <summary>
        /// 重複データ削除
        /// 隣り合う座標が同じもの、角度が180°になるものを削除
        /// </summary>
        public void squeeze()
        {
            //  隣同士が同一座標の削除
            for (int i = mPolygon.Count - 1; 0 < i; i--) {
                if (mPolygon[i] == null || mPolygon[i].isEqual(mPolygon[i - 1]))
                    mPolygon.RemoveAt(i);
            }
            //  始終点が同じ座標の時終点座標を削除
            if (1 < mPolygon.Count && mPolygon[mPolygon.Count - 1].isEqual(mPolygon[0]))
                mPolygon.RemoveAt(mPolygon.Count - 1);
            //  角度が180°になる座標を削除
            for (int i = mPolygon.Count - 2; i > 0; i--) {
                if ((Math.PI - mPolygon[i].angle(mPolygon[i - 1], mPolygon[i + 1])) < mEps)
                    mPolygon.RemoveAt(i);
            }
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
            int np = nearCrossLinePos(p);
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
        /// オフセット(進行方向に左が+値、右が-値)
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
        /// 円弧を含むポリラインのオフセット(進行方向に左が+値、右が-値)
        /// </summary>
        /// <param name="dis">オフセット</param>
        /// <returns>座標リスト</returns>
        public List<PointD> offsetLineArc(double dis)
        {
            if (dis == 0 || mPolygon.Count < 3) return null;
            List<PointD> pline = new List<PointD>();
            PointD ip, mp;
            LineD line0 = null, line1 = null;
            ArcD arc0 = null, arc1 = null;
            int mi, ii, pi, pi2;
            for (int i = 0; i <= mPolygon.Count; i++) {
                line1 = null;
                arc1 = null;
                ip = null;
                mi = ylib.mod(i - 1, mPolygon.Count);
                ii = ylib.mod(i, mPolygon.Count);
                pi = ylib.mod(i + 1, mPolygon.Count);
                pi2 = ylib.mod(i + 2, mPolygon.Count);
                //  線分か円弧化の区別
                if (mPolygon[ii].type == 0 && mPolygon[pi].type == 0) {
                    line1 = new LineD(mPolygon[ii], mPolygon[pi]);
                    line1.offset(dis);
                } else if (mPolygon[ii].type == 0 && mPolygon[pi].type == 1) {
                    arc1 = new ArcD(mPolygon[ii], mPolygon[pi], mPolygon[pi2]);
                    if (arc1.mCcw) {
                        arc1.mR -= dis;
                    } else {
                        arc1.mR += dis;
                    }
                    arc1.mR = Math.Abs(arc1.mR);
                    if (arc1.mR == 0)
                        return null;
                    i++;
                }
                //  交点検出
                if (line0 != null && line1 != null) {
                    //  線分と線分
                    ip = line0.intersection(line1);
                    if (ip != null)
                        pline.Add(ip);
                    else
                        return null;
                } else if (line0 != null && arc1 != null) {
                    //  線分と円弧
                    List<PointD> iplist = line0.intersection(arc1, false);
                    if (iplist.Count == 1) {
                        pline.Add(iplist[0]);
                    } else if (iplist.Count == 2) {
                        if (line0.pe.length(iplist[0]) < line0.pe.length(iplist[1]))
                            pline.Add(iplist[0]);
                        else
                            pline.Add(iplist[1]);
                    } else
                        return null;
                } else if (arc0 != null && line1 != null) {
                    //  円弧と線分
                    List<PointD> iplist = line1.intersection(arc0, false);
                    if (iplist.Count == 1) {
                        ip = iplist[0];
                    } else if (iplist.Count == 2) {
                        if (line1.ps.length(iplist[0]) < line1.ps.length(iplist[1]))
                            ip = iplist[0];
                        else
                            ip = iplist[1];
                    } else
                        return null;
                    if (ip != null) {
                        if (arc0.mCcw)
                            arc0.setPoint(pline[^1], ip);
                        else
                            arc0.setPoint(ip, pline[^1]);
                        mp = arc0.middlePoint();
                        mp.type = 1;
                        pline.Add(mp);
                        if (i < mPolygon.Count)
                            pline.Add(ip);
                        else
                            pline[0] = ip;
                    } else
                        return null;
                } else if (arc0 != null && arc1 != null) {
                    //  円弧と円弧
                    List<PointD> iplist = arc0.intersection(arc1, false);
                    if (iplist.Count == 1) {
                        ip = iplist[0];
                    } else if (iplist.Count == 2) {
                        if (mPolygon[i - 1].length(iplist[0]) < mPolygon[mi].length(iplist[1]))
                            ip = iplist[0];
                        else
                            ip = iplist[1];
                    } else
                        return null;
                    if (ip != null) {
                        if (arc0.mCcw)
                            arc0.setPoint(pline[^1], ip);
                        else
                            arc0.setPoint(ip, pline[^1]);
                        mp = arc0.middlePoint();
                        mp.type = 1;
                        pline.Add(mp);
                        pline.Add(ip);
                    } else
                        return null;
                } else if (line0 == null && arc0 == null && line1 != null) {
                    //  始点が線分の時
                    pline.Add(line1.ps);
                } else if (line0 == null && arc0 == null && arc1 != null) {
                    //  始点が円弧の時
                    pline.Add(arc1.intersection(mPolygon[i]));
                    pline.Add(arc1.intersection(mPolygon[pi]));
                    pline[^1].type = 1;
                } else
                    return null;
                line0 = line1;
                arc0 = arc1;
            }
            return pline;
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
            //offset(dis);
            List<PointD> pline = offsetLineArc(dis);
            if (pline != null)
                mPolygon = pline;

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
        /// 中間点付近をピックした時は座標点を追加
        /// </summary>
        /// <param name="vec">移動量</param>
        /// <param name="nearPos">指定位置</param>
        public void stretch(PointD vec, PointD nearPos, bool arc = false)
        {
            int n = nearCrossLinePos(nearPos);
            LineD line = new LineD(mPolygon[n], mPolygon[(n + 1) % mPolygon.Count]);
            PointD cp = line.centerPoint();
            int pos = nearPeackPos(nearPos);
            PointD np = mPolygon[pos];
            if (cp.length(nearPos) < np.length(nearPos)) {
                Insert(n + 1, cp + vec);
                mPolygon[n + 1].type = arc ? 1 : 0;
            } else {
                mPolygon[pos].translate(vec);
            }
        }

        /// <summary>
        /// 要素を分割するしたポリラインを作成
        /// </summary>
        /// <param name="dp">分割点</param>
        /// <returns>ポリライン</returns>
        public PolylineD divide(PointD dp)
        {
            PolylineD polyline = new PolylineD();
            (int pos, PointD ip) = nearCrossPos(dp);
            if (ip == null)
                return polyline;
            if (mPolygon[pos + 1].type == 1) {
                ArcD arc = new ArcD(mPolygon[pos], mPolygon[pos + 1], mPolygon[pos + 2]);
                ArcD arcStart = arc.toCopy();
                ArcD arcEnd = arc.toCopy();
                if (arc.mCcw) {
                    arcStart.setStartPoint(ip);
                    polyline.Add(arcStart.startPoint());
                    PointD mp = arcStart.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos + 2));
                    polyline.mPolyline.AddRange(mPolygon.Take(pos + 1));
                    arcEnd.setEndPoint(ip);
                    mp = arcEnd.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.Add(arcEnd.endPoint());
                } else {
                    arcStart.setEndPoint(ip);
                    polyline.Add(arcStart.endPoint());
                    PointD mp = arcStart.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos + 2));
                    polyline.mPolyline.AddRange(mPolygon.Take(pos + 1));
                    arcEnd.setStartPoint(ip);
                    mp = arcEnd.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.Add(arcEnd.startPoint());
                }
            } else {
                polyline.Add(ip);
                if (pos + 1 < mPolygon.Count)
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos + 1));
                polyline.mPolyline.AddRange(mPolygon.Take(pos + 1));
                polyline.Add(ip);
            }
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
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD arc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    PointD ip = arc.intersection(p);
                    if (ip != null && arc.onPoint(ip))
                        plist.Add(ip);
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    PointD ip = line.intersection(p);
                    if (ip != null && line.onPoint(ip))
                        plist.Add(ip);
                }
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
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD arc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    plist.AddRange(arc.intersection(l));
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    PointD ip = line.intersection(l);
                    if (ip != null && line.onPoint(ip))
                        plist.Add(ip);
                }
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
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD parc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    plist.AddRange(arc.intersection(parc));
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    plist.AddRange(arc.intersection(line));
                }
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
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD arc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    plist.AddRange(polyline.intersection(arc));
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    plist.AddRange(polyline.intersection(line));
                }
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
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD arc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    plist.AddRange(polygon.intersection(arc));
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    plist.AddRange(polygon.intersection(line));
                }
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
            int np = nearCrossLinePos(p);
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
        /// 交点とその線分または円弧の位置を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>(位置,交点座標)</returns>
        public (int n, PointD pos) nearCrossPos(PointD p)
        {
            int n = -1;
            PointD pos = new PointD();
            double l = double.MaxValue;
            for (int i = 0; i < mPolygon.Count; i++) {
                int ii = ylib.mod(i, mPolygon.Count);
                int i1 = ylib.mod(i + 1, mPolygon.Count);
                int i2 = ylib.mod(i + 2, mPolygon.Count);
                if (mPolygon[i1].type == 1) {   //  円弧
                    ArcD arc = new ArcD(mPolygon[ii], mPolygon[i1], mPolygon[i2]);
                    PointD ip = arc.intersection(p);
                    if (ip != null && arc.onPoint(ip)) {
                        double lt = ip.length(p);
                        if (lt < l) {
                            l = lt;
                            pos = ip;
                            n = i;
                        }
                    }
                    i++;
                } else {                        //  線分
                    LineD line = new LineD(mPolygon[ii], mPolygon[i1]);
                    PointD ip = line.intersection(p);
                    if (ip != null && line.onPoint(ip)) {
                        if (ip != null && line.onPoint(ip)) {
                            double lt = ip.length(p);
                            if (lt < l) {
                                l = lt;
                                pos = ip;
                                n = i;
                            }
                        }
                    }
                }
            }
            return (n, pos);
        }


        /// <summary>
        /// 交点の中で最も近い点の線分位置を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>線分位置</returns>
        public int nearCrossLinePos(PointD p)
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
