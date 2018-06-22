using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using BaseModelo.classes;

namespace BasePersistencia.classes
{
    /// <summary>
    /// classe usada para obter o nm_insumo da tabela, lista de campos e valores dos campos a partir de um object do model
    /// não é usada diretamente, mas uma classe auxiliar da classe DAL. Tanto é que nem é pública
    /// </summary>
    class Montador
    {
        /// <summary>
        /// procura nos atributos do tipo ex: typeof(Pessoa)
        /// um atributo do tipo "TabelaAtributo"
        /// retorna o nm_insumo da tabela se achar
        /// </summary>
        /// <param name="tipo">tipo do objeto, declare typeof(Classe)</param>
        /// <returns>retorna o nm_insumo da tabela</returns>
        public static string GetTableName(object obj)
        {
            // usa reflection para extrair os "Custom" atributes - aqueles criados manualmente 
            Attribute[] atributos = Attribute.GetCustomAttributes(obj.GetType());

            // pega todos atributos do tipo TableAttribute
            var attr =
                from at in atributos
                where (at is TableAttribute)
                select at;

            // não tem? gera uma exceção
            if (attr == null || attr.Count() == 0)
                throw new Exception("Nome da tabela não identificada");

            // retorna o nm_insumo do atributo (do primeiro, caso tenha mais de um)
            return (attr.First() as TableAttribute).Name;
        }

        // conforme a propriedade que ler, seta propriedades diferente de "Campo"
        private static bool SetProperties(object attribute, Campo campo)
        {
            if (attribute is ColumnAttribute)
            {
                campo.Nome = (attribute as ColumnAttribute).Name;
                return true;
            }

            if (attribute is KeyAttribute)
            {
                campo.IsKey = true;
                return false;
            }

            if (attribute is AutoIncAttribute)
            {
                campo.IsAutoInc = true;
                return false;
            }

            if (attribute is RequiredAttribute)
            {
                campo.Required = true;
                return false;
            }

            if (attribute is ForeignKeyAttribute)
            {
                campo.IsFK = true;
                // pega o nm_insumo do campo FK
                campo.NomeFK = (attribute as ForeignKeyAttribute).Name;
                return false;
            }

            if (attribute is OnlyInsertAttribute)
            {
                campo.IsOnlyInsert = true;
                return false;
            }

            if (attribute is OnlyUpdateAttribute)
            {
                campo.IsOnlyUpdate = true;
                return false;
            }

            if (attribute is OnlySelectAttribute)
            {
                campo.IsOnlySelect = true;
                return false;
            }

            return false;
        }

        /// <summary>
        /// método que recebe uma instância de uma instância de classe e um tipo
        /// pega os membros desse tipo e lê atributos custom
        /// para retornar o nm_insumo do campo (retorna num list de campo)
        /// e também busca na instância da classe (primeiro parametro) o valor
        /// </summary>
        /// <param name="t">instância da classe, exemplo: pessoa</param>
        /// <returns>retorna um list de Campo que contém o nm_insumo do campo e valor a ser gravado na base</returns>
        public static List<Campo> GetCampos<T>(T t)
        {
            // cria o retorno vazio
            List<Campo> retorno = new List<Campo>();

            // pega lista de propriedades
            List<PropertyInfo> propriedades = Auxiliar.GetListOfProperties(t);                 

            // percorre estas destas propriedades 
            foreach (PropertyInfo propriedade in propriedades)
            {
                // pegamos atributos desta propriedade
                object[] atributos = propriedade.GetCustomAttributes(true);

                // não tem atributos, pula
                if (atributos == null || atributos.Count() == 0)
                    continue;

                Campo campo = new Campo();

                // percorre os atributos customizados (neste exemplo só tem 1: CampoAttribute)
                foreach (object atributo in atributos)
                {
                    // esse SetProperties volta true se obter o atributo ColumnAttribute, nos demais false
                    if (Montador.SetProperties(atributo, campo))
                    {
                        // já percorre as propriedades do objeto e procura uma propriedade com mesmo nm_insumo do member.Nome
                        // achou? então já temos o valor deste campo (usado no insert, update)
                        campo.Valor = propriedade.GetValue(t, null);
                        // já aproveita e preenche o tipo para uso nos "where" de selects"
                        campo.Tipo = propriedade.PropertyType;
                    }
                }

                // se tem valor adiciona a lista
                if (!string.IsNullOrEmpty(campo.Nome))
                {
                    retorno.Add(campo);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Retornar o id do campo Mestre encontrado no objeto (trabalha só com 1 campo key)
        /// </summary>
        /// <param name="t">instância da classe, exemplo: pessoa</param>
        /// <returns>retorna um long com código do id</returns>
        public static long GetKeyId<T>(T t)
        {
            // pega lista de propriedades
            List<PropertyInfo> propriedades = Auxiliar.GetListOfProperties(t);

            // percorre estas destas propriedades e verifica se tem o atributo [Key]
            foreach (PropertyInfo propriedade in propriedades)
            {
                // pegamos atributos que são KeyAttribute
                var atributos = from attr in propriedade.GetCustomAttributes(true)
                                where attr is KeyAttribute
                                select attr;

                // não tem KeyAttribute? pega a próxima propriedade
                if (atributos != null && atributos.Count() > 0)
                {
                    // pega o valor da propriedade
                    object valor = propriedade.GetValue(t, null);
                    if (valor.GetType() == typeof(int))
                        return (int)valor;
                }
            }
            throw new Exception("Não encontrada propriedade KeyAttribute de " + t.ToString());
        }

        /// <summary>
        /// Retornar o nm_insumo do campo FK
        /// </summary>
        /// <param name="t">instância da classe, exemplo: pessoa</param>
        /// <returns>retorna um string com nm_insumo do campo FK</returns>
        public static string GetFieldFK<T>(T t)
        {
            // pega lista de propriedades
            List<PropertyInfo> propriedades = Auxiliar.GetListOfProperties(t);

            // percorre estas destas propriedades e verifica se tem o atributo [Key]
            foreach (PropertyInfo propriedade in propriedades)
            {
                // pegamos atributos que são FK
                var atributos = from attr in propriedade.GetCustomAttributes(true)
                                where attr is ForeignKeyAttribute
                                select attr;

                // não tem KeyAttribute? pega a próxima propriedade
                if (atributos != null && atributos.Count() > 0)
                {
                    return (atributos.First() as ForeignKeyAttribute).Name;
                }
            }
            return string.Empty;
        }
    }
}