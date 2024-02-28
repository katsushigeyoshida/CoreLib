
using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// ポリゴンクラス
    /// </summary>
    public class Polygon3D
    {
        public Point3D mCp = new Point3D();                 //  中心座標
        public Point3D mU = new Point3D(1, 0, 0);           //  平面のX軸の向き(単位ベクトル
        public Point3D mV = new Point3D(0, 1, 0);           //  平面のY軸の向き(単位ベクトル)
        public List<PointD> mPolygon;
        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Polygon3D()
        {
            mPolygon = new List<PointD>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plist">3D座標点リスト</param>
        public Polygon3D(List<Point3D> polyline)
        {
            if (2 < polyline.Count) {
                mCp = polyline[0];
                mU = polyline[1] - polyline[0];
                mU.unit();
                Line3D l = new Line3D(polyline[0], polyline[1]);
                Point3D ip = l.intersection(polyline[2]);
                mV = polyline[2] - ip;
                mV.unit();
            } else if (2 == polyline.Count) {
                mCp = polyline[0];
                mU = polyline[1] - polyline[0];
                mU.unit();
                mV = mU.toCopy();
                mV.rotate(new Point3D(), Math.PI / 2, FACE3D.XY);
            }
            mPolygon = new List<PointD>();
            for (int i = 0; i < polyline.Count; i++) {
                mPolygon.Add(Point3D.cnvPlaneLocation(polyline[i], mCp, mU, mV));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="face"></param>
        public Polygon3D(List<Point3D> polyline, FACE3D face)
        {
            mPolygon = polyline.ConvertAll(p => p.toPoint(face));
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plist">2D座標点リスト</param>
        /// <param name="face">2D平面</param>
        public Polygon3D(List<PointD> polyline, FACE3D face)
        {
            mPolygon = polyline.ConvertAll(p => p.toCopy());
            mU = Point3D.getUVector(face);
            mV = Point3D.getVVector(face);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polyline">3Dポリライン</param>
        public Polygon3D(Polyline3D polyline)
        {
            mPolygon = polyline.mPolyline.ConvertAll(p => p.toCopy());
            mCp = polyline.mCp.toCopy();
            mU = polyline.mU.toCopy();
            mV = polyline.mV.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="polygon">3Dポリゴン</param>
        public Polygon3D(Polygon3D polygon)
        {
            mPolygon = polygon.mPolygon.ConvertAll(p => p.toCopy());
            mCp = polygon.mCp.toCopy();
            mU = polygon.mU.toCopy();
            mV = polygon.mV.toCopy();
        }

        /// <summary>
        /// コピーの作成
        /// </summary>
        /// <returns>Polygon3D</returns>
        public Polygon3D toCopy()
        {
            Polygon3D polygon = new Polygon3D();
            polygon.mCp = mCp.toCopy();
            polygon.mU = mU.toCopy();
            polygon.mV = mV.toCopy();
            polygon.mPolygon = mPolygon.ConvertAll(p => p.toCopy());
            return polygon;
        }

        /// <summary>
        /// 2D座標に変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            return plist.ConvertAll(p => p.toPoint(face));
        }

        /// <summary>
        /// 3D座標リストの抽出
        /// </summary>
        /// <returns>3D座標リスト</returns>
        public List<Point3D> toPoint3D(bool close = false)
        {
            List<Point3D> plist = new List<Point3D>();
            for (int i = 0; i < mPolygon.Count; i++) {
                plist.Add(Point3D.cnvPlaneLocation(mPolygon[i], mCp, mU, mV));
            }
            if (close)
                plist.Add(Point3D.cnvPlaneLocation(mPolygon[0], mCp, mU, mV));
            return plist;
        }

        /// <summary>
        /// 3D座標で指定位置の座標を抽出
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>3D座標</returns>
        public Point3D toPoint3D(int n)
        {
            return Point3D.cnvPlaneLocation(mPolygon[n % mPolygon.Count], mCp, mU, mV);
        }

        /// <summary>
        /// 線分に変換
        /// </summary>
        /// <returns>3D線分リスト</returns>
        public List<Line3D> toLine3D()
        {
            List<Point3D> plist = toPoint3D();
            List<Line3D> lines = new List<Line3D>();
            for (int i = 0; i < plist.Count - 1; i++) {
                lines.Add(new Line3D(plist[i], plist[i + 1]));
            }
            return lines;
        }

        /// <summary>
        /// 2D線分に変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2D線分リスト</returns>
        public List<LineD> toLineD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            List<LineD> lines = new List<LineD>();
            LineD line;
            for (int i = 0; i < plist.Count - 1; i++) {
                line = new LineD(plist[i].toPoint(face), plist[i + 1].toPoint(face));
                line.mEps = 1e-4;
                lines.Add(line);
            }
            line = new LineD(plist[^1].toPoint(face), plist[0].toPoint(face));
            line.mEps = 1e-4;
            lines.Add(line);
            return lines;
        }

        /// <summary>
        /// 2Dポリゴンに変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2Dポリゴン</returns>
        public PolygonD toPolygonD(FACE3D face)
        {
            List<Point3D> plist = toPoint3D();
            PolygonD polygon = new PolygonD();
            polygon.mPolygon = plist.ConvertAll(p => p.toPoint(face));
            return polygon;
        }

        /// <summary>
        /// 指定位置で分割してポリラインに変換
        /// </summary>
        /// <param name="n">分割位置</param>
        /// <returns>ポリライン</returns>
        public Polyline3D toPolyline3D(int n = 0)
        {
            Polyline3D polyline = new Polyline3D();
            polyline.mCp = mCp.toCopy();
            polyline.mU = mU.toCopy();
            polyline.mV = mV.toCopy();
            for (int i = n; i < mPolygon.Count; i++)
                polyline.mPolyline.Add(mPolygon[i]);
            for (int i = 0; i < n; i++)
                polyline.mPolyline.Add(mPolygon[i]);
            polyline.mPolyline.Add(mPolygon[n]);
            return polyline;
        }

        /// <summary>
        /// 指定位置の線分を取得
        /// </summary>
        /// <param name="n">位置</param>
        /// <returns>線分</returns>
        public Line3D getLine3D(int n)
        {
            return new Line3D(toPoint3D(n), toPoint3D(n + 1));
        }

        /// <summary>
        /// 指定座標に最も近い線分位置を求める
        /// </summary>
        /// <param name="pos">座標</param>
        /// <returns>線分位置</returns>
        public int nearLine(Point3D pos)
        {
            int n = -1;
            double dis = double.MaxValue;
            List<Line3D> lines = toLine3D();
            for (int i = 0; i < lines.Count; i++) {
                Point3D ip = lines[i].intersection(pos);
                double l = ip.length(pos);
                if (l < dis && lines[i].onPoint(ip)) {
                    dis = l;
                    n = i;
                }
            }
            return n;
        }

        /// <summary>
        /// 2D座標で最も近い線分の位置を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>線分位置</returns>
        public int nearLine(PointD pos, FACE3D face)
        {
            int n = -1;
            double dis = double.MaxValue;
            List<LineD> lines = toLineD(face);
            for (int i = 0; i < lines.Count; i++) {
                PointD ip = lines[i].intersection(pos);
                double l = ip.length(pos);
                if (l < dis && lines[i].onPoint(ip)) {
                    dis = l;
                    n = i;
                }
            }
            return n;
        }

        /// <summary>
        /// 指定点に最も近い線分の分割座標から最も近い2D座標を求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標</returns>
        public PointD nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            int n = nearLine(pos, face);
            LineD l = getLine3D(n).toLineD(face);
            return l.nearPoint(pos, divideNo);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="v">移動ベクトル</param>
        public void translate(Point3D v)
        {
            mCp.translate(v);
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角(rad)</param>
        /// <param name="face">2D平面</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            mCp.rotate(cp, ang, face);
            mU.rotate(ang, face);
            mV.rotate(ang, face);
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(Point3D sp, Point3D ep)
        {
            Line3D line = getLine3D(nearLine(sp));
            PointD spp = Point3D.cnvPlaneLocation(sp, mCp, mU, mV);
            PointD epp = Point3D.cnvPlaneLocation(ep, mCp, mU, mV);
            LineD l = line.toLineD(mCp, mU, mV);
            double dis = l.distance(epp) * Math.Sign(l.crossProduct(epp)) - l.distance(spp) * Math.Sign(l.crossProduct(spp));
            PolygonD polygon = new PolygonD(mPolygon);
            polygon.offset(dis);
            mPolygon = polygon.mPolygon;
        }

        /// <summary>
        /// ポリゴンの分割(ポリラインに変換)
        /// </summary>
        /// <param name="pos">2D分割座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>ポリライン</returns>
        public Polyline3D divide(PointD pos, FACE3D face)
        {
            int n = nearLine(pos, face);
            Line3D line = new Line3D(toPoint3D(n), toPoint3D(n < mPolygon.Count - 1 ? n + 1 : 0));
            Point3D p = line.intersection(pos, face);
            Polyline3D polyline = toPolyline3D(n < mPolygon.Count - 1 ? n + 1 : 0);
            polyline.insert(0, p);
            polyline.mPolyline.RemoveAt(polyline.mPolyline.Count - 1);
            polyline.add(p);
            return polyline;
        }

        /// <summary>
        /// 法線ベクトル
        /// </summary>
        /// <returns></returns>
        public Point3D getNormal()
        {
            Point3D normal = new Point3D();
            for (int i = 0; i <  mPolygon.Count - 2; i++) {
                normal += toPoint3D(i).getNormal(toPoint3D(i + 1), toPoint3D(i + 2));
            }
            normal.unit();
            return normal;
        }

        /// <summary>
        /// 座標点が平面上で時計回りかの判定(NG)
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns></returns>
        public bool isClockwise(FACE3D face)
        {
            List<PointD> plist = toPointD(face);
            Polygon3D polygon = new Polygon3D(plist, FACE3D.XY);
            Point3D normal = polygon.getNormal();
            return 0 < normal.z;
        }

        /// <summary>
        /// 座標点を逆順にする
        /// </summary>
        public void reverse()
        {
            mPolygon.Reverse();
        }

        /// <summary>
        /// 隣り合う座標が同じものを削除する
        /// </summary>
        public void squeeze()
        {
            for (int i = mPolygon.Count - 1; i > 0; i--) {
                if (mPolygon[i].length(mPolygon[i - 1]) < mEps)
                    mPolygon.RemoveAt(i);
            }
            for (int i = mPolygon.Count - 2; i > 0; i--) {
                if ((Math.PI - mPolygon[i].angle(mPolygon[i - 1], mPolygon[i + 1])) < mEps)
                    mPolygon.RemoveAt(i);
            }
        }

        /// <summary>
        /// 多角形を三角形の集合に変換(座標リスト = 3座標 x 三角形の数)
        /// </summary>
        /// <returns>(3角形の座標リスト,リスト反転)</returns>
        public (List<Point3D> triangles, bool reverse) cnvTriangles()
        {
            bool reverse = false;
            PolygonD polygon = new PolygonD(mPolygon);
            int polygonCount = polygon.mPolygon.Count;
            int removeCount = 0;
            List<PointD> triangles;
            (triangles, removeCount) = cnvTriangles(polygon);
            if (triangles.Count / 3 < polygonCount - removeCount - 2) {
                polygon = new PolygonD(mPolygon);
                polygon.mPolygon.Reverse();
                (triangles, removeCount) = cnvTriangles(polygon);
                reverse = true;
            }
            List<Point3D> triangle3d = triangles.ConvertAll(p => Point3D.cnvPlaneLocation(p, mCp, mU, mV));
            return (triangle3d, reverse);
        }

        /// <summary>
        /// 多角形を三角形の集合に変換(座標リスト=3座標x三角形の数)
        /// 反時計回りに三角形を作っていき、2点目の角度が正で他の輪郭線と重ならないものを使う
        /// </summary>
        /// <param name="plist">多角形の座標リスト</param>
        /// <returns>(3角形の座標リスト,削除座標数)</returns>
        public (List<PointD>, int) cnvTriangles(PolygonD polygon)
        {
            List<PointD> triangles = new List<PointD>();
            List<PointD> plist = polygon.mPolygon;
            int removeCount = 0;
            if (plist.Count < 3)
                return (triangles, removeCount);
            int n = 0;
            while (n < plist.Count - 2) {
                //  反時計回りの３点の角度
                double ang = plist[n + 1].angle(plist[n + 2], plist[n], false);
                //  他の輪郭線と重ならないことをチェック
                LineD line = new LineD(plist[n], plist[n + 2]);
                List<PointD> iplist = polygon.intersection(line);
                iplist = plistSqueeze(iplist, line.toPointList());
                if (0 < ang && iplist.Count == 0) {
                    //  三角形を登録
                    triangles.Add(plist[n].toCopy());
                    triangles.Add(plist[n + 1].toCopy());
                    triangles.Add(plist[n + 2].toCopy());
                    //  登録した三角形の中央頂点を除外
                    plist.RemoveAt(n + 1);
                    n = 0;
                } else if (Math.Abs(ang) < mEps || Math.Abs(ang - Math.PI) < mEps) {
                    plist.RemoveAt(n + 1);
                    removeCount++;
                    n = 0;
                } else {
                    //  次の候補
                    n++;
                }
            }
            return (triangles, removeCount);
        }

        /// <summary>
        /// 座標リストから特定の座標を除外
        /// </summary>
        /// <param name="plist">座標リスト</param>
        /// <param name="clist">除外座標リスト</param>
        /// <returns>座標リスト</returns>
        private List<PointD> plistSqueeze(List<PointD> plist, List<PointD> clist)
        {
            for (int i = 0; i < clist.Count; i++) {
                for (int j = plist.Count - 1; 0 <= j; j--) {
                    if (clist[i].length(plist[j]) <= mEps)
                        plist.RemoveAt(j);
                }
            }
            return plist;
        }
    }
}
