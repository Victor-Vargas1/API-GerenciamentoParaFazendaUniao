
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
    public class FornecedoresController : Controller
    {
        private readonly FazendaUniaoContext _context;

        public FornecedoresController(FazendaUniaoContext context)
        {
            _context = context;
        }

        // GET: Fornecedores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fornecedores.ToListAsync());
        }

        // GET: Fornecedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedores = await _context.Fornecedores
                .FirstOrDefaultAsync(m => m.FornecedorId == id);
            if (fornecedores == null)
            {
                return NotFound();
            }

            return View(fornecedores);
        }

        // GET: Fornecedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fornecedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FornecedorId,Nome,Cnpj,Endereco,Telefone,Email")] Fornecedores fornecedores)
        {
            if (ModelState.IsValid)
            {
                // Verifica se já existe um fornecedor com o mesmo CNPJ
                var fornecedorExistente = await _context.Fornecedores
                    .FirstOrDefaultAsync(f => f.Cnpj == fornecedores.Cnpj);

                if (fornecedorExistente != null)
                {
                    ModelState.AddModelError("Cnpj", "Já existe um fornecedor com este CNPJ.");
                    return View(fornecedores);
                }

                _context.Add(fornecedores);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(fornecedores);
        }

        // GET: Fornecedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedores = await _context.Fornecedores.FindAsync(id);
            if (fornecedores == null)
            {
                return NotFound();
            }
            return View(fornecedores);
        }

        // POST: Fornecedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FornecedorId,Nome,Cnpj,Endereco,Telefone,Email")] Fornecedores fornecedores)
        {
            if (id != fornecedores.FornecedorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fornecedores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FornecedoresExists(fornecedores.FornecedorId))
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
            return View(fornecedores);
        }

        // GET: Fornecedores/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Verifica se o fornecedor existe
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            // Retorna a visão de confirmação de exclusão
            return View(fornecedor);
        }

        // POST: Fornecedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            // Verifica se existem insumos relacionados ao fornecedor antes de excluir
            var insumosRelacionados = await _context.Insumo
                .Where(i => i.FornecedorId == id)
                .FirstOrDefaultAsync();

            // Se houver insumos relacionados, impede a exclusão e exibe uma mensagem
            if (insumosRelacionados != null)
            {
                TempData["ErroMensagem"] = "Não é possível excluir o fornecedor porque existem insumos relacionados a ele.";
                // Retorna para a mesma view de confirmação, permitindo que o usuário veja a mensagem de erro
                return View(fornecedor);
            }

            // Caso contrário, realiza a exclusão do fornecedor
            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // Redireciona para a página de listagem de fornecedores
        }

        private bool FornecedoresExists(int id)
        {
            return _context.Fornecedores.Any(e => e.FornecedorId == id);
        }
    }
}
