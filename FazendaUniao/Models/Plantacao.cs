using System.ComponentModel.DataAnnotations;

namespace FazendaUniao.Models
{
    public class Plantacao
    {

        [Key]
        public int PlantacaoId { get; set; }

        [Required]
        public int InsumoId { get; set; }
        public required Insumo? Insumo { get; set; }

        [Required(ErrorMessage = "O tipo de planta é obrigatório.")]
        [StringLength(100, ErrorMessage = "O tipo de planta deve conter até 100 caracteres.")]
        [Display(Name = "Plantação de")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Deve conter apenas letras e espaços.")]
        public string? TipoPlanta { get; set; }

        [Required(ErrorMessage = "A data de plantio é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Plantio")]
        public DateTime DataPlantio { get; set; }

        [Required(ErrorMessage = "A data de colheita é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Colheita")]
        [CustomValidation(typeof(Plantacao), nameof(ValidarDataColheita))]
        public DateTime DataColheita { get; set; }

        [StringLength(250, ErrorMessage = "A descrição deve conter até 250 caracteres.")]
        [Display(Name = "Descrição")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Deve conter apenas letras e espaços.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A quantidade de plantio é obrigatória.")]
        [Range(1, 999999, ErrorMessage = "A quantidade de plantio deve ser positiva.")]
        [Display(Name = "Quantidade de Plantio")]
        public int QuantidadePlantio { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string? Status { get; set; } // "Pendente" ou "Concluído"

        
        public static ValidationResult ValidarDataColheita(DateTime dataColheita, ValidationContext context)
        {
            if (dataColheita <= DateTime.Today)
            {
                return new ValidationResult("A data da colheita deve ser superior a Plantação");
            }
            return ValidationResult.Success;
        }

    }
}
