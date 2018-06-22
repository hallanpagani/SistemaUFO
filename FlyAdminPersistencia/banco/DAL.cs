using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using BaseModelo.classes;
using BasePersistencia.classes;
using MySql.Data.MySqlClient;

namespace BasePersistencia.banco
{
    // Data Access Layer - acesso a camada de dados
    public class DAL
    {
        public static string GetStringConexao()
        {
            string parametro;
            parametro = "conexao_base_local";
            return ConfigurationManager.ConnectionStrings[parametro].ToString();
        }

        #region insert e update
        // grava um único objeto

        /// <summary>
        /// Persiste (grava) um objeto em banco mySQL baseando-se nos atributos do objeto para saber os campos e seus tipos
        /// Pessoa p = new Pessoa () { Nome = "Junior" };
        /// DAL.Gravar(p);
        /// </summary>
        /// <param name="data">instância do objeto a ser gravado.</param>
        /// <returns>Retornar o id do objeto (em caso de inserção)</returns>
        public static long Gravar(object data)
        {
            long idRetorno = 0;

            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo, se é PK e seu valor (Nullable<object>)
            List<Campo> campos = Montador.GetCampos(data);
            string tableName = Montador.GetTableName(data);

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            {
                conexao.Open();
                using (Comando comando = new Comando(conexao, GetSqlInsertUpdate(tableName, campos)))
                {
                    foreach (Campo campo in campos)
                    {
                        comando.AddParam(string.Format("@{0}", campo.Nome), (campo.Valor ?? DBNull.Value));
                    }
                    try
                    {
                        comando.Execute();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    idRetorno = comando.LastInsertId;
                }
            }
            return idRetorno;
        }

        public static int GetMaxColumn(string tabela, string coluna, string id_conta)
        {
            // monta o select e filtra pelo campo chave
            string sql = "select max(" + coluna + ") as cd_perfil from " + tabela + " where id_conta = " + id_conta;
            int value = 0;

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            using (Leitor leitor = comando.Select())
            {
                // não tem nenhum? volta nulo
                if (leitor.RecordCount == 0)
                    return value;

                // valor busta pelo nome do campo
                object valor = leitor.GetObject(coluna);
                value = Convert.ToInt32(valor);
            }

            return value;
        }

        /// <summary>
        /// Grava (persiste) no banco mySQL vários objetos da List (todos dentro da mesma transação)
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser gravado</typeparam>
        /// <param name="list">Lista de T</param>
        /// <returns>Quantidade de objetos gravados</returns>
        public static int GravarList<T>(List<T> list)
        {
            if (list.Count == 0)
                return 0;

            int idRetorno = 0;

            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo, se é PK e seu valor (Nullable<object>)
            List<Campo> campos = Montador.GetCampos(list[0]);
            string tableName = Montador.GetTableName(list[0]);

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
            using (Comando comando = new Comando(transacao, GetSqlInsertUpdate(tableName, campos)))
            {
                try
                {
                    // percorre os objetos da lista
                    foreach (Object obj in list)
                    {
                        // atualiza os valores
                        campos = Montador.GetCampos(obj);
                        foreach (Campo campo in campos)
                        {
                            comando.AddParam(string.Format("@{0}", campo.Nome), (campo.Valor ?? DBNull.Value));
                        }
                        comando.Execute();
                        // limpa os parâmetros e vamos para o próximo
                        comando.ClearParam();
                        idRetorno++;
                    }
                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.RollBack();
                    throw ex;
                }
            }
            return idRetorno;
        }

        /// <summary>
        /// Grava um objeto mestre e na mesma transação os filhos no list.
        /// <c>
        ///    List<Empresa> lista = new List<Empresa>();
        ///    lista.Add(new Empresa() { Nome = "empresa1" });
        ///    lista.Add(new Empresa() { Nome = "empresa2" });
        ///    lista.Add(new Empresa() { Nome = "empresa3" });
        ///    lista.Add(new Empresa() { Nome = "empresa4" });
        ///    lista.Add(new Empresa() { Nome = "empresa5" });
        ///
        ///    int registros_gravados = DAL.GravarList<Empresa>(lista);
        ///
        ///    Console.WriteLine("Total de registros gravados: " + registros_gravados.ToString());
        /// </c>
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser gravado</typeparam>
        /// <param name="objMestre">Objeto pai</param>
        /// <param name="listDetalhes">Lista de T (filhos)</param>
        /// <returns>O id do registro pai</returns>
        public static long GravarMestreDetalhe<T>(object objMestre, List<T> listDetalhes)
        {
            long idRetorno = Montador.GetKeyId(objMestre);

            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo, se é PK e seu valor (Nullable<object>)
            List<Campo> camposMestre = Montador.GetCampos(objMestre);
            string tableNameMestre = Montador.GetTableName(objMestre);

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Transacao transacao = new Transacao(conexao))
            {
                try
                {
                    #region mestre
                    using (Comando comando = new Comando(transacao, GetSqlInsertUpdate(tableNameMestre, camposMestre)))
                    {
                        foreach (Campo campo in camposMestre)
                        {
                            comando.AddParam(string.Format("@{0}", campo.Nome), (campo.Valor ?? DBNull.Value));
                        }

                        comando.Execute();
                        // se é inserção então lemos o ultimo id
                        if (idRetorno == 0)
                            idRetorno = comando.LastInsertId;
                    }
                    #endregion

                    #region detalhes
                    if (listDetalhes.Count > 0)
                    {
                        // pegamos o nm_insumo do campo FK no primeiro objeto da lista
                        string campoDetalheFK = Montador.GetFieldFK(listDetalhes[0]);
                        // montamos a estrutura base do SQL 
                        string tableNameDetalhes = Montador.GetTableName(listDetalhes[0]);
                        List<Campo> camposDetalhes = Montador.GetCampos(listDetalhes[0]);

                        using (Comando comando = new Comando(transacao, GetSqlInsertUpdate(tableNameDetalhes, camposDetalhes)))
                        {
                            // percorre os objetos da lista
                            foreach (Object obj in listDetalhes)
                            {
                                // atualiza os valores
                                camposDetalhes = Montador.GetCampos(obj);
                                foreach (Campo campo in camposDetalhes)
                                {
                                    // o campo atual é FK?  e o valor é ZERO (null converte pra zero) ?
                                    if (campo.Nome.Equals(campoDetalheFK) && (int)(campo.Valor ?? 0) == 0)
                                        campo.Valor = idRetorno;

                                    comando.AddParam(string.Format("@{0}", campo.Nome), (campo.Valor ?? DBNull.Value));
                                }
                                comando.Execute();
                                // limpa os parâmetros e vamos para o próximo
                                comando.ClearParam();
                            }
                        }
                    }
                    #endregion
                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.RollBack();
                    throw ex;
                }
            }
            return idRetorno;
        }
        #endregion

