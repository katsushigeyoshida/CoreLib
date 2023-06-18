﻿using System;
using System.Collections.Generic;
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
    /// List<PointD> toPointList()                      文字列の領域の頂点座標と中点座標を求める
    /// bool insideChk(PointD p)                        Pointの内外判定
    /// bool insideChk(Box b)                           Boxの内外判定
    /// Box getBox()                                    文字列のBox領域
    /// List<PointD> getArea()                          文字列の領域の頂点座標を求める
    /// PointD nearPoints(PointD p)                     頂点座標+中点座標で最も近い点を求める
    /// 
    /// PointD getTextOrg(string text, PointD leftTop, double rotate, HorizontalAlignment ha, VerticalAlignment va)
    /// Size measureText(string text, double textSize)  文字列の大きさを求める
    /// 
    /// </summary>
    public class TextD
    {
        public string mText = "";
        public double mTextSize = 12;
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
            VerticalAlignment va = VerticalAlignment.Top)
        {
            mText = text;
            mPos = pos;
            mTextSize = size;
            mRotate = rotate;
            mHa = ha;
            mVa = va;
        }

        /// <summary>
        /// コピーの作成
        /// </summary>
        /// <returns>TextD</returns>
        public TextD toCopy()
        {
            return new TextD(mText, mPos.toCopy(), mTextSize, mRotate, mHa, mVa);
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{mText},{mPos.ToString("F2")},{mTextSize.ToString("F2")},{mRotate.ToString("F2")},{mHa},{mVa}";
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
        /// 文字列の領域の頂点座標と中点座標を求める
        /// </summary>
        /// <returns></returns>
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
        /// Pointの内外判定
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool insideChk(PointD p)
        {
            PointD pos = mPos;
            Size size = measureText(mText, mTextSize);
            if (mHa != HorizontalAlignment.Left || mVa != VerticalAlignment.Top)
                pos = getTextOrg(mText, mPos, mRotate, mHa, mVa);
            Box box = new Box(pos, size);
            p.rotate(pos, -mRotate);
            return box.insideChk(p);
        }

        /// <summary>
        /// Boxの内外判定
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool insideChk(Box b)
        {
            PointD pos = mPos;
            Size size = measureText(mText, mTextSize);
            if (mHa != HorizontalAlignment.Left || mVa != VerticalAlignment.Top)
                pos = getTextOrg(mText, mPos, mRotate, mHa, mVa);
            Box box = new Box(pos, size);
            PointD tp = b.TopLeft;
            tp.rotate(pos, -mRotate);
            PointD bp = b.BottomRight;
            bp.rotate(pos, -mRotate);
            return box.insideChk(tp) && box.insideChk(bp);
        }

        /// <summary>
        /// 文字列のBox領域
        /// </summary>
        /// <returns>Box領域</returns>
        public Box getBox()
        {
            PointD pos = mPos;
            Size size = measureText(mText, mTextSize);
            if (mHa != HorizontalAlignment.Left || mVa != VerticalAlignment.Top)
                pos = getTextOrg(mText, mPos, mRotate, mHa, mVa);
            Box box = new Box(pos, size);
            box.rotateArea(pos, mRotate);
            return box;
        }

        /// <summary>
        /// 文字列の領域の頂点座標を求める
        /// </summary>
        /// <returns></returns>
        public List<PointD> getArea()
        {
            PointD pos = mPos;
            Size size = measureText(mText, mTextSize);
            if (mHa != HorizontalAlignment.Left || mVa != VerticalAlignment.Top)
                pos = getTextOrg(mText, mPos, mRotate, mHa, mVa);
            Box box = new Box(pos, size);
            return box.getRotateBox(pos, mRotate);
        }

        /// <summary>
        /// 頂点座標+中点座標で最も近い点を求める
        /// </summary>
        /// <param name="p">指定座標</param>
        /// <returns>座標</returns>
        public PointD nearPoints(PointD p)
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