using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// スプライン曲線
    /// 非周期 3次スプライン補間(Cube spline interpolation)
    /// (xi,yi) (x0 < x1 < x2 .... < xn-1 )を通る区分的3次式で補間する
    /// 各3次式のつなぎ目 x1,x2....xn-2 では2次導関数まで連続
    /// 端点処理は両端点で2次導関数を０とする
    /// </summary>
    public class Spline
    {
        private List<Point3D> mPoints = new List<Point3D>();

        /// <summary>
        /// 3次スプライン(xが一様増加時に摘要)
        /// </summary>
        /// <param name="points">座標リスト</param>
        public Spline(List<PointD> points)
        {
            int n = points.Count;
            for (int i = 0; i < n; i++) {
                mPoints.Add(new Point3D(points[i].x, points[i].y, 0));
            }
            //  yの増加率を求める(微分)
            double[] h = new double[n];
            double[] d = new double[n];
            mPoints[0].z = mPoints[n - 1].z = 0;    //  自然条件(非周期)
            for (int i = 0; i < n - 1; i++) {
                h[i]     = mPoints[i + 1].x - mPoints[i].x;
                d[i + 1] = (mPoints[i + 1].y - mPoints[i].y) / h[i];
            }
            //  2階微分値を求める
            mPoints[1].z = d[2] - d[1] - h[0] * mPoints[0].z;
            d[1]         = 2 * (mPoints[2].x - mPoints[0].x);
            for (int i = 1; i < n - 2; i++) {
                double t = h[i] / d[i];
                mPoints[i + 1].z = d[i + 2] - d[i + 1] - mPoints[i].z * t;
                d[i + 1]         = 2 * (mPoints[i + 2].x - mPoints[i].x) - h[i] * t;
            }
            mPoints[n - 2].z -= h[n - 2] * mPoints[n - 1].z;
            for (int i = n - 2; i > 0; i--) {
                mPoints[i].z = (mPoints[i].z - h[i] * mPoints[i + 1].z) / d[i];
            }
        }

        /// <summary>
        /// 補間点の抽出
        /// fj(x) = aj (x - xj)^3 + bj (x - xj)^2 + cj (x - xj) + dj
        /// aj = 
        /// bj = y''j / 2
        /// cj = 
        /// dj = yj
        /// </summary>
        /// <param name="t">補間x座標(x0 ～ xn)</param>
        /// <returns>補間y座標</returns>
        public double interpolate(double t)
        {
            int i = 0;
            int j = mPoints.Count - 1;
            while (i < j) {
                int k = (i + j) / 2;
                if (mPoints[k].x < t)
                    i = k + 1;
                else
                    j = k;
            }
            if (i > 0)
                i--;
            double h = mPoints[i + 1].x - mPoints[i].x;
            double d = t - mPoints[i].x;
            return
                (((mPoints[i + 1].z - mPoints[i].z) * d / h
                + mPoints[i].z * 3) * d
                + ((mPoints[i + 1].y - mPoints[i].y) / h - (mPoints[i].z * 2 + mPoints[i + 1].z) * h)) * d
                + mPoints[i].y;
        }
    }

    /// <summary>
    /// スプライン曲線
    /// 3次スプライン補間(Cube spline interpolation)
    /// y座標がx座標の1価関数関数でない時(開曲線)
    /// 媒介変数を使ってx,y座標を別々にスプライン関数にする
    /// </summary>
    public class Spline2
    {
        private List<Point3D> mPoints = new List<Point3D>();
        private Spline sx, sy;

        public Spline2(List<PointD> points)
        {
            int n = points.Count;
            mPoints.Add(new Point3D(points[0].x, points[0].y, 0));
            //  媒介変数の値(距離)を求める
            for (int i = 1; i < n; i++) {
                double t1 = points[i].x - points[i - 1].x;
                double t2 = points[i].y - points[i - 1].y;
                double p = mPoints[i - 1].z + Math.Sqrt(t1 * t1 + t2 * t2);
                mPoints.Add(new Point3D(points[i].x, points[i].y, p));
            }
            for (int i = 1; i < n; i++) {
                mPoints[i].z /= mPoints[n - 1].z;
            }
            List<PointD> xpoints = new List<PointD>();
            List<PointD> ypoints = new List<PointD>();
            for (int i = 0; i < mPoints.Count; i++) {
                xpoints.Add(new PointD(mPoints[i].z, mPoints[i].x));
                ypoints.Add(new PointD(mPoints[i].z, mPoints[i].y));
            }
            sx = new Spline(xpoints);
            sy = new Spline(ypoints);
        }

        /// <summary>
        /// 補間点の抽出
        /// </summary>
        /// <param name="t">媒介変数(0 ～1)</param>
        /// <returns>補間座標</returns>
        public PointD interpolate(double t)
        {
            return new PointD(sx.interpolate(t), sy.interpolate(t));
        }
    }

    /// <summary>
    /// 周期の3次スプライン関数 (xn - x0の周期関数)
    /// </summary>
    public class PSpline
    {
        private List<Point3D> mPoints = new List<Point3D>();

        public PSpline(List<PointD> points)
        {
            for (int i = 0; i < points.Count; i++)
                mPoints.Add(new Point3D(points[i].x, points[i].y, 0));
            List<double> h = new List<double>();
            List<double> d = new List<double>();
            List<double> w = new List<double>();
            for (int i = 0; i < mPoints.Count - 1; i++) {
                d.Add(0);
                h.Add(mPoints[i + 1].x - mPoints[i].x);
                w.Add((mPoints[i + 1].y - mPoints[i].y) / h[i]);
            }
            d.Add(0);
            h.Add(0);
            w.Add(w[0]);
            int n = mPoints.Count - 1;
            for (int i = 1; i < n; i++)
                d[i] = 2 * (mPoints[i + 1].x - mPoints[i - 1].x);
            d[n] = 2 * (h[n - 1] + h[0]);
            for (int i = 1; i <= n; i++)
                mPoints[i].z = w[i] - w[i - 1];
            w[1]     = h[0];
            w[n - 1] = h[n - 1];
            w[n]     = d[n];
            for (int i = 2; i < n - 1; i++)
                w[i] = 0;
            for (int i = 1; i <  n; i++) {
                double t = h[i] / d[i];
                mPoints[i + 1].z -= mPoints[i].z * t;
                d[i + 1]         -= h[i]         * t;
                w[i + 1]         -= w[i]         * t;
            }
            w[0] = w[n];
            mPoints[0].z = mPoints[n].z;
            for (int i = n - 2; i >= 0; i--) {
                double t = h[i] / d[i + 1];
                mPoints[i].z -= mPoints[i + 1].z * t;
                w[i]         -= w[i + 1]         * t;
            }
            double tmp = mPoints[0].z / w[0];
            mPoints[0].z = mPoints[n].z = tmp;
            for (int i = 1; i < n; i++)
                mPoints[i].z = (mPoints[i].z - w[i] * tmp) / d[i];
        }

        /// <summary>
        /// 補間点の抽出
        /// </summary>
        /// <param name="t">媒介変数(0 ～1)</param>
        /// <returns>補間y座標</returns>
        public double interpolate(double t)
        {
            int n = mPoints.Count - 1;
            double period = mPoints[n].x - mPoints[0].x;
            while (t > mPoints[n].x)
                t -= period;
            while (t < mPoints[0].x)
                t += period;
            int i = 0;
            int j = n;
            while (i < j) {
                int k = (i + j) / 2;
                if (mPoints[k].x < t)
                    i = k + 1;
                else
                    j = k;
            }
            if (i > 0)
                i--;
            double h = mPoints[i + 1].x - mPoints[i].x;
            double d = t - mPoints[i].x;
            return
                (((mPoints[i + 1].z - mPoints[i].z) * d / h
                + mPoints[i].z * 3) * d
                + ((mPoints[i + 1].y - mPoints[i].y) / h - (mPoints[i].z * 2 + mPoints[i + 1].z) * h)) * d
                + mPoints[i].y;
        }
    }

    /// <summary>
    /// 周期の3次スプライン関数(閉曲線)
    /// </summary>
    public class PSpline2
    {
        private List<Point3D> mPoints = new List<Point3D>();
        private PSpline sx, sy;

        public PSpline2(List<PointD> points)
        {
            for (int i = 0; i < points.Count; i++) {
                mPoints.Add(new Point3D(points[i].x, points[i].y, 0));
            }
            mPoints.Add(new Point3D(points[0].x, points[0].y, 0));
            //  媒介変数(隣り合う座標間の距離)を求める
            double t1, t2, p;
            for (int i = 1; i < mPoints.Count; i++) {
                t1 = mPoints[i].x - mPoints[i - 1].x;
                t2 = mPoints[i].y - mPoints[i - 1].y;
                p = mPoints[i - 1].z + Math.Sqrt(t1 * t1 + t2 * t2);
                mPoints[i].z = p;
            }
            for (int i = 1; i < mPoints.Count; i++) {
                mPoints[i].z /= mPoints[mPoints.Count - 1].z;
            }
            List<PointD> xpoints = new List<PointD>();
            List<PointD> ypoints = new List<PointD>();
            for (int i = 0; i < mPoints.Count;i++) {
                xpoints.Add(new PointD(mPoints[i].z, mPoints[i].x));
                ypoints.Add(new PointD(mPoints[i].z, mPoints[i].y));
            }
            sx = new PSpline(xpoints);
            sy = new PSpline(ypoints);
        }

        /// <summary>
        /// 補間点の抽出
        /// </summary>
        /// <param name="t">媒介変数(0 ～1)</param>
        /// <returns>補間座標</returns>
        public PointD interpolate(double t)
        {
            return new PointD(sx.interpolate(t), sy.interpolate(t));
        }
    }
}
