### CoreLib  
## .NET用のライブラリ  

### 2D幾何計算用  
PointD    : 2D座標、幾何計算用  
LineD.cs  : 線分の幾何計算用  
ArcD      : 円弧の幾何計算用  
EllipseD  : 楕円の幾何計算用  
PolygonD  : ポリゴンの幾何計算用  
PolylineD : ポリラインの幾何計算用  
TextD.cs  : 文字列の幾何計算用  
PartsD    : シンボルの幾何計算用  
Box       : Rectの代替え幾何ライブラリ  
Shigensu  : 4元数  

### 3D幾何計算用  
Point3D    : 3D座標の幾何計算用  
Line3D.cs  : 3D線分の幾何計算用  
Arc3D      : 2D平面円弧の3D幾何計算用  
Polygon3D  : 2D平面ポリゴンの3D幾何計算用  
Polyline3D : 2D平面ポリラインの3D幾何計算用  
Box3D      : 3D領域幾何計算  
Shigensu   : 4元数  

### 拡張機能  
ArrayExtension : 配列の拡張機能  
StringExtensions : 文字列の拡張機能  

### ファイル関連
DiffFolder    : フォルダ比較ダイヤログ  
DirectoryDiff : フォルダ比較クラス  
HashCode      : ハッシュコード  

### ドキュメント解析  
DxfReader    : DXFファイル読込クラス(超簡易版)  
HtmlLib      : HTML解析用  
WikiData     : WikipediaのHTMLデータ抽出解析用  
WikiDataList : Wikipedia抽出データ管理用  

### 各種ダイヤログ  
ChkListDialog : チェックリストダイヤログ  
ColorDialog   : カラー選択ダイヤログ  
FullView      : イメージ全画面表示ダイヤログ  
InputBox      : 文字入力ダイヤログ   
InputBox2     : 文字入力ダイヤログ(2段)  
InputSelect   : メニュー選択、文字列入力ダイヤログ(ComboBox)  
InputSelect2  : メニュー選択、文字列入力ダイヤログ(ComboBox+TextBox)  
MenuDialog    : メニュー選択ダイヤログ(ListBox)  
MessageBoxEx  : MessageBox拡張ダイヤログ  
SelectMenu    : メニュー選択ダイヤログ(ComboBox)  
SelectMenu2   : メニュー選択ダイヤログ(ComboBox+ListBox)  

### グラフィック表示機能  
YDraw      : 2Dグラフィック  
YWorldDraw : 2Dワールド座標グラフィック   

### 3Dグラフィック表示機能  
Y3DDraw     : 3D表示  
Y3DGraphics : 3D表示  
Y3DParts    : 3D部品表示  

### その他 
YCalc : 数式処理  
YLib  : 種々雑多な関数  

<BR>

### 履歴  
2023/06/11 YLibに統計関数の追加,YCalcにRandom関数追加  
2026/06/05 YLibに有効桁数で丸める関数を追加  
2026/05/08 複数項目入力ダイヤログ追加、スクリプト処理機能修正  
2026/04/23 Geometry/PartsDの矢印とラベルの折れ点複数化  
2025/09/14 スクリプト関数にマトリックス関数追加  
2025/08/29 Geometry/Arc3D,Plane3D,Polyline3Dの座標変換修正  
2025/07/25 スクリプト処理の配列処理の修正、Geometry/Plane3Dのベース変更  
2025/07/09 スクリプト拡張関数の追加  
2025/06/14 スクリプト処理をライブラリに追加、ディレトリ構成見直し  
2025/05/04 ポリライン、ポリゴンの円弧変換の修正  
2025/04/03 交点計算、面分割に対する修正  
2025/04/03 交点処理の修正、2Dから指定平面の3D変換を修正  
2025/01/19 楕円の始終角の角度変換の追加、円弧表示の修正など  
 :  
 :  



echo "# CoreLib" >> README.md  
git init  
git add README.md  
git commit -m "first commit"  
git branch -M main  
git remote add origin https://github.com/katsushigeyoshida/CoreLib.git  
git push -u origin main  
