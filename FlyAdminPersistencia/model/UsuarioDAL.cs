using BaseModelo.model.adm;
using BasePersistencia.banco;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace BasePersistencia.model
{
    public static class UsuarioDAL
    {
        public static void Gravar(Usuario usuario)
        {

        }

        public static Usuario FindByEmail(string email)
        {
            List<Usuario> lista = DAL.ListarObjetos<Usuario>(string.Format("ds_login='{0}' ", email));
            if (lista.Count == 0)
            {// No existing user was found that matched the given criteria
                return new Usuario();
            }
            var user = new Usuario()
            {
                Id = lista[0].Id,
                Email = lista[0].Email,
                NomeDoUsuario = lista[0].NomeDoUsuario
            };
            return user;
        }

        public static IEnumerable<Usuario> GetUsuarios()
        {
            var sb = new StringBuilder();
            sb.Append(" select * from tb_usuarios");

            sb.Append(" order by ds_login ");
            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new Usuario()
            {
                Id = t.Field<int>("id"),
                Email = (t.Field<string>("ds_login")),
                Password = (t.Field<string>("ds_senha")),
                is_ativo = (t.Field<int>("is_ativo")),
                NomeDoUsuario = (t.Field<string>("nm_usuario"))
            }).ToList();
        }

        public static IEnumerable<Usuario> GetUsuarios(long conta, string email)
        {
            var sb = new StringBuilder();
            sb.Append(" select * from tb_usuarios");
            sb.Append(" where ");
            sb.AppendFormat(" id_conta ={0}", conta);

            if (!email.Equals(""))
            {
                sb.AppendFormat(" and (ds_login like '%{0}%' ) ", email);
                //sb.AppendFormat("   or (id  like '%{0}%' )) ", email);
            }

            sb.Append(" order by ds_login ");
            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new Usuario()
            {
                Id = t.Field<int>("id"),
                Email = (t.Field<string>("ds_login")),
                Password = (t.Field<string>("ds_senha")),
                is_ativo = (t.Field<int>("is_ativo")),
                NomeDoUsuario = (t.Field<string>("nm_usuario"))
            }).ToList();
        }

    }
}
