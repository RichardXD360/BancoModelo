using GeradorPDF;
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
        private readonly PdfService  _pdf;
        public Controller(BancoVerify bancoVerify, PdfService pdf)
        {
            _bancoVerify = bancoVerify;
            _pdf = pdf;
        }
        [HttpGet("/")]
        public ActionResult Get()
        {
            _bancoVerify.AbrirConexao();
            return Ok("Working...");
        }

        [HttpPost("/Usuario/CriarUsuario")]
        public ActionResult CriarUsuario(UsuarioDTC usuario) {
            ResultadoRetornoUsuarioId retorno = _bancoVerify.CriarUsuario(usuario);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso = retorno.Sucesso,
            });
        }

        [HttpPost("/Usuario/VerificarUsuario")]
        public ActionResult BuscarDados(UsuarioLoginDTC usuario) {

            ResultadoRetornoUsuarioId retorno = _bancoVerify.ValidarUsuario(usuario);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso  = retorno.Sucesso,
                UsuarioId = retorno.UsuarioId,
            });
        }

        [HttpGet("/Usuario/DetalhesUsuario/{id}")]
        public ActionResult DetalhesUsuario(int id)
        {
            DadosUsuario retorno = _bancoVerify.DetalhesUsuario(id);
            if(retorno.Saldo == 404)
            {
                return StatusCode(retorno.Saldo, new
                {
                    Mensagem = retorno.Nome
                });
            }
            return StatusCode(200, new
            {
                DetalhesUsuario = retorno,
            });
        }
        [HttpPost("/Transacao/EfetuarTransacao")]
        public ActionResult EfetuarTransacao(TransacaoDTO transacao)
        {
            ResultadoRetornoHTTP retorno = _bancoVerify.EfetuarTransacao(transacao);
            return StatusCode(retorno.StatusCode, new
            {
                Mensagem = retorno.Mensagem,
                Sucesso = retorno.Sucesso,
            });
        }
        [HttpGet("/Transacao/{id}/GerarComprovante")]
        public byte[] GerarComprovante(int id)
        {
            TransacaoDTO transacao = _bancoVerify.RetornarTransacao(id);
            return _pdf.GerarComprovante(transacao);

        }

        [HttpPost("/Transacao/VerificarExtrato")]
        public ActionResult VerificarExtrato(string cpf, DateTime dataInicio, DateTime dataFim) 
        {
            
            return Ok();
        }
    }
}
