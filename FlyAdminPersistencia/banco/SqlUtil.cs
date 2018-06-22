using System;
using System.Text;
using MySql.Data.MySqlClient;

namespace BasePersistencia.banco
{
    public static class SqlUtil
    {
        /// <summary>
        /// Esta função recebe um campo (tipo string) e um valor. 
        /// Se foi passado um valor do tipo Flavio*  o asterisco é convertido para % se o valor não tiver *, entao será colocado % na frente e atrás (like full power)
        /// </summary>
        /// <param name="campo">Nome do campo. "nm_cliente"</param>
        /// <param name="valor">Conteúdo para like. "F*"</param>
        public static string MontarLike(string campo, string valor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" lower(");
            sb.Append(campo);
            sb.Append(") like '");
            if (valor.IndexOf('*') == -1)
            {
                sb.Append("%");
                sb.Append(valor.ToLower());
                sb.Append("%");
            }
            else
                sb.Append(valor.ToLower().Replace('*', '%'));
            sb.Append("' ");
            return sb.ToString();
        }

        // helper para classe NpgsqlParameter que retorna o valor formatado
        public static String ParameterValueForSQL(this MySqlParameter param)
        {
            string retval = "";
            switch (param.MySqlDbType)
            {
                case MySqlDbType.String:
                case MySqlDbType.Text:
                case MySqlDbType.LongText:
                case MySqlDbType.Date:
                case MySqlDbType.Time:
                case MySqlDbType.MediumText:
                case MySqlDbType.Timestamp:
                    if (param.Value == null)
                        retval = "null";
                    else
                        retval = "'" + param.Value.ToString().Replace("'", "''") + "'";
                    break;
                default:
                    if (param.Value == null)
                        retval = "null";
                    else
                        retval = param.Value.ToString(); //.Replace("'", "''");
                    break;
            }
            return retval;
        }


        // Helper para NpgsqlCommand que retorna o CommandText trocando os parâmetros pelos seus valores
        // através do helper ParameterValueForSQL (veja acima)
        public static String CommandAsSql(this MySqlCommand cmd)
        {
            StringBuilder sql = new StringBuilder(cmd.CommandText);

            foreach (MySqlParameter param in cmd.Parameters)
            {
                sql.Replace("@" + param.ParameterName, param.ParameterValueForSQL());
            }

            return sql.ToString();
        }
    }
}