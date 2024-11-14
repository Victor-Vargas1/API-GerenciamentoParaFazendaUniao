using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FazendaUniao.Data;
using FazendaUniao.Models;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;

namespace FazendaUniao.Controllers
{
    public class PlantacaoController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public PlantacaoController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: Plantacao
        public async Task<IActionResult> Index()
        {
            var fazendaUniaoContext = _context.Plantacao.Include(p => p.Insumo);
            return View(await fazendaUniaoContext.ToListAsync());
        }

        public async Task<IActionResult> GeneratePdfPlantacao()
        {
            var plantacoes = await _context.Plantacao
                .Include(p => p.Insumo) // Inclui o Insumo
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                // Cria o PDF
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        // TÍTULO DO RELATÓRIO
                        document.Add(new Paragraph("Relatório de Plantações")
                            .SetFontSize(20)
                            .SetBold());

                        // TABELA DE PLANTACÃO
                        var table = new Table(7); // Agora com 7 colunas: ID, Insumo, Tipo da Planta, Data de Plantio, Data de Colheita, Quantidade de Plantio, Status

                        // CABEÇALHO DA TABELA
                        table.AddHeaderCell("ID da Plantação");
                        table.AddHeaderCell("Insumo");
                        table.AddHeaderCell("Tipo da Planta");
                        table.AddHeaderCell("Data de Plantio");
                        table.AddHeaderCell("Data de Colheita");
                        table.AddHeaderCell("Quantidade de Plantio");
                        table.AddHeaderCell("Status");

                        // PREENCHE COM OS DADOS EXISTENTES
                        foreach (var plantacao in plantacoes)
                        {
                            table.AddCell(plantacao.PlantacaoId.ToString());
                            table.AddCell(plantacao.Insumo?.Nome ?? "N/A");
                            table.AddCell(plantacao.TipoPlanta ?? "N/A");
                            table.AddCell(plantacao.DataPlantio.ToString("dd/MM/yyyy") ?? "N/A");
                            table.AddCell(plantacao.DataColheita.ToString("dd/MM/yyyy") ?? "N/A");
                            table.AddCell(plantacao.QuantidadePlantio.ToString() ?? "0");
                            table.AddCell(plantacao.Status ?? "N/A");
                        }

                        // Adiciona a tabela ao documento
                        document.Add(table);
                    }
                }

                var fileName = "Relatorio_Plantacoes.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        // GET: Plantacao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantacao = await _context.Plantacao
                .Include(p => p.Insumo)
                .FirstOrDefaultAsync(m => m.PlantacaoId == id);
            if (plantacao == null)
            {
                return NotFound();
            }

            return View(plantacao);
        }

        // GET: Plantacao/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Insumos = await _context.Insumo.Where(i => i.Quantidade > 0).ToListAsync(); // Somente insumos com quantidade disponível
            return View();
        }

        // POST: Plantacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InsumoId,TipoPlanta,DataPlantio,DataColheita,Descricao,QuantidadePlantio,Status")] Plantacao plantacao)
        {
            // Verifica se o insumo existe e se a quantidade é suficiente
            var insumo = await _context.Insumo.FindAsync(plantacao.InsumoId);
            if (insumo == null)
            {
                ModelState.AddModelError("InsumoId", "O insumo selecionado não existe.");
            }
            else if (insumo.Quantidade < plantacao.QuantidadePlantio)
            {
                ModelState.AddModelError("QuantidadePlantio", "A quantidade de plantio não pode ser maior do que a quantidade disponível do insumo.");
            }

            if (ModelState.IsValid)
            {
                // Aqui você pode adicionar a lógica de redução da quantidade de insumos
                insumo.Quantidade -= plantacao.QuantidadePlantio;
                _context.Update(insumo);

                // Se o status for 'Concluído', adicione a quantidade ao estoque
                if (plantacao.Status == "Concluído")
                {
                    insumo.Quantidade += plantacao.QuantidadePlantio; // Adiciona a quantidade ao estoque
                    _context.Update(insumo);
                }

                _context.Add(plantacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se a validação falhar, recarrega os insumos
            ViewBag.Insumos = await _context.Insumo.ToListAsync();
            return View(plantacao);
        }

        // GET: Plantacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantacao = await _context.Plantacao.FindAsync(id);
            if (plantacao == null)
            {
                return NotFound();
            }

            ViewData["InsumoId"] = new SelectList(_context.Insumo, "InsumoId", "Nome", plantacao.InsumoId);
            return View(plantacao);
        }

        // POST: Plantacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlantacaoId,InsumoId,TipoPlanta,DataPlantio,DataColheita,Descricao,QuantidadePlantio,Status")] Plantacao plantacao)
        {
            if (id != plantacao.PlantacaoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verifique se o insumo existe
                var insumo = await _context.Insumo.FindAsync(plantacao.InsumoId);
                if (insumo == null)
                {
                    ModelState.AddModelError("InsumoId", "O insumo selecionado não existe.");
                    ViewData["InsumoId"] = new SelectList(_context.Insumo, "InsumoId", "Nome", plantacao.InsumoId);
                    return View(plantacao);
                }

                // Busca a plantação existente no banco de dados para verificar a quantidade anterior
                var plantacaoExistente = await _context.Plantacao.AsNoTracking().FirstOrDefaultAsync(p => p.PlantacaoId == id);
                if (plantacaoExistente == null)
                {
                    return NotFound();
                }

                // Permite alteração de quantidade apenas se o status não for 'Concluído'
                if (plantacaoExistente.Status != "Concluído")
                {
                    int diferencaQuantidade = plantacao.QuantidadePlantio - plantacaoExistente.QuantidadePlantio;

                    if (diferencaQuantidade > 0)
                    {
                        insumo.Quantidade -= diferencaQuantidade;
                    }
                    else if (diferencaQuantidade < 0)
                    {
                        insumo.Quantidade += Math.Abs(diferencaQuantidade);
                    }

                    _context.Update(insumo);
                }

                // Se o status for 'Concluído', adicione a quantidade ao estoque
                if (plantacao.Status == "Concluído")
                {
                    // Verificar se já existe um estoque para o produto
                    var estoqueItem = await _context.EstoqueProdutos
                        .FirstOrDefaultAsync(e => e.Produto.Nome == plantacao.TipoPlanta); // Verifique pelo nome do produto, que é o TipoPlanta

                    if (estoqueItem != null)
                    {
                        estoqueItem.QuantidadeEmEstoque += plantacao.QuantidadePlantio;
                        _context.Update(estoqueItem);
                    }
                    else
                    {
                        // Criar novo produto e estoque
                        var novoProduto = new Produtos
                        {
                            Nome = plantacao.TipoPlanta // Atribuir o TipoPlanta como nome do produto
                        };

                        var novoEstoque = new EstoqueProdutos
                        {
                            Produto = novoProduto,  // Associar o novo produto
                            QuantidadeEmEstoque = plantacao.QuantidadePlantio
                        };

                        // Adicionar ao contexto
                        _context.Add(novoProduto);
                        _context.Add(novoEstoque);
                    }
                }

                // Salva as alterações da plantação
                _context.Update(plantacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["InsumoId"] = new SelectList(_context.Insumo, "InsumoId", "Nome", plantacao.InsumoId);
            return View(plantacao);
        }


        // GET: Plantacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantacao = await _context.Plantacao
                .Include(p => p.Insumo)
                .FirstOrDefaultAsync(m => m.PlantacaoId == id);
            if (plantacao == null)
            {
                return NotFound();
            }

            return View(plantacao);
        }

        // POST: Plantacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plantacao = await _context.Plantacao.FindAsync(id);
            if (plantacao != null)
            {
                _context.Plantacao.Remove(plantacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlantacaoExists(int id)
        {
            return _context.Plantacao.Any(e => e.PlantacaoId == id);
        }
    }
}
