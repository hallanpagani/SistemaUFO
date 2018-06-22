using BaseModelo.classes;
using BaseModelo.model.generico;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseModelo.model.agendamentos
{
    [Table("agendamento_eventos")]
    public class eventos : BaseUGrav
    {
        //[AutoInc]
        //[Required]
        [Key]
        [AutoInc]
        [Column("id")]
        public long id { get; set; }

        [Column("id_sala")]
        public long id_sala { get; set; }

        [Column("titulo")]
        [Display(Name = "titulo")]
        public string titulo { get; set; }

        [Column("cor_padrao")]
        [Display(Name = "Cor Padrão")]
        public string cor_padrao { get; set; }

        [Column("dh_inicio")]
        [Display(Name = "Dt.Inicio")]
        public DateTime dh_inicio { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public string hr_inicio { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public string hr_fim { get; set; }

        [Column("dh_fim")]
        [Display(Name = "Dt.Fim")]
        public DateTime dh_fim { get; set; }

        [Column("descricao")]
        [Display(Name = "descricao")]
        public string descricao { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        [Display(Name = "Sala")]
        public string sala { get; set; }

        public List<sala> salas { get; set; }
        public List<convidados> convidados { get; set; }

        public eventos()
        {
            hr_inicio = null;
            hr_fim = null;
        }

    }
}
