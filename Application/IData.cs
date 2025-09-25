using Shared.Models;

namespace Application
{
    public interface IData
    {
        public ResultadoRetorno VerificarUsuario(UsuarioDTC usuario);
        public ResultadoRetorno CriarUsuario(UsuarioDTC usuario);
    }
}
