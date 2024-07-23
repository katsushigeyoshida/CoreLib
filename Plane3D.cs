
using System.Collections.Generic;
using System;

namespace CoreLib
{
    /// <summary>
    /// 三次元空間において１点を通り，２つの１次独立なベクトル，で張られる平面の方程式
    ///  ベクトル表示    Xv = x1v + s * l1v + t * l2v
    ///  媒介変数表示    x = x1 + s * l1 + t * l2
    ///                  y = y1 + s * m1 + t * m2
    ///                  z = z1 + s * n1 + t * n2
    ///  行列式表示      x - x1  y - y1  z - z1
    ///                   l1      m1      n1    = 0
    ///                   l2      m2      n2
    ///  参考: CAD・CG技術者のための実践NURBS
    /// </summary>
    public class Plane3D
    {
        public Point3D mCp = new Point3D(0, 0, 0);     //  中心座標
        public Point3D mU = new Point3D(1, 0, 0);      //  X軸の向き(中心から始点(0°)への方向単位ベクトル)
        public Point3D mV = new Point3D(0, 1, 0);      //  Y軸の向き(円の面でuに垂直な方向の単位ベクトル)

        private double mEps = 1E-8;

        public Plane3D() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">基準座標</param>
        /// <param name="u">X軸の向き</param>
        /// <param name="v">Y軸のの方向</param>
        public Plane3D(Point3D cp, Point3D u, Point3D v)
        {
            mCp = cp.toCopy();
            mU = u.toCopy();
            mV = v.toCopy();
            mU.unit();
            mV.unit();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plist">座標リスト</param>
        public Plane3D(List<Point3D> plist)
        {
            mCp = plist[0];
            (mU, mV) = getFace(plist);
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
                v = mU.toCopy();
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
        /// 2D平面の座標を3D座標に変換
        ///  P(mU,mV) = c + mU * x + mV * y
        /// </summary>
        /// <param name="p">2D座標</param>
        /// <returns>3D座標</returns>
        public Point3D cnvPlaneLocation(PointD p)
        {
            Point3D uv = mU * p.x + mV * p.y;
            return mCp + uv;
        }

        /// <summary>
        /// 2D平面上の3D座標から平面座標に変換
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>2D座標</returns>
        public PointD cnvPlaneLocation(Point3D pos)
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

        /// <summary>
        /// 点座標と平面との垂点
        /// pn = p - (w・n)・n     (w = p - mCp, n = mU・mV)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点座標</returns>
        public Point3D intersection(Point3D p)
        {
            Point3D w = p - mCp;                    //  平面の原点と点のベクトル
            Point3D n = mU.crossProduct(mV);        //  平面の法線
            Point3D pn = mCp - (w.crossProduct(n)).crossProduct(n);
            // Point3D pn = mCp - n * (n * w);
            return pn;
        }

        /// <summary>
        /// 点座標と平面との垂点を平面上の座標に変換
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>2D座標</returns>
        public PointD intersection2D(Point3D p)
        {
            return cnvPlaneLocation(intersection(p));
        }

        /// <summary>
        /// face上の点座標を平面に投影したときの交点座標
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点座標</returns>
        public Point3D intersection(PointD p, FACE3D face)
        {
            Line3D line = new Line3D(new Point3D(p, face), new Point3D(p, face, 1));
            return intersection(line);
        }

        /// <summary>
        /// 線分と平面との交点
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns>交点</returns>
        public Point3D intersection(Line3D l)
        {
            Point3D p = l.mSp.toCopy();
            Point3D t = l.mV.toCopy();
            t.unit();
            Point3D w = p - mCp;                    //  平面の原点と線分の端点とのベクトル
            Point3D n = mU.crossProduct(mV);        //  平面の法線
            double wn = w * n;
            double tn = t * n;
            if (Math.Abs(tn) < mEps) return null;
            return p - t * (wn / tn);
        }

        /// <summary>
        /// 平行確認
        /// </summary>
        /// <param name="plane">平面</param>
        /// <returns>平行</returns>
        public bool isParallel(Plane3D plane)
        {
            Point3D n0 = mU.crossProduct(mV);
            Point3D n1 = plane.mU.crossProduct(plane.mV);
            double l = n0.crossProduct(n1).length();
            return l < mEps;
        }

        /// <summary>
        /// 平行距離
        /// </summary>
        /// <param name="plane">平面</param>
        /// <returns>距離</returns>
        public double parallelLength(Plane3D plane)
        {
            Point3D ip = intersection(plane.mCp);
            return ip.length(plane.mCp);
        }
    }
}
