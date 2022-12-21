using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoreLib
{
    /// <summary>
    /// プリミティブデータの種別
    /// </summary>
    public enum PRIMITIVE {
        LINES, TRIANGLES, QUADS, POLYGON, TRIANGLE_STRIP, QUAD_STRIP, TRIANGLE_FAN, PARTS 
    };

    /// <summary>
    /// 3Dのプリミティブデータ
    /// </summary>
    public class SurfacePrimitive
    {
        public List<Point3D> mCoords = new List<Point3D>();    //  三角形の座標
        public List<Point3D> mOutCoords = new List<Point3D>();
        public Brush mColor = Brushes.Blue;                    //  三角形の色
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">3D座標配列</param>
        /// <param name="ccw">反時計回りを時計回りに変換</param>
        public SurfacePrimitive(Point3D[] p, bool ccw = false, Brush color = null)
        {
            for (int i = 0; i < p.Length; i++) {
                mCoords.Add(new Point3D(p[ccw ? p.Length - 1 - i : i]));
            }
            if (color != null) {
                mColor = color;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p">3D座標リスト</param>
        /// <param name="ccw">反時計回りを時計回りに変換</param>
        /// <param name="color">面の色</param>
        public SurfacePrimitive(List<Point3D> p, bool ccw = false, Brush color = null)
        {
            for (int i = 0; i < p.Count; i++) {
                mCoords.Add(new Point3D(p[ccw ? p.Count - 1 - i : i]));
            }
            if (color != null) {
                mColor = color;
            }
        }

        /// <summary>
        /// 座標変換の実行
        /// </summary>
        /// <param name="matrix">変換マトリックス</param>
        public void convMatrix(double[,] matrix)
        {
            mOutCoords = new List<Point3D>();
            for (int i = 0; i < mCoords.Count; i++) {
                Point3D p = mCoords[i].toCopy();
                p = p.matrix(matrix);
                mOutCoords.Add(p);
            }
        }

        /// <summary>
        /// 座標変換の実行
        /// </summary>
        /// <param name="matrixList">変換マトリックス</param>
        public void convMatrix(List<double[,]> matrixList)
        {
            mOutCoords = new List<Point3D>();
            for (int i = 0; i < mCoords.Count; i++) {
                Point3D p = mCoords[i].toCopy();
                for (int j = 0; j < matrixList.Count; j++) { 
                    p = p.matrix(matrixList[j]);
                }
                mOutCoords.Add(p);
            }
        }
    }

    /// <summary>
    /// 部品データクラス
    /// </summary>
    public class PartsPrimitive
    {
        //  サーフェスデータリスト
        private List<SurfacePrimitive> mDataList = new List<SurfacePrimitive>();
        private double[,] mPartsMatrix;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PartsPrimitive()
        {
            mPartsMatrix = ylib.unitMatrix(4);
        }

        /// <summary>
        /// 座標変換マトリックスリストのクリア
        /// </summary>
        public void clearMatrix()
        {
            mPartsMatrix = ylib.unitMatrix(4);
        }

        /// <summary>
        /// スケール変換マトリックスの登録
        /// </summary>
        /// <param name="scale">スケール</param>
        public void setScaleMatrix(Point3D scale)
        {
            double[,] mp = ylib.unitMatrix(4);
            mp[0, 0] = scale.x;
            mp[1, 1] = scale.y;
            mp[2, 2] = scale.z;
            mp[3, 3] = 1;
            mPartsMatrix = ylib.matrixMulti(mPartsMatrix, mp);
        }

        /// <summary>
        /// 移動マトリックスの登録
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        public void setTranlateMatrix(Point3D vec)
        {
            double[,] mp = ylib.unitMatrix(4);
            mp[0, 0] = 1;
            mp[1, 1] = 1;
            mp[2, 2] = 1;
            mp[3, 0] = vec.x;
            mp[3, 1] = vec.y;
            mp[3, 2] = vec.z;
            mp[3, 3] = 1;
            mPartsMatrix = ylib.matrixMulti(mPartsMatrix, mp);
        }

        /// <summary>
        /// X軸回転マトリックスの登録
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateXMatrix(double th)
        {
            double[,] mp = ylib.unitMatrix(4);
            mp[0, 0] = 1.0;
            mp[1, 1] = Math.Cos(th);
            mp[1, 2] = Math.Sin(th);
            mp[2, 1] = -Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            mPartsMatrix = ylib.matrixMulti(mPartsMatrix, mp);
        }

        /// <summary>
        /// Y軸回転マトリックスの登録
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateYMatrix(double th)
        {
            double[,] mp = ylib.unitMatrix(4);
            mp[0, 0] = Math.Cos(th);
            mp[0, 2] = -Math.Sin(th);
            mp[1, 1] = 1.0;
            mp[2, 0] = Math.Sin(th);
            mp[2, 2] = Math.Cos(th);
            mp[3, 3] = 1.0;
            mPartsMatrix = ylib.matrixMulti(mPartsMatrix, mp);
        }

        /// <summary>
        /// Z軸回転マトリックスの登録
        /// </summary>
        /// <param name="th">回転角(rad)</param>
        public void setRotateZMatrix(double th)
        {
            double[,] mp = ylib.unitMatrix(4);
            mp[0, 0] = Math.Cos(th);
            mp[0, 1] = Math.Sin(th);
            mp[1, 0] = -Math.Sin(th);
            mp[1, 1] = Math.Cos(th);
            mp[2, 2] = 1.0;
            mp[3, 3] = 1.0;
            mPartsMatrix = ylib.matrixMulti(mPartsMatrix, mp);
        }

        /// <summary>
        /// サーフェスデータリストのクリア
        /// </summary>
        public void clear()
        {
            mDataList.Clear();
        }

        /// <summary>
        /// 座標変換
        /// </summary>
        public void convCoords()
        {
            for (int i = 0; i < mDataList.Count; i++) {
                mDataList[i].convMatrix(mPartsMatrix);
            }
        }

        /// <summary>
        /// サーフェスリストにデータを登録
        /// </summary>
        /// <param name="ydraw"></param>
        public void setDrawData(Y3DDraw ydraw)
        {
            for (int i = 0; i < mDataList.Count; i++) {
                ydraw.addSurfaceList(mDataList[i].mOutCoords, mDataList[i].mColor);
            }
        }

        /// <summary>
        /// 3D座標の設定(サーフェスデータの登録))
        /// </summary>
        /// <param name="ps">3D座標リスト</param>
        /// <param name="primitive">プリミティブの種別</param>
        /// <param name="colors">カラーリスト</param>
        public void setVertex(List<Point3D> ps, PRIMITIVE primitive, List<Brush> colors)
        {
            int count = 0;
            int colorCount = 0;
            List<Point3D> buf = new List<Point3D>();
            Point3D orgp = new Point3D();
            for (int i = 0; i < ps.Count; i++) {
                buf.Add(ps[i].toCopy());
                if (primitive == PRIMITIVE.LINES) {
                    if (buf.Count == 2) {
                        mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = -1;
                    }
                } else if (primitive == PRIMITIVE.TRIANGLES) {
                    //  三角形
                    if (buf.Count == 3) {
                        mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = -1;
                    }
                } else if (primitive == PRIMITIVE.QUADS) {
                    //  四角形
                    if (buf.Count == 4) {
                        mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = -1;
                    }
                } else if (primitive == PRIMITIVE.POLYGON) {
                    //  ポリゴン
                } else if (primitive == PRIMITIVE.TRIANGLE_STRIP) {
                    //  三角形の帯
                    if (1 < count) {
                        bool ccw = mDataList.Count % 2 == 0;
                        mDataList.Add(new SurfacePrimitive(buf, ccw, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = -1;
                        i -= 2;
                    }
                } else if (primitive== PRIMITIVE.QUAD_STRIP) {
                    //  四角形の帯
                    if (count == 2) {
                        buf[count] = ps[i + 1].toCopy();
                    } else if (count == 3) {
                        buf[count] = ps[i - 1].toCopy();
                        mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = -1;
                        i -= 3;
                    }
                } else if (primitive == PRIMITIVE.TRIANGLE_FAN) {
                    //  三角形の扇
                    if (i == 0) {
                        orgp = ps[i].toCopy();
                    } else if (count == 2) {
                        mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
                        buf = new List<Point3D>();
                        count = 0;
                        buf.Add(orgp);
                        i -= 1;
                    }
                } else if (primitive == PRIMITIVE.PARTS) {

                }
                count++;
            }
            if (primitive == PRIMITIVE.POLYGON) {
                mDataList.Add(new SurfacePrimitive(buf, false, colors[colorCount++ % colors.Count]));
            }
        }
    }

    /// <summary>
    /// 3Dグラフィックライブラリ
    /// </summary>
    public class Y3DGraphics : Y3DDraw
    {
        public List<PartsPrimitive> mDataList = new List<PartsPrimitive>();
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">キャンバス</param>
        public Y3DGraphics(Canvas c) : base(c)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">キャンバス</param>
        /// <param name="viewWidth">Viewの幅</param>
        /// <param name="viewHeight">Viewの高さ</param>
        public Y3DGraphics(Canvas c, double viewWidth, double viewHeight) : base(c, viewWidth, viewHeight)
        {
        }

        /// <summary>
        /// 部品データリストとサーフェスリストをクリア
        /// </summary>
        public void dataClear()
        {
            mDataList.Clear();
            clearSurfaceList();
        }

        /// <summary>
        /// 部品データの登録
        /// </summary>
        /// <param name="parts">部品データ</param>
        public void add(PartsPrimitive parts)
        {
            mDataList.Add(parts);
        }
        
        /// <summary>
        /// 座標変換したデータをサーフェスリストに登録
        /// </summary>
        public void setDrawData()
        {
            clearSurfaceList();
            clear3DMatrix();

            foreach (var part in mDataList) {
                part.convCoords();
                part.setDrawData(this);
            }
        }

        /// <summary>
        /// サーフェスリストの表示
        /// </summary>
        public void draw()
        {
            drawSurfaceList();
        }
    }

}
