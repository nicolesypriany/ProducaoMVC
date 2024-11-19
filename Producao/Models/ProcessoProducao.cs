﻿namespace Producao.Models
{
    public class ProcessoProducao
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int MaquinaId { get; set; }
        public Maquina Maquina { get; set; }
        public int FormaId { get; set; }
        public Forma Forma { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public int Ciclos {  get; set; }
        public double QuantidadeProduzida { get; set; }
        public ICollection<ProducaoMateriaPrima> ProducaoMateriasPrimas { get; set; }
        public double CustoUnitario { get; set; }
        public double CustoTotal { get; set; }

    }
}
