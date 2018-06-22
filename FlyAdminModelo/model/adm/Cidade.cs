using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.adm
{
    [Table("cidade")]
    public class Cidade
    {
        [Key]
        [Column("id")]
        public int idCidade { get; set; }

        [Column("nm_cidade")]
        public string NmCidade { get; set; }

    }
}
