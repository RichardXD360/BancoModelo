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
        public ResultadoRetornoHTTP EfetuarTransacao(TransacaoDTO transacao)
        {
            if (transacao == null || transacao.Valor == 0 || transacao.DataTransacao == null)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Transação inválida ou nula, verifique os dados enviados",
                    Sucesso = false,
                    StatusCode = 400
                };
            }
            if (transacao.TipoTransacao > EnumTipoTransacao.Tranferencia)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Tipo Transação inválida ou inexistente, verifique os dados enviados",
                    Sucesso = false,
                    StatusCode = 404
                };
            }
            ResultadoRetorno validarUsuario = _data.VerificarUsuarioId(transacao.UsuarioId);
            if (validarUsuario.Sucesso == false)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Usuario não encontrado.",
                    Sucesso = false,
                    StatusCode = 404
                };
            }
            int validarUsuarioRecebedorCnpj = _data.VerificarUsuarioCnpj(transacao.UsuarioRecebedorCnpj);

            if(validarUsuarioRecebedorCnpj == transacao.UsuarioId)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Usuario recebedor não pode ser você.",
                    Sucesso = false,
                    StatusCode = 400
                };
            }

            if(validarUsuarioRecebedorCnpj == 0)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Usuario recebedor não encontrado.",
                    Sucesso = false,
                    StatusCode = 404
                };
            }
            ResultadoRetorno validarUsuarioRecebedor = _data.VerificarUsuarioId(validarUsuarioRecebedorCnpj);
            if (validarUsuarioRecebedor.Sucesso == false)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Usuario recebedor não encontrado.",
                    Sucesso = false,
                    StatusCode = 404
                };
            }
            int saldoUsuario = _data.VerificarSaldo(transacao.UsuarioId);
            if (saldoUsuario < transacao.Valor)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Saldo insuficiente.",
                    Sucesso = false,
                    StatusCode = 409
                };
            }
            ResultadoRetorno retorno = _data.EfetuarTransacao(transacao, validarUsuarioRecebedorCnpj);
            return new ResultadoRetornoHTTP
            {
                Mensagem = retorno.Mensagem,
                Sucesso = retorno.Sucesso,
                StatusCode = 200
            };
        }
        public ResultadoRetornoHTTP ValidarUsuario(UsuarioDTC usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Login))
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = "Usuario invalido ou vazio",
                    Sucesso = false,
                    StatusCode = 401
                };
            }
            resultadoRetorno = _data.VerificarUsuario(usuario);
            if (!resultadoRetorno.Sucesso)
            {
                return new ResultadoRetornoHTTP
                {
                    Mensagem = resultadoRetorno.Mensagem,
                    Sucesso = resultadoRetorno.Sucesso,
                    StatusCode = 404
                };
            }
            return new ResultadoRetornoHTTP
            {
                Mensagem = resultadoRetorno.Mensagem,
                Sucesso = resultadoRetorno.Sucesso,
                StatusCode = 200
            };
        }
        public ResultadoRetornoHTTP CriarUsuario(UsuarioDTC usuario)
        {
            ResultadoRetornoHTTP validarUsuarioExistente = new ResultadoRetornoHTTP();
            validarUsuarioExistente = ValidarUsuario(usuario);
            if (validarUsuarioExistente.Sucesso)
            {
                return new ResultadoRetornoHTTP
                {
                    Sucesso = false,
                    Mensagem = "Login de usuário já existente.",
                    StatusCode = 409
                };
            }
            if (usuario.Nome.Length <= 7 || usuario.Nome.Length >= 33)
            {
                return new ResultadoRetornoHTTP
                {
                    Sucesso = false,
                    Mensagem = "Nome de usuário inválido. O nome deve conter entre 8 a 32 caracteres.",
                    StatusCode = 401
                };
            }
            resultadoRetorno = _data.CriarUsuario(usuario);
            return new ResultadoRetornoHTTP
            {
                Mensagem = resultadoRetorno.Mensagem,
                Sucesso = resultadoRetorno.Sucesso,
                StatusCode = 201
            };
        }

        public DadosUsuario DetalhesUsuario(int usuarioId)
        {
            var validarUsuario = _data.VerificarUsuarioId(usuarioId);
            if (validarUsuario.Sucesso = false)
            {
                return new DadosUsuario
                {
                    Nome = "Usuário inexistente na base de dados",
                    Saldo = 404
                };
            }
            DadosUsuario retorno = _data.DetalhesUsuario(usuarioId);
            return retorno;
        }
    }
}
