using System;
using System.Collections.Generic;

namespace CoreLib
{
    /// <summary>
    /// コンストラクタ
    ///  ArcD()
    ///  ArcD(PointD cp, double r, double sa = 0, double ea = Math.PI * 2)
    ///  ArcD(double cx, double cy, double r, double sa = 0, double ea = Math.PI * 2)
    ///  ArcD(PointD sp, PointD mp, PointD ep)              三点円弧
    ///  ArcD(PointD cp, PointD sp, double ang)             中心座標と開始座標、開口角
    ///  ArcD(PointD cp, double r)                          円
    ///  ArcD(ArcD arc)
    ///  
    ///  void setArc(PointD cp, PointD sp, PointD ep)       中心点と始点、終点から円弧を作成
    ///  void normalize()                                   開始角と修了角の正規化
    ///  string ToString()
    ///  string ToString(string format)                     書式付き文字列変換
    ///  ArcD toCopy()                                      コピーを作成
    ///  void translate(PointD vec)                         ベクトル分移動させる
    ///  void rotate(double angle)                          円弧の回転
    ///  void rotate(PointD cp, double ang)                 指定点を中心に回転
    ///  void rotate(PointD cp, PointD mp)                  指定点を中心に回転
    ///  void mirror(PointD sp, PointD ep)                  指定成分でミラーする
    ///  void scale(PointD cp, double scale)                原点を指定して拡大縮小
    ///  void offset(PointD sp, PointD ep)                  円弧の半径をオフセットする
    ///  void trim(PointD sp, PointD ep)                    指定点でトリムする
    ///  void trimNear(PointD tp, PointD pos)               ピックした位置に近い方を消すようにトリミング
    ///  void trimFar(PointD tp, PointD pos)                ピックした位置に遠いを消すようにトリミング
    ///  void stretch(PointD vec, PointD pos)               円弧を端点または半径をストレッチ
    ///  PointD startPoint()                                始点座標
    ///  PointD endPoint()                                  終点座標
    ///  PointD middlePoint()                               中間点座標
    ///  List<PointD> dividePoints(int divNo)               円弧を分割した座標点リスト
    ///  PointD getPoint(double ang, bool clockwise = false)    角度から円周上の座標を求める
    ///  void setPoint(PointD sp, PointD ep)                指定座標で始角と終角を設定
    ///  double getAngle(PointD p)                          指定座標と中心との角度を求める
    ///  List<PointD> toPeakList()                          円周上の端点リスト(端点+4分割点)
    ///  List<PointD> toPointList(int divideNo, bool clockwise = false)     円弧の分割点リストを作成
    ///  List<PointD> toPointList(double da, bool clockwise = false)    円弧の分割点リストを作成
    ///  List<PointD> toAnglePointList(double da, bool clockwise = false)   円弧の角度分割点リストを作成
    ///  PointD nearPoints(PointD p, int divideNo = 4)      円弧の分割点で最も近い点を求める
    ///  bool onPoint(PointD p)                             点が円弧上にあるかの判定
    ///  bool innerAngle(double ang)                        指定の角度が円弧内かの判定
    ///  List<PointD> intersection(PointD p, bool on = true)    点との交点リスト
    ///  PointD intersection(PointD p)                      垂点(円に対して)
    ///  List<PointD> intersection(LineD line, bool on = true)  線分との交点
    ///  List<PointD> intersection(ArcD arc, bool on = true)    円と円との交点を求める
    ///  List<PointD> intersection(PolylineD polyline, bool on = true)  ポリラインとの交点を求める
    ///  List<PointD> intersection(PolygonD polygon, bool on = true)    ポリゴンとの交点を求める
    ///  List<PointD> tangentPoint(PointD p)                点からの接線の接点リスト
    ///  List<LineD> tangentArc(ArcD arc)                   円と円との接線リスト
    ///  List<ArcD> tangentCircle(LineD la, LineD lb, LineD lc) 3線分に接する円リスト
    ///  List<ArcD> tangentCircle(LineD lf, LineD ls, double r) 2線分に接する円のリスト
    ///  List<ArcD> tangentCircle(LineD lf, ArcD af, double r)  線分と円に接する円
    ///  List<ArcD> tangentCircle(ArcD a1, ArcD a2, double r)   2円に接する円
    ///  
    /// </summary>

