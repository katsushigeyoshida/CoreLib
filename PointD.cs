using System;
using System.Windows;

namespace CoreLib
{
    /// <summary>
    /// 点座標クラス
    /// PointD()
    /// PointD(double x, double y)
    /// PointD(PointD p)
    /// PointD(Point p)
    /// 
    /// override string ToString()
    /// Point toPoint()
    /// PointD toCopy()
    /// double angle()
    /// double angle(PointD p)
    /// double length()
    /// double length(PointD p)
    /// PointD vector(PointD p)
    /// void fromPoler(double r, double th)
    /// void Offset(double dx, double dy)
    /// void transform(double dx, double dy)
    /// void transform(PointD vp)
    /// void rotate(double ang)
    /// void rotatePoint(PointD cp, double ang)
    /// void scale(double scale)
    /// void scalePoint(PointD cp, double scale)
    /// </summary>
    public class PointD
    {
        public double x = 0.0;
        public double y = 0.0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PointD()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">PointD</param>
        public PointD(PointD p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">Point</param>
        public PointD(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
        }

        /// <summary>
        /// 文字列変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        /// <summary>
        /// 値を0クリアする
        /// </summary>
        public void clear()
        {
            x = 0.0;
            y = 0.0;
        }

        /// <summary>
        /// 値が空(0,0)かどうかを確認
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            return x == 0.0 && y == 0.0;
        }

        /// <summary>
        /// x,yのどちらかに非数があるかを確認
        /// </summary>
        /// <returns></returns>
        public bool isNaN()
        {
            return double.IsNaN(x) || double.IsNaN(y);
        }

        /// <summary>
        /// 値に非数を設定する
        /// </summary>
        public void setNaN()
        {
            x = double.NaN;
            y = double.NaN;
        }

        /// <summary>
        /// 符号を反転する
        /// </summary>
        public void invert()
        {
            x *= -1.0;
            y *= -1.0;
        }

        /// <summary>
        /// Pointクラスに変換
        /// </summary>
        /// <returns></returns>
        public Point toPoint()
        {
            return new Point(x, y);
        }

        /// <summary>
        /// PointDへコピー
        /// </summary>
        /// <returns></returns>
        public PointD toCopy()
        {
            return new PointD(x, y);
        }

        /// <summary>
        /// 原点に対する角度(rad)
        /// </summary>
        /// <returns>角度(rad)</returns>
        public double angle()
        {
            return Math.Atan2(y, x);
        }

        /// <summary>
        /// 指定点を原点とした角度(rad)
        /// </summary>
        /// <param name="p">PointD</param>
        /// <returns>角度(rad)</returns>
        public double angle(PointD p)
        {
            double dx = x - p.x;
            double dy = y - p.y;
            return Math.Atan2(dy, dx);
        }

        /// <summary>
        /// 原点からの距離
        /// </summary>
        /// <returns>距離</returns>
        public double length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// 指定点との距離
        /// </summary>
        /// <param name="p">PointD</param>
        /// <returns>距離</returns>
        public double length(PointD p)
        {
            double dx = p.x - x;
            double dy = p.y - y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 指定点に対するベクトル値
        /// </summary>
        /// <param name="p">PointD</param>
        /// <returns>ベクトル値</returns>
        public PointD vector(PointD p)
        {
            return new PointD(p.x - x, p.y - y);
        }


        /// <summary>
        /// 自ベクトルと指定ベクトルの内積(合成ベクトルの長さ)
        /// </summary>
        /// <param name="p">指定ベクトル</param>
        /// <returns>内積</returns>
        public double innerProduct(PointD p)
        {
            return (x * p.x + y * p.y);
        }

        /// <summary>
        /// 自ベクトルと指定ベクトルの外積(ベクトルの平行判定 0の時平行、点が線上かの判定)
        /// </summary>
        /// <param name="p">指定ベクトル</param>
        /// <returns>外積</returns>
        public double crossProduct(PointD p)
        {
            return (x * p.y - y * p.x);
        }

        /// <summary>
        /// 極座標からの取り込み
        /// </summary>
        /// <param name="r">長さ</param>
        /// <param name="th">角度(rad)</param>
        public void fromPoler(double r, double th)
        {
            x = r * Math.Cos(th);
            y = r * Math.Sin(th);
        }

        /// <summary>
        /// 指定の量だけ移動
        /// </summary>
        /// <param name="dx">X方向の移動距離</param>
        /// <param name="dy">Y方向の移動距離</param>
        public void Offset(double dx, double dy)
        {
            x += dx;
            y += dy;
        }

        /// <summary>
        /// 指定の量だけ移動
        /// </summary>
        /// <param name="offset">移動量</param>
        public void Offset(PointD offset)
        {
            x += offset.x;
            y += offset.y;
        }

        /// <summary>
        /// 点位置の移動
        /// </summary>
        /// <param name="dx">X方向の移動距離</param>
        /// <param name="dy">Y方向の移動距離</param>
        public void transform(double dx, double dy)
        {
            x += dx;
            y += dy;
        }

        /// <summary>
        /// 点位置の移動(ベクトル値で移動)
        /// </summary>
        /// <param name="vp">ベクトル値(PointD)</param>
        public void transform(PointD vp)
        {
            x += vp.x;
            y += vp.y;
        }

        /// <summary>
        /// 原点を中心に回転
        /// </summary>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(double ang)
        {
            double tx = x * Math.Cos(ang) - y * Math.Sin(ang);
            double ty = x * Math.Sin(ang) + y * Math.Cos(ang);
            x = tx;
            y = ty;
        }

        /// <summary>
        /// 中心点を指定して回転
        /// </summary>
        /// <param name="cp">中心点(PointD)</param>
        /// <param name="ang">角度(rad)</param>
        public void rotatePoint(PointD cp, double ang)
        {
            double dx = x - cp.x;
            double dy = y - cp.y;
            x = dx * Math.Cos(ang) - dy * Math.Sin(ang) + cp.x;
            y = dx * Math.Sin(ang) + dy * Math.Cos(ang) + cp.y;
        }

        /// <summary>
        /// 原点を中心に拡大縮小
        /// </summary>
        /// <param name="scale">拡大率</param>
        public void scale(double scale)
        {
            x *= scale;
            y *= scale;
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点(PointD)</param>
        /// <param name="scale">拡大率</param>
        public void scalePoint(PointD cp, double scale)
        {
            double dx = (x - cp.x) * scale;
            double dy = (y - cp.y) * scale;
            x = cp.x + dx;
            y = cp.y + dy;
        }

        /// <summary>
        /// 四角形の内側かどうかを判定
        /// </summary>
        /// <param name="r">四角形(Rect)</param>
        /// <returns>内外判定</returns>
        public bool isInside(Rect r)
        {
            if (x < r.Left || x > r.Right || y < r.Top || y > r.Bottom)
                return false;
            else
                return true;
        }
    }
}
