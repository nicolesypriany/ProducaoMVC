using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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
            List<Forma> formas = _context.Formas.Include(f => f.Produto).Include(f => f.Maquinas).ToList();
            return View(formas);
        }

        public IActionResult Criar()
        {
            var produtos = _context.Produtos.ToList();

            var maquinas = _context.Maquinas.ToList();

            var maquinasCheckbox = new List<MaquinaCheckboxViewModel>();

            foreach (var maquina in maquinas)
            {
                var maquinaCheck = new MaquinaCheckboxViewModel
                {
                    Id = maquina.Id,
                    Nome = maquina.Nome,
                    Selecionado = false
                };
                maquinasCheckbox.Add(maquinaCheck);
            }

            var viewModel = new FormaProdutoViewModel
            {
                Produtos = produtos,
                Maquinas = maquinas,
                MaquinasCheckbox = maquinasCheckbox,
            };
 
            return View(viewModel);
        }

        public IActionResult Editar(int id)
        {
            var produtos = _context.Produtos.ToList();

            var maquinas = _context.Maquinas.ToList();

            Forma forma = _context.Formas.Include(f => f.Maquinas).FirstOrDefault(m => m.Id == id);
            var maquinasDaForma = forma.Maquinas.ToList();

            var maquinasCheckbox = new List<MaquinaCheckboxViewModel>();

            foreach (var maquina in maquinas)
            {
                var maquinaCheck = new MaquinaCheckboxViewModel
                {
                    Id = maquina.Id,
                    Nome = maquina.Nome,
                    Selecionado = false
                };
                maquinasCheckbox.Add(maquinaCheck);
            }

            for (int i = 0; i < maquinasCheckbox.Count; i++)
            {
                Maquina maquina = _context.Maquinas.FirstOrDefault(m => m.Id == maquinasCheckbox[i].Id);
                if (maquinasDaForma.Contains(maquina))
                {
                    maquinasCheckbox[i].Selecionado = true;
                }
            }

            var viewModel = new FormaProdutoViewModel
            {
                Forma = forma,
                Nome = forma.Nome,
                ProdutoId = forma.ProdutoId,
                PecasPorCiclo = forma.PecasPorCiclo,
                Maquinas = maquinas,
                MaquinasCheckbox = maquinasCheckbox,
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

        public Forma CriarFormaPorModelo(FormaProdutoViewModel formaVm)
        {
            List<Maquina> maquinasSelecionadas = new List<Maquina>();
            foreach (var item in formaVm.MaquinasCheckbox)
            {
                if (item.Selecionado)
                {
                    maquinasSelecionadas.Add(_context.Maquinas.FirstOrDefault(m => m.Id == item.Id));
                }
            }

            var forma = new Forma();
            forma.Nome = formaVm.Nome;
            forma.ProdutoId = formaVm.ProdutoId;
            forma.PecasPorCiclo = formaVm.PecasPorCiclo;
            forma.Maquinas = maquinasSelecionadas;

            return (forma);
        }

        [HttpPost]
        public IActionResult Criar(FormaProdutoViewModel formaVm)
        {
            try
            {
                var forma = CriarFormaPorModelo(formaVm);
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
        public IActionResult Editar(FormaProdutoViewModel formaVm)
        {
            try
            {
                List<Maquina> maquinasSelecionadas = new List<Maquina>();
                foreach (var item in formaVm.MaquinasCheckbox)
                {
                    if (item.Selecionado)
                    {
                        maquinasSelecionadas.Add(_context.Maquinas.FirstOrDefault(m => m.Id == item.Id));
                    }
                }

                Forma formaDb = _context.Formas.Include(f => f.Maquinas).FirstOrDefault(m => m.Id == formaVm.Forma.Id);

                formaDb.Nome = formaVm.Nome;
                formaDb.ProdutoId = formaVm.ProdutoId;
                formaDb.PecasPorCiclo = formaVm.PecasPorCiclo;
                formaDb.Maquinas = maquinasSelecionadas;

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
