using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_email_convidados")]
    public class email_convidados : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("id_email")]
        [Display(Name = "id_email")]
        public long id_email { get; set; }

        [Column("ds_login")]
        [Display(Name = "ds_login")]
        public string ds_login { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public email_convidados()
        {
        }

    }
}
