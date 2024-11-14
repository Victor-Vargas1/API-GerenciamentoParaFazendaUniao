using System.ComponentModel.DataAnnotations;

namespace FazendaUniao.Models
{
    public class Fornecedores
    {
        [Key]
        public int FornecedorId { get; set; }


        [StringLength(90, ErrorMessage = "Nome deve conter até 90 caracteres.")]
        [Required(ErrorMessage = "O nome do fornecedor é obrigatório.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        [Display(Name = "Nome Fornecedor")]
        public string? Nome { get; set; }

        [StringLength(18, ErrorMessage = "CNPJ deve conter 18 caracteres.")]
        [Required(ErrorMessage = "O CNPJ do fornecedor é obrigatório.")]
        public string? Cnpj { get; set; }

        [StringLength(50, ErrorMessage = "Endereço deve conter 50 caracteres.")]
        [Required(ErrorMessage = "O endereço do fornecedor é obrigatório.")]
        [Display(Name = "Endereço")]
        public string? Endereco { get; set; }

        [StringLength(15, ErrorMessage = "Telefone deve conter até 15 caracteres.")]
        [Required(ErrorMessage = "O telefone do fornecedor é obrigatório.")]
        [RegularExpression(@"\(\d{2}\)\s\d{4,5}-\d{4}", ErrorMessage = "O telefone deve estar no formato (00) 0000-0000 ou (00) 00000-0000")]
        public string? Telefone { get; set; }

        [StringLength(50, ErrorMessage = "Email deve conter até 50 caracteres.")]
        [Required(ErrorMessage = "O email do fornecedor é obrigatório.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de email inválido.")]
        public string? Email { get; set; }
    }
}