        #region delete
        /// <summary>
        /// Exclui um objeto em banco mySQL baseando-se nos atributos do objeto para saber os campos e seus tipos
        /// Pessoa p = new Pessoa () { Id = 1 };
        /// DAL.Excluir(p);
        /// </summary>
        /// <param name="data">instância do objeto a ser eliminado.</param>
        public static void Excluir(object data)
        {
            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo, se é PK e seu valor (Nullable<object>)
            List<Campo> campos = Montador.GetCampos(data);
            string tableName = Montador.GetTableName(data);

            string sqlId = string.Empty;

            foreach (Campo campo in campos)
                if (campo.IsKey)
                {
                    sqlId = string.Format("{0}=@{0}", campo.Nome);
                    break;
                }

            if (sqlId == string.Empty)
            {
                // algo errado
                Exception e = new Exception("Id não localizado para exclusão");
                throw e;
            }

            // vamos pegar o valor id
            long id = DAL.GetIdValue(campos);

            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(tableName);
            sql.Append(" where ");
            sql.Append(sqlId);

            using (Conexao conexao = Conexao.Get(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql.ToString()))
            {
                foreach (Campo campo in campos)
                    if (campo.IsKey)
                    {
                        comando.AddParam(string.Format("@{0}", campo.Nome), campo.Valor);
                        break;
                    }

                comando.Execute();
            }
        }
        #endregion

        public static void ExcluirTrambique(string delete)
        {
            using (Conexao conexao = Conexao.Get(GetStringConexao()))
            using (Comando comando = new Comando(conexao, delete))
            {
                comando.Execute();
            }
        }

