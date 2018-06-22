using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_cadastro_departamento")]
    public class departamento : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("departamento")]
        [Display(Name = "departamento")]
        public string nm_departamento { get; set; }

        [Column("descricao")]
        [Display(Name = "descricao")]
        public string ds_descricao { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public departamento()
        {
        }

    }
}
