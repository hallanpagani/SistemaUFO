using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_mural")]
    public class mural : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }
        
        [Column("ds_titulo")]
        [Display(Name = "Título da mensagem")]
        public string ds_titulo { get; set; }

        [Column("ds_mensagem")]
        [Display(Name = "Descrição da mensagem")]
        public string ds_mensagem { get; set; }

        [Column("ds_imagem")]
        [Display(Name = "Imagem para a mensagem")]
        public string ds_imagem { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public List<mural> itens;

        public mural()
        {
            dh_inc = DateTime.Now;
        }

    }
}