        #region funcoes do tipo GetObjetos
        /// <summary>
        /// Efetua um select na base e retorna um objeto T com base no seu primary key
        /// <c>Empresa empresa = DAL.GetObjetoById<Empresa>(3);</c>
        /// </summary>
        /// <typeparam name="T">Tipo do retorno</typeparam>
        /// <param name="id">Código id para buscar o objeto na base</param>
        /// <returns>Retorna uma instância do Tipo</returns>
        public static T GetObjetoById<T>(int id) where T : class, new()
        {
            // cria uma instância do objeto
            T t = new T();

            // pega os campos para poder montar o select
            List<Campo> campos = Montador.GetCampos(t);

            // monta o select e filtra pelo campo chave
            string sql = GetSqlSelect(t, string.Format("{0}={1}", GetIdFieldName(campos), id));

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            using (Leitor leitor = comando.Select())
            {
                // tem retorno
                if (leitor.RecordCount > 0)
                    // percorre as propriedades
                    foreach (PropertyInfo property in Auxiliar.PropertySimple(t))
                    {
                        // valor busta pelo nm_insumo do campo
                        object valor = leitor.GetObject(Auxiliar.GetColumnName(property));

                        if ((valor != null) && (!(valor is System.DBNull)))
                        {
                            try
                            {
                                property.SetValue(t, valor, null);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
            }

            return t;
        }

        /// <summary>
        /// Efetua um select na base e retorna um objeto T com base em um filtro (retorna apenas 1 registro)
        /// </summary>
        /// <c>
        ///    Empregado empregado = new Empregado();
        ///
        ///    Filtros filtro = new Filtros().Add(() => empregado.Nome, empregado, FiltroExpressao.Igual, "Junior");
        ///    empregado = DAL.GetObjeto<Empregado>(filtro.ToString());
        ///
        ///    if (empregado == null)
        ///        Console.WriteLine("Nao encontrado!");
        ///    else
        ///        Console.WriteLine(empregado.Id + "-" + empregado.Nome);
        /// </c>
        /// <typeparam name="T">Tipo do retorno</typeparam>
        /// <param name="filtro">Use a classe Filtros para montar o filtro</param>
        /// <returns>Retorna uma instância do Tipo</returns>
        public static T GetObjeto<T>(string filtro = "") where T : class, new()
        {
            // cria uma instância do objeto
            T t = new T();

            // pega os campos para poder montar o select
            List<Campo> campos = Montador.GetCampos(t);

            // monta o select e filtra pelo campo chave
            string sql = GetSqlSelect(new T(), filtro);

            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            using (Leitor leitor = comando.Select())
            {
                // não tem nenhum? volta nulo
                if (leitor.RecordCount == 0)
                    return null;

                // percorre as propriedades
                foreach (PropertyInfo property in Auxiliar.PropertySimple(t))
                {
                    // valor busta pelo nm_insumo do campo
                    object valor = leitor.GetObject(Auxiliar.GetColumnName(property));

                    if ((valor != null) && (!(valor is System.DBNull)))
                    {
                        property.SetValue(t, valor, null);
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// Efetua um select na base e retorna um objeto T com base em um filtro (retorna apenas 1 registro)
        /// </summary>
        /// <c>
        ///    Empregado empregado = new Empregado();
        ///
        ///    Filtros filtro = new Filtros().Add(() => empregado.Nome, empregado, FiltroExpressao.Igual, "Junior");
        ///    empregado = DAL.GetObjeto<Empregado>(filtro);
        ///
        ///    if (empregado == null)
        ///        Console.WriteLine("Nao encontrado!");
        ///    else
        ///        Console.WriteLine(empregado.Id + "-" + empregado.Nome);
        /// </c>
        /// <typeparam name="T">Tipo do retorno</typeparam>
        /// <param name="filtro">Use a classe Filtros para montar o filtro</param>
        /// <returns>Retorna uma instância do Tipo</returns>
        public static T GetObjeto<T>(Filtros filtro) where T : class, new()
        {
            return GetObjeto<T>(filtro.ToString());
        }
        #endregion

        #region rotinas para retornar listas ou leitores
        /// <summary>
        /// Efetua um select na base com where e order by opcionais e retorna num objeto tipo Leitor
        /// </summary>
        /// <code>
        ///    Leitor l = DAL.Listar(new Empregado());
        ///    while (!l.Eof)
        ///    {
        ///        Console.WriteLine(l.GetString("id_empregado") + "-" + l.GetString("nm_empregado"));
        ///        l.Next();
        ///    }
        /// </code>
        /// <param name="data">Instância do objeto</param>
        /// <param name="filtro">É string, mas ao invés de passar um string direto, use o objeto Filtros para facilitar</param>
        /// <param name="ordem">É string, mas ao invés de passar os campos diretamente, use o objeto Order</param>
        /// <returns>Retorna um objeto do tipo Leitor para usar em loopings</returns>
        public static Leitor Listar(object data, string filtro = "", string ordem = "", string group = "", int limite = 0)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, GetSqlSelect(data, filtro, ordem, group, limite)))
            {
                return comando.Select();
            }
        }

        /// <summary>
        /// Efetua um select na base com where e order by opcionais e retorna num objeto tipo Leitor
        /// </summary>
        /// <code>
        ///    Leitor l = DAL.Listar(new Empregado(), new Filtros(), new Ordem());
        ///    while (!l.Eof)
        ///    {
        ///        Console.WriteLine(l.GetString("id_empregado") + "-" + l.GetString("nm_empregado"));
        ///        l.Next();
        ///    }
        /// </code>
        /// <param name="data">Instância do objeto</param>
        /// <param name="filtro">É um objeto do tipo Filtros para não retornar tudo</param>
        /// <param name="ordem">É um objeto tipo Ordem para organizar a saída</param>
        /// <returns>Retorna um objeto do tipo Leitor para usar em loopings</returns>
        public static Leitor Listar(object data, Filtros filtro, Ordem ordem, string group, int limite = 0)
        {
            return Listar(data, filtro.ToString(), ordem.ToString(), group, limite);
        }

        /// <summary>
        /// Efetua um select na base retorna num objeto tipo Leitor
        /// </summary>
        /// <code>
        ///    Leitor l = DAL.GetLeitorFromSQL("select cd_funcionario, nm_insumo from funcionario order by 1 desc");
        ///    while (!l.Eof)
        ///    {
        ///       System.Console.WriteLine(l.GetString("cd_funcionario") + "-" + l.GetString("nm_insumo"));
        ///       l.Next();
        ///     }
        /// </code>
        /// <param name="sql">Código sql para executar na base</param>
        /// <returns>Retorna um objeto do tipo Leitor para usar em loopings</returns>
        public static Leitor GetLeitorFromSQL(string sql)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                return comando.Select(sql);
            }
        }

        /// <summary>
        /// Efetua um select na base retorna num objeto tipo DataTable
        /// </summary>
        /// <code>
        ///    DataTable dt = DAL.ListarFromSQL("select cd_funcionario, nm_insumo from funcionario order by 1 desc");
        ///    foreach (DataRow dr in dt.Rows)
        ///        System.Console.WriteLine(dr["cd_funcionario"] + "|" + dr["nm_insumo"]);
        /// </code>
        /// <param name="sql">Código sql para executar na base</param>
        /// <returns>Retorna um objeto do tipo DataTable para usar em loopings</returns>
        public static DataTable ListarFromSQL(string sql)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                return comando.Select(sql).Data;
            }
        }

