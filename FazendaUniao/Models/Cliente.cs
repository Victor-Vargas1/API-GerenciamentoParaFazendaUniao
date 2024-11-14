using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FazendaUniao.Models
{
    public class Cliente
    {

        [Key]
        public int ClienteId { get; set; }

        [StringLength(100, ErrorMessage = "Nome deve conter até 100 caracteres.")]
        [DisplayName("Nome")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "O nome deve conter apenas letras e espaços.")]
        [Display(Name = "Nome do Cliente")]

        public string? NomeCliente { get; set; }

        [StringLength(18, ErrorMessage = "O CNPJ deve conter 18 caracteres.")]
        [Display(Name = "CNPJ")]
        [RegularExpression(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}", ErrorMessage = "O CNPJ deve estar no formato 00.000.000/0000-00")]
        public string? CnpjCliente { get; set; }

        [Display(Name = "Endereço")]
        public string? EnderecoCliente { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Display(Name = "Telefone")]
        [StringLength(15, ErrorMessage = "O telefone deve conter até 15 caracteres.")]
        [RegularExpression(@"\(\d{2}\)\s\d{4,5}-\d{4}", ErrorMessage = "O telefone deve estar no formato (00) 0000-0000 ou (00) 00000-0000")]
        public string? TelefoneCliente { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de email inválido.")]
        public string? EmailCliente { get; set; }
    }
}
