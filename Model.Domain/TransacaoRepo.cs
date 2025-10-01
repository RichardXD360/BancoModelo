
using Application;
using Shared.Models;

namespace Model.Domain
{
    public class TransacaoRepo
    {
        private readonly IData _data;
        public TransacaoRepo(IData data)
        {
            _data = data;
        }
        public string GetUsuarioNome(int usuarioId)
        {
            return _data.RetornarNomeUsuario(usuarioId);
        }
        public string GetUsuarioCpf(int usuarioId)
        {
            return _data.RetornarCpfUsuario(usuarioId);
        }
    }
}
