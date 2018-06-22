using System;
using System.Data;
using MySql.Data.MySqlClient;
using BasePersistencia.banco;

namespace BasePersistencia.classes
{
    public class Leitor : IDisposable
    {
        private bool Final = false;

        public int CurrentRecord { get; set; }
        public int RecordCount { get { return Data.Rows.Count; } }
        public DataTable Data { get; set; }

        /// <summary>Abre o comando do parâmetro e preenche o DataTable interno</summary>
        public Leitor(Comando comando)
        {
            this.Data = new DataTable();
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(comando.GetCommand()))
                dataAdapter.Fill(this.Data);
            this.First();
        }

        /// <summary>Associa um DataTable com o controller Leitor e posiciona no primeiro registro</summary>
        public Leitor(DataTable data)
        {
            this.Data = data;
            this.First();
        }

        /// <summary>Retorna true case esteja no final do DataTable interno</summary>
        public bool Eof { get { return this.Final; } }

        /// <summary>Move ponteiro do DataTable para próximo registro</summary>
        public void Next()
        {
            CurrentRecord++;
            this.Final = false;

            if (CurrentRecord > Data.Rows.Count)
            {
                Last();
                this.Final = true;
            }
        }

        /// <summary>Move ponteiro do DataTable para registro anterior</summary>
        public void Prev()
        {
            CurrentRecord--;
            this.Final = false;

            if (CurrentRecord < 1)
                First();
        }

        /// <summary>Move ponteiro do DataTable para primeiro registro</summary>
        public void First()
        {
            this.CurrentRecord = 1;
        }

        /// <summary>Move ponteiro do DataTable para último registro</summary>
        public void Last()
        {
            this.CurrentRecord = this.RecordCount;
        }

        #region retornos
        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato string
        /// </summary>
        /// <param name="field">nm_insumo do campo, exemplo: "nm_usuario"</param>
        public string GetString(string field)
        {
            return Data.Rows[CurrentRecord - 1][field].ToString();
        }

        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato object (DataRow)
        /// </summary>
        /// <param name="field">nm_insumo do campo"</param>
        public object GetObject(string field)
        {
            return Data.Rows[CurrentRecord - 1][field];
        }

        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato inteiro
        /// </summary>
        /// <param name="field">nm_insumo do campo, exemplo: "id_cliente"</param>
        public int GetInt(string field)
        {
            int retorno = 0;
            if (!int.TryParse(Data.Rows[CurrentRecord - 1][field].ToString(), out retorno))
                retorno = 0;
            return retorno;
        }

        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato decimal
        /// </summary>
        /// <param name="field">nm_insumo do campo, exemplo: "vl_valor"</param>
        public decimal GetDecimal(string field)
        {
            decimal retorno = 0;
            if (!decimal.TryParse(Data.Rows[CurrentRecord - 1][field].ToString(), out retorno))
                retorno = 0;
            return retorno;
        }

        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato date
        /// </summary>
        /// <param name="field">nm_insumo do campo, exemplo: "dt_inc"</param>
        public DateTime GetDate(string field)
        {
            return DateTime.Parse(Data.Rows[CurrentRecord - 1][field].ToString());
        }

        /// <summary>
        ///  Retorna o campo passado por parâmetro em formato booleano
        /// </summary>
        /// <param name="field">nm_insumo do campo, exemplo: "tp_ativo"</param>
        public bool GetBool(string field)
        {
            return bool.Parse(Data.Rows[CurrentRecord - 1][field].ToString());
        }
        #endregion

        #region controle de dispose
        public void Dispose()
        {
            Dispose(true);
        }

        // booleano para controlar se
        // o método Dispose já foi chamado
        bool disposed = false;

        // método privado para controle
        // da liberação dos recursos
        private void Dispose(bool disposing)
        {
            // Verifique se Dispose já foi chamado.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Liberando recursos gerenciados
                    this.Data.Dispose();
                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                disposed = true;
            }
        }
        #endregion
    }
}