        /// <summary>
        /// Efetua um select na base com where e order by opcionais e retorna num objeto tipo DataTable
        /// </summary>
        /// <c>
        ///    DataTable tb = DAL.ListarDataTable(new Usuario());
        ///    foreach (DataRow r in tb.Rows)
        ///        Console.WriteLine(r["nm_usuario"].ToString());
        /// </c>
        /// <param name="data">Instância do objeto</param>
        /// <param name="filtro">É string, mas ao invés de passar um string direto, use a classe Filtros para facilitar</param>
        /// <param name="ordem">É string, mas ao invés de passar os campos diretamente, use a classe Order</param>
        /// <returns>Retorna um objeto DataTable contendo os registros</returns>
        public static DataTable ListarDataTable(object data, string filtro = "", string ordem = "", string group = "", int limite = 0)
        {
            return Listar(data, filtro, ordem, group, limite).Data;
        }

        /// <summary>
        /// Efetua um select na base com where e order by opcionais e retorna num objeto tipo DataTable
        /// </summary>
        /// <c>
        ///    DataTable tb = DAL.ListarDataTable(new Usuario(), new Filtros(), new Ordem());
        ///    foreach (DataRow r in tb.Rows)
        ///        Console.WriteLine(r["nm_usuario"].ToString());
        /// </c>
        /// <param name="data">Instância do objeto</param>
        /// <param name="filtro">É string, mas ao invés de passar um string direto, use a classe Filtros para facilitar</param>
        /// <param name="group">É string, mas ao invés de passar um string direto, use a classe Filtros para facilitar</param>
        /// <param name="ordem">É string, mas ao invés de passar os campos diretamente, use a classe Order</param>
        /// <returns>Retorna um objeto DataTable contendo os registros</returns>
        public static DataTable ListarDataTable(object data, Filtros filtro, Ordem ordem, string group = "", int limite = 0)
        {
            return Listar(data, filtro.ToString(), ordem.ToString(), group, limite).Data;
        }

