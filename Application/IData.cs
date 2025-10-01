using Shared.Models;

namespace Application
{
    public interface IData
    {
        public ResultadoRetornoUsuarioId VerificarUsuario(UsuarioLoginDTC usuario);
        public ResultadoRetorno VerificarUsuarioId(int usuarioid);
        public DadosUsuario DetalhesUsuario(int usuarioId);
        public string RetornarNomeUsuario(int usuarioId);
        public string RetornarCpfUsuario(int usuarioId);
        public TransacaoDTO RetornarTransacao(int transacaoId);
        public ResultadoRetorno CriarUsuario(UsuarioDTC usuario);
        public ResultadoRetorno EfetuarTransacao(TransacaoDTO transacao, int usuarioRecebedorId);
        public int VerificarUsuarioCnpj(string usuarioCpf);
        public int VerificarSaldo(int usuarioId);
        public void AbrirConexao(); 
    }
}
