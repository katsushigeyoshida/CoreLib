### CoreLib  
## .NET用のライブラリ  

### 幾何計算用  
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
InputSelect   : メニュー選択、文字列入力ダイヤログ  
MenuDialog    : メニュー選択ダイヤログ(ListBox)  
MessageBoxEx  : MessageBox拡張ダイヤログ  
SelectMenu    : メニュー選択ダイヤログ(ConboBox)  
SelectMenu2   : メニュー選択ダイヤログ(ConboBox+ListBox)  

### グラフィック表示機能  
YDraw      : 2Dグラフィック  
YWorldDraw : 2Dワールド座標グラフィック   

### 3Dグラフィック表示機能  
Point3D     : 3D座標  
Y3DDraw     : 3D表示  
Y3DGraphics : 3D表示  
Y3DParts    : 3D部品表示  

### その他 
YCalc : 数式処理  
YLib  : 種々雑多な関数  

<BR>


echo "# CoreLib" >> README.md  
git init  
git add README.md  
git commit -m "first commit"  
git branch -M main  
git remote add origin https://github.com/katsushigeyoshida/CoreLib.git  
git push -u origin main  
