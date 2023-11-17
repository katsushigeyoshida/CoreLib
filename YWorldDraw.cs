using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
    /// void setWorldWindow(Box area)                               WorldWindowの設定
    /// void setWorldWindow()                                       WorldWindowを再計算
    /// void setWorldZoom(PointD wp, double zoom, bool inverse = false) 指定した座標を中心にしてスケーリング
    /// void setWorldZoom(double zoom)                              領域の中心からスケーリング
    /// void setWorldOffset(PointD vec)                             WorldWindowを移動
    /// void setWorldTextSize(double size)                          文字サイズをワールド座標で設定
    /// double getWorldTextSize()                                   文字サイズをワールド座標で取得
    /// void aspectFix()                                            論理座標のアスペクト比を1にする
    /// ---  スクリーン座標とワールド座標の相互変換
    /// double cnvWorld2ScreenX(double x)                   X軸のワールド座標をスクリーン座標に変換
    /// double cnvWorld2ScreenY(double y)                   Y軸のワールド座標をスクリーン座標に変換
    /// double cnvScreen2WorldX(double x)                   X軸のスクリーン座標をワールド座標に変換
    /// double cnvScreen2WorldY(double y)                   Y軸のスクリーン座標をワールド座標に変換
    /// PointD cnvWorld2Screen(PointD wp)                   論理座標(ワールド座標)をスクリーン座標に変換
    /// LineD cnvWorld2Screen(LineD wl)                     論理座標(ワールド座標)をスクリーン座標に変換
    /// ArcD cnvWorld2Screen(ArcD warc)                     論理座標(ワールド座標)をスクリーン座標に変換
    /// PointD cnvScreen2World(PointD sp)                   スクリーン座標を論理座標(ワールド座標)に変換
    /// double world2screenXlength(double x)                論理座標でのX方向の長さからスクリーン座標の長さを求める
    /// double world2screenYlength(double y)                論理座標でのY方向の長さからスクリーン座標の長さを求める
    /// double screen2worldXlength(double x)                スクリーン座標のX方向長さをワールド座標の長さに変換
    /// double screen2worldYlength(double y)                スクリーン座標のY方向長さをワールド座標の長さに変換
    /// ---  ワールド座標系での図形描画
    /// void drawWPoint(PointD p)                           点の描画
    /// void drawWLine(LineD l)                             線分の描画
    /// void drawWLine(PointD lps, PointD lpe)              線分の描画
    /// void drawWArc(PointD center, double radius, double startAngle, double endAngle, bool close = true)  円弧の描画
    /// void drawWArc(ArcD arc, bool close = true)          円弧の描画
    /// void drawWArcSub(PointD center, double radius, double startAngle, double endAngle, bool close = true)   円弧を楕円弧に変換して表示
    /// void drawWCircle(PointD ctr, double radius, bool close = true)  円の描画
    /// void drawWEllipse(EllipseD ellipse)                 楕円の描画
    /// void drawWRectangle(Rect rect, double rotate = 0.0) 四角形の描画
    /// void drawWRectangle(Box box, double rotate = 0.0)   四角形の描画
    /// void drawWRectangle(PointD ps, PointD pe, double rotate = 0.0)  四角形の描画
    /// List<PointD> toPointList(PointD ps, PointD pe, double rotate = 0.0) 2点指定のBoxの頂点リストを求める
    /// void drawWPolyline(PolylineD polyline)              ポリラインの描画
    /// void drawWPolyline(List<PointD> wpList)             ポリラインの描画
    /// void drawWPolygon(PolygonD polygon, bool fill = true)   ポリゴンの描画(閉領域)
    /// void drawWPolygon(List<PointD> wpList, bool fill = true)    ポリゴンの描画(閉領域)
    /// void drawWText(TextD text)                          文字列の描画(複数行対応)
    /// void drawWText(string text, PointD p, double textSize = 0, double rotate = 0, 
    ///     HorizontalAlignment ha = HorizontalAlignment.Left, VerticalAlignment va = VerticalAlignment.Top)    文字列の描画
    /// ---  文字列パラメータ処理
    /// List<PointD> textBoxArea(string text, PointD p, double rotate, HorizontalAlignment ha, VerticalAlignment va)    テキスト領域を求める
    /// Size measureWText(string text)                      文字列の大きさを取得
    /// 
    /// </summary>
    public class YWorldDraw : YDraw
    {
        public Box mWorld;                      //  ワールド座標
        public Box mClipBox;                    //  クリッピング領域
        public bool mClipping = false;          //  クリッピングの有無
        public bool mInvert = false;            //  倒立
        public bool mAspectFix = true;          //  アスペクト比固定
        public bool mTextOverWrite = true;      //  文字が重なった時に前の文字を透かす

        private double mEps = 1e-8;

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
            if (mWorld == null || mWorld.Height == 0 || double.IsNaN(mWorld.Width))
                mWorld = new Box(mView);
            if (mAspectFix)
                aspectFix();
            mClipBox = mWorld.toCopy();
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
            setWorldWindow(new Box(new PointD(left, top), new PointD(right, bottom)));
        }

        /// <summary>
        /// WorldWindowの設定
        /// </summary>
        /// <param name="area">Box領域</param>
        public void setWorldWindow(Box area)
        {
            //  Rectにデータを入れるとleft<right, top<bottomの関係に補正される
            mWorld = area.toCopy();
            if (mWorld.Width == 0 && mWorld.Height ==0)
                mWorld.Size = new Size(1,1);
            if (mWorld.Width == 0)
                mWorld.Width = mWorld.Height;
            if (mWorld.Height == 0)
                mWorld.Height = mWorld.Width;
            mInvert = mWorld.Top < mWorld.Bottom;   //  上下の向き
            if (mAspectFix)
                aspectFix();
            mClipBox = mWorld.toCopy();
        }

        /// <summary>
        /// WorldWindowを再計算
        /// </summary>
        public void setWorldWindow()
        {
            //  Rectにデータを入れるとleft<right, top<bottomの関係に補正される
            if (mWorld == null)
                mWorld = new Box(mView);
            mInvert = mWorld.Top < mWorld.Bottom;   //  上下の向き
            if (mAspectFix)
                aspectFix();
            mClipBox = mWorld.toCopy();
        }

        /// <summary>
        /// 指定した座標を中心にしてスケーリングする
        /// inverseを指定した場合中心点で反転した位置を基準に拡大縮小する
        /// </summary>
        /// <param name="wp">スケーリングの中心座標</param>
        /// <param name="zoom">倍率</param>
        /// <param name="inverse">反転</param>
        public void setWorldZoom(PointD wp, double zoom, bool inverse = false)
        {
            mWorld.zoom(wp, zoom, inverse);
            mClipBox = mWorld.toCopy();
        }

        /// <summary>
        /// 領域の中心からスケーリングする
        /// </summary>
        /// <param name="zoom">倍率</param>
        public void setWorldZoom(double zoom)
        {
            mWorld.zoom(zoom);
            mClipBox = mWorld.toCopy();
        }

        /// <summary>
        /// WorldWindowを移動する
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        public void setWorldOffset(PointD vec)
        {
            mWorld.offset(vec);
            mClipBox = mWorld.toCopy();
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
            return Math.Abs(screen2worldYlength(mTextSize));
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
                mWorld = new Box(new PointD(mWorld.Left, mWorld.Top - dy / 2),
                    new PointD(mWorld.Right, mWorld.Bottom + dy / 2));
            } else {
                double dx = Math.Sign(mWorld.Right - mWorld.Left) * (wWidth - mWorld.Width);
                mWorld = new Box(new PointD(mWorld.Left - dx / 2, mWorld.Top),
                    new PointD(mWorld.Right + dx / 2, mWorld.Bottom));
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
        public PointD cnvWorld2Screen(PointD wp)
        {
           return new PointD(cnvWorld2ScreenX(wp.x), cnvWorld2ScreenY(wp.y));
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="wl">ワールド座標<</param>
        /// <returns>スクリーン座標</returns>
        public LineD cnvWorld2Screen(LineD wl)
        {
            return new LineD(cnvWorld2Screen(wl.ps), cnvWorld2Screen(wl.pe));
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="warc">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public ArcD cnvWorld2Screen(ArcD warc)
        {
            ArcD arc = new ArcD(cnvWorld2Screen(warc.mCp), world2screenXlength(warc.mR));
            arc.mSa = mInvert ? warc.mSa : Math.PI * 2 - warc.mEa;
            arc.mEa = mInvert ? warc.mEa : Math.PI * 2 - warc.mSa;
            return arc;
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="wrect">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public Rect cnvWorld2Screen(Rect wrect)
        {
            return new Rect(cnvWorld2ScreenX(wrect.X), cnvWorld2ScreenY(wrect.Y),
                world2screenXlength(wrect.Width), world2screenYlength(wrect.Height));
        }

        /// <summary>
        /// 論理座標(ワールド座標)をスクリーン座標に変換する
        /// </summary>
        /// <param name="wbox">ワールド座標</param>
        /// <returns>スクリーン座標</returns>
        public Rect cnvWorld2Screen(Box wbox)
        {
            return new Rect(cnvWorld2Screen(wbox.TopLeft).toPoint(),
                                cnvWorld2Screen(wbox.BottomRight).toPoint());
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
            return y * (mView.Bottom - mView.Top) / (mWorld.Top - mWorld.Bottom);
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
        /// 形状 type : 0=dot 1:cross 2:plus 3:box 4: Circle
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="size">サイズ(screensize)</param>
        public void drawWPoint(PointD p)
        {
            if (mClipping) {
                if (!mClipBox.insideChk(p))
                    return;
            }
            drawPoint(cnvWorld2Screen(p));
        }

        /// <summary>
        /// 線分の描画(線種 0:実線 1:破線 2:一点鎖線 2:二点鎖線)
        /// </summary>
        /// <param name="l">線分データ</param>
        public void drawWLine(LineD l)
        {
            if (mClipping) {
                List<LineD> lines = mClipBox.clipLineList(l);
                for (int i = 0; i < lines.Count; i++)
                    drawLine(cnvWorld2Screen(lines[i]));
            } else {
                drawLine(cnvWorld2Screen(l));
            }
        }

        /// <summary>
        /// 線分の描画(線種 0:実線 1:破線 2:一点鎖線 2:二点鎖線)
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public void drawWLine(PointD lps, PointD lpe)
        {
            drawWLine(new LineD(lps, lpe));
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        /// <param name="close">閉領域)</param>
        public void drawWArc(PointD center, double radius, double startAngle, double endAngle, bool close = true)
        {
            drawWArc(new ArcD(center, radius, startAngle, endAngle), close);
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="arc">ArcD</param>
        /// <param name="close">閉領域)</param>
        public void drawWArc(ArcD arc, bool close = true)
        {
            if (2 * Math.PI <= Math.Abs(arc.mOpenAngle) + mEps) {
                //  円の描画
                drawWCircle(arc.mCp, arc.mR, close);
            } else {
                //  円弧の描画
                if (mClipping) {
                    //  クリッピングあり
                    if (mClipBox.insideChk(arc)) {
                        //  クリッピング処理なし
                        if (mLineType == 0) {
                            if (close)
                                drawWPolygonArc(arc);
                            else
                                drawWArcSub(arc.mCp, arc.mR, arc.mSa, arc.mEa, close);
                        } else
                            drawArc(cnvWorld2Screen(arc));
                    } else {
                        //  クリッピング処理あり
                        if (close) {
                            drawWPolygonArc(arc);
                        } else {
                            Brush tmpFillColor = mFillColor;
                            if (!close)
                                mFillColor = null;
                            drawWDivArc(arc);
                            mFillColor = tmpFillColor;
                        }
                    }
                } else {
                    drawWArcSub(arc.mCp, arc.mR, arc.mSa, arc.mEa, close);
                }
            }
        }

        /// <summary>
        /// 円弧をポリゴンで表示(塗潰し可)
        /// </summary>
        /// <param name="arc">円弧データ</param>
        private void drawWPolygonArc(ArcD arc)
        {
            double sr = world2screenXlength(arc.mR);
            if (2 < sr) {
                int div = sr < 20 ? 8 : (sr < 50 ? 32 : (sr < 150 ? 64 : 128));  //円弧をポリゴンに変換する時の分割数
                List<PointD> plist = arc.toPointList(div);
                plist = mClipBox.clipPolygonList(plist);
                drawWPolygon(plist);
            }
        }

        /// <summary>
        /// 円弧を実線以外の対応して複数の円弧で表示(塗潰し不可)
        /// </summary>
        /// <param name="arc">円弧データ</param>
        private void drawWDivArc(ArcD arc)
        {
            double sr = world2screenXlength(arc.mR);
            if (2 < sr) {
                List<ArcD> alist = mClipBox.clipArcList(arc);
                for (int i = 0; i < alist.Count; i++) {
                    drawArc(cnvWorld2Screen(alist[i]));
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
        /// <param name="close">閉領域)</param>
        private void drawWArcSub(PointD center, double radius, double startAngle, double endAngle, bool close = true)
        {
            Brush tmpFillColor = mFillColor;
            if (!close)
                mFillColor = null;
            //  円の大きさ
            Size size = new Size(Math.Abs(world2screenXlength(radius)), Math.Abs(world2screenYlength(radius)));     //  X軸半径,Y軸半径
                                                                                                                    //  始点座標
            PointD startPoint = new PointD(radius * Math.Cos(startAngle), Math.Sign(mWorld.Top - mWorld.Bottom) * radius * Math.Sin(startAngle));
            startPoint.offset(center.x, center.y);
            startPoint = cnvWorld2Screen(startPoint);
            //  終点座標
            PointD endPoint = new PointD(radius * Math.Cos(endAngle), Math.Sign(mWorld.Top - mWorld.Bottom) * radius * Math.Sin(endAngle));
            endPoint.offset(center.x, center.y);
            endPoint = cnvWorld2Screen(endPoint);

            bool isLarge = (endAngle - startAngle) > Math.PI ? true : false; //  180°を超える円弧化かを指定
            drawEllipse(cnvWorld2Screen(center), size, startPoint, endPoint, isLarge, SweepDirection.Counterclockwise, 0);
            mFillColor = tmpFillColor;
        }

        /// <summary>
        /// 円の描画
        /// </summary>
        /// <param name="ctr">中心座標</param>
        /// <param name="radius">半径</param>        
        /// <param name="close">閉領域)</param>
        public void drawWCircle(PointD ctr, double radius, bool close = true)
        {
            Brush tmpFillColor = mFillColor;
            if (!close)
                mFillColor = null;
            if (mClipping) {
                if (mClipBox.insideChk(ctr, radius * 2)) {
                    //  Box内
                    if (mLineType == 0)
                        drawCircle(cnvWorld2Screen(ctr), world2screenXlength(radius));
                    else
                        drawArc(new ArcD(cnvWorld2Screen(ctr), world2screenXlength(radius)));
                } else if (mClipBox.circleInsideChk(ctr, radius)) {
                    // Boxが円の内側
                    if (close)          //  Box内すべて塗潰し
                        drawWRectangle(mClipBox.ToRect());
                } else {
                    double sr = world2screenXlength(radius);
                    if (2 < sr) {
                        ArcD arc = new ArcD(ctr, radius);
                        if (close) {
                            int div = sr < 20 ? 8 : (sr < 50 ? 16 : (sr < 150 ? 32 : (sr < 300 ? 64 : 128)));  //円弧をポリゴンに変換する時の分割数
                            List<PointD> plist = arc.toPointList(div);
                            plist = mClipBox.clipPolygonList(plist);
                            drawWPolygon(plist);
                        } else {
                            List<ArcD> alist = mClipBox.clipArcList(arc);
                            for (int i = 0; i < alist.Count; i++) {
                                drawArc(cnvWorld2Screen(alist[i]));
                            }
                        }
                    }
                }
            } else {
                drawCircle(cnvWorld2Screen(ctr), world2screenXlength(radius));
            }
            mFillColor = tmpFillColor;
        }

        /// <summary>
        /// 楕円の描画
        /// </summary>
        /// <param name="ellipse">EllipseD</param>
        public void drawWEllipse(EllipseD ellipse, bool close = false)
        {
            Brush tmpFillColor = mFillColor;
            if (!close)
                mFillColor = null;
            if (mLineType == 0 && (mClipping && mClipBox.insideChk(ellipse))) {
                if (Math.PI * 2 <= ellipse.mOpenAngle) {
                    //  楕円
                    Box b = ellipse.getBox();
                    PointD pos = cnvWorld2Screen(b.BottomLeft);
                    double rotate = Math.PI * 2 - ellipse.mRotate;
                    drawOval(pos.x, pos.y, world2screenXlength(b.Width), world2screenYlength(b.Height), rotate);
                } else {
                    //  楕円弧
                    PointD cp = cnvWorld2Screen(ellipse.mCp);
                    double rx = world2screenXlength(ellipse.mRx);
                    double ry = world2screenYlength(ellipse.mRy);
                    double ea = Math.PI * 2 - ellipse.mSa;
                    double sa = Math.PI * 2 - ellipse.mEa;
                    ea += ea < sa ? Math.PI * 2 : 0;
                    double rotate = Math.PI * 2 - ellipse.mRotate;
                    if (close) {
                        drawWPolygonEllipse(ellipse);
                    } else {
                        drawEllipse(cp.x, cp.y, rx, ry, sa, ea, rotate);
                    }
                }
            } else if (ellipse.insideChk(mClipBox)) {
                if (close)          //  Box内すべて塗潰し
                    drawWRectangle(mClipBox.ToRect());
            } else {
                //  実線以外またはクリッピング有
                if (close)
                    drawWPolygonEllipse(ellipse);
                else
                    drawWPolylineEllipse(ellipse);
            }
            mFillColor = tmpFillColor;
        }

        /// <summary>
        /// 楕円をポリゴンで表示(塗潰し可)
        /// </summary>
        /// <param name="ellipse">楕円データ</param>
        private void drawWPolygonEllipse(EllipseD ellipse)
        {
            double sr = world2screenXlength(ellipse.mRx > ellipse.mRy ? ellipse.mRx : ellipse.mRy);
            if (2 < sr) {
                int div = sr < 20 ? 16 : (sr < 50 ? 32 : (sr < 150 ? 64 : 128));  //円弧をポリゴンに変換する時の分割数
                List<PointD> plist = ellipse.toPointList(div);
                drawWPolygon(plist);
            }
        }

        /// <summary>
        /// 楕円をポリラインで表示(塗潰し不可)
        /// </summary>
        /// <param name="ellipse">楕円データ</param>
        private void drawWPolylineEllipse(EllipseD ellipse)
        {
            double sr = world2screenXlength(ellipse.mRx > ellipse.mRy ? ellipse.mRx : ellipse.mRy);
            if (2 < sr) {
                int div = sr < 20 ? 16 : (sr < 50 ? 32 : (sr < 150 ? 64 : 128));  //円弧をポリゴンに変換する時の分割数
                List<PointD> plist = ellipse.toPointList(div);
                drawWPolyline(plist);
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
        /// <param name="box">Boxデータ</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawWRectangle(Box box, double rotate = 0.0)
        {
            drawWRectangle(box.TopLeft, box.BottomRight, rotate);
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
                plist = mClipBox.clipPolygonList(plist);
                mClipping = false;
                drawWPolygon(plist);
                mClipping = true;
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
            List<PointD> plist = b.ToPointList();
            for (int i = 0; i < plist.Count; i++)
                plist[i].rotate(ctr, rotate);
            return plist;
        }


        /// <summary>
        /// ポリラインの描画
        /// </summary>
        /// <param name="wpList">ポリライン</param>
        public void drawWPolyline(PolylineD polyline)
        {
            drawWPolyline(polyline.mPolyline);
        }

        /// <summary>
        /// ポリラインの描画
        /// </summary>
        /// <param name="wpList">座標リスト</param>
        public void drawWPolyline(List<PointD> wpList)
        {
            if (mClipping) {
                for (int i = 0; i < wpList.Count - 1; i++) {
                    drawWLine(new LineD(wpList[i], wpList[i + 1]));
                }
            } else {
                List<PointD> pointList = wpList.ConvertAll(p => cnvWorld2Screen(p));
                drawPolyline(pointList);
            }
        }

        /// <summary>
        /// ポリゴンの描画(閉領域)
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <param name="fill">塗潰し</param>
        public void drawWPolygon(PolygonD polygon, bool fill = true)
        {
            drawWPolygon(polygon.mPolygon, fill);
        }

        /// <summary>
        /// ポリゴンの描画(閉領域)
        /// </summary>
        /// <param name="wpList">点座標リスト</param>
        /// <param name="fill">塗り潰しの可否</param>
        public void drawWPolygon(List<PointD> wpList, bool fill = true)
        {
            List<PointD> pointList = new List<PointD>();
            if (!fill) {
                //  塗り潰しをしない場合ポリラインで表示
                pointList.AddRange(wpList);
                pointList.Add(wpList[0]);
                drawWPolyline(pointList);
                return;
            }
            if (mClipping) {
                if (!mClipBox.insideChk(wpList)) {
                    pointList = mClipBox.intersection(wpList);
                    if (pointList.Count == 0 && mClipBox.polygonInsideChk(wpList)) {
                        //  Boxがポリゴン領域内(Boxの頂点リストに変換)
                        pointList = mClipBox.ToPointList();
                        pointList = pointList.ConvertAll(p => cnvWorld2Screen(p));
                    } else {
                        //  Boxとポリゴンが交点を持っている
                        List<PointD> plist = mClipBox.clipPolygonList(wpList);
                        pointList = plist.ConvertAll(p => cnvWorld2Screen(p));
                    }
                } else {
                    //  すべての座標がBox領域内でクリッピングなし
                    pointList = wpList.ConvertAll(p => cnvWorld2Screen(p));
                }
            } else {
                //  クリッピングなし
                pointList = wpList.ConvertAll(p => cnvWorld2Screen(p));
            }
            drawPolygon(pointList);
        }

        /// <summary>
        /// 文字列の描画(複数行対応)
        /// </summary>
        /// <param name="text">TextD</param>
        public void drawWText(TextD text)
        {
            mTextSize = Math.Abs(world2screenYlength(text.mTextSize));
            string[] multiText = text.mText.Split(new char[] { '\n' });
            for (int i = 0; i < multiText.Length; i++) {
                TextD ttext = text.toCopy();
                ttext.mText = multiText[i].TrimEnd('\r');
                ttext.mPos += ttext.mPos.vector(ttext.mRotate - Math.PI / 2, i * ttext.mTextSize * ttext.mLinePitchRate);
                drawWText(ttext.mText, ttext.mPos, ttext.mTextSize, ttext.mRotate, ttext.mHa, ttext.mVa);
            }
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="p">文字位置</param>
        /// <param name="textSize">文字サイズ</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        public void drawWText(string text, PointD p, double textSize = 0, double rotate = 0, 
            HorizontalAlignment ha = HorizontalAlignment.Left, VerticalAlignment va = VerticalAlignment.Top)
        {
            if (textSize != 0)
                setWorldTextSize(textSize);
            if (mClipping) {
                if (mClipBox.outsideChk(new Box(textBoxArea(text, p, rotate, ha, va))))
                    return;
            }
            if (!mTextOverWrite) {
                //  文字列の領域をRectangleで白で塗潰す
                List<PointD> plist = textBoxArea(text, p, rotate, ha, va);
                Brush tmpFillColor = mFillColor;
                Brush tmpColor = mBrush;             //  要素の色
                mFillColor = Brushes.White;
                mBrush = Brushes.White;
                drawWPolygon(textBoxArea(text, new PointD(p), rotate, ha, va));
                mBrush = tmpColor;
                mFillColor = tmpFillColor;
            }
            if (!mInvert)
                rotate *= -1;
            //mTextColor = mBrush;
            drawText(text, cnvWorld2Screen(p), rotate, ha, va);
        }

        /// <summary>
        /// ワールド座標にイメージ表示(左上原点)
        /// </summary>
        /// <param name="bitmapImage">イメージデータ(BitmapImage)</param>
        /// <param name="box">表示領域</param>
        public void drawWImage(BitmapImage bitmapImage, Box box)
        {
            drawImage(bitmapImage, new Rect(cnvWorld2Screen(box.TopLeft).toPoint(),
                                        cnvWorld2Screen(box.BottomRight).toPoint()));
        }

        /// <summary>
        /// ワールド座標にイメージ表示(左上原点)
        /// </summary>
        /// <param name="bitmap">イメージデータ(Bitmap)</param>
        /// <param name="imageBox">表示領域</param>
        public void drawWBitmap(System.Drawing.Bitmap bitmap, Box imageBox, bool clipping = true)
        {
            if (mClipping) {
                if (!mClipBox.insideChk(imageBox) && clipping) {
                    //  Bitmapのトリミングを繰り返すと例外エラーが発生 原因不明
                    imageBox.normalize();
                    imageBox.Bottom = imageBox.Top - imageBox.Width * bitmap.Height / bitmap.Width;
                    Rect imageRect = cnvWorld2Screen(imageBox);
                    Box trimBox = mClipBox.andBox(imageBox);
                    if (trimBox == null) return;
                    Rect trimRect = cnvWorld2Screen(trimBox);
                    double rateW = bitmap.Width / imageRect.Width;
                    double rateH = bitmap.Height / imageRect.Height;
                    double sx = trimRect.X - imageRect.X;
                    double sy = trimRect.Y - imageRect.Y;
                    trimRect.X = (sx > 0 ? sx : 0) * rateW;
                    trimRect.Y = (sy > 0 ? sy : 0) * rateH;
                    trimRect.Width = 0 < sx ? bitmap.Width - trimRect.X : trimRect.Width * rateW;
                    trimRect.Height = 0 < sy ? bitmap.Height - trimRect.Y : trimRect.Height * rateH;
                    System.Drawing.Bitmap clipBitmap = trimingBitmap(bitmap, trimRect);
                    if (clipBitmap == null) return;
                    drawBitmap(clipBitmap, cnvWorld2Screen(trimBox));
                    clipBitmap.Dispose();
                }
                if (!mClipBox.outsideChk(imageBox) && !clipping) {
                    drawBitmap(bitmap, cnvWorld2Screen(imageBox));
                }
                return;
            }
            drawBitmap(bitmap, cnvWorld2Screen(imageBox));
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
        private List<PointD> textBoxArea(string text, PointD p, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            PointD op = p.toCopy();
            PointD cp = p.toCopy();
            //  文字列のサイズを求める
            Size size = measureWText(text);
            size.Width += size.Height * 0.1;
            size.Height *= 1.1;                         //  ベースラインを調整
            //  文字の起点(左上)を求める
            double dx = 0;
            double dy = 0;
            //  アライメントの調整
            if (ha == HorizontalAlignment.Center)
                dx = size.Width / 2;
            else if (ha == HorizontalAlignment.Right)
                dx = size.Width;
            if (va == VerticalAlignment.Center)
                dy = size.Height / 2;
            else if (va == VerticalAlignment.Bottom)
                dy = size.Height;
            op.offset(-dx, dy);
            op.rotate(cp, rotate);
            //  文字領域を回転に合わせてBoxを拡張する
            Box b = new Box(op, size);
            return b.getRoate(op, rotate);
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
