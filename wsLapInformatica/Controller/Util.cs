using System;
using System.Linq;
using System.Globalization;
using System.Text;

namespace wsLapInformatica.Controller
{
    public static class Util
    {
        public static string Host = "127.0.0.1";
        public static string User = "root";
        public static string Password = "7532159@Lap"; //"s3gur0@az";
        public static string Database = "notificacao";

        public static string RemoverAcentuacao(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        public static string ConvertFormData(string data)
        {
            DateTime dt = Convert.ToDateTime(data);
            data = String.Format("{0:yyyy/MM/dd HH:mm:ss}", dt);
            return data;

        }
    }
}