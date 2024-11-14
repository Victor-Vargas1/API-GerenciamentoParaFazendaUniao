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
using iText.Layout;
using iText.Layout.Element;

namespace FazendaUniao.Controllers
{
    public class PedidoesController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public PedidoesController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            var pedidos = _context.Pedido
                .Include(p => p.Cliente) // coloca o CLIENTE e o PRODUTO no INDEX 
                .Include(p => p.Produto)
                .ToListAsync();

            return View(await pedidos);
        }

        // AQUI COMEÇA O METODO DE GERAR RELATORIO
        public async Task<IActionResult> GeneratePdf()
        {
            var pedidos = await _context.Pedido
                .Include(p => p.Cliente) // inclui o Cliente e o Produto
                .Include(p => p.Produto)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                // CRIA O PDF
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        // TITULO DO RELATÓRIO
                        document.Add(new Paragraph("Relatório de Pedidos")
                            .SetFontSize(20)
                            .SetBold());

                        // TABELA DE PEDIDO
                        var table = new Table(6); // Agora com 6 colunas: ID, Nome Cliente, Produto, Quantidade, Valor Total, Data do Pedido

                        // CABEÇALHO DA TABELA
                        table.AddHeaderCell("ID do Pedido");
                        table.AddHeaderCell("Nome do Cliente");
                        table.AddHeaderCell("Produto");
                        table.AddHeaderCell("Quantidade");
                        table.AddHeaderCell("Valor Total");
                        table.AddHeaderCell("Data do Pedido");

                        // PREENCHE COM OS DADOS EXISTENTES
                        foreach (var pedido in pedidos)
                        {
                            table.AddCell(pedido.PedidoId.ToString());
                            table.AddCell(pedido.Cliente?.NomeCliente ?? "N/A");
                            table.AddCell(pedido.Produto?.Nome ?? "N/A");
                            table.AddCell(pedido.Quantidade?.ToString() ?? "0");
                            table.AddCell($"R$ {pedido.ValorTotal?.ToString("N2") ?? "0.00"}");

                            // Adicionando a data do pedido formatada
                            var dataPedido = pedido.DataPedido?.ToString("dd/MM/yyyy") ?? "N/A";
                            table.AddCell(dataPedido);
                        }

                        document.Add(table);
                    }
                }

                var fileName = "Relatorio_Pedidos.pdf";
                return File(memoryStream.ToArray(), "application/pdf", fileName);
            }
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Alteração: Incluindo Cliente e Produto no Pedido
            var pedido = await _context.Pedido
                .Include(p => p.Cliente) // Inclui o Cliente relacionado ao Pedido
                .Include(p => p.Produto) // Inclui o Produto relacionado ao Pedido
                .FirstOrDefaultAsync(m => m.PedidoId == id); // Buscar Pedido pelo ID

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido); // Passa o Pedido com Cliente e Produto para a View
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "NomeCliente");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos.Where(p => p.PrecoUnitario > 0), "ProdutoId", "Nome");
            return View();
        }

        // POST: Pedidoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PedidoId,ProdutoId,ClienteId,DataPedido,Quantidade,ValorTotal,Status")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                // Verificar se o produto existe no estoque
                var estoqueProduto = await _context.EstoqueProdutos
                    .Include(e => e.Produto)
                    .FirstOrDefaultAsync(e => e.Produto.ProdutoId == pedido.ProdutoId);

                if (estoqueProduto == null)
                {
                    ModelState.AddModelError("ProdutoId", "Este produto não está disponível em estoque.");
                }
                else if (pedido.Quantidade > estoqueProduto.QuantidadeEmEstoque)
                {
                    ModelState.AddModelError("Quantidade",
                        $"Quantidade solicitada: {pedido.Quantidade}. Estoque disponível: {estoqueProduto.QuantidadeEmEstoque}.");
                }
                else
                {
                    // Calcular o valor total sem alterar o estoque
                    pedido.ValorTotal = estoqueProduto.Produto.PrecoUnitario * pedido.Quantidade;

                    estoqueProduto.QuantidadeEmEstoque -= pedido.Quantidade;

                    // Adicionar o pedido ao contexto
                    _context.Add(pedido);

                    // Salvar as mudanças no banco de dados
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "NomeCliente", pedido.ClienteId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", pedido.ProdutoId);

            return View(pedido);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido.Include(p => p.Produto).Include(p => p.Cliente).FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Carregar listas de clientes e produtos para a view
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "Nome", pedido.ClienteId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", pedido.ProdutoId);

            return View(pedido);
        }

        // POST: Pedidoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,ProdutoId,ClienteId,DataPedido,Quantidade,ValorTotal,Status")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Obter o pedido original do banco de dados
                var pedidoOriginal = await _context.Pedido
                    .Include(p => p.Produto)
                    .Include(p => p.Cliente)
                    .Include(p => p.Estoque)
                    .FirstOrDefaultAsync(p => p.PedidoId == id);

                if (pedidoOriginal == null)
                {
                    return NotFound();
                }

                // Obter o produto no estoque correspondente ao pedido
                var estoqueProduto = await _context.EstoqueProdutos
                    .Include(e => e.Produto)
                    .FirstOrDefaultAsync(e => e.Produto.ProdutoId == pedido.ProdutoId);

                if (estoqueProduto == null)
                {
                    ModelState.AddModelError("ProdutoId", "Este produto não está disponível em estoque.");
                }
                else
                {
                    // Calcular o valor total do pedido
                    pedido.ValorTotal = estoqueProduto.Produto.PrecoUnitario * pedido.Quantidade;

                    // Atualizar a quantidade em estoque somente se o status for "Concluído"
                    if (pedido.Status == "Concluído")
                    {
                        // Restaurar a quantidade original antes de aplicar a nova quantidade
                        estoqueProduto.QuantidadeEmEstoque += pedidoOriginal.Quantidade ?? 0;

                        // Verificar se a nova quantidade solicitada é válida
                        if (pedido.Quantidade > estoqueProduto.QuantidadeEmEstoque)
                        {
                            ModelState.AddModelError("Quantidade",
                                $"Quantidade solicitada: {pedido.Quantidade}. Estoque disponível: {estoqueProduto.QuantidadeEmEstoque}.");
                            return View(pedido);
                        }

                        // Reduzir a quantidade em estoque pela nova quantidade
                        estoqueProduto.QuantidadeEmEstoque -= pedido.Quantidade ?? 0;
                    }

                    // Atualizar os dados do pedido
                    _context.Entry(pedidoOriginal).CurrentValues.SetValues(pedido);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index"); // Redirecionar para a página desejada
                }
            }

            // Recarregar listas para exibir a view de edição com dados válidos
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "NomeCliente", pedido.ClienteId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", pedido.ProdutoId);

            return View(pedido);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Produto) // Incluir produto
                .Include(p => p.Cliente) // Incluir cliente
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedido.Any(e => e.PedidoId == id);
        }


    }
}
