using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_eventos_convidados")]
    public class convidados : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("id_evento")]
        [Display(Name = "id_evento")]
        public long id_evento { get; set; }

        [Column("ds_login")]
        [Display(Name = "ds_login")]
        public string ds_login { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public convidados()
        {
        }

    }
}
