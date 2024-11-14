using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FazendaUniao.Data;
using FazendaUniao.Models;

namespace FazendaUniao.Controllers
{
    public class InsumoesController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public InsumoesController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: Insumoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Insumo.ToListAsync());
        }

        // GET: Insumoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumo
                .Include(i => i.Fornecedor)  // Carrega o fornecedor associado ao insumo
                .FirstOrDefaultAsync(m => m.InsumoId == id);

            if (insumo == null)
            {
                return NotFound();
            }

            return View(insumo);
        }

        // GET: Insumoes/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Fornecedores = await _context.Fornecedores.ToListAsync();
            return View();
        }

        // POST: Insumoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InsumoId,FornecedorId,Nome,Descricao,Quantidade")] Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                // Verifica se o insumo já existe com base no nome (ou outro critério único, como o código do insumo)
                var insumoExistente = await _context.Insumo
                    .FirstOrDefaultAsync(i => i.Nome == insumo.Nome);

                if (insumoExistente != null)
                {
                    // Se o insumo já existir, apenas adiciona a quantidade
                    insumoExistente.Quantidade += insumo.Quantidade;
                    _context.Update(insumoExistente); // Atualiza a quantidade do insumo existente
                }
                else
                {
                    // Se o insumo não existir, adiciona como novo
                    _context.Add(insumo);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarrega a lista de fornecedores caso a validação falhe
            ViewBag.Fornecedores = await _context.Fornecedores.ToListAsync();
            return View(insumo);
        }

        // GET: Insumoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Se o id for nulo, retorna NotFound
            }

            var insumo = await _context.Insumo.FindAsync(id); // Busca o insumo pelo id
            if (insumo == null)
            {
                return NotFound(); // Se o insumo não for encontrado, retorna NotFound
            }
            return View(insumo); // Retorna a view com o insumo para edição
        }

        // POST: Insumoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InsumoId,Nome,Descricao,Quantidade")] Insumo insumo)
        {
            if (id != insumo.InsumoId)
            {
                return NotFound(); // Verifica se o id corresponde ao insumo enviado
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insumo); // Marca o insumo para atualização no banco de dados
                    await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsumoExists((int)insumo.InsumoId))
                    {
                        return NotFound(); // Verifica se o insumo existe no banco
                    }
                    else
                    {
                        throw; // Caso contrário, lança a exceção
                    }
                }
                return RedirectToAction(nameof(Index)); // Redireciona para a lista de insumos após a atualização
            }
            return View(insumo); // Se o ModelState não for válido, retorna a view novamente com o insumo
        }

        // GET: Insumoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumo
                .FirstOrDefaultAsync(m => m.InsumoId == id);
            if (insumo == null)
            {
                return NotFound();
            }

            return View(insumo);
        }

        // POST: Insumoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insumo = await _context.Insumo.FindAsync(id);
            if (insumo == null)
            {
                return NotFound();
            }

            // Verifica se existem plantações associadas ao insumo
            var plantacoesAssociadas = await _context.Plantacao
                .Where(p => p.InsumoId == id)
                .ToListAsync();

            if (plantacoesAssociadas.Any())
            {
                // Exibe uma mensagem de erro na view de exclusão
                TempData["ErrorMessage"] = "Não é possível excluir este insumo porque ele está associado a uma ou mais plantações.";
                return RedirectToAction("Delete", new { id }); // Redireciona para a view de exclusão com o erro
            }

            // Se não houver plantações associadas, prossegue com a exclusão
            _context.Insumo.Remove(insumo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool InsumoExists(int id)
        {
            return _context.Insumo.Any(e => e.InsumoId == id);
        }
    }
}
