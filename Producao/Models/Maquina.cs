namespace Producao.Models
{
    public class Maquina
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public List<Forma> Formas { get; set; }
        public ICollection<ProcessoProducao> Producoes { get; set; }
    }
}
