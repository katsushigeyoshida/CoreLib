using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace CoreLib
{
    public class IconExtractor
    {
        // ファイルに関連づけられたアイコンを取得するAPI関数
        [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon")]
        static private extern IntPtr ExtractAssociatedIcon(
            IntPtr hInst,
            [MarshalAs(UnmanagedType.LPStr)] string lpIconPath,
            ref int lpiIcon);

        // ファイルに含まれるアイコンを取得するAPI関数
        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        static private extern IntPtr ExtractIcon(
            IntPtr hInst,
            [MarshalAs(UnmanagedType.LPStr)] string lpszExeFileName,
            int nIconIndex);

        static private IntPtr hInst;
        static private Hashtable m_ExtensionIndex;     //	Icon
        static private List<Icon> smallImageList;
        static private List<Icon> largeImageList;
        static private string[,] m_ShellIconName = new string[,] {
            {"0","file"},
            {"3","folder"},
            {"4","openFolder"},
            {"6","floppy"},
            {"7","removableDisk"},
            {"8","hardDisk"},
            {"9","networkDisk"},
            {"11","CDDrive"},
            {"12","memoryDrive"},
            {"13","networkRoot"},		//	ネットワーク全体
			{"14","network"},			//	Microsoft....
			{"15","networkServer"},		//	
			{"17","myNetwork"},			//	マイ ネットワーク
			{"18","networkDomain"},		//	ワークグループ
			{"20","myDocument"},
            {"33","modemConnect"},		//	モデム接続
			{"34","desktop"},			//	
			{"85","networkFolder"},
            {"126","myDocument"},
        };

        // コンストラクタ
        //public IconExtractor()
        //{
        //    hInst = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
        //    smallImageList = new List<Icon>();
        //    largeImageList = new List<Icon>();
        //    m_ExtensionIndex = new Hashtable(); //	Icon
        //}

        //public IconExtractor(List<Icon> smallImageList, List<Icon> largeImageList)
        //{
        //    hInst = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
        //    m_ExtensionIndex = new Hashtable(); //	Icon
        //    this.smallImageList = smallImageList;
        //    this.largeImageList = largeImageList;
        //}

        //public IconExtractor(List<Icon> smallImageList)
        //{
        //    hInst = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
        //    m_ExtensionIndex = new Hashtable(); //	Icon
        //    this.smallImageList = smallImageList;
        //    this.largeImageList = null;
        //}

        //public void LargeImageListSet(List<Icon> imageList)
        //{
        //    largeImageList = imageList;
        //}

        //public void SmallImageListSet(List<Icon> imageList)
        //{
        //    smallImageList = imageList;
        //}

        // ファイルに関連づけられたアイコンを取得する
        static public Icon ExtractAssociatedIcon(string fileName)
        {
            if (File.Exists(fileName) || Directory.Exists(fileName)) {
                int i = 0;

                IntPtr hIcon = ExtractAssociatedIcon(hInst, fileName, ref i);

                if (!hIcon.Equals(IntPtr.Zero)) {
                    return Icon.FromHandle(hIcon);
                } else {
                    return null;
                }
            } else {
                return null;
            }
        }

        //	shell32.dllのアイコンを取り出す
        static public Icon ExtractShellIcon(int index)
        {
            // shell32.dllへのパスを作成する
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string path = Path.Combine(systemFolder, "shell32.dll");
            return ExtractIcon(path, index);
        }

        /// <summary>
        /// ファイルからアイコンを抽出
        /// </summary>
        /// <param name="fileName">ファイルパス</param>
        /// <returns></returns>
        static public BitmapSource ExtractIconBitmapSource(string fileName, int index = 0)
        {
            return Icon2BitmapSource(ExtractIcon(fileName, index));
        }

        // ファイルからアイコンを抜き出す
        // デフォルトでインデックス0のアイコンが抜き出される
        static public Icon ExtractIcon(string fileName)
        {
            return ExtractIcon(fileName, 0);
        }

        // ファイルからアイコンを抜き出す
        // インデックスを指定することができる
        static public Icon ExtractIcon(string fileName, int index)
        {
            // ファイルに含まれるアイコンを列挙する
            if (File.Exists(fileName) || Directory.Exists(fileName)) {
                System.IntPtr hIcon;

                // ファイルに含まれるアイコンの総数を取得
                hIcon = ExtractIcon(hInst, fileName, -1);

                // 取得できなかった場合
                if (hIcon.Equals(IntPtr.Zero)) {
                    // ファイルに関連付けられたアイコンを取得
                    return ExtractAssociatedIcon(fileName);
                } else {
                    // アイコン数
                    int numOfIcons = hIcon.ToInt32();

                    // ファイルに含まれるすべてのアイコンを取得
                    if (0 <= index && index < numOfIcons) {
                        hIcon = ExtractIcon(hInst, fileName, index);

                        if (!hIcon.Equals(IntPtr.Zero)) {
                            return Icon.FromHandle(hIcon);
                        } else {
                            return null;
                        }
                    } else {
                        return null;
                    }
                }
            } else {
                return null;
            }
        }


        //	アイコンをイメージリストに登録する
        static public int GetExractIcon(string path, string ext)
        {
            string key;
            Icon appIcon = null;
            try {
                ext = ext.ToLower();
                if (ext == ".ico" || ext == ".bmp" || ext == ".exe" || ext == ".lnk")
                    key = path;
                else
                    key = ext;
                if (m_ExtensionIndex.ContainsKey(key))
                    return (int)m_ExtensionIndex[key];
                //				Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                if (path.Length == 0) {
                    for (int i = 0; i < m_ShellIconName.GetLength(0); i++) {
                        if (ext.CompareTo((string)m_ShellIconName[i, 1].ToLower()) == 0) {
                            appIcon = ExtractShellIcon(int.Parse(m_ShellIconName[i, 0]));
                            //							System.Diagnostics.Debug.WriteLine("Select ICON :" + ext + " (" + m_ShellIconName[i, 0]+")");
                            break;
                        }
                    }
                    if (appIcon == null) {
                        System.Diagnostics.Debug.WriteLine("ICON error :" + ext);
                        appIcon = ExtractShellIcon(3);
                    }
                } else {
                    appIcon = ExtractIcon(path);
                }
                int n = 0;
                if (smallImageList != null) {
                    smallImageList.Add(appIcon);     //	ImageList
                    n = smallImageList.Count - 1;
                }
                if (largeImageList != null) {
                    largeImageList.Add(appIcon);     //	ImageList
                    n = largeImageList.Count - 1;
                }
                m_ExtensionIndex.Add(key, n);
                return n;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"GetExractIcon: {e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Icon を BitmapSource に変換
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        static public BitmapSource Icon2BitmapSource(Icon icon)
        {
            try {
                using Bitmap bitmap = icon.ToBitmap();
                // Bitmap → BitmapSource
                BitmapSource bitmapSource = ConvertBitmapToBitmapSource(bitmap);
                return bitmapSource;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine($"Icon2BitmapSource: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Bitmap を BitmapSourceに変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        static private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            // GDI ハンドルから BitmapSource を作成
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
        }

    }
}
