using BaseModelo.model.dw;
using BasePersistencia.banco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BasePersistencia.model
{
    public static class DashboardDAL
    {
        public static BarChart GetReceitasDespesasPorMes(long idConta)
        {
            if (idConta == 0)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append("id_conta,");
            sb.Append("fillColor,");
            sb.Append("strokeColor,");
            sb.Append("highlightFill,");
            sb.Append("highlightStroke,");
            sb.Append("label,");
            sb.Append("group_concat(`labels` SEPARATOR ', ') as labels,");
            sb.Append("group_concat(`data` SEPARATOR ', ') as data ");
            sb.Append("from ");
            sb.Append("vw_chart_despesas_receitas_por_mes ");
            sb.AppendFormat("where id_conta={0} ", idConta);
            sb.Append("group by 1, 2, 3, 4, 5, 6 ");

            DataTable tb = DAL.ListarFromSQL(sb.ToString());
            

            var bchart = new BarChart();
            try
            {

                foreach (DataRow dr in tb.Rows)
                {
                    foreach (string l in dr["labels"].ToString().Split(',') )
                    {
                        if (!bchart.labels.Contains(l)) { 
                            bchart.labels.Add(l.Trim());
                        }
                    };
                    var list = new List<double>();
                    foreach (string l in dr["data"].ToString().Split(',').ToList())
                    {
                        list.Add(Convert.ToDouble(l.Replace('.',',')));
                    }

                    bchart.datasets.Add(new Dataset()
                    {
                        label = dr["label"].ToString(),
                        fillColor = dr["fillColor"].ToString(),
                        strokeColor = dr["strokeColor"].ToString(),
                        //highlightFill = dr["highlightFill"].ToString(),
                        highlightStroke = dr["highlightStroke"].ToString(),
                        data = list
                    });

                }
            }
            catch (Exception e )
            {
                throw e;
            }

            return bchart;
        }

        public static decimal GetTotalizadorMes(long idConta, long label)
        {
            if (idConta == 0)
            {
                return 0;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append("valor ");
            sb.Append("from ");
            sb.Append("vw_despesas_receitas_por_mes ");
            sb.AppendFormat("where id_conta={0} and ", idConta);
            sb.AppendFormat("mes={0} and label={1} ", DateTime.Now.Month, label);
            try
            {
                return DAL.GetDecimal(sb.ToString(), 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static decimal GetTotalizadorDia(long idConta, long label)
        {
            if (idConta == 0)
            {
                return 0;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append("valor ");
            sb.Append("from ");
            sb.Append("vw_despesas_receitas_por_dia ");
            sb.AppendFormat("where id_conta={0} and ", idConta);
            sb.AppendFormat("mes={0} and dia={1} and label={2} ", DateTime.Now.Month, DateTime.Now.Day, label);
            try
            {
                return DAL.GetDecimal(sb.ToString(), 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