        /// <summary>
        /// Efetua um select na base e retorna uma list de objeto T com base em um filtro e order by 
        /// <c> List<Usuario> lista = DAL.ListarObjetos<Usuario>(typeof(Usuario));</c>
        /// </summary>
        /// <typeparam name="E">Tipo do List de retorno</typeparam>
        /// <param name="filtro">Use a classe Filtros para montar o filtro</param>
        /// <param name="ordem">É string, mas ao invés de passar os campos diretamente, use a classe Order</param>
        /// <returns>Retorna uma List de instâncias do Tipo</returns>
        public static List<E> ListarObjetos<E>(string filtro = "", string ordem = "", string group = "") where E : class, new()
        {
            List<E> list = new List<E>();
            string sql = GetSqlSelect(new E(), filtro, ordem, group);
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                using (Leitor leitor = comando.Select())
                {
                    try
                    {
                        while (!leitor.Eof)
                        {
                            // cria uma instância do objeto
                            E item = new E();

                            // percorre as propriedades
                            foreach (PropertyInfo property in Auxiliar.PropertySimple(item))
                            {
                                // valor busta pelo nm_insumo do campo
                                object valor = leitor.GetObject(Auxiliar.GetColumnName(property));

                                if ((valor != null) && (!(valor is DBNull)))
                                {
                                    property.SetValue(item, valor, null);
                                }
                            }
                            list.Add(item);

                            //Proximo
                            leitor.Next();
                        }
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Efetua um select na base e retorna uma list de objeto T com base em um filtro e order by 
        /// <c> List<Usuario> lista = DAL.ListarObjetos<Usuario>(typeof(Usuario));</c>
        /// </summary>
        /// <typeparam name="E">Tipo do List de retorno</typeparam>
        /// <param name="filtro">Use a classe Filtros para montar o filtro</param>
        /// <param name="ordem">É string, mas ao invés de passar os campos diretamente, use a classe Order</param>
        /// <returns>Retorna uma List de instâncias do Tipo</returns>
        public static List<E> ListarObjetos<E>(Filtros filtro, Ordem ordem) where E : class, new()
        {
            return ListarObjetos<E>(filtro.ToString(), ordem.ToString());
        }
        #endregion

        #region rotinas auxiliares PROTECTED para montar código sql
        protected static string GetCampo<T, O>(Expression<Func<T>> exp, O obj)
        {
            return Auxiliar.GetFieldName(exp, obj);
        }

        // retorna um "select campo1, campo2, campo3 from tabela" a partir do objeto passar por parametro
        protected static string GetSqlSelect(object data, string filtro = "", string ordem = "", string group = "", int limite = 0)
        {
            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo
            List<Campo> campos = Montador.GetCampos(data);

            StringBuilder sqlCampos = new StringBuilder();
            foreach (Campo campo in campos)
            {
                sqlCampos.Append(string.Format("{0},", campo.Nome));
            }
            sqlCampos.Remove(sqlCampos.Length - 1, 1);

            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            sql.Append(sqlCampos.ToString());
            sql.Append(" from ");
            sql.Append(Montador.GetTableName(data));

            // temos where?
            if (!filtro.Trim().Equals(string.Empty))
            {
                sql.Append(" where ");
                sql.Append(filtro);
            }

            // temos order by?
            if (!ordem.Trim().Equals(string.Empty))
            {
                sql.Append(" order by ");
                sql.Append(ordem);
            }

            // temos order by?
            if (!group.Trim().Equals(string.Empty))
            {
                sql.Append(" group by ");
                sql.Append(group);
            }

            // temos um limite definido de registros?
            if (limite > 0)
            {
                sql.Append(" limit ");
                sql.Append(limite);
            }

            return sql.ToString();
        }

        // monta o código de insert e update
        public static string GetSqlInsertUpdate(string tableName, List<Campo> campos)
        {
            // campo1, campo2, campo3  (usado no inicio - insert into X (AQUI) values ...
            StringBuilder sqlCampos = new StringBuilder();

            // @campo1, @campo3, @campo3 (usado nos valores - insert into X (c,c,c) values (AQUI)
            StringBuilder sqlInsert = new StringBuilder();

            // campo1=@campo1, campo2=@campo3 (usado no on duplicate) - insert into x (c) values (@c) on duplicate key update AQUI
            StringBuilder sqlUpdate = new StringBuilder();
            string sqlId = string.Empty;

            foreach (Campo campo in campos)
            {
                if ((!campo.IsOnlyUpdate) && (!campo.IsOnlySelect))
                {
                    sqlCampos.Append(string.Format("{0},", campo.Nome));
                    sqlInsert.Append(string.Format("@{0},", campo.Nome));
                }

                if (campo.IsKey)
                    sqlId = string.Format("{0}=@{0}", campo.Nome);
                else
                    if ((!campo.IsOnlyInsert) && (!campo.IsOnlySelect))
                    sqlUpdate.Append(string.Format("{0}=@{0},", campo.Nome));
            }

            // remove a vírgula final de todos
            sqlCampos.Remove(sqlCampos.Length - 1, 1);
            sqlInsert.Remove(sqlInsert.Length - 1, 1);
            sqlUpdate.Remove(sqlUpdate.Length - 1, 1);

            // montamos o sql final
            StringBuilder sql = new StringBuilder();

            // vamos pegar o valor id
            long id = GetIdValue(campos);

            // inserção?
            if (id == 0)
            {
                sql.Append("insert into ");
                sql.Append(tableName);
                sql.Append(" (");
                sql.Append(sqlCampos.ToString());
                sql.Append(") values (");
                sql.Append(sqlInsert.ToString());
                sql.Append(")");
            }
            else
            {
                sql.Append("update ");
                sql.Append(tableName);
                sql.Append(" set ");
                sql.Append(sqlUpdate.ToString());
                sql.Append(" where ");
                sql.Append(sqlId);
            }
            return sql.ToString();
        }

        // usado para localizar um registro com base no campo chave (Attribute IsKey)
        protected static string GetSqlLocalizarId(string tableName, List<Campo> campos)
        {
            // campo1=@campo1, campo2=@campo3 
            StringBuilder sqlCampos = new StringBuilder();
            string campoRetorno = string.Empty;

            foreach (Campo campo in campos)
            {
                if (campo.IsKey)
                    campoRetorno = campo.Nome;
                else
                    sqlCampos.Append(string.Format("{0}=@{0},", campo.Nome));
            }

            // remove a vírgula final de todos
            sqlCampos.Remove(sqlCampos.Length - 1, 1);

            // montamos o sql final
            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            sql.Append(campoRetorno);
            sql.Append(" from ");
            sql.Append(tableName);
            sql.Append(" where ");
            sql.Append(sqlCampos.ToString());
            return sql.ToString();
        }

        // retorna o valor do campo chave (Attribute IsKey)
        protected static long GetIdValue(List<Campo> campos)
        {
            long idRetorno = 0;
            foreach (Campo campo in campos)
            {
                if (campo.IsKey)
                    if (campo.Valor.GetType() == typeof(int))
                    {
                        idRetorno = (int)campo.Valor;
                        break;
                    }
                    else if (campo.Valor.GetType() == typeof(long))
                    {
                        idRetorno = (long)campo.Valor;
                        break;
                    }
            }
            return idRetorno;
        }

        // percorre os campos e retorna o nm_insumo do campo chave (Atributo IsKey)
        protected static string GetIdFieldName(List<Campo> campos)
        {
            foreach (Campo campo in campos)
                if (campo.IsKey)
                    return campo.Nome;
            return string.Empty;
        }
        #endregion

        #region select com join
        /// <summary>
        /// Efetuar um select com JOIN usando com ponto de ligação o attribute IsKey do dataPai e o atribute IsPK do dataFilho
        /// </summary>
        /// <param name="dataPai">Instância do objeto pai (mestre)</param>
        /// <param name="dataFilho">Instância do objeto filho (detalhes)</param>
        /// <param name="filtro">Use a classe Filtros</param>
        /// <param name="ordem">Use a classe Ordem</param>
        /// <returns>Retorna um objeto Leitor</returns>
        /// <code>
        ///    Empresa emp = new Empresa();
        ///    Funcionario f1 = new Funcionario();
        ///    Leitor l = DAL.ListarJoin(emp, f1, new Filtros().ToString(), new Ordem().ToString());
        ///    while (!l.Eof)
        ///    {
        ///        System.Console.WriteLine(l.GetString("cd_empresa") + "-" + l.GetString("nm_empresa") + "=" + l.GetString("cd_funcionario") + "-" + l.GetString("nm_insumo"));
        ///        l.Next();
        ///    }
        /// </code>
        public static Leitor ListarJoin(object dataPai, object dataFilho, string filtro = "", string ordem = "")
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, GetSqlSelectJoin(dataPai, dataFilho, filtro, ordem)))
            {
                return comando.Select();
            }
        }

