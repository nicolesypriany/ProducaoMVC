using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class MateriaPrimaController : Controller
    {
        private readonly ProducaoContext _context;

        public MateriaPrimaController(ProducaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<MateriaPrima> materiasPrimas = _context.MateriasPrimas.ToList();
            return View(materiasPrimas);  
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            MateriaPrima materiaPrima = _context.MateriasPrimas.FirstOrDefault(m => m.Id == id);
            return View(materiaPrima);
        }

        public IActionResult DeletarConfirmacao(int id)
        {
            MateriaPrima materiaPrima = _context.MateriasPrimas.FirstOrDefault(m => m.Id == id);
            return View(materiaPrima);
        }

        public IActionResult Deletar(int id)
        {
            MateriaPrima materiaPrima = _context.MateriasPrimas.FirstOrDefault(m => m.Id == id);
            _context.MateriasPrimas.Remove(materiaPrima);
            _context.SaveChanges();
            TempData["MensagemSucesso"] = "Matéria-prima apagada com sucesso";
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Criar(MateriaPrima materiaPrima)
        {
            try
            {
                _context.MateriasPrimas.Add(materiaPrima);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Matéria-prima cadastrada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos cadastrar a matéria-prima, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Editar(MateriaPrima materiaPrima)
        {
            try
            {
                MateriaPrima materiaPrimaDb = _context.MateriasPrimas.FirstOrDefault(m => m.Id == materiaPrima.Id);

                materiaPrimaDb.Nome = materiaPrima.Nome;
                materiaPrimaDb.Fornecedor = materiaPrima.Fornecedor;
                materiaPrimaDb.Unidade = materiaPrima.Unidade;
                materiaPrimaDb.Preco = materiaPrima.Preco;

                _context.MateriasPrimas.Update(materiaPrimaDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Matéria-prima alterada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos atualizar a matéria-prima, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
