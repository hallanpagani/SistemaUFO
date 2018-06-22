using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BaseModelo.classes;
using BaseModelo.model.generico;

namespace BaseModelo.model.adm
{
    [Table("loja")]
    public class Loja : BaseID
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long idLoja { get; set; }

        [Required]
      //  [Tooltip("Informe aqui o nm_insumo do estabelecimento/anunciante")]
        [Column("ds_loja")]
        [Display(Name = "Nome do estabelecimento/anunciante")]
        public string NmEstabelecimento { get; set; }

        [Required]
     //   [Tooltip("Informe aqui o cpf/cnpj do estabelecimento/anunciante")]
        [Column("ds_cpfcnpj")]
        [Display(Name = "CPF ou CNPJ")]
        public string Cpfcnpj { get; set; }


      //  [Tooltip("Grupo para agrupar promoções para lojas cadastradas.")]
        [Column("ds_grupo_loja")]
        [Display(Name = "Grupo lojas")]
        public string DsGrupoLoja { get; set; }

        [Required]
      //  [Tooltip("Informe aqui número sequencial da loja, Exemplo: 1 ")]
        [Column("nr_loja")]
        [Display(Name = "Número loja")]
        public int NrLoja { get; set; }

        [Required]
      //  [Tooltip("Informe aqui a cidade da loja")]
        [Column("id_cidade")]
        [Display(Name = "Cidade")]
        public int IdCidade { get; set; }

        [Column("cep")]
        [Display(Name = "CEP")]
        public string cep { get; set; }

        [Column("logradouro")]
        [Display(Name = "Endereço")]
        public string logradouro { get; set; }

        [Column("numero")]
        [Display(Name = "Número")]
        public string numero { get; set; }

        [Column("complemento")]
        [Display(Name = "Complemento")]
        public string complemento { get; set; }

        [Column("bairro")]
        [Display(Name = "Bairro")]
        public string bairro { get; set; }

        [Column("fone_principal")]
        [Display(Name = "Telefone principal")]
        public string fone1 { get; set; }

        [Column("url_imagem")]
     //   [Tooltip("Informe aqui a url completa da imagem (Imagem da internet) ou escolha uma foto do seu computador logo abaixo.")]
        [Display(Name = "Url da logomarca")]
        public string imagem { get; set; }

        public HttpPostedFileBase imageupload { get; set; }

        public string NmCidade { get; set; }
        public string NmEstado { get; set; }
        
    }
}
