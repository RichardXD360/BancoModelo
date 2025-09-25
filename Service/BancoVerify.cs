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
        public ResultadoRetorno ValidarUsuario(UsuarioDTC usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Login))
            {
                resultadoRetorno.Sucesso = false;
                resultadoRetorno.Mensagem = "Usuario invalido ou vazio";
                return resultadoRetorno;
            }
            resultadoRetorno = _data.VerificarUsuario(usuario);
            return resultadoRetorno;
        }
        public ResultadoRetorno CriarUsuario(UsuarioDTC usuario)
        {
            ResultadoRetorno validarUsuarioExistente = new ResultadoRetorno();
            validarUsuarioExistente = ValidarUsuario(usuario);
            if (validarUsuarioExistente.Sucesso)
            {
                return new ResultadoRetorno
                {
                    Sucesso = false,
                    Mensagem = "Login de usuário já existente.";
                };
            }
            if(usuario.Nome.Length <= 7 || usuario.Nome.Length >= 33){
                return new ResultadoRetorno
                {
                    Sucesso = false,
                    Mensagem = "Nome de usuário inválido. O nome deve conter entre 8 a 32 caracteres."
                };
            }
            resultadoRetorno = _data.CriarUsuario(usuario);
            return resultadoRetorno;
        }
    }
}
