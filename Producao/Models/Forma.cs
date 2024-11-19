namespace Producao.Models
{
    public class Forma
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Produto Produto { get; set; }
        public int ProdutoId { get; set; }
        public int PecasPorCiclo { get; set; }
        public ICollection<Maquina> Maquinas { get; set; }
        public ICollection<ProcessoProducao> Producoes { get; set; }
    }
}
