namespace Producao.Models
{
    public class ProducaoViewModel
    {
        public DateTime Data { get; set; }
        public List<Maquina> Maquinas { get; set; }
        public List<Forma> Formas { get; set; }
        public List<Produto> Produtos { get; set; }
        public List<MateriaPrima> MateriasPrimas { get; set; }
        public int MaquinaId { get; set; }
        public int FormaId { get; set; }
        public int ProdutoId { get; set; }
        public int Ciclos {  get; set; }
        public List<MateriaPrimaCheckboxViewModel> MateriasPrimasCheckbox { get; set; }
    }
}
