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
    public class ProdutosController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public ProdutosController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Plantacao) // Inclui a informação da Plantacao
                .ToListAsync();

            return View(produtos);
        }

        public async Task<IActionResult> GeneratePdfProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                // CRIA O PDF
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        // TITULO DO RELATÓRIO
                        document.Add(new Paragraph("Relatório de Produtos")
                            .SetFontSize(20)
                            .SetBold());

                        // TABELA DE PRODUTOS
                        var table = new Table(4); // Agora com 4 colunas: ID, Nome, Categoria, Preço Unitário

                        // CABEÇALHO DA TABELA
                        table.AddHeaderCell("ID do Produto");
                        table.AddHeaderCell("Nome do Produto");
                        table.AddHeaderCell("Categoria");
                        table.AddHeaderCell("Preço Unitário");

                        // PREENCHE COM OS DADOS EXISTENTES
                        foreach (var produto in produtos)
                        {
                            table.AddCell(produto.ProdutoId.ToString());
                            table.AddCell(produto.Nome ?? "N/A");
                            table.AddCell(produto.Categoria ?? "N/A");
                            table.AddCell($"R$ {produto.PrecoUnitario?.ToString("N2") ?? "0.00"}");
                        }

                        document.Add(table);
                    }
                }

                var fileName = "Relatorio_Produtos.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtos = await _context.Produtos
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produtos == null)
            {
                return NotFound();
            }

            return View(produtos);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProdutoId,Nome,Categoria,PrecoUnitario")] Produtos produtos)
        {
            if (ModelState.IsValid)
            {
                // Verificar se o produto já existe pelo nome
                var produtoExistente = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.Nome == produtos.Nome);

                if (produtoExistente != null)
                {
                    // Adicionar um erro de validação no ModelState
                    ModelState.AddModelError("Nome", "Já existe um produto com este nome.");
                    return View(produtos); // Retornar a view com o erro
                }

                // Se o produto não existir, adicionar ao banco de dados
                _context.Add(produtos);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); // Redirecionar para a página de index
            }
            return View(produtos);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtos = await _context.Produtos.FindAsync(id);
            if (produtos == null)
            {
                return NotFound();
            }
            return View(produtos);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProdutoId,Nome,Categoria,PrecoUnitario")] Produtos produtos)
        {
            if (id != produtos.ProdutoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutosExists(produtos.ProdutoId))
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
            return View(produtos);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Verifica se há um estoque associado a este produto
                var estoqueProduto = await _context.EstoqueProdutos
                    .FirstOrDefaultAsync(e => e.Produto.ProdutoId == produto.ProdutoId);

                if (estoqueProduto != null)
                {
                    // Remove o estoque associado
                    _context.EstoqueProdutos.Remove(estoqueProduto);
                }

                // Remove o produto
                _context.Produtos.Remove(produto);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutosExists(int id)
        {
            return _context.Produtos.Any(e => e.ProdutoId == id);
        }
    }
}
