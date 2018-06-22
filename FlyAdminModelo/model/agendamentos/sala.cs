using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_cadastro_sala")]
    public class sala : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("sala")]
        [Display(Name = "sala")]
        public string ds_sala { get; set; }

        [Column("local")]
        [Display(Name = "local")]
        public string local { get; set; }

        [Column("capacidade")]
        [Display(Name = "capacidade")]
        public int capacidade { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public sala()
        {
        }

    }
}
