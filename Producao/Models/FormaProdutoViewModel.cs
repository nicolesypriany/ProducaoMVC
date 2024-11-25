namespace Producao.Models
{
    public class FormaProdutoViewModel
    {
        //public Forma Forma { get; set; }
        public string Nome { get; set; }
        public int ProdutoId { get; set; }
        public int PecasPorCiclo { get; set; }
        public List<Produto> Produtos { get; set; }
        public List<Maquina> Maquinas { get; set; }
        public List<MaquinaCheckboxViewModel> MaquinasCheckbox { get; set; }
    }
}
