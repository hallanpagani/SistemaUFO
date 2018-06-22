using System;
using System.Linq.Expressions;
using System.Text;
using BaseModelo.classes;

namespace BasePersistencia.banco
{
    public enum FiltrosOperador { And, Or };

    public class Filtros
    {
        private StringBuilder sbFiltros;
        object objetoModel;

        // private porque é obrigatório instancia a versão que recebe um objeto
        private Filtros()
        {
        }

        public Filtros(object ObjetoModel)
        {
            sbFiltros = new StringBuilder();
            objetoModel = ObjetoModel;
        }

        /// <summary>
        /// adiciona um filtro para usar com select, use toString para pegar o filtro completo
        /// para os parâmetros abaixo, tenha por exemplo:
        /// Pessoa p = new Pessoa();
        /// Filtros f = new Filtros(p);
        /// f.Add(() => p.Nome, FiltroExpressao.ComecaCom, "M");
        /// </summary>
        /// <param name="exp">Propridade em formato expressão Lambda (() => x.Nome)</param>
        /// <param name="conteudoFiltro">Valor para o filtro (100, "Maria", etc)</param>
        /// <param name="expressao">FiltroExpressao.*</param>
        /// <param name="prefixoCampo">quando é um join que campo repete em mais tabelas coloque um prefixo</param>
        /// <param name="operador">FiltroOperador.And ou FiltroOperador.Or - apenas para se usar mais de um filtro</param>
        public Filtros Add<T>(Expression<Func<T>> exp, object conteudoFiltro, FiltroExpressao expressao = FiltroExpressao.Igual, string prefixoCampo = "", FiltrosOperador operador = FiltrosOperador.And)
        {
            string campo = Auxiliar.GetFieldName(exp, objetoModel);
            this.Add(campo, expressao, conteudoFiltro, prefixoCampo, null, operador);
            return this;
        }

        /// <summary>
        /// adiciona um filtro com between para usar com select, use toString para pegar o filtro completo
        /// para os parâmetros abaixo, tenha por exemplo:
        /// Pessoa p = new Pessoa();
        /// Filtros f = new Filtros(p);
        /// f.AddEntre(() => p.Valor, 1, 20);
        /// </summary>
        /// <param name="exp">Propridade em formato expressão Lambda (() => x.Nome)</param>
        /// <param name="valorInicial">Valor inicial para o filtro (numeros ou datas)</param>
        /// <param name="valorFinal">Valor inicial para o filtro (numeros ou datas)</param>
        /// <param name="prefixoCampo">quando é um join que campo repete em mais tabelas coloque um prefixo</param>
        /// <param name="operador">FiltroOperador.And ou FiltroOperador.Or - apenas para se usar mais de um filtro</param>
        public Filtros AddEntre<T>(Expression<Func<T>> exp, object valorInicial, object valorFinal, string prefixoCampo = "", FiltrosOperador operador = FiltrosOperador.And)
        {
            string campo = Auxiliar.GetFieldName(exp, objetoModel);
            this.Add(campo, FiltroExpressao.Entre, valorInicial, prefixoCampo, valorFinal, operador);
            return this;
        }

        /// <summary>
        /// adiciona um filtro usando esquema de LIKE
        /// Pessoa p = new Pessoa();
        /// Filtros f = new Filtros();
        /// f.AddLike(() => p.Nome, p, "M");
        /// </summary>
        /// <param name="exp">Propridade em formato expressão Lambda (() => x.Nome)</param>
        /// <param name="obj">Instância do objeto que contém a propriedade do primeiro parâmetro (ex: p)</param>
        /// <param name="conteudoFiltro">Valor para o filtro ("Maria")</param>
        /// <param name="operador">FiltroOperador.And ou FiltroOperador.Or</param>
        public Filtros AddLike<T>(Expression<Func<T>> exp, string conteudoFiltro, string prefixoCampo = "", FiltrosOperador operador = FiltrosOperador.And)
        {
            string campo = Auxiliar.GetFieldName(exp, objetoModel);
            sbFiltros.Append(SqlUtil.MontarLike(prefixoCampo + campo, conteudoFiltro));
            AdicionarOperador(operador);
            return this;
        }

        /// <summary>
        /// Veja o outro overload para descrição e exemplo de uso.
        /// </summary>
        protected Filtros Add(string campo, FiltroExpressao expressao, object valorInicial, string prefixoCampo = "", object valorFinal = null, FiltrosOperador operador = FiltrosOperador.And)
        {
            Filtro filtro = new Filtro(campo, expressao, valorInicial, valorFinal, prefixoCampo);
            sbFiltros.Append(filtro.ToString());
            AdicionarOperador(operador);
            return this;
        }

        /// <summary>
        /// adiciona um "not" na frente da próxima expressão
        /// </summary>
        public Filtros AddNot()
        {
            sbFiltros.Append(" not ");
            return this;
        }

        /// <summary>
        /// abre um grupo de filtros, por exemplo você quer todos clientes que começam com X e tenho codigo > 100 -OU- começam com Z e código menor que 100
        /// então você tem 2 grupos:
        /// filtro.AddGrupo();
        /// filtro.Add(() => e.Nome, e, FiltroExpressao.ComecaCom, "X");
        /// filtro.Add(() => e.Id, e, FiltroExpressao.Maior, 100);
        /// f.FecharGrupo(FiltrosOperador.Or);
        /// filtro.AddGrupo();
        /// filtro.Add(() => e.Nome, e, FiltroExpressao.ComecaCom, "Z");
        /// filtro.Add(() => e.Id, e, FiltroExpressao.Menor, 100);
        /// f.FecharGrupo();
        /// </summary>
        public Filtros AbrirGrupo()
        {
            sbFiltros.Append("(");
            return this;
        }

        /// <summary>Veja AbrirGrupo()</summary>
        public Filtros FecharGrupo(FiltrosOperador operador = FiltrosOperador.And)
        {
            RemoverOperadorDoFinal();
            sbFiltros.Append(")");
            AdicionarOperador(operador);
            sbFiltros.Append("\n");
            return this;
        }

        /// <summary>Limpar os filtros [opcional]</summary>
        public void Clear()
        {
            sbFiltros.Clear();
        }

        /// <summary>Retorna os filtros em formato SQL</summary>
        public override string ToString()
        {
            RemoverOperadorDoFinal();
            return sbFiltros.ToString();
        }

        #region private
        // adiciona um operador, uso interno
        private void AdicionarOperador(FiltrosOperador operador)
        {
            switch (operador)
            {
                case FiltrosOperador.And: sbFiltros.Append(" and"); break;
                case FiltrosOperador.Or: sbFiltros.Append(" or"); break;
            }
        }

        // uso interno, remove o "and" ou "or" de lixo do final
        private void RemoverOperadorDoFinal()
        {
            if (sbFiltros.Length > 4)
            {
                string final = sbFiltros.ToString().Substring(sbFiltros.Length - 4, 4).Trim();
                if (final.Equals("and"))
                    sbFiltros.Remove(sbFiltros.Length - 4, 4);
                if (final.Equals("or"))
                    sbFiltros.Remove(sbFiltros.Length - 3, 3);
            }
        }
        #endregion
    }
}
