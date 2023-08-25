using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace CoreLib
{
    /// <summary>
    /// 実数(double)のLineクラス
    /// 
    ///  LineD()
    ///  LineD(double psx, double psy, double pex, double pey)
    ///  LineD(Point ps, Point pe)
    ///  LineD(PointD ps, PointD pe)
    ///  LineD(Line l)
    ///  LineD(LineD l)
    ///  LineD(PointD ps, double r, double th)
    ///  
    ///  override string ToString()
    ///  string ToString(string form)           データを書式付き文字列に変換
    ///  LineD toCopy()                         コピーを作成
    ///  Line toLine()                          Lineデータ(Winodws.Shapes)に変換
    ///  List<PointD> toPointList()             PointDのリストに変換
    ///  Rect toRect()                          Rectに変換
    ///  Box toBox()                            Boxに変換
    ///  void setNaN()                          不定値を設定(null)の代わり
    ///  bool isNaN()                           不定値の判定
    ///  
    ///  PointD vector()                        ベクトル(増分データ)に変換
    ///  PointD offset(PointD pos, double dis   指定座標をオフセットした座標を求める
    ///  PointD getVectorAngle(double ang, double l)    線分に対して角度と長さを指定したベクトルを求める
    ///  double length()                        線分の長さ
    ///  void setLength(double l)               線分の長さを再設定する
    ///  double distance(PointD p)              点と垂点との距離
    ///  double distance(LineD l)               線分との距離(平行でない場合は始点との距離)
    ///  double angle()                         ベクトルとしての角度(-π ～ π)
    ///  double angle(LineD l)                  2線分の角度(0 ～ π)
    ///  double angle2(LineD l)                 2線分の角度(0 ～ 2π)
    ///  bool isParalell(LineD l)               平行線の判定
    ///  bool innerAngle(double sa, double ea, double ang)  2角の間にあるかを判定(境界を含む)
    ///  double crossProduct(PointD p)          外積(線分に対しての左右の位置関係)
    ///  double pointDistance(PointD p)         点との垂線の距離
    ///  PointD intersection(PointD p)          点からの垂線の交点座標(垂点)
    ///  PointD intersection(LineD l)           2線分の交点(延長線上の交点も含む)
    ///  List<PointD> intersection(ArcD arc)    円との交点を求める
    ///  List<PointD> intersection(PointD c, double r, double sa = 0.0, double ea = Math.PI * 2.0)  円との交点を求める
    ///  bool intersectionHorizon(PointD p)     指定点の水平線と交点を持つかをチェック
    ///  PointD intersectHorizonPoint(PointD p) 指定点の水平線との交点(延長線上も含む)
    ///  PointD centerPoint()                   線分の中点を求める
    ///  bool onPoint(PointD pnt)               点が線分上にあるかを判定
    ///  PointD nearPoints(PointD p, int  divideNo = 4) 線分の分割点で最も近い点を求める
    ///  byte inOutAreaCode(PointD p, Rect rect)クリッピング領域に対する点の9分割位置の範囲
    ///  byte inOutAreaCode(Point p, Rect rect) クリッピング領域に対する点の9分割位置の範囲
    ///  LineD clippingLine(Rect rect)          矩形領域でクリッピングする
    ///  
    ///  void translate(PointD vec)             ベクトル分移動させる
    ///  void offset(double d)                  指定した距離分だけ平行移動させる
    ///  void offset(double dis, PointD pos)    指定点の方向に平行移動させる
    ///  void offset(PointD sp, PointD ep)      垂直方向に平行移動させる
    ///  void slide(double l)                   指定した距離分だけ同じ向きに平行移動させる
    ///  void move(double dx, double dy)        増分を指定して平行移動
    ///  void moveLength(double r,double th)    距離と角度を指定して平行移動
    ///  void moveLength(double l)              延長線上に距離を指定して移動
    ///  void rotate(Point ctr, double rotate)  中心を指定して線分を回転
    ///  void rotate(PointD ctr, double rotate) 中心を指定して線分を回転
    ///  void rotate(PointD cp, PointD mp)      指定点を中心に回転
    ///  void mirror(PointD sp, PointD ep)      指定線分でミラー
    ///  void trim(PointD sp, PointD ep)        指定点の垂点で線分の長さを変える
    ///  void trimNear(PointD tp, PointD pos)   ピックした位置に近い方を消すようにトリミングする
    ///  void trimFar(PointD tp, PointD pos)    ピックした位置に遠い方を消すようにトリミングする
    ///  void stretch(PointD vec, PointD pickPos)   指定地に近い端点を移動させる
    ///  List<LineD> divide(PointD p)           指定点で分割する
    ///  List<PointD> dividePoints(int divNo)   線分を分割する座標リスト
    ///  List<PointD> dividePattern(List<double> pattern)   指定したパターンに線分を分割し分割した座標リストを求めめる
    /// </summary>
    public class LineD
    {
        private YLib ylib = new YLib();

        public PointD ps = new();
        public PointD pe = new();
        private double mEps = 1E-8;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineD()
        {
            ps = new PointD();
            pe = new PointD();
            ps.setNaN();
            pe.setNaN();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="psx">始点X座標</param>
        /// <param name="psy">始点Y座標</param>
        /// <param name="pex">終点X座標</param>
        /// <param name="pey">終点Y座標</param>
        public LineD(double psx, double psy, double pex, double pey)
        {
            ps = new PointD(psx, psy);
            pe = new PointD(pex, pey);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public LineD(Point ps, Point pe)
        {
            this.ps = new PointD(ps.X, ps.Y);
            this.pe = new PointD(pe.X, pe.Y);
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public LineD(PointD ps, PointD pe)
        {
            this.ps = ps.toCopy();
            this.pe = pe.toCopy();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="l">Lineデータ(Winodws.Shapes)</param>
        public LineD(Line l)
        {
            ps.x = l.X1;
            ps.y = l.Y1;
            pe.x = l.X2;
            pe.y = l.Y2;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="l">Lineデータ(Winodws.Shapes)</param>
        public LineD(LineD l)
        {
            ps.x = l.ps.x;
            ps.y = l.ps.y;
            pe.x = l.pe.x;
            pe.y = l.pe.y;
        }

        /// <summary>
        /// コンストラクタ　開始点と長さと角度を指定する
        /// </summary>
        /// <param name="ps">開始点</param>
        /// <param name="r">長さ</param>
        /// <param name="th">角度(rad)</param>
        public LineD(PointD ps, double r, double th)
        {
            this.ps = ps.toCopy();
            pe.x = ps.x + r * Math.Cos(th);
            pe.y = ps.y + r * Math.Sin(th);
        }

        /// <summary>
        /// データを文字列に変換
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            return ps.ToString() + "," + pe.ToString();
        }

        /// <summary>
        /// データを書式付き文字列に変換
        /// </summary>
        /// <param name="form">書式</param>
        /// <returns>文字列</returns>
        public string ToString(string form)
        {
            return ps.ToString(form) + "," + pe.ToString(form);
        }

        /// <summary>
        /// コピーを作成
        /// </summary>
        /// <returns></returns>
        public LineD toCopy()
        {
            return new LineD(ps.x, ps.y, pe.x, pe.y);
        }

        /// <summary>
        /// Lineデータ(Winodws.Shapes)に変換
        /// </summary>
        /// <returns>Lineデータ(Winodws.Shapes)</returns>
        public Line toLine()
        {
            Line l = new Line();
            l.X1 = ps.x;
            l.Y1 = ps.y;
            l.X2 = pe.x;
            l.Y2 = pe.y;
            return l;
        }

        /// <summary>
        /// PointDのリストに変換
        /// </summary>
        /// <returns>PointDリスト</returns>
        public List<PointD> toPointList()
        {
            List<PointD> plist = new List<PointD>() { ps, pe };
            return plist;
        }

        /// <summary>
        /// Rectに変換
        /// </summary>
        /// <returns>Rectデータ</returns>
        public Rect toRect()
        {
            return new Rect(ps.toPoint(), pe.toPoint());
        }

        /// <summary>
        /// Boxに変換
        /// </summary>
        /// <returns>Box</returns>
        public Box toBox()
        {
            return new Box(ps, pe);
        }

        /// <summary>
        /// 不定値を設定(null)の代わり
        /// </summary>
        public void setNaN()
        {
            ps.setNaN();
            pe.setNaN();
        }

        /// <summary>
        /// 不定値の判定
        /// </summary>
        /// <returns></returns>
        public bool isNaN()
        {
            return ps.isNaN() || pe.isNaN();
        }

        /// <summary>
        /// ベクトル(増分データ)に変換
        /// </summary>
        /// <returns>増分データ</returns>
        public PointD vector()
        {
            return new PointD(pe.x - ps.x, pe.y - ps.y);
        }

        /// <summary>
        /// 指定座標を線分に対して平行移動した座標を求める
        /// </summary>
        /// <param name="pos">座標</param>
        /// <param name="dis">オフセット距離</param>
        /// <returns>オフセット座標</returns>
        public PointD offset(PointD pos, double dis)
        {
            PointD ip = intersection(pos);
            LineD line = new LineD(ip, pos);
            line.setLength(line.length() + dis);
            return line.pe;
        }

        /// <summary>
        /// 線分に対して角度と長さを指定したベクトルを求める
        /// </summary>
        /// <param name="ang">線分に対する角度(rad)</param>
        /// <param name="l">ベクトルの長さ</param>
        /// <returns>ベクトル</returns>
        public PointD getVectorAngle(double ang, double l)
        {
            PointD vec = vector();
            vec.rotate(ang);
            vec.setLength(l);
            return vec;
        }

        /// <summary>
        /// 線分の端点を求める(寸法線用)
        /// 参照点からピック位置と同じ方向でピック位置に近い端点
        /// </summary>
        /// <param name="pickPos">ピック位置</param>
        /// <param name="cp">参照点</param>
        /// <returns>端点座標</returns>
        public PointD getEndPointLine(PointD pickPos, PointD cp)
        {
            PointD ep;
            if (cp == null) {
                if (pickPos.length(ps) < pickPos.length(pe))
                    return ps;
                else
                    return pe;
            }
            LineD ls = new LineD(cp, ps);
            LineD lp = new LineD(cp, pickPos);
            LineD le = new LineD(cp, pe);
            if (ls.length() < mEps) {
                ep = pe;
            } else if (le.length() < mEps) {
                ep = ps;
            } else if (ls.angle2(lp) < Math.PI / 2 || Math.PI * 3 / 2 < ls.angle2(lp)) {
                ep = ps;
            } else {
                ep = pe;
            }
            return ep;
        }

        /// <summary>
        /// 始終点を入れ換える
        /// </summary>
        public void inverse()
        {
            PointD tp = ps;
            ps = pe;
            pe = tp;
        }

        /// <summary>
        /// 線分の長さ
        /// </summary>
        /// <returns>長さ</returns>
        public double length()
        {
            PointD v = vector();
            return Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// 線分の長さを再設定する
        /// </summary>
        /// <param name="l">線分の長さ</param>
        public void setLength(double l)
        {
            double th = angle();
            pe.x = ps.x + l * Math.Cos(th);
            pe.y = ps.y + l * Math.Sin(th);
        }

        /// <summary>
        /// 点との距離
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>距離</returns>
        public double distance(PointD p)
        {
            PointD ip = intersection(p);
            return ip.length(p);
        }

        /// <summary>
        /// 線分との距離(平行でない場合は始点との距離)
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns>距離</returns>
        public double distance(LineD l)
        {
            PointD ip = intersection(l.ps);
            return ip.length(l.ps);
        }

        /// <summary>
        /// ベクトルとしての角度(-π ～ π)
        /// </summary>
        /// <returns>回転角</returns>
        public double angle()
        {
            PointD v = vector();
            return Math.Atan2(v.y, v.x);
        }

        /// <summary>
        /// 2線分の角度(0 ～ π)
        /// </summary>
        /// <param name="l">対象線分</param>
        /// <returns>線分同士の角度</returns>
        public double angle(LineD l)
        {
            return Math.Abs(angle() - l.angle()) % Math.PI;
        }

        /// <summary>
        /// 2線分の角度(0 ～ 2π)
        /// </summary>
        /// <param name="l">対象線分</param>
        /// <returns>線分同士の角度</returns>
        public double angle2(LineD l)
        {
            double ang0 = angle();
            double ang1 = l.angle();
            if (ang1 < ang0)
                ang1 += Math.PI * 2;
            return ang1 - ang0;
        }

        /// <summary>
        /// 平行線の判定
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public bool isParalell(LineD l)
        {
            PointD v1 = vector();
            PointD v2 = l.vector();
            return Math.Abs(v1.x * v2.y - v2.x * v1.y) < mEps;
        }

        /// <summary>
        /// 2角の間にあるかを判定(境界を含む)
        /// </summary>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <param name="ang">対象角(rad)</param>
        /// <returns>間にある(true)</returns>
        private bool innerAngle(double sa, double ea, double ang)
        {
            if (ea < sa) {
                ea += Math.PI * 2.0;
                ang += ang < sa ? Math.PI * 2.0 : 0.0;
            }
            if (sa <= ang && ang <= ea)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 外積(線分に対しての左右の位置関係)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>左右の位置(+:左 -:右)</returns>
        public double crossProduct(PointD p)
        {
            PointD v1 = vector();
            PointD v2 = ps.vector(p);
            return v1.crossProduct(v2);
        }

        /// <summary>
        /// 点からの垂線の交点座標(垂点)
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>垂点座標</returns>
        public PointD intersection(PointD p)
        {
            LineD lp = new LineD(ps, p);                            //  始点と点をつなぐ線分
            double ll = lp.length() * Math.Cos(angle2(lp));          //  始点と垂点との距離
            double a = angle();                                     //  線分の角度
            return new PointD(ll * Math.Cos(a) + ps.x, ll * Math.Sin(a) + ps.y);
        }

        /// <summary>
        /// 2線分の交点(延長線上の交点も含む)
        /// </summary>
        /// <param name="l">対象線分</param>
        /// <returns>交点座標</returns>
        public PointD intersection(LineD l)
        {
            PointD v1 = vector();
            PointD v2 = l.vector();
            double k = v1.x * v2.y - v2.x * v1.y;
            if (Math.Abs(k) < mEps) //  平行線の判定
                return null;
            double x = (-1 / k) * (v1.y * v2.x * this.ps.x - v2.y * v1.x * l.ps.x + (l.ps.y - this.ps.y) * v1.x * v2.x);
            double y = (1 / k) * (v1.x * v2.y * this.ps.y - v2.x * v1.y * l.ps.y + (l.ps.x - this.ps.x) * v1.y * v2.y);
            return new PointD(x, y);
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(ArcD arc)
        {
            return intersection(arc.mCp, arc.mR, arc.mSa, arc.mEa);
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">円の半径</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PointD c, double r, double sa = 0.0, double ea = Math.PI * 2.0)
        {
            List<PointD> pointList = new List<PointD>();
            PointD mp = intersection(c);                    //  線分と中心点との垂点
            double l = distance(c);                    //  垂点と中心点との距離
            //  角度の正規化(0～4π)
            sa = ylib.mod(sa, Math.PI * 2);
            ea = ylib.mod(ea, Math.PI * 2);
            ea += ea <= sa ? Math.PI * 2.0 : 0.0;
            if (r < l - mEps) {
                //  交点なし
            } else if (l < mEps) {
                //  中心点を通る
                double ang = angle();
                ang += ang < 0 ? Math.PI : 0;
                if (sa <= ang && ang <= ea) {
                    PointD p = new PointD(r * Math.Cos(ang), r * Math.Sin(ang));
                    p.offset(c);
                    if (onPoint(p))
                        pointList.Add(p);
                }
                ang += Math.PI;
                if (sa <= ang && ang <= ea) {
                    PointD p = new PointD(r * Math.Cos(ang), r * Math.Sin(ang));
                    p.offset(c);
                    if (onPoint(p))
                        pointList.Add(p);
                }
            } else if (Math.Abs(r - l) < mEps) {
                //  接点
                if (innerAngle(sa, ea, mp.angle(c)))
                    pointList.Add(mp);
            } else {
                //  交点
                double th = Math.Acos(l / r);   //  垂線との角度
                LineD ml = new LineD(c, mp);
                double a = ml.angle();
                double ang = ylib.mod(a + th, Math.PI * 2.0);
                ang += ang < sa ? Math.PI * 2 : 0;
                if (sa <= ang && ang <= ea) {
                    PointD p1 = new PointD(c.x + r * Math.Cos(ang), c.y + r * Math.Sin(ang));
                    if (onPoint(p1))
                        pointList.Add(p1);
                }
                ang = ylib.mod(a - th, Math.PI * 2.0);
                ang += ang < sa ? Math.PI * 2 : 0;
                if (sa <= ang && ang <= ea) {
                    PointD p2 = new PointD(c.x + r * Math.Cos(ang), c.y + r * Math.Sin(ang));
                    if (onPoint(p2))
                        pointList.Add(p2);
                }
            }
            return pointList;
        }

        /// <summary>
        /// 指定点の水平線と交点を持つかをチェック
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点あり(true)</returns>
        public bool intersectionHorizon(PointD p)
        {
            if ((ps.y < pe.y && (ps.y <= p.y && p.y <= pe.y)) ||
                (pe.y < ps.y && (pe.y <= p.y && p.y <= ps.y)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 指定点の水平線との交点(延長線上も含む)
        /// </summary>
        /// <param name="p">指定点座標</param>
        /// <returns>交点座標</returns>
        public PointD intersectHorizonPoint(PointD p)
        {
            PointD v = vector();
            v.x = v.x * (p.y - ps.y) / v.y;
            return new PointD(ps.x + v.x, p.y);
        }

        /// <summary>
        /// 線分の中点を求める
        /// </summary>
        /// <returns>中点座標</returns>
        public PointD centerPoint()
        {
            double dx = (pe.x - ps.x) / 2;
            double dy = (pe.y - ps.y) / 2;
            return new PointD(ps.x + dx, ps.y + dy);
        }

        /// <summary>
        /// 点が線分上にあるかを判定
        /// </summary>
        /// <param name="pnt">対象点座標</param>
        /// <returns>線分上にある(true)</returns>
        public bool onPoint(PointD pnt)
        {
            if (pe.x < ps.x) {
                if (pnt.x < (pe.x - mEps) || (ps.x + mEps) < pnt.x)
                    return false;
            } else {
                if (pnt.x < (ps.x - mEps) || (pe.x + mEps) < pnt.x)
                    return false;
            }
            if (pe.y < ps.y) {
                if (pnt.y < (pe.y - mEps) || (ps.y + mEps) < pnt.y)
                    return false;
            } else {
                if (pnt.y < (ps.y - mEps) || (pe.y + mEps) < pnt.y)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 線分の分割点で最も近い点を求める
        /// </summary>
        /// <param name="p">近傍座標</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public PointD nearPoint(PointD p, int  divideNo = 4)
        {
            List<PointD> points = dividePoints(divideNo);
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

        private const int INSIDE = 0b0000;
        private const int LEFT   = 0b0001;
        private const int RIGHT  = 0b0010;
        private const int BOTTOM = 0b0100;
        private const int TOP    = 0b1000;

        /// <summary>
        /// クリッピング領域に対する点の9分割位置の範囲
        /// Cohen-Sutherland aalgorithm
        ///
        /// 1001|1000|1010    9 | 8 | A
        /// --------------   -----------
        /// 0001|0000|0010    1 | 0 | 2
        /// --------------   -----------
        /// 0101|0100|0110    5 | 4 | 6
        /// </summary>
        /// <param name="p">座標</param>
        /// <param name="rect">クリッピング領域</param>
        /// <returns>点位置のビットマップ</returns>
        private byte inOutAreaCode(PointD p, Rect rect)
        {
            double xmin = rect.X;
            double ymin = rect.Y;
            double xmax = rect.X + rect.Width;
            double ymax = rect.Y + rect.Height;
            byte code = INSIDE;
            if (p.x < xmin) code |= LEFT;
            else if (xmax < p.x) code |= RIGHT;
            if (p.y < ymin) code |= BOTTOM;
            else if (ymax < p.y) code |= TOP;
            return code;
        }

        /// <summary>
        /// クリッピング領域に対する点の9分割位置の範囲
        /// </summary>
        /// <param name="p"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private byte inOutAreaCode(Point p, Rect rect)
        {
            return inOutAreaCode(new PointD(p), rect);
        }

        /// <summary>
        /// 線分矩形領域でクリッピングする(Cohen-Sutherland clipping algorithm)
        /// </summary>
        /// <param name="rect">クリッピングの矩形領域</param>
        /// <returns>クリッピングした線分</returns>
        public LineD clippingLine(Rect rect)
        {
            double xmin = rect.X;
            double ymin = rect.Y;
            double xmax = rect.X + rect.Width;
            double ymax = rect.Y + rect.Height;
            LineD l2 = new LineD(ps, pe);
            byte outcode0 = inOutAreaCode(l2.ps, rect);
            byte outcode1 = inOutAreaCode(l2.pe, rect);

            while (true) {
                if ((outcode0 | outcode1) == 0) {           //  両端点がフレームの中(クリッピング不要)
                    break;
                } else if ((outcode0 & outcode1) != 0) {    //  確実に表示されない線分
                    l2 = null;
                    break;
                } else {                                    //  クリッピング処理
                    double x = 0, y = 0;
                    int outcode = outcode0 != 0 ? outcode0 : outcode1;
                    if ((outcode & TOP) != 0) {
                        x = l2.ps.x + (l2.pe.x - l2.ps.x) * (ymax - l2.ps.y) / (l2.pe.y - l2.ps.y);
                        y = ymax;
                    } else if ((outcode & BOTTOM) != 0) {
                        x = l2.ps.x + (l2.pe.x - l2.ps.x) * (ymin - l2.ps.y) / (l2.pe.y - l2.ps.y);
                        y = ymin;
                    } else if ((outcode & RIGHT) != 0) {
                        y = l2.ps.y + (l2.pe.y - l2.ps.y) * (xmax - l2.ps.x) / (l2.pe.x - l2.ps.x);
                        x = xmax;
                    } else if ((outcode & LEFT) != 0) {
                        y = l2.ps.y + (l2.pe.y - l2.ps.y) * (xmin - l2.ps.x) / (l2.pe.x - l2.ps.x);
                        x = xmin;
                    }
                    if (outcode == outcode0) {
                        l2.ps.x = x;
                        l2.ps.y = y;
                        outcode0 = inOutAreaCode(l2.ps, rect);
                    } else {
                        l2.pe.x = x;
                        l2.pe.y = y;
                        outcode1 = inOutAreaCode(l2.pe, rect);
                    }
                }
            }
            return l2;
        }

        /// <summary>
        /// ベクトル分移動させる
        /// </summary>
        /// <param name="vec"></param>
        public void translate(PointD vec)
        {
            ps.offset(vec);
            pe.offset(vec);
        }

        /// <summary>
        /// 鉛直方向に移動させる
        /// </summary>
        /// <param name="d">移動距離</param>
        public void offset(double d)
        {
            PointD vec = getVectorAngle(Math.PI / 2, d);
            translate(vec);
        }

        /// <summary>
        /// 指定点の方向に平行移動させる
        /// </summary>
        /// <param name="dis">オフセット距離</param>
        /// <param name="pos">指定点</param>
        public void offset(double dis, PointD pos)
        {
            PointD ip = intersection(pos);
            LineD line = new LineD(ip, pos);
            line.setLength(dis);
            translate(line.vector());
        }

        /// <summary>
        /// 垂直方向に平行移動させる
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void offset(PointD sp, PointD ep)
        {
            double dis = distance(ep) * Math.Sign(crossProduct(ep)) - distance(sp) * Math.Sign(crossProduct(sp));
            offset(dis);
        }

        /// <summary>
        /// ベクトルで移動する
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        public void offset(PointD vec)
        {
            offset(ps, ps + vec);
        }


        /// <summary>
        /// 指定した距離分だけ同じ向きに平行移動させる
        /// </summary>
        /// <param name="l">移動距離</param>
        public void slide(double l)
        {
            double len = length();
            PointD vec = vector();
            vec.scale(l / len);
            translate(vec);
        }

        /// <summary>
        /// 増分を指定して平行移動
        /// </summary>
        /// <param name="dx">x方向の増分</param>
        /// <param name="dy">y方向の増分</param>
        public void move(double dx, double dy)
        {
            ps.x += dx;
            ps.y += dy;
            pe.x += dx;
            pe.y += dy;
        }

        /// <summary>
        /// 距離と角度を指定して平行移動
        /// </summary>
        /// <param name="r">移動距離</param>
        /// <param name="th">角度(rad)</param>
        public void moveLength(double r, double th)
        {
            double dx = r * Math.Cos(th);
            double dy = r * Math.Sin(th);
            move(dx, dy);
        }

        /// <summary>
        /// 延長線上に距離を指定して移動
        /// </summary>
        /// <param name="l">移動距離</param>
        public void moveLength(double l)
        {
            moveLength(l, angle());
        }

        /// <summary>
        /// 原点を中心に回転
        /// </summary>
        /// <param name="rotate">回転角度(rad)</param>
        public void rotate(double rotate)
        {
            ps.rotate(rotate);
            pe.rotate(rotate);
        }

        /// <summary>
        /// 中心を指定して線分を回転させる
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        public void rotate(Point ctr, double rotate)
        {
            this.rotate(new PointD(ctr), rotate);
        }

        /// <summary>
        /// 中心を指定して線分を回転させる
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        public void rotate(PointD ctr, double rotate)
        {
            ps.rotate(ctr, rotate);
            pe.rotate(ctr, rotate);
        }

        /// <summary>
        /// 指定点を中心に回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="mp">回転角の座標</param>
        public void rotate(PointD cp, PointD mp)
        {
            double ang = mp.angle(cp);
            rotate(cp, ang);
        }

        /// <summary>
        /// 指定線分でミラーする
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void mirror(PointD sp, PointD ep)
        {
            ps.mirror(sp, ep);
            pe.mirror(sp, ep);
        }

        /// <summary>
        /// 指定点の垂点で線分の長さを変える
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        public void trim(PointD sp, PointD ep)
        {
            PointD tp = intersection(sp);
            pe = intersection(ep);
            ps = tp;
        }

        /// <summary>
        /// ピックした位置に近い方を消すようにトリミングする
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="pos"></param>
        public void trimNear(PointD tp, PointD pos)
        {
            if (ps.length(pos) < pe.length(pos)) {
                ps = intersection(tp);
            } else {
                pe = intersection(tp);
            }
        }

        /// <summary>
        /// ピックした位置に遠い方を消すようにトリミングする
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="pos"></param>
        public void trimFar(PointD tp, PointD pos)
        {
            if (ps.length(pos) > pe.length(pos)) {
                ps = intersection(tp);
            } else {
                pe = intersection(tp);
            }
        }

        /// <summary>
        /// 指定位置に近い端点を移動、中間点が近ければ全体を移動
        /// </summary>
        /// <param name="vec">移動量</param>
        /// <param name="pickPos">指定位置</param>
        public void stretch(PointD vec, PointD pickPos)
        {
            double sl = pickPos.length(ps);
            double ml = pickPos.length(centerPoint());
            double el = pickPos.length(pe);
            if (ml < sl & ml < el) {
                translate(vec);
            } else if (sl < el)
                ps.translate(vec);
            else
                pe.translate(vec);
        }

        /// <summary>
        /// 線分を指定点で分割する
        /// 垂点が線分上になければ分割しない
        /// </summary>
        /// <param name="p">分割参照点</param>
        /// <returns>線分リスト</returns>
        public List<LineD> divide(PointD p)
        {
            PointD ip = intersection(p);
            List<LineD> llist = new List<LineD>();
            if (onPoint(ip)) {
                llist.Add(new LineD(ps, ip));
                llist.Add(new LineD(ip, pe));
            }
            return llist;
        }

        /// <summary>
        /// 線分を分割する座標リスト
        /// </summary>
        /// <param name="divNo">分割数</param>
        /// <returns></returns>
        public List<PointD> dividePoints(int divNo)
        {
            List<PointD > points = new List<PointD>();
            double dx = (pe.x - ps.x) / divNo;
            double dy = (pe.y - ps.y) / divNo;
            for (int i = 0; i <= divNo; i++) {
                points.Add(new PointD(ps.x + dx * i, ps.y + dy * i));
            }
            return points;
        }

        /// <summary>
        /// 指定したパターンに線分を分割し分割した座標リストを求めメル
        /// </summary>
        /// <param name="pattern">分割パターン</param>
        /// <param name="offset">パターン開始位置</param>
        /// <returns>座標リスト</returns>
        public List<PointD> dividePattern(List<double> pattern, double offset = 0)
        {
            double leng = length();
            List<PointD> pList = new List<PointD>();
            PointD sp, ep;
            int i = 0;
            LineD l = toCopy();
            double sumLength = 0;
            while (sumLength <= offset) {
                sumLength += pattern[i++ % pattern.Count];
            }
            sumLength -= offset;
            if (i % 2 != 0)
                pList.Add(l.ps.toCopy());
            while (sumLength < leng) {
                l.setLength(sumLength);
                pList.Add(l.pe.toCopy());
                sumLength += pattern[i++ % pattern.Count];
            }
            l.setLength(sumLength);
            pList.Add(pe.toCopy());
            return pList;
        }
    }
}
