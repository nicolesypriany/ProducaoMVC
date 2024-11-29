using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class ProcessoProducaoController : Controller
    {
        private readonly ProducaoContext _context;

        public ProcessoProducaoController(ProducaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<ProcessoProducao> producoes = _context.Producoes
                .Include(p => p.Maquina)
                .Include(p => p.Forma)
                .Include(p => p.Produto)
                .Include(p => p.ProducaoMateriasPrimas)
                .ThenInclude(p => p.MateriaPrima)
                .ToList();

            return View(producoes);
        }

        public IActionResult Criar()
        {
            var maquinas = _context.Maquinas.ToList();
            var formas = _context.Formas.ToList();
            var produtos = _context.Produtos.ToList();
            var materiasPrimas = _context.MateriasPrimas.ToList();

            var materiasCheckbox = new List<MateriaPrimaCheckboxViewModel>();
            
            foreach (var materia in materiasPrimas)
            {
                var materiaCheck = new MateriaPrimaCheckboxViewModel
                {
                    Id = materia.Id,
                    Nome = materia.Nome,
                    Selecionado = false,
                    Quantidade = 0
                };
                materiasCheckbox.Add(materiaCheck);
            }

            var viewModel = new ProducaoViewModel
            {
                Maquinas = maquinas,
                Formas = formas,
                Produtos = produtos,
                MateriasPrimas = materiasPrimas,
                MateriasPrimasCheckbox = materiasCheckbox
            };

            return View(viewModel);
        }

        public IActionResult Editar(int id)
        {
            ProcessoProducao producao = _context.Producoes.Include(p => p.ProducaoMateriasPrimas).FirstOrDefault(m => m.Id == id);
            var maquinas = _context.Maquinas.ToList();
            var formas = _context.Formas.ToList();
            var produtos = _context.Produtos.ToList();
            var materiasPrimas = _context.MateriasPrimas.ToList();
            var producaoMaterias = _context.ProducoesMateriasPrimas.ToList();

            var materiasPrimasDaProducao = producao.ProducaoMateriasPrimas.ToList();
            var materiasCheckbox = new List<MateriaPrimaCheckboxViewModel>();

            foreach (var materia in materiasPrimas)
            {
                var materiaCheck = new MateriaPrimaCheckboxViewModel
                {
                    Id = materia.Id,
                    Nome = materia.Nome,
                    Selecionado = false,
                    Quantidade = 0
                };
                materiasCheckbox.Add(materiaCheck);
            }

            for (int i = 0; i < materiasCheckbox.Count; i++)
            {

                foreach (var item in materiasPrimasDaProducao)
                {
                    if (item.MateriaPrimaId == materiasCheckbox[i].Id)
                    {
                        materiasCheckbox[i].Selecionado = true;
                        materiasCheckbox[i].Quantidade = item.Quantidade;
                    }
                }
            }

            var viewModel = new ProducaoViewModel
            {
                Producao = producao,
                Data = producao.Data,
                MaquinaId = producao.MaquinaId,
                Maquinas = maquinas,
                FormaId = producao.FormaId,
                Formas = formas,
                ProdutoId = producao.ProdutoId,
                Produtos = produtos,
                Ciclos = producao.Ciclos,
                MateriasPrimas = materiasPrimas,
                MateriasPrimasCheckbox = materiasCheckbox
            };

            return View(viewModel);
        }

        public IActionResult DeletarConfirmacao(int id)
        {
            ProcessoProducao producao = _context.Producoes.FirstOrDefault(m => m.Id == id);
            return View(producao);
        }

        public IActionResult Deletar(int id)
        {
            ProcessoProducao producao = _context.Producoes.FirstOrDefault(m => m.Id == id);
            _context.Producoes.Remove(producao);
            _context.SaveChanges();
            TempData["MensagemSucesso"] = "Produção apagada com sucesso";
            
            return RedirectToAction("Index");
        }

        public ProcessoProducao CriarProducaoPorModelo(ProducaoViewModel producaoVm)
        {
            List<MateriaPrima> materiasSelecionadas = new List<MateriaPrima>();
            List<ProducaoMateriaPrima> producaoMateriaPrimas = new List<ProducaoMateriaPrima>();

            foreach (var item in producaoVm.MateriasPrimasCheckbox)
            {
                if (item.Selecionado)
                {
                    materiasSelecionadas.Add(_context.MateriasPrimas.FirstOrDefault(m => m.Id == item.Id));
                }
            }

            Forma forma = _context.Formas.FirstOrDefault(f => f.Id == producaoVm.FormaId);

            var producao = new ProcessoProducao();
            producao.Data = producaoVm.Data;
            producao.MaquinaId = producaoVm.MaquinaId;
            producao.FormaId = producaoVm.FormaId;
            producao.ProdutoId = forma.ProdutoId;
            producao.Ciclos = producaoVm.Ciclos;
            producao.ProducaoMateriasPrimas = new List<ProducaoMateriaPrima>();

            for (int i = 0; i < materiasSelecionadas.Count; i++)
            {
                var producaoMateriaPrima = new ProducaoMateriaPrima();
                producaoMateriaPrima.ProducaoId = producao.Id;
                producaoMateriaPrima.MateriaPrimaId = materiasSelecionadas[i].Id;
                producaoMateriaPrima.Quantidade = producaoVm.MateriasPrimasCheckbox[i].Quantidade;
                producao.ProducaoMateriasPrimas.Add(producaoMateriaPrima);
            };

            return (producao);
        }

        [HttpPost]
        public IActionResult Criar(ProducaoViewModel producaoVm)
        {
            try
            {
                var producao = CriarProducaoPorModelo(producaoVm);
                _context.Producoes.Add(producao);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Produção cadastrada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos cadastrar a produção, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Editar(ProducaoViewModel producaoVm)
        {
            try
            {
                ProcessoProducao producaoDb = _context.Producoes.Include(p => p.ProducaoMateriasPrimas).FirstOrDefault(m => m.Id == producaoVm.Producao.Id);
                Forma forma = _context.Formas.FirstOrDefault(m => m.Id == producaoVm.FormaId);
                var todasMateriasPrimas = _context.MateriasPrimas.ToList();

                List<int> iDMateriasSelecionadas = new List<int>();
                foreach (var item in producaoVm.MateriasPrimasCheckbox)
                {
                    if (item.Selecionado)
                    {
                        iDMateriasSelecionadas.Add(item.Id);
                    }
                }

                List<ProducaoMateriaPrima> producoesMateriasPrimas = producaoDb.ProducaoMateriasPrimas.ToList();

                List<int> iDMateriasAntigas = new List<int>();
                foreach (var item in producoesMateriasPrimas)
                {
                    iDMateriasAntigas.Add(item.MateriaPrimaId);
                }

                //verifica se a materia prima ja esta na lista e atribui a nova quantidade
                foreach (var item in iDMateriasSelecionadas)
                {
                    for(int i = 0; i < producoesMateriasPrimas.Count; i++)
                    {
                        if (producoesMateriasPrimas[i].MateriaPrimaId == item)
                        {
                            var item1 = producaoVm.MateriasPrimasCheckbox.FirstOrDefault(p => p.Id == item);
                            producoesMateriasPrimas[i].Quantidade = item1.Quantidade;
                        }
                    }
                }

                List<int> idMateriasNovas = new List<int>();
                foreach (var id in iDMateriasSelecionadas)
                {
                    if (!iDMateriasAntigas.Contains(id))
                    {
                        idMateriasNovas.Add(id);
                    }
                }

                List<ProducaoMateriaPrima> producoesnovas = new List<ProducaoMateriaPrima>();
                //adiciona as novas materias primas selecionadas
                for (int i = 0; i < idMateriasNovas.Count; i++)
                {
                    foreach (var item in producaoVm.MateriasPrimasCheckbox)
                    {
                        if (idMateriasNovas[i] == item.Id)
                        {
                            var novaProducaoMateria = new ProducaoMateriaPrima();
                            novaProducaoMateria.ProducaoId = producaoDb.Id;
                            novaProducaoMateria.MateriaPrimaId = idMateriasNovas[i];
                            novaProducaoMateria.Quantidade = item.Quantidade;
                            producaoDb.ProducaoMateriasPrimas.Add(novaProducaoMateria);
                            producoesnovas.Add(novaProducaoMateria);
                        }
                    }
                }

                foreach (var item in producoesnovas)
                {
                    producaoDb.ProducaoMateriasPrimas.Add(item);
                }

                List<int> idMateriasADeletar = new List<int>();
                foreach(var item in iDMateriasAntigas)
                {
                    if (!iDMateriasSelecionadas.Contains(item))
                    {
                        idMateriasADeletar.Add(item);
                    }
                }

                foreach (var item in idMateriasADeletar)
                {
                    var pm = producoesMateriasPrimas.FirstOrDefault(p => p.MateriaPrimaId == item);
                    _context.ProducoesMateriasPrimas.Remove(pm);
                }


                producaoDb.Data = producaoVm.Data;
                producaoDb.MaquinaId = producaoVm.MaquinaId;
                producaoDb.FormaId = producaoVm.FormaId;
                producaoDb.ProdutoId = forma.ProdutoId;
                producaoDb.Ciclos = producaoVm.Ciclos;
                
                _context.Producoes.Update(producaoDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Produção alterada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos atualizar a produção, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult CalcularProducao(ProcessoProducao producaoSelecionada)
        {
            var producao = _context.Producoes
                .Include(p => p.Forma)
                .ThenInclude(f => f.Produto)
                .Include(p => p.ProducaoMateriasPrimas)
                .ThenInclude(p => p.MateriaPrima)
                .FirstOrDefault(p => p.Id == producaoSelecionada.Id);

            producao.QuantidadeProduzida = (producao.Ciclos * producao.Forma.PecasPorCiclo) / producao.Produto.PecasPorUnidade;
            producao.CustoTotal = 0;

            foreach(var item in producao.ProducaoMateriasPrimas)
            {
                var soma = item.Quantidade * item.MateriaPrima.Preco;
                producao.CustoTotal += soma;
            }
            producao.CustoUnitario = producao.CustoTotal / producao.QuantidadeProduzida;
            
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
