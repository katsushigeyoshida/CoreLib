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
    ///  void draw3DWLine(Point3D sp, Point3D ep)       3次元座標での線分の描画
    ///  void draw3DWCircle(Point3D cp, double r)       3次元座標で円を描画
    ///  void draw3DWPolygon(List<Point3D> wpList)      3次元座標でラインまたはポリゴンを面で描画
    ///  Point3D dispConv(Point3D p)                    マトリックスによる座標変換
    ///  List<Point3D> dispConv(List<Point3D> plist)    マトリックスによる座標変換
    ///  Point3D perspective(Point3D p)                 透視変換
    ///  bool shading(List<Point3D> plist)              隠面判定(外積)
    ///  ---  三次元変換マトリックスパラメータの設定
    ///  void void clear3DMatrix()                      座標変換パラメートの初期化
    ///  setTarnslate3DMatrix(double dx, double dy, double dz)  平行移動パラメータ設定
    ///  void setRotateX3DMatrix(double th)             X軸回転パラメータ設定
    ///  void setRotateY3DMatrix(double th)             Y軸回転パラメータ設定
    ///  void setRotateZ3DMatrix(double th)             Z軸回転パラメータ設定
    ///  void setScale3DMatrix(double sx, double sy, double sz) 3次元縮尺パラメータ設定
    ///  void addTarnslate3DMatrix(double dx, double dy, double dz) 平行移動パラメータ追加設定
    ///  void addRotateX3DMatrix(double th)             X軸回転パラメータ追加設定
    ///  void addRotateY3DMatrix(double th)             Y軸回転パラメータ追加設定
    ///  void addRotateZ3DMatrix(double th)             Z軸回転パラメータ追加設定
    ///  void addScale3DMatrix(double sx, double sy, double sz) 3次元縮尺パラメータ追加設定
    ///  double[,] translate3DMatrix(double dx, double dy, double dz)   移動量を3D変換マトリックス(4x4)に設定
    ///  double[,] rotateX3DMatrix(double th)           X軸回転を3D変換マトリックス(4x4)に設定
    ///  double[,] rotateY3DMatrix(double th)           Y軸回転を3D変換マトリックス(4x4)に設定
    ///  double[,] rotateZ3DMatrix(double th)           Z軸回転を3D変換マトリックス(4x4)に設定
    ///  double[,] scale3DMatrix(double sx, double sy, double sz)   拡大縮小のスケール値を3D変換マトリックス(4x4)に設定
    ///  double[,] get3DMatrix()                        3D変換マトリックスを取得
    ///  void set3DMarix(double[,] mp)                  3D変換マトリックスを取得
    ///  double[,] conv3DMatrix(Matrix4x4 m4)           Matrix4x4をYWorldDrawの3Dマトリックスに変換
    ///  --  マウス処理
    ///  void mouseMoveStart(bool isRotate, Point pos)  マウスによる座標変換の開始
    ///  void mouseMoveEnd()                            マウスによる座標変換の終了
    ///  bool mouseMove(Point pos)                      マウスの位置移動
    ///  --- 3次元サーフェス表示処理
    ///  void clearSurfaceList()                        サーフェスデータをクリア
    ///  void addSurfaceList(List<Point3D> coordList, Brush fillColor)  サーフェスデータの追加
    ///  void dispConvSurfaceList()                     サーフェスの座標リストを表示座標変換する
    ///  void drawSurfaceList()                         サーフェスデータの表示(Z方向にソート)
    ///  
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
        private PointD mCurrentPos;                     //  現在位置(スクリーン座標)
        private PointD mPreviousPos;                    //  開始位置
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
            drawWLine(new LineD(sp3.toPointXY(), ep3.toPointXY()));
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
        /// 3次元座標でラインまたはポリゴンを面で描画
        /// </summary>
        /// <param name="wpList">3D座標リスト</param>
        public void draw3DWPolygon(List<Point3D> wpList)
        {
            if (wpList.Count < 2)
                return;

            if (wpList.Count == 2) {
                //  線分表示(投影変換込み)
                mBrush = mFillColor;
                draw3DWLine(wpList[0], wpList[1]);
                return;
            }

            //  投影変換
            List<Point3D> p3List = new List<Point3D>();
            for (int i = 0; i < wpList.Count; i++) {
                p3List.Add(perspective(wpList[i]));
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
            mBrush = mFillColor;

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
            return p.toMatrix(mMatrix);
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
                olist.Add(plist[i].toMatrix(mMatrix));
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
        /// 隠面判定(外積)
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
        /// マウスによる座標変換の開始
        /// </summary>
        /// <param name="isRotate">回転/移動</param>
        /// <param name="pos">マウス位置(スクリーン座標)</param>
        public void mouseMoveStart(bool isRotate, Point pos)
        {
            mIsRotate = isRotate;
            mIsTranslate = !isRotate;
            mCurrentPos = new PointD(pos);
        }

        /// <summary>
        /// マウスによる座標変換の終了
        /// </summary>
        public void mouseMoveEnd()
        {
            mIsRotate = false;
            mIsTranslate = false;
            mPreviousPos = new PointD();
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
                mCurrentPos = new PointD(pos);
                PointD delta = mPreviousPos - mCurrentPos;
                if (delta.length() <= 1.0)
                    return false;
                delta /= (float)Math.Sqrt(mView.Width * mView.Width + mView.Height * mView.Height);
                double length = delta.length();
                if (0 < length) {
                    double rad = length * Math.PI;
                    double theta = (float)Math.Sin(rad) / length;
                    Shigensu rotate = new Shigensu(delta.y * theta, delta.x * theta, 0.0f, Math.Cos(rad));
                    mMatrix = rotate.rotateMatrix();
                    //Quaternion rotate = new Quaternion((float)(delta.y * theta), (float)(delta.x * theta), 0.0f, (float)Math.Cos(rad));
                    //mMatrix = conv3DMatrix(Matrix4x4.CreateFromQuaternion(rotate));
                }
            } else if (mIsTranslate) {
                //  移動処理
                mPreviousPos = mCurrentPos;
                mCurrentPos = new PointD(pos);
                PointD delta = mCurrentPos - mPreviousPos;
                if (delta.length() <= 1.0)
                    return false;
                delta *= mWorld.Width / mView.Width;
                mMatrix = translate3DMatrix(delta.x , -delta.y, 0);
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
            for (int i = 0; i < mSurfaceList.Count; i++) {
                for (int j = 0; j < mSurfaceList[i].mCoordList.Count; j++) {
                    mSurfaceList[i].mCoordList[j].matrix(mMatrix);
                }
                mSurfaceList[i].mZOrder = mSurfaceList[i].mCoordList.Average(p => p.z);
            }
        }

        /// <summary>
        /// サーフェスデータの表示(Z方向にソート)
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
    }

    //  ---  サーフェスデータクラス

    /// <summary>
    /// サーフェスデータクラス
    /// LineまたはPolygonデータ
    /// </summary>
    public class Surface
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
