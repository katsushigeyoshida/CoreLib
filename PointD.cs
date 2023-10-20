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
    /// string ToString(string form)                書式付き文字列変換
    /// void clear()                                値を0クリアする
    /// bool isEmpty()                              値が空(0,0)かどうかを確認
    /// bool isNaN()                                x,yのどちらかに非数があるかを確認
    /// void setNaN()                               値に非数を設定する
    /// bool isEqual(PointD p)                      同じ値かの判定
    /// Point toPoint()                             Pointクラスに変換
    /// PointD toCopy()                             PointDへコピー
    /// double angle()                              原点に対する角度(rad)
    /// double angle(PointD p)                      指定点を原点とした角度(rad)
    /// double angle(PointD p1, PointD p2)          自点を中心とした2点の角度
    /// double length()                             原点からの距離
    /// double length(PointD p)                     指定点との距離
    /// void setLength(double l)                    原点からのベクトルの長さを設定する
    /// PointD vector(PointD p)                     指定点に対するベクトル値
    /// PointD vector(double angle, double length)  角度と距離を指定したベクトル値
    /// void invert()                               符号を反転する
    /// PointD inverse()                            ベクトルを反転させる
    /// double innerProduct(PointD p)               自ベクトルと指定ベクトルの内積(合成ベクトルの長さ)
    /// double crossProduct(PointD p)               ベクトルと指定ベクトルの外積(ベクトルの平行判定 0の時平行、点が線上かの判定)
    /// void floor(double f)                        数値を切り捨てて丸める
    /// void round(double f)                        数値を四捨五入で丸める
    /// void fromPoler(double r, double th)         極座標からの取り込み
    /// void offset(double dx, double dy)           指定の量だけ移動
    /// void offset(PointD offset)                  指定の量だけ移動
    /// void translate(double dx, double dy)        点位置の移動
    /// void translate(PointD vp)                   点位置の移動(ベクトル値で移動)
    /// void translate(PointD sp, PointD ep)        点位置の移動
    /// void rotate(double ang)                     原点を中心に回転
    /// void rotate(PointD cp, double ang)          中心点を指定して回転
    /// void rotate(PointD cp, PointD up)           指定点を中心に回転(回転角はcpとupでの角度)
    /// void mirror(PointD p)                       指定点でミラーする
    /// void mirror(PointD sp, PointD ep)           指定線分に対してミラーする
    /// void scale(double scale)                    原点を中心に拡大縮小
    /// void scale(PointD cp, double scale)         原点を指定して拡大縮小
    /// bool isInside(Rect r)                       四角形の内側かどうかを判定
    /// ---  static  ---
    /// PointD operator +(PointD v1, PointD v2)
    /// PointD operator -(PointD v1, PointD v2)
    /// PointD operator *(PointD v1, double m)
    /// double operator *(PointD v1, PointD v2)
    /// PointD operator /(PointD v1, double m)
    /// PointD operator /(PointD v1, PointD v2)
    /// 
    /// double distance(PointD p1, PointD p2)       2点間の距離
    /// 
    /// </summary>
    public class PointD
    {
        public double x = double.NaN;
        public double y = double.NaN;

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
        /// 書式付き文字列変換
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string ToString(string form)
        {
            return "(" + x.ToString(form) + "," + y.ToString(form) + ")";
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
        /// 同じ値かの判定
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool isEqual(PointD p)
        {
            if (x == p.x && y == p.y) return true;
            return false;
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
        /// 自点を中心とした2点の角度
        /// </summary>
        /// <param name="p1">点座標1</param>
        /// <param name="p2">点座標2</param>
        /// <returns>角度(rad)</returns>
        public double angle(PointD p1, PointD p2)
        {
            double angle1 = p1.angle(toCopy());
            double angle2 = p2.angle(toCopy());
            return Math.Abs(angle1 - angle2);
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
        /// 原点からのベクトルの長さを設定する
        /// </summary>
        /// <param name="l">長さ</param>
        public void setLength(double l)
        {
            double len = length();
            x *= l / len;
            y *= l / len;
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
        /// 角度と距離を指定したベクトル値
        /// </summary>
        /// <param name="angle">角度(rad)</param>
        /// <param name="length">距離</param>
        /// <returns>ベクトル値</returns>
        public PointD vector(double angle, double length)
        {
            PointD vec = new PointD(length * Math.Cos(angle), length * Math.Sin(angle));
            return vec;
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
        /// ベクトルを反転させる
        /// </summary>
        /// <returns>反転したベクトル</returns>
        public PointD inverse()
        {
            return new PointD(-x, -y);
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
        /// 数値を切り捨てて丸める
        /// </summary>
        /// <param name="f">丸めの単位</param>
        public void floor(double f)
        {
            if (0 < f) {
                x = Math.Floor(x / f) * f;
                y = Math.Floor(y / f) * f;
            }
        }

        /// <summary>
        /// 数値を四捨五入で丸める
        /// </summary>
        /// <param name="f">丸めの単位</param>
        public void round(double f)
        {
            if (0 < f) {
                x = Math.Round(x / f) * f;
                y = Math.Round(y / f) * f;
            }
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
        public void offset(double dx, double dy)
        {
            x += dx;
            y += dy;
        }

        /// <summary>
        /// 指定の量だけ移動
        /// </summary>
        /// <param name="offset">移動量</param>
        public void offset(PointD offset)
        {
            x += offset.x;
            y += offset.y;
        }

        /// <summary>
        /// 点位置の移動
        /// </summary>
        /// <param name="dx">X方向の移動距離</param>
        /// <param name="dy">Y方向の移動距離</param>
        public void translate(double dx, double dy)
        {
            x += dx;
            y += dy;
        }

        /// <summary>
        /// 点位置の移動(ベクトル値で移動)
        /// </summary>
        /// <param name="vp">ベクトル値(PointD)</param>
        public void translate(PointD vp)
        {
            x += vp.x;
            y += vp.y;
        }

        /// <summary>
        /// 点位置の移動
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void translate(PointD sp, PointD ep)
        {
            PointD vec = ep.vector(sp);
            translate(vec);
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
        public void rotate(PointD cp, double ang)
        {
            double dx = x - cp.x;
            double dy = y - cp.y;
            x = dx * Math.Cos(ang) - dy * Math.Sin(ang) + cp.x;
            y = dx * Math.Sin(ang) + dy * Math.Cos(ang) + cp.y;
        }

        /// <summary>
        /// 指定点を中心に回転(回転角はcpとupでの角度)
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="up">回転した点</param>
        public void rotate(PointD cp, PointD up)
        {
            double ang = up.angle(cp);
            rotate(cp, ang);
        }

        /// <summary>
        /// 指定点でミラーする
        /// </summary>
        /// <param name="p"></param>
        public void mirror(PointD p)
        {
            double dx = p.x - x;
            double dy = p.y - y;
            x = p.x + dx;
            y = p.y + dy;
        }

        /// <summary>
        /// 指定線分に対してミラーする
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">始点座標</param>
        public void mirror(PointD sp, PointD ep)
        {
            LineD l = new LineD(sp, ep);
            PointD p = new PointD(x, y);
            PointD cp = l.intersection(p);
            mirror(cp);
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
        public void scale(PointD cp, double scale)
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

        /// <summary>
        /// 2つのベクトルの加算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static PointD operator +(PointD v1, PointD v2)
        {
            return new PointD(v1.x + v2.x, v1.y + v2.y);
        }

        /// <summary>
        /// ベクトルv1からベクトルv2を減算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static PointD operator -(PointD v1, PointD v2)
        {
            return new PointD(v1.x - v2.x, v1.y - v2.y);
        }

        /// <summary>
        /// ベクトルv1をスカラー値mで乗算
        /// </summary>
        /// <param name="v1">^ベクトル</param>
        /// <param name="m">スカラー値</param>
        /// <returns>ベクトル</returns>
        public static PointD operator *(PointD v1, double m)
        {
            return new PointD(v1.x * m, v1.y * m);
        }

        /// <summary>
        /// 2 つのベクトルの要素の各ペアを乗算した値を値とするベクトルを返し
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static double operator *(PointD v1, PointD v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }

        /// <summary>
        /// ベクトルをスカラー値で除算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="m">スカラー値</param>
        /// <returns>ベクトル</returns>
        public static PointD operator /(PointD v1, double m)
        {
            return new PointD(v1.x / m, v1.y / m);
        }

        /// <summary>
        /// 最初のベクトルを 2 番目のベクトルで除算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static PointD operator /(PointD v1, PointD v2)
        {
            return new PointD(v1.x / v2.x, v1.y / v2.y);
        }

        /// <summary>
        /// 2点間の距離
        /// </summary>
        /// <param name="p1">座標店</param>
        /// <param name="p2">座標点</param>
        /// <returns>距離</returns>
        public static double distance(PointD p1, PointD p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }
    }
}
