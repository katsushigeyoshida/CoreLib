using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace CoreLib
{
    /// <summary>
    /// Rectクラスの代用で四角の領域を定義するクラス
    /// Rectでは必ず Top < Bottom,Left < Right の関係に正規化してしまうため
    /// ワールド座標で天地を逆にすることができないため作成したクラス
    /// 基本はRectと同じだが正規化はしない
    /// 
    /// プロパティ
    ///     Left,Top,Right,Bottom
    ///     TopLeft,BottomRight,TopRight,BottomLeft
    ///     Width,Height
    ///     Location,Size,
    /// コンストラクタ
    ///     Box(Size size)
    ///     Box(Rect rect)
    ///     Box(Point ps, Point pe)
    ///     Box(PointD ps, PointD pe)
    ///     Box(Point ps, Size size)
    ///     Box(PointD ps, Size size)
    ///     Box(PointD ps, Vector vector)
    ///     Box(PointD c, double r, bool inBox = false)
    ///     Box(PointD c, double r, double sa, double ea)
    ///     Box(double left, double top, double right, double bottom)
    ///     
    ///     override string ToString()
    ///     List<PointD> ToPointList()
    ///     List<LineD> ToLineList()
    ///     Rect ToRect()
    ///     void zoom(Point cp, double zoom)
    ///     void zoom(PointD cp, double zoom)
    ///     void zoom(double zoom)
    ///     void offset(PointD dp)
    ///     void offset(double dx, double dy)
    ///     Point getOffset(PointD p)
    ///     void setCenter(PointD ctr)
    ///     PointD getCenter()
    ///     void rotateArea(PointD org, double rotate)
    ///     bool insideChk(double x, double y)
    ///     bool insideChk(PointD p)
    ///     bool insideChk(Line l)
    ///     bool insideChk(LineD l)
    ///     bool insideChk(Rect r)
    ///     bool insideChk(Box b)
    ///     bool insideChk(Point c, double r)
    ///     bool insideChk(Point c, double r, double sa, double ea)
    ///     bool circleInsideChk(Point c, double r)
    ///     List<Point> intersectLine(LineD l)
    ///     List<Point> intersectCircle(Point c, double r)
    ///     List<Point> intersectArc(Point c, double r, double sa, double ea)
    ///     List<Point> intersectPolygon(List<Point> polygon)
    ///     List<Point> clipCircle2PolygonList(Point c, double r, int div = 32)
    ///     List<Point> clipPolygon2PolygonList(List<Point> polygon)
    ///     List<Point> innerPolygonList(List<Point> polygon)
    ///     isInnerPolygon(List<Point> polygon, Point p)
    /// 
    /// </summary>
    /// 

    public class Box
    {
        private double mEps = 1E-8;
        private YLib ylib = new YLib();

        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public PointD TopLeft
        {
            get {
                return new PointD(Left, Top);
            }
        }
        public PointD BottomRight
        {
            get {
                return new PointD(Right, Bottom);
            }
        }
        public PointD TopRight
        {
            get {
                return new PointD(Right, Top);
            }
        }
        public PointD BottomLeft
        {
            get {
                return new PointD(Left, Bottom);
            }
        }
        public double Width
        {
            get {
                return Math.Abs(Right - Left);
            }
            set {
                Right = Left + value;
            }
        }
        public double Height
        {
            get {
                return Math.Abs(Top - Bottom);
            }
            set {
                Bottom = Top - value;
            }
        }
        public PointD Location
        {
            get {
                return new PointD(Left, Top);
            }
            set {
                Left = value.x;
                Top = value.y;
            }
        }
        public Size Size
        {
            get {
                return new Size(Math.Abs(Right - Left), Math.Abs(Top - Bottom));
            }
            set {
                Right = Left + value.Width;
                Bottom = Top + value.Height;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size">サイズ(Size)</param>
        public Box(Size size)
        {
            Left = 0;
            Top = 0;
            Right = Left + size.Width;
            Bottom = Top + size.Height;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rect">Rect</param>
        public Box(Rect rect)
        {
            Left   = rect.Left;
            Top    = rect.Top;
            Right  = Left + rect.Width;
            Bottom = Top + rect.Height;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public Box(Point ps, Point pe)
        {
            Left  = ps.X;
            Top   = ps.Y;
            Right  = pe.X;
            Bottom = pe.Y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public Box(PointD ps, PointD pe)
        {
            Left   = ps.x;
            Top    = ps.y;
            Right  = pe.x;
            Bottom = pe.y;
        }

        /// <summary>
        /// コンストラクタ(SizeからBox)
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="size">サイズ(Size)</param>
        public Box(Point ps, Size size)
        {
            Left   = ps.X;
            Top    = ps.Y;
            Right  = Left + size.Width;
            Bottom = Top - size.Height;
        }

        /// <summary>
        /// コンストラクタ(SizeからBox)
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="size">サイズ(Size)</param>
        public Box(PointD ps, Size size)
        {
            Left   = ps.x;
            Top    = ps.y;
            Right  = Left + size.Width;
            Bottom = Top - size.Height;
        }

        /// <summary>
        /// コンストラクタ(ベクトルからBox)
        /// </summary>
        /// <param name="ps">左上座標</param>
        /// <param name="vector">ベクトル</param>
        public Box(Point ps, Vector vector)
        {
            Left   = ps.X;
            Top    = ps.Y;
            Right  = Left + vector.X;
            Bottom = Top + vector.Y;
        }

        /// <summary>
        /// コンストラクタ(ベクトルからBox)
        /// </summary>
        /// <param name="ps">左上座標</param>
        /// <param name="vector">ベクトル</param>
        public Box(PointD ps, Vector vector)
        {
            Left   = ps.x;
            Top    = ps.y;
            Right  = Left + vector.X;
            Bottom = Top + vector.Y;
        }

        /// <summary>
        /// コンストラクタ(円の外接Box)
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">半径</param>
        /// <param name="inBox">内接/外接</param>
        public Box(Point c, double r, bool inBox = false)
        {
            if (inBox) {
                //  内接の場合(四角の内側))
                r /= Math.Sqrt(2);
            }
            Left   = c.X - r;
            Top    = c.Y + r;
            Right  = c.X + r;
            Bottom = c.Y - r;
        }

        /// <summary>
        /// コンストラクタ(円の外接Box)
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">半径</param>
        /// <param name="inBox">内接/外接</param>
        public Box(PointD c, double r, bool inBox = false)
        {
            if (inBox) {
                //  内接の場合(四角の内側))
                r /= Math.Sqrt(2); 
            }
            Left   = c.x - r;
            Top    = c.y + r;
            Right  = c.x + r;
            Bottom = c.y - r;
        }

        /// <summary>
        /// コンストラクタ(円弧の外接Box)
        /// 開始角と終了角は反時計回りに設定
        /// </summary>
        /// <param name="c">中心点</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        public Box(Point c, double r, double sa, double ea)
        {
            List<Point> plist = ylib.arcPeakPoint(c, r, sa, ea);
            Left   = plist[0].X;
            Top    = plist[0].Y;
            Right  = plist[0].X;
            Bottom = plist[0].Y;
            foreach (Point p in plist) {
                Left   = Math.Min(Left, p.X);
                Top    = Math.Max(Top, p.Y);
                Right  = Math.Max(Right, p.X);
                Bottom = Math.Min(Bottom, p.Y);
            }
        }

        /// <summary>
        /// コンストラクタ(円弧の外接Box)
        /// 開始角と終了角は反時計回りに設定
        /// </summary>
        /// <param name="c">中心点</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        public Box(PointD c, double r, double sa, double ea)
        {
            List<PointD> plist = ylib.arcPeakPoint(c, r, sa, ea);
            Left   = plist[0].x;
            Top    = plist[0].y;
            Right  = plist[0].x;
            Bottom = plist[0].y;
            foreach (PointD p in plist) {
                Left   = Math.Min(Left, p.x);
                Top    = Math.Max(Top, p.y);
                Right  = Math.Max(Right, p.x);
                Bottom = Math.Min(Bottom, p.y);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="left">左X 座標</param>
        /// <param name="top">上Y座標</param>
        /// <param name="right">右X座標</param>
        /// <param name="bottom">下Y座標</param>
        public Box(double left, double top, double right, double bottom)
        {
            Left   = left;
            Top    = top;
            Right  = right;
            Bottom = bottom;
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Left + " " + Top + " " + Right + " " + Bottom;
        }

        /// <summary>
        /// 頂点リストに変換
        /// </summary>
        /// <returns>Pointリスト</returns>
        public List<Point> ToPointList()
        {
            List<Point> pList = new List<Point>();
            pList.Add(TopRight.toPoint());
            pList.Add(TopLeft.toPoint());
            pList.Add(BottomLeft.toPoint());
            pList.Add(BottomRight.toPoint());
            return pList;
        }

        /// <summary>
        /// 頂点リストに変換
        /// </summary>
        /// <returns>Pointリスト</returns>
        public List<PointD> ToPointDList()
        {
            List<PointD> pList = new List<PointD>();
            pList.Add(TopRight);
            pList.Add(TopLeft);
            pList.Add(BottomLeft);
            pList.Add(BottomRight);
            return pList;
        }

        /// <summary>
        /// LineDのリストに変換
        /// </summary>
        /// <returns>LindのList</returns>
        public List<LineD> ToLineDList()
        {
            List<LineD> lines = new List<LineD>();
            lines.Add(new LineD(TopLeft, TopRight));
            lines.Add(new LineD(BottomLeft, BottomRight));
            lines.Add(new LineD(TopLeft, BottomLeft));
            lines.Add(new LineD(BottomRight, TopRight));
            return lines;
        }

        /// <summary>
        /// RECTに変換する
        /// </summary>
        /// <returns>RECT</returns>
        public Rect ToRect()
        {
            return new Rect(TopLeft.toPoint(), BottomRight.toPoint());
        }

        /// <summary>
        /// 指定した座標を中心にしてスケーリングする
        /// </summary>
        /// <param name="cp">座標</param>
        /// <param name="zoom">スケール</param>
        public void zoom(Point cp, double zoom)
        {
            this.zoom(new PointD(cp), zoom);
        }

        /// <summary>
        /// 指定した座標を中心にしてスケーリングする
        /// </summary>
        /// <param name="cp">座標</param>
        /// <param name="zoom">スケール</param>
        public void zoom(PointD p, double zoom)
        {
            this.zoom(zoom);
            PointD v = p.vector(this.getCenter());
            v.scale(zoom - 1.0);
            this.offset(v);
        }

        /// <summary>
        /// 領域を中心からスケーリングする
        /// </summary>
        /// <param name="zoom">スケール</param>
        public void zoom(double zoom)
        {
            double dx = (Width * zoom - Width) / 2.0;
            double dy = (Height * zoom - Height) / 2.0;
            Left   -= Left < Right ? dx : -dx;
            Top    += Top > Bottom ? dy : -dy;
            Right  += Left < Right ? dx : -dx;
            Bottom -= Top > Bottom ? dy : -dy;
        }

        /// <summary>
        /// 指定した距離を移動する
        /// </summary>
        /// <param name="dp">移動量</param>
        public void offset(PointD dp)
        {
            offset(dp.x, dp.y);
        }

        /// <summary>
        /// 指定した距離を移動する
        /// </summary>
        /// <param name="dx">X移動量</param>
        /// <param name="dy">Y移動量</param>
        public void offset(double dx, double dy)
        {
            Left   += dx;
            Top    += dy;
            Right  += dx;
            Bottom += dy;
        }

        /// <summary>
        /// 指定した点とのオフセット量を求める
        /// </summary>
        /// <param name="p">オフセット量</param>
        /// <returns></returns>
        private PointD getOffset(PointD p)
        {
            PointD cp = getCenter();
            return new PointD(p.x - cp.x, p.y - cp.y);
        }

        /// <summary>
        /// 指定点を領域の中心に移動する
        /// </summary>
        /// <param name="ctr">指定座標点</param>
        public void setCenter(PointD ctr)
        {
            offset(getOffset(ctr));
        }

        /// <summary>
        /// 中心座標を求める
        /// </summary>
        /// <returns>中心座標</returns>
        public PointD getCenter()
        {
            return new PointD((Left + Right) / 2.0, (Top + Bottom) / 2.0);
        }

        /// <summary>
        /// 回転したできた領域をBoxに設定
        /// </summary>
        /// <param name="org">回転原点</param>
        /// <param name="rotate">回転角(rad)</param>
        public void rotateArea(PointD org, double rotate)
        {
            List<PointD> plist = ToPointDList();
            for (int i = 0; i< plist.Count; i++) {
                plist[i].rotatePoint(org, rotate);
                //plist[i] = ylib.rotatePoint(org, plist[i], rotate);
            }
            Left   = plist[0].x;
            Right  = plist[0].x;
            Top    = plist[0].y;
            Bottom = plist[0].y;
            for (int i = 1; i < plist.Count; i++) {
                Left = Math.Min(Left, plist[i].x);
                Right = Math.Max(Right, plist[i].x);
                Bottom = Math.Min(Bottom, plist[i].y);
                Top = Math.Max(Top, plist[i].y);
            }
        }

        /// <summary>
        /// 座標の内外判定
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns></returns>
        public bool insideChk(double x, double y)
        {
            return insideChk(new PointD(x, y));
        }


        /// <summary>
        /// Pointの内外判定
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(Point p)
        {
            return insideChk(new PointD(p));
        }

        /// <summary>
        /// Pointの内外判定
        /// </summary>
        /// <param name="p">点座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(PointD p)
        {
            if (Left < Right) {
                if (p.x < Left - mEps || Right + mEps < p.x)
                    return false;
            } else {
                if (p.x < Right - mEps || Left + mEps < p.x)
                    return false;
            }
            if (Top < Bottom) {
                if (p.y < Top - mEps || Bottom + mEps < p.y)
                    return false;
            } else {
                if (p.y < Bottom - mEps || Top + mEps < p.y)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Lineデータの内外判定
        /// すべて内側に入っている
        /// </summary>
        /// <param name="l">線分座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(Line l)
        {
            if (insideChk(l.X1, l.Y1) && insideChk(l.X2, l.Y2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// LineDデータの内外判定
        /// すべて内側に入っている
        /// </summary>
        /// <param name="l">線分座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(LineD l)
        {
            if (insideChk(l.ps.x, l.ps.y) && insideChk(l.pe.x, l.pe.y))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Rectデータの内外判定
        /// </summary>
        /// <param name="r">Rect座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(Rect r)
        {
            if (insideChk(r.TopLeft) && insideChk(r.BottomRight))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Boxデータの内外判定
        /// </summary>
        /// <param name="b">Box座標</param>
        /// <returns>内側(境界含む)true</returns>
        public bool insideChk(Box b)
        {
            if (insideChk(b.TopLeft) && insideChk(b.BottomRight))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 円データの内外判定
        /// すべてが四角の中に入っている
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">半径</param>
        /// <returns>内側</returns>
        public bool insideChk(PointD c, double r)
        {
            Box b = new Box(c, r);
            return insideChk(b);
        }

        /// <summary>
        /// 円弧データの内外判定
        /// すべてが四角の中に入っている
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <returns>内側</returns>
        public bool insideChk(PointD c, double r, double sa, double ea)
        {
            Box b = new Box(c, r, sa, ea);
            return insideChk(b);
        }

        /// <summary>
        /// 円の内側に入っているか
        /// </summary>
        /// <param name="c">円の中心座標</param>
        /// <param name="r">円の半径</param>
        /// <returns></returns>
        public bool circleInsideChk(PointD c, double r)
        {
            double l0 = c.length(TopLeft);
            double l1 = c.length(TopRight);
            double l2 = c.length(BottomLeft);
            double l3 = c.length(BottomRight);
            if (l0 <= r && l1 <= r && l2 <= r && l3 <= r)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 線分との交点を求める
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns>点座標リスト</returns>
        public List<PointD> intersectLine(LineD l)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> lines = ToLineDList();
            foreach (LineD line in lines) {
                PointD p = line.intersection(l);
                if (p.x != double.NaN && l.lineOnPoint(p))
                    pointList.Add(p);
            }
            return pointList;
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">円の半径</param>
        /// <returns>交点座標リスト</returns>
        public List<PointD> intersectCircle(PointD c, double r)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> lines = ToLineDList();
            foreach (LineD line in lines) {
                List<PointD> plist = line.intersection(c, r);
                if (0 < plist.Count)
                    pointList.AddRange(plist);
            }
            return pointList;
        }

        /// <summary>
        /// 円弧との交点リストを求める
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">円の半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <returns>交点座標点リスト</returns>
        public List<PointD> intersectArc(PointD c, double r, double sa, double ea)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> lines = ToLineDList();                           //  Boxの線分リスト
            foreach (LineD line in lines) {
                List<PointD> plist = line.intersection(c, r, sa, ea);    //  円弧との交点リスト
                if (0 < plist.Count)
                    pointList.AddRange(plist);
            }
            return pointList;
        }

        /// <summary>
        /// ポリゴンの座標点リストから交点リストを求める
        /// </summary>
        /// <param name="polygon">ポリゴン座標点リスト</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersectPolygon(List<PointD> polygon)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> ll = ToLineDList();
            for (int i = 0; i < polygon.Count; i++) {
                LineD l;
                if (i == polygon.Count - 1) {
                    l = new LineD(polygon[i], polygon[0]);
                } else {
                    l = new LineD(polygon[i], polygon[i + 1]);
                }
                for (int j = 0; j < ll.Count; j++) {
                    PointD p = ll[j].intersection(l);
                    if (!double.IsNaN(p.x) && !double.IsNaN(p.y)) {
                        if (ll[j].lineOnPoint(p) && l.lineOnPoint(p))
                            plist.Add(p);
                    }
                }
            }
            return plist;
        }

        /// <summary>
        /// 円と重ね合わせた時の重なる領域の点座標リスト(BOX & 円の領域)
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <returns>点座標リスト</returns>
        public List<PointD> clipCircle2PolygonList(PointD c, double r, int div = 32)
        {
            List<PointD> plist = intersectCircle(c, r);      //  BOXと円の交点リスト
            List<PointD> blist = ToPointDList();              //  BOXの頂点リスト
            //  円の範囲内の点座標
            foreach (PointD p in blist) {                    //  交点とBOX頂点とのマージ
                double l = c.length(p);
                if (l < r)
                    plist.Add(p);
            }
            List<PointD> cpList = ylib.divideCircleList(c, r, div);  //  円の頂点リスト
            //  BOX内の点座標
            foreach (PointD p in cpList) {
                if (insideChk(p))
                    plist.Add(p);
            }
            List<PointD> spList = ylib.pointSort(plist);
            return spList;
        }

        /// <summary>
        /// ポリゴンをクリッピングしたポリゴン座標点リストを求める
        /// ポリゴン領域とBox領域の AND のリスト
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <returns>クリッピングしたポリゴン</returns>
        public List<PointD> clipPolygon2PolygonList(List<PointD> polygon)
        {
            List<PointD> plist = intersectPolygon(polygon);  //  ポリゴンとBoxの交点リスト
            plist.AddRange(polygon);
            plist.AddRange(innerPolygonList(polygon));      //  ポリゴン内にあるBoxの頂点リスト追加
            plist = ylib.pointSort(plist);
            plist = ylib.pointListSqueeze(plist);

            //  Box内のみの頂点を抽出
            List<PointD> polgonList = new List<PointD>();
            for (int i = 0; i < plist.Count; i++) {
                if (insideChk(plist[i]))
                    polgonList.Add(plist[i]);
            }
            return polgonList;
        }

        /// <summary>
        /// ポリゴン内に存在する頂点リスト     [うまくいかない]
        /// </summary>
        /// <param name="polygon">ポリゴン座標リスト</param>
        /// <returns>頂点リスト</returns>
        public List<PointD> innerPolygonList(List<PointD> polygon)
        {
            List<PointD> hlist = new List<PointD>();
            List<PointD> plist = ToPointDList();
            polygon = ylib.pointSort(polygon);
            for (int i = 0; i < plist.Count; i++) {
                if (isInnerPolygon(polygon, plist[i]))
                //if (ylib.isInnerPolygon(polygon, plist[i]))
                    hlist.Add(plist[i]);
            }
            return hlist;
        }

        /// <summary>
        /// 点座標がポリゴン内かの内外判定
        /// </summary>
        /// <param name="polygon">ポリゴン(点座標リスト)</param>
        /// <param name="p">点座標</param>
        /// <returns>内側(true)</returns>
        private bool isInnerPolygon(List<PointD> polygon, PointD p)
        {
            List<int> ilist = new List<int>();
            for (int j = 0; j < polygon.Count; j++) {
                LineD l;
                if (j == polygon.Count - 1) {
                    l = new LineD(polygon[j], polygon[0]);
                } else {
                    l = new LineD(polygon[j], polygon[j + 1]);
                }
                if (l.intersectionHorizon(p)) {
                    PointD ip = l.intersectHorizonPoint(p);
                    ilist.Add(Math.Sign(ip.x - p.x));
                    //count += ylib.isClockWise(l.ps, l.pe, plist[i]);
                }
            }
            if (0 < ilist.Count) {
                int count = 0;
                foreach (int i in ilist) {
                    count += i;
                }
                if (count == 0)
                    return true;
            }
            return false;
        }
    }
}
