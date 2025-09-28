using Shared.Models;

namespace Application
{
    public interface IData
    {
        public ResultadoRetorno VerificarUsuario(UsuarioDTC usuario);
        public ResultadoRetorno VerificarUsuarioId(int usuarioid);
        public ResultadoRetorno CriarUsuario(UsuarioDTC usuario);
        public ResultadoRetorno EfetuarTransacao(TransacaoDTO transacao);
        public int VerificarSaldo(int usuarioId);
        public void AbrirConexao(); 
    }
}
