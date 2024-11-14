using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FazendaUniao.Models
{
    public class EstoqueProdutos 
    {
        [Key]
        public int EstoqueProdutosId { get; set; }

        public Produtos? Produto { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        [DisplayName("Quantidade em Estoque")]
        [Range(0, 999999, ErrorMessage = "A quantidade em estoque deve ser um valor positivo.")]
        public int? QuantidadeEmEstoque { get; set; }

        public string? Nome { get; set; }
    }
}
