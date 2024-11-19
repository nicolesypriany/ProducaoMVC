namespace Producao.Models
{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Fornecedor { get; set; }
        public string Unidade { get; set; }
        public double Preco { get; set; }
        public ICollection<ProducaoMateriaPrima> ProducaoMateriasPrimas { get; set; }
    }
}
