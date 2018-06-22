using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using BaseModelo.classes;

namespace BaseModelo.model.adm
{
    [Table("tb_sistema_conta")]
    public class Conta
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long Id { get; set; }
        
        [Required]
        [Column("dh_inc")]
        public DateTime DhInc { get; set; }

        [Required(ErrorMessage = "Campo nome da conta é obrigatório!", AllowEmptyStrings = false)]
        [Display(Name = "Nome da conta")]        
        [MaxLength(100, ErrorMessage = "Campo 'nome da conta' deve ter no máximo 100 caracteres!")]
        [Column("nm_conta")]
        public string NmConta { get; set; }
        
        [Required]
        [Column("is_ativo")]
        public string is_ativo { get; set; }

        public List<Usuario> usuarios { get; set; }
        public List<Perfil> perfil { get; set; }

        public Conta()
        {
            DhInc = DateTime.Now;
        }
    }
}
