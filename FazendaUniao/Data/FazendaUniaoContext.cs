using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FazendaUniao.Models;

namespace FazendaUniao.Data
{
    public class FazendaUniaoContext : DbContext
    {
        public FazendaUniaoContext (DbContextOptions<FazendaUniaoContext> options)
            : base(options)
        {

        }

        public DbSet<FazendaUniao.Models.Cliente> Cliente { get; set; } = default!;
        public DbSet<FazendaUniao.Models.Fornecedores> Fornecedores { get; set; } = default!;
        public DbSet<FazendaUniao.Models.Insumo> Insumo { get; set; } = default!;
        public DbSet<FazendaUniao.Models.EstoqueInsumos> EstoqueInsumos { get; set; } = default!;
        public DbSet<FazendaUniao.Models.Produtos> Produtos { get; set; } = default!;
        public DbSet<FazendaUniao.Models.EstoqueProdutos> EstoqueProdutos { get; set; } = default!;
        public DbSet<FazendaUniao.Models.Pedido> Pedido { get; set; } = default!;
        public DbSet<FazendaUniao.Models.Plantacao> Plantacao { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plantacao>()
                .HasOne(p => p.Insumo)
                .WithMany()
                .HasForeignKey(p => p.InsumoId)
                .OnDelete(DeleteBehavior.SetNull);
        }


    }
}
