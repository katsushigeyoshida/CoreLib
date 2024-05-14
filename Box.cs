using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
    ///     Box()
    ///     Box(PointD p)
    ///     Box(LineD l)
    ///     Box(Size size)                                  コンストラクタ(Sizeから)
    ///     Box(Rect rect)                                  コンストラクタ (Rectから)
    ///     Box(Box box)
    ///     Box(PointD ps, PointD pe)                       コンストラクタ(2点から)
    ///     Box(PointD ps, Size size)                       コンストラクタ(SizeからBox)
    ///     Box(PointD ps, Vector vector)                   コンストラクタ(ベクトルからBox)
    ///     Box(PointD ps, double size)                     コンストラクタ(中心座標と大きさ)
    ///     Box(PointD c, double r, bool inBox = false)     コンストラクタ(円の外接Box)
    ///     Box(ArcD arc)                                   コンストラクタ(円弧の外接Box)
    ///     Box(PointD c, double r, double sa, double ea)   コンストラクタ(円弧の外接Box)
    ///     Box(TextD text)                                 コンストラクタ(TextDの領域)
    ///     Box(double left, double top, double right, double bottom)
    ///     Box(double size)                                コンストラクタ(正方形、原点中心)
    ///     Box(List<PointD> plist)                         座標列からBoxを作成
    ///     Box(string buf)                                 カンマ区切りの文字列をデータに設定
    ///     
    ///     void normalize()                    正規化 (Left < Right, Bottom < Top)
    ///     Box toCopy()                        コピーを作る
    ///     override string ToString()
    ///     string ToString(string form)        書式指定で文字列に変換
    ///     List<PointD> ToPointList()          頂点リストに変換
    ///     List<LineD> ToLineList()            LineDのリストに変換
    ///     Rect ToRect()                       RECTに変換
    ///     void zoom(PointD cp, double zoom, bool inverse = false) 指定した座標を中心にスケーリング
    ///     void zoom(double zoom)              領域を中心からスケーリング
    ///     void offset(PointD dp)              指定した距離を移動
    ///     void offset(double dx, double dy)   指定した距離を移動
    ///     PointD getOffset(PointD p)          指定した点とのオフセット量を求める
    ///     void translate(PointD sp, PointD ep)    始点から終点の距離を移動する
    ///     void rotate(PointD cp, PointD mp)   指定点を中心に回転移動(Box自体は回転しない)
    ///     void mirror(PointD sp, PointD ep)   定線分に対してミラー(Box自体は回転しない)
    ///     void scale(PointD cp, double scale) 原点を指定して拡大縮小
    ///     void setCenter(PointD ctr)          指定点を領域の中心に移動
    ///     PointD getCenter()                  中心座標を求める
    ///     List<PointD> getRoate(PointD org, double rotate)    回転したできた領域の座標リスト
    ///     void rotateArea(PointD org, double rotate)  回転したできた領域をBoxに設定
    ///     List<PointD> getRotateBox(PointD org, double rotate)    指定点を中心に回転させたBoxの頂点リスト
    ///     PointD onPoint(PointD pos)          指定点に対する枠線上の垂点
    ///     PointD nearPoint(PointD pos, int divideNo = 4)  点座標リストから指定点にもっと近い点枠線の座標を取得
    ///     PointD nearPoint(PointD pos)        指定点に最も近い頂点ほ求める
    ///     LineD nearLine(PointD pos)          指定点に最も近いBoxの枠線を求める
    ///     bool outsideChk(Box b)              Boxの外側判定(要素同士がお互いに外側、重なりもなし)
    ///     bool insideChk(double x, double y)  座標の内外判定
    ///     bool insideChk(PointD p)            Pointの内外判定
    ///     bool insideChk(LineD l)             LineDデータの内外判定
    ///     bool insideChk(Rect r)              Rectデータの内外判定
    ///     bool insideChk(Box b)               Boxデータの内外判定
    ///     bool insideChk(PointD c, double r)  円データの内外判定
    ///     bool insideChk(ArcD arc)            円弧データの内外判定
    ///     bool insideChk(PointD c, double r, double sa, double ea)    円弧データの内外判定
    ///     bool insideChk(EllipseD ellipse)    楕円の内外判定
    ///     bool insideChk(List<PointD> plist)  座標リストの座標がすべて内側か
    ///     bool circleInsideChk(PointD c, double r)    円の内側に入っているか
    ///     bool polygonInsideChk(List<PointD> polygon) ポリゴン内にBoxの頂点が入っているの判定
    ///     List<PointD> intersection(LineD l) 線分との交点を求める
    ///     List<PointD> intersection(Box b)    Boxとの交点を求める
    ///     List<PointD> intersection(Point c, double r) 円との交点を求める
    ///     List<PointD> intersection(ArcD arc) 円弧との交点リストを求める
    ///     List<PointD> intersection(PoinDt c, double r, double sa, double ea) 円との交点を求める
    ///     List<PointD> intersection(List<PointD> polyline, bool close = false, bool abort = false)    ポリラインとの交点リストを求める
    ///     List<PointD> intersection(PartsD parts, bool abort = false) パーツ(Parts)の交点リストを求める
    ///     List<LineD> clipLineList(LineD line)    線分をクリッピングしてBox内の線分を求める
    ///     List<ArcD> clipArcList(ArcD arc)    円弧をクリッピングしてBox内の円弧リストを求める
    ///     List<LineD> clipPolyline2LineList(List<PointD> polyline)    ポリラインのクリッピングを線分リストに変換
    ///     List<PointD> clipCircle2PolygonList(Point c, double r, int div = 32) 円と重ね合わせた時の重なる領域の点座標リスト
    ///     List<PointD> clipPolygonList(List<Point> polygon)    ポリゴンをクリッピングしたポリゴン座標点リストを求める
    ///     List<PointD> innerPolygonList(List<Point> polygon)   ポリゴン内に存在する頂点リスト     [うまくいかない]
    ///     bool isInnerPolygon(List<Point> polygon, Point p)   点座標がポリゴン内かの内外判定
    ///     Box andBox(Box b)                   Box同士のANDをとったBoxを求める
    ///     void extension(PointD p)            領域を拡張
    ///     void extension(LineD l)             領域を拡張
    ///     void extension(List<PointD> plist)  領域を拡張
    ///     void extension(Box b)               領域を拡張
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
            set {
                Left = value.x;
                Top = value.y;
            }
        }
        public PointD BottomRight
        {
            get {
                return new PointD(Right, Bottom);
            }
            set {
                Right = value.x;
                Bottom = value.y;
            }
        }
        public PointD TopRight
        {
            get {
                return new PointD(Right, Top);
            }
            set {
                Right = value.x;
                Top = value.y;
            }
        }
        public PointD BottomLeft
        {
            get {
                return new PointD(Left, Bottom);
            }
            set {
                Left = value.x;
                Bottom = value.y;
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

        public Box()
        {
            Left   = 0;
            Right  = 0;
            Top    = 0;
            Bottom = 0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p"></param>
        public Box(PointD p)
        {
            Left   = p.x;
            Right  = p.x;
            Top    = p.y;
            Bottom = p.y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="l"></param>
        public Box(LineD l)
        {
            Left   = l.ps.x;
            Right  = l.ps.x;
            Top    = l.ps.y;
            Bottom = l.ps.y;
            extension(l.pe);
        }

        /// <summary>
        /// コンストラクタ(Sizeから)
        /// </summary>
        /// <param name="size">サイズ(Size)</param>
        public Box(Size size)
        {
            Left   = 0;
            Top    = 0;
            Right  = Left + size.Width;
            Bottom = Top + size.Height;
        }

        /// <summary>
        /// コンストラクタ (Rectから)
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
        /// コンストラクタ (Boxから)
        /// </summary>
        /// <param name="box">Box</param>
        public Box(Box box)
        {
            Left   = box.Left;
            Top    = box.Top;
            Right  = box.Right;
            Bottom = box.Bottom;
        }

        /// <summary>
        /// コンストラクタ(2点から)
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
        public Box(PointD ps, Vector vector)
        {
            Left   = ps.x;
            Top    = ps.y;
            Right  = Left + vector.X;
            Bottom = Top + vector.Y;
        }

        /// <summary>
        /// コンストラクタ(中心座標と大きさ) 
        /// </summary>
        /// <param name="ps">中心座標</param>
        /// <param name="size">四角の大きさ</param>
        public Box(PointD ps, double size)
        {
            size /= 2;
            Left   = ps.x - size;
            Top    = ps.y + size;
            Right  = ps.x + size;
            Bottom = ps.y - size;
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
        /// </summary>
        /// <param name="arc">ArcD</param>
        public Box(ArcD arc)
        {
            List<PointD> points = arc.toPeakList();
            Left = Right = points[0].x;
            Top = Bottom = points[0].y;
            extension(points);
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
            ArcD arc = new ArcD(c, r, sa, ea);
            List<PointD> points = arc.toPeakList();
            Left = Right = points[0].x;
            Top = Bottom = points[0].y;
            extension(points);
        }

        /// <summary>
        /// コンストラクタ(TextDの領域)
        /// </summary>
        /// <param name="text"></param>
        public Box(TextD text)
        {
            List<PointD> points = text.getArea();
            Left = Right = points[0].x;
            Top = Bottom = points[0].y;
            extension(points);
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
        /// コンストラクタ(正方形)
        /// </summary>
        /// <param name="size">一辺の長さ</param>
        public Box(double size)
        {
            Left   = -size / 2;
            Top    =  size / 2;
            Right  =  size / 2;
            Bottom = -size / 2;
        }

        /// <summary>
        /// 座標列からBoxを作成
        /// </summary>
        /// <param name="plist"></param>
        public Box(List<PointD> plist)
        {
            if (plist != null && 0 < plist.Count) {
                Right = Left = plist[0].x;
                Bottom = Top = plist[0].y;
                for (int i = 1; i < plist.Count; i++) {
                    extension(plist[i]);
                }
            } else {
                Left = Right = 0;
                Top = Bottom = 0;
            }
        }

        /// <summary>
        /// カンマ区切りの文字列をデータに設定する
        /// </summary>
        /// <param name="data">文字列</param>
        public Box(string buf)
        {
            string[] data = buf.Split(new char[] { ',' });
            if (3 < data.Length) {
                Left = ylib.string2double(data[0]);
                Top = ylib.string2double(data[1]);
                Right = ylib.string2double(data[2]);
                Bottom = ylib.string2double(data[3]);
            }
        }

        /// <summary>
        /// 正規化 (Left < Right, Bottom < Top)
        /// </summary>
        public void normalize()
        {
            if (Left > Right) {
                double tmp = Left; Left = Right; Right = tmp;
            }
            if (Top < Bottom) {
                double tmp = Top; Top = Bottom; Bottom = tmp;
            }
        }

        /// <summary>
        /// コピーを作る
        /// </summary>
        /// <returns></returns>
        public Box toCopy()
        {
            return new Box(Left, Top, Right, Bottom);
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Left + "," + Top + "," + Right + "," + Bottom;
        }

        /// <summary>
        /// 書式指定で文字列に変換
        /// "000" → [001], "000" → [-001], "X4" → [000C], "0.0" → [12.0], "f5" → [35.12346]
        /// </summary>
        /// <param name="form">書式</param>
        /// <returns></returns>
        public string ToString(string form)
        {
            return Left.ToString(form) + " " + Top.ToString(form) + " " + Right.ToString(form) + " " + Bottom.ToString(form);
        }

        /// <summary>
        /// 頂点リストに変換(TopLeftから時計回り)
        /// </summary>
        /// <param name="rotate">回転角</param>
        /// <returns>Pointリスト</returns>
        public List<PointD> ToPointList(double rotate = 0)
        {
            List<PointD> pList = new List<PointD>();
            pList.Add(TopLeft);
            pList.Add(TopRight);
            pList.Add(BottomRight);
            pList.Add(BottomLeft);
            if (rotate != 0) {
                PointD ctr = getCenter();
                for (int i = 0; i < pList.Count; i++)
                    pList[i].rotate(ctr, rotate);
            }
            return pList;
        }

        /// <summary>
        /// LineDのリストに変換
        /// </summary>
        /// <returns>LindのList</returns>
        public List<LineD> ToLineList()
        {
            List<LineD> lines = new List<LineD>();
            lines.Add(new LineD(TopLeft, BottomLeft));
            lines.Add(new LineD(BottomLeft, BottomRight));
            lines.Add(new LineD(BottomRight, TopRight));
            lines.Add(new LineD(TopRight, TopLeft ));
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
        /// inverseを指定した場合中心点で反転した位置を基準に拡大縮小する
        /// </summary>
        /// <param name="cp">座標</param>
        /// <param name="zoom">スケール</param>
        /// <param name="inverse">反転</param>
        public void zoom(PointD p, double zoom, bool inverse = false)
        {
            this.zoom(zoom);
            PointD v = p.vector(this.getCenter());
            if (inverse)
                v = v.inverse();
            v.scale(zoom - 1.0);
            this.offset(v);
        }

        /// <summary>
        /// 領域を中心からスケーリングする
        /// </summary>
        /// <param name="zoom">スケール</param>
        public void zoom(double zoom)
        {
            double dx = (Width / zoom - Width) / 2.0;
            double dy = (Height / zoom - Height) / 2.0;
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
        /// 始点から終点の距離を移動する
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        public void translate(PointD sp, PointD ep)
        {
            PointD vec = new PointD(ep.x - sp.x, ep.y - sp.y);
            offset(vec);
        }

        /// <summary>
        /// 指定点を中心に回転移動(Box自体は回転しない)
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="mp"></param>
        public void rotate(PointD cp, PointD mp)
        {
            PointD v = getCenter();
            v.rotate(cp, mp);
            translate(getCenter(), v);
        }

        /// <summary>
        /// 指定線分に対してミラー(Box自体は回転しない)
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(PointD sp, PointD ep)
        {
            PointD v = getCenter();
            v.mirror(sp, ep);
            translate(getCenter(), v);
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点</param>
        /// <param name="scale">倍率</param>
        public void scale(PointD cp, double scale)
        {
            PointD p = TopLeft;
            p.scale(cp, scale);
            TopLeft = p.toCopy();
            p = BottomRight;
            p.scale(cp, scale);
            BottomRight = p.toCopy();
        }

        /// <summary>
        /// 指定点に領域の中心を移動する
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
        /// 回転したできた領域の座標リスト
        /// </summary>
        /// <param name="org"></param>
        /// <param name="rotate"></param>
        /// <returns></returns>
        public List<PointD> getRoate(PointD org, double rotate)
        {
            List<PointD> plist = ToPointList();
            for (int i = 0; i < plist.Count; i++) {
                plist[i].rotate(org, rotate);
            }
            return plist;
        }

        /// <summary>
        /// 回転したできた領域をBoxに設定
        /// </summary>
        /// <param name="org">回転原点</param>
        /// <param name="rotate">回転角(rad)</param>
        public void rotateArea(PointD org, double rotate)
        {
            List<PointD> plist = ToPointList();
            for (int i = 0; i< plist.Count; i++) {
                plist[i].rotate(org, rotate);
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
        /// 指定点を中心に回転させたBoxの頂点リスト
        /// </summary>
        /// <param name="org">回転中心点</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <returns>頂点リスト</returns>
        public List<PointD> getRotateBox(PointD org, double rotate)
        {
            List<PointD> plist = ToPointList();
            for (int i = 0; i < plist.Count; i++) {
                plist[i].rotate(org, rotate);
            }
            return plist;
        }

        /// <summary>
        /// 指定点に対する枠線上の垂点
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>点座標</returns>
        public PointD onPoint(PointD pos)
        {
            LineD l = nearLine(pos);
            return l.intersection(pos);
        }

        /// <summary>
        /// 点座標リストから指定点にもっと近い点枠線の座標を取得
        /// </summary>
        /// <param name="pos">最小点</param>
        /// <param name="divideNo">分割数</param>
        /// <returns>座標</returns>
        public PointD nearPoint(PointD pos, int divideNo = 4)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> llist = ToLineList();
            for (int i = 0; i < llist.Count; i++)
                plist.AddRange(llist[i].dividePoints(divideNo));

            if (0 < plist.Count)
                return plist.MinBy(p => p.length(pos));
            else
                return null;
        }

        /// <summary>
        /// 指定点に最も近い頂点ほ求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>点座標</returns>
        public PointD nearPoint(PointD pos)
        {
            List<PointD> plist = ToPointList();
            return plist.MinBy(p => p.length(pos));
        }

        /// <summary>
        /// 指定点に最も近いBoxの枠線を求める
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>線分</returns>
        public LineD nearLine(PointD pos)
        {
            List<LineD> lines = ToLineList();
            return lines.MinBy(l => l.distance(pos));
        }

        /// <summary>
        /// Boxの外側判定(要素同士がお互いに外側、重なりもなし)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool outsideChk(Box b)
        {
            if (Left < Right) {
                if (Right < b.Left && Right < b.Right)
                    return true;
                if (b.Left < Left && b.Right < Left)
                    return true;
            } else {
                if (Left < b.Left && Left < b.Right)
                    return true;
                if (b.Left < Right && b.Right < Right)
                    return true;
            }
            if (Bottom < Top) {
                if (Top < b.Top && Top < b.Bottom)
                    return true;
                if (b.Top < Bottom && b.Bottom < Bottom)
                    return true;
            } else {
                if (Bottom < b.Top && Bottom < b.Bottom)
                    return true;
                if (b.Top < Top && b.Bottom < Top)
                    return true;
            }
            return false;
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
            if (insideChk(new PointD(r.TopLeft)) && insideChk(new PointD(r.BottomRight)))
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
        /// </summary>
        /// <param name="arc">円弧データ</param>
        /// <returns>内側</returns>
        public bool insideChk(ArcD arc)
        {
            Box b = new Box(arc);
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
        /// 楕円の内外判定
        /// </summary>
        /// <param name="ellipse">楕円データ</param>
        /// <returns></returns>
        public bool insideChk(EllipseD ellipse)
        {
            Box b = ellipse.getArea();
            return insideChk(b);
        }

        /// <summary>
        /// 座標リストの座標がすべて内側か
        /// </summary>
        /// <param name="plist"></param>
        /// <returns></returns>
        public bool insideChk(List<PointD> plist)
        {
            foreach (PointD p in plist) {
                if (!insideChk(p)) return false;
            }
            return true;
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
        /// ポリゴン内にBoxの頂点が入っているの判定
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public bool polygonInsideChk(List<PointD> polygon)
        {
            List<PointD> hlist = new List<PointD>();
            List<PointD> plist = ToPointList();
            for (int i = 0; i < plist.Count; i++) {
                if (isInnerPolygon(polygon, plist[i]))
                    hlist.Add(plist[i]);
            }
            return hlist.Count == 4;
        }

        /// <summary>
        /// 線分との交点を求める
        /// </summary>
        /// <param name="l">線分</param>
        /// <returns>点座標リスト</returns>
        public List<PointD> intersection(LineD l)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> lines = ToLineList();
            foreach (LineD line in lines) {
                PointD p = line.intersection(l);
                if (p != null && line.onPoint(p) && l.onPoint(p))
                    pointList.Add(p);
            }
            return pointList;
        }

        /// <summary>
        /// Boxとの交点を求める
        /// </summary>
        /// <param name="b">Box</param>
        /// <returns>座標リスト</returns>
        public List<PointD> intersection(Box b)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> lines = ToLineList();
            List<LineD> blines = b.ToLineList();
            foreach (LineD line in lines) {
                for (int j = 0; j < blines.Count; j++) {
                    PointD p = blines[j].intersection(line);
                    if (p != null) {
                        if (blines[j].onPoint(p) && line.onPoint(p))
                            plist.Add(p);
                    }
                }
            }
            return plist;
        }

        /// <summary>
        /// 円との交点を求める
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">円の半径</param>
        /// <returns>交点座標リスト</returns>
        public List<PointD> intersection(PointD c, double r)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> lines = ToLineList();
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
        /// <param name="arc">円弧データ</param>
        /// <returns></returns>
        public List<PointD> intersection(ArcD arc)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> boxLine = ToLineList();                           //  Boxの線分リスト
            foreach (LineD line in boxLine) {
                List<PointD> plist = arc.intersection(line);    //  円弧との交点リスト
                if (0 < plist.Count)
                    pointList.AddRange(plist);
            }
            return pointList;
            //return intersection(arc.mCp, arc.mR, arc.mSa, arc.mEa);
        }

        /// <summary>
        /// 円弧との交点リストを求める
        /// </summary>
        /// <param name="c">円の中心</param>
        /// <param name="r">円の半径</param>
        /// <param name="sa">開始角(rad)</param>
        /// <param name="ea">終了角(rad)</param>
        /// <returns>交点座標点リスト</returns>
        public List<PointD> intersection(PointD c, double r, double sa, double ea)
        {
            List<PointD> pointList = new List<PointD>();
            List<LineD> boxLine = ToLineList();                           //  Boxの線分リスト
            foreach (LineD line in boxLine) {
                List<PointD> plist = line.intersection(c, r, sa, ea);    //  円弧との交点リスト
                if (0 < plist.Count)
                    pointList.AddRange(plist);
            }
            return pointList;
        }

        /// <summary>
        /// ポリラインとの交点リストを求める
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <param name="close">閉ループ</param>
        /// <param name="abort">一つでも交点があれば中断</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(List<PointD> polyline, bool close = false, bool abort = false)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> ll = ToLineList();
            for (int i = 0; i < polyline.Count; i++) {
                if (!close && i == polyline.Count - 1)
                    break;
                if (i < polyline.Count - 2 && polyline[i + 1].type == 1) {
                    ArcD arc = new ArcD(polyline[i], polyline[i + 1], polyline[(i + 2) % polyline.Count]);
                    for (int j = 0; j < ll.Count; j++) {
                        plist.AddRange(ll[j].intersection(arc));
                    }
                    i++;
                } else {
                    LineD l = new LineD(polyline[i], polyline[(i + 1) % polyline.Count]);
                    for (int j = 0; j < ll.Count; j++) {
                        PointD p = ll[j].intersection(l);
                        if (p != null) {
                            if (ll[j].onPoint(p) && l.onPoint(p)) {
                                plist.Add(p);
                                if (abort)
                                    return plist;
                            }
                        }
                    }
                }
            }
            return plist;
        }


        /// <summary>
        /// パーツ(Parts)の交点リストを求める
        /// </summary>
        /// <param name="parts">パーツデータ</param>
        /// <param name="abort">一つでも交点があれば中断</param>
        /// <returns></returns>
        public List<PointD> intersection(PartsD parts, bool abort = false)
        {
            List<PointD> plist = new List<PointD>();
            List<LineD> ll = ToLineList();
            for (int i = 0; i < ll.Count; i++) {
                plist.AddRange(parts.intersection(ll[i]));
                if (abort && 0 < plist.Count)
                    return plist;
            }
            return plist;
        }

        /// <summary>
        /// 線分をクリッピングしてBox内の線分を求める
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>線分リスト</returns>
        public List<LineD> clipLineList(LineD line)
        {
            List<LineD> llist = new();
            if (insideChk(line)) {
                llist.Add(line);
            } else {
                List<PointD> plist = intersection(line);
                if (0 < plist.Count) {
                    plist.Add(line.ps);
                    plist.Add(line.pe);
                    List<PointD> lplist;
                    double ang = line.angle();
                    if ((Math.PI / 4 < ang && ang < Math.PI / 4 * 3) ||
                        (-Math.PI / 4 * 3 < ang && ang < -Math.PI / 4)) {
                        plist.Sort((a, b) => Math.Sign(a.y - b.y));
                    } else {
                        plist.Sort((a, b) => Math.Sign(a.x - b.x));
                    }
                    for (int i = 0; i < plist.Count - 1; i++) {
                        LineD l = new LineD(plist[i], plist[i + 1]);
                        if (0 < l.length() && insideChk(l.centerPoint()))
                            llist.Add(l);
                    }
                }
            }
            return llist;
        }

        /// <summary>
        /// 円弧をクリッピングしてBox内の円弧リストを求める
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>円弧リスト</returns>
        public List<ArcD> clipArcList(ArcD arc)
        {
            List<ArcD> alist = new();
            if (insideChk(arc)) {
                alist.Add(arc);
            } else {
                List<PointD> plist = intersection(arc);
                if (0 < plist.Count) {
                    List<double> angList = new();
                    angList.Add(arc.mSa);
                    angList.Add(arc.mEa);
                    for (int i = 0; i < plist.Count; i++)
                        angList.Add(arc.getAngle(plist[i]));
                    angList.Sort();
                    for (int i = 0; i < angList.Count - 1; i++) {
                        ArcD a = arc.toCopy();
                        a.mSa = angList[i];
                        a.mEa = angList[i + 1];
                        Box b = new Box(a);
                        if (0 < a.mEa - a.mSa && insideChk(b.getCenter()))
                            alist.Add(a);
                    }
                }
            }
            return alist;
        }

        /// <summary>
        /// ポリラインのクリッピングを線分リストに変換
        /// </summary>
        /// <param name="polyline">ポリライン点リスト</param>
        /// <returns>線分リスト</returns>
        public List<LineD> clipPolyline2LineList(List<PointD> polyline)
        {
            List<LineD> llist = new List<LineD>();
            List<LineD> ll = ToLineList();
            for (int i = 0; i < polyline.Count - 1; i++) {
                LineD l = new LineD(polyline[i], polyline[i + 1]);
                if (insideChk(l)) {
                    llist.Add(l);
                } else {
                    List<PointD> plist = new List<PointD>();
                    if (insideChk(l.ps))
                        plist.Add(l.ps);
                    for (int j = 0; j < ll.Count; j++) {
                        PointD p = ll[j].intersection(l);
                        if (p != null) {
                            if (ll[j].onPoint(p) && l.onPoint(p))
                                plist.Add(p);
                        }
                    }
                    if (insideChk(l.pe))
                        plist.Add(l.pe);
                    if (plist.Count == 2)
                        llist.Add(new LineD(plist[0], plist[1]));
                }
            }
            return llist;

        }


        /// <summary>
        /// 円と重ね合わせた時の重なる領域の点座標リスト(BOX & 円の領域)
        /// </summary>
        /// <param name="c">中心座標</param>
        /// <param name="r">半径</param>
        /// <returns>点座標リスト</returns>
        public List<PointD> clipCircle2PolygonList(PointD c, double r, int div = 32)
        {
            List<PointD> plist = intersection(c, r);      //  BOXと円の交点リスト
            List<PointD> blist = ToPointList();              //  BOXの頂点リスト
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
        public List<PointD> clipPolygonList(List<PointD> polygon)
        {
            List<PointD> plist = intersection(polygon, true);  //  ポリゴンとBoxの交点リスト
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
            List<PointD> plist = ToPointList();
            polygon = ylib.pointSort(polygon);
            for (int i = 0; i < plist.Count; i++) {
                if (isInnerPolygon(polygon, plist[i]))
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

        /// <summary>
        /// Box同士のANDをとったBoxを求める
        /// </summary>
        /// <param name="b">Box</param>
        /// <returns>AND Box</returns>
        public Box andBox(Box b)
        {
            if (insideChk(b))
                return new Box(b);
            if (b.insideChk(this))
                return this;
            List<PointD> plist = intersection(b);
            List<PointD> list = ToPointList();
            foreach (PointD p in list)
                if (b.insideChk(p))
                    plist.Add(p);
            list = b.ToPointList();
            foreach (PointD p in list)
                if (insideChk(p))
                    plist.Add(p);
            if (plist.Count == 0)
                return null;
            Box box = new Box(plist[0]);
            for (int i = 1; i < plist.Count; i++)
                box.extension(plist[i]);
            box.normalize();
            return box;
        }

        /// <summary>
        /// 領域を拡張する
        /// </summary>
        /// <param name="p">点座標</param>
        public void extension(PointD p)
        {
            if (Left < Right) {
                Left = Math.Min(Left, p.x);
                Right = Math.Max(Right, p.x);
            } else {
                Left = Math.Max(Left, p.x);
                Right = Math.Min(Right, p.x);
            }
            if (Bottom < Top) {
                Bottom = Math.Min(Bottom, p.y);
                Top = Math.Max(Top, p.y);
            } else {
                Bottom = Math.Max(Bottom, p.y);
                Top = Math.Min(Top, p.y);
            }
        }

        /// <summary>
        /// 領域を拡張する
        /// </summary>
        /// <param name="l">線分</param>
        public void extension(LineD l)
        {
            extension(l.ps);
            extension(l.pe);
        }

        /// <summary>
        /// 領域を拡張する
        /// </summary>
        /// <param name="plist"></param>
        public void extension(List<PointD> plist)
        {
            foreach (var p in plist)
                extension(p);
        }

        /// <summary>
        /// 領域を拡張する
        /// </summary>
        /// <param name="b">BOX</param>
        public void extension(Box b)
        {
            extension(b.TopLeft);
            extension(b.BottomRight);
        }
    }
}
