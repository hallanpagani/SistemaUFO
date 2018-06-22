using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_email")]
    public class email : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("ds_nome")]
        [Display(Name = "ds_nome")]
        public string ds_nome { get; set; }

        [Column("ds_destinatarios")]
        [Display(Name = "ds_destinatarios")]
        public string ds_destinatarios { get; set; }

        [Column("ds_assunto")]
        [Display(Name = "ds_assunto")]
        public string ds_assunto { get; set; }

        [Column("ds_mensagem")]
        [Display(Name = "ds_mensagem")]
        public string ds_mensagem { get; set; }
        
        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        //public List<email_convidados> convidados { get; set; }

        public email()
        {
            
        }

    }
}
