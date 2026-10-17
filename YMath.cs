using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CoreLib
{
    //  ---  数値処理関連  ------
    //	double distance(System.Windows.Point ps, System.Windows.Point pe)	 2点間の距離
    //	double mod(double a, double b)	 剰余関数 (負数の剰余を正数で返す)
    //	int mod(int a, int b)	 剰余関数 (負数の剰余を正数で返す)
    //	double log(double a, double b)	 底指定の対数(log b a)
    //	float roundFloor(float val, int n)	 指定の有効桁で切り捨てる
    //	float roundCeil(float val, int n)	 指定の有効桁で切り上げる
    //	float roundRound(float val, int n)	 指定の有効桁数で四捨五入する
    //	decimal roundRound(decimal val, int digits = 13)	 指定の有効桁数で四捨五入する
    //	double floorStepSize(double val)	 最上位桁が1,2,5になるように下側に丸める
    //	double graphStepSize(double range, double targetSteps, int fromBase = 10)	 グラフ作成時の補助線間隔を求める
    //	double graphHeightSize(double height, double stepSize)	 グラフの最大値を求める
    //	string axisScaleForm(double val, int digits = 4)	 有効桁数を指定して値を丸めて文字列に変換
    //	List<double> solveQuadraticEquation(double a, double b, double c)	 2次方程式の解を求める
    //	List<double> solveCubicEquation(double a, double b, double c, double d)	 3次方程式の解(カルダノの公式)
    //	List<double> solveQuarticEquation(double a, double b, double c, double d, double e)	 4次方程式の解(フェラリ(Ferrari)の公式)
    //	double Cuberoot(double x)	 3乗根(x^1/3)
    //	Complex Squreroot(Complex x)	 複素数の平方根
    //
    //  ---  統計処理  ---
    //	double getSumList(List<double> list)	  ∑x リストの合計を求める
    //	double getSqrSumList(List<double> list)	  ∑x^2 リストの自乗和(二乗の合計)を求める
    //	double getAverageList(List<double> list)	  ∑x / n 平均値
    //	double getDevSumList(List<double> list)	  ∑(x-xm) 偏差の和(常に0)
    //	double getVerSumList(List<double> list)	  ∑(x-xm)^2　偏差の平方和
    //	double getVariance(List<double> list)	  s^2 = ∑(x-xm)^2 / n 分散(variance)
    //	double getStdDev(List<double> list)	  標準偏差(standard deviation)
    //	double getDevProductSumList(List<double> Xlist, List<double> Ylist)	  ∑(x-xm)(y-ym) 偏差積和
    //	double getCovarince(List<double> Xlist, List<double> Ylist)	  共分散(Covariance)
    //	double getCorelation(List<double> Xlist, List<double> Ylist)	  相関係数(correlation coefficient)  ρ = σxy / (σx * σy)
    //	double getRegA(List<double> Xlist, List<double> Ylist)	  回帰分析(regression analysis)の係数(a)の取得  y = ax + b
    //	double getRegB(List<double> Xlist, List<double> Ylist)	  回帰分析(regression analysis)の係数(b)の取得  y = ax + b
    //	double getCoefficentDeterminatio(List<double> Xlist, List<double> Ylist, double a, double b)	  決定係数(coefficient of determination)
    //	double getXYSumList(List<PointD> list)	  ∑xy X*Yの合計
    //	double getDevProductSumList(List<PointD> list)	  ∑(x-xm)(y-ym) 偏差積和
    //	double getCovarince(List<PointD> list)	  共分散(Covariance)
    //	double getCorelation(List<PointD> list)	  相関係数(correlation coefficient)  ρ = σxy / (σx * σy)
    //	double getRegA(List<PointD> list)	  回帰分析(regression analysis)の係数(a)の取得  y = ax + b
    //	double getRegB(List<PointD> list)	  回帰分析(regression analysis)の係数(b)の取得  y = ax + b
    //	double getCoefficentDeterminatio(List<PointD> list, double a, double b)	  決定係数(coefficient of determination)
    //	double getRegVariance(List<PointD> list, double a, double b)	 理論値に対する分散(回帰曲線の残差の二乗和)

    //  ------  座標処理  --------
    //	double coordinateDistance(double longi1, double lati1, double longi2, double lati2)	 球面上の2点間座標の距離
    //	static double CoordinateDistance(double longi1, double lati1, double longi2, double lati2)	 球面上の2点間座標の距離(static関数)
    //	double coordinateDistance(System.Windows.Point ps, System.Windows.Point pe)	 球面上の2点間座標の距離
    //	static double CoordinateDistance(System.Windows.Point ps, System.Windows.Point pe)	 球面上の2点間座標の距離(static関数)
    //	double azimuth(double longi1, double lati1, double longi2, double lati2)	 球面上の2点間座標の方位
    //	double azimuth2(double longi1, double lati1, double longi2, double lati2)	 球面上の2点間座標の方位

    //  ---  行列計算  ---
    //	double[,] unitMatrix(int unit)	 単位行列の作成(正方行列のみ)
    //	double[,] matrixTranspose(double[,] A)	 転置行列  行列Aの転置A^T
    //	double[,] matrixMulti(double[,] A, double[,] B)	 行列の積  AxB
    //	double[,] matrixAdd(double[,] A, double[,] B)	 行列の和 A+B
    //	double[,] matrixInverse(double[,] A)	 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列)
    //	double[,] copyMatrix(double[,] A)	 行列のコピー
    //	double[,] translate3DMatrix(double dx, double dy, double dz)	 移動量を3D変換マトリックス(4x4)に設定
    //	double[,] rotateX3DMatrix(double th)	 X軸回転を3D変換マトリックス(4x4)に設定
    //	double[,] rotateY3DMatrix(double th)	 Y軸回転を3D変換マトリックス(4x4)に設定
    //	double[,] rotateZ3DMatrix(double th)	 Z軸回転を3D変換マトリックス(4x4)に設定
    //	double[,] rotate(Point3D vec, double th)	 任意の軸を中心に回転させる
    //	double[,] scale3DMatrix(double sx, double sy, double sz)	  拡大縮小のスケール値を3D変換マトリックス(4x4)に設定
    //	double[,] translate2DMatrix(double dx, double dy)	 移動量を2D変換マトリックス(3x3)に設定
    //	double[,] rotate2DMatrix(double th)	 原点回転を2D変換マトリックス(3x3)に設定
    //	double[,] scale2DMatrix(double sx, double sy)	  拡大縮小のスケール値を2D変換マトリックス(3x3)に設定
    //	double[,] inverse2DMatrix()	 原点に対して反転する2D変換マトリックス(3x3)に設定
    //	double[,] inverseX2DMatrix()	 X軸に対して反転する2D変換マトリックス(3x3)に設定
    //	double[,] inverseY2DMatrix()	 Y軸に対して反転する2D変換マトリックス(3x3)に設定

    public class YMath
    {
        //  ---  数値処理関連  ------

        /// <summary>
        /// 2点間の距離
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        /// <returns>距離</returns>
        public double distance(System.Windows.Point ps, System.Windows.Point pe)
        {
            double dx = ps.X - pe.X;
            double dy = ps.Y - pe.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 剰余関数 (負数の剰余を正数で返す)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>剰余</returns>
        public double mod(double a, double b)
        {
            double c = (a % b);
            c += c < 0.0 ? b : 0.0;
            return c;
        }

        /// <summary>
        /// 剰余関数 (負数の剰余を正数で返す)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>剰余</returns>
        public int mod(int a, int b)
        {
            int c = (a % b);
            c += c < 0 ? b : 0;
            return c;
        }

        /// <summary>
        /// 底指定の対数(log b a)
        /// </summary>
        /// <param name="a">数値</param>
        /// <param name="b">底(10進,60進とか)</param>
        /// <returns></returns>
        public double log(double a, double b)
        {
            return Math.Log10(a) / Math.Log10(b);
        }

        /// <summary>
        /// 指定の有効桁で切り捨てる
        /// </summary>
        /// <param name="val">数値</param>
        /// <param name="n">丸める有効桁</param>
        /// <returns>丸めた値</returns>
        public float roundFloor(float val, int n)
        {
            if (val == 0f)
                return val;
            float sign = Math.Sign(val);
            if (val < 0)
                val *= sign;
            float mag = (float)Math.Floor(Math.Log10(val));     //  桁数を求める
            float magPow = (float)Math.Pow(10, mag - n + 1);    //  
            return (float)Math.Floor(val * sign / magPow) * magPow;
        }

        /// <summary>
        /// 指定の有効桁で切り上げる
        /// </summary>
        /// <param name="val">数値</param>
        /// <param name="n">丸める有効桁</param>
        /// <returns>丸めた値</returns>
        public float roundCeil(float val, int n)
        {
            if (val == 0f)
                return val;
            float sign = Math.Sign(val);
            if (val < 0)
                val *= sign;
            float mag = (float)Math.Floor(Math.Log10(val));
            float magPow = (float)Math.Pow(10, mag - n + 1);
            return (float)Math.Ceiling(val * sign / magPow) * magPow;
        }

        /// <summary>
        /// 指定の有効桁数で四捨五入する
        /// </summary>
        /// <param name="val">数値</param>
        /// <param name="n">丸める有効桁数</param>
        /// <returns>丸めた値</returns>
        public float roundRound(float val, int n)
        {
            if (val == 0f)
                return val;
            float sign = Math.Sign(val);
            if (val < 0)
                val *= sign;
            float mag = (float)Math.Floor(Math.Log10(val));
            float magPow = (float)Math.Pow(10, mag - n + 1);
            return (float)Math.Round(val * sign / magPow) * magPow;
        }

        /// <summary>
        /// 指定の有効桁数で四捨五入する
        /// </summary>
        /// <param name="val">数値</param>
        /// <param name="digits">丸める有効桁数</param>
        /// <returns>丸めた数値</returns>
        public decimal roundRound(decimal val, int digits = 13)
        {
            if (val == 0M)
                return val;
            int mag = (int)Math.Log10(Math.Abs((double)val)) + 1;
            var scale = (decimal)Math.Pow(10, mag);
            var result = scale * Math.Round(val / scale, digits, MidpointRounding.AwayFromZero);
            int decimals = digits - mag;
            return Math.Round(result, (decimals < 0) ? 0 : decimals);
        }

        /// <summary>
        /// 最上位桁が1,2,5になるように下側に丸める
        /// </summary>
        /// <param name="val">数値</param>
        /// <returns>丸めた値</returns>
        public double floorStepSize(double val)
        {
            double mag = Math.Floor(Math.Log10(Math.Abs(val)));
            double magPow = Math.Floor(Math.Abs(val) / Math.Pow(10, mag));
            double magMsd = 0;
            if (magPow >= 5)
                magMsd = 5 * Math.Pow(10, mag);
            else if (magPow >= 2)
                magMsd = 2 * Math.Pow(10, mag);
            else
                magMsd = Math.Pow(10, mag);
            return magMsd * Math.Sign(val);
        }

        /// <summary>
        /// グラフ作成時の補助線間隔を求める
        /// 補助線の間隔は1,2,5の倍数
        /// </summary>
        /// <param name="range">補助線の範囲</param>
        /// <param name="targetSteps">補助線の数</param>
        /// <param name="fromBase">基底数(10進,60進,24進...)</param>
        /// <returns>補助線の間隔</returns>
        public double graphStepSize(double range, double targetSteps, int fromBase = 10)
        {
            // calculate an initial guess at step size
            double tempStep = range / targetSteps;                  //  仮の間隔

            // get the magnitude of the step size
            double mag = Math.Floor(log(tempStep, fromBase));       //  ステップサイズの桁数
            double magPow = Math.Pow(fromBase, mag);                //  最上位数

            // calculate most significant digit of the new step size
            double magMsd = (int)(tempStep / magPow + 0.5);         //  ステップサイズの最上位桁

            // promote the MSD to either 1, 2, or 5
            if (magMsd > fromBase / 2.0)
                magMsd = fromBase;
            else if (magMsd > fromBase / 5.0)
                magMsd = Math.Floor(fromBase / 2.0);
            else if (magMsd > fromBase / 10.0)
                magMsd = Math.Floor(fromBase / 5.0);

            return magMsd * magPow;
        }

        /// <summary>
        /// グラフの最大値を求める
        /// </summary>
        /// <param name="height">データの最大値</param>
        /// <param name="stepSize">目盛間隔</param>
        /// <returns></returns>
        public double graphHeightSize(double height, double stepSize)
        {
            //  グラフ高さの調整
            if (height < 0)
                return ((int)(height / stepSize) - 1) * stepSize;
            else
                return ((int)(height / stepSize) + 1) * stepSize;
        }

        /// <summary>
        /// 有効桁数を指定して値を丸めて文字列に変換
        /// 表示桁数が有効桁より大きくなる時指数表示にする
        /// </summary>
        /// <param name="val">値</param>
        /// <param name="digits">有効桁数</param>
        /// <returns>数値文字列</returns>
        public string axisScaleForm(double val, int digits = 4)
        {
            if (val == 0)
                return "0";
            int mag = (int)Math.Log10(Math.Abs(val)) - (Math.Abs(val) < 1 ? 1 : 0);
            var scale = Math.Pow(10, mag);
            val = (double)roundRound((decimal)val, digits);
            if (digits > Math.Abs(mag)) {
                return val.ToString();
            } else {
                return $"{val / scale}e{mag}";
            }

        }

        /// <summary>
        /// 2次方程式の解を求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>実数解リスト</returns>
        public List<double> solveQuadraticEquation(double a, double b, double c)
        {
            List<double> solve = new List<double>();
            if (a == 0) {
                solve.Add(-c / b);
            } else {
                double d = b * b - 4 * a * c;
                if (0 < d) {
                    d = Math.Sqrt(d);
                    solve.Add((-b + d) / (2 * a));
                    solve.Add((-b - d) / (2 * a));
                } else if (d == 0) {
                    solve.Add(-b / (2 * a));
                } else {
                    //  複素数の解
                    Complex ca = new Complex(a, 0);
                    Complex cb = new Complex(b, 0);
                    Complex cc = new Complex(c, 0);
                    Complex cd = cb * cb - 4 * ca * cc;
                    Complex x1 = (-cb - Complex.Sqrt(cd) / (2 * ca));
                    Complex x2 = (-cb + Complex.Sqrt(cd) / (2 * ca));
                    Console.WriteLine($"複素数の解 {x1} , {x2}");
                }
            }
            return solve;
        }

        /// <summary>
        /// 3次方程式の解(カルダノの公式)
        ///   https://qiita.com/yotapoon/items/42b1749b69c264d6f486
        ///   https://onihusube.hatenablog.com/entry/2018/10/08/140426
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns>実数解リスト</returns>
        public List<double> solveCubicEquation(double a, double b, double c, double d)
        {
            List<double> solve = new List<double>();
            if (a == 0)
                return solveQuadraticEquation(b, c, d);

            Complex[] x = new Complex[3];
            double A = b / a;
            double B = c / a;
            double C = d / a;
            double p = B - A * A / 3.0;
            double q = 2.0 * A * A * A / 27.0 - A * B / 3.0 + C;
            double D = q * q / 4.0 + p * p * p / 27.0;
            if (D < 0.0) {//three real solutions
                double theta = Math.Atan2(Math.Sqrt(-D), -q * 0.5);
                x[0] = 2.0 * Math.Sqrt(-p / 3.0) * Math.Cos(theta / 3.0) - A / 3.0;
                x[1] = 2.0 * Math.Sqrt(-p / 3.0) * Math.Cos((theta + 2.0 * Math.PI) / 3.0) - A / 3.0;
                x[2] = 2.0 * Math.Sqrt(-p / 3.0) * Math.Cos((theta + 4.0 * Math.PI) / 3.0) - A / 3.0;
            } else {//single real solution and two imaginary solutions(c.c)
                double u = Cuberoot(-q * 0.5 + Math.Sqrt(D));
                double v = Cuberoot(-q * 0.5 - Math.Sqrt(D));
                x[0] = u + v - A / 3.0;
                x[1] = -0.5 * (u + v) + Math.Sqrt(3.0) * 0.5 * Complex.ImaginaryOne * (u - v) - A / 3.0;
                x[2] = -0.5 * (u + v) - Math.Sqrt(3.0) * 0.5 * Complex.ImaginaryOne * (u - v) - A / 3.0;
            }
            for (int i = 0; i < x.Length; i++) {
                if (x[i].Imaginary == 0)
                    solve.Add(x[i].Real);
                Console.WriteLine($"複素数の解 {x[i]}");
            }
            return solve;
        }

        /// <summary>
        /// 4次方程式の解(フェラリ(Ferrari)の公式)
        ///   https://qiita.com/yotapoon/items/42b1749b69c264d6f486
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <returns>実数解リスト</returns>
        public List<double> solveQuarticEquation(double a, double b, double c, double d, double e)
        {
            List<double> solve = new List<double>();
            if (a == 0.0) {
                return solveCubicEquation(b, c, d, e);
            }
            Complex[] x = new Complex[4];
            double A = b / a;
            double B = c / a;
            double C = d / a;
            double D = e / a;
            double p = -6.0 * Math.Pow(A / 4.0, 2.0) + B;
            double q = 8.0 * Math.Pow(A / 4.0, 3.0) - 2.0 * B * A / 4.0 + C;
            double r = -3.0 * Math.Pow(A / 4.0, 4.0) + B * Math.Pow(A / 4.0, 2.0) - C * A / 4.0 + D;
            List<double> temp = solveCubicEquation(1.0, -p, -4.0 * r, 4.0 * p * r - q * q);
            Complex m = Squreroot(temp[0] - p);
            x[0] = (-m + Squreroot(-temp[0] - p + 2.0 * q / m)) * 0.5 - A / 4.0;
            x[1] = (-m - Squreroot(-temp[0] - p + 2.0 * q / m)) * 0.5 - A / 4.0;
            x[2] = (m + Squreroot(-temp[0] - p - 2.0 * q / m)) * 0.5 - A / 4.0;
            x[3] = (m - Squreroot(-temp[0] - p - 2.0 * q / m)) * 0.5 - A / 4.0;
            for (int i = 0; i < x.Length; i++) {
                if (x[i].Imaginary == 0)
                    solve.Add(x[i].Real);
                Console.WriteLine($"複素数の解 {x[i]}");
            }
            return solve;
        }

        /// <summary>
        /// 3乗根(x^1/3)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Cuberoot(double x)
        {
            if (x > 0.0) {
                return Math.Pow(x, 1.0 / 3.0);
            } else {
                return -Math.Pow(-x, 1.0 / 3.0);
            }
        }

        /// <summary>
        /// 複素数の平方根
        /// </summary>
        /// <param name="x">複素数</param>
        /// <returns></returns>
        public Complex Squreroot(Complex x)
        {
            Complex y;
            double r = x.Magnitude; //  複素数の大きさ(Sqrt(x.Real * x.Real + x.Imaginary * x.Imaginary)) 
            double theta = x.Phase; //  複素数の位相(Atan2(x.Imaginary, x.Real))
            if (x.Imaginary == 0.0) {
                if (x.Real > 0.0) {
                    y = Math.Sqrt(r);
                } else {
                    y = Math.Sqrt(r) * Complex.ImaginaryOne;
                }
            } else {
                if (theta < 0.0) {
                    theta += 2.0 * Math.PI;
                }
                y = Math.Sqrt(r) * (Math.Cos(theta * 0.5) + Complex.ImaginaryOne * Math.Sin(theta * 0.5));
            }
            return y;
        }

        //  ---  統計処理  ---

        /// <summary>
        ///  ∑x リストの合計を求める
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>合計値</returns>
        public double getSumList(List<double> list)
        {
            return list.Sum();
        }

        /// <summary>
        ///  ∑x^2 リストの自乗和(二乗の合計)を求める
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>二乗和</returns>
        public double getSqrSumList(List<double> list)
        {
            return list.Select(x => x * x).Sum();
        }

        /// <summary>
        ///  ∑x / n 平均値
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>平均値</returns>
        public double getAverageList(List<double> list)
        {
            return getSumList(list) / list.Count();
        }

        /// <summary>
        ///  ∑(x-xm) 偏差の和(常に0)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>偏差の和</returns>
        public double getDevSumList(List<double> list)
        {
            double ave = getAverageList(list);
            return list.Select(x => x - ave).Sum();
        }

        /// <summary>
        ///  ∑(x-xm)^2　偏差の平方和
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>偏差の平方和</returns>
        public double getVerSumList(List<double> list)
        {
            double ave = getAverageList(list);
            return list.Select(x => (x - ave) * (x - ave)).Sum();
        }

        /// <summary>
        ///  s^2 = ∑(x-xm)^2 / n 分散(variance)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>分散値</returns>
        public double getVariance(List<double> list)
        {
            return getVerSumList(list) / list.Count();
        }

        /// <summary>
        ///  標準偏差(standard deviation)
        ///  s = sqrt(∑(x-xm)^2 / n)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>標準偏差</returns>
        public double getStdDev(List<double> list)
        {
            return Math.Sqrt(getVerSumList(list) / list.Count());
        }

        /// <summary>
        ///  ∑(x-xm)(y-ym) 偏差積和
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <returns>偏差積和</returns>
        public double getDevProductSumList(List<double> Xlist, List<double> Ylist)
        {
            double xave = getAverageList(Xlist);
            double yave = getAverageList(Ylist);
            double sum = 0;
            for (int i = 0; i < Math.Min(Xlist.Count, Ylist.Count); i++) {
                sum += (Xlist[i] - xave) * (Ylist[i] - yave);
            }
            return sum;
        }

        /// <summary>
        ///  共分散(Covariance)
        ///  Cov(x,y) = 1/n * Σ(x-xm)(y-ym)
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <returns>共分散</returns>
        public double getCovarince(List<double> Xlist, List<double> Ylist)
        {
            return getDevProductSumList(Xlist, Ylist) / Math.Min(Xlist.Count, Ylist.Count);
        }

        /// <summary>
        ///  相関係数(correlation coefficient)  ρ = σxy / (σx * σy)
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <returns>相関係数</returns>
        public double getCorelation(List<double> Xlist, List<double> Ylist)
        {
            return getCovarince(Xlist, Ylist) / (getStdDev(Xlist) * getStdDev(Ylist));
        }

        /// <summary>
        ///  回帰分析(regression analysis)の係数(a)の取得  y = ax + b
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <returns>係数(傾き a)</returns>
        public double getRegA(List<double> Xlist, List<double> Ylist)
        {
            return getDevProductSumList(Xlist, Ylist) / getVerSumList(Xlist);
        }

        /// <summary>
        ///  回帰分析(regression analysis)の係数(b)の取得  y = ax + b
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <returns>係数(切片 b)</returns>
        public double getRegB(List<double> Xlist, List<double> Ylist)
        {
            return getAverageList(Ylist) - getRegA(Xlist, Ylist) * getAverageList(Xlist);
        }

        /// <summary>
        ///  決定係数(coefficient of determination)
        //  R^2 = 1 - (Σ(y - f(x))^2 / Σ(y - ym)^2) (分散) 
        /// </summary>
        /// <param name="Xlist">Xデータリスト</param>
        /// <param name="Ylist">Yデータリスト</param>
        /// <param name="a">係数a(傾き)</param>
        /// <param name="b">係数b(切片</param>
        /// <returns>決定係数</returns>
        public double getCoefficentDeterminatio(List<double> Xlist, List<double> Ylist, double a, double b)
        {
            double sum = 0;
            for (int i = 0; i < Xlist.Count; i++) {
                double t = Ylist[i] - (a * Xlist[i] + b);
                sum += t * t;
            }
            return 1 - sum / getVerSumList(Ylist);
        }

        /// <summary>
        ///  ∑xy X*Yの合計
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>X*Yの合計値</returns>
        public double getXYSumList(List<PointD> list)
        {
            return list.Select(p => p.x * p.y).Sum();
        }

        /// <summary>
        ///  ∑(x-xm)(y-ym) 偏差積和
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>偏差積和</returns>
        public double getDevProductSumList(List<PointD> list)
        {
            double xave = getAverageList(list.Select(p => p.x).ToList());
            double yave = getAverageList(list.Select(p => p.y).ToList());
            return list.Select(p => (p.x - xave) * (p.y - yave)).Sum();
        }

        /// <summary>
        ///  共分散(Covariance)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>共分散値</returns>
        //  Cov(x,y) = 1/n * Σ(x-xm)(y-ym)
        public double getCovarince(List<PointD> list)
        {
            return getDevProductSumList(list) / list.Count;
        }

        /// <summary>
        ///  相関係数(correlation coefficient)  ρ = σxy / (σx * σy)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>相関係数</returns>
        public double getCorelation(List<PointD> list)
        {
            return getCovarince(list) / (getStdDev(list.Select(p => p.x).ToList()) * getStdDev(list.Select(p => p.y).ToList()));
        }

        /// <summary>
        ///  回帰分析(regression analysis)の係数(a)の取得  y = ax + b
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>係数a(傾き)</returns>
        public double getRegA(List<PointD> list)
        {
            return getDevProductSumList(list) / getVerSumList(list.Select(p => p.x).ToList());
        }

        /// <summary>
        ///  回帰分析(regression analysis)の係数(b)の取得  y = ax + b
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <returns>係数b(切片)</returns>
        public double getRegB(List<PointD> list)
        {
            return getAverageList(list.Select(p => p.y).ToList()) - getRegA(list) * getAverageList(list.Select(p => p.x).ToList());
        }

        /// <summary>
        ///  決定係数(coefficient of determination)
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <param name="a">係数a(傾き)</param>
        /// <param name="b">係数b(切片</param>
        /// <returns>決定係数</returns>
        //  R^2 = 1 - (Σ(y - f(x))^2 / Σ(y - ym)^2) (分散) 
        public double getCoefficentDeterminatio(List<PointD> list, double a, double b)
        {
            double sum = 0;
            for (int i = 0; i < list.Count; i++) {
                double t = list[i].y - (a * list[i].x + b);
                sum += t * t;
            }
            return 1 - sum / getVerSumList(list.Select(p => p.y).ToList());
        }

        /// <summary>
        /// 理論値に対する分散(回帰曲線の残差の二乗和)
        ///  R^2 = Σ(y - f(x))^2 / n
        /// </summary>
        /// <param name="list">データリスト</param>
        /// <param name="a">係数a(傾き)</param>
        /// <param name="b">係数b(切片</param>
        /// <returns>残差の二乗和</returns>
        public double getRegVariance(List<PointD> list, double a, double b)
        {
            double sum = 0;
            for (int i = 0; i < list.Count; i++) {
                double t = list[i].y - (a * list[i].x + b);
                sum += t * t;
            }
            return sum / list.Count;
        }

        //  ------  座標処理  --------

        //	地球上の２地点の緯度・経度を指定して最短距離とその方位角を計算
        //	地球を赤道半径r=6378.137kmを半径とする球体として計算しています。
        //	方位角は北:0度、東:90度、南:180度、西:270度。
        //	地点A(経度x1, 緯度y1)、地点B(経度x2, 緯度y2)
        //	ABの距離(km) d = r*acos(sin(y1)*sin(y2)+cos(y1)*cos(y2)*cos(x2-x1))
        //	方位角　φ = 90 - atan2(sin(x2-x1), cos(y1)*tan(y2) - sin(y1)*cos(x2-x1))
        //	http://keisan.casio.jp/has10/SpecExec.cgi

        /// <summary>
        /// 球面上の2点間座標の距離
        /// </summary>
        /// <param name="longi1">座標1経度</param>
        /// <param name="lati1">座標1緯度</param>
        /// <param name="longi2">座標2経度</param>
        /// <param name="lati2">座標2緯度</param>
        /// <returns>距離(km)</returns>
        public double coordinateDistance(double longi1, double lati1, double longi2, double lati2)
        {
            double r = 6378.137;
            double x1 = longi1 / 180 * Math.PI;
            double y1 = lati1 / 180 * Math.PI;
            double x2 = longi2 / 180 * Math.PI;
            double y2 = lati2 / 180 * Math.PI;
            double dis = r * Math.Acos(Math.Sin(y1) * Math.Sin(y2) + Math.Cos(y1) * Math.Cos(y2) * Math.Cos(x2 - x1));
            return double.IsNaN(dis) ? 0 : dis;
        }

        /// <summary>
        /// 球面上の2点間座標の距離(static関数)
        /// </summary>
        /// <param name="longi1">座標1経度</param>
        /// <param name="lati1">座標1緯度</param>
        /// <param name="longi2">座標2経度</param>
        /// <param name="lati2">座標2緯度</param>
        /// <returns>距離(km)</returns>
        public static double CoordinateDistance(double longi1, double lati1, double longi2, double lati2)
        {
            double r = 6378.137;
            double x1 = longi1 / 180 * Math.PI;
            double y1 = lati1 / 180 * Math.PI;
            double x2 = longi2 / 180 * Math.PI;
            double y2 = lati2 / 180 * Math.PI;
            double dis = r * Math.Acos(Math.Sin(y1) * Math.Sin(y2) + Math.Cos(y1) * Math.Cos(y2) * Math.Cos(x2 - x1));
            return double.IsNaN(dis) ? 0 : dis;
        }

        /// <summary>
        /// 球面上の2点間座標の距離
        /// </summary>
        /// <param name="ps">緯度経度座標</param>
        /// <param name="pe">緯度経度座標</param>
        /// <returns>距離(km)</returns>
        public double coordinateDistance(System.Windows.Point ps, System.Windows.Point pe)
        {
            return coordinateDistance(ps.X, ps.Y, pe.X, pe.Y);
        }

        /// <summary>
        /// 球面上の2点間座標の距離(static関数)
        /// </summary>
        /// <param name="ps">緯度経度座標</param>
        /// <param name="pe">緯度経度座標</param>
        /// <returns>距離(km)</returns>
        public static double CoordinateDistance(System.Windows.Point ps, System.Windows.Point pe)
        {
            return CoordinateDistance(ps.X, ps.Y, pe.X, pe.Y);
        }


        /// <summary>
        /// 球面上の2点間座標の方位
        /// </summary>
        /// <param name="longi1">座標1経度</param>
        /// <param name="lati1">座標1緯度</param>
        /// <param name="longi2">座標2経度</param>
        /// <param name="lati2">座標2緯度</param>
        /// <returns>方位角(°)</returns>
        public double azimuth(double longi1, double lati1, double longi2, double lati2)
        {
            double x1 = longi1 / 180 * Math.PI;
            double y1 = lati1 / 180 * Math.PI;
            double x2 = longi2 / 180 * Math.PI;
            double y2 = lati2 / 180 * Math.PI;
            double phai = 90 - (Math.Atan2(Math.Sin(x2 - x1), Math.Cos(y1) * Math.Tan(y2) - Math.Sin(y1) * Math.Cos(x2 - x1))) * 180 / Math.PI;
            return phai;
        }

        /// <summary>
        /// 球面上の2点間座標の方位
        /// </summary>
        /// <param name="longi1">座標1経度</param>
        /// <param name="lati1">座標1緯度</param>
        /// <param name="longi2">座標2経度</param>
        /// <param name="lati2">座標2緯度</param>
        /// <returns>方位角(°)</returns>
        public double azimuth2(double longi1, double lati1, double longi2, double lati2)
        {
            double l1 = longi1 / 180 * Math.PI;         //	経度
            double b1 = lati1 / 180 * Math.PI;          //	緯度
            double l2 = longi2 / 180 * Math.PI;
            double b2 = lati2 / 180 * Math.PI;
            double Y = Math.Cos(b2) * Math.Sin(l2 - l1);
            double X = Math.Cos(b1) * Math.Sin(b2) - Math.Sin(b1) * Math.Cos(b2) * Math.Cos(l2 - l1);
            double phai = (Math.Atan2(Y, X)) * 180 / Math.PI;
            return phai < 0 ? phai + 360 : phai;
        }

        //  ---  行列計算  ---
        //  https://qiita.com/sekky0816/items/8c73a7ec32fd9b040127

        /// <summary>
        /// 単位行列の作成(正方行列のみ)
        /// </summary>
        /// <param name="unit">行列のサイズ</param>
        /// <returns>単位行列</returns>
        public double[,] unitMatrix(int unit)
        {
            double[,] A = new double[unit, unit];
            for (int i = 0; i < unit; i++)
                for (int j = 0; j < unit; j++)
                    if (i == j)
                        A[i, j] = 1;
                    else
                        A[i, j] = 0;
            return A;
        }

        /// <summary>
        /// 転置行列  行列Aの転置A^T
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>転置行列 AT</returns>
        public double[,] matrixTranspose(double[,] A)
        {
            double[,] AT = new double[A.GetLength(1), A.GetLength(0)];
            for (int i = 0; i < A.GetLength(1); i++) {
                for (int j = 0; j < A.GetLength(0); j++) {
                    AT[i, j] = A[j, i];
                }
            }
            return AT;
        }

        /// <summary>
        /// 行列の積  AxB
        /// 行列の積では 結合の法則  (AxB)xC = Ax(BxC) , 分配の法則 (A+B)xC = AxC+BxC , Cx(A+B) = CxA + CxB　が可
        /// 交換の法則は成立しない  AxB ≠ BxA
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <param name="B">行列 B</param>
        /// <returns>行列の積</returns>
        public double[,] matrixMulti(double[,] A, double[,] B)
        {
            double[,] product = new double[A.GetLength(0), B.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++) {
                for (int j = 0; j < B.GetLength(1); j++) {
                    for (int k = 0; k < A.GetLength(1); k++) {
                        product[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return product;
        }

        /// <summary>
        /// 行列の和 A+B
        /// 異なるサイズの行列はゼロ行列にする
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <param name="B">行列 B</param>
        /// <returns>行列の和</returns>
        public double[,] matrixAdd(double[,] A, double[,] B)
        {
            double[,] sum = new double[A.GetLength(0), A.GetLength(1)];
            if (A.GetLength(0) == B.GetLength(0) && A.GetLength(1) == B.GetLength(1)) {
                for (int i = 0; i < A.GetLength(0); i++) {
                    for (int j = 0; j < A.GetLength(1); j++) {
                        sum[i, j] = A[i, j] + B[i, j];
                    }
                }
            }
            return sum;
        }


        /// <summary>
        /// 逆行列 A^-1 (ある行列で線形変換した空間を元に戻す行列)
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>逆行列</returns>
        public double[,] matrixInverse(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            double[,] invA = new double[n, m];
            double[,] AA = copyMatrix(A);
            if (n == m) {
                //  正方行列
                int max;
                double tmp;
                for (int j = 0; j < n; j++) {
                    for (int i = 0; i < n; i++) {
                        invA[j, i] = (i == j) ? 1 : 0;
                    }
                }
                for (int k = 0; k < n; k++) {
                    max = k;
                    for (int j = k + 1; j < n; j++) {
                        if (Math.Abs(AA[j, k]) > Math.Abs(AA[max, k])) {
                            max = j;
                        }
                    }
                    if (max != k) {
                        for (int i = 0; i < n; i++) {
                            //  入力行列側
                            tmp = AA[max, i];
                            AA[max, i] = AA[k, i];
                            AA[k, i] = tmp;
                            //  単位行列側
                            tmp = invA[max, i];
                            invA[max, i] = invA[k, i];
                            invA[k, i] = tmp;
                        }
                    }
                    tmp = AA[k, k];
                    for (int i = 0; i < n; i++) {
                        AA[k, i] /= tmp;
                        invA[k, i] /= tmp;
                    }
                    for (int j = 0; j < n; j++) {
                        if (j != k) {
                            tmp = AA[j, k] / AA[k, k];
                            for (int i = 0; i < n; i++) {
                                AA[j, i] = AA[j, i] - AA[k, i] * tmp;
                                invA[j, i] = invA[j, i] - invA[k, i] * tmp;
                            }
                        }
                    }
                }
                //  逆行列が計算できなかった場合
                for (int j = 0; j < n; j++) {
                    for (int i = 0; i < n; i++) {
                        if (double.IsNaN(invA[j, i])) {
                            Debug.WriteLine("Error: Unable to compute inverse matrix");
                            invA[j, i] = 0;     //  とりあえず0に置き換える
                        }
                    }
                }
                return invA;
            } else {
                Debug.WriteLine("Error: It is not a squre matrix");
                return invA;
            }
        }

        /// <summary>
        /// 行列のコピー
        /// </summary>
        /// <param name="A">行列 A</param>
        /// <returns>コピー行列</returns>
        public double[,] copyMatrix(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            double[,] AA = new double[n, m];
            for (int i = 0; i < A.GetLength(0); i++) {
                for (int j = 0; j < A.GetLength(1); j++) {
                    AA[i, j] = A[i, j];
                }
            }
            return AA;
        }

        //  ---  座標変換関連  ------

        /// <summary>
        /// 移動量を3D変換マトリックス(4x4)に設定
        /// </summary>
        /// <param name="dx">X軸方向の移動量</param>
        /// <param name="dy">Y軸方向の移動量</param>
        /// <param name="dz">Z軸方向の移動量</param>
        /// <returns>変換マトリックス</returns>
        public double[,] translate3DMatrix(double dx, double dy, double dz)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = 1;
            mp[1, 1] = 1;
            mp[2, 2] = 1;
            mp[3, 0] = dx;
            mp[3, 1] = dy;
            mp[3, 2] = dz;
            mp[3, 3] = 1;
            return mp;
        }

        /// <summary>
        /// X軸回転を3D変換マトリックス(4x4)に設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        /// <returns>変換マトリックス</returns>
        public double[,] rotateX3DMatrix(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = 1.0;
            mp[1, 1] = Math.Cos(th);
            mp[1, 2] = Math.Sin(th);
            mp[2, 1] = -Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            return mp;
        }

        /// <summary>
        /// Y軸回転を3D変換マトリックス(4x4)に設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        /// <returns>変換マトリックス</returns>
        public double[,] rotateY3DMatrix(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = Math.Cos(th);
            mp[0, 2] = -Math.Sin(th);
            mp[1, 1] = 1.0;
            mp[2, 0] = Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            return mp;
        }

        /// <summary>
        /// Z軸回転を3D変換マトリックス(4x4)に設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        /// <returns>変換マトリックス</returns>
        public double[,] rotateZ3DMatrix(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = Math.Cos(th);
            mp[0, 1] = Math.Sin(th);
            mp[1, 0] = -Math.Sin(th);
            mp[1, 1] = Math.Cos(th);
            mp[2, 2] = 1.0;
            mp[3, 3] = 1.0;
            return mp;
        }

        /// <summary>
        /// 任意の軸を中心に回転させる
        /// </summary>
        /// <param name="vec">任意の軸(単位ベクトル)</param>
        /// <param name="th">回転角(rad)</param>
        /// <returns>変換マトリックス</returns>
        public double[,] rotate(Point3D vec, double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = vec.x * vec.x + (1 - vec.x * vec.x) * Math.Cos(th);
            mp[0, 1] = vec.x * vec.y * (1 - Math.Cos(th)) - vec.z * Math.Sin(th);
            mp[0, 2] = vec.y * vec.z * (1 - Math.Cos(th)) + vec.y * Math.Sin(th);
            mp[1, 0] = vec.x * vec.y * (1 - Math.Cos(th)) + vec.z * Math.Sin(th);
            mp[1, 1] = vec.y * vec.y + (1 - vec.y * vec.y) * Math.Cos(th);
            mp[1, 2] = vec.y * vec.z * (1 - Math.Cos(th)) - vec.x * Math.Sin(th);
            mp[2, 0] = vec.y * vec.z * (1 - Math.Cos(th)) - vec.y * Math.Sin(th);
            mp[2, 1] = vec.y * vec.z * (1 - Math.Cos(th)) + vec.x * Math.Sin(th);
            mp[2, 2] = vec.z * vec.z + (1 - vec.z * vec.z) * Math.Cos(th);
            mp[3, 3] = 1.0;
            return mp;
        }


        /// <summary>
        ///  拡大縮小のスケール値を3D変換マトリックス(4x4)に設定
        /// </summary>
        /// <param name="sx">X方向縮尺</param>
        /// <param name="sy">Y方向縮尺</param>
        /// <param name="sz">Z方向縮尺</param>
        /// <returns>変換マトリックス</returns>
        public double[,] scale3DMatrix(double sx, double sy, double sz)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = sx;
            mp[1, 1] = sy;
            mp[2, 2] = sz;
            mp[3, 3] = 1.0;
            return mp;
        }

        /// <summary>
        /// 移動量を2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <param name="dx">X軸方向の移動量</param>
        /// <param name="dy">Y軸方向の移動量</param>
        /// <returns>変換マトリックス</returns>
        public double[,] translate2DMatrix(double dx, double dy)
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = 1;
            mp[1, 1] = 1;
            mp[2, 0] = dx;
            mp[2, 1] = dy;
            mp[2, 2] = 1;
            return mp;
        }

        /// <summary>
        /// 原点回転を2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        /// <returns>変換マトリックス</returns>
        public double[,] rotate2DMatrix(double th)
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = Math.Cos(th);
            mp[0, 1] = Math.Sin(th);
            mp[1, 0] = -Math.Sin(th);
            mp[1, 1] = Math.Cos(th);
            mp[2, 2] = 1.0;
            return mp;
        }

        /// <summary>
        ///  拡大縮小のスケール値を2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <param name="sx">X方向縮尺</param>
        /// <param name="sy">Y方向縮尺</param>
        /// <returns>変換マトリックス</returns>
        public double[,] scale2DMatrix(double sx, double sy)
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = sx;
            mp[1, 1] = sy;
            mp[2, 2] = 1.0;
            return mp;
        }

        /// <summary>
        /// 原点に対して反転する2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <returns>変換マトリックス</returns>
        public double[,] inverse2DMatrix()
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = -1;
            mp[1, 1] = -1;
            mp[2, 2] = 1;
            return mp;
        }

        /// <summary>
        /// X軸に対して反転する2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <returns>変換マトリックス</returns>
        public double[,] inverseX2DMatrix()
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = 1;
            mp[1, 1] = -1;
            mp[2, 2] = 1;
            return mp;
        }

        /// <summary>
        /// Y軸に対して反転する2D変換マトリックス(3x3)に設定
        /// </summary>
        /// <returns>変換マトリックス</returns>
        public double[,] inverseY2DMatrix()
        {
            double[,] mp = new double[3, 3];
            mp[0, 0] = -1;
            mp[1, 1] = 1;
            mp[2, 2] = 1;
            return mp;
        }

    }
}
