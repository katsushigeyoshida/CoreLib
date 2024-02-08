using System;

namespace CoreLib
{
    /// <summary>
    /// 3D座標
    /// 
    /// Point3D()                                                       コンストラクタ
    /// Point3D(double x,double y, double z)                            コンストラクタ
    /// Point3D(Point3D p)                                              コンストラクタ
    /// Point3D(PointD p, FACE3D face = FACE3D.XY)                      コンストラクタ
    /// string ToString()                                               文字列に変換
    /// string ToString(string form)                                    書式を指定して文字列に変換
    /// 
    /// </summary>

    /// <summary>
    /// 3Dでの表示面
    /// </summary>
    public enum FACE3D { XY, YX, YZ, ZY, ZX, XZ, NON }

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
        /// <param name="x"></param>
        public Point3D(double x)
        {
            this.x = x;
            this.y = x;
            this.z = x;
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
        /// <param face="p">面の向き(0:XY 1:YZ 2:ZX 3:YX 4:ZY 5:XZ)</param>
        public Point3D(PointD p, FACE3D face = FACE3D.XY)
        {
            if (face == FACE3D.XY) {
                x = p.x;
                y = p.y;
            } else if (face == FACE3D.YZ) {
                y = p.x;
                z = p.y;
            } else if(face == FACE3D.ZX) {
                z = p.x;
                x = p.y;
            } else if (face == FACE3D.YX) {
                y = p.x;
                x = p.y;
            } else if (face == FACE3D.ZY) {
                z = p.x;
                y = p.y;
            } else if (face == FACE3D.XZ) {
                x = p.x;
                z = p.y;
            } else {
                x = p.x;
                y = p.y;
            }
        }

        /// <summary>
        /// Uベクター X軸の向き(平面での方向単位ベクト
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>X軸ベクトル</returns>
        public static Point3D getUVector(FACE3D face)
        {
            if (face == FACE3D.XY || face == FACE3D.XZ) {
                return new Point3D(1, 0, 0);
            } else if (face == FACE3D.YZ || face == FACE3D.YX) {
                return new Point3D(0, 1, 0);
            } else if (face == FACE3D.ZX || face == FACE3D.ZY) {
                return new Point3D(0, 0, 1);
            } else {
                return new Point3D(1, 0, 0);
            }
        }

        /// <summary>
        /// Vベクター Y軸の向き(平面でuに垂直な方向の単位ベクトル)
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>Y軸ベクトル</returns>
        public static Point3D getVVector(FACE3D face)
        {
            if (face == FACE3D.XY || face == FACE3D.ZY) {
                return new Point3D(0, 1, 0);
            } else if (face == FACE3D.YZ || face == FACE3D.XZ) {
                return new Point3D(0, 0, 1);
            } else if (face == FACE3D.ZX || face == FACE3D.YX) {
                return new Point3D(1, 0, 0);
            } else {
                return new Point3D(0, 1, 0);
            }
        }

        /// <summary>
        /// 2D平面の座標を3D座標に変換
        ///  P(u,v) = c + u * x + v * y
        /// </summary>
        /// <param name="p">2D座標</param>
        /// <param name="cp">2D平面の中心座標</param>
        /// <param name="u">2D平面のX軸向き</param>
        /// <param name="v">2D座標のY軸の向き</param>
        /// <returns>3D座標</returns>
        public static Point3D cnvPlaneLocation(PointD p, Point3D cp, Point3D u, Point3D v)
        {
            Point3D uv = u * p.x + v * p.y;
            return cp + uv;
        }

        /// <summary>
        /// 3D座標から平面座標に変換
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <param name="cp">2D平面の中心座標</param>
        /// <param name="u">2D平面のX軸向き</param>
        /// <param name="v">2D座標のY軸の向き</param>
        /// <returns>2D座標</returns>
        public static PointD cnvPlaneLocation(Point3D pos, Point3D cp, Point3D u, Point3D v)
        {
            PointD t = new PointD();
            Point3D p = pos - cp;
            double a = (u.y * v.x - u.x * v.y);
            double b = (u.z * v.y - u.y * v.z);
            double c = (u.x * v.z - u.z * v.x);
            if (a != 0) {
                t.x = (p.y * v.x - p.x * v.y) / a;
                t.y = (p.x * u.y - p.y * u.x) / a;
            } else if (b != 0) {
                t.x = (p.z * v.y - p.y * v.z) / b;
                t.y = (p.y * u.z - p.z * u.y) / b;
            } else {
                t.x = (p.x * v.z - p.z * v.x) / c;
                t.y = (p.z * u.x - p.x * u.z) / c;
            }
            return t;
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }

        /// <summary>
        /// 書式を指定して文字列に変換
        /// </summary>
        /// <param name="form">書式</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            return "(" + x.ToString(form) + "," + y.ToString(form) + "," + z .ToString(form) + ")";
        }

        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="z">Z座標</param>
        public void setPoint3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 値の設定
        /// </summary>
        /// <param name="p">Point3D値</param>
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
        /// YX座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointYX()
        {
            return new PointD(y, x);
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
        /// ZY座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointZY()
        {
            return new PointD(z, y);
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
        /// XZ座標を2次元変換
        /// </summary>
        /// <returns>2D座標</returns>
        public PointD toPointXZ()
        {
            return new PointD(x, z);
        }

        /// <summary>
        /// 2次元変換
        /// </summary>
        /// <param name="face">表示面</param>
        /// <returns></returns>
        public PointD toPoint(FACE3D face)
        {
            if (face == FACE3D.XY)
                return toPointXY();
            else if (face == FACE3D.YZ)
                return toPointYZ();
            else if (face == FACE3D.ZX)
                return toPointZX();
            else if (face == FACE3D.YX)
                return toPointYX();
            else if (face == FACE3D.ZY)
                return toPointZY();
            else if (face == FACE3D.XZ)
                return toPointXZ();

            return null;
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
        /// 単位ベクトル化
        /// </summary>
        public void unit()
        {
            double l = length();
            x /= l;
            y /= l;
            z /= l;
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
        /// 原点からの距離またはベクタとしての長さを求める
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// 2点間の距離
        /// </summary>
        /// <param name="p">3D座標</param>
        /// <returns>距離</returns>
        public double length(Point3D p)
        {
            double dx = p.x - x;
            double dy = p.y - y;
            double dz = p.z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 方向を変えずにベクタの長さを設定
        /// </summary>
        /// <param name="size">長さ</param>
        public void length(double size)
        {
            double rate = size / length();
            x *= rate;
            y *= rate;
            z *= rate;
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
            costheta = Math.Min(costheta, 1.0);
            costheta = Math.Max(costheta, -1.0);
            return Math.Acos(costheta);
        }

        /// <summary>
        /// 自点を中心とした2点の角度
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        /// <returns>角度(rad)</returns>
        public double angle(Point3D ps, Point3D pe)
        {
            Point3D vs = ps - this;
            Point3D ve = pe - this;
            return vs.angle(ve);
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

        /// <summary>
        /// 法線ベクトルを求める
        /// </summary>
        /// <param name="p1">次の座標</param>
        /// <param name="p2">その次の座標</param>
        /// <returns>法線ベクトルのZ値</returns>
        public Point3D getNormal(Point3D p1, Point3D p2)
        {
            Point3D v = (p1 - this).crossProduct(p2 - p1);
            v.unit();
            return v;
        }

        /// <summary>
        /// 座標移動
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
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
        /// つのベクトルの加算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static Point3D operator +(Point3D v1, Point3D v2)
        {
            return new Point3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        /// <summary>
        /// クトルv1からベクトルv2を減算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static Point3D operator -(Point3D v1, Point3D v2)
        {
            return new Point3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        /// <summary>
        /// ベクトルv1をスカラー値mで乗算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="m">スカラー値</param>
        /// <returns>ベクトル</returns>
        public static Point3D operator *(Point3D v1, double m)
        {
            return new Point3D(v1.x * m, v1.y * m, v1.z * m);
        }

        /// <summary>
        /// 2 つのベクトルの要素の各ペアを乗算した値を値とするスカラー値を返す
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>スカラー値</returns>
        public static double operator *(Point3D v1, Point3D v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.y * v2.y;
        }

        /// <summary>
        /// クトルをスカラー値で除算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="m">スカラー値</param>
        /// <returns>ベクトル</returns>
        public static Point3D operator /(Point3D v1, double m)
        {
            return new Point3D(v1.x / m, v1.y / m, v1.z / m);
        }

        /// <summary>
        /// 最初のベクトルを 2 番目のベクトルで除算
        /// </summary>
        /// <param name="v1">ベクトル</param>
        /// <param name="v2">ベクトル</param>
        /// <returns>ベクトル</returns>
        public static Point3D operator /(Point3D v1, Point3D v2)
        {
            return new Point3D(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        /// <summary>
        /// 2点間の距離
        /// </summary>
        /// <param name="p1">始点</param>
        /// <param name="p2">終点</param>
        /// <returns>距離</returns>
        public double distance(Point3D p1, Point3D p2)
        {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2) + Math.Pow(p1.z - p2.z, 2));
        }

        /// <summary>
        /// 座標データの移動
        /// </summary>
        /// <param name="vec"></param>
        public void translate(Point3D vec)
        {
            translate(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// 座標データの回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角(rad)</param>
        /// <param name="face">表示面</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            cp.inverse();
            translate(cp);
            rotate(ang, face);
            cp.inverse();
            translate(cp);
        }

        /// <summary>
        /// 座標データを表示面で回転
        /// </summary>
        /// <param name="th">回転角</param>
        /// <param name="face">表示面</param>
        public void rotate(double th, FACE3D face)
        {
            if (face == FACE3D.XY)
                rotateZ(th);
            else if (face == FACE3D.YZ)
                rotateX(th);
            else if (face == FACE3D.ZX)
                rotateY(th);
        }

        /// <summary>
        /// 座標データの移動
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        public void translate(double dx, double dy, double dz)
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
        /// 任意の軸を中心に回転させる
        /// </summary>
        /// <param name="vec">任意の軸(単位ベクトル)</param>
        /// <param name="th">回転角(rad)</param>
        public void rotate(Point3D v, double th)
        {
            Point3D vec = v.toCopy();
            vec.unit();
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
            matrix(mp);
        }

        /// <summary>
        /// 座標データを原点を中心に拡大・縮小する
        /// </summary>
        /// <param name="vec"></param>
        public void scale(Point3D vec)
        {
            scale(vec.x, vec.y, vec.z);
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
        /// <param name="mp">変換マトリックス</param>
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

        /// <summary>
        /// アフィン変換のマトリックスを3Dから2DXYに変換
        /// </summary>
        /// <param name="mp">3Dマトリックス</param>
        /// <returns>2Dマトリックス</returns>
        public static double[,] toMatrix2DXY(double[,] mp)
        {
            double[,] xym = new double[3, 3];
            xym[0, 0] = mp[0, 0]; xym[0, 1] = mp[0, 1]; xym[0, 2] = mp[0, 3];
            xym[1, 0] = mp[1, 0]; xym[1, 1] = mp[1, 1]; xym[1, 2] = mp[1, 3];
            xym[2, 0] = mp[3, 0]; xym[2, 1] = mp[3, 1]; xym[2, 2] = mp[3, 3];
            return xym;
        }

        /// <summary>
        /// アフィン変換のマトリックスを3Dから2DYZに変換
        /// </summary>
        /// <param name="mp">3Dマトリックス</param>
        /// <returns>2Dマトリックス</returns>
        public static double[,] toMatrix2DYZ(double[,] mp)
        {
            double[,] xym = new double[3, 3];
            xym[0, 0] = mp[1, 1]; xym[0, 1] = mp[1, 2]; xym[0, 2] = mp[1, 3];
            xym[1, 0] = mp[2, 1]; xym[1, 1] = mp[2, 2]; xym[1, 2] = mp[2, 3];
            xym[2, 0] = mp[3, 1]; xym[2, 1] = mp[3, 2]; xym[2, 2] = mp[3, 3];
            return xym;
        }

        /// <summary>
        /// アフィン変換のマトリックスを3Dから2DZXに変換
        /// </summary>
        /// <param name="mp">3Dマトリックス</param>
        /// <returns>2Dマトリックス</returns>
        public static double[,] toMatrix2DZX(double[,] mp)
        {
            double[,] xym = new double[3, 3];
            xym[0, 0] = mp[2, 2]; xym[0, 1] = mp[2, 0]; xym[0, 2] = mp[2, 3];
            xym[1, 0] = mp[0, 2]; xym[1, 1] = mp[0, 0]; xym[1, 2] = mp[0, 3];
            xym[2, 0] = mp[3, 2]; xym[2, 1] = mp[3, 0]; xym[2, 2] = mp[3, 3];
            return xym;
        }
    }
}
