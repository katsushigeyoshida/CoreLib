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
    /// void normalize()                                    開始角と修了角の正規化
    ///  string ToString()
    ///  string ToString(string format)                     書式付き文字列変換
    ///  ArcD toCopy()                                      コピーを作成
    ///  void rotate(double angle)                          円弧の回転
    ///  void rotate(PointD cp, double ang)                 指定点を中心に回転
    ///  PointD startPoint()                                始点座標
    ///  PointD endPoint()                                  終点座標
    ///  PointD getPoint(double ang)                        角度から円周上の座標を求める
    ///  double getAngle(PointD p)                          指定座標と中心との角度を求める
    ///  List<PointD> toPeakList()                          円周上の端点リスト(端点+4分割点)
    ///  List<PointD> toPointlist(int divideNo)             円弧の分割点リストを作成
    ///  List<PointD> toAnglePointList(double da, bool clockwise = false)   円弧の角度分割点リストを作成
    ///  PointD nearPoints(PointD p, int divideNo = 4)      円弧の分割点で最も近い点を求める
    ///  bool onPoint(PointD p)                             点が円弧上にあるかの判定
    ///  bool innerAngle(double ang)                        指定の角度が円弧内かの判定
    ///  List<PointD> interSection(PointD p, bool on = true)    点との交点リスト
    ///  PointD intersection(PointD p)                      垂点(円に対して)
    ///  List<PointD> intersection(LineD line, bool on = true)  線分との交点
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
        /// 開始角と修了角の正規化
        /// 0 <= 開始角 < 2π, 開始角 <= 修了角 < 4π
        /// </summary>
        public void normalize()
        {
            mSa = ylib.mod(mSa, Math.PI * 2);
            mEa = ylib.mod(mEa, Math.PI * 2);
            if (mEa <= mSa)
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
        /// <param name="divideNo"></param>
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
        public List<PointD> toAnglePointList(double da, bool clockwise = false)
        {
            List<PointD> pointList = new List<PointD>();
            double ang = mSa;
            while (ang < mEa - mEps) {
                pointList.Add(getPoint(ang, clockwise));
                ang += da;
            }
            pointList.Add(getPoint(mEa, clockwise));
            return pointList;
        }

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
        public List<PointD> interSection(PointD p, bool on = true)
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

            if (mR < l) {
                //  交点なし
            } else if (Math.Abs(mR - l) < mEps) {
                //  接点
                if (!on || innerAngle(mp.angle(mCp)))
                    plist.Add(mp);
            } else if (l == 0) {
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

    }
}
