using System;
using System.Globalization;
using System.Text;

namespace BasePersistencia.banco
{
    public enum FiltroExpressao { Igual, Maior, Menor, MaiorIgual, MenorIgual, Entre, Diferente, ComecaCom, TerminaCom, Contem };

    /// <summary>
    /// Classe de uso interno, para montar filtros para SQL, usada pela classe "Filtros"
    /// </summary>
    class Filtro
    {
        private string campo { get; set; }
        private object valorInicial { get; set; }
        private object valorFinal { get; set; }
        private FiltroExpressao filtroExpressao { get; set; }

        private Filtro() { }

        public Filtro(string nomeCampo, FiltroExpressao expressao, object valorInicial, object valorFinal = null, string prefixoCampo = "")
        {
            this.campo = string.Format("{0}{1}", prefixoCampo, nomeCampo);
            this.filtroExpressao = expressao;
            this.valorInicial = valorInicial;
            this.valorFinal = valorFinal;
        }

        private string GetValorQuoted(string valor)
        {
            return string.Format("'{0}'", valor);
        }

        // específicas de string
        private string GetFiltroString()
        {
            switch (filtroExpressao)
            {
                case FiltroExpressao.ComecaCom: return string.Format(" like '{0}%'", (string)valorInicial);
                case FiltroExpressao.TerminaCom: return string.Format(" like '%{0}'", (string)valorInicial);
                case FiltroExpressao.Contem: return string.Format(" like '%{0}%'", (string)valorInicial);
                case FiltroExpressao.Igual: return string.Format("='{0}'", (string)valorInicial);
                case FiltroExpressao.Diferente: return string.Format("<>'{0}'", (string)valorInicial);
            }
            return string.Empty;
        }

        // específicas de date
        private string GetFiltroDate()
        {
            DateTime inicial = (DateTime)valorInicial;
            string inicialStr = inicial.ToString("yyyy-MM-dd");

            switch (filtroExpressao)
            {
                case FiltroExpressao.Igual: return string.Format("='{0}'", inicialStr);
                case FiltroExpressao.Diferente: return string.Format("<>'{0}'", inicialStr);
                case FiltroExpressao.Maior: return string.Format(">'{0}'", inicialStr);
                case FiltroExpressao.Menor: return string.Format("<'{0}'", inicialStr);
                case FiltroExpressao.MaiorIgual: return string.Format(">='{0}'", inicialStr);
                case FiltroExpressao.MenorIgual: return string.Format("<='{0}'", inicialStr);
                case FiltroExpressao.Entre:
                    DateTime fim = (DateTime)valorFinal;
                    return string.Format(" between '{0}' and '{1}'", inicialStr, fim.ToString("yyyy-MM-dd"));
            }
            return string.Empty;
        }

        private string GetFiltroNumerico<T>()
        {
            switch (filtroExpressao)
            {
                case FiltroExpressao.Igual: return string.Format("={0}", (T)valorInicial);
                case FiltroExpressao.Diferente: return string.Format("<>{0}", (T)valorInicial);
                case FiltroExpressao.Entre: return string.Format(" between {0} and {1}", (T)valorInicial, (T)valorFinal);
                case FiltroExpressao.Maior: return string.Format(">{0}", (T)valorInicial);
                case FiltroExpressao.Menor: return string.Format("<{0}", (T)valorInicial);
                case FiltroExpressao.MaiorIgual: return string.Format(">={0}", (T)valorInicial);
                case FiltroExpressao.MenorIgual: return string.Format("<={0}", (T)valorInicial);
            }
            return string.Empty;
        }

        // específicas de decimal
        private string GetFiltroDecimal()
        {
            return GetFiltroNumerico<decimal>();
        }

        // específicas de double
        private string GetFiltroDouble()
        {
            return GetFiltroNumerico<double>();
        }

        // específicas de inteiro
        private string GetFiltroInt()
        {
            return GetFiltroNumerico<int>();
        }

        private string GetFiltroLong()
        {
            return GetFiltroNumerico<long>();
        }

        public override string ToString()
        {
            StringBuilder retorno = new StringBuilder(" ");
            retorno.Append(campo);

            if (valorInicial.GetType() == typeof(string))
                retorno.Append(GetFiltroString());

            if (valorInicial.GetType() == typeof(int))
                retorno.Append(GetFiltroInt());

            if (valorInicial.GetType() == typeof(DateTime))
                retorno.Append(GetFiltroDate());

            if (valorInicial.GetType() == typeof(long))
                retorno.Append(GetFiltroLong());

            if (valorInicial.GetType() == typeof(decimal))
                retorno.Append(GetFiltroDecimal().Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator, CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator));

            if (valorInicial.GetType() == typeof(double))
                retorno.Append(GetFiltroDouble().Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator, CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator));

            return retorno.ToString();
        }
    }
}
