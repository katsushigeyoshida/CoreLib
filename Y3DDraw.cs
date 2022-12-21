using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoreLib
{
    /// ---  三次元変換表示
    ///  void draw3DWLine(Point3D sp, Point3D ep)
    ///  void draw3DWCircle(Point3D cp, double r)
    ///  void draw3DWPolygon(List<Point3D> wpList)
    ///  Point3D dispConv(Point3D p)
    ///  List<Point3D> dispConv(List<Point3D> plist)
    ///  Point3D perspective(Point3D p)
    ///  bool shading(List<Point3D> plist)
    ///  ---  三次元変換マトリックスパラメータの設定
    ///  void void clear3DMatrix()
    ///  setTarnslate3DMatrix(double dx, double dy, double dz)
    ///  void setRotateX3DMatrix(double th)
    ///  void setRotateY3DMatrix(double th)
    ///  void setRotateZ3DMatrix(double th)
    ///  void setScale3DMatrix(double sx, double sy, double sz)
    ///  double[,] get3DMatrix()
    ///  void set3DMarix(double[,] mp)
    ///  double[,] conv3DMatrix(Matrix4x4 m4)
    ///  --  マウス処理
    ///  void initPosition(float xrotate, float yrotate, float zrotate)
    ///  void initMove(float x, float y, float z)
    ///  void mouseMoveStart(bool isRotate, Point pos)
    ///  void mouseMoveEnd()
    ///  bool mouseMove(Point pos)
    ///  --- 3次元サーフェス表示処理
    ///  void clearSurfaceList()
    ///  void addSurfaceList(List<Point3D> coordList, Brush fillColor)
    ///  void dispConvSurfaceList()
    ///  void drawSurfaceList()
    ///  ---  サーフェスデータクラス
    /// class Surface
    ///     Surface(List<Point3D> coordList, Brush fillColor)
    ///  

    /// <summary>
    /// 3Dグラフィックライブラリ
    /// </summary>
    public class Y3DDraw : YWorldDraw
    {
        private double[,] mMatrix = new double[4, 4];   //  3D座標変換パラメータ
        private Point3D mCrossProduct;                  //  陰面判定した時の面の向き
        public double mPerspectivLength = 0.0;          //  視点からスクリーン(z = 0)までの距離
        public Point3D mLight = new Point3D();          //  3Dの光源ベクトル
        //  マウス処理
        private Vector2 mCurrentPos;                    //  現在位置(スクリーン座標)
        private Vector2 mPreviousPos;                   //  開始位置
        private Matrix4x4 mRotate;                      //  回転マトリックス
        private bool mIsRotate = false;                 //  回転
        private bool mIsTranslate = false;              //  移動
        //  サーフェスデータリスト
        private List<Surface> mSurfaceList = new List<Surface>();

        private YLib ylib = new YLib();

        public Y3DDraw(Canvas c) : base(c)
        {
        }

        public Y3DDraw(Canvas c, double viewWidth, double viewHeight) : base(c, viewWidth, viewHeight)
        {
        }

        //  ---  三次元変換表示

        /// <summary>
        /// 3次元座標での線分の描画
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void draw3DWLine(Point3D sp, Point3D ep)
        {
            Point3D sp3 = perspective(sp);
            Point3D ep3 = perspective(ep);
            drawWLine(sp3.toPointXY(), ep3.toPointXY());
        }

        /// <summary>
        /// 3次元座標で円を描画
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">円のの半径</param>
        public void draw3DWCircle(Point3D cp, double r)
        {
            Point3D cp3 = perspective(cp);
            drawWCircle(cp3.toPointXY(), r);
        }

        /// <summary>
        /// 3次元座標でポリゴンを描画
        /// </summary>
        /// <param name="wpList">3D座標リスト</param>
        public void draw3DWPolygon(List<Point3D> wpList)
        {
            if (wpList.Count < 2)
                return;
            List<Point3D> p3List = new List<Point3D>();
            //  投影変換
            for (int i = 0; i < wpList.Count; i++) {
                p3List.Add(perspective(wpList[i]));
            }
            if (wpList.Count == 2) {
                //  線分表示
                draw3DWLine(wpList[0], wpList[1]);
                return;
            }
            //  陰面判定
            if (!shading(p3List))
                return;

            if (!mLight.isEmpty()) {
                //  光源処理
                double light = 1 - mCrossProduct.angle(mLight) / Math.PI;
                SolidColorBrush brush = (SolidColorBrush)mFillColor;
                byte r = (byte)((double)brush.Color.R * light);
                byte g = (byte)((double)brush.Color.G * light);
                byte b = (byte)((double)brush.Color.B * light);
                mFillColor = new SolidColorBrush(Color.FromRgb(r, g, b));
            }

            List<PointD> pList = new List<PointD>();
            //  2次元変換
            for (int i = 0; i < p3List.Count; i++) {
                pList.Add(p3List[i].toPointXY());
            }
            drawWPolygon(pList);
        }

        /// <summary>
        /// マトリックスによる座標変換
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point3D dispConv(Point3D p)
        {
            return p.matrix(mMatrix);
        }

        /// <summary>
        /// マトリックスによる座標変換
        /// </summary>
        /// <param name="plist"></param>
        /// <returns></returns>
        public List<Point3D> dispConv(List<Point3D> plist)
        {
            List<Point3D> olist = new List<Point3D>();
            for (int i = 0; i < plist.Count; i++)
                olist.Add(plist[i].matrix(mMatrix));
            return olist;
        }

        /// <summary>
        /// 透視変換
        /// 視点からスクリーン(z = 0)までの距離 (mPerspectivLength) を設定
        /// </summary>
        /// <param name="p">3D座標</param>
        /// <returns></returns>
        private Point3D perspective(Point3D p)
        {
            Point3D po = new Point3D(p.x, p.y, p.z);
            if (mPerspectivLength != 0) {
                double w = mPerspectivLength / (p.z + mPerspectivLength);
                po.x = p.x / w;
                po.y = p.y / w;
            }
            return po;
        }

        /// <summary>
        /// 隠面判定
        /// </summary>
        /// <param name="plist">3D座標リスト</param>
        /// <returns>隠面判定(隠面=false)</returns>
        public bool shading(List<Point3D> plist)
        {
            if (plist.Count < 3)
                return true;
            Point3D v1 = plist[0].vector(plist[1]);
            Point3D v2 = plist[1].vector(plist[2]);
            mCrossProduct = v1.crossProduct(v2);
            return mCrossProduct.z > 0;
        }


        //  ---  三次元変換マトリックスパラメータの設定

        /// <summary>
        /// 座標変換パラメートの初期化
        /// 単位行列を設定
        /// </summary>
        public void clear3DMatrix()
        {
            mMatrix = ylib.unitMatrix(4);
        }

        /// <summary>
        /// 平行移動パラメータ設定
        /// </summary>
        /// <param name="dx">X軸方向の移動量</param>
        /// <param name="dy">Y軸方向の移動量</param>
        /// <param name="dz">Z軸方向の移動量</param>
        public void setTarnslate3DMatrix(double dx, double dy, double dz)
        {
            mMatrix = translate3DMatrix(dx, dy, dz);
        }

        /// <summary>
        /// X軸回転パラメータ設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateX3DMatrix(double th)
        {
            mMatrix = rotateX3DMatrix(th);
        }

        /// <summary>
        /// Y軸回転パラメータ設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateY3DMatrix(double th)
        {
            mMatrix = rotateY3DMatrix(th);
        }

        /// <summary>
        /// Z軸回転パラメータ設定
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateZ3DMatrix(double th)
        {
            mMatrix = rotateZ3DMatrix(th);
        }

        /// <summary>
        /// 3次元縮尺パラメータ設定
        /// </summary>
        /// <param name="sx">X方向縮尺</param>
        /// <param name="sy">Y方向縮尺</param>
        /// <param name="sz">Z方向縮尺</param>
        public void setScale3DMatrix(double sx, double sy, double sz)
        {
            mMatrix = scale3DMatrix(sx, sy, sz);
        }

        /// <summary>
        /// 平行移動パラメータ追加設定
        /// 既存変換パラメータに行列の積を実施
        /// </summary>
        /// <param name="dx">X軸方向の移動量</param>
        /// <param name="dy">Y軸方向の移動量</param>
        /// <param name="dz">Z軸方向の移動量</param>
        public void addTarnslate3DMatrix(double dx, double dy, double dz)
        {
            mMatrix = ylib.matrixMulti(mMatrix, translate3DMatrix(dx, dy, dz));
        }

        /// <summary>
        /// X軸回転パラメータ追加設定
        /// 既存変換パラメータに行列の積を実施
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void addRotateX3DMatrix(double th)
        {
            mMatrix = ylib.matrixMulti(mMatrix, rotateX3DMatrix(th));
        }

        /// <summary>
        /// Y軸回転パラメータ追加設定
        /// 既存変換パラメータに行列の積を実施
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void addRotateY3DMatrix(double th)
        {
            mMatrix = ylib.matrixMulti(mMatrix, rotateY3DMatrix(th));
        }

        /// <summary>
        /// Z軸回転パラメータ追加設定
        /// 既存変換パラメータに行列の積を実施
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void addRotateZ3DMatrix(double th)
        {
            mMatrix = ylib.matrixMulti(mMatrix, rotateZ3DMatrix(th));
        }

        /// <summary>
        /// 3次元縮尺パラメータ追加設定
        /// 既存変換パラメータに行列の積を実施
        /// </summary>
        /// <param name="sx">X方向縮尺</param>
        /// <param name="sy">Y方向縮尺</param>
        /// <param name="sz">Z方向縮尺</param>
        public void addScale3DMatrix(double sx, double sy, double sz)
        {
            mMatrix = ylib.matrixMulti(mMatrix, scale3DMatrix(sx, sy, sz));
        }

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
        /// 3D変換マトリックスを取得
        /// </summary>
        /// <returns>3D変換マトリックス</returns>
        public double[,] get3DMatrix()
        {
            return mMatrix;
        }

        /// <summary>
        /// 3D変換マトリックスを取得
        /// </summary>
        /// <param name="mp">3D変換マトリックス</param>
        public void set3DMarix(double[,] mp)
        {
            mMatrix = mp;
        }

        /// <summary>
        /// Matrix4x4をYWorldDrawの3Dマトリックスに変換
        /// </summary>
        /// <param name="m4">Matrix4x4マトリックス</param>
        /// <returns>3Dマトリックス</returns>
        private double[,] conv3DMatrix(Matrix4x4 m4)
        {
            double[,] mp = new double[4, 4];
            mp[0, 0] = m4.M11;
            mp[0, 1] = m4.M12;
            mp[0, 2] = m4.M13;
            mp[0, 3] = m4.M14;
            mp[1, 0] = m4.M21;
            mp[1, 1] = m4.M22;
            mp[1, 2] = m4.M23;
            mp[1, 3] = m4.M24;
            mp[2, 0] = m4.M31;
            mp[2, 1] = m4.M32;
            mp[2, 2] = m4.M33;
            mp[2, 3] = m4.M34;
            mp[3, 0] = m4.M41;
            mp[3, 1] = m4.M42;
            mp[3, 2] = m4.M43;
            mp[3, 3] = m4.M44;
            return mp;
        }

        //  --  マウス処理

        /// <summary>
        /// 初期値を設定
        /// </summary>
        /// <param name="xrotate">X軸回転角(rad)</param>
        /// <param name="yrotate">Y軸回転角(rad)</param>
        /// <param name="zrotate">Z軸回転角(rad)</param>
        public void initPosition(float xrotate, float yrotate, float zrotate)
        {
            mIsRotate = false;
            mIsTranslate = false;
            mCurrentPos = Vector2.Zero;
            mPreviousPos = Vector2.Zero;
            //  XYZ軸を中心に回転
            Quaternion after = new Quaternion(xrotate, yrotate, zrotate, 0);
            mRotate = Matrix4x4.CreateFromQuaternion(after);
        }

        /// <summary>
        /// 移動位置の初期値設定
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void initMove(float x, float y, float z)
        {
            mRotate = Matrix4x4.CreateTranslation(x, y, z);
        }

        /// <summary>
        /// マウスによる座標変換の開始
        /// </summary>
        /// <param name="isRotate">回転/移動</param>
        /// <param name="pos">マウス位置(スクリーン座標)</param>
        public void mouseMoveStart(bool isRotate, Point pos)
        {
            mIsRotate = isRotate;
            mIsTranslate = !isRotate;
            mCurrentPos = new Vector2((float)pos.X, (float)pos.Y);
        }

        /// <summary>
        /// マウスによる座標変換の終了
        /// </summary>
        public void mouseMoveEnd()
        {
            mIsRotate = false;
            mIsTranslate = false;
            mPreviousPos = Vector2.Zero;
        }

        /// <summary>
        /// マウスの位置移動
        /// </summary>
        /// <param name="pos">マウス位置(スクリーン座標)</param>
        /// <returns>座標変換の有無</returns>
        public bool mouseMove(Point pos)
        {
            if (mIsRotate) {
                //  回転処理
                mPreviousPos = mCurrentPos;
                mCurrentPos = new Vector2((float)pos.X, (float)pos.Y);
                Vector2 delta = mPreviousPos - mCurrentPos;
                if (delta.Length() <= 1f)
                    return false;
                delta /= (float)Math.Sqrt(mView.Width * mView.Width + mView.Height * mView.Height);
                float length = delta.Length();
                if (0 < length) {
                    float rad = length * (float)Math.PI;
                    float theta = (float)Math.Sin(rad) / length;
                    Quaternion after = new Quaternion(delta.Y * theta, delta.X * theta, 0.0f, (float)Math.Cos(rad));
                    mRotate = Matrix4x4.CreateFromQuaternion(after);
                    mMatrix = conv3DMatrix(mRotate);
                }
            } else if (mIsTranslate) {
                //  移動処理
                mPreviousPos = mCurrentPos;
                mCurrentPos = new Vector2((float)pos.X, (float)pos.Y);
                Vector2 delta = mCurrentPos - mPreviousPos;
                if (delta.Length() <= 1f)
                    return false;
                mRotate = Matrix4x4.CreateTranslation(delta.X * 4f / (float)mView.Width, -delta.Y * 4f / (float)mView.Height, 0f);
                mMatrix = conv3DMatrix(mRotate);
            } else {
                //  変換マトリックスをクリア
                mMatrix = ylib.unitMatrix(4);
                return false;
            }
            return true;
        }


        //  --- 3次元サーフェス表示処理

        /// <summary>
        /// サーフェスデータをクリア
        /// </summary>
        public void clearSurfaceList()
        {
            mSurfaceList.Clear();
        }

        /// <summary>
        /// サーフェスデータの追加
        /// </summary>
        /// <param name="coordList">3D座標リスト</param>
        /// <param name="fillColor">色</param>
        public void addSurfaceList(List<Point3D> coordList, Brush fillColor)
        {
            mSurfaceList.Add(new Surface(coordList, fillColor));
        }

        /// <summary>
        /// サーフェスの座標リストを表示座標変換する
        /// </summary>
        private void dispConvSurfaceList()
        {
            List<Surface> outList = new List<Surface>();
            for (int i = 0; i < mSurfaceList.Count; i++) {
                outList.Add(new Surface(dispConv(mSurfaceList[i].mCoordList), mSurfaceList[i].mFillColor));
            }
            mSurfaceList = outList;
        }

        /// <summary>
        /// サーフェスデータの表示
        /// </summary>
        public void drawSurfaceList()
        {
            dispConvSurfaceList();
            //  サーフェスデータをZ方向にソート
            mSurfaceList.Sort((a, b) => Math.Sign(a.mZOrder - b.mZOrder));
            //  サーフェスデータの表示
            for (int i = 0; i < mSurfaceList.Count; i++) {
                mFillColor = mSurfaceList[i].mFillColor;
                draw3DWPolygon(mSurfaceList[i].mCoordList);
            }
        }

        //  ---  サーフェスデータクラス

        /// <summary>
        /// サーフェスデータクラス
        /// </summary>
        class Surface
        {
            public List<Point3D> mCoordList;        //  座標リスト
            public double mZOrder = 0.0;            //  Z座標の平均値
            public Brush mFillColor = Brushes.Blue; //  サーフェスの色

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="coordList">3D座標リスト</param>
            /// <param name="fillColor">色</param>
            public Surface(List<Point3D> coordList, Brush fillColor)
            {
                mCoordList = coordList;
                mZOrder = mCoordList.Average(p => p.z);
                mFillColor = fillColor;
            }
        }
    }
}
