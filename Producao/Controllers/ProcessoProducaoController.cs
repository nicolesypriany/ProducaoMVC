using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<ProcessoProducao> producoes = _context.Producoes.ToList();
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
            ProcessoProducao producao = _context.Producoes.FirstOrDefault(m => m.Id == id);
            return View(producao);
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

            var producao = new ProcessoProducao();
            producao.Data = producaoVm.Data;
            producao.MaquinaId = producaoVm.MaquinaId;
            producao.FormaId = producaoVm.FormaId;
            producao.ProdutoId = producaoVm.ProdutoId;
            producao.Ciclos = producaoVm.Ciclos;

            for (int i = 0; i < materiasSelecionadas.Count; i++)
            {
                var producaoMateriaPrima = new ProducaoMateriaPrima();
                producaoMateriaPrima.ProducaoId = producao.Id;
                producaoMateriaPrima.MateriaPrimaId = materiasSelecionadas[i].Id;
                producaoMateriaPrima.Quantidade = producaoVm.MateriasPrimasCheckbox[i].Quantidade;
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
        public IActionResult Editar(ProcessoProducao producao)
        {
            try
            {
                ProcessoProducao producaoDb = _context.Producoes.FirstOrDefault(m => m.Id == producao.Id);

                //produtoDb.Nome = produto.Nome;
                //produtoDb.Medidas = produto.Medidas;
                //produtoDb.Unidade = produto.Unidade;
                //produtoDb.PecasPorUnidade = produto.PecasPorUnidade;

                //_context.Produtos.Update(producaoDb);
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
    }
}
