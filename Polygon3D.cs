﻿
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
        /// 線分以外の要素を含む
        /// </summary>
        /// <returns>MultiType</returns>
        public bool IsMultiType()
        {
            return 0 <= mPolygon.FindIndex(p => p.type != 0);
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
        /// 2Dの座標点リストに変換
        /// </summary>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD()
        {
            return mPolygon.ConvertAll(p => p.toCopy());
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
        /// <param name="divAng">円弧の分割角度</param>
        /// <param name="close">閉領域</param>
        /// <returns>3D座標リスト</returns>
        public List<Point3D> toPoint3D(double divAng = 0, bool close = false)
        {
            List<Point3D> plist = new List<Point3D>();
            List<PointD> polygon = new PolygonD(mPolygon).toPointList(divAng);
            for (int i = 0; i < polygon.Count; i++) {
                plist.Add(Point3D.cnvPlaneLocation(polygon[i], mCp, mU, mV));
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
        public Polyline3D toPolyline3D(int n = 0, bool loop = true)
        {
            Polyline3D polyline = new Polyline3D();
            polyline.mCp = mCp.toCopy();
            polyline.mU = mU.toCopy();
            polyline.mV = mV.toCopy();
            for (int i = n; i < mPolygon.Count; i++)
                polyline.mPolyline.Add(mPolygon[i]);
            for (int i = 0; i < n; i++)
                polyline.mPolyline.Add(mPolygon[i]);
            if (loop)
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
        /// 指定点に最も近い線分または円弧の分割座標から最も近い2D座標を求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標</returns>
        public Point3D nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            if (mPolygon.Count == 2) {
                Line3D line = new Line3D(toPoint3D(0), toPoint3D(1));
                Line3D l = new Line3D(new Point3D(pos, face), new Point3D(pos, face, 1));
                return line.intersection(l);
            } else {
                Plane3D plane = new Plane3D(mCp, mU, mV);
                Point3D ip = plane.intersection(pos, face); //  投影点座標
                if (ip == null) return null;
                return nearPoint(ip, divideNo);
            }
        }

        /// <summary>
        /// 指定点に最も近い線分または円弧の分割座標から最も近い3D座標を求める
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>3D座標</returns>
        public Point3D nearPoint(Point3D pos, int divideNo)
        {
            PointD p = Point3D.cnvPlaneLocation(pos, mCp, mU, mV);
            int n = nearPosition(pos);
            if (n < 0)
                return null;
            PointD np;
            int n1 = (n + 1) % mPolygon.Count;
            int n2 = (n + 2) % mPolygon.Count;
            if (mPolygon[n].type == 1) {
                ArcD arc = new ArcD(mPolygon[n - 1], mPolygon[n], mPolygon[n1]);
                np = arc.nearPoints(p, divideNo);
            } else if (mPolygon[n1].type == 1) {
                ArcD arc = new ArcD(mPolygon[n], mPolygon[n1], mPolygon[n2]);
                np = arc.nearPoints(p, divideNo);
            } else {
                LineD l = new LineD(mPolygon[n], mPolygon[n1]);
                np = l.nearPoint(p, divideNo);
            }
            return Point3D.cnvPlaneLocation(np, mCp, mU, mV);
        }

        /// <summary>
        /// 指定点に最も近い座標点の位置
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <returns>座標位置</returns>
        public int nearPosition(Point3D pos)
        {
            PointD p = Point3D.cnvPlaneLocation(pos, mCp, mU, mV);
            PolygonD polygon = new PolygonD(mPolygon);
            double len = polygon.length(p);
            if (len < 0)
                return -1;
            for (int i = 1; i <= mPolygon.Count; i++) {
                if (len < polygon.length(i))
                    return i - 1;
            }
            return 0;
        }

        /// <summary>
        /// 座標リストの開始位置を変更する
        /// </summary>
        /// <param name="st">開始位置</param>
        public void changeStart(int st)
        {
            st = st % mPolygon.Count;
            List<PointD> plist = mPolygon.GetRange(0, st);
            mPolygon.RemoveRange(0, st);
            mPolygon.AddRange(plist);
        }

        /// <summary>
        /// 座標点の挿入
        /// </summary>
        /// <param name="n">挿入位置</param>
        /// <param name="p">3D座標</param>
        public void insert(int n, Point3D p)
        {
            if (mPolygon == null)
                mPolygon = new List<PointD>();
            mPolygon.Insert(n, Point3D.cnvPlaneLocation(p, mCp, mU, mV));
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
            PointD spp = Point3D.cnvPlaneLocation(sp, mCp, mU, mV);
            PointD epp = Point3D.cnvPlaneLocation(ep, mCp, mU, mV);
            if (spp.isNaN() || epp.isNaN())
                return;
            PolygonD polygon = new PolygonD(mPolygon);
            polygon.offset(spp, epp);
            polygon.squeeze();
            mPolygon = polygon.mPolygon;
        }

        /// <summary>
        /// ミラー
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(Point3D sp, Point3D ep)
        {
            Line3D l = new Line3D(sp, ep);
            mCp = l.mirror(mCp);
            l.mSp = new Point3D();
            mU = l.mirror(mU);
            mV = l.mirror(mV);
        }

        /// <summary>
        /// ミラー
        /// </summary>
        /// <param name="line">基準線</param>
        /// <param name="face">2D平面</param>
        public void mirror(Line3D line, FACE3D face)
        {
            mCp = line.mirror(mCp, face);
            line.mSp = new Point3D();
            mU = line.mirror(mU,face);
            mV = line.mirror(mV, face);
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
        /// ポリゴンの分割(ポリラインに変換)
        /// </summary>
        /// <param name="pos">3D分割座標</param>
        /// <returns>ポリライン</returns>
        public Polyline3D divide(Point3D pos)
        {
            PointD p = pos.toPointD(mCp, mU, mV);
            PolygonD polygon = new PolygonD(mPolygon);
            PolylineD polyline = polygon.divide(p);
            return new Polyline3D(polyline, mCp, mU, mV);
        }


        /// <summary>
        /// 拡大縮小
        /// </summary>
        /// <param name="cp">拡大中心</param>
        /// <param name="scale">倍率</param>
        public void scale(Point3D cp, double scale)
        {
            PolygonD polygon = new PolygonD(mPolygon);
            PointD cp2 = Point3D.cnvPlaneLocation(cp, mCp, mU, mV);
            polygon.scale(cp2, scale);
            mPolygon = polygon.mPolygon;
        }


        /// <summary>
        /// 指定点に最も近い座標点を移動する
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        /// <param name="pickPos">ピック位置</param>
        public void stretch(Point3D vec, Point3D pickPos, bool arc = false)
        {
            PointD vvec = vec.toPointD(new Point3D(0, 0, 0), mU, mV);
            PointD ppos = pickPos.toPointD(mCp, mU, mV);
            PolygonD polygon = new PolygonD(mPolygon);
            polygon.stretch(vvec, ppos, arc);
            mPolygon = polygon.mPolygon;
            squeeze();
        }

        /// <summary>
        /// 法線ベクトル
        /// </summary>
        /// <returns></returns>
        public Point3D getNormal()
        {
            Point3D normal = new Point3D();
            for (int i = 0; i < mPolygon.Count - 2; i++) {
                normal += toPoint3D(i).getNormal(toPoint3D(i + 1), toPoint3D(i + 2));
            }
            normal.unit();
            return normal;
        }

        /// <summary>
        /// 多角形の法線
        /// </summary>
        /// <returns>法線ベクトル</returns>
        public Point3D getNormalLine()
        {
            bool ccw = isCounterClockWise();
            for (int i = 0; i < mPolygon.Count; i++) {
                for (int j = 2; j < mPolygon.Count + 2; j++) {
                    int cp = (i + 1) % mPolygon.Count;
                    int p1 = (i) % mPolygon.Count;
                    int p2 = (i + j) % mPolygon.Count;
                    double angle = mPolygon[cp].angle(mPolygon[p2], mPolygon[p1], false);
                    if (ccw && 0 < Math.Sign(angle) * (Math.PI - Math.Abs(angle))) {
                        return toPoint3D(p1).getNormal(toPoint3D(cp), toPoint3D(p2));
                    } else if (!ccw && 0 > Math.Sign(angle) * (Math.PI - Math.Abs(angle))) {
                        return toPoint3D(p1).getNormal(toPoint3D(cp), toPoint3D(p2));
                    }
                }
            }
            return new Point3D();
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
        /// 多角形の回転方向(角度で判定)
        /// </summary>
        /// <returns>反時計回り</returns>
        public bool isCounterClockWise()
        {
            return isCounterClockWise(mPolygon);
        }

        /// <summary>
        /// 多角形の回転方向(角度で判定)
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns></returns>
        public bool isCounterClockWise(FACE3D face)
        {
            List<PointD> plist = toPointD(face);
            return isCounterClockWise(plist);
        }

        /// <summary>
        /// 多角形の回転方向(角度で判定)
        /// </summary>
        /// <param name="plist">2D座標リスト</param>
        /// <returns>反時計回り</returns>
        public bool isCounterClockWise(List<PointD> plist)
        {
            double ang = 0;
            for (int i = 0; i < plist.Count; i++) {
                int cp = (i + 1) % plist.Count;
                int p1 = (i) % plist.Count;
                int p2 = (i + 2) % plist.Count;
                double angle = plist[cp].angle(plist[p2], plist[p1], false);
                ang += Math.Sign(angle) * (Math.PI - Math.Abs(angle));
            }
            return  0 < ang;
        }

        /// <summary>
        /// ポリゴンの長さ(周長)
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            PolygonD polygon = new PolygonD(mPolygon);
            return polygon.length();
        }

        /// <summary>
        /// 始点からの周長
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>長さ</returns>
        public double length(Point3D pos)
        {
            PointD p = pos.toPointD(mCp, mU, mV);
            PolygonD polygon = new PolygonD(mPolygon);
            return polygon.length(p);
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
            //  隣と同じ座標削除
            for (int i = mPolygon.Count - 1; i >= 0; i--) {
                if (mPolygon[i].length(mPolygon[i == 0 ? ^1 : i - 1]) < mEps) {
                    if (mPolygon[i].type == 1)
                        mPolygon.RemoveAt(i == 0 ? mPolygon.Count - 1 : i - 1);
                    else
                        mPolygon.RemoveAt(i);
                }
            }
            //  角度が180°になるものを削除
            for (int i = mPolygon.Count - 2; i > 0; i--) {
                if ((mPolygon[i - 1].type == 0 && mPolygon[i].type == 0 && mPolygon[i + 1].type == 0)
                    && (Math.PI - mPolygon[i].angle(mPolygon[i == 0 ? ^1 : i - 1], mPolygon[i + 1])) < mEps)
                    mPolygon.RemoveAt(i);
            }
        }

        /// <summary>
        /// 多角形を三角形の集合に変換(座標リスト = 3座標 x 三角形の数)
        /// </summary>
        /// <param name="divAng">円弧の分割角度</param>
        /// <returns>(3角形の座標リスト,リスト反転)</returns>
        public (List<Point3D> triangles, bool reverse) cnvTriangles(double divAng = 0)
        {
            bool reverse = false;
            List<PointD> polygon = new PolygonD(mPolygon).toPointList(divAng);
            int polygonCount = polygon.Count;
            int removeCount = 0;
            List<PointD> triangles;
            //  座標リストの回転方向を反時計回りにする
            if (!isCounterClockWise(polygon)) {
                polygon.Reverse();
                reverse = true;
            }
            //  三角形に分割
            (triangles, removeCount) = cnvTriangles(polygon);
            //  3D座標に変換
            List<Point3D> triangle3d = triangles.ConvertAll(p => Point3D.cnvPlaneLocation(p, mCp, mU, mV));
            return (triangle3d, reverse);
        }

        /// <summary>
        /// 多角形を三角形の集合に変換(座標リスト=3座標x三角形の数)
        /// 反時計回りに三角形を作っていき、2点目の角度が正で他の輪郭線と重ならないものを使う
        /// </summary>
        /// <param name="pplist">多角形の座標リスト</param>
        /// <returns>(3角形の座標リスト,削除座標数)</returns>
        public (List<PointD>, int) cnvTriangles(List<PointD> pplist)
        {
            List<PointD> triangles = new List<PointD>();
            int removeCount = 0;
            List<PointD> plist = pplist.ConvertAll(p => p.toCopy());
            int n = 0;
            while (2 < plist.Count && n < plist.Count) {
                PolygonD polygon = new PolygonD(plist);
                int n0 = (n) % plist.Count;
                int n1 = (n + 1) % plist.Count;
                int n2 = (n + 2) % plist.Count;
                //  反時計回りの３点の角度
                double ang = plist[n1].angle(plist[n2], plist[n0], false);
                //  他の輪郭線と重ならないことをチェック
                LineD line = new LineD(plist[n0], plist[n2]);
                List<PointD> iplist = polygon.intersection(line);
                iplist = plistSqueeze(iplist, line.toPointList());
                if (Math.Abs(ang) < mEps || Math.Abs(ang - Math.PI) < mEps) {
                    //  不要データ
                    plist.RemoveAt(n1);
                    removeCount++;
                    n = 0;
                } else if (0 < ang && iplist.Count == 0) {
                    //  三角形データ
                    List<PointD> triangle = new List<PointD>() {
                        plist[n0].toCopy(), plist[n1].toCopy(), plist[n2].toCopy()
                    };
                    if (triangleInsideChk(triangle, plist)) {
                        // 三角形内に座標点があるものは除外
                        n++;
                    } else {
                        //  三角形を登録
                        triangles.AddRange(triangle);
                        //  登録した三角形の中央頂点を除外
                        plist.RemoveAt(n1);
                        n = 0;
                        continue;
                    }
                } else {
                    //  次の候補
                    n++;
                }
            }
            return (triangles, removeCount);
        }

        /// <summary>
        /// 多角形内に座標点の有無をチェック
        /// </summary>
        /// <param name="polygon">多角形</param>
        /// <param name="plist">座標リスト</param>
        /// <returns>内外の有無</returns>
        private bool triangleInsideChk(List<PointD> polygon, List<PointD> plist)
        {
            for (int i =  0; i < plist.Count; i++) {
                if (insideChk(polygon, plist[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 多角形の内外判定
        /// </summary>
        /// <param name="plist">多角形の座標リスト</param>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool insideChk(List<PointD> plist, PointD p)
        {
            for (int i = 0; i < plist.Count; i++) {
                double ang = plist[(i + 1) % plist.Count].angle(p, plist[i], false);
                if (ang <= 0)
                    return false;
            }
            return true;
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
