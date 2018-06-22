using System;

namespace BaseModelo.model.generico
{
    public class Respostas
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public object Dados { get; set; }
        public int ChavePrimaria { get; set; }

        public Respostas(bool sucesso, string mensagem, object dados)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Dados = dados;
            ChavePrimaria = 0;
        }

        public Respostas(bool sucesso, string mensagem, int dados)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Dados = null;
            ChavePrimaria = dados;
        }

        public Respostas()
        {
            Sucesso = true;
            Mensagem = null;
            Dados = null;
            ChavePrimaria = 0;
        }
    }
}