        /// <summary>
        /// Efetuar um select com JOIN usando com ponto de ligação o attribute IsKey do dataPai e o atribute IsPK do dataFilho
        /// </summary>
        /// <param name="dataPai">Instância do objeto pai (mestre)</param>
        /// <param name="dataFilho">Instância do objeto filho (detalhes)</param>
        /// <param name="filtro">Use a classe Filtros</param>
        /// <param name="ordem">Use a classe Ordem</param>
        /// <returns>Retorna um objeto Leitor</returns>
        /// <code>
        ///    Empresa emp = new Empresa();
        ///    Funcionario f1 = new Funcionario();
        ///    Leitor l = DAL.ListarJoin(emp, f1, new Filtros(), new Ordem());
        ///    while (!l.Eof)
        ///    {
        ///        System.Console.WriteLine(l.GetString("cd_empresa") + "-" + l.GetString("nm_empresa") + "=" + l.GetString("cd_funcionario") + "-" + l.GetString("nm_insumo"));
        ///        l.Next();
        ///    }
        /// </code>
        public static Leitor ListarJoin(object dataPai, object dataFilho, Filtros filtro, Ordem ordem)
        {
            return ListarJoin(dataPai, dataFilho, filtro.ToString(), ordem.ToString());
        }

        /// <summary>
        /// Efetuar um select com JOIN usando com ponto de ligação o attribute IsKey do dataPai e o atribute IsPK do dataFilho
        /// </summary>
        /// <param name="dataPai">Instância do objeto pai (mestre)</param>
        /// <param name="dataFilho">Instância do objeto filho (detalhes)</param>
        /// <param name="filtro">Use a classe Filtros</param>
        /// <param name="ordem">Use a classe Ordem</param>
        /// <returns>Retorna um objeto DataTable</returns>
        public static DataTable ListarDataTableJoin(object dataPai, object dataFilho, string filtro = "", string ordem = "")
        {
            return ListarJoin(dataPai, dataFilho, filtro, ordem).Data;
        }

