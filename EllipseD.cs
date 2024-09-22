using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib
{
    /// <summary>
    /// EllipseD()                                              コンストラクタ
    /// EllipseD(PointD cp, double rx, double ry, double sa = 0, double ea = Math.PI * 2, double rotate = 0)
    /// EllipseD(PointD sp, PointD ep)
    /// EllipseD(EllipseD ellipse)
    /// 
    /// EllipseD toCopy()                                       コピーを作成
    /// void normalize()                                        開始角と修了角の正規化
    /// override string ToString()
    /// string ToString(string form)                            データを文字列化
    /// void translate(PointD vec)                              ベクトル分移動させる
    /// void rotate(double angle)                               楕円の回転
    /// void rotate(PointD cp, double ang)                      指定点を中心に回転
    /// void rotate(PointD cp, PointD mp)                       指定点を中心に回転
    /// void mirror(PointD sp, PointD ep)                       指定点でミラーする
    /// void scale(PointD cp, double scale)                     原点を指定して拡大縮小
    /// void offset(PointD sp, PointD ep)                       楕円の半径をオフセットする
    /// void offset(double dis)                                 楕円の半径をオフセットする
    /// void trim(PointD sp, PointD ep)                         指定点でトリムする
    /// void stretch(PointD vec, PointD pos)                    円を端点または半径をストレッチする
    /// Box getBox()                                            Rotateを0とした楕円のBox
    /// Box getArea()                                           楕円の領域
    /// PointD startPoint()                                     始点座標
    /// PointD endPoint()                                       終点座標
    /// PointD middlePoint()                                    中間点
    /// PointD getPoint(double ang)                             指定角度の座標
    /// double getAngle(PointD p)                               指定座標の垂点の楕円上の角度を求める
    /// List<PointD> toPeakList()                               円周上の端点リスト(端点+4分割点)
    /// bool onPoint(PointD p)                                  指定点が楕円上の点かの判定
    /// List<PointD> dividePoints(int divNo)                    楕円を分割した座標点リスト
    /// List<PointD> toPoint3D(int divideNo)                  楕円の分割点リストを作成
    /// PointD nearPoints(PointD p, int divideNo = 4)           円弧の分割点で最も近い点を求める
    /// bool insideChk(PointD p)                                点が楕円の内側かの判定
    /// bool insideChk(LineD line)                              線分が楕円の内側かの判定
    /// bool insideChk(Box b)                                   線分が楕円の内側かの判定
    /// PointD intersection(PointD pos)                         点との交点(垂点)
    /// List<PointD> dropPoint(PointD pos)                      点との交点(垂点)
    /// List<PointD> intersection(LineD line, bool on = true)   線分との交点
    /// List<PointD> intersection(ArcD arc)                     楕円と円の交点を求める
    /// List<PointD> intersection(EllipseD ellipse)             楕円同士の交点
    /// List<PointD> intersection(PolylineD polyline, bool on = true)   ポリラインとの交点を求める
    /// List<PointD> intersection(Box b, bool on = true)        Boxとの交点を求める
    /// List<LineD> tangentLine(PointD pos)                     指定点を通る接線を求める
    /// List<LineD> tangentLine(LineD line)                     線分と同じ傾きの接線を求める
    /// List<PointD> tangentPoint(PointD pos)                   指定点を通る接線の接点座標
    /// List<PointD> tangentPoint(LineD line)                   線分と同じ傾きの接線の接点を求める
    /// 
    /// List<double> canonical2Implicit()                       陰関数の形式に変換する
    /// 
    /// </summary>

    public class EllipseD
    {
        public PointD mCp = new PointD();
        public double mRx = 1;
        public double mRy = 1;
        public double mSa = 0;
        public double mEa = Math.PI * 2;
        public double mRotate = 0;
        public double mOpenAngle { get { return mEa - mSa; } }
        private double mEps = 1E-6;

        private YLib ylib = new YLib();

        public EllipseD()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="rx">X方向半径</param>
        /// <param name="ry">y方向半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <param name="rotate">回転角(rad)</param>
        public EllipseD(PointD cp, double rx, double ry, double sa = 0, double ea = Math.PI * 2, double rotate = 0)
        {
            mCp = cp;
            mRx = rx;
            mRy = ry;
            mSa = sa;
            mEa = ea;
            mRotate = rotate;
            normalize();
        }

        /// <summary>
        /// コンストラクタ(四角領域で指定)
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public EllipseD(PointD sp, PointD ep)
        {
            Box b = new Box(sp, ep);
            mCp = b.getCenter();
            mRx = b.Width / 2;
            mRy = b.Height / 2;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ellipse">EllipseD</param>
        public EllipseD(EllipseD ellipse)
        {
            mCp = ellipse.mCp.toCopy();
            mRx = ellipse.mRx;
            mRy = ellipse.mRy;
            mSa = ellipse.mSa;
            mEa = ellipse.mEa;
            mRotate = ellipse.mRotate;
            normalize();
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns></returns>
        public EllipseD toCopy()
        {
            EllipseD ellipse = new EllipseD();
            ellipse.mCp = mCp.toCopy();
            ellipse.mRx = mRx;
            ellipse.mRy = mRy;
            ellipse.mSa = mSa;
            ellipse.mEa = mEa;
            ellipse.mRotate = mRotate;
            return ellipse;
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
            mRotate = ylib.mod(mRotate, Math.PI * 2);
        }

        /// <summary>
        /// データを文字列化
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return $"{mCp},{mRx},{mRy},{mSa},{mEa},{mRotate}";
        }

        /// <summary>
        /// データを文字列化
        /// </summary>
        /// <param name="form">数値の書式</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            return $"{mCp.ToString(form)},{mRx.ToString(form)},{mRy.ToString(form)},{mSa.ToString(form)},{mEa.ToString(form)},{mRotate.ToString(form)}";
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
        /// 楕円の回転
        /// </summary>
        /// <param name="angle">回転角(rad)</param>
        public void rotate(double angle)
        {
            mRotate += angle;
            mRotate = mRotate % (Math.PI * 2);
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
        /// 指定点でミラーする
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        public void mirror(PointD sp, PointD ep)
        {
            mCp.mirror(sp, ep);
            double sa = Math.PI - mSa;
            double ea = Math.PI - mEa;
            mSa = ea;
            mEa = sa;
            mRotate = ep.angle(sp) * 2 - mRotate + Math.PI;
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
            mRx *= scale;
            mRy *= scale;
        }

        /// <summary>
        /// 楕円の半径をオフセットする
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(PointD sp, PointD ep)
        {
            double dis = mCp.length(ep) - mCp.length(sp);
            mRx += dis;
            mRy += dis;
        }

        /// <summary>
        /// 楕円の半径をオフセットする
        /// </summary>
        /// <param name="dis">オフセット距離</param>
        public void offset(double dis)
        {
            mRx += dis;
            mRy += dis;
        }

        /// <summary>
        /// 指定点でトリムする
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void trim(PointD sp, PointD ep)
        {
            List<PointD> plist = dropPoint(sp);
            if (0 < plist.Count)
                mSa = getAngle(plist.MinBy(p => p.length(sp)));
            plist = dropPoint(ep);
            if (0 < plist.Count)
                mEa = getAngle(plist.MinBy(p => p.length(ep)));
            normalize();
        }

        /// <summary>
        /// 楕円を端点または半径をストレッチする
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
                //  オフセット
                mp += vec;
                PointD ip = intersection(mp);
                offset(mp.length(mCp) - ip.length(mCp));
            } else if (sl < el) {
                //  端点移動
                sp += vec;
                List<PointD> plist = dropPoint(sp);
                mSa = getAngle(plist.MinBy(p => p.length(sp)));
                normalize();
            } else {
                //  端点移動
                ep += vec;
                List<PointD> plist = dropPoint(ep);
                mEa = getAngle(plist.MinBy(p => p.length(ep)));
                normalize();
            }
        }

        /// <summary>
        /// Rotateを0とした楕円のBox
        /// </summary>
        /// <returns></returns>
        public Box getBox()
        {
            PointD sp = new PointD(mCp.x - mRx, mCp.y - mRy);
            PointD ep = new PointD(mCp.x + mRx, mCp.y + mRy);
            return new Box(sp, ep);
        }

        /// <summary>
        /// 楕円の領域
        /// </summary>
        /// <returns></returns>
        public Box getArea()
        {
            List<PointD> plist = new List<PointD>();
            LineD hLine = new LineD(new PointD(0, 0), new PointD(1, 0));
            LineD vLine = new LineD(new PointD(0, 0), new PointD(0, 1));
            plist.AddRange(tangentPoint(hLine));
            plist.AddRange(tangentPoint(vLine));
            Box b = new Box(getPoint(mSa));
            for (int i = 0; i < plist.Count; i++)
                if (onPoint(plist[i]))
                    b.extension(plist[i]);
            b.extension(getPoint(mEa));
            return b;
        }

        /// <summary>
        /// 始点座標
        /// </summary>
        /// <returns>座標</returns>
        public PointD startPoint()
        {
            return getPoint(mSa);
        }

        /// <summary>
        /// 終点座標
        /// </summary>
        /// <returns>座標</returns>
        public PointD endPoint()
        {
            return getPoint(mEa);
        }

        /// <summary>
        /// 中間点
        /// </summary>
        /// <returns>座標</returns>
        public PointD middlePoint()
        {
            return getPoint((mEa + mSa) / 2);
        }

        /// <summary>
        /// 指定角度の座標
        /// </summary>
        /// <param name="ang">角度</param>
        /// <returns>座標</returns>
        public PointD getPoint(double ang)
        {
            PointD pos = new PointD();
            pos.x = mRx * Math.Cos(ang);
            pos.y = mRy * Math.Sin(ang);
            pos.rotate(mRotate);
            pos.translate(mCp);
            return pos;
        }

        /// <summary>
        /// 指定座標の垂点の楕円上の角度を求める
        /// </summary>
        /// <param name="pos">点座標</param>
        /// <returns>角度</returns>
        public double getAngle(PointD p)
        {
            PointD pos = p.toCopy();
            pos.translate(mCp.inverse());
            pos.rotate(-mRotate);
            return Math.Atan2(pos.y * mRx / mRy, pos.x);
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
        /// 指定点が楕円上の点かの判定
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>判定</returns>
        public bool onPoint(PointD p)
        {
            double ang = getAngle(p);
            PointD op = getPoint(ang);
            if (mEps < op.length(p))
                return false;
            ang += ang < 0 ? Math.PI * 2 : 0;
            ang += ang < mSa ? Math.PI * 2 : 0;
            if (ang <= mEa)
                return true;
            return false;
        }

        /// <summary>
        /// 楕円を分割した座標点リスト
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <returns>座標点リスト</returns>
        public List<PointD> dividePoints(int divNo)
        {
            List<PointD> points = new List<PointD>();
            if (0 < divNo) {
                double da = mOpenAngle / divNo;
                double ang = mSa;
                while (ang <= mEa + mEps) {
                    points.Add(getPoint(ang));
                    ang += da;
                }
            }
            return points;
        }

        /// <summary>
        /// 楕円の分割点リストを作成
        /// </summary>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標リスト</returns>
        public List<PointD> toPointList(int divideNo)
        {
            List<PointD> pointList = new List<PointD>();
            double ang = mSa;
            double da = (mEa - mSa) / divideNo;
            while (ang < mEa - mEps) {
                pointList.Add(getPoint(ang));
                ang += da;
            }
            pointList.Add(endPoint());
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
        /// 点が楕円の内側かの判定
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>判定</returns>
        public bool insideChk(PointD p)
        {
            PointD pos = p.toCopy();
            pos.translate(mCp.inverse());
            pos.rotate(-mRotate);
            pos.y *= mRx / mRy;
            if (mRx < pos.length())
                return false;
            Box b = getArea();
            if (b.insideChk(p))
                return true;
            return false;
        }

        /// <summary>
        /// 線分が楕円の内側かの判定
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>判定</returns>
        public bool insideChk(LineD line)
        {
            if (insideChk(line.ps) && insideChk(line.pe))
                return true;
            return false;
        }

        /// <summary>
        /// Boxが楕円の内側かの判定
        /// </summary>
        /// <param name="b">Box</param>
        /// <returns>判定</returns>
        public bool insideChk(Box b)
        {
            List<PointD> plist = b.ToPointList();
            for (int i = 0; i < plist.Count; i++)
                if (!insideChk(plist[i]))
                return false;
            return true;
        }

        /// <summary>
        /// 点との交点(垂点)
        /// </summary>
        /// <param name="pos">点座標</param>
        /// <returns>楕円上の座標</returns>
        public PointD intersection(PointD pos)
        {
            List<PointD> plist = dropPoint(pos);
            double dis = double.MaxValue;
            PointD mp = null;
            for (int i = 0; i < plist.Count; i++) {
                if (onPoint(plist[i])) {
                    if (dis > pos.length(plist[i])) {
                        dis = pos.length(plist[i]);
                        mp = plist[i];
                    }
                }
            }
            return mp;
        }

        /// <summary>
        /// 点との交点(垂点)
        /// </summary>
        /// <param name="pos">点座標</param>
        /// <returns>座標点リスト</returns>
        public List<PointD> dropPoint(PointD pos)
        {
            List<PointD> plist = new List<PointD>();
            PointD p = pos.toCopy();
            p.translate(mCp.inverse());
            p.rotate(-mRotate);

            //  4次方程式でエラーとなるケース
            if (p.x == 0) {
                PointD ip = new PointD(0, mRy);
                ip.rotate(mRotate);
                ip.translate(mCp);
                plist.Add(ip);
                ip = new PointD(0, -mRy);
                ip.rotate(mRotate);
                ip.translate(mCp);
                plist.Add(ip);
                return plist;
            } else if (p.y == 0) {
                PointD ip = new PointD(mRx, 0);
                ip.rotate(mRotate);
                ip.translate(mCp);
                plist.Add(ip);
                ip = new PointD(-mRx, 0);
                ip.rotate(mRotate);
                ip.translate(mCp);
                plist.Add(ip);
                return plist;
            }
            //  方程式から垂点を求める
            double wa = mRx * mRx;
            double wb = mRy * mRy;
            double k1 = wa - wb;
            double k2 = wb * p.y;
            double k3 = wa * p.x;
            double a = wa * k1 * k1;
            double b = 2 * wa * k1 * k2;
            double c = wa * k2 * k2 + wb * k3 * k3 - wa * wb * k1 * k1;
            double d = -2 * wa * wb * k1 * k2;
            double e = -wa * wb * k2 * k2;
            List<double> ylist = ylib.solveQuarticEquation(a, b, c, d, e);
            for (int i = 0; i < ylist.Count; i++) {
                PointD ip = new PointD();
                if (mEps < Math.Abs((k1 * ylist[i] + k2))) {
                    ip.x = (k3 * ylist[i]) / (k1 * ylist[i] + k2);
                    ip.y = ylist[i];
                    ip.rotate(mRotate);
                    ip.translate(mCp);
                    plist.Add(ip);
                } else {
                    double dx = (wa * wb + wa * ylist[i] * ylist[i]) / wb;
                    if (0 <= dx) {
                        ip.x = Math.Sqrt(dx);
                        ip.y = ylist[i];
                        ip.rotate(mRotate);
                        ip.translate(mCp);
                        plist.Add(ip);
                    }
                }
            }

            return plist;
        }

        /// <summary>
        /// 線分との交点
        /// </summary>
        /// <param name="l">線分</param>
        /// <param name="on">判定あり</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(LineD line, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD p = new PointD();

            LineD l = line.toCopy();
            l.translate(mCp.inverse());
            l.rotate(-mRotate);
            if (Math.Abs(l.pe.x - l.ps.x) < mEps) {
                //  垂線の場合
                double d = (l.ps.x * l.ps.x) / (mRx * mRx);
                if (1 < d)
                    return plist;
                p.x = l.ps.x;
                p.y = Math.Sqrt((1 - d) * (mRy * mRy));
                p.rotate(mRotate);
                p.translate(mCp);
                if (!on || (onPoint(p) && line.onPoint(p)))
                    plist.Add(p);
                p = new PointD();
                p.x = l.ps.x;
                p.y = -Math.Sqrt((1 - d) * (mRy * mRy));
                if (p.y != 0) {
                    p.rotate(mRotate);
                    p.translate(mCp);
                    if (!on || (onPoint(p) && line.onPoint(p)))
                        plist.Add(p);
                }
            } else {
                double m = (l.pe.y - l.ps.y) / (l.pe.x - l.ps.x);
                double n = l.ps.y - l.ps.x * m;
                double a = 1 / (mRx * mRx) + (m * m) / (mRy * mRy);
                double b = (2 * m * n) / (mRy * mRy);
                double c = (n * n) / (mRy * mRy) - 1;
                if (a != 0) {
                    double d = b * b - 4 * a * c;
                    if (d < mEps) {
                        //  接点
                        p.x = -b / (2 * a);
                        p.y = m * p.x + n;
                        p.rotate(mRotate);
                        p.translate(mCp);
                        if (!on || (onPoint(p) && line.onPoint(p)))
                            plist.Add(p);
                    } else if (mEps <= d) {
                        //  交点
                        p.x = (-b + Math.Sqrt(d)) / (2 * a);
                        p.y = m * p.x + n;
                        p.rotate(mRotate);
                        p.translate(mCp);
                        if (!on || (onPoint(p) && line.onPoint(p)))
                            plist.Add(p);
                        p = new PointD();
                        p.x = (-b - Math.Sqrt(d)) / (2 * a);
                        p.y = m * p.x + n;
                        p.rotate(mRotate);
                        p.translate(mCp);
                        if (!on || (onPoint(p) && line.onPoint(p)))
                            plist.Add(p);
                    }
                }
            }
            return plist;
        }

        /// <summary>
        /// 楕円と円の交点を求める
        /// </summary>
        /// <param name="arc">円</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(ArcD arc)
        {
            List<PointD> plist = new List<PointD>();

            ArcD arc2 = arc.toCopy();
            arc2.translate(mCp.inverse());
            arc2.rotate(-mRotate);
            double w1 = mRx * mRx / (mRy * mRy);
            double w2 = mRx * mRx;
            double k1 = 0.5 * (1 - w1);
            double k2 = -arc2.mCp.y;
            double k3 = 0.5 * (arc2.mCp.x * arc2.mCp.x + arc2.mCp.y * arc2.mCp.y - arc2.mR * arc2.mR + w2);

            if (Math.Abs(arc2.mCp.x) < mEps) {
                //  k1 == 0 の時は2次方程式の解
                List<double> ylist = ylib.solveQuadraticEquation(k1, k2, k3);
                for (int i = 0; i < ylist.Count; i++) {
                    double wk = w2 - w1 * ylist[i] * ylist[i];
                    if (wk < 0)
                        continue;
                    PointD p = new PointD(Math.Sqrt(wk), ylist[i]);
                    p.rotate(mRotate);
                    p.translate(mCp);
                    plist.Add(p);
                    p = new PointD(-Math.Sqrt(wk), ylist[i]);
                    p.rotate(mRotate);
                    p.translate(mCp);
                    if (plist.FindIndex(pc => pc.isEqual(p)) < 0)
                        plist.Add(p);
                }
            } else {
                double a = k1 * k1;
                double b = 2 * k1 * k2;
                double c = k2 * k2 + 2 * k1 * k3 + w1 * arc2.mCp.x * arc2.mCp.x;
                double d = 2 * k2 * k3;
                double e = k3 * k3 - arc2.mCp.x * arc2.mCp.x * w2;
                List<double> ylist = ylib.solveQuarticEquation(a, b, c, d, e);
                for (int i = 0; i < ylist.Count; i++) {
                    double x = (k1 * ylist[i] * ylist[i] + k2 * ylist[i] + k3) / arc2.mCp.x;
                    double y = ylist[i];
                    PointD p = new PointD(x, y);
                    p.rotate(mRotate);
                    if (plist.FindIndex(pc => pc.isEqual(p)) < 0)
                        p.translate(mCp);
                    plist.Add(p);
                }
            }

            return plist;
        }

        /// <summary>
        /// 楕円同士の交点
        /// </summary>
        /// <param name="ellipse">楕円</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(EllipseD ellipse)
        {
            List<PointD> plist = new List<PointD>();
            EllipseD elli = ellipse.toCopy();
            elli.translate(mCp.inverse());
            elli.mCp.rotate(-mRotate);
            elli.rotate(-mRotate);
            List<double> impli = elli.canonical2Implicit();
            double a1 = 1 / (mRx * mRx);
            double a2 = 1 / (mRy * mRy);
            double a3 = -1;
            double b1 = impli[0];
            double b2 = impli[1];
            double b3 = impli[2];
            double b4 = impli[3];
            double b5 = impli[4];
            double b6 = impli[5];
            double k = b1 / a1;
            double c1 = b1;
            double c2 = a2 * k;
            double c3 = a3 * k;
            double d1 = c2 - b2;
            double d2 = -b5;
            double d3 = c3 - b6;
            double g1 = b3;
            double g2 = b4;

            double a = a1 * d1 * d1 + a2 * g1 * g1;
            double b = 2 * (a1 * d1 * d2 + a2 * g1 * g2);
            double c = a1 * d2 * d2 + 2 * a1 * d1 * d3 + a2 * g2 * g2 + a3 * g1 * g1;
            double d = 2 * (a1 * d2 * d3 + a3 * g1 * g2);
            double e = a1 * d3 * d3 + a3 * g2 * g2;

            List<double> ylist = ylib.solveQuarticEquation(a, b, c, d, e);
            for (int i = 0; i < ylist.Count; i++) {
                double x = (d1 * ylist[i] * ylist[i] + d2 * ylist[i] + d3) / (g1 * ylist[i] + g2);
                double y = ylist[i];
                PointD p = new PointD(x, y);
                p.rotate(mRotate);
                elli.mCp.rotate(mRotate);
                if (plist.FindIndex(pc => pc.isEqual(p)) < 0)
                    p.translate(mCp);
                plist.Add(p);
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
        /// Boxとの交点を求める
        /// </summary>
        /// <param name="b">Box</param>
        /// <param name="on">線上の有無</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(Box b, bool on = true)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip;
            List<LineD> lines = b.ToLineList();
            foreach (var line in lines) {
                plist.AddRange(intersection(line, on));
            }
            return plist;
        }

        /// <summary>
        /// 指定点を通る接線を求める
        /// </summary>
        /// <param name="pos">指定点座標</param>
        /// <returns>接線リスト</returns>
        public List<LineD> tangentLine(PointD pos)
        {
            List<PointD> plist = tangentPoint(pos);
            List<LineD> llist = new List<LineD>();
            for (int i = 0; i < plist.Count; i++) {
                LineD l = new LineD(pos, plist[i]);
                llist.Add(l);
            }
            return llist;
        }

        /// <summary>
        /// 線分と同じ傾きの接線を求める
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>接線リスト</returns>
        public List<LineD> tangentLine(LineD line)
        {
            List<LineD> llist =  new List<LineD>();
            LineD l1, l2;
            LineD l = line.toCopy();
            l.rotate(-mRotate);
            if ((l.pe.x - l.ps.x) != 0) {
                //  垂直線以外
                double m = (l.pe.y - l.ps.y) / (l.pe.x - l.ps.x);
                double n = mRx * mRx * m * m + mRy * mRy;
                double c = Math.Sqrt(n);
                l1 = new LineD(new PointD(-mRx, (-mRx * m) - c), new PointD(mRx, (mRx * m) - c));
                l2 = new LineD(new PointD(-mRx, (-mRx * m) + c), new PointD(mRx, (mRx * m) + c));
            } else {
                //  垂直線
                l1 = new LineD(new PointD(mRx, mRy), new PointD(mRx, -mRy));
                l2 = new LineD(new PointD(-mRx, mRy), new PointD(-mRx, -mRy));
            }
            l1.rotate(mRotate);
            l1.translate(mCp);
            l1.ps = l1.intersection(line.ps);
            l1.pe = l1.intersection(line.pe);
            llist.Add(l1);
            l2.rotate(mRotate);
            l2.translate(mCp);
            l2.ps = l2.intersection(line.ps);
            l2.pe = l2.intersection(line.pe);
            llist.Add(l2);
            return llist;
        }

        /// <summary>
        /// 指定点を通る接線の接点座標
        /// </summary>
        /// <param name="pos">指定点座標</param>
        /// <returns>接点リスト</returns>
        public List<PointD> tangentPoint(PointD pos)
        {
            PointD p = pos.toCopy();
            p.translate(mCp.inverse());
            p.rotate(-mRotate);
            p.y *= mRx / mRy;
            ArcD arc = new ArcD(new PointD(0, 0), mRx);
            List<PointD> plist = arc.tangentPoint(p);
            for (int i = 0; i < plist.Count; i++) {
                plist[i].y *= mRy / mRx;
                plist[i].rotate(mRotate);
                plist[i].translate(mCp);
            }
            return plist;
        }

        /// <summary>
        /// 線分と同じ傾きの接線の接点を求める
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>接点リスト</returns>
        public List<PointD> tangentPoint(LineD line)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = tangentLine(line);
            for (int i = 0; i < llist.Count; i++) {
                List<PointD> iplist = intersection(llist[i], false);
                if (0 < iplist.Count)
                    plist.Add(iplist[0]);
            }
            return plist;
        }

        /// <summary>
        /// 陰関数の形式に変換する
        /// a x2 + b y2 + c x y + d x + e y + f = 0
        /// </summary>
        /// <returns>陰関数の係数リスト</returns>
        public List<double> canonical2Implicit()
        {
            double cos2 = Math.Cos(mRotate) * Math.Cos(mRotate);
            double sin2 = Math.Sin(mRotate) * Math.Sin(mRotate);
            double cossin = Math.Cos(mRotate) * Math.Sin(mRotate);
            double a2 = mRx * mRx;
            double b2 = mRy * mRy;

            double a = cos2 / a2 + sin2 / b2;
            double b = sin2 / a2 + cos2 / b2;
            double c = 2 * cossin * (1 / a2 - 1 / b2);
            double d = -2 * ((mCp.x * cos2 + mCp.y * cossin) / a2 + (mCp.x * sin2 - mCp.y * cossin) / b2);
            double e = -2 * ((mCp.x * cossin + mCp.y * sin2) / a2 - (mCp.x * cossin - mCp.y * cos2) / b2);
            double f = (mCp.x * mCp.x * cos2 + 2 * mCp.x * mCp.y * cossin + mCp.y * mCp.y * sin2) / a2
                + (mCp.x * mCp.x * sin2 - 2 * mCp.x * mCp.y * cossin + mCp.y * mCp.y * cos2) / b2 - 1;
            List<double> impli = new List<double>() { a, b, c, d, e, f };
            return impli;
        }
    }
}
