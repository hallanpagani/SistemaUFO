using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseModelo.classes;
using BaseModelo.model.generico;

namespace BaseModelo.model.cadastros
{
    [Table("tb_tour_imagem")]
    public class TourImagem : BaseUGrav
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_tour")]
        public long id_tour { get; set; }

        [Column("ds_caminho")]
        public string ds_caminho { get; set; }

        [OnlyInsert]
        [Column("dh_inc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Dt.Inc")]
        public DateTime dh_inc { get; set; }

        public TourImagem()
        {
            dh_inc = DateTime.Now;
        }
    }
}
