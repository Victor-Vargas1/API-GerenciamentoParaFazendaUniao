using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FazendaUniao.Models
{
    public class EstoqueInsumos
    {
        [Key]
        public int EstoqueInsumosId { get; set; }

        [Required(ErrorMessage = "O ID do insumo é obrigatório.")]
        [DisplayName("Insumo")]
        public Insumo? Insumo { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        [DisplayName("Quantidade em Estoque")]
        [Range(0, 999999, ErrorMessage = "A quantidade em estoque deve ser um valor positivo.")]
        public int? QuantidadeEmEstoque { get; set; }

    }
}
