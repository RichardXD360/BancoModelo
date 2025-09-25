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
            return Ok("Working...");
        }

        [HttpPost("/CriarUsuario")]
        public ActionResult CriarUsuario(UsuarioDTC usuario) {
            ResultadoRetorno retorno = _bancoVerify.CriarUsuario(usuario);
            return Ok(retorno);
        }

        [HttpPost("/BuscarDados")]
        public ActionResult DB(UsuarioDTC usuario) {

            ResultadoRetorno retorno = _bancoVerify.ValidarUsuario(usuario); //Desejo utiliza-lo aqui
            return Ok(retorno);
        }

        [HttpPost("/VerificarExtrato")]
        public ActionResult VerificarExtrato(string cpf, DateTime dataInicio, DateTime dataFim) 
        {
            
            return Ok();
        }
    }
}
