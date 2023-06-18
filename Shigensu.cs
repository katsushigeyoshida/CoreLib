using CoreLib;
using System;

namespace CoreLib
{
    /// <summary>
    /// 四元数(クオタニオン)
    /// </summary>
    public class Shigensu
    {
        public double x;
        public double y;
        public double z;
        public double w;

        public Shigensu(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString()
        {
            return $"X:{x} Y:{y} Z:{z} W:{w}";
        }

        /// <summary>
        /// 共役クオタニオン
        /// </summary>
        /// <returns></returns>
        public Shigensu conjugate()
        {
            return new Shigensu(-x, -y, -z, w);
        }

        public static Shigensu conjugate(Shigensu q)
        {
            return new Shigensu(-q.x, -q.y, -q.z, q.w);
        }

        /// <summary>
        /// クオタニオン同士の加算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Shigensu operator +(Shigensu a, Shigensu b)
        {
            return new Shigensu(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        /// <summary>
        /// クオタニオン同士の乗算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Shigensu operator *(Shigensu a, Shigensu b)
        {
            return new Shigensu(
                 a.w * b.x - a.z * b.y + a.y * b.z + a.x * b.w,
                 a.z * b.x + a.w * b.y - a.x * b.z + a.y * b.w,
                -a.y * b.x + a.x * b.y + a.w * b.z + a.z * b.w,
                -a.x * b.x - a.y * b.y - a.z * b.z + a.w * b.w
            );
        }

        /// <summary>
        /// クオタニオンの加算
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Shigensu add(Shigensu q)
        {
            return new Shigensu(x + q.x, y + q.y, z + q.z, w + q.w);
        }

        /// <summary>
        /// クオタニオンの乗算
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public Shigensu multi(Shigensu q)
        {
            return new Shigensu(
                 w * q.x - z * q.y + y * q.z + x * q.w,
                 z * q.x + w * q.y - x * q.z + y * q.w,
                -y * q.x + x * q.y + w * q.z + z * q.w,
                -x * q.x - y * q.y - z * q.z + w * q.w
            );
        }

        /// <summary>
        /// ベクトルをクオタニオンで回転
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point3D rotate(Point3D v)
        {
            var vq = new Shigensu(v.x, v.y, v.z, 0.0);
            var cq = conjugate();
            var mq = multi(vq);
            mq = mq.multi(cq);
            return new Point3D(mq.x, mq.y, mq.z);
        }

        /// <summary>
        /// クオタニオンから回転行列への変換
        /// http://marupeke296.sakura.ne.jp/DXG_No58_RotQuaternionTrans.html
        /// </summary>
        /// <returns></returns>
        public double[,] rotateMatrix()
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = 1 - 2 * y * y - 2 * z * z;
            mp[0, 1] = 2 * x * y + 2 * w * z;
            mp[0, 2] = 2 * x * z - 2 * w * y;
            mp[1, 0] = 2 * x * y - 2 * w * z;
            mp[1, 1] = 1 - 2 * x * x - 2 * z * z;
            mp[1, 2] = 2 * y * z + 2 * w * x;
            mp[2, 0] = 2 * x * z + 2 * w * y;
            mp[2, 1] = 2 * y * z - 2 * w * x;
            mp[2, 2] = 1 - 2 * x * x - 2 * y * y;
            return mp;
        }

        /// <summary>
        /// 回転行列からクオタニオンへの変換
        /// https://qiita.com/aa_debdeb/items/abe90a9bd0b4809813da
        /// </summary>
        /// <param name="mp"></param>
        public void toShigensu(double[,] mp)
        {
            double px =  mp[0, 0] - mp[1, 1] - mp[2, 2] + 1;
            double py = -mp[0, 0] + mp[1, 1] - mp[2, 2] + 1;
            double pz = -mp[0, 0] - mp[1, 1] + mp[2, 2] + 1;
            double pw =  mp[0, 0] + mp[1, 1] + mp[2, 2] + 1;

            int selected = 0;
            double max = px;
            if (max < py) {
                selected = 1;
                max = py;
            }
            if (max < pz) {
                selected = 2;
                max = pz;
            }
            if (max < pw) {
                selected = 3;
                max = pw;
            }
            if (selected == 0) {
                double x = Math.Sqrt(px) * 0.5;
                double d = 1 / (4 * x);
                this.x = x;
                this.y = (mp[1, 0] + mp[0, 1]) * d;
                this.z = (mp[0, 2] + mp[2, 0]) * d;
                this.w = (mp[2, 1] - mp[1, 2]) * d;
            } else if (selected == 1) {
                double y = Math.Sqrt(py) * 0.5;
                double d = 1 / (4 * y);
                this.x = (mp[1, 0] + mp[0, 1]) * d;
                this.y = y;
                this.z = (mp[2, 1] + mp[1, 2]) * d;
                this.w = (mp[0, 2] - mp[2, 0]) * d;
            } else if (selected == 2) {
                double z = Math.Sqrt(pz) * 0.5;
                double d = 1 / (4 * z);
                this.x = (mp[0, 2] + mp[2, 0]) * d;
                this.y = (mp[2, 1] + mp[1, 2]) * d;
                this.z = z;
                this.w = (mp[1, 0] - mp[0, 1]) * d;
            } else if (selected == 3) {
                double w = Math.Sqrt(pw) * 0.5;
                double d = 1 / (4 * w);
                this.x = (mp[2, 1] - mp[1, 2]) * d;
                this.y = (mp[0, 2] - mp[2, 0]) * d;
                this.z = (mp[1, 0] - mp[0, 1]) * d;
                this.w = w;
            }
        }
    }
}