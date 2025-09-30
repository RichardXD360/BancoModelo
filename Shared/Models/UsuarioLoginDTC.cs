using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class UsuarioLoginDTC
    {
        [Required]
        public string? Login { get; set; }

        [Required]
        [RegularExpression("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$")]
        public string? Cpf { get; set; }

    }
}
