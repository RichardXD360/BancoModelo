using Application;
using Microsoft.AspNetCore.Mvc;
using Service;
using Shared.Models;

namespace ExtratoBanco.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class Controller: ControllerBase
    {
        private readonly BancoVerify _bancoVerify;
        public Controller(BancoVerify bancoVerify)
        {
            _bancoVerify = bancoVerify;
        }
        [HttpGet("/")]
        public ActionResult Get()
        {
            _bancoVerify.AbrirConexao();
            return Ok("Working...");
        }

        [HttpPost("/CriarUsuario")]
        public ActionResult CriarUsuario(UsuarioDTC usuario) {
            ResultadoRetornoHTTP retorno = _bancoVerify.CriarUsuario(usuario);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso = retorno.Sucesso,
            });
        }

        [HttpPost("/VerificarUsuario")]
        public ActionResult BuscarDados(UsuarioDTC usuario) {

            ResultadoRetornoHTTP retorno = _bancoVerify.ValidarUsuario(usuario);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso  = retorno.Sucesso,
            });
        }

        [HttpPost("/EfetuarTransacao")]
        public ActionResult EfetuarTransacao(TransacaoDTO transacao)
        {
            ResultadoRetornoHTTP retorno = _bancoVerify.EfetuarTransacao(transacao);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso = retorno.Sucesso,
            });
        }

        [HttpPost("/VerificarExtrato")]
        public ActionResult VerificarExtrato(string cpf, DateTime dataInicio, DateTime dataFim) 
        {
            
            return Ok();
        }
    }
}
