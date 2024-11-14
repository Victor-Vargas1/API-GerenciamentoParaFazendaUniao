using FazendaUniao.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FazendaUniao.Models
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        [Required]
        public int? ProdutoId { get; set; } // Referência ao ID do produto
        public required Produtos? Produto { get; set; }

        [Required]
        public int? ClienteId { get; set; } // Referência ao ID do cliente
        public required Cliente? Cliente { get; set; }

        public EstoqueProdutos? Estoque { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DataPedido { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "O valor total deve ser positivo e menor que 1 milhão.")]
        public double? ValorTotal { get; set; }

        public string? Status { get; set; } = "Pendente";

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int? Quantidade { get; set; }

       

    }
}
