using System.ComponentModel.DataAnnotations;

namespace FazendaUniao.Models
{
    public class Produtos
    {
        [Key]
        public int ProdutoId { get; set; }

        [StringLength(100, ErrorMessage = "Nome deve conter até 100 caracteres.")]
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        [Display(Name = "Nome do Produto")]
        public string? Nome { get; set; }

        [StringLength(50, ErrorMessage = "Categoria deve conter até 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s\-]+$", ErrorMessage = "A categoria deve conter apenas letras, espaços e hífen.")]
        public string? Categoria { get; set; }

        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [Display(Name = "Preço Unitario")]
        [Range(0.01, 999999.99, ErrorMessage = "O preço deve ser positivo e menor que 1 milhão.")]
        [DataType(DataType.Currency, ErrorMessage = "O formato de moeda é inválido.")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "O preço deve ter no máximo duas casas decimais.")]
        public double? PrecoUnitario { get; set; } = 0;
        public Plantacao? Plantacao { get; set; }
    }
}
