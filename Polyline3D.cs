using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// ポリラインクラス
    /// </summary>
    public class Polyline3D
    {
        public Point3D mCp = new Point3D();                 //  中心座標
        public Point3D mU = new Point3D(1, 0, 0);           //  平面のX軸の向き(単位ベクトル
        public Point3D mV = new Point3D(0, 1, 0);           //  平面のY軸の向き(単位ベクトル)
        public List<PointD> mPolyline;

        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Polyline3D() {
            mPolyline = new List<PointD>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        /// <param name="squeezeFlg">重複チェック</param>
        public Polyline3D(List<Point3D> polyline, bool squeezeFlg = true)
        {
            if (squeezeFlg)
                polyline = squeeze(polyline);
            mCp = polyline[0].toCopy();
            (mU, mV) = getFace(polyline);
            mPolyline = new List<PointD>();
            for (int i = 0; i < polyline.Count; i++) {
                mPolyline.Add(Point3D.cnvPlaneLocation(polyline[i], mCp, mU, mV));
            }
        }

        /// <summary>
        /// 座標点からポリラインの平面を求める
        /// </summary>
        /// <param name="plist">3D座標点リスト</param>
        /// <returns>平面のパラメータ</returns>
        public (Point3D u, Point3D v) getFace(List<Point3D> plist)
        {
            Point3D u = new Point3D(1, 0, 0), v = new Point3D(0, 1, 0);
            if (plist.Count ==2) {
                u = plist[1] - plist[0];
                u.unit();
                v = mU.toCopy();
                if (mEps < plist[1].toPointXY().length(plist[0].toPointXY()))
                    v.rotate(new Point3D(), Math.PI / 2, FACE3D.XY);
                else
                    v.rotate(new Point3D(), Math.PI / 2, FACE3D.YZ);
            } else if (2 < plist.Count) {
                u = plist[1] - plist[0];
                u.unit();
                Line3D l = new Line3D(plist[0], plist[1]);
                Point3D ip = l.intersection(plist[2]);
                v = plist[2] - ip;
                v.unit();
            }
            return (u, v);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        public Polyline3D(List<Point3D> polyline, FACE3D face)
        {
            mPolyline = polyline.ConvertAll(p => p.toPoint(face));
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">2D座標リスト</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(List<PointD> polyline, FACE3D face)
        {
            mPolyline = polyline.ConvertAll(p => p.toCopy());
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        public Polyline3D(Polyline3D polyline)
        {
            mPolyline = polyline.toPointD();
            mCp = polyline.mCp.toCopy();
            mU = polyline.mU.toCopy();
            mV = polyline.mV.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        public Polyline3D(Polygon3D polygon)
        {
            mPolyline = polygon.toPointD();
            mPolyline.Add(mPolyline[0].toCopy());
            mCp = polygon.mCp.toCopy();
            mU = polygon.mU.toCopy();
            mV = polygon.mV.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">線分</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(Line3D line, FACE3D face)
        {
            mPolyline = new List<PointD>() { line.mSp.toPoint(face), line.endPoint().toPoint(face) };
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="divAng">分割角度</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(Arc3D arc, double divAng, FACE3D face)
        {
            mPolyline = arc.toPointD(divAng, face);
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Polyline3D</returns>
        public Polyline3D toCopy()
        {
            Polyline3D poly = new Polyline3D();
            poly.mCp = mCp.toCopy();
            poly.mU = mU.toCopy();
            poly.mV = mV.toCopy();
            poly.mPolyline = mPolyline.ConvertAll(p => p.toCopy());
            return poly;
        }

        /// <summary>
        /// 3D座標点リストに変換
        /// </summary>
        /// <returns>3D座標点リスト</returns>
        public List<Point3D> toPoint3D()
        {
            List<Point3D> plist = new List<Point3D>();
            for (int i = 0; i < mPolyline.Count; i++) {
                plist.Add(Point3D.cnvPlaneLocation(mPolyline[i], mCp, mU, mV));
            }
            return plist;
        }

        /// <summary>
        /// 3D座標で指定位置の座標を抽出
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>3D座標</returns>
        public Point3D toPoint3D(int n)
        {
            return Point3D.cnvPlaneLocation(mPolyline[n], mCp, mU, mV);
        }

        /// <summary>
        /// 始点の座標
        /// </summary>
        /// <returns>3D座標</returns>
        public Point3D toFirstPoint3D()
        {
            return Point3D.cnvPlaneLocation(mPolyline[0], mCp, mU, mV);
        }

        /// <summary>
        /// 終端の座標
        /// </summary>
        /// <returns>3D座標</returns>
        public Point3D toLastPoint3D()
        {
            return Point3D.cnvPlaneLocation(mPolyline[^1], mCp, mU, mV);
        }

        /// <summary>
        /// 2Dの座標点リストに変換
        /// </summary>
        /// <returns></returns>
        public List<PointD> toPointD()
        {
            return mPolyline.ConvertAll(p => p.toCopy());
        }

        /// <summary>
        /// 2Dの座標点リストに変換
        /// </summary>
        /// <param name="face">作成面</param>
        /// <returns>2Dの座標点リスト</returns>
        public List<PointD> toPointD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            return plist.ConvertAll(p => p.toPoint(face));
        }

        /// <summary>
        /// 線分リストに変換する
        /// </summary>
        /// <returns>線分リスト</returns>
        public List<Line3D> toLine3D()
        {
            List<Point3D> plist = toPoint3D();
            List<Line3D> lines = new List<Line3D>();
            for (int i = 0; i < plist.Count - 1; i++) {
                lines.Add(new Line3D(plist[i], plist[i + 1]));
            }
            return lines;
        }

        /// <summary>
        /// 2D線分に変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2D線分リスト</returns>
        public List<LineD> toLineD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            List<LineD> lines = new List<LineD>();
            for (int i = 0; i < plist.Count - 1; i++) {
                LineD line = new LineD(plist[i].toPoint(face), plist[i + 1].toPoint(face));
                line.mEps = 1e-4;
                lines.Add(line);
            }
            return lines;
        }

        /// <summary>
        /// 指定位置の線分を取得
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>線分</returns>
        public Line3D getLine3D(int n)
        {
            if (n < mPolyline.Count - 1)
                return new Line3D(toPoint3D(n), toPoint3D(n + 1));
            else
                return null;
        }

        /// <summary>
        /// 2Dのポリラインに変換する
        /// </summary>
        /// <param name="face">作成面</param>
        /// <returns>2Dポリライン</returns>
        public PolylineD toPolylineD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            PolylineD polyline = new PolylineD();
            polyline.mPolyline = plist.ConvertAll(p => p.toPoint(face));
            return polyline;
        }

        /// <summary>
        /// 座標点の追加(このポリラインの平面上の座標に変換)
        /// </summary>
        /// <param name="p">座標点</param>
        public void add(Point3D p)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            mPolyline.Add(Point3D.cnvPlaneLocation(p, mCp, mU, mV));
        }

        /// <summary>
        /// 座標点の追加(このポリラインの平面上の座標に変換)
        /// </summary>
        /// <param name="l">線分</param>
        public void add(Line3D l)
        {
            List<Point3D> plist = l.toPoint3D();
            add(plist);
        }

        /// <summary>
        /// 座標点の追加(このポリラインの平面上の座標に変換)
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="divAng">円弧の分割角度</param>
        public void add(Arc3D arc, double divAng = Math.PI / 20)
        {
            List<Point3D> plist = arc.toPoint3D(divAng);
            add(plist);
        }

        /// <summary>
        /// 座標点リストの追加(このポリラインの平面上の座標に変換)
        /// </summary>
        /// <param name="plist">座標点リスト</param>
        public void add(List<Point3D> plist)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            List<Point3D> flist = toPoint3D();
            flist.AddRange(plist);
            flist = squeeze(flist);
            mCp = flist[0].toCopy();
            (mU, mV) = getFace(flist);
            mPolyline = new List<PointD>();
            for (int i = 0; i < flist.Count; i++)
                mPolyline.Add(Point3D.cnvPlaneLocation(flist[i], mCp, mU, mV));
        }

        /// <summary>
        /// 指定点に遠い方を始点として座標データを追加
        /// </summary>
        /// <param name="plist">座標リスト</param>
        /// <param name="loc">指定点</param>
        /// <param name="near">近点を始点</param>
        public void add(List<Point3D> plist, PointD loc, FACE3D face, bool near)
        {
            Polyline3D polyline = new Polyline3D(plist);
            if (polyline.nearStart(loc, face) ^ near)
                plist.Reverse();
            add(plist);
        }

        /// <summary>
        /// 座標点を先頭に追加する
        /// </summary>
        /// <param name="p"></param>
        public void addFirst(Point3D p)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            mPolyline.Insert(0, Point3D.cnvPlaneLocation(p, mCp, mU, mV));
        }

        /// <summary>
        /// 円弧の座標リストを先頭に追加する
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="divAng">円弧の分割角度</param>
        public void addFirst(Arc3D arc, double divAng = Math.PI / 20)
        {
            List<Point3D> plist = arc.toPoint3D(divAng);
            addFirst(plist);
        }

        /// <summary>
        /// 座標リストを先頭に追加する
        /// </summary>
        /// <param name="plist"></param>
        public void addFirst(List<Point3D> plist)
        {
            List<PointD> p2list = plist.ConvertAll(p => Point3D.cnvPlaneLocation(p, mCp, mU, mV));
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            p2list.AddRange(mPolyline);
            mPolyline = p2list;
        }

        /// <summary>
        /// 座標点の挿入
        /// </summary>
        /// <param name="n"></param>
        /// <param name="p"></param>
        public void insert(int n, Point3D p)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            mPolyline.Insert(n, Point3D.cnvPlaneLocation(p, mCp, mU, mV));
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="d">オフセット値</param>
        public void offset(double d)
        {
            PolylineD polyline = new PolylineD(mPolyline);
            polyline.offset(d);
            mPolyline = polyline.mPolyline;
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(Point3D sp, Point3D ep)
        {
            Line3D line = getLine3D(nearLine(sp));
            PointD spp = Point3D.cnvPlaneLocation(sp, mCp, mU, mV);
            PointD epp = Point3D.cnvPlaneLocation(ep, mCp, mU, mV);
            LineD l = line.toLineD(mCp, mU, mV);
            double dis = l.distance(epp) * Math.Sign(l.crossProduct(epp)) - l.distance(spp) * Math.Sign(l.crossProduct(spp));
            PolylineD polyline = new PolylineD(mPolyline);
            polyline.offset(dis);
            mPolyline = polyline.mPolyline;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="v">移動ベクトル</param>
        public void translate(Point3D v)
        {
            mCp.translate(v);
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角度</param>
        /// <param name="face">回転面ン</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            mCp.rotate(cp, ang, face);
            mU.rotate(ang, face);
            mV.rotate(ang, face);
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(Point3D sp, Point3D ep)
        {
            PolylineD polyline = new PolylineD(mPolyline);
            PointD sp2 = Point3D.cnvPlaneLocation(sp, mCp, mU, mV);
            PointD ep2 = Point3D.cnvPlaneLocation(ep, mCp, mU, mV);
            polyline.trim(sp2, ep2);
            mPolyline = polyline.mPolyline;
        }

        /// <summary>
        /// ミラー
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(Point3D sp, Point3D ep)
        {
            Line3D l = new Line3D(sp, ep);
            mCp = l.mirror(mCp);
            l.mSp = new Point3D();
            mU = l.mirror(mU);
            mV = l.mirror(mV);
        }

        /// <summary>
        /// 拡大縮小
        /// </summary>
        /// <param name="cp">拡大中心</param>
        /// <param name="scale">倍率</param>
        public void scale(Point3D cp, double scale)
        {
            PolylineD polyline = new PolylineD(mPolyline);
            PointD cp2 = Point3D.cnvPlaneLocation(cp, mCp, mU, mV);
            polyline.scale(cp2, scale);
            mPolyline = polyline.mPolyline;
        }

        /// <summary>
        /// 2D分割(2D分割位置による分割)
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">表示面</param>
        /// <returns>ポリラインリスト</returns>
        public List<Polyline3D> divide(PointD pos, FACE3D face)
        {
            List<Polyline3D> polylines = new List<Polyline3D>();
            PolylineD pline = toPolylineD(face);
            PointD mp = pline.nearCrossPoint(pos);
            if (mp == null)
                return polylines;
            int n = pline.nearCrossLinePos(mp, true);
            Point3D ipp = getLine3D(n).intersection(mp, face);
            PointD ip = Point3D.cnvPlaneLocation(ipp, mCp, mU, mV);
            Polyline3D polyline = toCopy();
            polyline.mPolyline = mPolyline.GetRange(0, n + 1);
            polyline.mPolyline.Add(ip);
            polylines.Add(polyline);
            polyline = toCopy();
            polyline.mPolyline = mPolyline.GetRange(n + 1, mPolyline.Count - n - 1);
            polyline.mPolyline.Insert(0, ip);
            polylines.Add(polyline);
            return polylines;
        }

        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="pos">分割座標</param>
        /// <returns>ポリラインリスト</returns>
        public List<Polyline3D> divide(Point3D pos)
        {
            List<Polyline3D> polylines = new List<Polyline3D>();
            int n = nearLine(pos);
            if (0 > n)
                return polylines;
            Line3D l = getLine3D(n);
            Point3D ipp = l.intersection(pos);
            PointD ip = Point3D.cnvPlaneLocation(ipp, mCp, mU, mV);
            Polyline3D polyline = new Polyline3D();
            polyline.mPolyline = mPolyline.GetRange(0, n);
            polyline.mPolyline.Add(ip);
            polylines.Add(polyline);
            polyline = new Polyline3D();
            polyline.mPolyline = mPolyline.GetRange(n, mPolyline.Count - n);
            polyline.mPolyline.Insert(0, ip);
            polylines.Add(polyline);
            return polylines;
        }

        /// <summary>
        /// 隣り合う座標が同じもの、角度が180°になるものを削除する
        /// </summary>
        public void squeeze()
        {
            for (int i = mPolyline.Count - 1; i > 0; i--) {
                if (mPolyline[i].length(mPolyline[i - 1]) < mEps)
                    mPolyline.RemoveAt(i);
            }
            for (int i = mPolyline.Count - 2; i > 0; i--) {
                if ((Math.PI - mPolyline[i].angle(mPolyline[i - 1], mPolyline[i + 1])) < mEps)
                    mPolyline.RemoveAt(i);
            }
        }

        /// <summary>
        /// 隣り合う座標が同じもの、角度が180°になるものを削除する
        /// </summary>
        /// <param name="polyline">3D座標リスト</param>
        /// <returns>3D座標リスト</returns>
        public List<Point3D> squeeze(List<Point3D> polyline)
        {
            for (int i = polyline.Count - 1; i > 0; i--) {
                if (polyline[i].length(polyline[i - 1]) < mEps)
                    polyline.RemoveAt(i);
            }
            for (int i = polyline.Count - 2; i > 0; i--) {
                if ((Math.PI - polyline[i].angle(polyline[i - 1], polyline[i + 1])) < mEps)
                    polyline.RemoveAt(i);
            }
            return polyline;
        }

        /// <summary>
        /// 指定点が終点よりも始点に近いかどうかの判定
        /// </summary>
        /// <param name="loc">指定点</param>
        /// <returns>判定</returns>
        public bool nearStart(Point3D loc)
        {
            PointD loc2d = Point3D.cnvPlaneLocation(loc, mCp, mU, mV);
            return mPolyline[0].length(loc2d) < mPolyline[^1].length(loc2d);
        }

        /// <summary>
        /// 2D平面上で指定点が終点よりも始点に近いかどうかの判定
        /// </summary>
        /// <param name="loc">指定点</param>
        /// <param name="face">2D平面</param>
        /// <returns>判定</returns>
        public bool nearStart(PointD loc, FACE3D face)
        {
            PointD sp = toPoint3D(0).toPoint(face);
            PointD ep = toPoint3D(mPolyline.Count - 1).toPoint(face);
            return sp.length(loc) < ep.length(loc);
        }

        /// <summary>
        /// 指定点に最も近い線分の位置を求める
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>線分の位置</returns>
        public int nearLine(Point3D pos)
        {
            int n = -1;
            double dis = double.MaxValue;
            List<Line3D> lines = toLine3D();
            for (int i = 0; i < lines.Count; i++) {
                Point3D ip = lines[i].intersection(pos);
                double l = ip.length(pos);
                if (l < dis && lines[i].onPoint(ip)) {
                    dis = l;
                    n = i;
                }
            }
            return n;
        }

        /// <summary>
        /// 2D座標で最も近い線分を抽出
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>線分位置</returns>
        public int nearLine(PointD pos, FACE3D face)
        {
            int n = -1;
            double dis = double.MaxValue;
            List<LineD> lines = toLineD(face);
            for (int i = 0; i < lines.Count; i++) {
                PointD ip = lines[i].intersection(pos);
                double l = ip.length(pos);
                if (l < dis && lines[i].onPoint2(ip, 1.0)) {
                    dis = l;
                    n = i;
                }
            }
            return n;
        }

        /// <summary>
        /// 指定点に最も近い線分の分割座標から最も近い2D座標を求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標</returns>
        public PointD nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            int n = nearLine(pos, face);
            LineD l = getLine3D(n).toLineD(face);
            return l.nearPoint(pos, divideNo);
        }

        /// <summary>
        /// 2D座標で交点(垂点)を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face"><2D平面/param>
        /// <returns>3D交点</returns>
        public Point3D intersection(PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            if (0 <= n) {
                Line3D line = getLine3D(n);
                return line.intersection(pos, face);
            } else
                return null;
        }
    }
}
