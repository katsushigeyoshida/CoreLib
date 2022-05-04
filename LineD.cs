using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace CoreLib
{
    /// <summary>
    /// 実数(double)のLineクラス
    /// 
    ///  LineD(double psx, double psy, double pex, double pey)
    ///  LineD(Point ps, Point pe)
    ///  LineD(PointD ps, PointD pe)
    ///  LineD(Line l)
    ///  LineD(LineD l)
    ///  LineD(Point ps, double r, double th)
    ///  
    ///  override string ToString()
    ///  Line toLine()
    ///  PointD vector()
    ///  Rect toRect()
    ///  double length()
    ///  void setLength(double l)
    ///  double angle()
    ///  double angle(LineD l)
    ///  bool isParalell(LineD l)
    ///  double pointDistance(Point p)          点との垂線の距離
    ///  double pointDistance(PointD p)
    ///  Point intersectPoint(Point p)          点からの垂線の交点座標(垂点)
    ///  PointD intersectPoint(PointD p)
    ///  List<Point> intersection(Point c, double r, double sa = 0.0, double ea = Math.PI * 2.0)    円との交点リスト
    ///  List<PointD> intersection(PointD c, double r, double sa = 0.0, double ea = Math.PI * 2.0)
    ///  bool intersectionHorizon(Point p)      指定点の水平線と交点を持つかをチェック
    ///  bool intersectionHorizon(PointD p)
    ///  PointD intersectHorizonPoint(Point p)   指定点の水平線との交点(延長線上も含む)
    ///  PointD intersectHorizonPoint(PointD p)
    ///  PointD intersection(LineD l)
    ///  bool lineOnPoint(PointD pnt)
    ///  byte inOutAreaCode(PointD p, Rect rect)
    ///  byte inOutAreaCode(Point p, Rect rect)
    ///  LineD clippingLine(Rect rect)
    ///  void move(double dx, double dy)
    ///  void moveLength(double r,double th)
    ///  void moveLength(double l)
    ///  void rotate(Point ctr, double rotate)
    ///  void rotate(PointD ctr, double rotate)
    ///  bool innerAngle(double sa, double ea, double ang)
    /// </summary>
    public class LineD
    {
        private YLib ylib = new YLib();

        public PointD ps;
        public PointD pe;
        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="psx">始点X座標</param>
        /// <param name="psy">始点Y座標</param>
        /// <param name="pex">終点X座標</param>
        /// <param name="pey">終点Y座標</param>
        public LineD(double psx, double psy, double pex, double pey)
        {
            ps = new PointD(psx, psy);
            pe = new PointD(pex, pey);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public LineD(Point ps, Point pe)
        {
            this.ps = new PointD(ps.X, ps.Y);
            this.pe = new PointD(pe.X, pe.Y);
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public LineD(PointD ps, PointD pe)
        {
            this.ps = ps.toCopy();
            this.pe = pe.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="l">Lineデータ(Winodws.Shapes)</param>
        public LineD(Line l)
        {
            ps.x = l.X1;
            ps.y = l.Y1;
            pe.x = l.X2;
            pe.y = l.Y2;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="l">Lineデータ(Winodws.Shapes)</param>
        public LineD(LineD l)
        {
            ps.x = l.ps.x;
            ps.y = l.ps.y;
            pe.x = l.pe.x;
            pe.y = l.pe.y;
        }

        /// <summary>
        /// コンストラクタ　開始点と長さと角度を指定する
        /// </summary>
        /// <param name="ps">開始点</param>
        /// <param name="r">長さ</param>
        /// <param name="th">角度(rad)</param>
        public LineD(Point ps, double r, double th)
        {
            this.ps = new PointD(ps);
            pe.x = ps.X + r * Math.Cos(th);
            pe.y = ps.Y + r * Math.Sin(th);
        }

        public override string ToString()
        {
            return ps.ToString() + "," + pe.ToString();
        }

        /// <summary>
        /// Lineデータ(Winodws.Shapes)に変換
        /// </summary>
        /// <returns>Lineデータ(Winodws.Shapes)</returns>
        public Line toLine()
        {
            Line l = new Line();
            l.X1 = ps.x;
            l.Y1 = ps.y;
            l.X2 = pe.x;
            l.Y2 = pe.y;
            return l;
        }

        /// <summary>
        /// ベクトル(増分データ)に変換
        /// </summary>
        /// <returns>増分データ</returns>
        public PointD vector()
        {
            return new PointD(pe.x - ps.x, pe.y - ps.y);
        }

        /// <summary>
        /// Rectに変換
        /// </summary>
        /// <returns>Rectデータ</returns>
        public Rect toRect()
        {
            return new Rect(ps.toPoint(), pe.toPoint());
        }

        /// <summary>
        /// 線分の長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            PointD v = vector();
            return Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// 線分の長さを再設定する
        /// </summary>
        /// <param name="l">線分の長さ</param>
        public void setLength(double l)
        {
            double th = angle();
            pe.x = ps.x + l * Math.Cos(th);
            pe.y = ps.y + l * Math.Sin(th);
        }

        /// <summary>
        /// ベクトルとしての角度(-π ～ π)
        /// </summary>
        /// <returns>回転角</returns>
        public double angle()
        {
            PointD v = vector();
            return Math.Atan2(v.y, v.x);
        }

        /// <summary>
        /// 2線分の角度(0 ～ π)
        /// </summary>
        /// <param name="l">対象線分</param>
        /// <returns>線分同士の角度</returns>
        public double angle(LineD l)
        {
            return Math.Abs(angle() - l.angle());
        }

        /// <summary>
        /// 平行線の判定
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public bool isParalell(LineD l)
        {
            PointD v1 = vector();
            PointD v2 = l.vector();
            return Math.Abs(v1.x * v2.y - v2.x * v1.y) < mEps;
        }

        /// <summary>
        /// 点との垂線の距離
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>距離</returns>
        public double pointDistance(Point p)
        {
            double dx = ps.x - p.X;
            double dy = ps.y - p.Y;
            double l = Math.Sqrt(dx * dx + dy * dy);
            LineD lp = new LineD(ps, new PointD(p));
            return l * Math.Sin(angle(lp));
        }

        /// <summary>
        /// 点との垂線の距離
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>距離</returns>
        public double pointDistance(PointD p)
        {
            double dx = ps.x - p.x;
            double dy = ps.y - p.y;
            double l = Math.Sqrt(dx * dx + dy * dy);
            LineD lp = new LineD(ps, p);
            return l * Math.Sin(angle(lp));
        }

        /// <summary>
        /// 点からの垂線の交点座標(垂点)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点座標</returns>
        public Point intersectPoint(Point p)
        {
            double dx = ps.x - p.X;
            double dy = ps.y - p.Y;
            double l = Math.Sqrt(dx * dx + dy * dy);
            LineD lp = new LineD(ps, new PointD(p));
            double ll =l *  Math.Cos(angle(lp));
            double a = angle();
            return new Point(ll * Math.Cos(a) + ps.x, ll * Math.Sin(a) + ps.y);
        }

        /// <summary>
        /// 点からの垂線の交点座標(垂点)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点座標</returns>
        public PointD intersectPoint(PointD p)
        {
            double dx = ps.x - p.x;
            double dy = ps.y - p.y;
            double l = Math.Sqrt(dx * dx + dy * dy);
            LineD lp = new LineD(ps, p);
            double ll = l * Math.Cos(angle(lp));
            double a = angle();
            return new PointD(ll * Math.Cos(a) + ps.x, ll * Math.Sin(a) + ps.y);
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">円の半径</param>
        /// <returns>交点リスト</returns>
        public List<Point> intersection(Point c, double r, double sa = 0.0, double ea = Math.PI * 2.0)
        {
            List<PointD> pointList = intersection(new PointD(c), r, sa, ea);
            List<Point> pList = new List<Point>();
            foreach (PointD p in pointList)
                pList.Add(p.toPoint());
            return pList;
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">円の半径</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PointD c, double r, double sa = 0.0, double ea = Math.PI * 2.0)
        {
            List<PointD> pointList = new List<PointD>();
            PointD mp = intersectPoint(c);               //  線分との垂点
            double l = pointDistance(c);                //  垂点と中心点との距離
            if (ea < sa) {
                ea += Math.PI * 2.0;
            }
            if (r < l) {
                //  交点なし
            } else if (Math.Abs(r - l) < mEps) {
                //  接点
                //if (ylib.innerAngle(sa, ea, ylib.anglePoint(c, mp)))
                if (innerAngle(sa, ea, mp.angle(c)))
                    pointList.Add(mp);
            } else {
                //  交点
                double th = Math.Acos(l / r);           //  垂線との角度
                LineD ml = new LineD(c, mp);
                double a = ml.angle();
                double ang = (a + th + Math.PI * 2.0) % (Math.PI * 2.0);
                if (ea < sa) {
                    ea += Math.PI * 2.0;
                    ang += ang < sa ? Math.PI * 2.0 : 0.0;
                }
                if (sa <= ang && ang <= ea) {
                    PointD p1 = new PointD(c.x + r * Math.Cos(ang), c.y + r * Math.Sin(ang));
                    if (lineOnPoint(p1))
                        pointList.Add(p1);
                }
                ang = (a - th + Math.PI * 2.0) % (Math.PI * 2.0);
                if (sa <= ang && ang <= ea) {
                    PointD p2 = new PointD(c.x + r * Math.Cos(a - th), c.y + r * Math.Sin(a - th));
                    if (lineOnPoint(p2))
                        pointList.Add(p2);
                }
            }
            return pointList;
        }

        /// <summary>
        /// 指定点の水平線と交点を持つかをチェック
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点あり(true)</returns>

        public bool intersectionHorizon(Point p) 
        {
            return intersectionHorizon(new PointD(p));
        }

        /// <summary>
        /// 指定点の水平線と交点を持つかをチェック
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点あり(true)</returns>
        public bool intersectionHorizon(PointD p)
        {
            if ((ps.y < pe.y && (ps.y <= p.y && p.y <= pe.y)) ||
                (pe.y < ps.y && (pe.y <= p.y && p.y <= ps.y)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 指定点の水平線との交点(延長線上も含む)
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点座標</returns>
        public PointD intersectHorizonPoint(Point p)
        {
            return intersectHorizonPoint(new PointD(p));
        }

        /// <summary>
        /// 指定点の水平線との交点(延長線上も含む)
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点座標</returns>
        public PointD intersectHorizonPoint(PointD p)
        {
            PointD v = vector();
            v.x = v.x * (p.y - ps.y) / v.y;
            return new PointD(ps.x + v.x, p.y);
        }

        /// <summary>
        /// 2線分の交点
        /// </summary>
        /// <param name="l">対象線分</param>
        /// <returns>交点座標</returns>
        public PointD intersection(LineD l)
        {
            PointD v1 = vector();
            PointD v2 = l.vector();
            double k = v1.x * v2.y - v2.x * v1.y;
            if (Math.Abs(k) < mEps)
                return new PointD(double.NaN, double.NaN);
            double x = (-1 / k) * (v1.y * v2.x * this.ps.x - v2.y * v1.x * l.ps.x + (l.ps.y - this.ps.y) * v1.x * v2.x);
            double y =  (1 / k) * (v1.x * v2.y * this.ps.y - v2.x * v1.y * l.ps.y + (l.ps.x - this.ps.x) * v1.y * v2.y);
            return new PointD(x, y);
        }

        /// <summary>
        /// 点が線分上にあるかを判定
        /// </summary>
        /// <param name="pnt">対象点座標</param>
        /// <returns>線分上にある(true)</returns>
        public bool lineOnPoint(PointD pnt)
        {
            if (pe.x < ps.x) {
                if (pnt.x < (pe.x - mEps) || (ps.x + mEps) < pnt.x)
                    return false;
            } else {
                if (pnt.x < (ps.x - mEps) || (pe.x + mEps) < pnt.x)
                    return false;
            }
            if (pe.y < ps.y) {
                if (pnt.y < (pe.y - mEps) || (ps.y + mEps) < pnt.y)
                    return false;
            } else {
                if (pnt.y < (ps.y - mEps) || (pe.y + mEps) < pnt.y)
                    return false;
            }
            return true;
        }

        private const int INSIDE = 0b0000;
        private const int LEFT   = 0b0001;
        private const int RIGHT  = 0b0010;
        private const int BOTTOM = 0b0100;
        private const int TOP    = 0b1000;

        /// <summary>
        /// クリッピング領域に対する点の9分割位置の範囲
        /// Cohen-Sutherland aalgorithm
        ///
        /// 1001|1001|1010
        /// --------------
        /// 0001|0000|0010
        /// --------------
        /// 0101|0110|0110
        /// </summary>
        /// <param name="p">座標</param>
        /// <param name="rect">クリッピング領域</param>
        /// <returns>点位置のビットマップ</returns>
        private byte inOutAreaCode(PointD p, Rect rect)
        {
            double xmin = rect.X;
            double ymin = rect.Y;
            double xmax = rect.X + rect.Width;
            double ymax = rect.Y + rect.Height;
            byte code = INSIDE;
            if (p.x < xmin) code |= LEFT;
            else if (xmax < p.x) code |= RIGHT;
            if (p.y < ymin) code |= BOTTOM;
            else if (ymax < p.y) code |= TOP;
            return code;
        }

        /// <summary>
        /// クリッピング領域に対する点の9分割位置の範囲
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private byte inOutAreaCode(Point p, Rect rect)
        {
            return inOutAreaCode(new PointD(p), rect);
        }

        /// <summary>
        /// 線分矩形領域でクリッピングする(Cohen-Sutherland clipping algorithm)
        /// </summary>
        /// <param name="rect">クリッピングの矩形領域</param>
        /// <returns>クリッピングした線分</returns>
        public LineD clippingLine(Rect rect)
        {
            double xmin = rect.X;
            double ymin = rect.Y;
            double xmax = rect.X + rect.Width;
            double ymax = rect.Y + rect.Height;
            LineD l2 = new LineD(ps, pe);
            byte outcode0 = inOutAreaCode(l2.ps, rect);
            byte outcode1 = inOutAreaCode(l2.pe, rect);

            while (true) {
                if ((outcode0 | outcode1) == 0) {         //  両端点がフレームの中(クリッピング不要)
                    break;
                } else if ((outcode0 & outcode1) != 0) {   //  確実に表示されない線分
                    l2 = null;
                    break;
                } else {                                //  クリッピング処理
                    double x = 0, y = 0;
                    int outcode = outcode0 != 0 ? outcode0 : outcode1;
                    if ((outcode & TOP) != 0) {
                        x = l2.ps.x + (l2.pe.x - l2.ps.x) * (ymax - l2.ps.y) / (l2.pe.y - l2.ps.y);
                        y = ymax;
                    } else if ((outcode & BOTTOM) != 0) {
                        x = l2.ps.x + (l2.pe.x - l2.ps.x) * (ymin - l2.ps.y) / (l2.pe.y - l2.ps.y);
                        y = ymin;
                    } else if ((outcode & RIGHT) != 0) {
                        y = l2.ps.y + (l2.pe.y - l2.ps.y) * (xmax - l2.ps.x) / (l2.pe.x - l2.ps.x);
                        x = xmax;
                    } else if ((outcode & LEFT) != 0) {
                        y = l2.ps.y + (l2.pe.y - l2.ps.y) * (xmin - l2.ps.x) / (l2.pe.x - l2.ps.x);
                        x = xmin;
                    }
                    if (outcode == outcode0) {
                        l2.ps.x = x;
                        l2.ps.y = y;
                        outcode0 = inOutAreaCode(l2.ps, rect);
                    } else {
                        l2.pe.x = x;
                        l2.pe.y = y;
                        outcode1 = inOutAreaCode(l2.pe, rect);
                    }
                }
            }
            return l2;
        }

        /// <summary>
        /// 増分を指定して平行移動
        /// </summary>
        /// <param name="dx">x方向の増分</param>
        /// <param name="dy">y方向の増分</param>
        public void move(double dx, double dy)
        {
            ps.x += dx;
            ps.y += dy;
            pe.x += dx;
            pe.y += dy;
        }

        /// <summary>
        /// 距離と角度を指定して平行移動
        /// </summary>
        /// <param name="r">移動距離</param>
        /// <param name="th">角度(rad)</param>
        public void moveLength(double r,double th)
        {
            double dx = r * Math.Cos(th);
            double dy = r * Math.Sin(th);
            move(dx, dy);
        }

        /// <summary>
        /// 延長線上に距離を指定して移動
        /// </summary>
        /// <param name="l">移動距離</param>
        public void moveLength(double l)
        {
            moveLength(l, angle());
        }

        /// <summary>
        /// 中心を指定して線分を回転させる
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        public void rotate(Point ctr, double rotate)
        {
            this.rotate(new PointD(ctr), rotate);
        }

        /// <summary>
        /// 中心を指定して線分を回転させる
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        public void rotate(PointD ctr, double rotate)
        {
            ps.rotatePoint(ctr, rotate);
            pe.rotatePoint(ctr, rotate);
        }

        /// <summary>
        /// 2角の間にあるかを判定(境界を含む)
        /// </summary>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <param name="ang">対象角(rad)</param>
        /// <returns>間にある(true)</returns>
        private bool innerAngle(double sa, double ea, double ang)
        {
            if (ea < sa) {
                ea += Math.PI * 2.0;
                ang += ang < sa ? Math.PI * 2.0 : 0.0;
            }
            if (sa <= ang && ang <= ea)
                return true;
            else
                return false;
        }
    }
}
