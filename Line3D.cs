using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// 3D 線分
    /// 
    /// Line3D()                                                        コンストラクタ
    /// Line3D(Point3D sp, Point3D ep)                                  コンストラクタ
    /// Line3D toCopy()                                                 コピーを作成
    /// string ToString(string form)                                    文字列に変換
    /// List<Point3D> toPoint3D()                                       座標点リストに変換
    /// LineD toLineD(FACE3D face)                                      LineDに変換する
    /// LineD toLineD(Point3D cp, Point3D u, Point3D v)                 LineDに変換する
    /// Point3D endPoint()                                              終点座標
    /// Point3D centerPoint()                                           中点
    /// double length()                                                 線分の長さ
    /// double length(Point3D pos)                                      指定点との距離
    /// void length(double l)                                           長さを設定
    /// void reverse()                                                  始終点を反転する
    /// Point3D intersection(Point3D p)                                 点との交点(垂点)
    /// Point3D intersection(PointD pos, FACE3D face)                   表示面で点と交わる位置の線分状の座標
    /// bool onPoint(Point3D p)                                         線上の点かを判断
    /// void translate(Point3D v)                                       移動する
    /// void rotate(Point3D cp, double ang, FACE3D face)                回転
    /// void offset(Point3D v)                                          オフセット
    /// List<Line3D> divide(PointD pos, FACE3D face)                    2D座標で分割(表示面で点と交わる座標で線分を分割)
    /// 
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
            double ang = line.ps.angle(line.pe, p, true);
            if (Math.PI / 2 < ang) l1 *= -1;
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

        /// <summary>
        /// 点を線分で反転した座標を求める
        /// </summary>
        /// <param name="pos">座標</param>
        /// <returns>反転座標</returns>
        public Point3D mirror(Point3D pos)
        {
            Point3D p = intersection(pos);
            Point3D v = p - pos;
            return p + v;
        }

        /// <summary>
        /// 線分を線分で反転した座標を求める
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>反転線分</returns>
        public Line3D mirror(Line3D line)
        {
            Point3D ps = mirror(line.mSp);
            Point3D pe = mirror(line.endPoint());
            return new Line3D(ps, pe);
        }

        /// <summary>
        /// ミラー
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(Point3D sp, Point3D ep)
        {
            Line3D l = new Line3D(sp, ep);
            mSp = l.mirror(mSp);
            l.mSp = new Point3D();
            mV = l.mirror(mV);
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        /// <param name="face"></param>
        public void trim(PointD sp, PointD ep, FACE3D face)
        {
            Point3D ps = intersection(sp, face);
            Point3D pe = intersection(ep, face);
            Line3D l = new Line3D(ps, pe);
            mSp = l.mSp;
            mV = l.mV;
        }


        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(Point3D sp, Point3D ep)
        {
            Point3D ps = intersection(sp);
            Point3D pe = intersection(ep);
            Line3D l = new Line3D(ps, pe);
            mSp = l.mSp;
            mV = l.mV;
        }

        /// <summary>
        /// 拡大縮小
        /// </summary>
        /// <param name="cp">拡大中心</param>
        /// <param name="scale">倍率</param>
        public void scale(Point3D cp, double scale)
        {
            Point3D ep = endPoint();
            ep.scale(cp, scale);
            Point3D sp = mSp.toCopy();
            sp.scale(cp, scale);
            Line3D l = new Line3D(sp, ep);
            mSp = l.mSp;
            mV = l.mV;
        }
    }
}
