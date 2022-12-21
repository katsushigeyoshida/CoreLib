using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CoreLib
{
    /// <summary>
    /// ワールド座標系でのグラフィック表示
    /// 
    /// YWorldDraw(Canvas c)
    /// YWorldDraw(Canvas c, double viewWidth, double viewHeight)
    /// ---  ViewとWorldの領域設定
    /// void setViewArea(double left, double top, double right, double bottom)
    /// void setWorldWindow(double left, double top, double right, double bottom)
    /// void setWorldTextSize(double size)
    /// double getWorldTextSize()
    /// void aspectFix()
    /// ---  スクリーン座標とワールド座標の相互変換
    /// double cnvWorld2ScreenX(double x)
    /// double cnvWorld2ScreenY(double y)
    /// double cnvScreen2WorldX(double x)
    /// double cnvScreen2WorldY(double y)
    /// Point cnvWorld2Screen(Point wp)
    /// PointD cnvWorld2Screen(PointD wp)
    /// Point cnvScreen2World(Point sp)
    /// PointD cnvScreen2World(PointD sp)
    /// double world2screenXlength(double x)
    /// double world2screenYlength(double y)
    /// double screen2worldXlength(double x)
    /// double screen2worldYlength(double y)
    /// ---  ワールド座標系での図形描画
    /// void drawWPoint(PointD p, int size = 1)
    /// void drawWLine(Line l)
    /// void drawWLine(LineD l)
    /// void drawWLine(Point lps, Point lpe)
    /// void drawWLine(PointD lps, PointD lpe)
    /// void drawWArc(Point center, double radius, double startAngle, double endAngle)
    /// void drawWArc(PointD center, double radius, double startAngle, double endAngle)
    /// void drawWArcSub(Point center, double radius, double startAngle, double endAngle)
    /// void drawWArcSub(PointD center, double radius, double startAngle, double endAngle)
    /// void drawWCircle(Point ctr, double radius)
    /// void drawWCircle(PointD ctr, double radius)
    /// void drawWRectangle(Rect rect, double rotate = 0.0
    /// void drawWRectangle(PointD ps, PointD pe, double rotate = 0.0)
    /// List<PointD> toPointList(PointD ps, PointD pe, double rotate = 0.0)
    /// void drawWPolygon(List<Point> wpList)
    /// void drawWPolygon(List<PointD> wpList)
    /// void drawWText(string text, PointD p, double rotate = 0.0)
    /// void drawWText(string text, Point p, double rotate = 0.0)
    /// void drawWText(string text, PointD p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// void drawWText(string text, Point p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// ---  文字列パラメータ処理
    /// List<Point> toPointList(Point ps, Point pe, double rotate = 0.0)
    /// Box textBoxArea(string text, Point p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// Point transformPoint(Point po, Size size, double rotate)
    /// Point rotatePoint(Point po, Point ctr, double rotate)
    /// Point rotateOrg(Point po, double rotate)
    /// Size measureWText(string text)
    /// 
    /// </summary>
    public class YWorldDraw : YDraw
    {
        public Box mWorld;                      //  ワールド座標
        public bool mClipping = false;          //  クリッピングの有無
        public bool mInvert = false;            //  倒立
        public bool mAspectFix = true;          //  アスペクト比固定
        public bool mTextOverWrite = true;      //  文字が重なった時に前の文字を透かす

        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">Canvas</param>
        public YWorldDraw(Canvas c)
        {
            mCanvas = c;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">Canvas</param>
        /// <param name="viewWidth">Viewの幅</param>
        /// <param name="viewHeight">Viewの高さ</param>
        public YWorldDraw(Canvas c, double viewWidth, double viewHeight)
        {
            mCanvas = c;
            mView.Width = viewWidth;
            mView.Height = viewHeight;
            mWorld = new Box(mView);
        }

        //  ---  ViewとWorldの領域設定

        /// <summary>
        /// Viewの設定
        /// </summary>
        /// <param name="left">左座標</param>
        /// <param name="top">上座標</param>
        /// <param name="right">右座標</param>
        /// <param name="bottom">下座標</param>
        public void setViewArea(double left, double top, double right, double bottom)
        {
            //  Rectにデータを入れるとleft<right, top<bottomの関係に補正される
            mView = new Rect(new Point(left, top), new Point(right, bottom));
            if (mWorld == null)
                mWorld = new Box(mView);
            if (mAspectFix)
                aspectFix();
        }

        /// <summary>
        /// WorldWindowの設定
        /// </summary>
        /// <param name="left">左座標</param>
        /// <param name="top">上座標</param>
        /// <param name="right">右座標</param>
        /// <param name="bottom">下座標</param>
        public void setWorldWindow(double left, double top, double right, double bottom)
        {
            //  Rectにデータを入れるとleft<right, top<bottomの関係に補正される
            mWorld = new Box(new Point(left, top), new Point(right, bottom));
            mInvert = top < bottom;
            if (mAspectFix)
                aspectFix();
        }

        /// <summary>
        /// 文字サイズをワールド座標で設定
        /// </summary>
        /// <param name="size">文字サイズ</param>
        public void setWorldTextSize(double size)
        {
            mTextSize = Math.Abs(world2screenYlength(size));
        }

        /// <summary>
        /// 文字サイズをワールド座標で取得
        /// </summary>
        /// <returns>文字サイズ</returns>
        public double getWorldTextSize()
        {
            return screen2worldYlength(mTextSize);
        }

        /// <summary>
        /// 論理座標のアスペクト比を1にするように論理座標を変更する
        /// 常にビューポート内に収まるようにする
        /// </summary>
        private void aspectFix()
        {
            //  ビューポートに合わせて論理座標の大きさを求める
            double wHeight = mWorld.Width * mView.Height / mView.Width;
            double wWidth = mWorld.Height * mView.Width / mView.Height;
            //  縦横で小さい方を論理座標をビューポートに合わせる
            if (mWorld.Height < wHeight) {
                double dy = Math.Sign(mWorld.Bottom - mWorld.Top) * (wHeight - mWorld.Height);
                mWorld = new Box(new Point(mWorld.Left, mWorld.Top - dy / 2),
                    new Point(mWorld.Right, mWorld.Bottom + dy / 2));
            } else {
                double dx = Math.Sign(mWorld.Right - mWorld.Left) * (wWidth - mWorld.Width);
                mWorld = new Box(new Point(mWorld.Left - dx / 2, mWorld.Top),
                    new Point(mWorld.Right + dx / 2, mWorld.Bottom));
            }
        }

        //  ---  スクリーン座標とワールド座標の相互変換

        /// <summary>
        /// X軸のワールド座標をスクリーン座標に変換する
        /// </summary>
        /// <param name="x">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public double cnvWorld2ScreenX(double x)
        {
            return (x - mWorld.Left) * (mView.Right - mView.Left) / (mWorld.Right - mWorld.Left) + mView.Left;
        }

        /// <summary>
        /// Y軸のワールド座標をスクリーン座標に変換する
        /// </summary>
        /// <param name="y">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public double cnvWorld2ScreenY(double y)
        {
            return (y - mWorld.Top) * (mView.Top - mView.Bottom) / (mWorld.Top - mWorld.Bottom) + mView.Top;
        }

        /// <summary>
        /// X軸のスクリーン座標をワールド座標に変換
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double cnvScreen2WorldX(double x)
        {
            return (x - mView.Left) * (mWorld.Right - mWorld.Left) / (mView.Right - mView.Left) + mWorld.Left;
        }

        /// <summary>
        /// Y軸のスクリーン座標をワールド座標に変換
        /// </summary>
        /// <param name="y">スクリーン座標</param>
        /// <returns>ワールド座標</returns>
        public double cnvScreen2WorldY(double y)
        {
            return (y - mView.Top) * (mWorld.Bottom - mWorld.Top) / (mView.Bottom - mView.Top) + mWorld.Top;
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="wp">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public Point cnvWorld2Screen(Point wp)
        {
            Point sp = new Point();
            sp.X = cnvWorld2ScreenX(wp.X);
            sp.Y = cnvWorld2ScreenY(wp.Y);
            return sp;
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="wp">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public PointD cnvWorld2Screen(PointD wp)
        {
           return new PointD(cnvWorld2ScreenX(wp.x), cnvWorld2ScreenY(wp.y));
        }

        /// <summary>
        /// スクリーン座標を論理座標(ワールド座標)に変換
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public Point cnvScreen2World(Point sp)
        {
            Point wp = new Point();
            wp.X = cnvScreen2WorldX(sp.X);
            wp.Y = cnvScreen2WorldY(sp.Y);
            return wp;
        }

        /// <summary>
        /// スクリーン座標を論理座標(ワールド座標)に変換
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public PointD cnvScreen2World(PointD sp)
        {
            return new PointD(cnvScreen2WorldX(sp.x), cnvScreen2WorldY(sp.y));
        }

        /// <summary>
        /// 論理座標でのX方向の長さからスクリーン座標の長さを求める
        /// </summary>
        /// <param name="x">ワールド座標でのX軸方向長さ</param>
        /// <returns>スクリーン座標での長さ</returns>
        public double world2screenXlength(double x)
        {
            return x * (mView.Right - mView.Left) / (mWorld.Right - mWorld.Left);
        }

        /// <summary>
        /// 論理座標でのY方向の長さからスクリーン座標の長さを求める
        /// </summary>
        /// <param name="y">ワールド座標でのY軸方向の長さ</param>
        /// <returns>スクリーン座標での長さ</returns>
        public double world2screenYlength(double y)
        {
            return y * (mView.Top - mView.Bottom) / (mWorld.Top - mWorld.Bottom);
        }

        /// <summary>
        /// スクリーン座標のX方向長さをワールド座標の長さに変換
        /// </summary>
        /// <param name="x">長さ</param>
        /// <returns></returns>
        public double screen2worldXlength(double x)
        {
            return x * (mWorld.Right - mWorld.Left) / (mView.Right - mView.Left);
        }

        /// <summary>
        /// スクリーン座標のY方向長さをワールド座標の長さに変換
        /// </summary>
        /// <param name="y">長さ</param>
        /// <returns></returns>
        public double screen2worldYlength(double y)
        {
            return y * (mWorld.Top - mWorld.Bottom) / (mView.Top - mView.Bottom);
        }

        //  ---  ワールド座標系での図形描画

        /// <summary>
        /// 点の描画
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="size">サイズ(screensize)</param>
        public void drawWPoint(PointD p, int size = 1)
        {
            PointD ps = cnvWorld2Screen(p);
            if (mClipping) {
                if (!ps.isInside(mView))
                    return;
            }
            drawPoint(ps, size);
        }


        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="l">線分データ</param>
        public void drawWLine(Line l)
        {
            drawWLine(new PointD(l.X1, l.Y1), new PointD(l.X2, l.Y2));
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="l">線分データ</param>
        public void drawWLine(LineD l)
        {
            drawWLine(new Point(l.ps.x, l.ps.y), new Point(l.pe.x, l.pe.y));
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public void drawWLine(Point lps, Point lpe)
        {
            Point ps = cnvWorld2Screen(lps);
            Point pe = cnvWorld2Screen(lpe);
            if (mClipping) {
                LineD l = new LineD(ps, pe);
                LineD cl = l.clippingLine(mView);
                if (cl != null)
                    drawLine(cl.ps, cl.pe);
            } else {
                drawLine(ps, pe);
            }
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public void drawWLine(PointD lps, PointD lpe)
        {
            PointD ps = cnvWorld2Screen(lps);
            PointD pe = cnvWorld2Screen(lpe);
            if (mClipping) {
                LineD l = new LineD(ps, pe);
                LineD cl = l.clippingLine(mView);
                if (cl != null)
                    drawLine(cl.ps, cl.pe);
            } else {
                drawLine(ps, pe);
            }
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        public void drawWArc(Point center, double radius, double startAngle, double endAngle)
        {
            drawWArc(new PointD(center), radius, startAngle, endAngle);
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        public void drawWArc(PointD center, double radius, double startAngle, double endAngle)
        {
            if (2 * Math.PI <= Math.Abs(endAngle - startAngle))
                drawWCircle(center, radius);
            else {
                if (mClipping) {
                    if (mWorld.insideChk(center, radius, startAngle, endAngle)) {
                        drawWArcSub(center, radius, startAngle, endAngle);
                    } else {
                        double sr = world2screenXlength(radius);
                        if (20 < sr) {
                            int div = sr < 100 ? 8 : (sr < 400 ? 32 : 64);  //円弧をポリゴンに変換する時の分割数
                            List<PointD> plist = ylib.devideArcList(center, radius, startAngle, endAngle, div);
                            plist = mWorld.clipPolygon2PolygonList(plist);
                            drawWPolygon(plist);
                        }
                    }
                } else {
                    drawWArcSub(center, radius, startAngle, endAngle);
                }
            }
        }


        /// <summary>
        /// 円弧を楕円弧に変換して表示
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角(rad)</param>
        /// <param name="endAngle">終了角(rad)</param>
        private void drawWArcSub(Point center, double radius, double startAngle, double endAngle)
        {
            drawWArcSub(new PointD(center), radius, startAngle, endAngle);
        }

        /// <summary>
        /// 円弧を楕円弧に変換して表示
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角(rad)</param>
        /// <param name="endAngle">終了角(rad)</param>
        private void drawWArcSub(PointD center, double radius, double startAngle, double endAngle)
        {
            //  円の大きさ
            Size size = new Size(Math.Abs(world2screenXlength(radius)), Math.Abs(world2screenYlength(radius)));     //  X軸半径,Y軸半径
                                                                                                                    //  始点座標
            PointD startPoint = new PointD(radius * Math.Cos(startAngle), Math.Sign(mWorld.Top - mWorld.Bottom) * radius * Math.Sin(startAngle));
            startPoint.Offset(center.x, center.y);
            startPoint = cnvWorld2Screen(startPoint);
            //  終点座標
            PointD endPoint = new PointD(radius * Math.Cos(endAngle), Math.Sign(mWorld.Top - mWorld.Bottom) * radius * Math.Sin(endAngle));
            endPoint.Offset(center.x, center.y);
            endPoint = cnvWorld2Screen(endPoint);

            bool isLarge = (endAngle - startAngle) > Math.PI ? true : false; //  180°を超える円弧化かを指定
            drawEllipse(cnvWorld2Screen(center), size, startPoint, endPoint, isLarge, SweepDirection.Counterclockwise, 0);
        }

        /// <summary>
        /// 円の描画
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="radius">半径</param>        
        public void drawWCircle(Point ctr, double radius)
        {
            drawWCircle(new PointD(ctr), radius);
        }

        /// <summary>
        /// 円の描画
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="radius">半径</param>        
        public void drawWCircle(PointD ctr, double radius)
        {
            if (mClipping) {
                if (mWorld.insideChk(ctr, radius)) {
                    drawCircle(cnvWorld2Screen(ctr), world2screenXlength(radius));
                } else if (mWorld.circleInsideChk(ctr, radius)) {
                    drawWRectangle(mWorld.ToRect());
                } else {
                    double sr = world2screenXlength(radius);
                    if (20 < sr) {
                        int div = sr < 100 ? 8 : (sr < 400 ? 32 : 64);
                        List<PointD> plist = mWorld.clipCircle2PolygonList(ctr, radius, div);   //  BOXと円の交点リスト
                        drawWPolygon(plist);
                    }
                }
            } else {
                drawCircle(cnvWorld2Screen(ctr), world2screenXlength(radius));
            }
        }

        /// <summary>
        /// 四角形の描画
        /// </summary>
        /// <param name="rect">四角の座標(左,上,幅,高さ(下向き))</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawWRectangle(Rect rect, double rotate = 0.0)
        {
            drawWRectangle(new PointD(rect.TopLeft), new PointD(rect.BottomRight), rotate);
        }

        /// <summary>
        /// 四角形の描画
        /// </summary>
        /// <param name="ps">四角形の端点座標</param>
        /// <param name="pe">四角形の端点座標</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawWRectangle(PointD ps, PointD pe, double rotate = 0.0)
        {
            if (mClipping) {
                List<PointD> plist = toPointList(ps, pe, rotate);
                plist = mWorld.clipPolygon2PolygonList(plist);
                drawWPolygon(plist);
            } else {
                Rect rect = new Rect(cnvWorld2Screen(ps).toPoint(), cnvWorld2Screen(pe).toPoint());
                drawRectangle(rect, rotate);
            }
        }

        /// <summary>
        /// 2点指定のBoxの頂点リストを求める
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <returns>座標点リスト</returns>
        private List<PointD> toPointList(PointD ps, PointD pe, double rotate = 0.0)
        {
            Box b = new Box(ps, pe);
            PointD ctr = b.getCenter();
            List<PointD> plist = b.ToPointDList();
            for (int i = 0; i < plist.Count; i++)
                plist[i].rotatePoint(ctr, rotate);
            return plist;
        }


        /// <summary>
        /// ポリゴンの描画
        /// </summary>
        /// <param name="wpList">点座標リスト</param>
        public void drawWPolygon(List<Point> wpList)
        {
            List<Point> pointList = new List<Point>();
            foreach (Point p in wpList) {
                pointList.Add(cnvWorld2Screen(p));
            }
            drawPolygon(pointList);
        }


        /// <summary>
        /// ポリゴンの描画
        /// </summary>
        /// <param name="wpList">点座標リスト</param>
        public void drawWPolygon(List<PointD> wpList)
        {
            List<PointD> pointList = new List<PointD>();
            foreach (PointD p in wpList) {
                pointList.Add(cnvWorld2Screen(p));
            }
            drawPolygon(pointList);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">文字位置(左上)</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawWText(string text, PointD p, double rotate = 0.0)
        {
            drawWText(text, p.toPoint(), rotate);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">文字位置(左上)</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawWText(string text, Point p, double rotate = 0.0)
        {
            if (mClipping) {
                if (!mWorld.insideChk(textBoxArea(text, p, rotate, HorizontalAlignment.Left, VerticalAlignment.Top)))
                    return;
            }
            drawText(text, cnvWorld2ScreenX(p.X), cnvWorld2ScreenY(p.Y), rotate);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">文字位置</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        public void drawWText(string text, PointD p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            drawWText(text, p.toPoint(), rotate, ha, va);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">文字位置</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        public void drawWText(string text, Point p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            if (mClipping) {
                if (!mWorld.insideChk(textBoxArea(text, p, rotate, ha, va)))
                    return;
            }
            if (!mTextOverWrite) {
                //  文字列の領域をRectangleで白で塗潰す
                Point op = getTextOrg(text, cnvWorld2Screen(p), rotate, ha, va);
                Size size = measureWText(text);
                op = transformPoint(cnvScreen2World(op), size, rotate);
                Brush col = mBrush;             //  要素の色
                mFillColor = Brushes.White;
                mBrush = Brushes.White;
                drawWRectangle(new Rect(op, size), rotate);
                mBrush = col;
            }
            drawText(text, cnvWorld2Screen(p), rotate, ha, va);
        }

        //  ---  文字列パラメータ処理

        /// <summary>
        /// テキスト領域を求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">原点</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        /// <returns>Box領域</returns>
        private Box textBoxArea(string text, Point p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            return textBoxArea(text, new PointD(p), rotate, ha, va);
        }

        /// <summary>
        /// テキスト領域を求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">原点</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        /// <returns>Box領域</returns>
        private Box textBoxArea(string text, PointD p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            PointD op = cnvWorld2Screen(p);
            if (ha != HorizontalAlignment.Left && va != VerticalAlignment.Top)
                op = getTextOrg(text, cnvWorld2Screen(p), rotate, ha, va);
            Size size = measureWText(text);
            Box b = new Box(cnvScreen2World(op), size);
            b.rotateArea(op, rotate);
            return b;
        }

        /// <summary>
        /// 文字列の回転(左上原点)に合わせてRectangleを回転(中心原点)させるための原点を求める
        /// RectAngleを左上起点に回転させるための原点移動
        /// (Rectangleの回転は中心を原点として回る)
        /// </summary>
        /// <param name="po">回転原点(左上)</param>
        /// <param name="size">Rectangleの大きさ</param>
        /// <param name="rotate">回転角度</param>
        /// <returns>補正した起点</returns>
        private Point transformPoint(Point po, Size size, double rotate)
        {
            Point cp = new Point(po.X + size.Width / 2.0, po.Y - size.Height / 2.0);
            cp = rotatePoint(cp, po, -rotate);
            Point tp = new Point(cp.X - size.Width / 2.0, cp.Y - size.Height / 2.0);
            return tp;
        }

        /// <summary>
        /// 回転中心を指定して回転
        /// </summary>
        /// <param name="po">対象座標</param>
        /// <param name="ctr">回転の中心座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        /// <returns>回転後の座標</returns>
        private Point rotatePoint(Point po, Point ctr, double rotate)
        {
            Point p = new Point(po.X - ctr.X, po.Y - ctr.Y);
            p = rotateOrg(p, rotate);
            return new Point(p.X + ctr.X, p.Y + ctr.Y);
        }

        /// <summary>
        /// 原点を中心に回転
        /// </summary>
        /// <param name="po">対象座標</param>
        /// <param name="rotate">回転角度(rad)</param>
        /// <returns>回転後の座標</returns>
        private Point rotateOrg(Point po, double rotate)
        {
            Point p = new Point();
            p.X = po.X * Math.Cos(rotate) - po.Y * Math.Sin(rotate);
            p.Y = po.X * Math.Sin(rotate) + po.Y * Math.Cos(rotate);
            return p;
        }

        /// <summary>
        /// 文字列の大きさを取得する
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>文字列の大きさ(Size.Width/Height)</returns>
        public Size measureWText(string text)
        {
            Size size = measureText(text);
            //  YDrawingShapsから取得した大きさをワールド座標の大きさに逆変換する
            size.Width  = size.Width  / Math.Abs(world2screenXlength(1));
            size.Height = size.Height / Math.Abs(world2screenYlength(1));

            return size;
        }

    }
}
