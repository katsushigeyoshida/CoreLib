using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CoreLib
{
    /// <summary>
    /// WFPグラフィックライブラリ
    /// System.Windows.Shapesを使ってグラフィック処理をする
    /// 描画Canvasに対しておこない、それにグラフィック要素を追加して表示する。
    /// 
    /// YDraw(Canvas c)
    /// YDraw(Canvas c, double width, double height)
    /// 
    /// void setViewSize(double width, double height)           ビューサイズを設定
    /// void clear()                                            登録した図形をすべて削除する
    /// string getColorName(Brush color)                        Brush を色名に変換
    /// Brush getColor(string color)                            色名を Brush値に変換
    /// void setBackColor(Brush color)                          キャンパスの背景色を設定
    /// 
    /// void drawPoint(PointD p, int size = 1, int type = 0)    点の描画
    /// void drawLine(Line l)                                   線分の描画
    /// void drawLine(LineD l)
    /// void drawLine(Point ps, Point pe)
    /// void drawLine(PointD ps, PointD pe)
    /// void drawLine(double xs, double ys, double xe, double ye)
    /// void drawArc(double cx, double cy, double radius, double startAngle, double endAngle)   円弧を描画(Pathオブジェクトを使用)
    /// void drawCircle(Point center, double radius)            円の表示
    /// void drawCircle(PointD center, double radius)
    /// void drawCircle(double cx, double cy, double radius)
    /// void drawOval(Point centor, double rx, double ry, double rotate)    楕円の描画
    /// void drawOval(double left, double top, double width, double height, double rotate)  楕円の描画
    /// void drawEllipse(Point centor, double rx, double ry, double startAngle, double endAngle, double rotate = 0.0)   楕円弧を描画(Pathオブジェクトを使用)
    /// void drawEllipse(PointD centor, double rx, double ry, double startAngle, double endAngle, double rotate = 0.0)
    /// void drawEllipse(double cx, double cy, double rx, double ry, double startAngle, double endAngle, double rotate =0.0)
    /// void drawEllipse(Point center, Size size, Point startPoint, Point endPoint, bool isLarge, SweepDirection sweepDirection, double rotate = 0.0)
    /// void drawRectangle(Rect rect, double rotate = 0.0 )     四角形の描画
    /// void drawRectangle(double left, double top, double width, double height, double rotate = 0.0)
    /// void drawPolyline(List<Point> polygonList)              ポリラインの描画
    /// void drawPolyline(List<PointD> polylineList)
    /// void drawPolygon(List<Point> polygonList)               ポリゴンの描画(閉領域を作成)
    /// void drawPolygon(List<PointD> polygonList)
    /// void drawText(string text, Point leftTop, double rotate = 0.0)          文字列の描画
    /// void drawText(string text, double left, double top, double rotate = 0.0)
    /// void drawText(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// 
    /// Point getTextOrg(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)   文字列の左上原点座標を求める
    /// PointD getTextOrg(string text, PointD leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// Size measureText(string text)                           文字列の大きさを求める
    /// virtual double measureTextRatio(string text)            文字列の縦横比を求める(高さ/幅)
    /// 
    /// 
    /// </summary>
    public class YDraw
    {
        public record ColorTitle(string colorTitle, Brush brush);
        //  16色の色パターン
        public List<ColorTitle> m16ColorList = new List<ColorTitle>() {
            new ColorTitle("Black",  Brushes.Black),
            new ColorTitle("Silver", Brushes.Silver),
            new ColorTitle("Gray",   Brushes.Gray),
            new ColorTitle("White",  Brushes.White),
            new ColorTitle("Maroon", Brushes.Maroon),
            new ColorTitle("Red",    Brushes.Red),
            new ColorTitle("Purple", Brushes.Purple),
            new ColorTitle("Fuchsia",Brushes.Fuchsia),
            new ColorTitle("Green",  Brushes.Green),
            new ColorTitle("Lime",   Brushes.Lime),
            new ColorTitle("Olive",  Brushes.Olive),
            new ColorTitle("Yellow", Brushes.Yellow),
            new ColorTitle("Navy",   Brushes.Navy),
            new ColorTitle("Blue",   Brushes.Blue),
            new ColorTitle("Teal",   Brushes.Teal),
            new ColorTitle("Aqua",   Brushes.Aqua)
        };
        //  140色の色パターン
        public List<ColorTitle> mColorList = new List<ColorTitle>() {
            new ColorTitle("AliceBlue",Brushes.AliceBlue),
            new ColorTitle("AntiqueWhite",Brushes.AntiqueWhite),
            new ColorTitle("Aqua",Brushes.Aqua),
            new ColorTitle("Aquamarine",Brushes.Aquamarine),
            new ColorTitle("Azure",Brushes.Azure),
            new ColorTitle("Beige",Brushes.Beige),
            new ColorTitle("Bisque",Brushes.Bisque),
            new ColorTitle("Black",Brushes.Black),
            new ColorTitle("BlanchedAlmond",Brushes.BlanchedAlmond),
            new ColorTitle("Blue",Brushes.Blue),
            new ColorTitle("BlueViolet",Brushes.BlueViolet),
            new ColorTitle("Brown",Brushes.Brown),
            new ColorTitle("BurlyWood",Brushes.BurlyWood),
            new ColorTitle("CadetBlue",Brushes.CadetBlue),
            new ColorTitle("Chartreuse",Brushes.Chartreuse),
            new ColorTitle("Chocolate",Brushes.Chocolate),
            new ColorTitle("Coral",Brushes.Coral),
            new ColorTitle("CornflowerBlue",Brushes.CornflowerBlue),
            new ColorTitle("Cornsilk",Brushes.Cornsilk),
            new ColorTitle("Crimson",Brushes.Crimson),
            new ColorTitle("Cyan",Brushes.Cyan),
            new ColorTitle("DarkBlue",Brushes.DarkBlue),
            new ColorTitle("DarkCyan",Brushes.DarkCyan),
            new ColorTitle("DarkGoldenrod",Brushes.DarkGoldenrod),
            new ColorTitle("DarkGray",Brushes.DarkGray),
            new ColorTitle("DarkGreen",Brushes.DarkGreen),
            new ColorTitle("DarkKhaki",Brushes.DarkKhaki),
            new ColorTitle("DarkMagenta",Brushes.DarkMagenta),
            new ColorTitle("DarkOliveGreen",Brushes.DarkOliveGreen),
            new ColorTitle("DarkOrange",Brushes.DarkOrange),
            new ColorTitle("DarkOrchid",Brushes.DarkOrchid),
            new ColorTitle("DarkRed",Brushes.DarkRed),
            new ColorTitle("DarkSalmon",Brushes.DarkSalmon),
            new ColorTitle("DarkSeaGreen",Brushes.DarkSeaGreen),
            new ColorTitle("DarkSlateBlue",Brushes.DarkSlateBlue),
            new ColorTitle("DarkSlateGray",Brushes.DarkSlateGray),
            new ColorTitle("DarkTurquoise",Brushes.DarkTurquoise),
            new ColorTitle("DarkViolet",Brushes.DarkViolet),
            new ColorTitle("DeepPink",Brushes.DeepPink),
            new ColorTitle("DeepSkyBlue",Brushes.DeepSkyBlue),
            new ColorTitle("DimGray",Brushes.DimGray),
            new ColorTitle("DodgerBlue",Brushes.DodgerBlue),
            new ColorTitle("Firebrick",Brushes.Firebrick),
            new ColorTitle("FloralWhite",Brushes.FloralWhite),
            new ColorTitle("ForestGreen",Brushes.ForestGreen),
            new ColorTitle("Fuchsia",Brushes.Fuchsia),
            new ColorTitle("Gainsboro",Brushes.Gainsboro),
            new ColorTitle("GhostWhite",Brushes.GhostWhite),
            new ColorTitle("Gold",Brushes.Gold),
            new ColorTitle("Goldenrod",Brushes.Goldenrod),
            new ColorTitle("Gray",Brushes.Gray),
            new ColorTitle("Green",Brushes.Green),
            new ColorTitle("GreenYellow",Brushes.GreenYellow),
            new ColorTitle("Honeydew",Brushes.Honeydew),
            new ColorTitle("HotPink",Brushes.HotPink),
            new ColorTitle("IndianRed",Brushes.IndianRed),
            new ColorTitle("Indigo",Brushes.Indigo),
            new ColorTitle("Ivory",Brushes.Ivory),
            new ColorTitle("Khaki",Brushes.Khaki),
            new ColorTitle("Lavender",Brushes.Lavender),
            new ColorTitle("LavenderBlush",Brushes.LavenderBlush),
            new ColorTitle("LawnGreen",Brushes.LawnGreen),
            new ColorTitle("LemonChiffon",Brushes.LemonChiffon),
            new ColorTitle("LightBlue",Brushes.LightBlue),
            new ColorTitle("LightCoral",Brushes.LightCoral),
            new ColorTitle("LightCyan",Brushes.LightCyan),
            new ColorTitle("LightGoldenrodYellow",Brushes.LightGoldenrodYellow),
            new ColorTitle("LightGray",Brushes.LightGray),
            new ColorTitle("LightGreen",Brushes.LightGreen),
            new ColorTitle("LightPink",Brushes.LightPink),
            new ColorTitle("LightSalmon",Brushes.LightSalmon),
            new ColorTitle("LightSeaGreen",Brushes.LightSeaGreen),
            new ColorTitle("LightSkyBlue",Brushes.LightSkyBlue),
            new ColorTitle("LightSlateGray",Brushes.LightSlateGray),
            new ColorTitle("LightSteelBlue",Brushes.LightSteelBlue),
            new ColorTitle("LightYellow",Brushes.LightYellow),
            new ColorTitle("Lime",Brushes.Lime),
            new ColorTitle("LimeGreen",Brushes.LimeGreen),
            new ColorTitle("Linen",Brushes.Linen),
            new ColorTitle("Magenta",Brushes.Magenta),
            new ColorTitle("Maroon",Brushes.Maroon),
            new ColorTitle("MediumAquamarine",Brushes.MediumAquamarine),
            new ColorTitle("MediumBlue",Brushes.MediumBlue),
            new ColorTitle("MediumOrchid",Brushes.MediumOrchid),
            new ColorTitle("MediumPurple",Brushes.MediumPurple),
            new ColorTitle("MediumSeaGreen",Brushes.MediumSeaGreen),
            new ColorTitle("MediumSlateBlue",Brushes.MediumSlateBlue),
            new ColorTitle("MediumSpringGreen",Brushes.MediumSpringGreen),
            new ColorTitle("MediumTurquoise",Brushes.MediumTurquoise),
            new ColorTitle("MediumVioletRed",Brushes.MediumVioletRed),
            new ColorTitle("MidnightBlue",Brushes.MidnightBlue),
            new ColorTitle("MintCream",Brushes.MintCream),
            new ColorTitle("MistyRose",Brushes.MistyRose),
            new ColorTitle("Moccasin",Brushes.Moccasin),
            new ColorTitle("NavajoWhite",Brushes.NavajoWhite),
            new ColorTitle("Navy",Brushes.Navy),
            new ColorTitle("OldLace",Brushes.OldLace),
            new ColorTitle("Olive",Brushes.Olive),
            new ColorTitle("OliveDrab",Brushes.OliveDrab),
            new ColorTitle("OliveDrab",Brushes.Orange),
            new ColorTitle("OrangeRed",Brushes.OrangeRed),
            new ColorTitle("Orchid",Brushes.Orchid),
            new ColorTitle("PaleGoldenrod",Brushes.PaleGoldenrod),
            new ColorTitle("PaleGreen",Brushes.PaleGreen),
            new ColorTitle("PaleTurquoise",Brushes.PaleTurquoise),
            new ColorTitle("PaleVioletRed",Brushes.PaleVioletRed),
            new ColorTitle("PapayaWhip",Brushes.PapayaWhip),
            new ColorTitle("PeachPuff",Brushes.PeachPuff),
            new ColorTitle("Peru",Brushes.Peru),
            new ColorTitle("Pink",Brushes.Pink),
            new ColorTitle("Plum",Brushes.Plum),
            new ColorTitle("PowderBlue",Brushes.PowderBlue),
            new ColorTitle("Purple",Brushes.Purple),
            new ColorTitle("Red",Brushes.Red),
            new ColorTitle("RosyBrown",Brushes.RosyBrown),
            new ColorTitle("RoyalBlue",Brushes.RoyalBlue),
            new ColorTitle("SaddleBrown",Brushes.SaddleBrown),
            new ColorTitle("Salmon",Brushes.Salmon),
            new ColorTitle("SandyBrown",Brushes.SandyBrown),
            new ColorTitle("SeaGreen",Brushes.SeaGreen),
            new ColorTitle("SeaShell",Brushes.SeaShell),
            new ColorTitle("Sienna",Brushes.Sienna),
            new ColorTitle("Silver",Brushes.Silver),
            new ColorTitle("SkyBlue",Brushes.SkyBlue),
            new ColorTitle("SlateBlue",Brushes.SlateBlue),
            new ColorTitle("SlateGray",Brushes.SlateGray),
            new ColorTitle("Snow",Brushes.Snow),
            new ColorTitle("SpringGreen",Brushes.SpringGreen),
            new ColorTitle("SteelBlue",Brushes.SteelBlue),
            new ColorTitle("Tan",Brushes.Tan),
            new ColorTitle("Teal",Brushes.Teal),
            new ColorTitle("Thistle",Brushes.Thistle),
            new ColorTitle("Tomato",Brushes.Tomato),
            new ColorTitle("Transparent",Brushes.Transparent),
            new ColorTitle("Turquoise",Brushes.Turquoise),
            new ColorTitle("Violet",Brushes.Violet),
            new ColorTitle("Wheat",Brushes.Wheat),
            new ColorTitle("White",Brushes.White),
            new ColorTitle("WhiteSmoke",Brushes.WhiteSmoke),
            new ColorTitle("Yellow",Brushes.Yellow),
            new ColorTitle("YellowGreen",Brushes.YellowGreen),
        };

        protected Canvas mCanvas;   //  通常はCanvasを使う
                                    //  StackPanelだとLineとEllipseは表示されるかRectAngleやTextBlockが表示されない

        public Rect mView = new Rect();             //  描画領域

        public Brush mBrush = Brushes.Black;        //  要素の色
        public Brush mFillColor = null;             //  塗潰しカラー(null:透明)
        public double mPointSize = 1;               //  点の大きさ
        public double mThickness = 1;               //  線の太さ
        public double mTextSize = 20;               //  文字サイズ
        public Brush mTextColor = Brushes.Black;    //  文字色
        public int mPointType = 0;                  //  点種(0:dot 1: cross 2:plus 3:box 4:circle)
        public int mLineType = 0;                   //  線種(0:solid 1:dash 2:center 3:phantom)
        public SweepDirection mSweepDirection = SweepDirection.Clockwise;   //  描画の回転方向
        public int mLastIndex;                      //  最終登録要素番号

        public YDraw()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">Canvas</param>
        public YDraw(Canvas c)
        {
            mCanvas = c;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="c">Canvas</param>
        /// <param name="width">Viewの幅</param>
        /// <param name="height">Viewの高さ</param>
        public YDraw(Canvas c, double width, double height)
        {
            mCanvas = c;
            mView.Width = width;
            mView.Height = height;
        }

        /// <summary>
        /// ビューサイズを設定
        /// </summary>
        /// <param name="width">ビューの幅</param>
        /// <param name="height">ビューの高さ</param>
        public void setViewSize(double width, double height)
        {
            mView.Width = width;
            mView.Height = height;
        }

        /// <summary>
        /// 登録した図形をすべて削除する
        /// </summary>
        public void clear()
        {
            mCanvas.Children.Clear();
        }

        /// <summary>
        /// Brush を色名に変換
        /// </summary>
        /// <param name="color">Brush値</param>
        /// <returns>色名</returns>
        public string getColorName(Brush color)
        { 
            int index = mColorList.FindIndex(x => x.brush == color);
            return mColorList[index < 0 ? 0 : index].colorTitle;
        }

        /// <summary>
        /// 色名を Brush値に変換
        /// </summary>
        /// <param name="color">色名</param>
        /// <returns>Brush値</returns>
        public Brush getColor(string color)
        {
            int index = mColorList.FindIndex(x => x.colorTitle == color);
            return mColorList[index < 0 ? 0 : index].brush;
        }

        /// <summary>
        /// キャンパスの背景色を設定
        /// </summary>
        /// <param name="color"></param>
        public void setBackColor(Brush color)
        {
            mCanvas.Background = color;
        }

        /// <summary>
        /// 点の描画
        /// 形状 type : 0=dot 1:cross 2:plus 3:box 4:Circle
        /// </summary>
        /// <param name="p">点座標</param>
        /// <param name="size">点の大きさ</param>
        /// <param name="type">点の形状</param>
        public void drawPoint(PointD p)
        {
            double tmpThickness = mThickness;
            int tmpLineType = mLineType;
            mThickness = 1;
            mLineType = 0;
            PointD ps = p.toCopy();
            PointD pe = p.toCopy();
            double size = mPointSize;
            switch (mPointType) {
                case 1:     //  cross
                    size *= 3;
                    ps.offset(-size / 2.0, -size / 2.0);
                    pe.offset( size / 2.0, size / 2.0);
                    drawLine(new LineD(ps, pe));
                    ps = p.toCopy();
                    pe = p.toCopy();
                    ps.offset(-size / 2.0,  size / 2.0);
                    pe.offset( size / 2.0, -size / 2.0);
                    drawLine(new LineD(ps, pe));
                    break;
                case 2:     //  plus
                    size *= 4;
                    ps.offset(0, -size / 2.0);
                    pe.offset(0,  size / 2.0);
                    drawLine(new LineD(ps, pe));
                    ps = p.toCopy();
                    pe = p.toCopy();
                    ps.offset(-size / 2.0, 0);
                    pe.offset( size / 2.0, 0);
                    drawLine(new LineD(ps, pe));
                    break;
                case 3:     //  Box
                    size *= 4;
                    ps.offset(-size / 2.0, -size / 2.0);
                    pe.offset( size / 2.0,  size / 2.0);
                    drawRectangle(new Rect(ps.toPoint(), pe.toPoint()));
                    break;
                case 4:     //  Circle
                    size *= 4;
                    drawCircle(p, size / 2.0);
                    break;
                default:    //  Dot
                    ps.offset(-size / 2.0, -size / 2.0);
                    pe.offset( size / 2.0, -size / 2.0);
                    for (int y = 0; y < size; y++) {
                        drawLine(new LineD(ps, pe));
                        ps.offset(0.0, 1.0);
                        pe.offset(0.0, 1.0);
                    }
                    break;
            }
            mThickness = tmpThickness;
            mLineType = tmpLineType;
        }

        /// <summary>
        /// 線分のパターン
        /// </summary>
        private List<List<double>> mLinePattern = new List<List<double>>() {
            new List<double> (){ 5, 3},                 //  破線 dashd
            new List<double> (){ 20, 3, 5, 3},          //  一点鎖線 center
            new List<double> (){ 20, 3, 5, 3, 5, 3},    //  二点鎖線 phantom
        };

        /// <summary>
        /// 線分の描画(線種 0:実線 1:破線 2:一点鎖線 2:二点鎖線)
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        /// <param name="type">線種</param>
        public void drawLine(PointD ps, PointD pe)
        {
            drawLine(new LineD(ps, pe));
        }

        /// <summary>
        /// 線分の描画(線種 0:実線 1:破線 2:一点鎖線 2:二点鎖線)
        /// </summary>
        /// <param name="l">線分座標</param>
        public void drawLine(LineD l)
        {
            if (0 < mLineType) {
                List<PointD> plist = l.dividePattern(mLinePattern[mLineType - 1]);
                for (int i = 0; i < plist.Count - 1; i += 2)
                    drawLine(new LineD(plist[i].x, plist[i].y, plist[i+1].x, plist[i+1].y).toLine());
            } else {
                drawLine(l.toLine());
            }
        }

        /// <summary>
        /// 線分の表示
        /// </summary>
        /// <param name="line">線分座標</param>
        public void drawLine(Line line)
        {
            line.Stroke = mBrush;
            line.StrokeThickness = mThickness;
            mLastIndex = mCanvas.Children.Add(line);
        }

        /// <summary>
        /// 円弧の描画
        /// </summary>
        /// <param name="arc">ArcD</param>
        public void drawArc(ArcD arc)
        {
            arc.normalize();
            if (mLineType == 0) {
                drawEllipse(arc.mCp.x, arc.mCp.y, arc.mR, arc.mR, arc.mSa, arc.mEa);
            } else {
                //  線種対応
                PointD sp = arc.startPoint();
                PointD ep;
                double ang = arc.mSa;
                int i = 0;
                while (ang < arc.mEa) {
                    ang += mLinePattern[mLineType - 1][(i++) % mLinePattern[mLineType - 1].Count] / arc.mR;
                    if (arc.mEa < ang) {
                        drawLine(new LineD(sp, arc.endPoint()));
                        break;
                    }
                    ep = arc.getPoint(ang);
                    drawLine(new LineD(sp, ep));
                    ang += mLinePattern[mLineType - 1][(i++) % mLinePattern[mLineType - 1].Count] / arc.mR;
                    sp = arc.getPoint(ang);
                }
            }
        }
 
        /// <summary>
        /// 円の表示
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="radius">半径</param>
        public void drawCircle(PointD center, double radius)
        {
            drawCircle(center.x, center.y, radius);
        }

        /// <summary>
        /// 円の表示
        /// </summary>
        /// <param name="cx">中心X座標</param>
        /// <param name="cy">中心Y座標</param>
        /// <param name="radius">半径</param>
        public void drawCircle(double cx, double cy, double radius)
        {
            double left = cx - radius;
            double top = cy - radius;
            double width = radius * 2;
            double height = radius * 2;

            drawOval(left, top, width, height, 0);
        }

        /// <summary>
        /// 楕円の描画
        /// </summary>
        /// <param name="left">左位置</param>
        /// <param name="top">上位置</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawOval(double left, double top, double width, double height, double rotate)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Margin = new Thickness(left, top, 0.0, 0.0);   // Marginによる位置の指定
            ellipse.Fill = mFillColor;
            ellipse.Width = Math.Abs(width);
            ellipse.Height = Math.Abs(height);
            ellipse.Stroke = mBrush;
            ellipse.StrokeThickness = mThickness;
            //  楕円の回転角と回転中心
            RotateTransform rotateTransform = new RotateTransform(rotate * 180.0 / Math.PI, width / 2, height / 2);
            ellipse.RenderTransform = rotateTransform;
            //RotateTransform rotateTransform = new RotateTransform(rotate);          //  回転角を設定(度)
            //TranslateTransform translateTransform = new TranslateTransform(30, 0);  //  移動距離(X軸,Y軸)
            //TransformGroup transformGroup = new TransformGroup();                   //  座標変換をまとめる
            //transformGroup.Children.Add(rotateTransform);
            //transformGroup.Children.Add(translateTransform);
            //ellipse.RenderTransform = transformGroup;

            mLastIndex = mCanvas.Children.Add(ellipse);
        }

        /// <summary>
        /// 楕円弧を描画(Pathオブジェクトを使用)
        /// </summary>
        /// <param name="centor">中心座標</param>
        /// <param name="rx">X軸半径</param>
        /// <param name="ry">Y軸半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawEllipse(PointD centor, double rx, double ry, double startAngle, double endAngle, double rotate = 0.0)
        {
            drawEllipse(centor.x, centor.y, rx, ry, startAngle, endAngle, rotate);
        }

        /// <summary>
        /// 楕円弧を描画(Pathオブジェクトを使用)
        /// </summary>
        /// <param name="cx">中心X座標</param>
        /// <param name="cy">中心Y座標</param>
        /// <param name="rx">X軸半径</param>
        /// <param name="ry">Y軸半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        /// <param name="rotate">回転角(rad)</param>
        public void drawEllipse(double cx, double cy, double rx, double ry, double startAngle, double endAngle, double rotate =0.0)
        {
            //  円の大きさ
            Size size = new Size(rx, ry);     //  X軸半径,Y軸半径
            //  始点座標
            Point startPoint = new Point(rx * Math.Cos(startAngle), ry * Math.Sin(startAngle));
            startPoint.X += cx;
            startPoint.Y += cy;
            //  終点座標
            Point endPoint = new Point(rx * Math.Cos(endAngle), ry * Math.Sin(endAngle));
            endPoint.X += cx;
            endPoint.Y += cy;
            bool isLarge = (endAngle - startAngle) > Math.PI ? true : false; //  180°を超える円弧化かを指定

            drawEllipse(new Point(cx, cy), size, startPoint, endPoint, isLarge, mSweepDirection, rotate);
        }

        /// <summary>
        /// 楕円弧を描画(Pathオブジェクトを使用)
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="size">楕円の半径(X軸半径,Y軸半径)</param>
        /// <param name="startPoint">始点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="isLarge">大円弧? (180°＜ 円弧角度)</param>
        /// <param name="sweepDirection">円弧の描画方向</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawEllipse(PointD center, Size size, PointD startPoint, PointD endPoint, bool isLarge, SweepDirection sweepDirection, double rotate = 0.0)
        {
            drawEllipse(center.toPoint(), size, startPoint.toPoint(), endPoint.toPoint(), isLarge, sweepDirection, rotate);
        }

        /// <summary>
        /// 楕円弧を描画(Pathオブジェクトを使用)
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="size">楕円の半径(X軸半径,Y軸半径)</param>
        /// <param name="startPoint">始点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="isLarge">大円弧? (180°＜ 円弧角度)</param>
        /// <param name="sweepDirection">円弧の描画方向</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawEllipse(Point center, Size size, Point startPoint, Point endPoint, bool isLarge, SweepDirection sweepDirection, double rotate = 0.0)
        {
            //  Pathオブジェクトの生成(Path→PathGeometry→PathFigure→ArcSegment)
            Path path = new Path();                     //  Pathオブジェクト(System.Windows.Shapes.Path )
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPoint;         //  始点
            bool isStorked = true;                      //  セグメントに線を付ける
            //  ArcSegmentのRotationAngleは始終点を変えずに楕円を回転させるため0にする
            ArcSegment arcSegment = new ArcSegment(endPoint, size, 0, isLarge, sweepDirection, isStorked);
            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            //  楕円の回転角と回転中心
            RotateTransform rotateTransform = new RotateTransform(rotate * 180.0 / Math.PI, center.X, center.Y);
            pathGeometry.Transform = rotateTransform;
            //  色と線の太さを設定
            path.Fill = mFillColor;
            path.Stroke = mBrush;
            path.StrokeThickness = mThickness;
            path.Data = pathGeometry;

            mLastIndex = mCanvas.Children.Add(path);
        }

        /// <summary>
        /// 四角形の描画
        /// </summary>
        /// <param name="rect">座標(左上,幅,高さ)</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawRectangle(Rect rect, double rotate = 0.0 )
        {
            drawRectangle(rect.Left, rect.Top, rect.Width, rect.Height, rotate);
        }

        /// <summary>
        /// 四角形を描画
        /// </summary>
        /// <param name="left">左位置</param>
        /// <param name="top">上位置</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="rotate">回転角度(中心で時計回)(rad)</param>
        public void drawRectangle(double left, double top, double width, double height, double rotate = 0.0)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Margin = new Thickness(left, top, 0.0, 0.0);  // Marginによる位置の指定
            rectangle.Fill = mFillColor;
            rectangle.Width = Math.Abs(width);
            rectangle.Height = Math.Abs(height);
            rectangle.Stroke = mBrush;
            rectangle.StrokeThickness = mThickness;
            RotateTransform rotateTransform = new RotateTransform(rotate * 180.0 / Math.PI, width / 2, height / 2);
            rectangle.RenderTransform = rotateTransform;

            mLastIndex = mCanvas.Children.Add(rectangle);
        }

        /// <summary>
        /// ポリラインの描画
        /// </summary>
        /// <param name="polylineList">座標リスト</param>
        public void drawPolyline(List<Point> polygonList)
        {
            Polyline polyline = new Polyline();
            polyline.Stroke = mBrush;
            polyline.Fill = mFillColor;
            polyline.StrokeThickness = mThickness;
            polyline.HorizontalAlignment = HorizontalAlignment.Left;
            polyline.VerticalAlignment = VerticalAlignment.Center;
            PointCollection pointCollection = new PointCollection();
            foreach (Point p in polygonList) {
                pointCollection.Add(p);
            }
            polyline.Points = pointCollection;

            mLastIndex = mCanvas.Children.Add(polyline);
        }

        /// <summary>
        /// ポリラインの描画
        /// </summary>
        /// <param name="polylineList">座標リスト</param>
        public void drawPolyline(List<PointD> polylineList)
        {
            if (mLineType == 0) {
                List<Point> pointList = new List<Point>();
                foreach (PointD p in polylineList) {
                    pointList.Add(p.toPoint());
                }
                drawPolyline(pointList);
            } else {
                for (int i = 0; i < polylineList.Count - 1; i++) {
                    drawLine(polylineList[i], polylineList[i + 1]);
                }
            }
        }

        /// <summary>
        /// ポリゴンの描画(閉領域を作成)
        /// </summary>
        /// <param name="polygonList">点座標リスト</param>
        public void drawPolygon(List<Point> polygonList)
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = mBrush;
            polygon.Fill = mFillColor;
            polygon.StrokeThickness = mThickness;
            polygon.HorizontalAlignment = HorizontalAlignment.Left;
            polygon.VerticalAlignment = VerticalAlignment.Center;
            PointCollection pointCollection = new PointCollection();
            foreach (Point p in polygonList) {
                pointCollection.Add(p);
            }
            polygon.Points = pointCollection;

            mLastIndex = mCanvas.Children.Add(polygon);
        }

        /// <summary>
        /// ポリゴンの描画(閉領域を作成)
        /// </summary>
        /// <param name="polygonList">点座標リスト</param>
        public void drawPolygon(List<PointD> polygonList)
        {
            List<Point> pointList = new List<Point>();
            foreach (PointD p in polygonList) {
                pointList.Add(p.toPoint());
            }
            drawPolygon(pointList);
        }


        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="leftTop">左上座標</param>
        /// <param name="rotate">回転角(時計回り)(rad)</param>
        public void drawText(string text, Point leftTop, double rotate = 0.0)
        {
            drawText(text, leftTop.X, leftTop.Y, rotate);
        }

        /// <summary>
        /// 文字列の描画(左上位置)
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="left">左位置</param>
        /// <param name="top">上位置</param>
        /// <param name="rotate">文字列角度(rad)</param>
        public void drawText(string text, double left, double top, double rotate = 0.0)
        {
            if (text == null)
                return;
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = mTextColor;
            textBlock.Margin = new Thickness(left, top, 0.0, 0.0);          // Marginによる位置の指定
            textBlock.FontSize = mTextSize;                                 //  文字サイズ
            RotateTransform rotateTransform = new RotateTransform(rotate * 180.0 / Math.PI);  //  文字列角度
            textBlock.RenderTransform = rotateTransform;                    //  文字列角度を設定
            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;

            mLastIndex = mCanvas.Children.Add(textBlock);
        }

        /// <summary>
        /// 文字列の描画
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="leftTop">左上座標</param>
        /// <param name="rotate">回転角(時計回り)(rad)</param>
        /// <param name="ha">水平配置(Left/Center/Right)</param>
        /// <param name="va">垂直配置(Top/Center/Bottom)</param>
        public void drawText(string text, PointD leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            if (text == null)
                return;
            leftTop = getTextOrg(text, leftTop, rotate, ha, va);
            //  文字列のパラメータ設定
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = mTextColor;
            //double width = mTextSize * text.Length;
            //double height = mTextSize;
            textBlock.Margin = new Thickness(leftTop.x, leftTop.y, 0.0, 0.0);   // Marginによる位置の指定
            textBlock.FontSize = mTextSize;                                     //  文字サイズ
            RotateTransform rotateTransform = new RotateTransform(rotate * 180.0 / Math.PI);  //  文字列角度
            textBlock.RenderTransform = rotateTransform;                        //  文字列角度を設定
            //  アライメントは使えていない？
            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
            textBlock.VerticalAlignment = VerticalAlignment.Top;
            //textBlock.HorizontalAlignment = ha;
            //textBlock.VerticalAlignment = va;

            mLastIndex = mCanvas.Children.Add(textBlock);
        }

        /// <summary>
        /// 文字列の左上原点座標を求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="leftTop">原点座標</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平配置(Left/Center/Right)</param>
        /// <param name="va">垂直配置(Top/Center/Bottom)</param>
        /// <returns>左上原点</returns>
        public PointD getTextOrg(string text, PointD leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            PointD op = new PointD(leftTop.x, leftTop.y);
            Size size = measureText(text);             //  文字列の大きさ(幅と高さ))
            size.Width += size.Height * 0.1;
            size.Height *= 1.1;                         //  ベースラインを調整
            //  アライメントの調整
            if (ha == HorizontalAlignment.Center)
                op.x -= size.Width / 2;
            else if (ha == HorizontalAlignment.Right)
                op.x -= size.Width;
            if (va == VerticalAlignment.Center)
                op.y -= size.Height / 2;
            else if (va == VerticalAlignment.Bottom)
                op.y -= size.Height;
            //  文字列回転時の回転原点を求める(文字列の左上)
            double dx = (op.x - leftTop.x);
            double dy = (op.y - leftTop.y);
            op.x = leftTop.x + dx * Math.Cos(rotate) - dy * Math.Sin(rotate);
            op.y = leftTop.y + dx * Math.Sin(rotate) + dy * Math.Cos(rotate);
            return op;
        }


        /// <summary>
        /// 文字列の大きさを求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>文字列の大きさ(Size.Width/Height)</returns>
        public Size measureText(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = mTextColor;
            textBlock.FontSize = mTextSize;             //  文字サイズ
            //  auto sized (https://code.i-harness.com/ja-jp/q/8d5d0e)
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }

        /// <summary>
        /// 文字列の縦横比を求める(高さ/幅)
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>縦横比</returns>
        public virtual double measureTextRatio(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = mTextSize;             //  文字サイズ
            //  auto sized (https://code.i-harness.com/ja-jp/q/8d5d0e)
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            return textBlock.ActualHeight / textBlock.ActualWidth;
        }
    }
}
