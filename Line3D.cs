using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// 3D 線分
    /// </summary>
    public class Line3D
    {
        public Point3D mSp;
        public Point3D mV;

        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Line3D()
        {
            mSp = new Point3D();
            mV = new Point3D();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public Line3D(Point3D sp, Point3D ep)
        {
            mSp = sp.toCopy();
            mV = ep - sp;
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Line3D</returns>
        public Line3D toCopy()
        {
            return new Line3D(mSp, mSp + mV);
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <param name="form">書式</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            return $"{mSp.ToString(form)},{(mSp + mV).ToString(form)}";
        }

        /// <summary>
        /// 座標点リストに変換
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPoint3D()
        {
            return new List<Point3D>() {
                mSp, mSp + mV
            };
        }

        /// <summary>
        /// LineDに変換する
        /// </summary>
        /// <param name="face">表示面</param>
        /// <returns>2D線分</returns>
        public LineD toLineD(FACE3D face)
        {
            return new LineD(mSp.toPoint(face), (mSp + mV).toPoint(face));
        }

        /// <summary>
        /// LineDに変換する
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public LineD toLineD(Point3D cp, Point3D u, Point3D v)
        {
            PointD sp = Point3D.cnvPlaneLocation(mSp, cp, u, v);
            PointD ep = Point3D.cnvPlaneLocation((mSp + mV), cp, u, v);
            return new LineD(sp, ep);
        }


        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns>終点</returns>
        public Point3D endPoint()
        {
            return mSp + mV;
        }

        /// <summary>
        /// 中点
        /// </summary>
        /// <returns></returns>
        public Point3D centerPoint()
        {
            double l = mV.length();
            Point3D v = mV.toCopy();
            v.length(l / 2);
            return mSp + v;
        }

        /// <summary>
        /// 線分の長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            return mV.length();
        }

        /// <summary>
        /// 指定点との距離
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>長さ</returns>
        public double length(Point3D pos)
        {
            Point3D ip = intersection(pos);
            return ip.length(pos);
        }

        /// <summary>
        /// 長さを設定
        /// </summary>
        /// <param name="l">長さ</param>
        public void length(double l)
        {
            mV.length(l);
        }

        /// <summary>
        /// 始終点を反転する
        /// </summary>
        public void reverse()
        {
            mSp = mSp + mV;
            mV.inverse();
        }

        /// <summary>
        /// 点との交点(垂点)
        /// https://qiita.com/takenakadx/items/ca137088d3f897bc8b45
        /// u = mSp→p , t = v * u / |v|^2
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点</returns>
        public Point3D intersection(Point3D p)
        {
            Point3D v = mV.toCopy();
            double t = (v * (p - mSp)) / (v.length() * v.length());
            v = v * t;
            return mSp + v;
          }

        /// <summary>
        /// 表示面で点と交わる位置の線分状の座標を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">表示面</param>
        /// <returns>3D座標</returns>
        public Point3D intersection(PointD pos, FACE3D face)
        {
            LineD line = toLineD(face);
            PointD p = line.intersection(pos);
            double l1 = line.ps.length(p);
            Point3D v = mV.toCopy();
            double l = v.length();
            v.length(l * l1 / line.length());
            return mSp + v;
        }

        /// <summary>
        /// 線上の点かを判断
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>判定</returns>
        public bool onPoint(Point3D p)
        {
            Point3D ip = intersection(p);
            if (mEps < ip.length(p))
                return false;
            if (mV.length() < ip.length(mSp))
                return false;
            if (mV.length() < ip.length(mSp + mV))
                return false;
            return true;
        }

        /// <summary>
        /// 移動する
        /// </summary>
        /// <param name="v">移動ベクトル</param>
        public void translate(Point3D v)
        {
            mSp.add(v);
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角(rad)</param>
        /// <param name="face">表示面</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            mSp.rotate(cp, ang, face);
            mV.rotate(ang, face);
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="v"></param>
        public void offset(Point3D v)
        {
            Point3D p = mSp.toCopy();
            Line3D l = toCopy();
            l.translate(v);
            mSp = l.intersection(mSp);
        }

        /// <summary>
        /// 2D座標で分割(表示面で点と交わる座標で線分を分割)
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">表示面</param>
        /// <returns>線分リスト</returns>
        public List<Line3D> divide(PointD pos, FACE3D face)
        {
            Point3D p = intersection(pos, face);
            List<Line3D> lines = new List<Line3D> {
                new Line3D(mSp, p),
                new Line3D(p, mSp + mV)
            };
            return lines;
        }
    }
}
