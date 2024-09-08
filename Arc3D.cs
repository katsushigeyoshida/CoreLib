using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    /// <summary>
    /// 3Dの円弧データ(3次元平面上の2次元の円弧)
    /// </summary>
    public class Arc3D
    {
        public Point3D mCp =new Point3D();                 //  中心座標
        public Point3D mU = new Point3D(1, 0, 0);          //  X軸の向き(中心から始点(0°)への方向単位ベクトル)
        public Point3D mV = new Point3D(0, 1, 0);          //  Y軸の向き(円の面でuに垂直な方向の単位ベクトル)
        public double mR = 1;                              //  半径
        public double mSa = 0;                             //  開始角
        public double mEa = Math.PI * 2;                   //  修了角
        public bool mCcw = true;                            //  3点円弧の座標順

        public double mOpenAngle { get { return mEa - mSa; } }
        private double mEps = 1E-8;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Arc3D()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        public Arc3D(Point3D cp, double r)
        {
            mCp = cp.toCopy();
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="u">X軸の向き</param>
        /// <param name="v">Y軸の向き</param>
        public Arc3D(Point3D cp, double r, Point3D u, Point3D v)
        {
            mCp = cp.toCopy();
            mU = u.toCopy();
            mU.unit();
            mV = v.toCopy();
            mV.unit();
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="face">作成面</param>
        public Arc3D(Point3D cp, double r, FACE3D face)
        {
            mCp = cp.toCopy();
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角</param>
        /// <param name="ea">週旅客</param>
        /// <param name="face">作成面</param>
        public Arc3D(Point3D cp, double r, double sa, double ea, FACE3D face)
        {
            mCp = cp.toCopy();
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
            mR = r;
            mSa = sa;
            mEa = ea;
            normalize();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arc">2D円弧データ</param>
        /// <param name="face">作成面</param>
        public Arc3D(ArcD arc, FACE3D face)
        {
            mCp = new Point3D(arc.mCp, face);
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
            normalize();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="mp">中点</param>
        /// <param name="ep">終点</param>
        public Arc3D(Point3D sp, Point3D mp, Point3D ep)
        {
            List<Point3D> plist = new List<Point3D>() {
                sp, mp, ep
            };
            Plane3D plane = new Plane3D(plist);
            mU = plane.mU;
            mV = plane.mV;
            PointD sp2 = plane.cnvPlaneLocation(plist[0]);
            PointD mp2 = plane.cnvPlaneLocation(plist[1]);
            PointD ep2 = plane.cnvPlaneLocation(plist[2]);
            ArcD arc = new ArcD(sp2, mp2, ep2);
            mCp = plane.cnvPlaneLocation(arc.mCp);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
            mCcw = arc.mCcw;
            normalize();
        }

        /// <summary>
        /// 開始角と修了角の正規化
        /// 0 <= 開始角 < 2π, 開始角 <= 修了角 < 4π
        /// </summary>
        public void normalize()
        {
            mSa = ylib.mod(mSa, Math.PI * 2);
            mEa = ylib.mod(mEa, Math.PI * 2);
            if (mEa <= mSa + mEps)
                mEa += Math.PI * 2;
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Arc3D</returns>
        public Arc3D toCopy()
        {
            Arc3D arc = new Arc3D(mCp, mR, mU, mV);
            arc.mSa = mSa;
            arc.mEa = mEa;
            return arc;
        }

        /// <summary>
        /// 円弧データをコピーする
        /// </summary>
        /// <param name="arc">円弧</param>
        public void setArc(Arc3D arc)
        {
            mCp = arc.mCp.toCopy();
            mU = arc.mU.toCopy();
            mV = arc.mV.toCopy();
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
        }

        /// <summary>
        /// 分割座標点リストに変換
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPoint3D(int divNo)
        {
            return toPoint3D((mEa - mSa) / divNo);
        }

        /// <summary>
        /// 分割座標点リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPoint3D(double divAng = Math.PI / 20)
        {
            List<Point3D> plist = new List<Point3D>();
            plist.Add(getPosition(mSa));
            if (0 < divAng) {
                double ang = mSa + divAng;
                while (ang < mEa) {
                    plist.Add(getPosition(ang));
                    ang += divAng;
                }
            } else {
                double ang = (mSa + mEa) / 2;
                Point3D p = getPosition(ang);
                p.type = 1;
                plist.Add(p);
            }
            plist.Add(getPosition(mEa));
            return plist;
        }

        /// <summary>
        /// ポリラインに変換する
        /// </summary>
        /// <param name="divAng"></param>
        /// <returns></returns>
        public Polyline3D toPolyline3D(double divAng = Math.PI / 20)
        {
            Polyline3D polyline = new Polyline3D();
            polyline.mPolyline = toPointD(divAng);
            polyline.mCp = mCp.toCopy();
            polyline.mU = mU.toCopy();
            polyline.mV = mV.toCopy();
            return polyline;
        }

        /// <summary>
        /// 2D座標リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD(double divAng = Math.PI / 20)
        {
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            if (0 < divAng)
                return arc.toPointList(divAng);
            else
                return arc.to3PointList();
        }

        /// <summary>
        /// 分割座標点リストを2D平面に変換
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>座標点リスト</returns>
        public List<PointD> toPointD(int divNo, FACE3D face)
        {
            return toPointD((mEa - mSa) / divNo, face);
        }

        /// <summary>
        /// 2D平面の楕円に変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2D楕円</returns>
        public EllipseD toEllipseD(FACE3D face)
        {
            EllipseD ellipse = new EllipseD();
            ellipse.mCp = mCp.toPoint(face);
            PointD sp = getPosition(0).toPoint(face);
            PointD ep = getPosition(Math.PI/ 2).toPoint(face);
            ellipse.mRx = ellipse.mCp.length(sp);
            ellipse.mRy = ellipse.mCp.length(ep);
            ellipse.mRotate = sp.angle(ellipse.mCp);
            ellipse.mSa = mSa;
            ellipse.mEa = mEa;
            return ellipse;
        }

        /// <summary>
        /// 分割座標点リストを2D平面に変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD(double divAng = Math.PI / 20, FACE3D face = FACE3D.XY)
        {
            List<Point3D> plist = toPoint3D(divAng);
            return plist.ConvertAll(p => p.toPoint(face));
        }

        /// <summary>
        /// 円周上の端点リスト(端点+4分割点)
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPeackList()
        {
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            List<PointD> plist = arc.toPeakList();
            return plist.ConvertAll(p => cnvPosition(p));
        }

        /// <summary>
        /// 終点側から座標点リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toReversePoint3D(double divAng = Math.PI / 20)
        {
            List<Point3D> plist = new List<Point3D>();
            double ang = mEa;
            while (ang > mSa) {
                plist.Add(getPosition(ang));
                ang -= divAng;
            }
            plist.Add(getPosition(mSa));
            return plist;
        }

        /// <summary>
        /// 始点座標
        /// </summary>
        /// <returns></returns>
        public Point3D startPosition()
        {
            return getPosition(mSa);
        }

        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns></returns>
        public Point3D endPosition()
        {
            return getPosition(mEa);
        }

        /// <summary>
        /// 中間点の座標
        /// </summary>
        /// <returns></returns>
        public Point3D midPosition()
        {
            return getPosition((mSa + mEa) / 2);
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
        /// 反転
        /// </summary>
        /// <param name="line">反転基準線</param>
        /// <param name="face">2D平面</param>
        public void mirror(Line3D line, FACE3D face)
        {
            Point3D cp = line.mirror(mCp, face);
            Point3D u = line.mirror(mCp + mU, face);
            Point3D v = line.mirror(mCp + mV, face);
            mCp = cp.toCopy();
            mU = u - cp;
            mV = v - cp;
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(Point3D sp, Point3D ep)
        {
            mR += ep.length(mCp) - sp.length(mCp);
        }

        /// <summary>
        /// 2D位置での分割(ポリラインにして近似的に分割角度を求める)
        /// </summary>
        /// <param name="pos">2D分割位置</param>
        /// <param name="face">作成面</param>
        /// <returns>円弧リスト</returns>
        public List<Arc3D> divide(PointD pos, FACE3D face)
        {
            List<Arc3D> arcs = new List<Arc3D>();
            //  2D平面上で円弧に対する交点角度を求める
            double ang = intersectionAngle(pos, face);
            //  分割した円弧を作成
            Arc3D arc = toCopy();
            arc.mEa = ang;
            arc.normalize();
            arcs.Add(arc);
            arc = toCopy();
            arc.mSa = ang;
            arc.normalize();
            arcs.Add(arc);
           return arcs;
        }

        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="pos">分割位置</param>
        /// <returns>円弧リスト</returns>
        public List<Arc3D> divide(Point3D pos)
        {
            List<Arc3D> arcs = new List<Arc3D>();
            //  2D平面上で円弧に対する交点角度を求める
            double ang = intersectionAngle(pos);
            //  分割した円弧を作成
            Arc3D arc = toCopy();
            arc.mEa = ang;
            arc.normalize();
            arcs.Add(arc);
            arc = toCopy();
            arc.mSa = ang;
            arc.normalize();
            arcs.Add(arc);
            return arcs;
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        /// <param name="face">2D平面</param>
        public void trim(PointD sp, PointD ep, FACE3D face)
        {
            mSa = intersectionAngle(sp, face);
            mEa = intersectionAngle(ep, face);
            normalize();
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(Point3D sp, Point3D ep)
        {
            mSa = intersectionAngle(sp);
            mEa = intersectionAngle(ep);
            System.Diagnostics.Debug.WriteLine($"{sp.ToString("F2")} {ep.ToString("F2")} {mSa.ToString("F2")} {mEa.ToString("F2")}");
            normalize();
        }

        /// <summary>
        /// ストレッチ
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        /// <param name="pickPos">ピック位置</param>
        public void stretch(Point3D vec, Point3D pickPos)
        {
            PointD svec = vec.toPointD(new Point3D(0, 0, 0), mU, mV);
            PointD ppos = pickPos.toPointD(mCp, mU, mV);
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            arc.stretch(svec, ppos);
            mCp = Point3D.cnvPlaneLocation(arc.mCp, mCp, mU, mV);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
        }

        /// <summary>
        /// 交点を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>3D座標</returns>
        public Point3D intersection(PointD pos, FACE3D face)
        {
            Plane3D plane = new Plane3D(mCp, mU, mV);
            Point3D p = plane.intersection(pos, face);
            return intersection(p);
        }

        /// <summary>
        /// 交点を求める
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>交点座標</returns>
        public Point3D intersection(Point3D pos)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(pos);
            PointD ip = arcD.intersection(p);
            return cnvPosition(ip);
        }

        /// <summary>
        /// 交点の円上の角度を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>角度</returns>
        public double intersectionAngle(PointD pos, FACE3D face)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(new Point3D(pos, face));
            PointD ip = arcD.intersection(p);
            return arcD.getAngle(ip);
        }

        /// <summary>
        /// 交点の円上の角度
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>角度</returns>
        public double intersectionAngle(Point3D pos)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(pos);
            PointD ip = arcD.intersection(p);
            return arcD.getAngle(ip);
        }

        /// <summary>
        /// 分割した座標点リストで最も近い座標゜
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>座標</returns>
        public Point3D nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            List<PointD> plist = toPointD(divideNo, face);
            PointD p = plist.MinBy(p => p.length(pos));
            return intersection(p, face);
        }

        /// <summary>
        /// 分割した座標点リストで最も近い座標
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public Point3D nearPoint(Point3D pos, int divideNo)
        {
            List<Point3D> plist = toPoint3D(divideNo);
            return plist.MinBy(p => p.length(pos));
        }

        /// <summary>
        /// 円周上の点
        /// </summary>
        /// <param name="ang">角度(rad)</param>
        /// <returns>座標</returns>
        public Point3D getPosition(double ang)
        {
            Point3D uv = (mU * Math.Cos(ang)) + (mV * Math.Sin(ang));
            return mCp + uv * mR;
        }

        /// <summary>
        /// 平面の座標を3D座標に変換
        /// Plane3D.cnvPlaneLocationと同じ
        /// P(mU,mV) = c + mU * x + mV * y
        /// </summary>
        /// <param name="p">2D座標</param>
        /// <returns>3D座標</returns>
        public Point3D cnvPosition(PointD p)
        {
            Point3D uv = mU * p.x + mV * p.y;
            return mCp + uv;
        }

        /// <summary>
        /// 3D座標から平面座標に変換
        /// Plane3D.cnvPlaneLocationと同じ
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>2D座標</returns>
        public PointD cnvPosition(Point3D pos)
        {
            PointD t = new PointD();
            Point3D p = pos - mCp;
            double a = (mU.y * mV.x - mU.x * mV.y);
            double b = (mU.z * mV.y - mU.y * mV.z);
            double c = (mU.x * mV.z - mU.z * mV.x);
            double A = Math.Abs(a);
            double B = Math.Abs(b);
            double C = Math.Abs(c);
            if (B < A && C < A) {
                t.x = (p.y * mV.x - p.x * mV.y) / a;
                t.y = (p.x * mU.y - p.y * mU.x) / a;
            } else if (C < B && A < B) {
                t.x = (p.z * mV.y - p.y * mV.z) / b;
                t.y = (p.y * mU.z - p.z * mU.y) / b;
            } else if (A < C && B < C) {
                t.x = (p.x * mV.z - p.z * mV.x) / c;
                t.y = (p.z * mU.x - p.x * mU.z) / c;
            }
            return t;
        }
    }
}