        /// <summary>
        /// Efetuar um select com JOIN usando com ponto de ligação o attribute IsKey do dataPai e o atribute IsPK do dataFilho
        /// </summary>
        /// <param name="dataPai">Instância do objeto pai (mestre)</param>
        /// <param name="dataFilho">Instância do objeto filho (detalhes)</param>
        /// <param name="filtro">Use um objeto da classe Filtros</param>
        /// <param name="ordem">Use um objeto da classe Ordem</param>
        /// <returns>Retorna um objeto DataTable</returns>
        /// <code>
        ///     Empresa emp = new Empresa();
        ///     Funcionario f1 = new Funcionario();
        ///     
        ///     DataTable dt = DAL.ListarDataTableJoin(emp, f1, new Filtros(), new Ordem());
        ///     
        ///     foreach (DataRow dr in dt.Rows)
        ///         System.Console.WriteLine(dr["cd_empresa"] + "-" + dr["nm_empresa"] + "=" + dr["cd_funcionario"] + "-" + dr["nm_insumo"]);
        /// </code>
        public static DataTable ListarDataTableJoin(object dataPai, object dataFilho, Filtros filtro, Ordem ordem)
        {
            return ListarDataTableJoin(dataPai, dataFilho, filtro.ToString(), ordem.ToString());
        }

