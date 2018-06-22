using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseModelo.classes;
using System.Collections.Generic;
using System.Reflection;

namespace BaseModelo.model.adm
{
    [Table("tb_sistema_perfil")]
    public class Perfil
    {
        [Key]
        [AutoInc]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_conta")]
        public long id_conta { get; set; }

        [Required]
        [Column("dh_inc")]
        public DateTime DhInc { get; set; }

        [Display(Name = "Perfil (Ex: Administrador. Convidado. Parcial. Gestor ...")]
        [Column("tp_perfil")]
        public string tp_perfil { get; set; }

        [Column("nm_menu")]
        public string nm_menu { get; set; }

        [Column("cd_perfil")]
        public long cd_perfil { get; set; }

        [Display(Name = "Departamentos")]
        public bool departamento { get; set; }

        [Display(Name = "Salas")]
        public bool sala { get; set; }

        [Display(Name = "Eventos")]
        public bool evento { get; set; }

        [Display(Name = "Recados")]
        public bool recado { get; set; }

        [Display(Name = "Agenda")]
        public bool agenda { get; set; }

        [Display(Name = "Mural")]
        public bool mural { get; set; }

        [Display(Name = "Conta")]
        public bool conta { get; set; }

        [Display(Name = "Perfil")]
        public bool perfil { get; set; }

        [Display(Name = "Usuário")]
        public bool usuario { get; set; }

        public Dictionary<string, bool> getPermissoes()
        {
            Dictionary<string, bool> acessoList = new Dictionary<string, bool>();
            acessoList.Add("departamento", departamento);
            acessoList.Add("sala", sala);
            acessoList.Add("eventos", evento);            
            acessoList.Add("agenda", agenda);
            acessoList.Add("mural", mural);
            acessoList.Add("conta", conta);
            acessoList.Add("perfil", perfil);
            acessoList.Add("usuario", usuario);

            return acessoList;
        }

        /**
         *  Seta as permissões para edição
         */
        public Perfil setPermissoes(string opcoes, bool value)
        {
            Perfil p = new Perfil();

            string[] menu = opcoes.Split(';');
            foreach (string m in menu)
            {
                PropertyInfo propertyInfo = p.GetType().GetProperty(m);
                if (propertyInfo != null)
                    propertyInfo.SetValue(p, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            }

            return p;
        }

        public Perfil()
        {
            DhInc = DateTime.Now;
        }
    }
}
