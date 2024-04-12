using System;

namespace CoreLib
{
    /// <summary>
    /// 3次元BOXクラス
    /// </summary>
    public class Box3D
    {
        public Point3D mMin;
        public Point3D mMax;

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Box3D()
        {
            mMin = new Point3D();
            mMax = new Point3D();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size">大きさ</param>
        public Box3D(double size)
        {
            mMin = new Point3D(-size / 2, -size / 2, -size / 2);
            mMax = new Point3D(size / 2, size / 2, size / 2);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">座標</param>
        public Box3D(Point3D p)
        {
            mMin = p.toCopy();
            mMax = p.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">座標</param>
        /// <param name="pe">座標</param>
        public Box3D(Point3D ps, Point3D pe)
        {
            mMin = ps.toCopy();
            mMax = pe.toCopy();
            normalize();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="b"></param>
        /// <param name="face"></param>
        public Box3D(Box b, FACE3D face)
        {
            mMin = new Point3D(b.BottomLeft, face);
            mMax = new Point3D(b.TopRight, face);
            normalize();
        }

        /// <summary>
        /// コンストラクタ(カンマセパレートで６個の数値文字列を変換))
        /// </summary>
        /// <param name="buf">文字列</param>
        public Box3D(string buf)
        {
            string[] data = buf.Split(new char[] { ',' });
            if (5 < data.Length) {
                mMin = new Point3D(
                    ylib.doubleParse(data[0]),
                    ylib.doubleParse(data[1]),
                    ylib.doubleParse(data[2]));
                mMax = new Point3D(
                    ylib.doubleParse(data[3]),
                    ylib.doubleParse(data[4]),
                    ylib.doubleParse(data[5]));
            }
        }


        /// <summary>
        /// Min/Maxになるように正規化
        /// </summary>
        public void normalize()
        {
            if (mMax.x < mMin.x) YLib.Swap(ref mMin.x, ref mMax.x);
            if (mMax.y < mMin.y) YLib.Swap(ref mMin.y, ref mMax.y);
            if (mMax.z < mMin.z) YLib.Swap(ref mMin.z, ref mMax.z);
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Box3D</returns>
        public Box3D toCopy()
        {
            return new Box3D(mMin, mMax);
        }

        /// <summary>
        /// 2次元Boxに変換
        /// </summary>
        /// <param name="face">表示面(XY/YZ/ZX)</param>
        /// <returns>Box</returns>
        public Box toBox(FACE3D face)
        {
            Box b = new Box(mMin.toPoint(face), mMax.toPoint(face));
            b.normalize();
            return b;
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return $"{mMin.ToString()} {mMax.ToString()}";
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <param name="form">数値の書式("F2"など)</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            return $"{mMin.ToString(form)} {mMax.ToString(form)}";
        }

        /// <summary>
        /// NaNかの判定
        /// </summary>
        /// <returns>NaN状態</returns>
        public bool isNaN()
        {
            if (mMax.isNaN() || mMin.isNaN())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Empty状態の判定
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            if (mMax.isEmpty() && mMin.isEmpty())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Boxの大きさ(対角線の長さ)
        /// </summary>
        /// <returns>対角線の長さ</returns>
        public double getSize()
        {
            return mMax.length(mMin);
        }

        /// <summary>
        /// Boxの中心座標
        /// </summary>
        /// <returns>中心座標</returns>
        public Point3D getCenter()
        {
            return new Point3D((mMax.x + mMin.x) / 2, (mMax.y + mMin.y) / 2, (mMax.z + mMin.z) / 2);
        }

        /// <summary>
        /// Box領域の拡張
        /// </summary>
        /// <param name="p">Point3D</param>
        public void extension(Point3D p)
        {
            mMin.x = Math.Min(mMin.x, p.x);
            mMin.y = Math.Min(mMin.y, p.y);
            mMin.z = Math.Min(mMin.z, p.z);
            mMax.x = Math.Max(mMax.x, p.x);
            mMax.y = Math.Max(mMax.y, p.y);
            mMax.z = Math.Max(mMax.z, p.z);
        }

        /// <summary>
        /// Box領域の拡張
        /// </summary>
        /// <param name="box">Box3D</param>
        public void extension(Box3D box)
        {
            extension(box.mMin);
            extension(box.mMax);
        }
    }
}
