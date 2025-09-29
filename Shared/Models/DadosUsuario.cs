using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class DadosUsuario
    {
        public string? Nome {  get; set; }
        public string? CPF { get; set; }
        public int Saldo { get; set; }
        public int AgenciaId {  get; set; }
        public List<TransacaoDTO> Transacao { get; set; }
    }
}
