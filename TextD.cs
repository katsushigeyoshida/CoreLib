using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CoreLib
{
    /// <summary>
    /// 文字列管理クラス
    /// 
    /// コンストラクタ
    /// TextD()
    /// TextD(string text, PointD pos, double size = 12, double rotate = 0,
    ///     HorizontalAlignment ha = HorizontalAlignment.Left,
    ///     VerticalAlignment va = VerticalAlignment.Top)
    ///     
    /// TextD toCopy()                                  コピーの作成
    /// string ToString()
    /// void rotate(double angle)                       文字列の回転
    /// void rotate(PointD cp, double angle)            指定点を中心に回転
    /// void rotate(PointD cp, PointD mp)               指定点を中心に回転
    /// void mirror(PointD sp, PointD ep)               指定線分でミラーする
    ///  void scale(PointD cp, double scale)            原点を指定して拡大縮小
    /// List<PointD> toPointList()                      文字列の領域の頂点座標と中点座標を求める
    /// List<LineD> toLineList()                        文字列の領域を示す線分を求める
    /// bool insideChk(PointD p)                        Pointの内外判定
    /// bool insideChk(Box b)                           Boxの内外判定
    /// Box getBox()                                    文字列のBox領域
    /// Box getBox(string text, PointD pos)             定した文字列の領域を求める
    /// List<PointD> getArea()                          文字列の領域の頂点座標を求める
    /// PointD nearPeakPoint(PointD p)                  頂点座標+中点座標で最も近い点を求める
    /// PointD nearPoints(PointD p)                     テキストの領域を示す線分上で最も近い点を求める
    /// LineD nearLine(PointD pos)                      テキストの領域を示す線分で最も近いものを求める
    /// 
    /// PointD getTextOrg(string text, PointD leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// Size measureText(string text, double textSize)  文字列の大きさを求める
    /// 
    /// </summary>
    public class TextD
    {
        public string mText = "";
        public double mTextSize = 12;
        public double mLinePitchRate = 1.2;
        public PointD mPos = new PointD();
        public double mRotate = 0;
        public HorizontalAlignment mHa = HorizontalAlignment.Left;
        public VerticalAlignment mVa = VerticalAlignment.Top;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextD()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="pos">文字位置</param>
        /// <param name="size">文字サイズ</param>
        /// <param name="rotate">回転角(rad)</param>
        /// <param name="ha">水平方向アライメント</param>
        /// <param name="va">垂直方向アライメント</param>
        public TextD(string text, PointD pos, double size = 12, double rotate = 0,
            HorizontalAlignment ha = HorizontalAlignment.Left,
            VerticalAlignment va = VerticalAlignment.Top, double linePitchRate = 1.0)
        {
            mText = text;
            mPos = pos;
            mTextSize = size;
            mRotate = rotate;
            mHa = ha;
            mVa = va;
            mLinePitchRate = linePitchRate;
        }

        /// <summary>
        /// コピーの作成
        /// </summary>
        /// <returns>TextD</returns>
        public TextD toCopy()
        {
            return new TextD(mText, mPos.toCopy(), mTextSize, mRotate, mHa, mVa, mLinePitchRate);
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{mText},{mPos.ToString("F2")},{mTextSize.ToString("F2")},{mLinePitchRate.ToString("F2")}," +
                $"{mRotate.ToString("F2")},{mHa},{mVa}";
        }

        /// <summary>
        /// 文字列の回転
        /// </summary>
        /// <param name="angle"></param>
        public void rotate(double angle)
        {
            mRotate += angle;
        }

        /// <summary>
        /// 指定点を中心に回転
        /// </summary>
        /// <param name="cp">中心点</param>
        /// <param name="angle">回転角(rad)</param>
        public void rotate(PointD cp, double angle)
        {
            mRotate += angle;
            mPos.rotate(cp, angle);
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
        /// 指定線分でミラーする
        /// </summary>
        /// <param name="sp">始点座標</param>
        /// <param name="ep">終点座標</param>
        public void mirror(PointD sp, PointD ep)
        {
            mPos.mirror(sp, ep);
            mVa = mVa == VerticalAlignment.Top ? VerticalAlignment.Bottom : mVa == VerticalAlignment.Bottom ? VerticalAlignment.Top : VerticalAlignment.Center;
            double ang = ep.angle(sp);
            mRotate = ang - mRotate + ang;
        }

        /// <summary>
        /// 原点を指定して拡大縮小
        /// </summary>
        /// <param name="cp">原点</param>
        /// <param name="scale">拡大率</param>
        public void scale(PointD cp, double scale)
        {
            mPos.scale(cp, scale);
            mTextSize *= scale;
        }

        /// <summary>
        /// 文字列の領域の頂点座標と中点座標を求める
        /// </summary>
        /// <returns>点リスト</returns>
        public List<PointD> toPointList()
        {
            List<PointD> plist = new List<PointD> ();
            List<PointD> pplist = getArea();
            LineD l = new LineD();
            for (int i = 0; i < pplist.Count; i++) {
                plist.Add(pplist[i]);
                l = new LineD(pplist[i], pplist[i == pplist.Count - 1 ? 0 : i+1]);
                plist.Add(l.centerPoint());
            }
            l = new LineD(pplist[0], pplist[2]);
            plist.Add(l.centerPoint());
            return plist;
        }

        /// <summary>
        /// 文字列の領域を示す線分を求める
        /// </summary>
        /// <returns>線分リスト</returns>
        public List<LineD> toLineList()
        {
            List<LineD> llist = new();
            List<PointD> plist = getArea();
            for (int i = 0; i < plist.Count - 1; i++)
                llist.Add(new LineD(plist[i], plist[i + 1]));
            llist.Add(new LineD(plist[plist.Count - 1], plist[0]));
            return llist;
        }

        /// <summary>
        /// Pointの内外判定
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool insideChk(PointD pos)
        {
            PointD p = pos.toCopy();
            double tRotate = mRotate;
            mRotate = 0;
            Box box = getBox();
            mRotate = tRotate;
            p.rotate(mPos, -mRotate);
            return box.insideChk(p);
        }

        /// <summary>
        /// Boxの内外判定(Boxの中にすべて含まれている)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool insideChk(Box b)
        {
            double tRotate = mRotate;
            mRotate = 0;
            Box box = getBox();
            mRotate = tRotate;
            PointD tp = b.TopLeft;
            tp.rotate(mPos, -mRotate);
            PointD bp = b.BottomRight;
            bp.rotate(mPos, -mRotate);
            return box.insideChk(tp) && box.insideChk(bp);
        }

        /// <summary>
        /// 文字列のBox領域(複数行に対応)
        /// </summary>
        /// <returns>Box領域</returns>
        public Box getBox()
        {
            string[] multiText = mText.Split(new char[] { '\n' });
            Box area = null;
            PointD pos = mPos;
            for (int i = 0; i < multiText.Length; i++) {
                string text = multiText[i].TrimEnd('\r');
                if (i == 0)
                    area = getBox(text, pos);
                else
                    area.extension(getBox(text, pos));
                pos += pos.vector(mRotate - Math.PI / 2, mTextSize * mLinePitchRate);
            }
            return area;
        }

        /// <summary>
        /// 指定した文字列の領域を求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="pos">文字位置</param>
        /// <returns>Box領域</returns>
        public Box getBox(string text, PointD pos)
        {
            PointD tpos = pos;
            Size size = measureText(text, mTextSize);
            if (mHa != HorizontalAlignment.Left || mVa != VerticalAlignment.Top)
                tpos = getTextOrg(text, pos, mRotate, mHa, mVa);
            Box box = new Box(tpos, size);
            box.rotateArea(tpos, mRotate);
            return box;
        }

        /// <summary>
        /// 文字列の領域の頂点座標を求める
        /// </summary>
        /// <returns>頂点リスト</returns>
        public List<PointD> getArea()
        {
            double tRotate = mRotate;
            mRotate = 0;
            Box box = getBox();
            mRotate = tRotate;
            return box.getRotateBox(mPos, mRotate);
        }

        /// <summary>
        /// 頂点座標+中点座標で最も近い点を求める
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>座標</returns>
        public PointD nearPeakPoint(PointD p)
        {
            List<PointD> points = toPointList();
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
        /// テキストの領域を示す線分上で最も近い点を求める
        /// </summary>
        /// <param name="pos">参照店</param>
        /// <returns>座標</returns>
        public PointD nearPoint(PointD pos)
        {
            LineD l = nearLine(pos);
            return l.intersection(pos);
        }


        /// <summary>
        /// テキストの領域を示す線分で最も近いものを求める
        /// </summary>
        /// <param name="pos">参照点</param>
        /// <returns>線分</returns>
        public LineD nearLine(PointD pos)
        {
            List<LineD> lines = toLineList();
            return lines.MinBy(l => l.distance(pos));
        }


        //  ---  文字列パラメータ処理

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
            PointD op = leftTop.toCopy();
            PointD cp = leftTop.toCopy();
            Size size = measureText(text, mTextSize);   //  文字列の大きさ(幅と高さ))
            size.Width += size.Height * 0.1;
            size.Height *= 1.1;                         //  ベースラインを調整
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
            //  文字列回転時の回転原点を求める(文字列の左上)
            op.offset(-dx, dy);
            op.rotate(cp, rotate);
            return op;
        }

        /// <summary>
        /// 文字列の大きさを求める
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>文字列の大きさ(Size.Width/Height)</returns>
        public Size measureText(string text, double textSize)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = System.Windows.Media.Brushes.Black;
            textBlock.FontSize = textSize;             //  文字サイズ
            //  auto sized (https://code.i-harness.com/ja-jp/q/8d5d0e)
            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            textBlock.Arrange(new Rect(textBlock.DesiredSize));

            return new Size(textBlock.ActualWidth, textBlock.ActualHeight);
        }
    }
}
