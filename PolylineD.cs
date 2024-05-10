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
    /// void rotate(double iang)                         原点を中心に全体を回転する
    /// void rotate(PointD cp, double iang)              指定点を中心に回転する
    /// void rotate(PointD cp, PointD ip)               指定点を中心に回転する
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
        public bool mMultiType = false;         //  線分以外の要素を含む
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
        /// <param name="withoutArc">円弧データを除く</param>
        /// <returns>線分リスト</returns>
        public List<LineD> toLineList(bool withoutArc = false)
        {
            List<LineD> llist = new List<LineD>();
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (withoutArc && mPolyline[i + 1].type == 1) {
                    i++;
                } else {
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1) {
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    arclist.Add(arc);
                    i++;
                }
            }
            return arclist;
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
        /// 隣り合う座標が同じもの、角度が180°になるものを削除
        /// </summary>
        public void squeeze()
        {
            for (int i = mPolyline.Count - 1; 0 < i; i--) {
                if (mPolyline[i] == null || mPolyline[i].isEqual(mPolyline[i - 1]))
                    mPolyline.RemoveAt(i);
            }
            for (int i = mPolyline.Count - 2; i > 0; i--) {
                if ((Math.PI - mPolyline[i].angle(mPolyline[i - 1], mPolyline[i + 1])) < mEps)
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
                if (i < mPolyline.Count - 1 && mPolyline[i + 1].type == 1) {
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    len += arc.mOpenAngle + arc.mR * 2;
                } else {
                    len += mPolyline[i].length(mPolyline[i + 1]);
                }
            }
            if (n < mPolyline.Count - 1 && mPolyline[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolyline[n], mPolyline[n + 1], mPolyline[n + 2]);
                if (arc.mCcw)
                    arc.setEndPoint(pos);
                else
                    arc.setStartPoint(pos);
                len += arc.mOpenAngle + arc.mR * 2;
            } else {
                len += mPolyline[n].length(pos);
            }
            return len;
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
            (int np, PointD pos) = nearCrossPos(p);
            return getLine(np);
        }

        /// <summary>
        /// オフセット(垂直方向に平行移動させる)
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(PointD sp, PointD ep)
        {
            //  オフセットの方向(進行方向に左が+値、右が-値)と距離
            LineD line = getLine(sp);
            double dis = line.distance(ep) * Math.Sign(line.crossProduct(ep)) - line.distance(sp) * Math.Sign(line.crossProduct(sp));
            if (dis == 0)
                return;
            //offset(dis);

            List<PointD> pline = offsetLineArc(dis);
            if (pline != null)
                mPolyline = pline;
        }

        /// <summary>
        /// オフセット(進行方向に左が+値、右が-値)
        /// </summary>
        /// <param name="d">オフセット距離</param>
        public void offset(double d)
        {
            List<LineD> llist = toLineList();
            llist.ForEach(l => l.offset(d));
            if (0 < llist.Count) {
                mPolyline.Clear();
                mPolyline.Add(llist[0].ps);
                for (int i = 0; i < llist.Count - 1; i++) {
                    PointD ip = llist[i].intersection(llist[i + 1]);
                    if (ip != null)
                        mPolyline.Add(ip);
                }
                mPolyline.Add(llist[llist.Count - 1].pe);
            }
        }

        /// <summary>
        /// 円弧を含むポリラインのオフセット(進行方向に左が+値、右が-値)
        /// </summary>
        /// <param name="dis">オフセット</param>
        /// <returns>座標リスト</returns>
        public List<PointD> offsetLineArc(double dis)
        {
            List<PointD> pline = new List<PointD>();
            PointD ip, mp;
            LineD line0 = null, line1 = null;
            ArcD arc0 = null, arc1 = null;
            for (int i = 0; i < mPolyline.Count; i++) {
                line1 = null;
                arc1 = null;
                ip = null;
                //  線分か円弧化の区別
                if (i < mPolyline.Count - 1 && mPolyline[i].type == 0 && mPolyline[i + 1].type == 0) {
                    line1 = new LineD(mPolyline[i], mPolyline[i + 1]);
                    line1.offset(dis);
                } else if (i < mPolyline.Count - 2 && mPolyline[i].type == 0 && mPolyline[i + 1].type == 1) {
                    arc1 = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
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
                        pline.Add(ip);
                    } else
                        return null;
                } else if (arc0 != null && arc1 != null) {
                    //  円弧と円弧
                    List<PointD> iplist = arc0.intersection(arc1, false);
                    if (iplist.Count == 1) {
                        ip = iplist[0];
                    } else if (iplist.Count == 2) {
                        if (mPolyline[i - 1].length(iplist[0]) < mPolyline[i - 1].length(iplist[1]))
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
                    pline.Add(arc1.intersection(mPolyline[i]));
                    pline.Add(arc1.intersection(mPolyline[i + 1]));
                    pline[^1].type = 1;
                } else if (line0 != null && line1 == null && arc1 == null) {
                    //  終点が線分の時
                    pline.Add(line0.pe);
                } else if (arc0 != null && line1 == null && arc1 == null) {
                    //  終点が円弧の時
                    if (mPolyline[i].length(arc0.startPoint()) < mPolyline[i].length(arc0.endPoint())) {
                        ip = arc0.startPoint();
                    } else {
                        ip = arc0.endPoint();
                    }
                    if (ip != null) {
                        if (arc0.mCcw)
                            arc0.setEndPoint(ip);
                        else
                            arc0.setStartPoint(ip);
                        mp = arc0.middlePoint();
                        mp.type = 1;
                        pline.Add(mp);
                        pline.Add(ip);
                    } else
                        return null;
                } else
                    return null;
                line0 = line1;
                arc0 = arc1;
            }
            return pline;
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
        /// 要素の指定位置に近い座標を移動させる
        /// 中間点付近をピックした時は座標点を追加
        /// </summary>
        /// <param name="vec">移動量</param>
        /// <param name="nearPos">指定位置(pick pos)</param>
        /// <param name="arc">円弧指定</param>
        public void stretch(PointD vec, PointD nearPos, bool arc = false)
        {
            (int n, PointD ip) = nearCrossPos(nearPos);
            LineD line = new LineD(mPolyline[n], mPolyline[n + 1]);
            PointD cp = line.centerPoint();
            int pos = nearPeackPos(nearPos);
            PointD np = mPolyline[pos];
            if (cp.length(nearPos) < np.length(nearPos)) {
                Insert(n + 1, cp + vec);
                mPolyline[n + 1].type = arc ? 1 : 0;
            } else {
                mPolyline[pos].translate(vec);
            }
        }

        /// <summary>
        /// 指定点でトリミングする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(PointD sp, PointD ep)
        {
            (int sn, PointD ps) = nearCrossPos(sp, false);
            (int en, PointD pe) = nearCrossPos(ep, false);
            if (sn < 0 || en < 0)
                return;
            if (sn < en && 0 <= sn && en <= mPolyline.Count - 2) {
                endCut(en, pe);
                startCut(sn, ps);
            } else if (sn > en && 0 <= en && sn <= mPolyline.Count - 2){
                endCut(sn, ps);
                startCut(en, pe);
            } else if (sn == en && length(ps, sn, sn) < length(pe, en,en)) {
                endCut(en, pe);
                startCut(sn, ps);
            } else if (sn == en && length(ps, sn, sn) > length(pe, en, en)) {
                endCut(sn, ps);
                startCut(en, pe);
            }
        }

        /// <summary>
        /// 折線の後をトリムする
        /// </summary>
        /// <param name="n">トリム座標位置</param>
        /// <param name="pos">トリム座標</param>
        public void endCut(int n, PointD pos)
        {
            if (mPolyline[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolyline[n], mPolyline[n + 1], mPolyline[n + 2]);
                PointD mp;
                if (arc.mCcw) {
                    arc.setEndPoint(pos);
                } else {
                    arc.setStartPoint(pos);
                }
                mp = arc.middlePoint();
                mp.type = 1;
                mPolyline.RemoveRange(n + 1, mPolyline.Count - n - 1);
                mPolyline.Add(mp);
                pos = arc.intersection(pos);
            } else {
                mPolyline.RemoveRange(n + 1, mPolyline.Count - n - 1);
            }
            mPolyline.Add(pos);
        }

        /// <summary>
        /// 折線の前をトリムする
        /// </summary>
        /// <param name="n">トリム座標位置</param>
        /// <param name="pos">トリム座標</param>
        public void startCut(int n, PointD pos)
        {
            if (mPolyline[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolyline[n], mPolyline[n + 1], mPolyline[n + 2]);
                PointD mp;
                if (arc.mCcw) {
                    arc.setStartPoint(pos);
                } else {
                    arc.setEndPoint(pos);
                }
                mp = arc.middlePoint();
                mp.type = 1;
                mPolyline.RemoveRange(0, n + 2);
                mPolyline.Insert(0, mp);
                mPolyline.Insert(0, pos);
            } else {
                mPolyline.RemoveRange(0, n + 1);
            }
            mPolyline.Insert(0, pos);
        }


        /// <summary>
        /// 指定位置で分割する
        /// </summary>
        /// <param name="dp">分割参照点</param>
        /// <returns>分割したポリラインリスト</returns>
        public List<PolylineD> divide(PointD dp)
        {
            List<PolylineD> polylineList = new();
            (int pos, PointD ip) = nearCrossPos(dp, true);
            if (pos < 0 || ip == null)
                return polylineList;
            //  前半部
            PolylineD polyline0 = toCopy();
            int last = polyline0.mPolyline.Count - 1;
            polyline0.mPolyline.RemoveRange(pos + 1, last - pos);
            if (mPolyline[pos + 1].type == 1) { // 円弧
                ArcD arc = new ArcD(mPolyline[pos], mPolyline[pos + 1], mPolyline[pos + 2]);
                if (arc.mCcw) {
                    arc.setEndPoint(ip);
                    PointD mp = arc.middlePoint();
                    mp.type = 1;
                    polyline0.Add(mp);
                } else {
                    arc.setStartPoint(ip);
                    PointD mp = arc.middlePoint();
                    mp.type = 1;
                    polyline0.Add(mp);
                }
            }
            polyline0.Add(ip);
            polylineList.Add(polyline0);
            //  後半部
            PolylineD polyline1 = toCopy();
            polyline1.mPolyline.RemoveRange(0, pos + 1);
            polyline1.mPolyline.Insert(0, ip);
            if (mPolyline[pos + 1].type == 1) { // 円弧
                ArcD arc = new ArcD(mPolyline[pos], mPolyline[pos + 1], mPolyline[pos + 2]);
                if (arc.mCcw) {
                    arc.setStartPoint(ip);
                    PointD mp = arc.middlePoint();
                    mp.type = 1;
                    polyline1.mPolyline[1] = mp;
                } else {
                    arc.setEndPoint(ip);
                    PointD mp = arc.middlePoint();
                    mp.type = 1;
                    polyline1.mPolyline[1] = mp;
                }
            }
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    PointD ip = arc.intersection(p);
                    if (ip != null && arc.onPoint(ip))
                        plist.Add(ip);
                    i++;
                } else {    //  線分
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
                    PointD ip = line.intersection(p);
                    if (ip != null && line.onPoint(ip))
                        plist.Add(ip);
                }
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    plist.AddRange(arc.intersection(l));
                    i++;
                } else {    //  線分
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                    ArcD parc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    plist.AddRange(arc.intersection(parc));
                    i++;
                } else {    //  線分
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    plist.AddRange(polyline.intersection(arc));
                    i++;
                } else {    //  線分
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
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
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                    ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                    plist.AddRange(polygon.intersection(arc));
                    i++;
                } else {    //  線分
                    LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
                    plist.AddRange(polygon.intersection(line));
                }
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
            (int np, PointD pos) = nearCrossPos(p, on);
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
        /// 交点の中で最も近い点の線分(円弧)位置を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="on">線上の交点</param>
        /// <returns>線分位置</returns>
        public (int, PointD) nearCrossPos(PointD p, bool on = false)
        {
            double length = double.MaxValue;
            int nearNo = -1;
            PointD ip, sp, ep, pos = new PointD();
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                sp = mPolyline[i];
                ip = interSection(i, p);
                if (ip != null) {
                    double len = ip.length(p);
                    if (len < length) {
                        if (len < length - mEps) {
                            length = len;
                            nearNo = i;
                            pos = ip;
                        }
                    }
                    if (mPolyline[i + 1].type == 1)
                        i++;
                }
            }
            if (!on && nearNo < 0) {
                //  ポリラインの範囲外
                PointD ps = interSection(0, p, false);
                int en = mPolyline[^2].type == 1 ? mPolyline.Count - 3 : mPolyline.Count - 2;
                PointD pe = interSection(en, p, false);
                if (ps != null && pe != null) {
                    if (ps.length(p) < pe.length(p)) {
                        pos = ps;
                        nearNo = 0;
                    } else {
                        pos = pe;
                        nearNo = en;
                    }
                } else if (ps != null && pe == null) {
                    pos = ps;
                    nearNo = 0;
                } else if (ps == null && pe != null) {
                    pos = pe;
                    nearNo = en;
                }
            }
            return (nearNo, pos);
        }

        /// <summary>
        /// 区間指定の交点
        /// </summary>
        /// <param name="i">座標位置</param>
        /// <param name="p">指定座標</param>
        /// <param name="on">線上の交点</param>
        /// <returns></returns>
        private PointD interSection(int i, PointD p, bool on = true)
        {
            PointD ip;
            if (mPolyline[i + 1].type == 1 && i < mPolyline.Count - 2) {    //  円弧
                ArcD arc = new ArcD(mPolyline[i], mPolyline[i + 1], mPolyline[i + 2]);
                ip = arc.intersection(p);
                if (on && !arc.onPoint(ip))
                    ip = null;
            } else {    //  線分
                LineD line = new LineD(mPolyline[i], mPolyline[i + 1]);
                ip = line.intersection(p);
                if (on && !line.onPoint(ip))
                    ip = null;
            }
            return ip;
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
