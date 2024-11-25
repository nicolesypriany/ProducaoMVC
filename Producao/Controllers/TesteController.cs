using Microsoft.AspNetCore.Mvc;
using Producao.Data;
using Producao.Models;

namespace Producao.Controllers
{
    public class TesteController : Controller
    {
        private readonly ProducaoContext _context;

        public TesteController(ProducaoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var maquinas = new List<Maquina>();
            maquinas = _context.Maquinas.ToList();

            var model = maquinas.Select(m => new MaquinaCheckboxViewModel
            {
                Id = m.Id,
                Nome = m.Nome,
                Selecionado = false
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(List<MaquinaCheckboxViewModel> model)
        {
            var selecionados = model.Where(m => m.Selecionado).ToList();

            var maquinas = new List<Maquina>();
            maquinas = _context.Maquinas.ToList();

            var maquinasSelecionadas = maquinas
                .Where(m => selecionados.Any(s => s.Id == m.Id))
                .ToList();

            return View(model);
        }
    }
}
