using System;

namespace BaseModelo.classes
{
    /*    
     * [Table("tb_usuario")]
     * public class Usuario
     * {
     *     [Key]
     *     [Column("id_usuario"]
     *     public int Id { get; set; }
     * 
     *     [Required]
     *     [Column("nm_insumo")]
     *     public string Nome { get; set; }
     * }
     * 
     * -------------------------- 
     * Usuario u = new Usuario() { Id = 1, Nome = "Maria" }       
     */

    // classe "Campo" é utilizada para conter todas as informações necessárias dos campos e valores - estas informações
    // são capturadas de um objeto Model usando suas propriedades e valores
    // estas informações ficam neste objeto, com o nm_insumo do campo (lido através de attributes / anotations)
    public class Campo
    {
        // nm_insumo do campo na tabela física obtido via attributes - no exemplo acima pode ser o campo 'id_usuario'
        public string Nome { get; set; }

        // se é campo chave obtido via attribute do model - no exemplo acima seria true 
        public bool IsKey { get; set; }

        // se é campo auto-incremento
        public bool IsAutoInc { get; set; }

        // conteúdo a ser gravado no banco de dados é object porque pode ser int, string, date, etc
        // ele é obtido diretamente do model - no exempo acima seria "1"
        public object Valor { get; set; }

        // se é campo obrigatório
        public bool Required { get; set; }

        // se é campo chave estrangeira
        public bool IsFK { get; set; }
        public string NomeFK { get; set; }

        // tipo primitivo do campo (DateTime, string, int, etc) - não usado diretamente, 
        // mas preenchido automaticamente na classe "Montador" para auxiliar rotinas internas
        public Type Tipo { get; set; }

        // não inclui o campo quando faz updade
        public bool IsOnlyInsert { get; set; }

        // não inclui o campo quando faz insert
        public bool IsOnlyUpdate { get; set; }

        // não inclui o campo quando faz insert ou update
        public bool IsOnlySelect { get; set; }

        // descrição
        public string Descricao { get; set; }
    }
}
