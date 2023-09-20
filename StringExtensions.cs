using System;

namespace ConsoleApp
{
    /// <summary>
    /// 文字列の拡張メソッド
    /// https://johobase.com/custom-extension-methods-list/
    /// </summary>
    public static class StringExtensions
    {
        public static int Hex2Int(this string s)
        {
            return Convert.ToInt32(s, 16);
        }

        /// <summary>
        /// Format の簡易版。 (string.Format(format, arg0) が format.Format(arg0) で呼び出せます。)
        /// </summary>
        /// <param name="value">複合書式設定文字列</param>
        /// <param name="arg0">書式指定するオブジェクト</param>
        /// <returns>書式設定した文字列を返します。</returns>
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        /// <summary>
        /// 文字列をbool型に変換する。変換できなかった場合、 alternativeValue を返す。
        /// alternateValueの初期値は false。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="alternativeValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string str, bool? alternativeValue = null)
        {
            return (bool.TryParse(str, out bool value) ? value : alternativeValue ?? false);
        }

        /// <summary>
        /// 文字列をint型に変換する。変換できなかった場合、 alternativeValue を返す。
        /// alternateValueの初期値は 0。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="alternativeValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int? alternativeValue = null)
        {
            return (int.TryParse(str, out int value) ? value : alternativeValue ?? 0);
        }

        /// <summary>
        /// 文字列をdouble型に変換する。変換できなかった場合、 alternativeValue を返す。
        /// alternateValueの初期値は  double.NaN。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="alternativeValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string str, double? alternativeValue = null)
        {
            return (double.TryParse(str, out double value) ? value : alternativeValue ?? double.NaN);
        }

        /// <summary>
        /// 文字列を日付型に変換する。変換できなかった場合、 alternativeValue を返す。
        /// alternateValueの初期値は DateTime.MaxValue。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="alternativeValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime? alternativeValue = null)
        {
            return (DateTime.TryParse(str, out DateTime value) ? value : alternativeValue ?? DateTime.MinValue);
        }

        /// <summary>
        /// 文字列をString.Formatのフォーマット文字に見立て、可変長引数を渡して文字列を作成する
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Args(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
