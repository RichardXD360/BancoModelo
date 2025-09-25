using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class UsuarioDTO
    {
        [Required]
        public int Id{ get; set; }

        [Required]
        [RegularExpression("Az-aZ")]
        public string? Nome { get; set; }

        [Required]
        public string? Login { get; set; }

        [Required]
        [RegularExpression("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$")]
        public string? Cpf { get; set; }

        [Required]
        public int AgenciaId { get; set; }
    }
}
