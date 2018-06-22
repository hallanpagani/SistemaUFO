using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BasePersistencia.banco
{
    /// <summary>
    /// Encapsula e faz o controle de conectar na base, abrir e fechar. É Disposable, permite o uso de "using"
    /// </summary>
    public class Conexao : IDisposable
    {
        private MySqlConnection conexaoMySql = null;

        public static Conexao Get(string connectionString)
        {
            Conexao conexao = new Conexao(connectionString);
            conexao.Connect();
            return conexao;
        }

        public Conexao(string connectionString)
        {
            this.conexaoMySql = new MySqlConnection(connectionString);
        }

        public MySqlConnection GetConnection()
        {
            return this.conexaoMySql;
        }

        public bool IsConnected()
        {
            return (this.conexaoMySql.State == ConnectionState.Open);
        }

        public void Connect()
        {
            this.conexaoMySql.Open();
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
                    this.conexaoMySql.Close();
                    this.conexaoMySql.Dispose();
                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                disposed = true;
            }
        }

        // Por fim, precisamos programar o método finalizador da classe, que é invocado pelo garbage collector para liberar os recursos. 
        // Toda liberacao implementada dentro dele só será executada quando o metodo for invocado, 
        // porém não conseguimos saber quando isto vai acontecer e recursos importantes podem ficar presos até que isto aconteça.
        ~Conexao()
        {
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}