using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BaseModelo.classes
{
    public static class Auxiliar
    {
        // obtém de um objeto todas as propriedades que possuem Attributes
        public static List<PropertyInfo> GetListOfProperties(object objeto)
        {
            var propriedades = from prop in objeto.GetType().GetProperties()
                               where prop.GetCustomAttributes().Count() > 0
                               select prop;
            return propriedades.ToList<PropertyInfo>();
        }

        /*
         * http://stackoverflow.com/questions/4657311/reflection-get-property-name
         * 
        public class Pessoa
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
            public int Idade { get; set; }
        }

            string propName1 = GetPropertyName(() => new Pessoa().Nome);
            string propName2 = GetPropertyName(() => new Pessoa().Codigo);
            string propName3 = GetPropertyName(() => new Pessoa().Idade);

            Console.WriteLine(propName1 + ", " + propName2 + ", " + propName3);
        */
        /// <summary>
        /// Método privado que recebe uma expressão lambda contendo um objeto.propriedade
        /// e executando um reflection pega o nm_insumo dessa propriedade em forma de string.
        /// Veja no fonte alguns exemplos
        /// </summary>
        private static string GetPropName<T>(Expression<Func<T>> exp)
        {
            var me = exp.Body as MemberExpression;
            if (me == null)
                return null;
            else
                return me.Member.Name;
        }

        /// <summary>
        /// Método que recebe uma expressão lambda contendo um objeto.propriedade
        /// e executando um reflection pega o nm_insumo do campo vinculado ao atributo CampoAttribute desta propriedade
        /// </summary>
        public static string GetFieldName<T, O>(Expression<Func<T>> exp, O obj)
        {
            // aqui transforma a propriedade em string (entra Usuario.Nome e sai "Nome")
            string nomePropridade = GetPropName(exp);

            // aqui pegamos os "membros" do objeto pai da propriedade
            PropertyInfo propriedade = Auxiliar.GetListOfProperties(obj)
                .Where(x => x.Name.Equals(nomePropridade)).FirstOrDefault();

            if (propriedade == null)
                throw new Exception("Campo da propriedade " + nomePropridade + " do objeto " + obj.GetType().Name + " não localizado");

            // pegamos oos "CustosAtributes"
            object[] attributes = propriedade.GetCustomAttributes(true);

            // pegamos apenas o atributo ColumnAttribute
            var atributo = from attr in attributes
                           where attr is ColumnAttribute
                           select attr;

            if (atributo == null)
                throw new Exception("Campo da propriedade " + nomePropridade + " não localizado");

            // retorna somente o nm_insumo
            return (atributo.First() as ColumnAttribute).Name;
        }

        #region funções auxiliares para obter informações de propriedades
        public static List<PropertyInfo> PropertySimple(Type type)
        {
            object data = GetObject(type);
            return new List<PropertyInfo>(from PropertyInfo p in Property(type)
                                          where (GetColumnAttribute(p) != null)
                                          select p);
        }

        public static List<PropertyInfo> PropertySimple(object data)
        {
            return PropertySimple(data.GetType());
        }

        public static PropertyInfo GetProperty(object data, string propertyName)
        {
            List<PropertyInfo> list = PropertySimple(data);

            foreach (PropertyInfo property in data.GetType().GetProperties())
                if (property.Name == propertyName)
                    return property;

            return null;
        }

        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            List<PropertyInfo> list = PropertySimple(type);

            foreach (PropertyInfo property in type.GetProperties())
                if (property.Name == propertyName)
                    return property;

            return null;
        }

        public static object GetObject(Type type)
        {
            try
            {
                return type.GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }

        public static List<PropertyInfo> Property(Type type)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();

            foreach (PropertyInfo property in type.GetProperties())
                list.Add(property);

            return list;
        }

        public static List<PropertyInfo> PropertyList(object data)
        {
            return PropertyList(data.GetType());
        }

        public static ColumnAttribute GetColumnAttribute(PropertyInfo property)
        {
            foreach (object attribute in property.GetCustomAttributes(true))
                if (attribute is ColumnAttribute)
                    return (attribute as ColumnAttribute);
            return null;
        }

        public static string GetColumnName(PropertyInfo property)
        {
            ColumnAttribute ca = GetColumnAttribute(property);
            if (ca == null)
            {
                return string.Empty;
            }
            else if (ca.Name.Length > 2)
            {
                if (ca.Name.Substring(0, 3).Equals("sum"))
                {
                    return property.Name;
                }
                else if (ca.Name.Contains("(select"))
                {
                    return property.Name;
                }
                else return ca.Name;
            }
            else return ca.Name;
        }
        #endregion
    }

}