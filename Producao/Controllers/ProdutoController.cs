using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly ProducaoContext _context;

        public ProdutoController(ProducaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Produto> produtos = _context.Produtos.Include(p => p.Formas).ToList();
            return View(produtos);  
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            Produto produto = _context.Produtos.FirstOrDefault(m => m.Id == id);
            return View(produto);
        }

        public IActionResult DeletarConfirmacao(int id)
        {
            Produto produto = _context.Produtos.FirstOrDefault(m => m.Id == id);
            return View(produto);
        }

        public IActionResult Deletar(int id)
        {
            Produto produto = _context.Produtos.Include(p => p.Formas).FirstOrDefault(m => m.Id == id);
            if(produto.Formas.Count > 0)
            {
                TempData["MensagemErro"] = "Existem formas com esse produto!";
            } else
            {
                _context.Produtos.Remove(produto);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Produto apagado com sucesso";
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Criar(Produto produto)
        {
            try
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Produto cadastrado com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos cadastrar o seu produto, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Editar(Produto produto)
        {
            try
            {
                Produto produtoDb = _context.Produtos.FirstOrDefault(m => m.Id == produto.Id);

                produtoDb.Nome = produto.Nome;
                produtoDb.Medidas = produto.Medidas;
                produtoDb.Unidade = produto.Unidade;
                produtoDb.PecasPorUnidade = produto.PecasPorUnidade;

                _context.Produtos.Update(produtoDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Produto alterado com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos atualizar o produto, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
