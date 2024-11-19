using Microsoft.AspNetCore.Mvc;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class MaquinaController : Controller
    {
        private readonly ProducaoContext _context;

        public MaquinaController(ProducaoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Maquina> maquinas = _context.Maquinas.ToList();
            return View(maquinas);  
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            Maquina maquina = _context.Maquinas.FirstOrDefault(m => m.Id == id);
            return View(maquina);
        }

        public IActionResult DeletarConfirmacao(int id)
        {
            Maquina maquina = _context.Maquinas.FirstOrDefault(m => m.Id == id);
            return View(maquina);
        }

        public IActionResult Deletar(int id)
        {
            Maquina maquina = _context.Maquinas.FirstOrDefault(m => m.Id == id);
            _context.Maquinas.Remove(maquina);
            _context.SaveChanges();
            TempData["MensagemSucesso"] = "Máquina apagada com sucesso";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Criar(Maquina maquina)
        {
            try
            {
                _context.Maquinas.Add(maquina);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Máquina cadastrada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos cadastrar a sua máquina, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Editar(Maquina maquina)
        {
            try
            {
                Maquina maquinaDb = _context.Maquinas.FirstOrDefault(m => m.Id == maquina.Id);

                maquinaDb.Nome = maquina.Nome;
                maquinaDb.Marca = maquina.Marca;

                _context.Maquinas.Update(maquinaDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Máquina alterada com sucesso";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos atualizar a máquina, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
