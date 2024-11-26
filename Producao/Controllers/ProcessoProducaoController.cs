﻿using Microsoft.AspNetCore.Mvc;
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
                ProducaoMateriaPrima materia = _context.ProducoesMateriasPrimas.FirstOrDefault(p => p.MateriaPrimaId == materiasCheckbox[i].Id);
                if (materiasPrimasDaProducao.Contains(materia))
                {
                    materiasCheckbox[i].Selecionado = true;
                    materiasCheckbox[i].Quantidade = materia.Quantidade;
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

            var producao = new ProcessoProducao();
            producao.Data = producaoVm.Data;
            producao.MaquinaId = producaoVm.MaquinaId;
            producao.FormaId = producaoVm.FormaId;
            producao.ProdutoId = producaoVm.ProdutoId;
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

        public IActionResult CalcularProducao(ProcessoProducao producaoSelecionada)
        {
            var producao = _context.Producoes
                .Include(p => p.Forma)
                .ThenInclude(f => f.Produto)
                .Include(p => p.ProducaoMateriasPrimas)
                .ThenInclude(p => p.MateriaPrima)
                .FirstOrDefault(p => p.Id == producaoSelecionada.Id);

            //var forma = _context.Formas.FirstOrDefault(f => f.Id == producaoSelecionada.Id);
            //var produto = _context.Produtos.FirstOrDefault(p => p.Id == forma.ProdutoId);
            //var producaoMateriasPrimas = _context.ProducoesMateriasPrimas.ToList();
            //var materiasPrimas = _context.MateriasPrimas.ToList();

            //producao.QuantidadeProduzida = (producao.Ciclos * forma.PecasPorCiclo)/produto.PecasPorUnidade;
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
