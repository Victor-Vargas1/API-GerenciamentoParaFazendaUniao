using System.ComponentModel.DataAnnotations;

namespace FazendaUniao.Models
{
    public class Insumo
    {
        [Key]
        public int? InsumoId { get; set; }

        [Required]
        public int? FornecedorId { get; set; }
        public Fornecedores? Fornecedor { get; set; }

        [StringLength(100, ErrorMessage = "Nome deve conter até 100 caracteres.")]
        [Display(Name = "Nome Insumo")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        public string? Nome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(200, ErrorMessage = "A descrição deve conter até 200 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "A descrição deve conter apenas letras e espaços.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A quantidade do insumo é obrigatória.")]
        [Range(1, 999999, ErrorMessage = "A quantidade deve ser positiva.")]
        [Display(Name = "Quantidade(g)")]
        public int? Quantidade { get; set; }
    }
}
