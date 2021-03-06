using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class Point3D
    {
        public double x = 0.0;
        public double y = 0.0;
        public double z = 0.0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Point3D()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="z">z座標</param>
        public Point3D(double x,double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">座標</param>
        public Point3D(Point3D p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        } 

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }

        public void setPoint3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void setPoint3D(Point3D p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }

        /// <summary>
        /// XY座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointXY()
        {
            return new PointD(x, y);
        }

        /// <summary>
        /// YZ座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointYZ()
        {
            return new PointD(y, z);
        }

        /// <summary>
        /// ZX座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointZX()
        {
            return new PointD(z, x);
        }

        /// <summary>
        /// 座標値を〇クリアする
        /// </summary>
        public void clear()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.z = 0.0;
        }

        /// <summary>
        /// 座標データの値の符号を反転する
        /// </summary>
        public void inverse()
        {
            this.x *= -1.0;
            this.y *= -1.0;
            this.z *= -1.0;
        }

        /// <summary>
        /// 座標をオフセット分移動させる
        /// </summary>
        /// <param name="offset"></param>
        public void offset(Point3D offset)
        {
            this.x += offset.x;
            this.y += offset.y;
            this.z += offset.z;
        }

        public Point3D add(double x, double y, double z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// 指定分を加えた値を返す
        /// </summary>
        /// <param name="p">指定値</param>
        /// <returns>加えた値</returns>
        public Point3D add(Point3D p)
        {
            this.x += p.x;
            this.y += p.y;
            this.z += p.z;
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// 指定分を引いた値を返す
        /// </summary>
        /// <param name="p">指定値</param>
        /// <returns>引いた値</returns>
        public Point3D sub(Point3D p)
        {
            this.x -= p.x;
            this.y -= p.y;
            this.z -= p.z;
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// 座標データの移動
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        public void tarnslate(double dx, double dy, double dz)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = 1;
            mp[1, 1] = 1;
            mp[2, 2] = 1;
            mp[3, 0] = dx;
            mp[3, 1] = dy;
            mp[3, 2] = dz;
            mp[3, 3] = 1;
            setPoint3D(matrix(mp));
        }

        /// <summary>
        /// 座標データをX軸で回転
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void rotateX(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = 1.0;
            mp[1, 1] = Math.Cos(th);
            mp[1, 2] = Math.Sin(th);
            mp[2, 1] = -Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            setPoint3D(matrix(mp));
        }


        /// <summary>
        /// 座標データをY軸で回転
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void rotateY(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = Math.Cos(th);
            mp[0, 2] = -Math.Sin(th);
            mp[1, 1] = 1.0;
            mp[2, 0] = Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            setPoint3D(matrix(mp));
        }


        /// <summary>
        /// 座標データをZ軸で回転
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void rotateZ(double th)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = Math.Cos(th);
            mp[0, 1] = Math.Sin(th);
            mp[1, 0] = -Math.Sin(th);
            mp[1, 1] = Math.Cos(th);
            mp[2, 2] = 1.0;
            mp[3, 3] = 1.0;
            setPoint3D(matrix(mp));
        }

        /// <summary>
        /// 座標データを原点を中心に拡大・縮小する
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        /// <param name="sz"></param>
        public void scale(double sx, double sy, double sz)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = sx;
            mp[1, 1] = sy;
            mp[2, 2] = sz;
            mp[3, 3] = 1.0;
            setPoint3D(matrix(mp));
        }

        /// <summary>
        /// アフィン変換のマトリックス計算
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        Point3D matrix(double[,] mp)
        {
            Point3D p = new Point3D();
            p.x = mp[0, 0] * x + mp[0, 1] * y + mp[0, 2] * z + mp[0, 3];
            p.y = mp[1, 0] * x + mp[1, 1] * y + mp[1, 2] * z + mp[1, 3];
            p.z = mp[2, 0] * x + mp[2, 1] * y + mp[2, 2] * z + mp[2, 3];
            return p;
        }
    }
}
