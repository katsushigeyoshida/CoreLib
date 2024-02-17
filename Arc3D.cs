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
        public double mEa = Math.PI* 2;                    //  修了角

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
        }

        /// <summary>
        /// 
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
            double ang = mSa;
            while (ang < mEa) {
                plist.Add(getPosion(ang));
                ang += divAng;
            }
            plist.Add(getPosion(mEa));
            return plist;
        }

        /// <summary>
        /// ポリラインに変換する
        /// </summary>
        /// <param name="divAng"></param>
        /// <returns></returns>
        public Polyline3D toPolyline3D(double divAng = Math.PI / 20)
        {
            return new Polyline3D(toPoint3D(divAng));
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
        /// 終点側から座標点リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toReversePoint3D(double divAng = Math.PI / 20)
        {
            List<Point3D> plist = new List<Point3D>();
            double ang = mEa;
            while (ang > mSa) {
                plist.Add(getPosion(ang));
                ang -= divAng;
            }
            plist.Add(getPosion(mSa));
            return plist;
        }

        /// <summary>
        /// 始点座標
        /// </summary>
        /// <returns></returns>
        public Point3D startPosition()
        {
            return getPosion(mSa);
        }

        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns></returns>
        public Point3D endPosition()
        {
            return getPosion(mEa);
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
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(new Point3D(pos, face));
            PointD ip = arcD.intersection(p);
            double ang = arcD.getAngle(ip);
            //  分割した円弧を作成
            Arc3D arc = toCopy();
            arc.mEa = ang;
            arcs.Add(arc);
            arc = toCopy();
            arc.mSa = ang;
            arcs.Add(arc);
           return arcs;
        }

        /// <summary>
        /// 分割した座標点リストで最も近い座標゜
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>座標</returns>
        public PointD nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            List<PointD> plist = toPointD(divideNo, face);
            return plist.MinBy(p => p.length(pos));
        }

        /// <summary>
        /// 円周上の点
        /// </summary>
        /// <param name="ang">角度(rad)</param>
        /// <returns>座標</returns>
        public Point3D getPosion(double ang)
        {
            Point3D uv = (mU * Math.Cos(ang)) + (mV * Math.Sin(ang));
            return mCp + uv * mR;
        }

        /// <summary>
        /// 平面の座標を3D座標に変換
        /// P(u,v) = c + u * x + v * y
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
            if (a != 0) {
                t.x = (p.y * mV.x - p.x * mV.y) / a;
                t.y = (p.x * mU.y - p.y * mU.x) / a;
            } else if (b != 0) {
                t.x = (p.z * mV.y - p.y * mV.z) / b;
                t.y = (p.y * mU.z - p.z * mU.y) / b;
            } else {
                t.x = (p.x * mV.z - p.z * mV.x) / c;
                t.y = (p.z * mU.x - p.x * mU.z) / c;
            }
            return t;
        }
    }
}
