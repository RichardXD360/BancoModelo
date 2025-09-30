using Shared.Models;
using Application;

namespace Service
{
    public class BancoVerify
    {
        private readonly IData _data;
        public BancoVerify(IData data)
        {
            _data = data;
        }

        ResultadoRetorno resultadoRetorno = new ResultadoRetorno();

        public void AbrirConexao()
        {
            _data.AbrirConexao();
        }
        private ResultadoRetornoHTTP CriarResultadoRetornoHTTP(string mensagem, bool sucesso, int statusCode)
        {
            return new ResultadoRetornoHTTP
            {
                Mensagem = mensagem,
                Sucesso = sucesso,
                StatusCode = statusCode
            };
        }
        private ResultadoRetornoUsuarioId CriarResultadoRetornoUsuarioId(string mensagem, bool sucesso, int statusCode, int usuarioId)
        {
            return new ResultadoRetornoUsuarioId
            {
                Mensagem = mensagem,
                Sucesso = sucesso,
                StatusCode = statusCode,
                UsuarioId = usuarioId
            };
        }
        public ResultadoRetornoHTTP EfetuarTransacao(TransacaoDTO transacao)
        {
            if (transacao == null || transacao.Valor == 0 || transacao.DataTransacao == null)
            {
                return CriarResultadoRetornoHTTP("Transação inválida ou nula, verifique os dados enviados", false, 400);
            };
            if (transacao.TipoTransacao > EnumTipoTransacao.Tranferencia)
            {
                return CriarResultadoRetornoHTTP("Tipo Transação inválida ou inexistente, verifique os dados enviados", false, 404);
            };
            ResultadoRetorno validarUsuario = _data.VerificarUsuarioId(transacao.UsuarioId);
            if (validarUsuario.Sucesso == false)
            {
                return CriarResultadoRetornoHTTP("Usuario não encontrado", false, 404);
            };
            int validarUsuarioRecebedorCnpj = _data.VerificarUsuarioCnpj(transacao.UsuarioRecebedorCnpj);

            if(validarUsuarioRecebedorCnpj == transacao.UsuarioId)
            {
                return CriarResultadoRetornoHTTP("Usuario recebedor não pode ser você.", false, 400);
            };

            if(validarUsuarioRecebedorCnpj == 0)
            {
                return CriarResultadoRetornoHTTP("Usuario recebedor não encontrado.", false, 404);
            };
            ResultadoRetorno validarUsuarioRecebedor = _data.VerificarUsuarioId(validarUsuarioRecebedorCnpj);
            if (validarUsuarioRecebedor.Sucesso == false)
            {
                return CriarResultadoRetornoHTTP("Usuario recebedor não encontrado", false, 400);
            };
            int saldoUsuario = _data.VerificarSaldo(transacao.UsuarioId);
            if (saldoUsuario < transacao.Valor)
            {
                return CriarResultadoRetornoHTTP("Saldo insuficiente.", false, 409);
            };
            ResultadoRetorno retorno = _data.EfetuarTransacao(transacao, validarUsuarioRecebedorCnpj);
            return CriarResultadoRetornoHTTP(retorno.Mensagem, retorno.Sucesso, 200);
        }
        public ResultadoRetornoUsuarioId ValidarUsuario(UsuarioLoginDTC usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Login))
            {
                return CriarResultadoRetornoUsuarioId("Usuario invalido ou vazio", false, 401, 0);
            };
            ResultadoRetornoUsuarioId resultadoRetorno = _data.VerificarUsuario(usuario);
            if (!resultadoRetorno.Sucesso)
            {
                return CriarResultadoRetornoUsuarioId(resultadoRetorno.Mensagem, resultadoRetorno.Sucesso, 404, 0);
            };
            return CriarResultadoRetornoUsuarioId(resultadoRetorno.Mensagem, resultadoRetorno.Sucesso, 200, resultadoRetorno.UsuarioId);
        }
        public ResultadoRetornoUsuarioId CriarUsuario(UsuarioDTC usuario)
        {
            ResultadoRetornoUsuarioId validarUsuarioExistente = new ResultadoRetornoUsuarioId();
            UsuarioLoginDTC usuarioLogin = new UsuarioLoginDTC();
            usuarioLogin.Login = usuario.Login;
            usuarioLogin.Cpf = usuario.Cpf;
            validarUsuarioExistente = ValidarUsuario(usuarioLogin);
            if (validarUsuarioExistente.Sucesso)
            {
                return CriarResultadoRetornoUsuarioId("Login de usuário já existente", false, 409, 0);
            };
            if (usuario.Nome.Length <= 7 || usuario.Nome.Length >= 33)
            {
                return CriarResultadoRetornoUsuarioId("Nome de usuário inválido. O nome deve conter entre 8 a 32 caracteres.", false, 401, 0);
            };
            resultadoRetorno = _data.CriarUsuario(usuario);
            return CriarResultadoRetornoUsuarioId(resultadoRetorno.Mensagem, resultadoRetorno.Sucesso, 201, 0);
        }
        public DadosUsuario DetalhesUsuario(int usuarioId)
        {
            var validarUsuario = _data.VerificarUsuarioId(usuarioId);
            if (validarUsuario.Sucesso == false)
            {
                return new DadosUsuario
                {
                    Nome = "Usuário inexistente na base de dados",
                    Saldo = 404
                };
            };
            DadosUsuario retorno = _data.DetalhesUsuario(usuarioId);
            return retorno;
        }
    }
}
