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
    public class EstoqueInsumosController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public EstoqueInsumosController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: EstoqueInsumos
        public async Task<IActionResult> Index()
        {
            var estoqueInsumos = await _context.EstoqueInsumos.Include(e => e.Insumo).ToListAsync();
            return View(estoqueInsumos);
        }

        // GET: EstoqueInsumos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueInsumos = await _context.EstoqueInsumos
                .FirstOrDefaultAsync(m => m.EstoqueInsumosId == id);
            if (estoqueInsumos == null)
            {
                return NotFound();
            }

            return View(estoqueInsumos);
        }

        // GET: EstoqueInsumos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Insumos = await _context.Insumo.ToListAsync();
            return View();
        }

        // POST: EstoqueInsumos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int insumoId, int quantidade)
        {
            if (ModelState.IsValid)
            {
                // Encontra o insumo correspondente no banco de dados
                var insumo = await _context.Insumo.FindAsync(insumoId);
                if (insumo == null)
                {
                    ModelState.AddModelError("", "Insumo não encontrado.");
                    return View();
                }

                // Verifica se já existe um registro para o insumo no estoque
                var estoqueItem = await _context.EstoqueInsumos.FirstOrDefaultAsync(e => e.Insumo.InsumoId == insumoId);

                if (estoqueItem != null)
                {
                    // Se já existe, apenas atualiza a quantidade
                    estoqueItem.QuantidadeEmEstoque += quantidade;
                    _context.Update(estoqueItem);
                }
                else
                {
                    // Se não existe, cria um novo registro no estoque
                    estoqueItem = new EstoqueInsumos
                    {
                        Insumo = insumo,
                        QuantidadeEmEstoque = quantidade
                    };
                    _context.Add(estoqueItem);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo não for válido, retornar a view com o erro
            return View();
        }

        // GET: EstoqueInsumos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueInsumos = await _context.EstoqueInsumos.FindAsync(id);
            if (estoqueInsumos == null)
            {
                return NotFound();
            }
            return View(estoqueInsumos);
        }

        // POST: EstoqueInsumos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EstoqueInsumosId,QuantidadeEmEstoque")] EstoqueInsumos estoqueInsumos)
        {
            if (id != estoqueInsumos.EstoqueInsumosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estoqueInsumos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstoqueInsumosExists(estoqueInsumos.EstoqueInsumosId))
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
            return View(estoqueInsumos);
        }

        // GET: EstoqueInsumos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estoqueInsumos = await _context.EstoqueInsumos
                .FirstOrDefaultAsync(m => m.EstoqueInsumosId == id);
            if (estoqueInsumos == null)
            {
                return NotFound();
            }

            return View(estoqueInsumos);
        }

        // POST: EstoqueInsumos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estoqueInsumos = await _context.EstoqueInsumos.FindAsync(id);
            if (estoqueInsumos != null)
            {
                _context.EstoqueInsumos.Remove(estoqueInsumos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstoqueInsumosExists(int id)
        {
            return _context.EstoqueInsumos.Any(e => e.EstoqueInsumosId == id);
        }
    }
}
