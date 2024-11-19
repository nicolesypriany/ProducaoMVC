namespace Producao.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Medidas { get; set; }
        public string Unidade { get; set; }
        public int PecasPorUnidade { get; set; }
        public ICollection<Forma> Formas { get; set; }
        public ICollection<ProcessoProducao> Producoes { get; set; }
    }
}
