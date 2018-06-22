using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Funcoes.classes
{
    public static class StringHelper
    {
        public static int ToInt(this string s)
        {
            int retorno;
            if (!int.TryParse(s, out retorno))
                retorno = 0;
            return retorno;
        }

        public static string SemAcentos(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return String.Empty;
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(s);
                return Encoding.UTF8.GetString(bytes);
            }
        }

        public static string URLAmigavel(this string s)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(s, "").Replace(' ', '_');
            //return HttpUtility.UrlEncode(s.SemAcentos().Replace(' ', '-').Replace('/', '-').ToLower());
        }
    }
}
