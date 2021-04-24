namespace SD.ModuloV.Atividade5.Server
{
    public enum PessoaGenero
    {
        Feminino = 0,
        Masculino = 1
    }

    public class Pessoa
    {
        public decimal Altura { get; set; }
        public PessoaGenero Genero { get; set; }
    }
}