    public class ArcD
    {
        public PointD mCp;
        public double mR = 0;
        public double mSa = 0;
        public double mEa = Math.PI * 2;
        public double mOpenAngle { get { return mEa - mSa; } }
        private double mEps = 1E-8;

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ArcD() {
            mCp = new PointD();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">修了角(rad)</param>
        public ArcD(PointD cp, double r, double sa = 0, double ea = Math.PI * 2)
        {
            mCp = cp;
            mR = r;
            mSa = sa;
            mEa = ea;
        }

        /// <summary>
        /// コンストラクタ 
        /// </summary>
        /// <param name="cx">中心X座標</param>
        /// <param name="cy">中心Y座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">修了角(rad)</param>
        public ArcD(double cx, double cy, double r, double sa = 0, double ea = Math.PI * 2)
        {
            mCp = new PointD(cx, cy);
            mR = r;
            mSa = sa;
            mEa = ea;
        }

        /// <summary>
        /// コンストラクタ 3点円弧
        /// 3点を頂点とする三角形の外心(外接円の中心)から求める
        /// </summary>
        /// <param name="sp">1点目</param>
        /// <param name="mp">2点目</param>
        /// <param name="ep">3点目</param>
        public ArcD(PointD sp, PointD mp, PointD ep)
        {
            LineD l1 = new LineD(sp, mp);
            LineD l2 = new LineD(mp, ep);
            double crossProduct = l1.crossProduct(ep);  //  外積で回転方向を求める
            PointD mp1 = l1.centerPoint();
            PointD mp2 = l2.centerPoint();
            l1.rotate(mp1, Math.PI / 2);
            l2.rotate(mp2, Math.PI / 2);
            mCp = l1.intersection(l2);
            if (mCp != null) {
                mR = mCp.length(sp);
                LineD sl = new LineD(mCp, sp);
                LineD el = new LineD(mCp, ep);
                mSa = sl.angle();
                mEa = el.angle();
                if (crossProduct < 0) {
                    YLib.Swap(ref mSa, ref mEa);
                }
                mEa += mEa < mSa ? Math.PI * 2 : 0;
            }
        }

        /// <summary>
        /// コンストラクタ 中心点と始点と開口角
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="sp">開始点</param>
        /// <param name="ang">開口角</param>
        public ArcD(PointD cp, PointD sp, double ang)
        {
            mCp = new PointD(cp.x, cp.y);
            mR = cp.length(sp);
            LineD sl = new LineD(mCp, sp);
            mSa = sl.angle();
            mEa = mSa + ang;
        }

        /// <summary>
        /// コンストラクタ (円)
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="r">半径</param>
        public ArcD(PointD cp, double r)
        {
            mCp = new PointD(cp.x, cp.y);
            mR = r;
            mSa = 0;
            mEa = Math.PI * 2;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arc"></param>
        public ArcD(ArcD arc)
        {
            mCp = arc.mCp.toCopy();
            mR = arc.mR;
            mSa = arc.mSa;
            mEa = arc.mEa;
        }

        /// <summary>
        /// 中心点と始点、終点から円弧を作成
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void setArc(PointD cp, PointD sp, PointD ep)
        {
            mCp = cp;
            mR = cp.length(sp);
            mSa = sp.angle(mCp);
            mEa = ep.angle(mCp);
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

        public override string ToString()
        {
            return $"({mCp.x},{mCp.y}),{mR},{mSa},{mEa}";
        }

        /// <summary>
        /// 書式付き文字列変換
        /// </summary>
        /// <param name="format">書式</param>
        /// <returns>文字列</returns>
        public string ToString(string format)
        {
            return $"({mCp.x.ToString(format)},{mCp.y.ToString(format)}),{mR.ToString(format)},{mSa.ToString(format)},{mEa.ToString(format)}";
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns></returns>
        public ArcD toCopy()
        {
            return new ArcD(mCp.toCopy(), mR, mSa, mEa);
        }

        /// <summary>
        /// ベクトル分移動させる
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        public void translate(PointD vec)
        {
            mCp.translate(vec);
        }

        /// <summary>
        /// 円弧の回転
        /// </summary>
        /// <param name="angle">回転角(rad)</param>
        public void rotate(double angle)
        {
            mSa += angle;
            mEa += angle;
            normalize();
        }

        /// <summary>
        /// 指定点を中心に回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="ang">回転角(rad)</param>
        public void rotate(PointD cp, double ang)
        {
            rotate(ang);
            mCp.rotate(cp, ang);
        }

        /// <summary>
        /// 指定点を中心に回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="mp">回転角座標</param>
        public void rotate(PointD cp, PointD mp)
        {
            double ang = mp.angle(cp);
            rotate(cp, ang);
        }

        /// <summary>
        /// 指定成分でミラーする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(PointD sp, PointD ep)
        {
            PointD ps = startPoint();
            PointD pe = endPoint();
            mCp.mirror(sp, ep);
            ps.mirror(sp, ep);
            pe.mirror(sp, ep);
            mSa = getAngle(pe);
            mEa = getAngle(ps);
            normalize();
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点</param>
        /// <param name="scale">拡大率</param>
        public void scale(PointD cp, double scale)
        {
            mCp.scale(cp, scale);
            mR *= scale;
        }

        /// <summary>
        /// 円弧の半径をオフセットする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(PointD sp, PointD ep)
        {
            double dis = mCp.length(ep) - mCp.length(sp);
            mR += dis;
        }

        /// <summary>
        /// 指定点でトリムする
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void trim(PointD sp, PointD ep)
        {
            mSa = getAngle(sp);
            mEa = getAngle(ep);
            normalize();
        }

        /// <summary>
        /// ピックした位置に近い方を消すようにトリミングする
        /// </summary>
        /// <param name="tp">トリミング位置</param>
        /// <param name="pos">ピック位置</param>
        public void trimNear(PointD tp, PointD pos)
        {
            PointD sp = startPoint();
            PointD ep = endPoint();
            double sl = pos.length(sp);
            double el = pos.length(ep);
            if (sl < el) {
                mSa = getAngle(pos);
            } else {
                mEa = getAngle(pos);
            }
            normalize();
        }

        /// <summary>
        /// ピックした位置に遠いを消すようにトリミングする
        /// </summary>
        /// <param name="tp">トリミング位置</param>
        /// <param name="pos">ピック位置</param>
        public void trimFar(PointD tp, PointD pos)
        {
            PointD sp = startPoint();
            PointD ep = endPoint();
            double sl = pos.length(sp);
            double el = pos.length(ep);
            if (sl < el) {
                mEa = getAngle(pos);
            } else {
                mSa = getAngle(pos);
            }
            normalize();
        }

        /// <summary>
        /// 円弧を端点または半径をストレッチする
        /// </summary>
        /// <param name="vec">ストレッチの方向</param>
        /// <param name="pos">ストレッチの基準点</param>
        public void stretch(PointD vec, PointD pos)
        {
            PointD sp = startPoint();
            PointD mp = middlePoint();
            PointD ep = endPoint();
            double sl = pos.length(sp);
            double ml = pos.length(mp);
            double el = pos.length(ep);
            if (Math.PI * 2 <= mOpenAngle || (ml < sl && ml < el)) {
                mp += vec;
                mR = mp.length(mCp);
            } else if (sl < el) {
                sp += vec;
                mSa = getAngle(sp);
                normalize();
            } else {
                ep += vec;
                mEa = getAngle(ep);
                normalize();
            }
        }


        /// <summary>
        /// 始点座標
        /// </summary>
        /// <returns></returns>
        public PointD startPoint()
        {
            return getPoint(mSa) ;
        }

        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns></returns>
        public PointD endPoint()
        {
            return getPoint(mEa);
        }

        /// <summary>
        /// 中間点座標
        /// </summary>
        /// <returns>座標</returns>
        public PointD middlePoint()
        {
            return getPoint((mSa + mEa) / 2);
        }

        /// <summary>
        /// 円弧を分割した座標点リスト
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <returns>座標点リスト</returns>
        //public List<PointD> dividePoints(int divNo)
        //{
        //    List<PointD> points = new List<PointD>();
        //    if (0 < divNo) {
        //        double da = mOpenAngle / divNo;
        //        double ang = mSa;
        //        while (ang <= mEa + mEps) {
        //            points.Add(getPoint(ang));
        //            ang += da;
        //        }
        //    }
        //    return points;
        //}

        /// <summary>
        /// 角度から円周上の座標を求める
        /// </summary>
        /// <param name="ang"></param>
        /// <param name="clockwise">回転方向</param>
        /// <returns></returns>
        public PointD getPoint(double ang, bool clockwise = false)
        {
            double dx = mR * Math.Cos(ang);
            double dy = mR * Math.Sin(ang) * (clockwise ? -1 : 1);
            PointD sp = new PointD(dx, dy);
            sp.offset(mCp);
            return sp;
        }

        /// <summary>
        /// 指定座標で始角と終角を設定
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void setPoint(PointD sp, PointD ep)
        {
            mSa = sp.angle(mCp);
            mEa = ep.angle(mCp);
            normalize();
        }

        /// <summary>
        /// 指定座標の角度を求める
        /// </summary>
        /// <param name="p">座標</param>
        /// <returns>角度(rad)</returns>
        public double getAngle(PointD p)
        {
            double ang = p.angle(mCp);
            ang = ylib.mod(ang, Math.PI * 2);
            if (ang < mSa)
                ang += Math.PI * 2;
            return ang;
        }

        /// <summary>
        /// 円周上の端点リスト(端点+4分割点)
        /// </summary>
        /// <returns>座標リスト</returns>
        public List<PointD> toPeakList()
        {
            List<PointD> peakList = new List<PointD>();
            peakList.Add(startPoint());
            double pi2 = Math.PI / 2;
            double ang = Math.Floor(mSa / pi2) * pi2; ;
            while (ang < mEa - mEps) {
                if (mSa < ang)
                    peakList.Add(getPoint(ang));
                ang += pi2;
            }
            peakList.Add(endPoint());
            return peakList;
        }

        /// <summary>
        /// 円弧の分割点リストを作成
        /// </summary>
        /// <param name="divideNo">分割数</param>
        /// <param name="clockwise">回転方向</param>
        /// <returns>座標リスト</returns>
        public List<PointD> toPointList(int divideNo, bool clockwise = false)
        {
            List<PointD> pointList = new List<PointD>();
            double ang = mSa;
            double da = (mEa - mSa) / divideNo;
            while (ang < mEa - mEps) {
                pointList.Add(getPoint(ang, clockwise));
                ang += da;
            }
            pointList.Add(endPoint());
            return pointList;
        }

        /// <summary>
        /// 円弧の分割点リストを作成
        /// </summary>
        /// <param name="da">分割角度</param>
        /// <param name="clockwise">回転方向</param>
        /// <returns>座標リスト</returns>
        public List<PointD> toPointList(double da, bool clockwise = false)
        {
            List<PointD> pointList = new List<PointD>();
            double ang = mSa;
            while (ang < mEa - mEps) {
                pointList.Add(getPoint(ang, clockwise));
                ang += da;
            }
            pointList.Add(endPoint());
            return pointList;
        }

        /// <summary>
        /// 円弧の分割点リストを作成
        /// </summary>
        /// <param name="da">分割角度</param>
        /// <param name="clockwise">回転方向</param>
        /// <returns>座標リスト</returns>
        //public List<PointD> toAnglePointList(double da, bool clockwise = false)
        //{
        //    List<PointD> pointList = new List<PointD>();
        //    double ang = mSa;
        //    while (ang < mEa - mEps) {
        //        pointList.Add(getPoint(ang, clockwise));
        //        ang += da;
        //    }
        //    pointList.Add(getPoint(mEa, clockwise));
        //    return pointList;
        //}

        /// <summary>
        /// 円弧の分割点で最も近い点を求める
        /// </summary>
        /// <param name="p">近傍座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public PointD nearPoints(PointD p, int divideNo = 4)
        {
            List<PointD> points = toPointList(divideNo);
            double l = double.MaxValue;
            PointD np = new PointD();
            foreach (PointD pt in points) {
                double lt = pt.length(p);
                if (lt < l) {
                    np = pt;
                    l = lt;
                }
            }
            return np;
        }

        /// <summary>
        /// 点が円弧上にあるかの判定
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>判定</returns>
        public bool onPoint(PointD p)
        {
            PointD cp = intersection(p);
            if (p.length(cp) < mEps) {
                return innerAngle(p.angle(mCp));
            }
            return false;
        }

        /// <summary>
        /// 指定の角度が円弧内かの判定
        /// </summary>
        /// <param name="ang">角度(rad)</param>
        /// <returns>判定</returns>
        public bool innerAngle(double ang)
        {
            normalize();
            ang = ylib.mod(ang, Math.PI * 2);
            ang += ang < mSa ? Math.PI * 2 : 0;
            if (ang <= mEa)
                return true;
            return false;
        }

        /// <summary>
        /// 点との交点
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="on">判定あり</param>
        /// <returns></returns>
        public List<PointD> intersection(PointD p, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = intersection(p);
            if (!on || onPoint(ip))
                plist.Add(ip);
            return plist;
        }

        /// <summary>
        /// 垂点(円に対して)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点座標</returns>
        public PointD intersection(PointD p)
        {
            LineD line = new LineD(mCp, p);
            line.setLength(mR);
            return line.pe;
        }

        /// <summary>
        /// 線分との交点
        /// </summary>
        /// <param name="line">線分</param>
        /// <param name="on">判定あり</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(LineD line, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD mp = line.intersection(mCp);           //  線分と中心点との垂点
            double l = mp.length(mCp);                      //  垂点と中心点との距離
            normalize();

            if (mR < l - mEps) {
                //  交点なし
            } else if (Math.Abs(mR - l) < mEps) {
                //  接点
                if (!on || innerAngle(mp.angle(mCp)))
                    plist.Add(mp);
            } else if (l < mEps) {
                double ang = line.angle();
                PointD cp = getPoint(ang);
                if (!on || (onPoint(cp) && line.onPoint(cp)))
                    plist.Add(cp);
                ang += Math.PI;
                cp = getPoint(ang);
                if (!on || (onPoint(cp) && line.onPoint(cp)))
                    plist.Add(cp);
            } else {
                //  交点
                double th = Math.Acos(l / mR);           //  垂線との角度
                LineD ml = new LineD(mCp, mp);
                ml.rotate(mCp, th);
                ml.setLength(mR);
                if (!on || (onPoint(ml.pe) && line.onPoint(ml.pe)))
                    plist.Add(ml.pe);
                ml = new LineD(mCp, mp);
                ml.rotate(mCp, -th);
                ml.setLength(mR);
                if (!on || (onPoint(ml.pe) && line.onPoint(ml.pe)))
                    plist.Add(ml.pe);
            }
            return plist;
        }

        /// <summary>
        /// 円と円との交点を求める
        /// </summary>
        /// <param name="Arc">円弧</param>
        /// <param name="on">円弧上の有無</param>
        /// <returns></returns>
        public List<PointD> intersection(ArcD Arc, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip;
            double dis = mCp.length(Arc.mCp);
            PointD vec = mCp.vector(Arc.mCp);
            if (mR + Arc.mR < dis || Math.Abs(mR - Arc.mR) > dis) {
                //  交点なし
            } else if (mR + Arc.mR == dis || Math.Abs(mR - Arc.mR) == dis) {
                //  接点
                vec.setLength(mR);
                ip = mCp + vec;
                if (!on || (onPoint(ip) && Arc.onPoint(ip)))
                    plist.Add(ip);
            } else {
                //  交点 余弦定理(cos(th) = (a^2+b^2-c^2) / 2ab)交点の角度を求める
                //                  a = distance(cp1,cp2) b = r1 c = r2
                double ang = Math.Acos((dis * dis + mR * mR - Arc.mR * Arc.mR) / (2 * dis * mR));
                vec.rotate(ang);
                vec.setLength(mR);
                ip = mCp + vec;
                if (!on || (onPoint(ip) && Arc.onPoint(ip)))
                    plist.Add(ip);
                vec.rotate(-ang * 2);
                ip = mCp + vec;
                if (!on || (onPoint(ip) && Arc.onPoint(ip)))
                    plist.Add(ip);
            }
            return plist;
        }

        /// <summary>
        /// ポリラインとの交点を求める
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <param name="on">線上の有無</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PolylineD polyline, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip;
            List<LineD> lines = polyline.toLineList();
            foreach (var line in lines) {
                plist.AddRange(intersection(line, on));
            }
            return plist;
        }

        /// <summary>
        /// ポリゴンとの交点を求める
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <param name="on">線上の有無</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PolygonD polygon, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip;
            List<LineD> lines = polygon.toLineList();
            foreach (var line in lines) {
                plist.AddRange(intersection(line, on));
            }
            return plist;
        }

        /// <summary>
        /// 点からの接線の接点リスト
        /// </summary>
        /// <param name="pos">点座標</param>
        /// <returns>座標リスト</returns>
        public List<PointD> tangentPoint(PointD p)
        {
            PointD pos = p.toCopy();
            pos.translate(mCp.inverse());
            PointD p1 = new PointD();
            PointD p2 = new PointD();
#if false
            //  円を原点に移動し点をX軸上に並べて求める
            double rotate = pos.angle();
            pos.rotate(-rotate);
            double l = pos.x;
            double al = Math.Sqrt(l * l - mR * mR);
            if (l < mR)
                return null;
            p1.x = mR * mR / l;
            p1.y = al / l * mR;
            p2.x = p1.x;
            p2.y = -p1.y;
            p1.rotate(rotate);
            p2.rotate(rotate);
            p1.translate(mCp);
            p2.translate(mCp);
#else
            //  円を原点に移動した相対的位置関係で求める
            double di = Math.Sqrt(pos.x * pos.x + pos.y * pos.y - mR * mR);
            double ai = pos.x * pos.x + pos.y * pos.y;
            
            p1.x = mR * (pos.x * mR + pos.y * di) / ai;
            p1.y = mR * (pos.y * mR - pos.x * di) / ai;
            p1.translate(mCp);
            
            p2.x = mR * (pos.x * mR - pos.y * di) / ai;
            p2.y = mR * (pos.y * mR + pos.x * di) / ai;
            p2.translate(mCp);
#endif
            List<PointD> plist = new List<PointD>() { p1, p2 };
            return plist;
        }

        /// <summary>
        /// 円と円との接線リスト
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>線分リスト</returns>
        public List<LineD> tangentArc(ArcD arc)
        {
            List<LineD> llist = new List<LineD>();
            ArcD a = arc.toCopy();
            a.translate(mCp.inverse());
            double rotate = a.mCp.angle();
            a.mCp.rotate(-rotate);
            double l = mCp.length(arc.mCp);
            if (l == 0)
                return llist;
            double xs = mR * (mR - a.mR) / l;
            double ys = Math.Sqrt(mR * mR - xs * xs);
            double xe = l + a.mR * (mR - a.mR) / l;
            double ye = Math.Sqrt(a.mR * a.mR - (xe - l) * (xe - l));
            LineD line = new LineD(xs, ys, xe, ye);
            LineD line2 = line.toCopy();
            line.rotate(rotate);
            line.translate(mCp);
            llist.Add(line);
            line2.mirror(new PointD(0, 0), a.mCp);
            line2.rotate(rotate);
            line2.translate(mCp);
            llist.Add(line2);
            if (l < mR + a.mR)
                return llist;
            xs = mR * (mR + a.mR) / l;
            ys = Math.Sqrt(mR * mR - xs * xs);
            xe = l - a.mR * (mR + a.mR) / l;
            ye = -Math.Sqrt(a.mR * a.mR - (xe - l) * (xe - l));
            line = new LineD(xs, ys, xe, ye);
            line2 = line.toCopy();
            line.rotate(rotate);
            line.translate(mCp);
            llist.Add(line);
            line2.mirror(new PointD(0, 0), a.mCp);
            line2.rotate(rotate);
            line2.translate(mCp);
            llist.Add(line2);

            return llist;
        }

        /// <summary>
        /// 3線分に接する円リスト
        /// </summary>
        /// <param name="lf">線分</param>
        /// <param name="ls">線分</param>
        /// <param name="lt">線分</param>
        /// <returns>円リスト</returns>
        public List<ArcD> tangentCircle(LineD la, LineD lb, LineD lc)
        {
            if (la.isParalell(lb) || lb.isParalell(lc) || lc.isParalell(la))
                return tangentCircleParalell(la, lb, lc);
            //  三角形の頂点
            PointD ap = lb.intersection(lc);
            PointD bp = lc.intersection(la);
            PointD cp = la.intersection(lb);
            //  三角形の辺の長さ
            double a = bp.length(cp);
            double b = cp.length(ap);
            double c = ap.length(bp);
            //  三角形の面積
            double s = (a + b + c) / 2;
            double A = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

            List<ArcD> arcs = new List<ArcD>();
            //  内接円
            double r = 2 * A / (a + b + c);
            PointD ctr = tangentCircleCenter(r, la, ap, lb, bp);
            arcs.Add(new ArcD(ctr, r));

            //  頂点apに対する傍接円
            r = 2 * A / (b + c - a);
            ctr = tangentCircleCenter(r, lb, bp, lc, cp);
            arcs.Add(new ArcD(ctr, r));

            //  頂点bpに対する傍接円
            r = 2 * A / (c + a - b);
            ctr = tangentCircleCenter(r, lc, cp, la, ap);
            arcs.Add(new ArcD(ctr, r));

            //  頂点cpに対する傍接円
            r = 2 * A / (a + b - c);
            ctr = tangentCircleCenter(r, la, ap, lb, bp);
            arcs.Add(new ArcD(ctr, r));

            return arcs;
        }

        /// <summary>
        /// 半径と2辺から接円の中心点を求める
        /// </summary>
        /// <param name="r">半径</param>
        /// <param name="la">線分</param>
        /// <param name="ap">laの対頂点</param>
        /// <param name="lb">線分</param>
        /// <param name="bp">lbの対頂点</param>
        /// <returns>中心点</returns>
        private PointD tangentCircleCenter(double r, LineD la, PointD ap, LineD lb, PointD bp)
        {
            LineD laa = la.toCopy();
            laa.offset(r, ap);
            LineD lbb = lb.toCopy();
            lbb.offset(r, bp);
            return laa.intersection(lbb);
        }

        /// <summary>
        /// 平行な線分を含む3線分から接円リストを求める
        /// </summary>
        /// <param name="la">線分</param>
        /// <param name="lb">線分</param>
        /// <param name="lc">線分</param>
        /// <returns>円リスト</returns>
        private List<ArcD> tangentCircleParalell(LineD la, LineD lb, LineD lc)
        {
            if (la.isParalell(lb) && lb.isParalell(lc))
                return null;
            List<ArcD> arcs = new List<ArcD>();
            double r;
            PointD ctr;
            if (la.isParalell(lb)) {
                r = la.distance(lb) / 2;
                ctr = tangentCircleParalellCenter(r, la, lb.ps, lc);
                arcs.Add(new ArcD(ctr, r));
                ctr = tangentCircleParalellCenter(-r, la, lb.ps, lc);
                arcs.Add(new ArcD(ctr, r));
            } else if (lb.isParalell(lc)) {
                r = lb.distance(lc) / 2;
                ctr = tangentCircleParalellCenter(r, lb, lc.ps, la);
                arcs.Add(new ArcD(ctr, r));
                ctr = tangentCircleParalellCenter(-r, lb, lc.ps, la);
                arcs.Add(new ArcD(ctr, r));
            } else if (lc.isParalell(la)) {
                r = lc.distance(la) / 2;
                ctr = tangentCircleParalellCenter(r, lc, la.ps, lb);
                arcs.Add(new ArcD(ctr, r));
                ctr = tangentCircleParalellCenter(-r, lc, la.ps, lb);
                arcs.Add(new ArcD(ctr, r));
            }
            return arcs;
        }

        /// <summary>
        /// 平行な線分を含む3線分から接円の中心点を求める
        /// </summary>
        /// <param name="r">半径</param>
        /// <param name="la">平行線分</param>
        /// <param name="ps">もう一つの平行線分の端点</param>
        /// <param name="lc">平行でない線分</param>
        /// <returns>中心点</returns>
        private PointD tangentCircleParalellCenter(double r, LineD la, PointD ps, LineD lc)
        {
            LineD laa = la.toCopy();
            laa.offset(Math.Abs(r), ps);
            LineD lcc = lc.toCopy();
            lcc.offset(r);
            return laa.intersection(lcc);
        }

        /// <summary>
        /// 2線分に接する円のリスト
        /// </summary>
        /// <param name="lf">線分</param>
        /// <param name="ls">線分</param>
        /// <param name="r">半径</param>
        /// <returns>円リスト</returns>
        public List<ArcD> tangentCircle(LineD lf, LineD ls, double r)
        {
            List<LineD> llf = new List<LineD>();
            llf.Add(lf.toCopy());
            llf[0].offset(r);
            llf.Add(lf.toCopy());
            llf[1].offset(-r);

            List<LineD> lls = new List<LineD>();
            lls.Add(ls.toCopy());
            lls[0].offset(r);
            lls.Add(ls.toCopy());
            lls[1].offset(-r);

            List<ArcD> arcs = new List<ArcD>();
            for (int i = 0; i < llf.Count; i++) {
                for (int j = 0; j < lls.Count; j++) {
                    PointD cp = llf[i].intersection(lls[j]);
                    if (cp != null && 0 < r) {
                        ArcD arc = new ArcD(cp, r);
                        arcs.Add(arc);
                    }
                }
            }
            return arcs;
        }

        /// <summary>
        /// 線分と円に接する円リスト
        /// </summary>
        /// <param name="lf">線分</param>
        /// <param name="af">円</param>
        /// <param name="r">半径</param>
        /// <returns>円リスト</returns>
        public List<ArcD> tangentCircle(LineD lf, ArcD af, double r)
        {
            List<LineD> llf = new List<LineD>();
            llf.Add(lf.toCopy());
            llf[0].offset(r);
            llf.Add(lf.toCopy());
            llf[1].offset(-r);

            List<ArcD> afl = new List<ArcD>();
            afl.Add(new ArcD(af.mCp, af.mR + r));
            if (0 < af.mR - r)
                afl.Add(new ArcD(af.mCp, af.mR - r));

            List<ArcD> arcs = new List<ArcD>();
            ArcD arc = new ArcD(af.mCp, af.mR + r);
            for (int i = 0; i < llf.Count; i++) {
                for (int j = 0; j < afl.Count; j++) {
                    List<PointD> cplist = afl[j].intersection(llf[i]);
                    for (int k = 0; k < cplist.Count; k++) {
                        arcs.Add(new ArcD(cplist[k], r));
                    }
                }
            }
            return arcs;
        }

        /// <summary>
        /// 2円に接する円リスト
        /// </summary>
        /// <param name="a1">円1</param>
        /// <param name="a2">円2</param>
        /// <param name="r">半径</param>
        /// <returns>円リスト</returns>
        public List<ArcD> tangentCircle(ArcD a1, ArcD a2, double r)
        {
            List<ArcD> a1l = new List<ArcD>();
            a1l.Add(new ArcD(a1.mCp, a1.mR + r));
            if (0 < a1.mR - r)
                a1l.Add(new ArcD(a1.mCp, a1.mR - r));
            List<ArcD> a2l = new List<ArcD>();
            a2l.Add(new ArcD(a2.mCp, a2.mR + r));
            if (0 < a2.mR - r)
                a2l.Add(new ArcD(a2.mCp, a2.mR - r));
            List<ArcD> arcs = new List<ArcD>();
            for (int i = 0; i < a1l.Count; i++) {
                for (int j = 0; j < a2l.Count; j++) {
                    List<PointD> cplist = a1l[i].intersection(a2l[j]);
                    for (int k = 0; k < cplist.Count; k++) {
                        arcs.Add(new ArcD(cplist[k], r));
                    }
                }
            }
            return arcs;
        }
    }
}
