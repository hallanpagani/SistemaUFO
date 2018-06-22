using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseModelo.classes;
using BaseModelo.model.generico;
using System.Collections.Generic;

namespace BaseModelo.model.cadastros
{
    [Table("tb_tour")]
    public class tour : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long Id { get; set; }

        [Column("nm_tour")]
        [Display(Name = "Tour")]
        public string nm_tour { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public List<TourImagem> itens;

        public tour()
        {
            dh_inc = DateTime.Now;
        }
    }
}
