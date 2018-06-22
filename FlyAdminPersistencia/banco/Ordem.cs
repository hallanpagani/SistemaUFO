using System;
using System.Linq.Expressions;
using System.Text;
using BaseModelo.classes;

namespace BasePersistencia.banco
{
    public enum OrdemTipo { Asc, Desc };

    /// <summary>
    /// Classe de uso interno, para montar order by para SQL
    /// </summary>
    public class Ordem
    {
        private StringBuilder orderBy;
        private object objetoModel;

        // setado como privado, pois a versão com parametro é obrigatória
        private Ordem()
        {
        }

        /// <summary>
        /// Monta um order by com base em Linq. 
        /// </summary>
        /// <code>
        ///   Funcionario fun = new Funcionario();
        ///   Ordem ordem = new Ordem(fun);
        ///   ordem.Add(() => fun.Nome, OrdemTipo.Asc);
        /// </code>
        /// <param name="ObjetoModel">Objeto que será usado para extrair nm_insumo dos campos a partir da propriedade adicionada via médoto Add com LINQ</param>
        public Ordem(object ObjetoModel)
        {
            this.orderBy = new StringBuilder();
            objetoModel = ObjetoModel;
        }

        /// <summary>
        /// adiciona um campo para ordenar um select (use o ToString())
        /// </summary>
        /// <param name="exp">Propridade em formato expressão Lambda (() => x.Nome)</param>
        /// <param name="obj">Instância do objeto que contém a propriedade do primeiro parâmetro (ex: p)</param>
        /// <param name="operador">OrdemTipo.Asc (default) ou OrderTipo.Desc</param>
        public Ordem Add<T>(Expression<Func<T>> exp, OrdemTipo tipo = OrdemTipo.Asc)
        {
            string campo = Auxiliar.GetFieldName(exp, objetoModel);
            this.Add(campo, tipo);
            return this;
        }

        /// <summary>
        /// veja o outro overload para exemplo (é igual, só troca os dois primeiros parâmetros pelo nm_insumo do campo em string
        /// </summary>
        protected Ordem Add(string nomeCampo, OrdemTipo tipo = OrdemTipo.Asc)
        {
            if (this.orderBy.Length > 0)
                this.orderBy.Append(",");

            this.orderBy.Append(nomeCampo);

            if (tipo == OrdemTipo.Desc)
                this.orderBy.Append(" desc");

            return this;
        }

        public void Clear()
        {
            this.orderBy.Clear();
        }

        /// <summary>
        /// retorna em formato SQL os campos para order by
        /// </summary>
        public override string ToString()
        {
            return this.orderBy.ToString();
        }
    }
}
