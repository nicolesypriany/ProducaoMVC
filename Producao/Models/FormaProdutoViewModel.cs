namespace Producao.Models
{
    public class FormaProdutoViewModel
    {
        public Forma Forma { get; set; }
        public List<Produto> Produtos { get; set; }
        public List<MaquinaViewModel> Maquinas { get; set; }
        
    }
}
