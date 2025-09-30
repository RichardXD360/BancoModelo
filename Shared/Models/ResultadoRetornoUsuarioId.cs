using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ResultadoRetornoUsuarioId
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public int StatusCode { get; set; }
        public int UsuarioId { get; set; }
    }
}
