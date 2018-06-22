using System;
using System.Collections;
using System.IO;
using System.Web;

namespace Funcoes.classes
{
    public class Log
    {
        private enum TipoLog { Erro, Sql, Debug };

        public static void Erro(string linha)
        {
            Write(linha, TipoLog.Erro);
        }

        public static void Sql(string linha)
        {
            Write(linha, TipoLog.Sql);
        }

        public static void Debug(string linha)
        {
            Write(linha, TipoLog.Debug);
        }

        private static void Write(string linha, TipoLog tipo)
        {
            try
            {
                string[] tipoSt = new string[3] { "erro", "sql", "debug" };
                
                // o nome do arquivo é a data ano-mes-dia + tipo 
                string path = "~/logs/" + DateTime.Today.ToString("yy-MM-dd") + "_" + tipoSt[(int)tipo] + ".txt";

                // se não existe o arquivo, cria...
                if (!File.Exists(HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(HttpContext.Current.Server.MapPath(path)).Close();
                }

                // adiciona ao arquivo existente... (AppendText)
                using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath(path)))
                {
                    // formato é hora  URL  mensagem do parâmetro e linha divisória
                    w.WriteLine("{0}  {1}\r\n{2}\r\n{3}",
                        DateTime.Now.ToUniversalTime().AddHours(-3).ToString("HH:mm:ss"), 
                        HttpContext.Current.Request.Url.ToString(), 
                        linha,
                        string.Concat(ArrayList.Repeat('-', 150).ToArray()));
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }
    }
}