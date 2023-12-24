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
        /// <param name="p">3D座標</param>
        public Point3D(Point3D p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }

        /// <summary>
        /// コンストラクタ(PointD → Poin3D)
        /// </summary>
        /// <param name="p">2D座標</param>
        /// <param face="p">面の向き(0:XY 1:YZ 2:ZX)</param>
        public Point3D(PointD p, int face = 0)
        {
            if (face == 1) {
                y = p.x;
                z = p.y;
            } else if(face == 2) {
                z = p.x;
                x = p.y;
            } else {
                x = p.x;
                y = p.y;
            }
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
        /// コピーデータを返す
        /// </summary>
        /// <returns>コピー座標</returns>
        public Point3D toCopy()
        {
            Point3D p = new Point3D();
            p.x = this.x;
            p.y = this.y;
            p.z = this.z;
            return p;
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
        /// 座標値を0クリアする
        /// </summary>
        public void clear()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.z = 0.0;
        }

        /// <summary>
        /// 座標値が0かの確認
        /// </summary>
        /// <returns>0状態</returns>
        public bool isEmpty()
        {
            if (x == 0 && y ==0 && z == 0)
                return true;
            else
                return false;
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

        /// <summary>
        /// 自点座標から指定座標へのベクトル
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>ベクトル</returns>
        public Point3D vector(Point3D p)
        {
            return new Point3D(p.x - x, p.y - y, p.z - z);
        }

        /// <summary>
        /// ベクトル同士の角度(内積から角度を求める)
        /// 内積(a1b1+a2b2+a3b3) = |a||b|cos(θ)
        /// cos(θ) = (a1b1+a2b2+a3b3)/(|a||b|)
        /// </summary>
        /// <param name="v">対象ベクトル</param>
        /// <returns>角度(rad)</returns>
        public double angle(Point3D v)
        {
            double costheta = (x * v.x + y * v.y + z * v.z) / 
                (Math.Sqrt(x * x + y * y + z * z) * Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z));
            return Math.Acos(costheta);
        }

        /// <summary>
        /// 自ベクトルと指定ベクトルの内積(合成ベクトルの長さ)
        /// </summary>
        /// <param name="p">指定ベクトル</param>
        /// <returns>内積</returns>
        public double innerProduct(Point3D p)
        {
            return (x * p.x + y * p.y + z * p.z);
        }

        /// <summary>
        /// 自ベクトルと指定ベクトルの外積(２つのベクトルが作る平行四辺形に垂直のベクトル)
        /// </summary>
        /// <param name="p">指定ベクトル</param>
        /// <returns>内積</returns>
        public Point3D crossProduct(Point3D p)
        {
            Point3D v = new Point3D();
            v.x =  y * p.z - p.y * z;
            v.y = -x * p.z + p.x * z;
            v.z =  x * p.y - p.x * y;
            return v;
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
            //setPoint3D(toMatrix(mp));
            matrix(mp);
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
            //setPoint3D(toMatrix(mp));
            matrix(mp);
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
            //setPoint3D(toMatrix(mp));
            matrix(mp);
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
            //setPoint3D(toMatrix(mp));
            matrix(mp);
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
            //setPoint3D(toMatrix(mp));
            matrix(mp);
        }

        /// <summary>
        /// アフィン変換のマトリックス計算
        /// </summary>
        /// <param name="mp"></param>
        /// <returns></returns>
        public Point3D toMatrix(double[,] mp)
        {
            Point3D p = new Point3D();
            p.x = mp[0, 0] * x + mp[0, 1] * y + mp[0, 2] * z + mp[3, 0];
            p.y = mp[1, 0] * x + mp[1, 1] * y + mp[1, 2] * z + mp[3, 1];
            p.z = mp[2, 0] * x + mp[2, 1] * y + mp[2, 2] * z + mp[3, 2];
            return p;
        }

        /// <summary>
        /// アフィン変換のマトリックス計算
        /// </summary>
        /// <param name="mp">変換マトリックス</param>
        public void matrix(double[,] mp)
        {
            double tx = mp[0, 0] * x + mp[0, 1] * y + mp[0, 2] * z + mp[3, 0];
            double ty = mp[1, 0] * x + mp[1, 1] * y + mp[1, 2] * z + mp[3, 1];
            double tz = mp[2, 0] * x + mp[2, 1] * y + mp[2, 2] * z + mp[3, 2];
            x = tx;
            y = ty;
            z = tz;
        }
    }
}
