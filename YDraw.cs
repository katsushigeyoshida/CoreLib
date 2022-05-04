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
    /// void setViewSize(double width, double height)
    /// void clear()
    /// void setBackColor(Brush color)
    /// 
    /// void drawLine(Line l)
    /// void drawLine(Point ps, Point pe)
    /// void drawLine(double xs, double ys, double xe, double ye)
    /// virtual void drawArc(double cx, double cy, double radius, double startAngle, double endAngle)
    /// void drawCircle(Point center, double radius)
    /// void drawCircle(double cx, double cy, double radius)
    /// void drawOval(Point centor, double rx, double ry, double rotate)
    /// void drawOval(double left, double top, double width, double height, double rotate)
    /// void drawEllipse(Point centor, double rx, double ry, double startAngle, double endAngle, double rotate = 0.0)
    /// void drawEllipse(double cx, double cy, double rx, double ry, double startAngle, double endAngle, double rotate =0.0)
    /// void drawEllipse(Point center, Size size, Point startPoint, Point endPoint, bool isLarge, SweepDirection sweepDirection, double rotate = 0.0)
    /// void drawRectangle(Rect rect, double rotate = 0.0 )
    /// void drawRectangle(double left, double top, double width, double height, double rotate = 0.0)
    /// void drawPolygon(List<Point> polygonList)
    /// void drawText(string text, Point leftTop, double rotate = 0.0)
    /// void drawText(string text, double left, double top, double rotate = 0.0)
    /// void drawText(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// 
    /// Point getTextOrg(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// Size measureText(string text)
    /// virtual double measureTextRatio(string text)
    /// 
    /// 
    /// </summary>
    public class YDraw
    {
        protected Canvas mCanvas;   //  通常はCanvasを使う
                                    //  StackPanelだとLineとEllipseは表示されるかRectAngleやTextBlockが表示されない

        public Rect mView = new Rect();             //  描画領域

        public Brush mBrush = Brushes.Black;        //  要素の色
        public Brush mFillColor = null;             //  塗潰しカラー(null:透明)
        public double mThickness = 1;               //  線の太さ
        public double mTextSize = 20;               //  文字サイズ
        public Brush mTextColor = Brushes.Black;    //  文字色
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
        /// 登録した図形をすべて探所する
        /// </summary>
        public void clear()
        {
            mCanvas.Children.Clear();
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
        /// 線分の描画
        /// </summary>
        /// <param name="l">線分座標</param>
        public void drawLine(Line l)
        {
            drawLine(l.X1, l.Y1, l.X2, l.Y2);
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="l">線分座標</param>
        public void drawLine(LineD l)
        {
            drawLine(l.ps.x, l.ps.y, l.pe.x, l.pe.y);
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public void drawLine(Point ps, Point pe)
        {
            drawLine(ps.X, ps.Y, pe.X, pe.Y);
        }

        /// <summary>
        /// 線分の描画
        /// </summary>
        /// <param name="ps">始点座標</param>
        /// <param name="pe">終点座標</param>
        public void drawLine(PointD ps, PointD pe)
        {
            drawLine(ps.x, ps.y, pe.x, pe.y);
        }

        /// <summary>
        /// 線分の表示
        /// </summary>
        /// <param name="xs">始点座標X</param>
        /// <param name="ys">始点座標Y</param>
        /// <param name="xe">終点座標X</param>
        /// <param name="ye">終点座標Y</param>
        public void drawLine(double xs, double ys, double xe, double ye)
        {
            Line line = new Line();
            line.X1 = xs;
            line.Y1 = ys;
            line.X2 = xe;
            line.Y2 = ye;
            line.Stroke = mBrush;
            line.StrokeThickness = mThickness;

            mLastIndex = mCanvas.Children.Add(line);
        }

        /// <summary>
        ///  円弧を描画(Pathオブジェクトを使用)
        /// </summary>
        /// <param name="cx">中心X座標</param>
        /// <param name="cy">中心Y座標</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        public virtual void drawArc(double cx, double cy, double radius, double startAngle, double endAngle)
        {
            if (Math.PI * 2 <= Math.Abs(startAngle - endAngle))
                drawCircle(cx, cy, radius);
            else
                drawEllipse(cx, cy, radius, radius, startAngle, endAngle, 0);
        }

        /// <summary>
        /// 円の表示
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="radius">半径</param>
        public void drawCircle(Point center, double radius)
        {
            drawCircle(center.X, center.Y, radius);
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
        /// <param name="centor">中心座標</param>
        /// <param name="rx">X軸半径</param>
        /// <param name="ry">Y軸半径</param>
        /// <param name="rotate">回転角(時計回り)(rad)</param>
        public void drawOval(Point centor, double rx, double ry, double rotate)
        {
            drawOval(centor.X - rx, centor.Y - ry, rx * 2.0, ry * 2.0, rotate);
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
        /// <param name="centor">中心座標</param>
        /// <param name="rx">X軸半径</param>
        /// <param name="ry">Y軸半径</param>
        /// <param name="startAngle">開始角度(rad)</param>
        /// <param name="endAngle">終了角度(rad)</param>
        /// <param name="rotate">回転角(時計回)(rad)</param>
        public void drawEllipse(Point centor, double rx, double ry, double startAngle, double endAngle, double rotate = 0.0)
        {
            drawEllipse(centor.X, centor.Y, rx, ry, startAngle, endAngle, rotate);
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
        public void drawText(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            if (text == null)
                return;
            leftTop = getTextOrg(text, leftTop, rotate, ha, va);
            //  文字列のパラメータ設定
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = mTextColor;
            double width = mTextSize * text.Length;
            double height = mTextSize;
            textBlock.Margin = new Thickness(leftTop.X, leftTop.Y, 0.0, 0.0);   // Marginによる位置の指定
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
        public Point getTextOrg(string text, Point leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
        {
            PointD p = getTextOrg(text, new PointD(leftTop), rotate, ha, va);
            return p.toPoint();
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
