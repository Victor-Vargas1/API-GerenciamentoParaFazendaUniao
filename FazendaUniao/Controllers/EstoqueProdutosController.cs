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
    public class EstoqueProdutosController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public EstoqueProdutosController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: EstoqueProdutos
        public async Task<IActionResult> Index()
        {
            var estoqueProdutos = await _context.EstoqueProdutos.Include(e => e.Produto).ToListAsync();
            return View(estoqueProdutos);
        }

        public async Task<IActionResult> GeneratePdfEstoque()
        {
            var estoqueProdutos = await _context.EstoqueProdutos.Include(e => e.Produto).ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                // CRIA O PDF
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        // TITULO DO RELATÓRIO
                        document.Add(new Paragraph("Relatório de Estoque de Produtos")
                            .SetFontSize(20)
                            .SetBold());

                        // TABELA DE ESTOQUE
                        var table = new Table(4); // Agora com 4 colunas: ID do Estoque, Nome do Produto, Quantidade em Estoque e Preço Unitário

                        // CABEÇALHO DA TABELA
                        table.AddHeaderCell("ID do Estoque");
                        table.AddHeaderCell("Nome do Produto");
                        table.AddHeaderCell("Quantidade em Estoque");
                        table.AddHeaderCell("Preço Unitário");

                        // PREENCHE COM OS DADOS EXISTENTES
                        foreach (var estoqueItem in estoqueProdutos)
                        {
                            table.AddCell(estoqueItem.EstoqueProdutosId.ToString());
                            table.AddCell(estoqueItem.Produto.Nome ?? "N/A");
                            table.AddCell(estoqueItem.QuantidadeEmEstoque.ToString());
                            table.AddCell($"R$ {estoqueItem.Produto.PrecoUnitario?.ToString("N2") ?? "0.00"}");
                        }

                        document.Add(table);
                    }
                }

                var fileName = "Relatorio_Estoque_Produtos.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        // GET: EstoqueProdutos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueProdutos = await _context.EstoqueProdutos
                .FirstOrDefaultAsync(m => m.EstoqueProdutosId == id);
            if (estoqueProdutos == null)
            {
                return NotFound();
            }

            return View(estoqueProdutos);
        }

        // GET: EstoqueProdutos/Create
        public async Task<IActionResult> Create()
        {
            // Carregar a lista de produtos para o dropdown
            ViewBag.Produtos = await _context.Produtos.ToListAsync();
            return View();
        }

        // POST: EstoqueProdutos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int produtoId, int quantidadeEmEstoque)
        {
            if (ModelState.IsValid)
            {
                // Verificar se o produto existe
                var produto = await _context.Produtos.FindAsync(produtoId);
                if (produto == null)
                {
                    ModelState.AddModelError("", "Produto não encontrado.");
                    return View();
                }

                // Verificar se já existe um registro para o produto no estoque
                var estoqueItem = await _context.EstoqueProdutos.FirstOrDefaultAsync(e => e.Produto.ProdutoId == produtoId);

                if (estoqueItem != null)
                {
                    // Se já existe, apenas atualiza a quantidade
                    estoqueItem.QuantidadeEmEstoque += quantidadeEmEstoque;
                    _context.Update(estoqueItem);
                }
                else
                {
                    // Se não existe, cria um novo registro no estoque
                    estoqueItem = new EstoqueProdutos
                    {
                        Produto = produto,
                        QuantidadeEmEstoque = quantidadeEmEstoque
                    };
                    _context.Add(estoqueItem);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirecionar para a lista de estoque (você pode criar este método se desejar)
            }

            // Se o modelo não for válido, retornar a view com os dados necessários
            ViewBag.Produtos = await _context.Produtos.ToListAsync();
            return View();
        }

        // GET: EstoqueProdutos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueProdutos = await _context.EstoqueProdutos
                .Include(e => e.Produto) // Inclui o produto associado
                .FirstOrDefaultAsync(e => e.EstoqueProdutosId == id);

            if (estoqueProdutos == null)
            {
                return NotFound();
            }

            return View(estoqueProdutos);
        }

        // POST: EstoqueProdutos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstoqueProdutosId,QuantidadeEmEstoque")] EstoqueProdutos estoqueProdutos)
        {
            if (id != estoqueProdutos.EstoqueProdutosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estoqueProdutos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstoqueProdutosExists(estoqueProdutos.EstoqueProdutosId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(estoqueProdutos);
        }

        // GET: EstoqueProdutos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueProdutos = await _context.EstoqueProdutos
            .Include(e => e.Produto) // Inclui o produto associado
            .FirstOrDefaultAsync(m => m.EstoqueProdutosId == id);
            if (estoqueProdutos == null)
            {
                return NotFound();
            }

            return View(estoqueProdutos);
        }

        // POST: EstoqueProdutos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estoqueProdutos = await _context.EstoqueProdutos.FindAsync(id);
            if (estoqueProdutos != null)
            {
                _context.EstoqueProdutos.Remove(estoqueProdutos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstoqueProdutosExists(int id)
        {
            return _context.EstoqueProdutos.Any(e => e.EstoqueProdutosId == id);
        }
    }
}
