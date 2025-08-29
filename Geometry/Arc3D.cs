using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    /// <summary>
    /// 3Dの円弧データ(3次元平面上の2次元の円弧)
    /// </summary>
    public class Arc3D
    {
        public Plane3D mPlane = new Plane3D();          //  円の平面
        public double mR = 1;                           //  半径
        public double mSa = 0;                          //  開始角
        public double mEa = Math.PI * 2;                //  修了角
        public bool mCcw = true;                        //  3点円弧の座標順

        public double mOpenAngle { get { return mEa - mSa; } }
        private double mEps = 1E-8;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Arc3D()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        public Arc3D(Point3D cp, double r)
        {
            mPlane.mCp = cp.toCopy();
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="u">X軸の向き</param>
        /// <param name="v">Y軸の向き</param>
        public Arc3D(Point3D cp, double r, Point3D u, Point3D v)
        {
            mPlane = new Plane3D(cp, u, v);
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="face">作成面</param>
        public Arc3D(Point3D cp, double r, FACE3D face)
        {
            mPlane = new Plane3D(cp, Point3D.getUVector(face), Point3D.getVVector(face));
            mR = r;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角</param>
        /// <param name="ea">週旅客</param>
        /// <param name="face">作成面</param>
        public Arc3D(Point3D cp, double r, double sa, double ea, FACE3D face)
        {
            mPlane = new Plane3D(cp, Point3D.getUVector(face), Point3D.getVVector(face));
            mR = r;
            mSa = sa;
            mEa = ea;
            normalize();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arc">2D円弧データ</param>
        /// <param name="face">作成面</param>
        public Arc3D(ArcD arc, FACE3D face)
        {
            mPlane = new Plane3D(new Point3D(arc.mCp, face), Point3D.getUVector(face), Point3D.getVVector(face));
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
            normalize();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="mp">中点</param>
        /// <param name="ep">終点</param>
        public Arc3D(Point3D sp, Point3D mp, Point3D ep)
        {
            List<Point3D> plist = new List<Point3D>() {
                sp, mp, ep
            };
            mPlane = new Plane3D(plist);
            PointD sp2 = mPlane.cnvPlaneLocation(plist[0]);
            PointD mp2 = mPlane.cnvPlaneLocation(plist[1]);
            PointD ep2 = mPlane.cnvPlaneLocation(plist[2]);
            ArcD arc = new ArcD(sp2, mp2, ep2);
            mPlane.mCp = mPlane.cnvPlaneLocation(arc.mCp);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
            mCcw = arc.mCcw;
            normalize();
        }

        /// <summary>
        /// 開始角と修了角の正規化
        /// 0 <= 開始角 < 2π, 開始角 <= 修了角 < 4π
        /// </summary>
        public void normalize()
        {
            mSa = ylib.mod(mSa, Math.PI * 2);
            mEa = ylib.mod(mEa, Math.PI * 2);
            if (mEa <= mSa + mEps)
                mEa += Math.PI * 2;
        }

        /// <summary>
        /// 円弧の平面
        /// </summary>
        /// <returns>2D平面</returns>
        public FACE3D getFace()
        {
            return mPlane.mU.getFace(mPlane.mV);
        }

        /// <summary>
        /// 平面が指定の平面と同じか
        /// </summary>
        /// <param name="face">平面</param>
        /// <returns>同平面</returns>
        public bool isFace(FACE3D face)
        {
            return mPlane.mU.isFace(mPlane.mV, face);
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns>Arc3D</returns>
        public Arc3D toCopy()
        {
            Arc3D arc = new Arc3D(mPlane.mCp, mR, mPlane.mU, mPlane.mV);
            arc.mSa = mSa;
            arc.mEa = mEa;
            return arc;
        }

        /// <summary>
        /// 円弧データをコピーする
        /// </summary>
        /// <param name="arc">円弧</param>
        public void setArc(Arc3D arc)
        {
            mPlane = new Plane3D(arc.mPlane.mCp, arc.mPlane.mU, arc.mPlane.mV);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
        }

        /// <summary>
        /// 分割座標点リストに変換
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPoint3D(int divNo)
        {
            return toPoint3D((mEa - mSa) / divNo);
        }

        /// <summary>
        /// 分割座標点リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPoint3D(double divAng = 0)
        {
            List<Point3D> plist = new List<Point3D>();
            plist.Add(getPosition(mSa));
            if (0 < divAng) {
                double ang = mSa + divAng;
                while (ang < mEa) {
                    plist.Add(getPosition(ang));
                    ang += divAng;
                }
            } else {
                double ang = (mSa + mEa) / 2;
                Point3D p = getPosition(ang);
                p.type = 1;
                plist.Add(p);
            }
            plist.Add(getPosition(mEa));
            return plist;
        }

        /// <summary>
        /// 円弧を含む座標点リストに変換
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toArcPoint3D(double divAng = Math.PI / 20)
        {
            if ((Math.Abs(mPlane.mV.x) < mEps || Math.Abs(mPlane.mV.y) < mEps || Math.Abs(mPlane.mV.z) < mEps) &&
                (Math.Abs(mPlane.mU.x) < mEps || Math.Abs(mPlane.mU.y) < mEps || Math.Abs(mPlane.mU.z) < mEps)) {
                //  XY,YZ,ZX平面上の場合、3点円弧の座標点に変換
                List<Point3D> plist = new List<Point3D>();
                plist.Add(getPosition(mSa));
                if (mOpenAngle < Math.PI) {
                    plist.Add(getPosition(mSa + mOpenAngle / 2));
                    plist[1].type = 1;
                } else {
                    plist.Add(getPosition(mSa + mOpenAngle / 4));
                    plist[1].type = 1;
                    plist.Add(getPosition(mSa + mOpenAngle / 2));
                    plist.Add(getPosition(mSa + mOpenAngle * 3 / 4));
                    plist[3].type = 1;

                }
                plist.Add(getPosition(mEa));
                return plist;
            } else {
                //  軸平面から外れる場合、単なる座標点リストに変換
                return toPoint3D(divAng);
            }
        }

        /// <summary>
        /// ポリラインに変換する
        /// </summary>
        /// <param name="divAng"></param>
        /// <returns></returns>
        public Polyline3D toPolyline3D(double divAng = Math.PI / 20)
        {
            Polyline3D polyline = new Polyline3D();
            polyline.mPolyline = toPointD(divAng);
            polyline.mPlane = mPlane.toCopy();
            return polyline;
        }

        /// <summary>
        /// 2D座標リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD(double divAng = Math.PI / 20)
        {
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            if (0 < divAng)
                return arc.toPointList(divAng);
            else
                return arc.to3PointList();
        }

        /// <summary>
        /// 分割座標点リストを2D平面に変換
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>座標点リスト</returns>
        public List<PointD> toPointD(int divNo, FACE3D face)
        {
            return toPointD((mEa - mSa) / divNo, face);
        }

        /// <summary>
        /// 2D円弧に変換
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public ArcD toArcD(FACE3D face)
        {
            PointD sp = startPosition().toPoint(face);
            PointD mp = midPosition().toPoint(face);
            PointD ep = endPosition().toPoint(face);
            return new ArcD(sp, mp, ep);
        }

        /// <summary>
        /// 2D平面の楕円に変換
        /// </summary>
        /// <param name="face">2D平面</param>
        /// <returns>2D楕円</returns>
        public EllipseD toEllipseD(FACE3D face)
        {
            EllipseD ellipse = new EllipseD();
            ellipse.mCp = mPlane.mCp.toPoint(face);
            PointD sp = getPosition(0).toPoint(face);
            PointD ep = getPosition(Math.PI/ 2).toPoint(face);
            ellipse.mRx = ellipse.mCp.length(sp);
            ellipse.mRy = ellipse.mCp.length(ep);
            ellipse.mRotate = sp.angle(ellipse.mCp);
            ellipse.mSa = mSa;
            ellipse.mEa = mEa;
            return ellipse;
        }

        /// <summary>
        /// 分割座標点リストを2D平面に変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <param name="face">2D平面</param>
        /// <returns>2D座標リスト</returns>
        public List<PointD> toPointD(double divAng = Math.PI / 20, FACE3D face = FACE3D.XY)
        {
            List<Point3D> plist = toPoint3D(divAng);
            return plist.ConvertAll(p => p.toPoint(face));
        }

        /// <summary>
        /// 円周上の端点リスト(端点+4分割点)
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toPeackList()
        {
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            List<PointD> plist = arc.toPeakList();
            return plist.ConvertAll(p => cnvPosition(p));
        }

        /// <summary>
        /// 終点側から座標点リストに変換
        /// </summary>
        /// <param name="divAng">分割角度</param>
        /// <returns>座標点リスト</returns>
        public List<Point3D> toReversePoint3D(double divAng = Math.PI / 20)
        {
            List<Point3D> plist = new List<Point3D>();
            double ang = mEa;
            while (ang > mSa) {
                plist.Add(getPosition(ang));
                ang -= divAng;
            }
            plist.Add(getPosition(mSa));
            return plist;
        }

        /// <summary>
        /// 始点座標
        /// </summary>
        /// <returns></returns>
        public Point3D startPosition()
        {
            return getPosition(mSa);
        }

        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns></returns>
        public Point3D endPosition()
        {
            return getPosition(mEa);
        }

        /// <summary>
        /// 中間点の座標
        /// </summary>
        /// <returns></returns>
        public Point3D midPosition()
        {
            return getPosition((mSa + mEa) / 2);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="v">移動ベクトル</param>
        public void translate(Point3D v)
        {
            mPlane.translate(v);
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="ang">回転角度</param>
        /// <param name="face">回転面ン</param>
        public void rotate(Point3D cp, double ang, FACE3D face)
        {
            mPlane.rotate(cp, ang, face);
        }

        /// <summary>
        /// 反転
        /// </summary>
        /// <param name="line">反転基準線</param>
        /// <param name="face">2D平面</param>
        public void mirror(Line3D line, FACE3D face)
        {
            mPlane.mirror(line, face);
        }

        /// <summary>
        /// オフセット
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(Point3D sp, Point3D ep)
        {
            mR += ep.length(mPlane.mCp) - sp.length(mPlane.mCp);
        }

        /// <summary>
        /// 2D位置での分割(ポリラインにして近似的に分割角度を求める)
        /// </summary>
        /// <param name="pos">2D分割位置</param>
        /// <param name="face">作成面</param>
        /// <returns>円弧リスト</returns>
        public List<Arc3D> divide(PointD pos, FACE3D face)
        {
            List<Arc3D> arcs = new List<Arc3D>();
            //  2D平面上で円弧に対する交点角度を求める
            double ang = intersectionAngle(pos, face);
            //  分割した円弧を作成
            Arc3D arc = toCopy();
            arc.mEa = ang;
            arc.normalize();
            arcs.Add(arc);
            arc = toCopy();
            arc.mSa = ang;
            arc.normalize();
            arcs.Add(arc);
           return arcs;
        }

        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="pos">分割位置</param>
        /// <returns>円弧リスト</returns>
        public List<Arc3D> divide(Point3D pos)
        {
            List<Arc3D> arcs = new List<Arc3D>();
            //  2D平面上で円弧に対する交点角度を求める
            double ang = intersectionAngle(pos);
            //  分割した円弧を作成
            Arc3D arc = toCopy();
            arc.mEa = ang;
            arc.normalize();
            arcs.Add(arc);
            arc = toCopy();
            arc.mSa = ang;
            arc.normalize();
            arcs.Add(arc);
            return arcs;
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        /// <param name="face">2D平面</param>
        public void trim(PointD sp, PointD ep, FACE3D face)
        {
            mSa = intersectionAngle(sp, face);
            mEa = intersectionAngle(ep, face);
            normalize();
        }

        /// <summary>
        /// トリム
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void trim(Point3D sp, Point3D ep)
        {
            mSa = intersectionAngle(sp);
            mEa = intersectionAngle(ep);
            normalize();
        }

        /// <summary>
        /// ストレッチ
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        /// <param name="pickPos">ピック位置</param>
        public void stretch(Point3D vec, Point3D pickPos)
        {
            PointD svec = vec.toPointD(new Point3D(0, 0, 0), mPlane.mU, mPlane.mV);
            PointD ppos = pickPos.toPointD(mPlane.mCp, mPlane.mU, mPlane.mV);
            ArcD arc = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            arc.stretch(svec, ppos);
            mPlane.mCp = mPlane.cnvPlaneLocation(arc.mCp);
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
        }

        /// <summary>
        /// 交点を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>3D座標</returns>
        public Point3D intersection(PointD pos, FACE3D face)
        {
            Point3D p = mPlane.intersection(pos, face);
            if (p != null)
                return intersection(p);
            return null;
        }

        /// <summary>
        /// 交点を求める
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>交点座標</returns>
        public Point3D intersection(Point3D pos)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(pos);
            PointD ip = arcD.intersection(p);
            if (ip != null)
                return cnvPosition(ip);
            return null;
        }

        /// <summary>
        /// 2D平面から投影した位置で点と交点を求める
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Point3D p, FACE3D face)
        {
            return intersection(p.toPoint(face), face);
        }

        /// <summary>
        /// 表示面で線分と交わる参照位置に最も近い円弧上の座標
        /// </summary>
        /// <param name="line">線分</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Line3D line, PointD pos, FACE3D face)
        {
            EllipseD elli = toEllipseD(face);
            LineD l = line.toLineD(face);
            List<PointD> iplist = elli.intersection(l, false);
            PointD ip = iplist.MinBy(p => p.length(pos));
            if (ip != null)
                return intersection(ip, face);
            return null;
        }

        /// <summary>
        /// 表示面で円弧と交わる参照位置に最も近い円弧上の座標
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="pos">参照位置</param>
        /// <param name="face">2D平面</param>
        /// <returns>交点</returns>
        public Point3D intersection(Arc3D arc, PointD pos, FACE3D face)
        {
            FACE3D face0 = getFace();
            FACE3D face1 = arc.getFace();
            if (face0 != FACE3D.NON && isFace(face1)) {
                ArcD arc0 = toArcD(face);
                ArcD arc1 = arc.toArcD(face);
                List<PointD> iplist = arc0.intersection(arc1);
                PointD ip = iplist.MinBy(p => p.length(pos));
                if (ip != null)
                    return intersection(ip, face);
            } else {
                EllipseD elli = toEllipseD(face);
                EllipseD elli2 = arc.toEllipseD(face);
                List<PointD> iplist = elli.intersection(elli2);
                PointD ip = iplist.MinBy(p => p.length(pos));
                if (ip != null)
                    return intersection(ip, face);
            }

            return null;
        }

        /// <summary>
        /// 交点の円上の角度を求める
        /// </summary>
        /// <param name="pos">2D座標</param>
        /// <param name="face">2D平面</param>
        /// <returns>角度</returns>
        public double intersectionAngle(PointD pos, FACE3D face)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(new Point3D(pos, face));
            PointD ip = arcD.intersection(p);
            return arcD.getAngle(ip);
        }

        /// <summary>
        /// 交点の円上の角度
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>角度</returns>
        public double intersectionAngle(Point3D pos)
        {
            ArcD arcD = new ArcD(new PointD(0, 0), mR, mSa, mEa);
            PointD p = cnvPosition(pos);
            PointD ip = arcD.intersection(p);
            return arcD.getAngle(ip);
        }

        /// <summary>
        /// 分割した座標点リストで最も近い座標゜
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <param name="face">2D平面</param>
        /// <returns>座標</returns>
        public Point3D nearPoint(PointD pos, int divideNo, FACE3D face)
        {
            List<PointD> plist = toPointD(divideNo, face);
            PointD p = plist.MinBy(p => p.length(pos));
            return intersection(p, face);
        }

        /// <summary>
        /// 分割した座標点リストで最も近い座標
        /// </summary>
        /// <param name="pos">指定座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public Point3D nearPoint(Point3D pos, int divideNo)
        {
            List<Point3D> plist = toPoint3D(divideNo);
            return plist.MinBy(p => p.length(pos));
        }

        /// <summary>
        /// 円周上の点
        /// </summary>
        /// <param name="ang">角度(rad)</param>
        /// <returns>座標</returns>
        public Point3D getPosition(double ang)
        {
            Point3D uv = (mPlane.mU * Math.Cos(ang)) + (mPlane.mV * Math.Sin(ang));
            return mPlane.mCp + uv * mR;
        }

        /// <summary>
        /// 平面の座標を3D座標に変換
        /// Plane3D.cnvPlaneLocationと同じ
        /// P(mU,mV) = c + mU * x + mV * y
        /// </summary>
        /// <param name="p">2D座標</param>
        /// <returns>3D座標</returns>
        public Point3D cnvPosition(PointD p)
        {
            return mPlane.cnvPlaneLocation(p);
        }

        /// <summary>
        /// 3D座標から平面座標に変換
        /// Plane3D.cnvPlaneLocationと同じ
        /// </summary>
        /// <param name="pos">3D座標</param>
        /// <returns>2D座標</returns>
        public PointD cnvPosition(Point3D pos)
        {
            return mPlane.cnvPlaneLocation(pos);
        }
    }
}
