using System;
using System.Collections.Generic;
using System.Linq;

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
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Polyline3D() {
            mPolyline = new List<PointD>();
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
        /// <param name="polyline">2D Polyline</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(PolylineD polyline, FACE3D face)
        {
            mPolyline = polyline.mPolyline.ConvertAll(p => p.toCopy());
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">2D Polyline</param>
        /// <param name="cp">平面中心座標</param>
        /// <param name="u">平面X軸の向き</param>
        /// <param name="v">平面y軸の向き</param>
        public Polyline3D(PolylineD polyline, Point3D cp, Point3D u, Point3D v)
        {
            mPolyline = polyline.mPolyline.ConvertAll(p => p.toCopy());
            mCp = cp.toCopy();
            mU = u.toCopy();
            mV = v.toCopy(); ;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        /// <param name="squeezeFlg">重複チェック</param>
        public Polyline3D(List<Point3D> polyline, bool squeezeFlg = true)
        {
            setData(polyline, squeezeFlg);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(List<Point3D> polyline, FACE3D face)
        {
            mPolyline = polyline.ConvertAll(p => p.toPoint(face));
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
        /// <param name="line">線分</param>
        public Polyline3D(Line3D line)
        {
            List<Point3D> plist = new List<Point3D>() { line.mSp, line.endPoint() };
            Polyline3D polyline = new Polyline3D(plist);
            mPolyline = polyline.mPolyline;
            mCp = polyline.mCp;
            mU = polyline.mU;
            mV = polyline.mV;
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
        /// コンストラクタ
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="divAng">分割角度</param>
        public Polyline3D(Arc3D arc, double divAng)
        {
            Polyline3D polyline = arc.toPolyline3D(divAng);
            mPolyline = polyline.mPolyline;
            mCp = polyline.mCp;
            mU = polyline.mU;
            mV = polyline.mV;
        }

        /// <summary>
        /// 座標点からポリラインの平面を求める
        /// </summary>
        /// <param name="plist">3D座標点リスト</param>
        /// <returns>平面のパラメータ</returns>
        public (Point3D u, Point3D v) getFace(List<Point3D> plist)
        {
            Point3D u = new Point3D(1, 0, 0), v = new Point3D(0, 1, 0);
            if (plist.Count == 2) {
                u = plist[1] - plist[0];
                u.unit();
                v = u.toCopy();
                if (mEps < plist[1].toPointXY().length(plist[0].toPointXY()))
                    v.rotate(new Point3D(), Math.PI / 2, FACE3D.XY);
                else
                    v.rotate(new Point3D(), Math.PI / 2, FACE3D.YZ);
            } else if (2 < plist.Count) {
                u = plist[1] - plist[0];
                u.unit();
                Line3D l = new Line3D(plist[0], plist[1]);
                //  平面の精度を上げるために線分ともっとも離れた点を使って平面を作成
                double dis = 0;
                for (int i = 2; i < plist.Count; i++) {
                    Point3D ip = l.intersection(plist[i]);
                    double d = ip.length(plist[i]);
                    if (dis < d) {
                        dis = d;
                        v = plist[i] - ip;
                    }
                }
                v.unit();
            }
            return (u, v);
        }

        /// <summary>
        /// 線分以外の要素を含むポリラインデータの確認
        /// </summary>
        /// <returns>MultiType</returns>
        public bool IsMultiType()
        {
            return 0 <= mPolyline.FindIndex(p => p.type != 0);
        }

        /// <summary>
        /// データを設定する
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        /// <param name="squeezeFlg">重複チェック</param>
        public void setData(List<Point3D> polyline, bool squeezeFlg = true)
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
        public List<Point3D> toPoint3D(double divAng = 0)
        {
            List<Point3D> plist = new List<Point3D>();
            List<PointD> polyline = new PolylineD(mPolyline).toPointList(divAng);
            for (int i = 0; i < polyline.Count; i++)
                plist.Add(Point3D.cnvPlaneLocation(polyline[i], mCp, mU, mV));
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
            if (mPolyline.Count <= n || mPolyline[n].type == 1 ||
                mPolyline[(n + 1) % mPolyline.Count].type == 1)
                return null;
            return new Line3D(toPoint3D(n), toPoint3D(n + 1));
        }

        /// <summary>
        /// 指定位置の円弧の取得
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>円弧</returns>
        public Arc3D getArc3D(int n)
        {
            int n0 = ylib.mod(n - 1, mPolyline.Count);
            int n1 = ylib.mod(n, mPolyline.Count);
            int n2 = ylib.mod(n + 1, mPolyline.Count);
            int n3 = ylib.mod(n + 2, mPolyline.Count);
            if (mPolyline[n1].type == 1)
                return new Arc3D(toPoint3D(n0), toPoint3D(n1), toPoint3D(n2));
            else if (mPolyline[n2].type == 1)
                return new Arc3D(toPoint3D(n1), toPoint3D(n2), toPoint3D(n3));
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
        /// 2Dのポリラインに変換する
        /// </summary>
        /// <param name="cp">2D平面の中心座標</param>
        /// <param name="u">2D平面のX軸向き</param>
        /// <param name="v">2D座標のY軸の向き</param>
        /// <param name="divAng">円弧の分割角度</param>
        /// <returns>2Dポリライン</returns>
        public PolylineD toPolylineD(Point3D cp, Point3D u, Point3D v, double divAng = 0)
        {
            if (divAng == 0) {
                Plane3D plane0 = new Plane3D(mCp, mU, mV);
                Plane3D plane1 = new Plane3D(cp, u, v);
                if (!plane0.isParallel(plane1))
                    divAng = Math.PI / 12;
            }
            List<Point3D> plist = toPoint3D(divAng);
            PolylineD polyline = new PolylineD();
            polyline.mPolyline = plist.ConvertAll(p => p.toPointD(cp, u, v));
            return polyline;
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>平行</returns>
        public bool isParallel(Arc3D arc)
        {
            Plane3D plane0 = new Plane3D(mCp, mU, mV);
            Plane3D plane1 = new Plane3D(arc.mCp, arc.mU, arc.mV);
            return plane0.isParallel(plane1);
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <returns>平行</returns>
        public bool isParallel(Polyline3D polyline)
        {
            Plane3D plane0 = new Plane3D(mCp, mU, mV);
            Plane3D plane1 = new Plane3D(polyline.mCp, polyline.mU, polyline.mV);
            return plane0.isParallel(plane1);
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <returns>平行</returns>
        public bool isParallel(Polygon3D polygon)
        {
            Plane3D plane0 = new Plane3D(mCp, mU, mV);
            Plane3D plane1 = new Plane3D(polygon.mCp, polygon.mU, polygon.mV);
            return plane0.isParallel(plane1);
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
            List<Point3D> flist = toPoint3D();
            Line3D l1 = new Line3D(flist[^1], flist[^2]);
            Line3D l2 = new Line3D(plist[0], plist[1]);
            Point3D p1 = l1.intersection(l2);
            Point3D p2 = l2.intersection(l1);
            if (p1 != null && !p1.isNaN() && l1.onPoint(p1) && 
                p2 != null && !p2.isNaN() && l2.onPoint(p2)) {
                flist[^1] = p1;
                plist[0] = p2;
            }
            flist.AddRange(plist);
            setPointList(flist);
        }

        /// <summary>
        /// 指定点に近い方を始点として座標データを追加
        /// </summary>
        /// <param name="plist">3D座標リスト</param>
        /// <param name="loc">指定点</param>
        /// <param name="face">3D平面</param>
        /// <param name="near">近点を始点</param>
        public void add(List<Point3D> plist, PointD loc, FACE3D face, bool near)
        {
            Polyline3D polyline = new Polyline3D(plist);
            if (polyline.length(new Point3D(loc, face)) < polyline.length() / 2)
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
            mPolyline.Reverse();
            add(plist);
            //List<Point3D> p1list = toPoint3D();
            //plist.AddRange(p1list);
            //setPointList(plist);
        }

        /// <summary>
        /// 3D座標リストを値として設定する
        /// </summary>
        /// <param name="plist">3D座標リスト</param>
        public void setPointList(List<Point3D> plist)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            plist = squeeze(plist);
            mCp = plist[0].toCopy();
            (mU, mV) = getFace(plist);
            mPolyline = new List<PointD>();
            for (int i = 0; i < plist.Count; i++)
                mPolyline.Add(Point3D.cnvPlaneLocation(plist[i], mCp, mU, mV));
        }

        /// <summary>
        /// 座標点の挿入
        /// </summary>
        /// <param name="n">挿入位置</param>
        /// <param name="p">3D座標</param>
        public void insert(int n, Point3D p)
        {
            if (mPolyline == null)
                mPolyline = new List<PointD>();
            mPolyline.Insert(n, Point3D.cnvPlaneLocation(p, mCp, mU, mV));
        }

        /// <summary>
        /// ポリラインの挿入
        /// </summary>
        /// <param name="n">挿入位置</param>
        /// <param name="polyline">3Dポリライン</param>
        public void insert(int n, Polyline3D polyline)
        {
            for (int i = 0; i < polyline.mPolyline.Count; i++) {
                Point3D p = polyline.toPoint3D(i);
                insert(n, p);
            }
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
            PointD spp = Point3D.cnvPlaneLocation(sp, mCp, mU, mV);
            PointD epp = Point3D.cnvPlaneLocation(ep, mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            polyline.offset(spp, epp);
            polyline.squeeze();
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
        /// ミラー
        /// </summary>
        /// <param name="l">基準線分</param>
        /// <param name="face">2D平面</param>
        public void mirror(Line3D l, FACE3D face)
        {
            mCp = l.mirror(mCp, face);
            l.mSp = new Point3D();
            mU = l.mirror(mU, face);
            mV = l.mirror(mV, face);
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
        /// 指定点に最も近い座標点を移動する
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        /// <param name="pickPos">ピック位置</param>
        /// <param name="arc">円弧ストレッチ</param>
        public void stretch(Point3D vec, Point3D pickPos, bool arc = false)
        {
            PointD vvec = vec.toPointD(new Point3D(0, 0, 0), mU, mV);
            PointD ppos = pickPos.toPointD(mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            polyline.stretch(vvec, ppos, arc);
            polyline.squeeze();
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
            (int n, PointD mp) = pline.nearCrossPos(pos, true);
            if (n < 0 || mp == null)
                return polylines;
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
        /// ポリライン同士の接続
        /// </summary>
        /// <param name="polyline">3Dポリライン</param>
        public void connect(Polyline3D polyline)
        {
            if (mPolyline.Count == 0) {
                mPolyline = polyline.mPolyline.ConvertAll(p => p.toCopy());
                mCp = polyline.mCp.toCopy();
                mU = polyline.mU.toCopy();
                mV = polyline.mV.toCopy();
                return;
            }
            if (polyline.mPolyline.Count == 0)
                return;
            List<double> dis = new List<double> {
                toFirstPoint3D().length(polyline.toFirstPoint3D()),
                toFirstPoint3D().length(polyline.toLastPoint3D()),
                toLastPoint3D().length(polyline.toFirstPoint3D()),
                toLastPoint3D().length(polyline.toLastPoint3D())
            };
            int n = dis.IndexOf(dis.Min());
            switch (n) {
                case 0:
                    addFirst(polyline.toPoint3D());
                    break;
                case 1:
                    polyline.reverse();
                    addFirst(polyline.toPoint3D());
                    break;
                case 2:
                    add(polyline.toPoint3D());
                    break;
                case 3:
                    polyline.reverse();
                    add(polyline.toPoint3D());
                    break;
            }
            squeeze();
        }

        /// <summary>
        /// ポリライン同士の接続
        /// </summary>
        /// <param name="pos">ピック位置</param>
        /// <param name="polyline">ポリライン</param>
        /// <param name="pos2">ピック位置</param>
        public void connect(Point3D pos, Polyline3D polyline, Point3D pos2, double divAng = 0)
        {
            if (mPolyline.Count < 3) {
                //  ポリラインが2点以下では平面ができないので接続する要素と合わせて平面を決定
                List<Point3D> p3list = toPoint3D();
                p3list.AddRange(polyline.toPoint3D());
                setData(p3list);
                mPolyline.RemoveRange(2, mPolyline.Count - 2);
            }

            PolylineD polyline0 = new PolylineD(mPolyline);
            PolylineD polyline1 = polyline.toPolylineD(mCp, mU, mV, divAng);
            PointD pos0 = pos.toPointD(mCp, mU, mV);
            PointD pos1 = pos2.toPointD(mCp, mU, mV);
            polyline0.connect(pos0, polyline1, pos1);
            mPolyline = polyline0.toPointList();
        }

        /// <summary>
        /// 座標順を反転する
        /// </summary>
        public void reverse()
        {
            mPolyline.Reverse();
        }

        /// <summary>
        /// 隣り合う座標が同じもの、角度が180°になるものを削除する
        /// </summary>
        public void squeeze()
        {
            //  隣り合う座標が同じものを削除
            for (int i = mPolyline.Count - 1; i > 0; i--) {
                if (mPolyline[i].length(mPolyline[i - 1]) < mEps)
                    mPolyline.RemoveAt(i);
            }
            //  角度が180°になるものを削除
            for (int i = mPolyline.Count - 2; i > 0; i--) {
                if ((mPolyline[i - 1].type == 0 && mPolyline[i].type == 0 && mPolyline[i + 1].type == 0)
                    && (Math.PI - mPolyline[i].angle(mPolyline[i - 1], mPolyline[i + 1])) < mEps)
                    mPolyline.RemoveAt(i);
            }
        }

        /// <summary>
        /// 始終線分交差をチェックしあれば削除する
        /// 始終点が近い状態であれば延長交点を追加
        /// それ以外はそのまま
        /// </summary>
        public void lastCrossCheck()
        {
            if (3 < mPolyline.Count) {
                List<PointD> plist = new List<PointD>();
                plist = mPolyline.ConvertAll(p => p.toCopy());
                PointD ip = null;
                List<PointD> iplist = new List<PointD>();
                if (plist[1].type == 1) {
                    ArcD arc0 = new ArcD(plist[0], plist[1], plist[2]);
                    if (plist[^2].type == 1) {
                        ArcD arc1 = new ArcD(plist[^3], plist[^2], plist[^1]);
                        iplist = arc0.intersection(arc1);
                    } else {
                        LineD line1 = new LineD(mPolyline[^2], mPolyline[^1]);
                        iplist = arc0.intersection(line1);
                    }
                } else {
                    LineD line0 = new LineD(mPolyline[0], mPolyline[1]);
                    if (plist[^2].type == 1) {
                        ArcD arc1 = new ArcD(plist[^3], plist[^2], plist[^1]);
                        iplist = arc1.intersection(line0);
                    } else {
                        LineD line1 = new LineD(mPolyline[^2], mPolyline[^1]);
                        ip = line0.intersection(line1);
                    }
                }
                if (iplist.Count == 1)
                    ip = iplist[0];
                else if (1 < iplist.Count)
                    ip = iplist.MinBy(p => p.length(plist[0]) + p.length(plist[^1]));
                if (ip != null) {
                    //  延長交点
                    double l = plist[0].length(plist[^1]) * 2;
                    if (l > plist[0].length(ip) || l > plist[^1].length(ip)) {
                        //  延長交点が端点の近傍の時使用する
                        plist[0] = ip.toCopy();
                        plist[^1] = ip.toCopy();
                    }
                }
                mPolyline = plist;
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
        /// ポリラインの長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.length();
        }

        /// <summary>
        /// 始点からの周長
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>長さ</returns>
        public double length(Point3D pos)
        {
            PointD p = pos.toPointD(mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.length(p);
        }

        /// <summary>
        /// 指定座標が線上の点かの各線
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>線上</returns>
        public bool onPoint(Point3D pos)
        {
            PointD loc2d = Point3D.cnvPlaneLocation(pos, mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.onPoint(loc2d);
        }

        /// <summary>
        /// 指定点が周長で終点よりも始点に近いかどうかの判定
        /// </summary>
        /// <param name="loc">指定点</param>
        /// <returns>判定</returns>
        public bool nearStart(Point3D loc)
        {
            PointD loc2d = Point3D.cnvPlaneLocation(loc, mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.length(loc2d) < polyline.length() / 2;
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
        /// 指定点に最も近い線分または円弧の分割座標から最も近い2D座標を求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標</returns>
        public Point3D nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            if (mPolyline.Count == 2) {
                Line3D line = new Line3D(toPoint3D(0), toPoint3D(1));
                Line3D l = new Line3D(new Point3D(pos, face), new Point3D(pos, face, 1));
                return line.intersection(l);
            } else {
                Plane3D plane = new Plane3D(mCp, mU, mV);
                Point3D ip = plane.intersection(pos, face); //  投影点座標
                return nearPoint(ip, divideNo);
            }
        }

        /// <summary>
        /// 指定点に最も近い線分または円弧の分割座標から最も近い3D座標を求める
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>3D座標</returns>
        public Point3D nearPoint(Point3D pos, int divideNo)
        {
            PointD p = Point3D.cnvPlaneLocation(pos, mCp, mU, mV);
            int n = nearPosition(pos);
            PointD np;
            if (n < mPolyline.Count - 1 && mPolyline[n].type == 1) {
                ArcD arc = new ArcD(mPolyline[n - 1], mPolyline[n], mPolyline[n + 1]);
                np = arc.nearPoints(p, divideNo);
            } else if (n < mPolyline.Count - 2 && mPolyline[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolyline[n], mPolyline[n + 1], mPolyline[n + 2]);
                np = arc.nearPoints(p, divideNo);
            } else if (n < mPolyline.Count - 1) {
                LineD l = new LineD(mPolyline[n], mPolyline[n + 1]);
                np = l.nearPoint(p, divideNo);
            } else {
                np = mPolyline[mPolyline.Count - 1];
            }
            return Point3D.cnvPlaneLocation(np, mCp, mU, mV);
        }

        /// <summary>
        /// 線上の距離で指定点を超えない最も近い座標点の位置
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>座標位置</returns>
        public int nearPosition(Point3D pos)
        {
            PointD p = Point3D.cnvPlaneLocation(pos, mCp, mU, mV);
            PolylineD polyline = new PolylineD(mPolyline);
            double len = polyline.length(p);
            for (int i = 1; i < mPolyline.Count; i++) {
                if (len < polyline.length(i))
                    return i - 1;
            }
            return mPolyline.Count - 1;
        }

        /// <summary>
        /// 2D座標で交点(垂点)を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face"><2D平面/param>
        /// <returns>3D交点</returns>
        public Point3D intersection(PointD pos, FACE3D face)
        {
            Point3D pos3d = new Point3D(pos, face);
            PointD p = Point3D.cnvPlaneLocation(pos3d, mCp, mU, mV);
            int n = nearPosition(pos3d);
            PointD? ip = null;
            if (n < mPolyline.Count - 1 && mPolyline[n].type == 1) {
                ArcD arc = new ArcD(mPolyline[n - 1], mPolyline[n], mPolyline[n + 1]);
                ip = arc.intersection(p);
            } else if (n < mPolyline.Count - 2 && mPolyline[n + 1].type == 1) {
                ArcD arc = new ArcD(mPolyline[n], mPolyline[n + 1], mPolyline[n + 2]);
                ip = arc.intersection(p);
            } else if (n < mPolyline.Count - 1) {
                LineD line = new LineD(mPolyline[n], mPolyline[n + 1]);
                ip = line.intersection(p);
            } else {
                ip = mPolyline[mPolyline.Count - 1];
            }
            if (ip != null)
                return Point3D.cnvPlaneLocation(ip, mCp, mU, mV);
            else
                return null;
        }

        /// <summary>
        /// 2D平面から投影した位置で点と交点を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Point3D p, PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            Line3D line = getLine3D(n);
            if (line != null) {
                return line.intersection(p.toPoint(face), face);
            } else {
                Arc3D arc2 = getArc3D(n);
                if (arc2 != null)
                    return arc2.intersection(p.toPoint(face), face);
            }
            return null;
        }

        /// <summary>
        /// 2D平面から投影した位置で線分と交点を求める
        /// </summary>
        /// <param name="l">線分</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Line3D l, PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            Line3D line = getLine3D(n);
            if (line != null) {
                return line.intersection(l, face);
            } else {
                Arc3D arc2 = getArc3D(n);
                if (arc2 != null)
                    return arc2.intersection(l, pos, face);
            }
            return null;
        }

        /// <summary>
        /// 2D平面から投影した位置で円弧と交点を求める
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Arc3D arc, PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            Line3D line = getLine3D(n);
            if (line != null) {
                return arc.intersection(line, pos, face);
            } else {
                Arc3D arc2 = getArc3D(n);
                if (arc2 != null)
                    return arc2.intersection(arc, pos, face);
            }
            return null;
        }

        /// <summary>
        /// 2D平面から投影した位置でポリラインと交点を求める
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Polyline3D polyline, PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            int n2 = polyline.nearLine(pos, face);
            Line3D line = getLine3D(n);
            Line3D line2 = polyline.getLine3D(n2);
            if (line != null) {
                if (line2 != null) {
                    return line.intersection(line2, face);
                } else {
                    Arc3D arc = polyline.getArc3D(n2);
                    if (arc != null)
                        return arc.intersection(line, pos, face);
                }
            } else {
                Arc3D arc = getArc3D(n);
                if (line2 != null) {
                    return arc.intersection(line2, pos, face);
                } else {
                    Arc3D arc2 = polyline.getArc3D(n2);
                    if (arc2 != null)
                        return arc.intersection(arc2, pos, face);
                }
            }
            return null;
        }
    }
}
