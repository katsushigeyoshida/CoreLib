using System;
using System.Collections.Generic;
using System.Windows;

namespace CoreLib
{
    public enum DIMENSIONTYPE { horizontal, vertical, paralell, diameater, angle }

    /// <summary>
    /// パーツ(部品)クラス
    /// </summary>
    public class PartsD
    {
        //  パーツ名
        public string mName = "";
        //  表示データ
        public List<LineD> mLines;
        public List<ArcD>  mArcs;
        public List<TextD> mTexts;
        //  参照点
        public List<string> mRefString;
        public List<PointD> mRefPoints;
        public List<double> mRefValue;
        //  文字パラメータ、矢印パラメータ
        public double mArrowAngle    = Math.PI / 6;
        public double mArrowSize     = 6;
        public double mTextSize      = 12;
        public double mLinePitchRate = 1.2;
        public double mTextRotate    = 0;
        public string mDimValForm    = "f1";

        private double mEps = 1e-8;
        private YLib ylib = new YLib();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PartsD()
        {
            mLines = new List<LineD>();
            mArcs = new List<ArcD>();
            mTexts = new List<TextD>();
            mRefString = new List<string>();
            mRefPoints = new List<PointD>();
            mRefValue = new List<double>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mName">パーツ名</param>
        /// <param name="mLines">線分リスト</param>
        /// <param name="mArcs">円弧リスト</param>
        /// <param name="mTexts">文字リスト</param>
        public PartsD(string mName, List<LineD> mLines, List<ArcD> mArcs, List<TextD> mTexts)
        {
            this.mName = mName;
            this.mLines = mLines;
            this.mArcs = mArcs;
            this.mTexts = mTexts;
        }

        /// <summary>
        /// コピーの作成
        /// </summary>
        /// <returns></returns>
        public PartsD toCopy()
        {
            PartsD parts = new PartsD();
            parts.mName = mName;
            parts.mLines = mLines.ConvertAll(p => p.toCopy());
            parts.mArcs  = mArcs.ConvertAll(p => p.toCopy());
            parts.mTexts = mTexts.ConvertAll(p => p.toCopy());
            parts.mRefString = mRefString.ConvertAll(p => p);
            parts.mRefPoints = mRefPoints.ConvertAll(p => p.toCopy());
            parts.mRefValue  = mRefValue.ConvertAll(p => p);
            parts.mTextSize      = mTextSize;
            parts.mLinePitchRate = mLinePitchRate;
            parts.mTextRotate    = mTextRotate;
            parts.mArrowSize  = mArrowSize;
            parts.mArrowAngle = mArrowAngle;
            parts.mDimValForm = mDimValForm;
            return parts;
        }

        /// <summary>
        /// 領域Boxを求める
        /// </summary>
        /// <returns>Box領域</returns>
        public Box getBox()
        {
            Box box = null;
            if (mLines != null && 0 < mLines.Count) {
                if (box == null)
                    box = new Box(mLines[0]);
                for (int i = 0; i < mLines.Count; i++)
                    box.extension(mLines[i]);
            }
            if (mArcs != null && 0 < mArcs.Count) {
                if (box == null)
                    box = new Box(mArcs[0]);
                for (int i = 0; i < mArcs.Count; i++)
                    box.extension(new Box(mArcs[i]));
            }
            if (mTexts != null && 0 < mTexts.Count) {
                if (box == null)
                    box = new Box(mTexts[0]);
                for (int i = 0; i < mTexts.Count; i++)
                    box.extension(new Box(mTexts[i]));
            }
            return box;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        public void translate(PointD vec)
        {
            if (mLines != null) {
                foreach (var line in mLines)
                    line.translate(vec);
            }
            if (mArcs != null) {
                foreach (var arc in mArcs)
                    arc.mCp.offset(vec);
            }
            if (mTexts != null) {
                foreach (var text in mTexts)
                    text.mPos.offset(vec);
            }
            if (mRefPoints != null) {
                foreach (var point in mRefPoints)
                    point.translate(vec);
            }
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="cp">回転中心</param>
        /// <param name="mp">回転位置</param>
        public void rotate(PointD cp, PointD mp)
        {
            if (mLines != null) {
                foreach (var line in mLines)
                    line.rotate(cp, mp);
            }
            if (mArcs != null) {
                foreach (var arc in mArcs)
                    arc.rotate(cp, mp);
            }
            if (mTexts != null) {
                foreach (var text in mTexts)
                    text.rotate(cp, mp);
            }
            if (mRefPoints != null) {
                foreach (var point in mRefPoints)
                    point.rotate(cp, mp);
            }
        }

        /// <summary>
        /// 反転
        /// </summary>
        /// <param name="sp">始点</param>
        /// <param name="ep">終点</param>
        public void mirror(PointD sp, PointD ep)
        {
            if (mLines != null) {
                foreach (var line in mLines)
                    line.mirror(sp, ep);
            }
            if (mArcs != null) {
                foreach (var arc in mArcs)
                    arc.mirror(sp, ep);
            }
            if (mTexts != null) {
                foreach (var text in mTexts)
                    text.mirror(sp, ep);
            }
            if (mRefPoints != null) {
                foreach (var point in mRefPoints)
                    point.mirror(sp, ep);
            }
        }

        /// <summary>
        /// ストレッチ
        /// </summary>
        /// <param name="vec">移動ベクトル</param>
        /// <param name="pos">参照点</param>
        public void stretch(PointD vec, PointD pos)
        {
            if (mName == "矢印") {
                PointD ps = mLines[0].ps.toCopy();
                PointD pe = mLines[0].pe.toCopy();
                if (ps.length(pos) < pe.length(pos)) {
                    ps.offset(vec);
                } else {
                    pe.offset(vec);
                }
                createArrow(ps, pe);
            } else if (mName == "ラベル") {
                PointD ps = mLines[0].ps.toCopy();
                PointD pe = mLines[0].pe.toCopy();
                List<PointD> plist = new List<PointD>() { ps, pe, mTexts[0].mPos };
                if (textPickChk(pos)) {
                    plist[2] = mTexts[0].mPos + vec;
                } else {
                    if (ps.length(pos) < pe.length(pos)) {
                        plist[0].offset(vec);
                    } else {
                        plist[1].offset(vec);
                    }
                }
                createLabel(plist, mTexts[0].mText);
            } else if (mName == "寸法線") {
                List<PointD> plist;
                if (0 < mRefPoints.Count)
                    plist = mRefPoints;
                else
                    plist = getDimensionPos();
                LineD dl = mLines[2].toCopy();
                dl.offset(vec);
                plist[2] = dl.centerPoint();
                plist = addTextPos(plist, 3, pos, mTexts[0].mPos + vec);
                createDimension(plist);
            } else if (mName == "角度寸法線") {
                List<PointD> plist;
                if (0 < mRefPoints.Count)
                    plist = mRefPoints;
                else
                    plist = getDimensionPos();
                plist[3].offset(vec);
                plist = addTextPos(plist, 4, pos, plist[3].toCopy());
                createAngleDimension(plist);
            } else if (mName == "直径寸法線") {
                ArcD arc = new ArcD();
                if (mRefPoints.Count < 3)
                    mRefPoints = getDimensionPos();
                arc.setArc(mRefPoints[0], mRefPoints[1], mRefPoints[2]);
                List<PointD> plist = new List<PointD>() { mRefPoints[3] };
                plist[0].offset(vec);
                plist = addTextPos(plist, 1, pos, plist[0].toCopy());
                createDiameterDimension(arc, plist);
            } else if (mName == "半径寸法線") {
                ArcD arc = new ArcD();
                if (mRefPoints.Count < 3)
                    mRefPoints = getDimensionPos();
                arc.setArc(mRefPoints[0], mRefPoints[1], mRefPoints[2]);
                List<PointD> plist = new List<PointD>() { mRefPoints[3] };
                plist[0].offset(vec);
                plist = addTextPos(plist, 1, pos, plist[0].toCopy());
                createRadiusDimension(arc, plist);
            } else {

            }
        }

        /// <summary>
        /// 矢印データを作成
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        public void createArrow(PointD ps, PointD pe)
        {
            mName = "矢印";
            mLines = new List<LineD>();
            LineD l = new LineD(ps, pe);
            mLines.Add(l);
            mLines.AddRange(arrow(ps, pe));
        }

        /// <summary>
        /// ラベルの作成
        /// plist [0]始点 [1]終点 ([2]文字位置)
        /// </summary>
        /// <param name="plist">座標リスト</param>
        /// <param name="str">ラベル文字</param>
        public void createLabel(List<PointD> plist, string str)
        {
            mName = "ラベル";
            mLines = new List<LineD>();
            mTexts = new List<TextD>();
            PointD textPos = new PointD();
            HorizontalAlignment ha = HorizontalAlignment.Left;
            if (2 < plist.Count) {
                //  文字位置がある時
                textPos = new PointD(plist[2].x, plist[1].y);
                if (plist[1].x <= plist[2].x)
                    ha = HorizontalAlignment.Left;
                else
                    ha = HorizontalAlignment.Right;
            } else {
                //  文字位置がない時は寸法値位置で決める
                if (plist[0].x <= plist[1].x) {
                    textPos = new PointD(plist[1].x + mTextSize, plist[1].y);
                    ha = HorizontalAlignment.Left;
                } else {
                    textPos = new PointD(plist[1].x - mTextSize, plist[1].y);
                    ha = HorizontalAlignment.Right;
                }
            }
            LineD l = new LineD(plist[0], plist[1]);
            LineD l2 = new LineD(plist[1], textPos);
            mLines.Add(l);
            mLines.Add(l2);
            mLines.AddRange(arrow(plist[0], plist[1]));
            TextD text = new TextD(str, textPos, mTextSize, mTextRotate, ha, VerticalAlignment.Center);
            text.mLinePitchRate = mLinePitchRate;
            mTexts.Add(text);
        }

        /// <summary>
        /// 直線寸法線の作成(対象座標と寸法位置との関係で水平/垂直/平行寸法に切り替える)
        /// plist [0] 寸法対象座標 [1] 寸法対象座標 [2] 寸法位置 ([3] 文字位置)
        /// mRefPoints[0],[1] 寸法対象座標 [2] 寸法位置
        /// </summary>
        /// <param name="plist">座標リスト</param>
        public void createDimension(List<PointD> plist)
        {
            mName = "寸法線";
            PointD ps  = plist[0];
            PointD pe  = plist[1];
            PointD pos = plist[2];
            mRefPoints = plist;
            mRefValue = new();
            mRefString = new();
            int dimType = getDimType(ps, pe, pos);
            PointD poss, pose;
            LineD ls, le, lp;
            if (dimType == 1) {
                //  水平寸法
                poss = new PointD(ps.x, pos.y);
                ls = new LineD(ps, poss);
                pose = new PointD(pe.x, pos.y);
                le = new LineD(pe, pose);
                lp = new LineD(poss, pose);
            } else if (dimType == -1) {
                //  垂直寸法
                poss = new PointD(pos.x, ps.y);
                ls = new LineD(ps, poss);
                pose = new PointD(pos.x, pe.y);
                le = new LineD(pe, pose);
                lp = new LineD(poss, pose);
            } else {
                //  平行寸法
                lp = new LineD(ps, pe);
                PointD ip = lp.intersection(pos);
                PointD vec = ip.vector(pos);
                lp.translate(vec);
                ls = new LineD(ps, lp.ps);
                le = new LineD(pe, lp.pe);
            }
            //  引き出し線の座標
            ls.setLength(ls.length() + mTextSize / 2);
            ls.slide(mTextSize / 4);
            le.setLength(le.length() + mTextSize / 2);
            le.slide(mTextSize / 4);
            //  文字位置
            PointD tp = 3 < plist.Count ? plist[3] : lp.centerPoint();
            tp = lp.intersection(tp);
            double rotate = lp.angle();
            rotate += rotate <= -Math.PI / 2 ? Math.PI : rotate > Math.PI / 2 ? -Math.PI : 0;
            string mesure = ylib.double2StrZeroSup(lp.length(), mDimValForm);
            TextD text = new TextD(mesure, tp, mTextSize, rotate, System.Windows.HorizontalAlignment.Center, System.Windows.VerticalAlignment.Bottom); ;
            mLines = new List<LineD>() { ls, le, lp };
            mLines.AddRange(arrow(lp.ps, lp.pe));
            mLines.AddRange(arrow(lp.pe, lp.ps));
            mTexts = new List<TextD>();
            mTexts.Add(text);

            if (!lp.onPoint(tp)) {
                //  文字下線追加
                LineD tl;
                if (lp.ps.length(tp) < lp.pe.length(tp)) {
                    tl = new LineD(lp.ps, tp);
                } else {
                    tl = new LineD(lp.pe, tp);
                }
                Box b = text.getBox();
                tl.setLength(tl.length() + b.Size.Width / 2);
                mLines.Add(tl);
            }
        }

        /// <summary>
        /// 角度寸法の作成
        /// plist [0] 中心点 [1] 始点 [2] 終点 [3] 寸法位置 [4] 文字位置
        /// mRefPoints[0] : 中心点 [1] 始点 [2] 終点 ([3] 寸法位置)
        /// </summary>
        /// <param name="plist">座標リスト</param>
        public void createAngleDimension(List<PointD> plist)
        {
            mName = "角度寸法線";
            PointD cp  = plist[0];
            PointD ps  = plist[1];
            PointD pe  = plist[2];
            PointD pos = plist[3];
            mRefPoints = new List<PointD>(plist);
            mRefValue = new();
            mRefString = new();
            ArcD arc = new ArcD(cp, cp.length(pos));
            arc.mSa = arc.getAngle(ps);
            arc.mEa = arc.getAngle(pe);
            arc.normalize();
            //  引き出し線
            LineD ls = new LineD(ps, arc.startPoint());
            LineD le = new LineD(pe, arc.endPoint());
            ls.setLength(ls.length() + mTextSize / 2);
            ls.slide(mTextSize / 4);
            le.setLength(le.length() + mTextSize / 2);
            le.slide(mTextSize / 4);

            //  文字位置
            PointD tp = 4 < plist.Count ? plist[4] : arc.middlePoint();
            double rotate = (arc.getAngle(tp) + Math.PI / 2) % (Math.PI * 2);
            rotate -= rotate > Math.PI ? Math.PI : 0;
            rotate += rotate <= -Math.PI / 2 ? Math.PI : rotate > Math.PI / 2 ? -Math.PI : 0;
            string mesure = $"{ylib.double2StrZeroSup(ylib.R2D(arc.mOpenAngle),mDimValForm)}°";
            TextD text = new TextD(mesure, tp, mTextSize, rotate, System.Windows.HorizontalAlignment.Center, System.Windows.VerticalAlignment.Bottom); ;

            mLines = new List<LineD>();
            mLines.Add(ls);
            mLines.Add(le);
            mLines.AddRange(angleArrow(arc));
            mArcs = new List<ArcD>();
            mArcs.Add(arc);
            mTexts = new List<TextD>();
            mTexts.Add(text);

            if (!arc.onPoint(tp)) {
                //  文字下線追加
                ArcD ta = arc.toCopy();
                PointD sp = arc.startPoint();
                PointD ep = arc.endPoint();
                Box b = text.getBox();
                double ang = b.Size.Width / 2 / ta.mR;
                if (sp.length(tp) < ep.length(tp)) {
                    ta.mSa = arc.getAngle(tp) - ang;
                    ta.mEa = arc.getAngle(sp);
                } else {
                    ta.mSa = arc.getAngle(ep);
                    ta.mEa = arc.getAngle(tp) + ang;
                }
                ta.normalize();
                mArcs.Add(ta);
            }
        }

        /// <summary>
        /// 直径寸法線の作成
        /// plist [0] 寸法位置 ([1] 文字位置)
        /// mRefPoints[0] 中心点 [1] 始点 [2] 終点 [3] 寸法位置
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="plist">座標点リスト</param>
        public void createDiameterDimension(ArcD arc, List<PointD> plist)
        {
            mName = "直径寸法線";
            LineD lp = new LineD(arc.mCp, plist[0]);
            List<PointD> aplist = arc.intersection(lp, false);
            if (aplist.Count == 2) {
                lp.ps = aplist[0];
                lp.pe = aplist[1];
                PointD pos = plist[0];
                PointD tp = 1 < plist.Count ? plist[1] : lp.centerPoint();
                mRefPoints = new List<PointD>() {
                    arc.mCp, arc.startPoint(), arc.endPoint(), pos, tp
                };
                mRefValue = new();
                mRefString = new();
                double rotate = lp.angle();
                rotate += rotate <= -Math.PI / 2 ? Math.PI : rotate > Math.PI / 2 ? -Math.PI : 0;
                string mesure = $"Φ{ylib.double2StrZeroSup(lp.length(), mDimValForm)}";
                TextD text = new TextD(mesure, tp, mTextSize, rotate, System.Windows.HorizontalAlignment.Center, System.Windows.VerticalAlignment.Bottom); ;

                mLines = new List<LineD>();
                mLines.Add(lp);
                mLines.AddRange(arrow(lp.ps, lp.pe));
                mLines.AddRange(arrow(lp.pe, lp.ps));
                mTexts = new List<TextD>();
                mTexts.Add(text);

                if (!lp.onPoint(tp)) {
                    //  文字下線追加
                    LineD tl;
                    if (lp.ps.length(tp) < lp.pe.length(tp)) {
                        tl = new LineD(lp.ps, tp);
                    } else {
                        tl = new LineD(lp.pe, tp);
                    }
                    Box b = text.getBox();
                    tl.setLength(tl.length() + b.Size.Width / 2);
                    mLines.Add(tl);
                }
            }
        }

        /// <summary>
        /// 半径寸法線の作成
        /// plist [0] 寸法位置 ([1] 文字位置)
        /// mRefPoints[0] 中心点 [1] 始点 [2] 終点 [3] 寸法位置
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <param name="plist">座標点リスト</param>
        public void createRadiusDimension(ArcD arc, List<PointD> plist)
        {
            mName = "半径寸法線";
            LineD lp = new LineD(arc.mCp, plist[0]);
            PointD pos = plist[0];
            List<PointD> aplist = arc.intersection(lp, false);
            if (aplist.Count == 2) {
                lp.ps = aplist[0].length(pos) < aplist[1].length(pos) ? aplist[0] : aplist[1];
                lp.pe = arc.mCp;
                PointD tp = 1 < plist.Count ? plist[1] : lp.centerPoint();
                mRefPoints = new List<PointD>() {
                    arc.mCp, arc.startPoint(), arc.endPoint(), pos, tp
                };
                mRefValue = new();
                mRefString = new();
                double rotate = lp.angle();
                rotate += rotate <= -Math.PI / 2 ? Math.PI : rotate > Math.PI / 2 ? -Math.PI : 0;
                string mesure = $"R{ylib.double2StrZeroSup(lp.length(), mDimValForm)}";
                TextD text = new TextD(mesure, tp, mTextSize, rotate, HorizontalAlignment.Center, VerticalAlignment.Bottom); ;

                mLines = new List<LineD>();
                mLines.Add(lp);
                mLines.AddRange(arrow(lp.ps, lp.pe));
                mTexts = new List<TextD>();
                mTexts.Add(text);

                if (!lp.onPoint(tp)) {
                    //  文字下線追加
                    LineD tl;
                    if (lp.ps.length(tp) < lp.pe.length(tp)) {
                        tl = new LineD(lp.ps, tp);
                    } else {
                        tl = new LineD(lp.pe, tp);
                    }
                    Box b = text.getBox();
                    tl.setLength(tl.length() + b.Size.Width / 2);
                    mLines.Add(tl);
                }
            }
        }

        /// <summary>
        /// パラメータ(矢印と文字サイズ)を更新して再作成
        /// </summary>
        public void remakeData()
        {
            if (mRefPoints == null || mRefPoints.Count == 0) {
                mRefPoints = getDimensionPos();
            }
            if (mName == "矢印") {
                createArrow(mLines[0].ps, mLines[0].pe);
            } else if (mName == "ラベル" && 0 < mTexts.Count) {
                List<PointD> plist = new List<PointD>() { mLines[0].ps, mLines[0].pe, mTexts[0].mPos };
                createLabel(plist, mTexts[0].mText);
            } else if (mName == "寸法線" && 2 < mRefPoints.Count) {
                createDimension(mRefPoints);
            } else if (mName == "角度寸法線" && 3 < mRefPoints.Count) {
                createAngleDimension(mRefPoints);
            } else if (mName == "直径寸法線" && 3 < mRefPoints.Count) {
                ArcD arc = new ArcD();
                arc.setArc(mRefPoints[0], mRefPoints[1], mRefPoints[2]);
                arc.normalize();
                List<PointD> plist = new List<PointD>() { mRefPoints[3] };
                if (4 < mRefPoints.Count)
                    plist.Add(mRefPoints[4]);
                createDiameterDimension(arc, plist);
            } else if (mName == "半径寸法線" && 3 < mRefPoints.Count) {
                ArcD arc = new ArcD();
                arc.setArc(mRefPoints[0], mRefPoints[1], mRefPoints[2]);
                arc.normalize();
                List<PointD> plist = new List<PointD>() { mRefPoints[3] };
                if (4 < mRefPoints.Count)
                    plist.Add(mRefPoints[4]);
                createRadiusDimension(arc, plist);
            }
        }

        /// <summary>
        /// 寸法線の参照点を求める
        /// </summary>
        /// <returns>座標点リスト</returns>
        public List<PointD> getDimensionPos()
        {
            List<PointD> plist = new List<PointD>();
            if (mName == "寸法線") {
                mLines[0].slide(-mTextSize / 4);
                mLines[1].slide(-mTextSize / 4);
                PointD mp = mLines[2].centerPoint();
                plist.Add(mLines[0].ps);        //  始点
                plist.Add(mLines[1].ps);        //  終点
                plist.Add(mp);                  //  寸法値位置
            } else if (mName == "角度寸法線") {
                PointD cp = mLines[0].intersection(mLines[1]);
                mLines[0].slide(-mTextSize / 4);
                mLines[1].slide(-mTextSize / 4);
                PointD mp = mTexts[0].mPos;
                plist.Add(cp);                  //  中心点
                plist.Add(mLines[0].ps);        //  始点
                plist.Add(mLines[1].ps);        //  終点
                plist.Add(mp);                  //  寸法値位置
            } else if (mName == "直径寸法線") {
                plist.Add(mLines[0].centerPoint()); //  中心点
                plist.Add(mLines[0].ps);            //  始点
                plist.Add(mLines[0].pe);            //  終点
                plist.Add(mLines[0].centerPoint()); //  寸法値位置
            } else if (mName == "半径寸法線") {
                plist.Add(mLines[0].ps);            //  始点(中心点)
                plist.Add(mLines[0].pe);            //  始点
                plist.Add(mLines[0].pe);            //  終点
                plist.Add(mLines[0].centerPoint()); //  寸法値位置
            }
            return plist;
        }

        /// <summary>
        /// 寸法線のタイプ
        /// 1:水平寸法  -1:垂直寸法 0:平行寸法
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        /// <param name="pos">寸法位置</param>
        /// <returns></returns>
        private int getDimType(PointD ps, PointD pe, PointD pos)
        {
            bool horSign = false;
            if (ps.x < pe.x) {
                if (ps.x < pos.x && pos.x< pe.x)
                    horSign = true;
            } else {
                if (pe.x < pos.x && pos.x < ps.x)
                    horSign = true;
            }
            bool verSign = false;
            if (ps.y < pe.y) {
                if (ps.y < pos.y && pos.y < pe.y)
                    verSign = true;
            } else {
                if (pe.y < pos.y && pos.y < ps.y)
                    verSign = true;
            }
            if (horSign && verSign)
                return 0;
            else if (horSign)
                return 1;
            else if (verSign)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// 矢印の先端形状の作成
        /// </summary>
        /// <param name="ps">始点</param>
        /// <param name="pe">終点</param>
        /// <returns>線分リスト</returns>
        public List<LineD> arrow(PointD ps, PointD pe)
        {
            List<LineD> llist = new List<LineD>();
            LineD l = new LineD(ps, pe);
            LineD l0 = l.toCopy();
            l0.rotate(ps, mArrowAngle);
            l0.setLength(mArrowSize);
            llist.Add(l0);
            LineD l1 = l.toCopy();
            l1.rotate(ps, -mArrowAngle);
            l1.setLength(mArrowSize);
            llist.Add(l1);
            return llist;
        }

        /// <summary>
        /// 円弧矢印の先端形状の作成
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>線分リスト</returns>
        public List<LineD> angleArrow(ArcD arc)
        {
            List<LineD> llist = new List<LineD>();
            LineD ls = new LineD(arc.startPoint(), arc.mCp);
            LineD ls0 = ls.toCopy();
            ls0.rotate(ls0.ps, -Math.PI / 2 + mArrowAngle);
            ls0.setLength(mArrowSize);
            LineD ls1 = ls.toCopy();
            ls1.rotate(ls1.ps, -Math.PI / 2 - mArrowAngle);
            ls1.setLength(mArrowSize);

            LineD le = new LineD(arc.endPoint(), arc.mCp);
            LineD le0 = le.toCopy();
            le0.rotate(le0.ps, Math.PI / 2 + mArrowAngle);
            le0.setLength(mArrowSize);
            LineD le1 = le.toCopy();
            le1.rotate(le1.ps, Math.PI / 2 - mArrowAngle);
            le1.setLength(mArrowSize);

            llist.Add(ls0);
            llist.Add(ls1);
            llist.Add(le0);
            llist.Add(le1);
            return llist;
        }

        /// <summary>
        /// ピック位置が文字位置の場合文字位置を追加
        /// </summary>
        /// <param name="plist">座標リスト</param>
        /// <param name="n">寸法値配列位置</param>
        /// <param name="pos">ピック位置</param>
        /// <param name="tp">文字位置</param>
        /// <returns></returns>
        private List<PointD> addTextPos(List<PointD> plist, int n, PointD pos, PointD tp)
        {
            if (textPickChk(pos)) {
                //  寸法値をピックした場合は寸法値を移動
                if (n < plist.Count)
                    plist[n] = tp;
                else
                    plist.Add(tp);
            } else {
                //  寸法値以外をピックした場合、寸法値の位置関係を保持
                if (mName == "直径寸法線" || mName == "半径寸法線") {
                    LineD tl = new LineD(mRefPoints[0], tp);
                    if (tl.length() < mEps)
                        tl = new LineD(mRefPoints[0], mLines[0].pe);
                    tl.setLength(mRefPoints[0].length(mTexts[0].mPos));
                    tp = tl.pe;
                } else if (mName == "角度寸法線") {
                    double ang = mTexts[0].mPos.angle(mRefPoints[0]);
                    LineD tl = new LineD(mRefPoints[0], tp);
                    tl.rotate(tl.ps, ang - tl.angle());
                    tp = tl.pe;
                } else if (mName == "寸法線") {
                    LineD tl = new LineD(mTexts[0].mPos, 1, mTexts[0].mRotate + Math.PI / 2);
                    tp = tl.intersection(tp);
                } else if (mName == "ラベル") {

                } else {
                    plist.RemoveRange(n, plist.Count - n);
                    return plist;
                }
                if (n < plist.Count) {
                    plist[n] = tp;
                } else {
                    plist.Add(tp);
                }
            }
            return plist;
        }

        /// <summary>
        /// 指定点が文字位置に含まれているかの判定
        /// </summary>
        /// <param name="pos">指定点</param>
        /// <returns>文字位置内</returns>
        private bool textPickChk(PointD pos)
        {
            foreach (var text in mTexts)
                if (text.insideChk(pos))
                    return true;
            return false;
        }


        /// <summary>
        /// 要素上の座標かの確認
        /// </summary>
        /// <param name="pos">座標</param>
        /// <returns></returns>
        public PointD onPoint(PointD pos)
        {
            PointD np = new();
            double dis = double.MaxValue;
            if (mLines != null) {
                foreach (var line in mLines) {
                    PointD ip = line.intersection(pos);
                    if (line.onPoint(ip)) {
                        double l = ip.length(pos);
                        if (l < dis) {
                            dis = l;
                            np = ip;
                        }
                    }
                }
            }
            if (mArcs != null) {
                foreach (var arc in mArcs) {
                    PointD ip = arc.intersection(pos);
                    if (arc.onPoint(ip)) {
                        double l = ip.length(pos);
                        if (l < dis) {
                            dis = l;
                            np = ip;
                        }
                    }
                }
            }
            if (mTexts != null) {
                foreach (var text in mTexts) {

                }
            }
            if (dis == double.MaxValue)
                return null;
            else
                return np;
        }

        /// <summary>
        /// Boxが文字列内にあるかをチェック
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool textInsideChk(Box b)
        {
            foreach (var text in mTexts) {
                if (text.insideChk(b))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 点との垂点を求める
        /// </summary>
        /// <param name="point">点座標</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PointD point)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = null;
            foreach (var pline in mLines) {
                ip = pline.intersection(point);
                if (pline.onPoint(ip))
                    plist.Add(ip);
            }
            foreach (var parc in mArcs) {
                ip = parc.intersection(point);
                if (parc.onPoint(ip))
                    plist.Add(ip);
            }
            return plist;
        }

        /// <summary>
        /// 線分との交点を求める
        /// </summary>
        /// <param name="line">線分</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(LineD line)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = null;
            foreach (var pline in mLines) {
                ip = pline.intersection(line);
                if (ip != null && pline.onPoint(ip) && line.onPoint(ip))
                    plist.Add(ip);
            }
            foreach (var parc in mArcs) {
                plist.AddRange(parc.intersection(line));
            }
            return plist;
        }

        /// <summary>
        /// 円弧との交点を求める
        /// </summary>
        /// <param name="arc">円弧</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(ArcD arc)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = null;
            foreach (var pline in mLines) {
                plist.AddRange(pline.intersection(arc));
            }
            foreach (var parc in mArcs) {
                plist.AddRange(parc.intersection(arc));
            }
            return plist;
        }

        /// <summary>
        /// ポリラインとの交点を求める
        /// </summary>
        /// <param name="polyline">ポリライン</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PolylineD polyline)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = null;
            foreach (var pline in mLines) {
                plist.AddRange(polyline.intersection(pline));
            }
            foreach (var parc in mArcs) {
                plist.AddRange(parc.intersection(polyline));
            }
            return plist;
        }

        /// <summary>
        /// ポリゴンとの交点を求める
        /// </summary>
        /// <param name="polygon">ポリゴン</param>
        /// <returns>交点リスト</returns>
        public List<PointD> intersection(PolygonD polygon)
        {
            List<PointD> plist = new List<PointD>();
            PointD ip = null;
            foreach (var pline in mLines) {
                plist.AddRange(polygon.intersection(pline));
            }
            foreach (var parc in mArcs) {
                plist.AddRange(parc.intersection(polygon));
            }
            return plist;
        }
    }
}
