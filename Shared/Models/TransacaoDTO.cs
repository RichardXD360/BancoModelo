using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{

    public class TransacaoDTO
    {
        public string? Descricao { get; set; }
        [Required]
        public decimal? Valor { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        public string UsuarioRecebedorCnpj { get; set; }
        public int UsuarioRecebedorId { get; set; }
        [Required]
        public EnumTipoTransacao TipoTransacao { get; set; }
        [Required]
        public DateTime DataTransacao { get; set; }
    }
}
