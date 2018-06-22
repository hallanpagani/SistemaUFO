using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BasePersistencia.banco
{
    /// <summary>
    /// Faz todo o controle de transação para gravação no banco, facilita o trabalho por diminuir o código ao programar a DAL
    /// </summary>
    public class Transacao : IDisposable
    {
        public bool InTransaction { get; set; }

        private MySqlTransaction transacaoMySql = null;

        public MySqlTransaction GetTransaction()
        {
            return this.transacaoMySql;
        }

        public Transacao(Conexao conexao)
        {
            this.transacaoMySql = conexao.GetConnection().BeginTransaction();
            this.InTransaction = true;
        }

        public Transacao(MySqlConnection conexao)
        {
            this.transacaoMySql = conexao.BeginTransaction();
            this.InTransaction = true;
        }

        public void Commit()
        {
            this.transacaoMySql.Commit();
            this.InTransaction = false;
        }

        public void RollBack()
        {
            this.transacaoMySql.Rollback();
            this.InTransaction = false;
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
                    if (this.InTransaction)
                    {
                        RollBack();
                        throw new Exception("Processo em transação e não foram efetuados comandos COMMIT ou ROLLBACK.");
                    }

                    this.transacaoMySql.Dispose();
                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                disposed = true;
            }
        }

        // Por fim, precisamos programar o método finalizador da classe, que é invocado pelo garbage collector para liberar os recursos. 
        // Toda liberacao implementada dentro dele só será executada quando o metodo for invocado, 
        // porém não conseguimos saber quando isto vai acontecer e recursos importantes podem ficar presos até que isto aconteça.
        ~Transacao()
        {
            // Do not re-create Dispose clean-up code here. Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
