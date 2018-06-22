using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using BaseModelo.classes;

namespace BasePersistencia.classes
{
    public static class Validador
    {
        /// <summary>
        /// A classe Validator é usada para descobrir se alguma propriedade obrigatória não foi preenchido
        /// </summary>
        /// <param name="t">instância da classe, exemplo: pessoa</param>
        /// <param name="mensagem">parametro de saída - mensagem para ser exibida em caso de campo obrigatório não preenchido</param>
        /// <returns>retorna true se todos os campos required estão preenchidos</returns>
        public static bool IsValid<T>(T t, out string mensagem, out string nome)
        {
            // resposta padrão
            mensagem = string.Empty;
            nome = string.Empty;

            // reflection que pega os membros de um tipo
            List<PropertyInfo> propriedades = Auxiliar.GetListOfProperties(t);

            // vamos percorrer todas as propriedades
            foreach (PropertyInfo propriedade in propriedades)
            {
                // pegamos os "CustomAtributes" da propriedade
                object[] attributes = propriedades.First().GetCustomAttributes(true);

                // pegamos apenas o atributo RequiredAttribute
                var atributo = from attr in attributes
                               where attr is RequiredAttribute
                               select attr;

                // não tem required? ignora a propriedade
                if (atributo == null)
                    continue;

                // se tem required, vamos buscar informações da coluna
                atributo = from attr in attributes
                           where attr is ColumnAttribute
                           select attr;

                // tem?
                if (atributo != null && atributo.Count() > 0)
                {
                    // pega atributo 
                    ColumnAttribute ca = (atributo.First() as ColumnAttribute);
                    if (propriedade.GetValue(t, null) == null)
                    {
                        // verifica se tem atributo de Display (nm_insumo informativo)
                        var display = from attr in attributes
                                      where attr is DisplayAttribute
                                      select attr;

                        mensagem = "O preenchimento de " + (display == null ? ca.Name : (display.First() as DisplayAttribute).Name) + " é obrigatório";
                        nome = propriedade.Name;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}