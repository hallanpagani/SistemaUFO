using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BaseModelo.model.adm;
using BaseModelo.model.agendamentos;
using System;
using BasePersistencia.banco;

namespace Persistencia.model
{
	public static class AgendamentoDAO
	{
		public static IEnumerable<departamento> GetDepartamentos()
		{
			var sb = new StringBuilder();
			sb.Append(" select id, departamento, descricao from agendamento_cadastro_departamento");
			sb.Append(" order by departamento ");

			return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new departamento()
			{
				id = t.Field<long>("id"),
                nm_departamento = (t.Field<string>("departamento")),
                ds_descricao = (t.Field<string>("descricao"))
			}).ToList();
		}

        public static int GetDepartamentosCadastrados()
        {
            var sb = new StringBuilder();
            sb.Append(" select id, departamento, descricao from agendamento_cadastro_departamento");
            sb.Append(" order by departamento ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new departamento()
            {
                id = t.Field<long>("id"),
                nm_departamento = (t.Field<string>("departamento")),
                ds_descricao = (t.Field<string>("descricao"))
            }).ToList().Count;
        }

        public static IEnumerable<sala> GetSalas()
        {
            var sb = new StringBuilder();
            sb.Append(" select id, sala, local, capacidade from agendamento_cadastro_sala");
            sb.Append(" order by sala ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new sala()
            {
                id = t.Field<long>("id"),
                ds_sala = (t.Field<string>("sala")),
                local = (t.Field<string>("local")),
                capacidade = t.Field<int>("capacidade")
            }).ToList();
        }

        public static int GetSalasCadastradas()
        {
            var sb = new StringBuilder();
            sb.Append(" select id, sala, local, capacidade from agendamento_cadastro_sala");
            sb.Append(" order by sala ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new sala()
            {
                id = t.Field<long>("id"),
                ds_sala = (t.Field<string>("sala")),
                local = (t.Field<string>("local")),
                capacidade = t.Field<int>("capacidade")
            }).ToList().Count;
        }

        public static IEnumerable<eventos> GetEventos()
        {
            var sb = new StringBuilder();
            sb.Append(" select titulo,");
            sb.Append(" id,");
            sb.Append(" dh_inc,");
            sb.Append(" dh_inicio,");
            sb.Append(" dh_fim,");
            sb.Append(" descricao,");
            sb.Append(" id_sala");
            sb.Append(" from agendamento_eventos");
            sb.Append(" order by titulo ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new eventos()
            {
                id = t.Field<long>("id"),
                titulo = (t.Field<string>("titulo")),
                dh_inc = (t.Field<DateTime>("dh_inc")),
                dh_inicio = (t.Field<DateTime>("dh_inicio")),
                dh_fim = (t.Field<DateTime>("dh_fim")),
                descricao = (t.Field<string>("descricao")),
                salas = DAL.ListarObjetos<sala>(string.Format("id = {0}", t.Field<long>("id_sala"))),
                convidados = DAL.ListarObjetos<convidados>(string.Format("id_evento = {0}", t.Field<long>("id")))
            }).ToList();
        }

        public static int GetEventosCadastradosHoje()
        {
            var sb = new StringBuilder();
            sb.Append(" select titulo,");
            sb.Append(" id,");
            sb.Append(" dh_inc,");
            sb.Append(" dh_inicio,");
            sb.Append(" dh_fim,");
            sb.Append(" descricao,");
            sb.Append(" id_sala");
            sb.Append(" from agendamento_eventos");
            sb.Append(" where ");
            sb.Append(" DAY(dh_inicio) = DAY(NOW()) AND MONTH(dh_inicio) = MONTH(NOW()) ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new eventos()
            {
                id = t.Field<long>("id"),
                titulo = (t.Field<string>("titulo")),
                dh_inc = (t.Field<DateTime>("dh_inc")),
                dh_inicio = (t.Field<DateTime>("dh_inicio")),
                dh_fim = (t.Field<DateTime>("dh_fim")),
                descricao = (t.Field<string>("descricao")),
                salas = DAL.ListarObjetos<sala>(string.Format("id = {0}", t.Field<long>("id_sala"))),
                convidados = DAL.ListarObjetos<convidados>(string.Format("id_evento = {0}", t.Field<long>("id")))
            }).ToList().Count;
        }

        public static int GetEventosCadastradosMes()
        {
            var sb = new StringBuilder();
            sb.Append(" select titulo,");
            sb.Append(" id,");
            sb.Append(" dh_inc,");
            sb.Append(" dh_inicio,");
            sb.Append(" dh_fim,");
            sb.Append(" descricao,");
            sb.Append(" id_sala");
            sb.Append(" from agendamento_eventos");
            sb.Append(" where ");
            sb.Append(" MONTH(dh_inicio) = MONTH(NOW()) ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new eventos()
            {
                id = t.Field<long>("id"),
                titulo = (t.Field<string>("titulo")),
                dh_inc = (t.Field<DateTime>("dh_inc")),
                dh_inicio = (t.Field<DateTime>("dh_inicio")),
                dh_fim = (t.Field<DateTime>("dh_fim")),
                descricao = (t.Field<string>("descricao")),
                salas = DAL.ListarObjetos<sala>(string.Format("id = {0}", t.Field<long>("id_sala"))),
                convidados = DAL.ListarObjetos<convidados>(string.Format("id_evento = {0}", t.Field<long>("id")))
            }).ToList().Count;
        }

        public static IEnumerable<Usuario> GetConvidados()
        {
            var sb = new StringBuilder();
            sb.Append(" select id,");
            sb.Append(" ds_login,");
            sb.Append(" nm_usuario,");
            sb.Append(" departamento");
            sb.Append(" from usuario");
            sb.Append(" where departamento is not null and ");
            sb.Append(" departamento <> 'Sem Departamento' ");
            sb.Append(" order by nm_usuario ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new Usuario()
            {
                Id = t.Field<int>("id"),
                Email = (t.Field<string>("ds_login")),
                NomeDoUsuario = (t.Field<string>("nm_usuario")),
                departamento = (t.Field<string>("departamento"))
            }).ToList();
        }

        public static IEnumerable<mural> GetMural()
        {
            var sb = new StringBuilder();
            sb.Append(" select id,");
            sb.Append(" ds_titulo,");
            sb.Append(" ds_mensagem,");
            sb.Append(" ds_imagem");
            sb.Append(" from agendamento_mural");
            sb.Append(" order by dh_inc ");

            return DAL.ListarFromSQL(sb.ToString()).AsEnumerable().Select(t => new mural()
            {
                id = t.Field<int>("id"),
                ds_titulo = (t.Field<string>("ds_titulo")),
                ds_mensagem = (t.Field<string>("ds_mensagem")),
                ds_imagem = (t.Field<string>("ds_imagem"))
            }).ToList();
        }
    }
}
