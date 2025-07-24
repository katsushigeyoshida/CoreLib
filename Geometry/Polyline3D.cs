using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    /// <summary>
    /// ポリラインクラス
    /// Polyline3D()                                        コンストラクタ
    /// Polyline3D(List<PointD> polyline, FACE3D face)
    /// Polyline3D(PolylineD polyline, FACE3D face)
    /// Polyline3D(PolylineD polyline, Point3D cp, Point3D u, Point3D v)
    /// Polyline3D(List<Point3D> polyline, bool squeezeFlg = true)
    /// Polyline3D(List<Point3D> polyline, FACE3D face)
    /// Polyline3D(Polyline3D polyline)
    /// Polyline3D(Polygon3D polygon)
    /// Polyline3D(Line3D line, FACE3D face)
    /// Polyline3D(Line3D line)
    /// Polyline3D(Arc3D arc, double divAng, FACE3D face)
    /// Polyline3D(Arc3D arc, double divAng)
    /// (Point3D u, Point3D v) getFace(List<Point3D> plist) 座標点からポリラインの平面を求める
    /// bool IsMultiType()                                  線分以外の要素を含むポリラインデータの確認
    /// void setData(List<Point3D> polyline, bool squeezeFlg = true)    データを設定する
    /// Polyline3D toCopy()                                 コピーを作成
    /// List<Point3D> toPoint3D(double divAng = 0)          3D座標点リストに変換
    /// Point3D toPoint3D(int n)                            3D座標で指定位置の座標を抽出
    /// Point3D toFirstPoint3D()                            始点の座標
    /// Point3D toLastPoint3D()                             終端の座標
    /// List<PointD> toPointD()                             2Dの座標点リストに変換
    /// List<PointD> toPointD(FACE3D face)                  2Dの座標点リストに変換
    /// List<Line3D> toLine3D()                             線分リストに変換する
    /// List<LineD> toLineD(FACE3D face)                    2D線分に変換
    /// Line3D getLine3D(int n)                             指定位置の線分を取得
    /// Arc3D getArc3D(int n)                               指定位置の円弧の取得
    /// PolylineD toPolylineD(FACE3D face)                  2Dのポリラインに変換する
    /// bool isParallel(Arc3D arc)                          平行確認
    /// bool isParallel(Polyline3D polyline)                平行確認
    /// bool isParallel(Polygon3D polygon)                  平行確認
    /// void add(Point3D p)                                 座標点の追加
    /// void add(Line3D l)                                  座標点の追加
    /// void add(Arc3D arc, double divAng = Math.PI / 20)   座標点の追加
    /// void add(List<Point3D> plist)                       座標点リストの追加
    /// void add(List<Point3D> plist, PointD loc, FACE3D face, bool near)    指定点に近い方を始点として座標データを追加
    /// void addFirst(Point3D p)                            座標点を先頭に追加
    /// void addFirst(Arc3D arc, double divAng = Math.PI / 20)  円弧の座標リストを先頭に追加
    /// void addFirst(List<Point3D> plist)                  座標リストを先頭に追加
    /// void setPointList(List<Point3D> plist)              3D座標リストを値として設定
    /// void insert(int n, Point3D p)                       座標点の挿入
    /// void insert(int n, Polyline3D polyline)             ポリラインの挿入
    /// void offset(double d)                               オフセット
    /// void offset(Point3D sp, Point3D ep)                 オフセット
    /// void translate(Point3D v)                           移動
    /// void rotate(Point3D cp, double ang, FACE3D face)    回転
    /// void trim(Point3D sp, Point3D ep)                   トリム
    /// void mirror(Point3D sp, Point3D ep)                 ミラー
    /// void mirror(Line3D l, FACE3D face)                  ミラー
    /// void scale(Point3D cp, double scale)                拡大縮小
    /// void stretch(Point3D vec, Point3D pickPos, bool arc = false)    指定点に最も近い座標点を移動する
    /// List<Polyline3D> divide(PointD pos, FACE3D face)    2D分割(2D分割位置による分割)
    /// List<Polyline3D> divide(Point3D pos)                分割
    /// void connect(Polyline3D polyline)                   ポリライン同士の接続
    /// void connect(Point3D pos, Polyline3D polyline, Point3D pos2, double divAng = 0)
    /// void reverse()                                      座標順を反転する
    /// void squeeze()                                      隣り合う座標が同じもの、角度が180°になるものを削除
    /// void lastCrossCheck()                               始終線分交差をチェックしあれば削除
    /// List<Point3D> squeeze(List<Point3D> polyline)       隣り合う座標が同じもの、角度が180°になるものを削除
    /// double length()                                     ポリラインの長さ
    /// double length(Point3D pos)                          始点からの周長
    /// bool onPoint(Point3D pos)                           指定座標が線上の点かの判定
    /// bool nearStart(Point3D loc)                         指定点が周長で終点よりも始点に近いかどうかの判定
    /// int nearLine(Point3D pos)                           指定点に最も近い線分の位置を求める
    /// int nearLine(PointD pos, FACE3D face)               2D座標で最も近い線分を抽出
    /// Point3D nearPoint(PointD pos, int divideNo, FACE3D face)    指定点に最も近い線分または円弧の分割座標から最も近い2D座標
    /// Point3D nearPoint(Point3D pos, int divideNo)        指定点に最も近い線分または円弧の分割座標から最も近い3D座標
    /// int nearPosition(Point3D pos)                       線上の距離で指定点を超えない最も近い座標点の位置
    /// Point3D intersection(PointD pos, FACE3D face)       2D座標で交点(垂点)を求める
    /// Point3D intersection(Point3D p, PointD pos, FACE3D face)    2D平面から投影した位置で点と交点を求める
    /// Point3D intersection(Line3D l, PointD pos, FACE3D face) 2D平面から投影した位置で線分と交点を求める
    /// Point3D intersection(Arc3D arc, PointD pos, FACE3D face)    2D平面から投影した位置で円弧と交点を求める
    /// Point3D intersection(Polyline3D polyline, PointD pos, FACE3D face)  2D平面から投影した位置でポリラインと交点を求める
    /// List<Point3D> rotate2Quads(Line3D centerline, double divAngle, double sang, double eang)    回転体の作成(QUADS)
    /// List<List<Point3D>> rotate2QuadStrip(Line3D centerline, double divAngle, double sang, double eang)  回転体の作成(QUAD_STRIP)
    /// List<List<Point3D>> getCenterLineRotate(Line3D centerline, List<Point3D> outline, double divideAngle, double sa = 0, double ea = 2 * Math.PI)   回転体の外形線作成
    /// 
    /// 
    /// </summary>
    public class Polyline3D
    {
        public Plane3D mPlane = new Plane3D();          //  円の平面
        public List<PointD> mPolyline;
        public double mDivAngle = 0;
        public double mArcDivideAng = Math.PI / 12;     //  円弧の分割角度

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
            mPlane.mU = Point3D.getUVector(face);
            mPlane.mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">2D Polyline</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(PolylineD polyline, FACE3D face)
        {
            mPolyline = polyline.mPolyline.ConvertAll(p => p.toCopy());
            mPlane.mU = Point3D.getUVector(face);
            mPlane.mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">2D Polyline</param>
        /// <param name="plane">3D平面</param>
        public Polyline3D(PolylineD polyline, Plane3D plane)
        {
            mPolyline = polyline.mPolyline.ConvertAll(p => p.toCopy());
            mPlane = plane.toCopy();
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
            mPlane.mCp = cp.toCopy();
            mPlane.mU = u.toCopy();
            mPlane.mV = v.toCopy(); ;
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
        /// <param name="divAng">円弧の分割角度</param>
        /// <param name="squeezeFlg">重複チェック</param>
        public Polyline3D(List<Point3D> polyline, double divAng, bool squeezeFlg = true)
        {
            setData(polyline, squeezeFlg);
            mArcDivideAng = divAng;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">座標点リスト</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(List<Point3D> polyline, FACE3D face)
        {
            mPolyline = polyline.ConvertAll(p => p.toPoint(face));
            mPlane.mU = Point3D.getUVector(face);
            mPlane.mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        public Polyline3D(Polyline3D polyline)
        {
            mPolyline = polyline.toPointD();
            mPlane = polyline.mPlane.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        public Polyline3D(Polygon3D polygon, bool close = true)
        {
            mPolyline = polygon.toPointD();
            if (close)
                mPolyline.Add(mPolyline[0].toCopy());
            mPlane = polygon.mPlane.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">線分</param>
        /// <param name="face">2D平面</param>
        public Polyline3D(Line3D line, FACE3D face)
        {
            mPolyline = new List<PointD>() { line.mSp.toPoint(face), line.endPoint().toPoint(face) };
            mPlane.mU = Point3D.getUVector(face);
            mPlane.mV = Point3D.getVVector(face);
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
            mPlane = polyline.mPlane.toCopy();
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
            mPlane.mU = Point3D.getUVector(face);
            mPlane.mV = Point3D.getVVector(face);
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
            mPlane = polyline.mPlane.toCopy();
        }

        /// <summary>
        /// 座標点からポリラインの平面を求める
        /// </summary>
        /// <param name="plist">3D座標点リスト</param>
        /// <returns>平面のパラメータ</returns>
        public (Point3D u, Point3D v) getFace(List<Point3D> plist)
        {
            return mPlane.getFace(plist);
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
            mPlane.mCp = polyline[0].toCopy();
            (mPlane.mU, mPlane.mV) = getFace(polyline);
            mPolyline = new List<PointD>();
            for (int i = 0; i < polyline.Count; i++) {
                mPolyline.Add(Point3D.cnvPlaneLocation(polyline[i], mPlane.mCp, mPlane.mU, mPlane.mV));
            }
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Polyline3D</returns>
        public Polyline3D toCopy()
        {
            Polyline3D poly = new Polyline3D();
            poly.mPlane = mPlane.toCopy();
            poly.mPolyline = mPolyline.ConvertAll(p => p.toCopy());
            return poly;
        }

        /// <summary>
        /// 3D座標点リストに変換
        /// 指定平面と同一平面の時、円弧は分割しない
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <param name="face">平面</param>
        /// <returns>3D座標点リスト</returns>
        public List<Point3D> toPoint3D(double divAng = 0, FACE3D face = FACE3D.NON)
        {
            List<Point3D> plist = new List<Point3D>();
            if (face != FACE3D.NON && !mPlane.mU.isFace(mPlane.mV, face)) divAng = mArcDivideAng;
            List<PointD> polyline = new PolylineD(mPolyline).toPointList(divAng);
            for (int i = 0; i < polyline.Count; i++)
                plist.Add(Point3D.cnvPlaneLocation(polyline[i], mPlane));
            return plist;
        }

        /// <summary>
        /// 3D座標で指定位置の座標を抽出
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>3D座標</returns>
        public Point3D toPoint3D(int n)
        {
            return Point3D.cnvPlaneLocation(mPolyline[n], mPlane);
        }

        /// <summary>
        /// 始点の座標
        /// </summary>
        /// <returns>3D座標</returns>
        public Point3D toFirstPoint3D()
        {
            return Point3D.cnvPlaneLocation(mPolyline[0], mPlane);
        }

        /// <summary>
        /// 終端の座標
        /// </summary>
        /// <returns>3D座標</returns>
        public Point3D toLastPoint3D()
        {
            return Point3D.cnvPlaneLocation(mPolyline[^1], mPlane);
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
            List<Point3D> plist = toPoint3D(0, face);
            PolylineD polyline = new PolylineD();
            polyline.mPolyline = plist.ConvertAll(p => p.toPoint(face));
            return polyline;
        }

        /// <summary>
        /// 2Dのポリラインに変換する
        /// </summary>
        /// <param name="plane">3D平面</param>
        /// <param name="divAng">円弧の分割角度</param>
        /// <returns>2Dポリライン</returns>
        public PolylineD toPolylineD(Plane3D plane, double divAng = 0)
        {
            return toPolylineD(plane.mCp, plane.mU, plane.mV, divAng);
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
                Plane3D plane0 = mPlane.toCopy();
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
            Plane3D plane0 = mPlane.toCopy();
            Plane3D plane1 = arc.mPlane.toCopy();
            return plane0.isParallel(plane1);
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <returns>平行</returns>
        public bool isParallel(Polyline3D polyline)
        {
            Plane3D plane0 = mPlane.toCopy();
            Plane3D plane1 = polyline.mPlane.toCopy();
            return plane0.isParallel(plane1);
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <returns>平行</returns>
        public bool isParallel(Polygon3D polygon)
        {
            Plane3D plane0 = mPlane.toCopy();
            Plane3D plane1 = polygon.mPlane.toCopy();
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
            mPolyline.Add(Point3D.cnvPlaneLocation(p, mPlane));
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
            mPolyline.Insert(0, Point3D.cnvPlaneLocation(p, mPlane));
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
            mPlane.mCp = plist[0].toCopy();
            (mPlane.mU, mPlane.mV) = getFace(plist);
            mPolyline = new List<PointD>();
            for (int i = 0; i < plist.Count; i++)
                mPolyline.Add(Point3D.cnvPlaneLocation(plist[i], mPlane));
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
            mPolyline.Insert(n, Point3D.cnvPlaneLocation(p, mPlane));
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
            if (sp.length(ep) < mEps) return;
            PointD spp = Point3D.cnvPlaneLocation(sp, mPlane);
            PointD epp = Point3D.cnvPlaneLocation(ep, mPlane);
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
            mPlane.translate(v);
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角度</param>
        /// <param name="face">回転面ン</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            mPlane.rotate(cp, ang, face);
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(Point3D sp, Point3D ep)
        {
            PolylineD polyline = new PolylineD(mPolyline);
            PointD sp2 = mPlane.cnvPlaneLocation(sp);
            PointD ep2 = mPlane.cnvPlaneLocation(ep);
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
            mPlane.mirror(sp, ep);
        }

        /// <summary>
        /// ミラー
        /// </summary>
        /// <param name="l">基準線分</param>
        /// <param name="face">2D平面</param>
        public void mirror(Line3D l, FACE3D face)
        {
            mPlane.mirror(l, face);
        }

        /// <summary>
        /// 拡大縮小
        /// </summary>
        /// <param name="cp">拡大中心</param>
        /// <param name="scale">倍率</param>
        public void scale(Point3D cp, double scale)
        {
            PolylineD polyline = new PolylineD(mPolyline);
            PointD cp2 = mPlane.cnvPlaneLocation(cp);
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
            PointD vvec = vec.toPointD(new Point3D(0, 0, 0), mPlane.mU, mPlane.mV);
            PointD ppos = pickPos.toPointD(mPlane);
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
            PointD ip = mPlane.cnvPlaneLocation(ipp);
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
            PointD ip = mPlane.cnvPlaneLocation(ipp);
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
                mPlane = polyline.mPlane.toCopy();
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
            PolylineD polyline1 = polyline.toPolylineD(mPlane, divAng);
            PointD pos0 = pos.toPointD(mPlane);
            PointD pos1 = pos2.toPointD(mPlane);
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
                if (polyline[i - 1].type == 0 && polyline[i].type == 0 && polyline[i + 1].type == 0 &&
                    (Math.PI - polyline[i].angle(polyline[i - 1], polyline[i + 1])) < mEps)
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
            PointD p = pos.toPointD(mPlane);
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.length(p);
        }

        /// <summary>
        /// 指定座標が線上の点かの判定
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>線上</returns>
        public bool onPoint(Point3D pos)
        {
            PointD loc2d = mPlane.cnvPlaneLocation(pos);
            PolylineD polyline = new PolylineD(mPolyline);
            return polyline.onPoint(loc2d);
        }

        /// <summary>
        /// 円弧か?
        /// </summary>
        /// <returns>円弧</returns>
        public bool isArc()
        {
            if (mPolyline.Count == 3 && mPolyline[1].type == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 円弧に変換
        /// </summary>
        /// <returns>円弧</returns>
        public Arc3D toArc()
        {
            return new Arc3D(toPoint3D(0), toPoint3D(1), toPoint3D(2));
        }

        /// <summary>
        /// 線分か?
        /// </summary>
        /// <returns>線分</returns>
        public bool isLine()
        {
            if (mPolyline.Count == 2 && mPolyline[0].type == 0 && mPolyline[1].type == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 線分に変換
        /// </summary>
        /// <returns>線分</returns>
        public Line3D toLine()
        {
            return new Line3D(toPoint3D(0), toPoint3D(1));
        }

        /// <summary>
        /// 平面の取得
        /// </summary>
        /// <returns>2D平面</returns>
        public FACE3D getFace()
        {
            return mPlane.getFace();
        }

        /// <summary>
        /// 平面が指定の平面と同じか
        /// </summary>
        /// <param name="face">指定平面</param>
        /// <returns>同じ</returns>
        public bool isFace(FACE3D face)
        {
            return mPlane.isFace(face);
        }

        /// <summary>
        /// 指定点が周長で終点よりも始点に近いかどうかの判定
        /// </summary>
        /// <param name="loc">指定点</param>
        /// <returns>判定</returns>
        public bool nearStart(Point3D loc)
        {
            PointD loc2d = mPlane.cnvPlaneLocation(loc);
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
                Plane3D plane = mPlane.toCopy();
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
            PointD p = mPlane.cnvPlaneLocation(pos);
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
            return Point3D.cnvPlaneLocation(np, mPlane);
        }

        /// <summary>
        /// 線上の距離で指定点を超えない最も近い座標点の位置
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>座標位置</returns>
        public int nearPosition(Point3D pos)
        {
            PointD p = mPlane.cnvPlaneLocation(pos);
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
            PointD p = mPlane.cnvPlaneLocation(pos3d);
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
                return Point3D.cnvPlaneLocation(ip, mPlane);
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
        public Point3D intersection(Point3D point, PointD pos, FACE3D face)
        {
            List<Point3D[]> lineArcList = toLineArcList();
            double dis = double.MaxValue;
            Point3D p = null;
            Point3D ip = null;
            foreach (Point3D[] lineArc in lineArcList) {
                if (lineArc.Length == 2) {
                    Line3D line = new Line3D(lineArc[0], lineArc[1]);
                    ip = line.intersection(point, face);
                } else if (lineArc.Length == 3) {
                    Arc3D arc = new Arc3D(lineArc[0], lineArc[1], lineArc[2]);
                    ip = arc.intersection(point, face);
                } else
                    continue;
                if (ip == null) continue;
                double d = ip.toPoint(face).length(pos);
                if (d < dis) {
                    p = ip.toCopy();
                    dis = d;
                }
            }
            return p;
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
            List<Point3D[]> lineArcList = toLineArcList();
            double dis = double.MaxValue;
            Point3D p = null;
            Point3D ip = null;
            foreach (Point3D[] lineArc in lineArcList) {
                if (lineArc.Length == 2) {
                    Line3D line = new Line3D(lineArc[0], lineArc[1]);
                    ip = line.intersection(l, face);
                } else if (lineArc.Length == 3) {
                    Arc3D arc = new Arc3D(lineArc[0], lineArc[1], lineArc[2]);
                    ip = arc.intersection(l, pos, face);
                } else
                    continue;
                if (ip == null) continue;
                double d = ip.toPoint(face).length(pos);
                if (d < dis) {
                    p = ip.toCopy();
                    dis = d;
                }
            }
            return p;
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
            List<Point3D[]> lineArcList = toLineArcList();
            double dis = double.MaxValue;
            Point3D p = null;
            Point3D ip = null;
            foreach (Point3D[] lineArc in lineArcList) {
                if (lineArc.Length == 2) {
                    Line3D line = new Line3D(lineArc[0], lineArc[1]);
                    ip = arc.intersection(line, pos, face);
                } else if (lineArc.Length == 3) {
                    Arc3D a = new Arc3D(lineArc[0], lineArc[1], lineArc[2]);
                    ip = arc.intersection(a, pos, face);
                } else
                    continue;
                if (ip == null) continue;
                double d = ip.toPoint(face).length(pos);
                if (d < dis) {
                    p = ip.toCopy();
                    dis = d;
                }
            }
            return p;
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
            List<Point3D[]> lineArcList = toLineArcList();
            double dis = double.MaxValue;
            Point3D p = null;
            Point3D ip = null;
            foreach (Point3D[] lineArc in lineArcList) {
                if (lineArc.Length == 2) {
                    Line3D line = new Line3D(lineArc[0], lineArc[1]);
                    ip = polyline.intersection(line, pos, face);
                } else if (lineArc.Length == 3) {
                    Arc3D a = new Arc3D(lineArc[0], lineArc[1], lineArc[2]);
                    ip = polyline.intersection(a, pos, face);
                } else
                    continue;
                if (ip == null) continue;
                double d = ip.toPoint(face).length(pos);
                if (d < dis) {
                    p = ip.toCopy();
                    dis = d;
                }
            }
            return p;
        }

        /// <summary>
        /// ポリラインの側面データの作成
        /// </summary>
        /// <param name="vec">押出ベクトル</param>
        /// <returns>座標リスト(QUADS)</returns>
        public List<Point3D> sideFace2Quads(Point3D vec)
        {
            List<Point3D> plist = new List<Point3D>();
            List<Point3D> polylineList = toPoint3D(mArcDivideAng);
            for (int i = 0; i < polylineList.Count -　1; i++) {
                plist.Add(polylineList[i]);
                Point3D p = polylineList[i].toCopy();
                p.translate(vec);
                plist.Add(p);
                p = polylineList[i + 1].toCopy();
                p.translate(vec);
                plist.Add(p);
                plist.Add(polylineList[i + 1]);
            }
            return plist;
        }

        /// <summary>
        /// 回転体の作成(QUADS)
        /// </summary>
        /// <param name="centerline">中心線</param>
        /// <param name="divAngle">分割角度</param>
        /// <param name="sang">開始角</param>
        /// <param name="eang">終了角</param>
        /// <returns>座標リスト(QUADS)</returns>
        public List<Point3D> rotate2Quads(Line3D centerline, double divAngle, double sang, double eang)
        {
            List<Point3D> quads = new List<Point3D>();
            List<Point3D> outLine = toPoint3D(mDivAngle);
            List<List<Point3D>> outlines = getCenterLineRotate(centerline, outLine, divAngle, sang, eang);
            for (int i = 0; i < outlines.Count - 1; i++) {
                for (int j = 0; j < outlines[i].Count - 1; j++) {
                    quads.Add(outlines[i][j]);
                    quads.Add(outlines[i+1][j]);
                    quads.Add(outlines[i+1][j+1]);
                    quads.Add(outlines[i][j+1]);
                }
            }
            return quads;
        }

        /// <summary>
        /// 回転体の作成(QUAD_STRIP)
        /// </summary>
        /// <param name="centerline">中心線</param>
        /// <param name="divAngle">分割角度</param>
        /// <param name="sang">開始角</param>
        /// <param name="eang">終了角</param>
        /// <returns>座標リスト(QUAD_STRIP)</returns>
        public List<List<Point3D>> rotate2QuadStrip(Line3D centerline, double divAngle, double sang, double eang)
        {
            List<List<Point3D>> quadStrip = new List<List<Point3D>>();
            List<Point3D> outLine = toPoint3D(mDivAngle);
            List<List<Point3D>> outlines = getCenterLineRotate(centerline, outLine, divAngle, sang, eang);
            for (int i = 0; i < outlines.Count - 1; i++) {
                List<Point3D> buf = new List<Point3D> ();
                for (int j = 0; j < outlines[i].Count - 1; j++) {
                    buf.Add(outlines[i][j]);
                    buf.Add(outlines[i + 1][j]);
                }
                quadStrip.Add(buf);
            }
            return quadStrip;
        }

        /// <summary>
        /// 回転体の外形線作成
        /// </summary>
        /// <param name="centerline">中心線</param>
        /// <param name="outline">外形線</param>
        /// <param name="divideAngle">分割角度</param>
        /// <param name="sa">開始角</param>
        /// <param name="ea">終了角</param>
        /// <returns>外形線リスト</returns>
        private List<List<Point3D>> getCenterLineRotate(Line3D centerline, List<Point3D> outline, double divideAngle, double sa = 0, double ea = 2 * Math.PI)
        {
            List<List<Point3D>> outLines = new List<List<Point3D>>();
            Point3D cp = centerline.mSp;
            Point3D cv = cp.vector(centerline.endPoint());    //  中心線ベクトル
            cp.inverse();
            outline.ForEach(p => p.add(cp));
            cp.inverse();
            double ang = sa;
            double dang = divideAngle;
            while ((ang - dang) < ea) {
                if (ea < ang)
                    ang = ea;
                List<Point3D> plist = outline.ConvertAll(p => p.toCopy());
                plist.ForEach(p => p.rotate(cv, ang));
                plist.ForEach(p => p.add(cp));
                outLines.Add(plist);
                ang += dang;
            }
            return outLines;
        }

        /// <summary>
        /// 線分と円弧の座標配列リストに変換
        /// 座標配列数 = 2 線分、 = 3 円弧
        /// </summary>
        /// <returns>座標配列リスト</returns>
        public List<Point3D[]> toLineArcList()
        {
            List<Point3D[]> lineArcList = new List<Point3D[]>();
            Point3D[] buf;
            for (int i = 0; i < mPolyline.Count - 1; i++) {
                if (i + 2 < mPolyline.Count && mPolyline[i + 1].type == 1) {
                    buf = new Point3D[3];
                    buf[0] = toPoint3D(i);
                    buf[1] = toPoint3D(i + 1);
                    buf[2] = toPoint3D(i + 2);
                    i++;
                } else if (i + 1 < mPolyline.Count && mPolyline[i + 1].type == 0) {
                    buf = new Point3D[2];
                    buf[0] = toPoint3D(i);
                    buf[1] = toPoint3D(i + 1);
                } else
                    continue;
                lineArcList.Add(buf);
            }
            return lineArcList;
        }
    }
}