        // retorna um "select campo1, campo2, campo3 from tabela" a partir do objeto passar por parametro
        protected static string GetSqlSelectJoin(object dataPai, object dataFilho, string filtro = "", string ordem = "")
        {
            // o Montador.GetCampos retorna num List<Campo> o nm_insumo do campo
            List<Campo> camposPai = Montador.GetCampos(dataPai);
            string tabelaPai = Montador.GetTableName(dataPai);
            string campoKeyPai = string.Empty;

            List<Campo> camposFilho = Montador.GetCampos(dataFilho);
            string tabelaFilho = Montador.GetTableName(dataFilho);
            string campoFKFilho = string.Empty;

            #region monta os campos
            StringBuilder sqlCampos = new StringBuilder();
            foreach (Campo campo in camposPai)
            {
                if (campo.IsKey)
                {
                    sqlCampos.Append("a.");
                    campoKeyPai = "a." + campo.Nome;
                }
                sqlCampos.Append(string.Format("{0},", campo.Nome));
            }
            foreach (Campo campo in camposFilho)
            {
                if (campo.IsFK)
                    campoFKFilho = "b." + campo.Nome;
                else
                    sqlCampos.Append(string.Format("{0},", campo.Nome));
            }
            sqlCampos.Remove(sqlCampos.Length - 1, 1);
            #endregion

            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            sql.Append(sqlCampos.ToString());
            sql.Append(" from ");
            sql.Append(tabelaFilho);
            sql.Append(" as a join ");
            sql.Append(tabelaPai);
            sql.Append(" as b on ");
            sql.Append(campoKeyPai);
            sql.Append("=");
            sql.Append(campoFKFilho);

            // temos where?
            if (!filtro.Trim().Equals(string.Empty))
            {
                sql.Append(" where ");
                // caso tenha no where o campo key ou FK, trocamos pelo "lias.campo" para não dar erro de ambiguous field
                filtro = filtro.Replace(campoKeyPai.Split('.')[1], campoKeyPai);
                sql.Append(filtro);
            }

            // temos order by?
            if (!ordem.Trim().Equals(string.Empty))
            {
                sql.Append(" order by ");
                // caso tenha no order o campo key ou FK, trocamos pelo "lias.campo" para não dar erro de ambiguous field
                ordem = ordem.Replace(campoKeyPai.Split('.')[1], campoKeyPai);
                sql.Append(ordem);
            }
            return sql.ToString();
        }
        #endregion

        #region funções que retornar 1 campo de 1 registro
        /// <summary>
        /// Retorna 1 valor (campo) de 1 registro
        /// </summary>
        /// <param name="sql">Código SQL para o comando</param>
        /// <param name="padrao">Caso o SQL não retorne nada ou retorno inválido, esse valor retorna no lugar</param>
        /// <returns>Retorna um valor</returns>
        /// <code>
        ///    decimal valor = DAL.GetDecimal("select salario from funcionario where id=10", 10);
        ///    Console.WriteLine(valor);
        /// </code>
        public static decimal GetDecimal(string sql, decimal padrao = 0)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                try
                {
                    decimal dec = decimal.Parse(comando.ExecuteScalar(sql).ToString());
                    return dec;
                }
                catch
                {
                    return padrao;
                }
            }
        }

        /// <summary>
        /// Retorna 1 valor (campo) de 1 registro
        /// </summary>
        /// <param name="sql">Código SQL para o comando</param>
        /// <param name="padrao">Caso o SQL não retorne nada ou retorno inválido, esse valor retorna no lugar</param>
        /// <returns>Retorna um valor</returns>
        /// <code>
        ///    string nm_insumo = DAL.GetString("select nm_insumo from funcionario where id=10");
        ///    Console.WriteLine(nm_insumo);
        /// </code>
        public static string GetString(string sql, string padrao = "")
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                try
                {
                    return comando.ExecuteScalar(sql).ToString();
                }
                catch
                {
                    return padrao;
                }
            }
        }

        /// <summary>
        /// Retorna 1 valor (campo) de 1 registro
        /// </summary>
        /// <param name="sql">Código SQL para o comando</param>
        /// <param name="padrao">Caso o SQL não retorne nada ou retorno inválido, esse valor retorna no lugar</param>
        /// <returns>Retorna um valor</returns>
        /// <code>
        ///    int i = DAL.GetInt("select cd_empresa from funcionario where id=10");
        ///    Console.WriteLine(i);
        /// </code>
        public static decimal GetInt(string sql, int padrao = 0)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                conexao.Open();
                try
                {
                    int i = int.Parse(comando.ExecuteScalar(sql).ToString());
                    return i;
                }
                catch (Exception e)
                {
                    return padrao;
                }
            }
        }

        /// <summary>
        /// Retorna 1 valor (campo) de 1 registro
        /// </summary>
        /// <param name="sql">Código SQL para o comando</param>
        /// <returns>Retorna um valor</returns>
        /// <code>
        ///    DateTime dt = DAL.GetDate("select nascimento from funcionario where id=10");
        ///    Console.WriteLine(dt);
        /// </code>
        public static DateTime GetDate(string sql)
        {
            using (MySqlConnection conexao = new MySqlConnection(GetStringConexao()))
            using (Comando comando = new Comando(conexao, sql))
            {
                try
                {
                    DateTime dt = DateTime.Parse(comando.ExecuteScalar(sql).ToString());
                    return dt;
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }
        #endregion
    }
}