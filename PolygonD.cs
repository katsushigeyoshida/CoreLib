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
    /// void Insert(int index, PointD p)                座標点の挿入
    /// override string ToString()
    /// string ToString(string form)                    座標を書式付きで文字列に変換
    /// PolygonD toCopy()                               コピーを作成
    /// List<PointD> toPointList()                      座標点リストに変換
    /// List<LineD> toLineList(bool withoutArc = false) 線分リストに変換
    /// List<ArcD> toArcList()                          円弧データの取得
    /// void squeeze()                                  重複データ削除
    /// double length()                                 全体の長さ
    /// Box getBox()                                    ポリゴンの領域を求める
    /// LineD getLine(int n)                            指定位置のポリラインの線分を取り出す
    /// LineD getLine(PointD p)                         指定位置に最も近い線分を取り出す
    /// void translate(PointD vec)                      全体を移動する
    /// void rotate(double ang)                         原点を中心に全体を回転する
    /// void rotate(PointD cp, double ang)              指定点を中心に全体を回転
    /// void rotate(PointD cp, PointD rp)               指定点を中心に全体を回転
    /// void mirror(PointD sp, PointD ep)               指定の線分に対してミラーする
    /// void mirror(LineD line)                         指定の線分に対してミラーする
    /// void offset(double d)                           オフセットする
    /// List<PointD> offsetLineArc(double dis)          円弧を含むポリラインのオフセット(進行方向に左が+値、右が-値)
    /// void offset(PointD sp, PointD ep)               垂直方向に平行移動させる
    /// void scale(PointD cp, double scale)             原点を指定して拡大縮小
    ///void stretch(PointD vec, PointD nearPos, bool arc = false)   要素の指定位置に近い座標を移動させる
    /// PolylineD divide(PointD dp)                     要素を分割するしたポリラインを作成
    /// List<PointD> intersection(PointD p)             点との交点(垂点)リストを求める
    /// List<PointD> intersection(LineD l)              線分との交点リストを求める
    /// List<PointD> intersection(ArcD arc)             円弧との交点の座標リストを求める
    /// List<PointD> intersection(PolylineD polyline)   ポリラインとの交点の座標リストを求める
    /// List<PointD> intersection(PolygonD polygon)     ポリゴンとの交点の座標リストを求める
    /// LineD nearLine(PointD p)                        最も近い線分を求める
    /// PointD nearCrossPoint(PointD p)                 交点の中で最も近い点を求める
    /// (int n, PointD pos) nearCrossPos(PointD p)      交点とその線分または円弧の位置
    /// int nearCrossLinePos(PointD p)                  交点の中で最も近い点の線分位置
    /// ointD nearPeackPoint(PointD p)                  頂点の中で最も近い点の座標
    /// int nearPeackPos(PointD p)                      頂点の中で最も近い点の位置
    /// List<PointD> holePlate2Quads(List<PolygonD> polygons)   中抜き面の作成
    /// 
    /// </summary>
    public class PolygonD
    {
        public List<PointD> mPolygon;
        public double mArcDivideAng = Math.PI / 12;         //  円弧の分割角度
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
        /// 座標の挿入
        /// </summary>
        /// <param name="index">挿入位置</param>
        /// <param name="p">座標</param>
        public void Insert(int index, PointD p)
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
        public List<PointD> toPointList(double divAng = 0)
        {
            List<PointD> plist = new List<PointD>();
            for (int i = 0; i < mPolygon.Count; i++) {
                if (0 < divAng && mPolygon[(i + 1) % mPolygon.Count].type == 1) {
                    //  円弧を座標点リストに変換
                    ArcD arc = new ArcD(mPolygon[i], mPolygon[(i + 1) % mPolygon.Count], mPolygon[(i + 2) % mPolygon.Count]);
                    if (arc.mCp != null) {
                        List<PointD> pplist = arc.toPointList(divAng);
                        if (!arc.mCcw)
                            pplist.Reverse();
                        pplist.RemoveAt(pplist.Count - 1);
                        plist.AddRange(pplist);
                    } else {
                        plist.Add(mPolygon[i].toCopy());
                        plist.Add(mPolygon[(i + 1) % mPolygon.Count].toCopy());
                    }
                    i++;
                } else {
                    plist.Add(mPolygon[i].toCopy());
                }
            }
            return plist;
            //return mPolygon.ConvertAll(p => new PointD(p));
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
                if (withoutArc && mPolygon[(i + 1) % mPolygon.Count].type == 1) {
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
                if (mPolygon[(i + 1) % mPolygon.Count].type == 1) {
                    ArcD arc = new ArcD(mPolygon[i], mPolygon[(i + 1) % mPolygon.Count], mPolygon[(i + 2) % mPolygon.Count]);
                    arclist.Add(arc);
                    i++;
                }
            }
            return arclist;
        }

        /// <summary>
        /// 重複データ削除
        /// 隣り合う座標が同じもの、角度が180°になるものを削除
        /// </summary>
        public void squeeze()
        {
            //  隣同士が同一座標の削除
            for (int i = mPolygon.Count - 1; 0 <= i; i--) {
                if (mPolygon[i] == null || mPolygon[i].length(mPolygon[i == 0 ? ^1 : i - 1]) < mEps)
                    if (mPolygon[i + 1].type ==1)
                        mPolygon.RemoveAt(i == 0 ? mPolygon.Count - 1 : i - 1);
                    else
                        mPolygon.RemoveAt(i);
            }
            //  角度が180°になる座標を削除
            for (int i = mPolygon.Count - 2; 0 <= i; i--) {
                if ((Math.PI - mPolygon[i].angle(mPolygon[i == 0 ? ^1 : i - 1], mPolygon[i + 1])) < mEps)
                    mPolygon.RemoveAt(i);
            }
        }

        /// <summary>
        /// 全体の長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            return length(mPolygon.Count);
            //double length = 0;
            //List<LineD> llist = toLineList(true);
            //length = llist.Sum(l => l.length());
            //List<ArcD> alist = toArcList();
            //length += alist.Sum(a => a.mOpenAngle * a.mR);
            //return length;
        }

        /// <summary>
        /// 始点からの線上の距離
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>距離</returns>
        public double length(PointD pos)
        {
            (int n, PointD sp) = nearCrossPos(pos);
            if (n < 0)
                return -1;
            else
                return length(sp, n);
        }

        /// <summary>
        /// 座標位置までの戦場の長さ
        /// </summary>
        /// <param name="n">座標位置</param>
        /// <param name="st">開始座標位置</param>
        /// <returns>距離</returns>
        public double length(int n, int st = 0)
        {
            double len = 0;
            for (int i = st; i < n; i++) {
                int i1 = (i + 1) % mPolygon.Count;
                int i2 = (i + 2) % mPolygon.Count;
                if (i < mPolygon.Count - 1 && mPolygon[i1].type == 1) {
                    ArcD arc = new ArcD(mPolygon[i], mPolygon[i1], mPolygon[i2]);
                    len += arc.length();
                } else {
                    len += mPolygon[i].length(mPolygon[i1]);
                }
            }
            return len;
        }


        /// <summary>
        /// 指定座標位置から線上の距離
        /// </summary>
        /// <param name="pos">線上の座標</param>
        /// <param name="n">線上の位置</param>
        /// <param name="st">開始位置</param>
        /// <returns>距離</returns>
        public double length(PointD pos, int n, int st = 0)
        {
            double len = 0;
            for (int i = st; i < n; i++) {
                if (i < mPolygon.Count - 1 && mPolygon[i + 1].type == 1) {
                    ArcD arc = new ArcD(mPolygon[i], mPolygon[i + 1], mPolygon[i + 2]);
                    len += arc.length();
                } else {
                    len += mPolygon[i].length(mPolygon[i + 1]);
                }
            }
            if (n < mPolygon.Count - 1 && mPolygon[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolygon[n], mPolygon[n + 1], mPolygon[n + 2]);
                if (arc.mCcw)
                    arc.setEndPoint(pos);
                else
                    arc.setStartPoint(pos);
                len += arc.length();
            } else {
                len += mPolygon[n].length(pos);
            }
            return len;
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
            LineD line0 = null, line1;
            ArcD arc0 = null, arc1;
            int ii, ii1, ii2;
            for (int i = 0; i <= mPolygon.Count; i++) {
                line1 = null;
                arc1 = null;
                ip = null;
                ii = ylib.mod(i, mPolygon.Count);
                ii1 = ylib.mod(i + 1, mPolygon.Count);
                ii2 = ylib.mod(i + 2, mPolygon.Count);
                //  線分か円弧化の区別
                if (mPolygon[ii].type == 0 && mPolygon[ii1].type == 0) {
                    line1 = new LineD(mPolygon[ii], mPolygon[ii1]);
                    line1.offset(dis);
                } else if (mPolygon[ii].type == 0 && mPolygon[ii1].type == 1) {
                    arc1 = new ArcD(mPolygon[ii], mPolygon[ii1], mPolygon[ii2]);
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
                    if (ip == null)
                        return null;
                } else if (line0 != null && arc1 != null) {
                    //  線分と円弧
                    List<PointD> iplist = line0.intersection(arc1, false);
                    if (iplist.Count == 1) {
                        ip = iplist[0];
                    } else if (iplist.Count == 2) {
                        if (line0.pe.length(iplist[0]) < line0.pe.length(iplist[1]))
                            ip = iplist[0];
                        else
                            ip = iplist[1];
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
                    } else
                        return null;
                } else if (arc0 != null && arc1 != null) {
                    //  円弧と円弧
                    List<PointD> iplist = arc0.intersection(arc1, false);
                    if (iplist.Count == 1) {
                        ip = iplist[0];
                    } else if (iplist.Count == 2) {
                        if (mPolygon[ii].length(iplist[0]) < mPolygon[ii].length(iplist[1]))
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
                    } else
                        return null;
                } else if (line0 == null && arc0 == null && line1 != null) {
                    //  始点が線分の時
                    ip = line1.ps;
                } else if (line0 == null && arc0 == null && arc1 != null) {
                    //  始点が円弧の時
                    if (arc1.mCcw) {
                        pline.Add(arc1.startPoint());
                    } else {
                        pline.Add(arc1.endPoint());
                    }
                } else
                    return null;

                if (ip != null) {
                    if (i < mPolygon.Count)
                        pline.Add(ip);
                    else
                        pline[ii] = ip;
                }

                line0 = line1;
                arc0 = arc1;
            }
            return pline;
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
        /// <param name="arc">円弧ストレッチ</param>
        public void stretch(PointD vec, PointD nearPos, bool arc = false)
        {
            (int n, PointD ip) = nearCrossPos(nearPos);
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
            int pos1 = (pos + 1) % mPolygon.Count;
            int pos2 = (pos + 2) % mPolygon.Count;
            if (mPolygon[pos1].type == 1) {
                ArcD arc = new ArcD(mPolygon[pos], mPolygon[pos1], mPolygon[pos2]);
                ArcD arcStart = arc.toCopy();
                ArcD arcEnd = arc.toCopy();
                if (arc.mCcw) {
                    arcStart.setStartPoint(ip);
                    polyline.Add(arcStart.startPoint());
                    PointD mp = arcStart.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos2));
                    polyline.mPolyline.AddRange(mPolygon.Take(pos1));
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
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos2));
                    polyline.mPolyline.AddRange(mPolygon.Take(pos1));
                    arcEnd.setStartPoint(ip);
                    mp = arcEnd.middlePoint();
                    mp.type = 1;
                    polyline.Add(mp);
                    polyline.Add(arcEnd.startPoint());
                }
            } else {
                polyline.Add(ip);
                if (pos1 < mPolygon.Count)
                    polyline.mPolyline.AddRange(mPolygon.Skip(pos1));
                polyline.mPolyline.AddRange(mPolygon.Take(pos1));
                polyline.Add(ip);
            }
            return polyline;
        }


        /// <summary>
        /// 指定位置をフィレットに変換
        /// </summary>
        /// <param name="r">フィレット半径</param>
        /// <param name="pos">座標位置</param>
        public void fillet(double r, PointD pos)
        {
            int n = nearPeackPos(pos);
            int pn = ylib.mod((n - 1), this.mPolygon.Count);
            int nn = ylib.mod((n + 1), this.mPolygon.Count);
            LineD line0 = null, line1;
            ArcD arc, arc0 = null, arc1;
            if (mPolygon[n].type == 1) {
                if (pos.length(mPolygon[pn]) < pos.length(mPolygon[nn]))
                    n -= 1;
                else
                    n += 1;
                n = ylib.mod(n, this.mPolygon.Count);
                pn = ylib.mod((n - 1), this.mPolygon.Count);
                nn = ylib.mod((n + 1), this.mPolygon.Count);
            }
            int ppn = ylib.mod((n - 2), this.mPolygon.Count);
            int nnn = ylib.mod((n + 2), this.mPolygon.Count);
            if (r <= 0) return;
            if (mPolygon[pn].type != 1 && mPolygon[nn].type != 1) {
                line0 = new LineD(mPolygon[pn], mPolygon[n]);
                line1 = new LineD(mPolygon[n], mPolygon[nn]);
                arc = new ArcD(r, line0, mPolygon[pn], line1, mPolygon[nn]);
            } else if (mPolygon[pn].type != 1 && mPolygon[nn].type == 1) {
                line0 = new LineD(mPolygon[pn], mPolygon[n]);
                arc1 = new ArcD(mPolygon[n], mPolygon[nn], mPolygon[nnn]);
                arc = new ArcD(r, line0, mPolygon[pn], arc1, mPolygon[nn]);
            } else if (mPolygon[pn].type == 1 && mPolygon[nn].type != 1) {
                arc0 = new ArcD(mPolygon[ppn], mPolygon[pn], mPolygon[n]);
                line1 = new LineD(mPolygon[n], mPolygon[nn]);
                arc = new ArcD(r, line1, mPolygon[nn], arc0, mPolygon[pn]);
            } else if (mPolygon[pn].type == 1 && mPolygon[nn].type == 1) {
                arc0 = new ArcD(mPolygon[ppn], mPolygon[pn], mPolygon[n]);
                arc1 = new ArcD(mPolygon[n], mPolygon[nn], mPolygon[nnn]);
                arc = new ArcD(r, arc0, mPolygon[pn], arc1, mPolygon[nn]);
            } else
                return;
            List<PointD> plist = arc.to3PointList();
            if ((mPolygon[pn].type != 1 && line0.onPoint(plist[2])) ||
                (mPolygon[pn].type == 1 && arc0.onPoint(plist[2])))
                plist.Reverse();
            mPolygon.RemoveAt(n);
            mPolygon.InsertRange(n, plist);
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
                    if (ip != null && line.onPoint2(ip) && l.onPoint2(ip))
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
        /// 分割点で最も近い点を求める
        /// </summary>
        /// <param name="p">近傍座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public PointD nearPoint(PointD p, int divideNo = 4)
        {
            (int np, PointD pos) = nearCrossPos(p);
            int np1 = (np + 1) % mPolygon.Count;
            int np2 = (np + 2) % mPolygon.Count;
            if (mPolygon[np1].type == 1) {
                ArcD arc = new ArcD(mPolygon[np], mPolygon[np1], mPolygon[np2]);
                return arc.nearPoints(p, divideNo);
            } else if (np < 0) {
                return null;
            } else {
                LineD line = new LineD(mPolygon[np], mPolygon[np1]);
                return line.nearPoint(p, divideNo);
            }
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

        /// <summary>
        /// ポリゴン穴の存在するポリゴン枠を四角形で分割する
        /// 穴付き面を四角形(QUADS)で分割
        /// </summary>
        /// <param name="polygons">内部ポリゴンリスト</param>
        /// <param name="triangle">三角形出力</param>
        /// <returns>四/三角形の座標リスト(QUADS/TRIANGLRS)</returns>
        public List<PointD> holePlate2Quads(List<PolygonD> polygons, bool triangle = false)
        {
            List<List<PointD>> ps = scanMultiPolygon(arc2LinePolygon(polygons));
            if (triangle)
                return scanData2Triangles(ps);
            else
                return scanData2Quads(ps);
        }

        /// <summary>
        /// 円弧を含むポリゴンを線分だけのポリゴンに変換
        /// </summary>
        /// <param name="polygons">ポリゴンリスト</param>
        /// <returns>ポリゴンリスト</returns>
        private List<PolygonD> arc2LinePolygon(List<PolygonD> polygons)
        {
            List<PolygonD> polyList = new List<PolygonD>();
            PolygonD pg = new PolygonD(mPolygon);
            polyList.Add(new PolygonD(pg.toPointList(mArcDivideAng)));
            if (polygons != null && 0 < polygons.Count) {
                foreach (var polygon in polygons) {
                    polyList.Add(new PolygonD(polygon.toPointList(mArcDivideAng)));
                }
            }
            return polyList;
        }


        /// <summary>
        /// ポリゴンで中抜きのポリゴンがある四角形で分割した平面の作成
        /// 水平スキャンした座標リストから四角形座標リスト(QUADS)を作成
        /// </summary>
        /// <param name="scanPoints">水平スキャン座標リスト</param>
        /// <returns>四角形の座標リスト</returns>
        private List<PointD> scanData2Quads(List<List<PointD>> scanPoints)
        {
            List<PointD> quads = new List<PointD>();
            for (int i = 0; i < scanPoints.Count; i++) {
                scanPoints[i].Sort((a, b) => scanCompare(a, b));
                int step = scanPoints[i].Count / 2;
                for (int j = 0; j < step; j += 2) {
                    quads.Add(scanPoints[i][j + 0]);
                    quads.Add(scanPoints[i][j + 1]);
                    quads.Add(scanPoints[i][j + 1 + step]);
                    quads.Add(scanPoints[i][j + 0 + step]);
                }
            }
            return quads;
        }

        /// <summary>
        /// ポリゴンで中抜きのポリゴンがある三角形で分割した平面の作成
        /// 水平スキャンした座標リストから三角形座標リスト(TRIANGLES)を作成
        /// </summary>
        /// <param name="scanPoints">水平スキャン座標リスト</param>
        /// <returns>三角形の座標リスト</returns>
        private List<PointD> scanData2Triangles(List<List<PointD>> scanPoints)
        {
            List<PointD> quads = new List<PointD>();
            for (int i = 0; i < scanPoints.Count; i++) {
                scanPoints[i].Sort((a, b) => scanCompare(a, b));
                int step = scanPoints[i].Count / 2;
                for (int j = 0; j < step; j += 2) {
                    if (mEps < scanPoints[i][j + 1 + step].length(scanPoints[i][j + 0 + step])) {
                        quads.Add(scanPoints[i][j + 0]);
                        quads.Add(scanPoints[i][j + 1 + step]);
                        quads.Add(scanPoints[i][j + 0 + step]);
                    }
                    if (mEps < scanPoints[i][j + 0].length(scanPoints[i][j + 1])) {
                        quads.Add(scanPoints[i][j + 0]);
                        quads.Add(scanPoints[i][j + 1]);
                        quads.Add(scanPoints[i][j + 1 + step]);
                    }
                }
            }
            return quads;
        }

        /// <summary>
        /// 水平座標データの比較
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int scanCompare(PointD a, PointD b)
        {
            if (Math.Abs(a.y - b.y) < mEps)
                return Math.Sign(a.x - b.x);
            return Math.Sign(a.y - b.y);
        }

        /// <summary>
        /// ポリゴン内にポリゴンがある時四角形で分割する座標点を求める
        /// 隣り合う水平線の座標点リスト
        /// </summary>
        /// <param name="polygons">ポリゴン</param>
        /// <returns>座標点リスト</returns>
        private List<List<PointD>> scanMultiPolygon(List<PolygonD> polygons)
        {
            //  全座標リスト → 水平線基準座標
            List<PointD> points = new List<PointD>();
            for (int i = 0; i < polygons.Count; i++)
                points.AddRange(polygons[i].toPointList());
            points.Sort((a, b) => Math.Sign(a.y - b.y));
            for (int i = points.Count - 1; i > 0; i--) {
                if (Math.Abs(points[i].y - points[i - 1].y) < mEps)
                    points.RemoveAt(i);
            }
            //  全線分リスト
            List<List<LineD>> linesList = new List<List<LineD>>();
            for (int i = 0; i < polygons.Count; i++) {
                List<LineD> lines = polygons[i].toLineList();
                linesList.Add(lines);
            }

            //  水平線交点リスト
            List<List<PointD>> scanPoints = new List<List<PointD>>();
            for (int i = 0; i < points.Count - 1; i++) {
                List<PointD> plist = new List<PointD>();
                //  下部
                for (int j = 0; j < linesList.Count; j++) {
                    for (int k = 0; k < linesList[j].Count; k++) {
                        if (Math.Abs(linesList[j][k].ps.y - linesList[j][k].pe.y) < mEps)
                            continue;
                        if (Math.Abs(points[i].y - linesList[j][k].pe.y) < mEps &&
                            points[i].y < linesList[j][k].ps.y) {
                            plist.Add(linesList[j][k].pe);
                        } else if (Math.Abs(points[i].y - linesList[j][k].ps.y) < mEps &&
                            points[i].y < linesList[j][k].pe.y) {
                            plist.Add(linesList[j][k].ps);
                        } else if ((Math.Abs(points[i].y - linesList[j][k].ps.y) > mEps) &&
                            (Math.Abs(points[i].y - linesList[j][k].pe.y) > mEps) &&
                                    linesList[j][k].intersectionHorizon(points[i])) {
                            //  線分との交差点
                            PointD p = linesList[j][k].intersectHorizonPoint(points[i]);
                            if (linesList[j][k].onPoint(p))
                                plist.Add(p);
                        }
                    }
                }
                //  上部
                for (int j = 0; j < linesList.Count; j++) {
                    for (int k = 0; k < linesList[j].Count; k++) {
                        if (Math.Abs(linesList[j][k].ps.y - linesList[j][k].pe.y) < mEps)
                            continue;
                        if (Math.Abs(points[i + 1].y - linesList[j][k].pe.y) < mEps &&
                            points[i + 1].y > linesList[j][k].ps.y) {
                            plist.Add(linesList[j][k].pe);
                        } else if (Math.Abs(points[i + 1].y - linesList[j][k].ps.y) < mEps &&
                            points[i + 1].y > linesList[j][k].pe.y) {
                            plist.Add(linesList[j][k].ps);
                        } else if ((Math.Abs(points[i + 1].y - linesList[j][k].ps.y) > mEps) &&
                            (Math.Abs(points[i + 1].y - linesList[j][k].pe.y) > mEps) &&
                                    linesList[j][k].intersectionHorizon(points[i + 1])) {
                            //  線分との交差点
                            PointD p = linesList[j][k].intersectHorizonPoint(points[i + 1]);
                            if (linesList[j][k].onPoint(p))
                                plist.Add(p);
                        }
                    }
                }
                if (0 < plist.Count)
                    scanPoints.Add(plist);
            }
            return scanPoints;
        }
    }
}
