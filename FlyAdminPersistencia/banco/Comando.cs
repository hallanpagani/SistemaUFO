using System;
using System.Data;
using MySql.Data.MySqlClient;
using BasePersistencia.classes;

namespace BasePersistencia.banco
{
    /// <summary>
    /// Objeto que encapsula um dbCommand e facilita o uso atráves de métodos como Execute, AddParam, etc
    /// </summary>
    public class Comando : IDisposable
    {
        private MySqlCommand cmd;
        private long lastInsertId;

        // "abre" um comando passando-se o sql, sem transação
        public Comando(Conexao conexao, string sql = "")
        {
            this.cmd = new MySqlCommand(sql, conexao.GetConnection());
        }

        public Comando(MySqlConnection conexao, string sql = "")
        {
            this.cmd = new MySqlCommand(sql, conexao);
        }

        // com transação
        public Comando(Transacao transacao, string sql = "")
        {
            if (!transacao.InTransaction)
                throw new Exception("Componente de transação não esta ativo para este comando");

            this.cmd = new MySqlCommand(sql, transacao.GetTransaction().Connection, transacao.GetTransaction());
        }

        public DataTable GetDataTable(string sql = "")
        {
            if (sql != "")
                this.Sql = sql;

            DataTable tb = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(this.cmd))
            {
                // transfere os pensamentos da base para o DataTable
                da.Fill(tb);
            }
            return tb;
        }

        /// <summary>Adiciona um parâmetro e valor ao dbCommand</summary>
        public void AddParam(string parameter, object value)
        {
            this.cmd.Parameters.AddWithValue(parameter, value);
        }

        /// <summary>Limpa os parâmetros do dbCommand</summary>
        public void ClearParam()
        {
            this.cmd.Parameters.Clear();
        }

        // propriedade sql (get/set)
        public string Sql { get { return this.cmd.CommandText; } set { this.cmd.CommandText = value; } }

        /// <summary>Executa um dbCommand. Preenche o LastInsertedId em caso de insert</summary>
        public void Execute(string sql = "")
        {
            if (sql != "")
                this.Sql = sql;

            this.cmd.ExecuteNonQuery();
            lastInsertId = cmd.LastInsertedId;
        }

        /// <summary>Após um insert esta propriedade contém o número do id</summary>
        public long LastInsertId { get { return this.lastInsertId; } }

        /// <summary>Executa select e retorna 1 campo 1 registro (object)</summary>
        public object ExecuteScalar(string sql = "")
        {
            if (sql != "")
                this.Sql = sql;

            return this.cmd.ExecuteScalar();
        }

        // retorna o command
        public MySqlCommand GetCommand()
        {
            return this.cmd;
        }

        // executa um select simples e retorna num objeto reader
        public Leitor Select(string sql = "")
        {
            return new Leitor(this);
        }

        #region controle de dispose
        // estes métodos de dispose foi tirado do site da propria MS: 
        // http://msdn.microsoft.com/pt-br/library/vstudio/system.idisposable.aspx
        // e http://ferottoboni.wordpress.com/2010/08/20/exemplo-de-implementacao-de-idisposable/
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        // booleano para controlar se
        // o método Dispose já foi chamado
        bool disposed = false;

        // método privado para controle
        // da liberação dos recursos
        protected virtual void Dispose(bool disposing)
        {
            // Verifique se Dispose já foi chamado.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Liberando recursos gerenciados
                    this.cmd.Dispose();
                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                disposed = true;
            }
        }

        // Por fim, precisamos programar o método finalizador da classe, que é invocado pelo garbage collector para liberar os recursos. 
        // Toda liberacao implementada dentro dele só será executada quando o metodo for invocado, 
        // porém não conseguimos saber quando isto vai acontecer e recursos importantes podem ficar presos até que isto aconteça.
        ~Comando()
        {
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
