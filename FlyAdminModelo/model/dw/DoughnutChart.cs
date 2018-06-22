using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.dw
{
    [Table("vw_despesas_receitas_mes")]
    public class DoughnutChart 
    {
        [Column("sum(value) as value")]
        public double value { get; set; }

        [Column("color")]
        public string color { get; set; }

        [Column("highlight")]
        public string highlight { get; set; }

        [Column("label")]
        public string label { get; set; }
    }
}