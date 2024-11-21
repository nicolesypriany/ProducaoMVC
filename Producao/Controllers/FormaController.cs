using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class FormaController : Controller
    {
        private readonly ProducaoContext _context;

        public FormaController(ProducaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Forma> formas = _context.Formas.Include(f => f.Produto).ToList();
            return View(formas);
        }

        public IActionResult Criar()
        {
            var produtos = new List<Produto>();
            produtos = _context.Produtos.ToList();

            var maquinas = new List<Maquina>();
            maquinas = _context.Maquinas.ToList();

            var model = maquinas.Select(m => new MaquinaViewModel
            {
                Id = m.Id,
                Nome = m.Nome,
                Selecionado = false
            }).ToList();

            var viewModel = new FormaProdutoViewModel
            {
                Forma = new Forma(),
                Produtos = produtos,
                Maquinas = model
            };

            return View(viewModel);
        }

        public IActionResult Editar(int id)
        {
            Forma forma = _context.Formas.FirstOrDefault(m => m.Id == id);
            var produtos = new List<Produto>();
            produtos = _context.Produtos.ToList();

            var viewModel = new FormaProdutoViewModel
            {
                Forma = forma,
                Produtos = produtos
            };
            
            return View(viewModel);
        }

        public IActionResult DeletarConfirmacao(int id)
        {
            Forma forma = _context.Formas.FirstOrDefault(m => m.Id == id);
            return View(forma);
        }

        public IActionResult Deletar(int id)
        {
            Forma forma = _context.Formas.FirstOrDefault(m => m.Id == id);
            _context.Formas.Remove(forma);
            _context.SaveChanges();
            TempData["MensagemSucesso"] = "Forma apagada com sucesso";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Criar(Forma forma)
        {
            try
            {
                _context.Formas.Add(forma);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Forma cadastrada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos cadastrar a sua forma, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Editar(Forma forma)
        {
            try
            {
                Forma formaDb = _context.Formas.FirstOrDefault(m => m.Id == forma.Id);

                formaDb.Nome = forma.Nome;
                formaDb.ProdutoId = forma.ProdutoId;
                formaDb.PecasPorCiclo = forma.PecasPorCiclo;

                _context.Formas.Update(formaDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Forma alterada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos atualizar a forma, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
