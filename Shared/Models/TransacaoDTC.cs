using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class TransacaoDTC
    {
        public int UsuarioId { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataTransacao { get; set; }
        public decimal Valor {  get; set; }
        public EnumTipoTransacao TipoTransacao { get; set; }

    }
}
