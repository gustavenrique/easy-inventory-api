using Stoquei.Application.Interfaces;
using Stoquei.Domain.Dtos;
using Stoquei.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Stoquei.Api.Controllers
{
    [ApiController]
    [Route("usuarios")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IUsuariosService _service;

        public UsuariosController(ILogger<UsuariosController> logger, IUsuariosService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<List<UsuarioDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Usuarios - Get All - Início");

            MensagemBase<UsuarioSimplificadoDto> retorno = await _service.BuscarTodos();

            _logger.LogInformation("Usuarios - Get All - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpGet]
        [Route("{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<UsuarioDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        public async Task<IActionResult> Get([FromHeader(Name = "x-username")] string username,
                                             [FromRoute(Name = "usuarioId")] int usuarioId = 0)
        {
            _logger.LogInformation($"Usuarios - Get - Início");

            MensagemBase<UsuarioDto> retorno = await _service.BuscarUsuario(username, usuarioId);

            _logger.LogInformation("Usuarios - Get - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MensagemBase<List<UsuarioDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        public async Task<IActionResult> Post(UsuarioCriacaoViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Post - Início");

            MensagemBase<int> retorno = await _service.CriarUsuario(usuario);
            
            _logger.LogInformation("Usuarios - Post - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<LoginResponseViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<LoginResponseViewModel>))]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed, Type = typeof(MensagemBase<LoginResponseViewModel>))]
        public async Task<IActionResult> Login([FromBody] LoginViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Login - Início");

            MensagemBase<LoginResponseViewModel> retorno = await _service.LogarUsuario(usuario);
            
            _logger.LogInformation("Usuarios - Login - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPatch]
        [Route("alterar-senha")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<LoginResponseViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<LoginResponseViewModel>))]
        public async Task<IActionResult> AlterarSenha([FromBody] AlteracaoSenhaViewModel model)
        {
            _logger.LogInformation($"Usuarios - AlterarSenha - Início");

            MensagemBase<LoginResponseViewModel> retorno = await _service.AlterarSenha(model);
          
            _logger.LogInformation("Usuarios - AlterarSenha - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [Route("{usuarioId:int}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int usuarioId)
        {
            _logger.LogInformation($"Usuarios - Delete - Início");

            var retorno = await _service.DeletarUsuario(usuarioId);
            
            _logger.LogInformation("Usuarios - Delete - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<bool>))]
        public async Task<IActionResult> Put([FromBody] UsuarioAlteracaoViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Put - Início");

            var retorno = await _service.AtualizarUsuario(usuario);

            _logger.LogInformation("Usuarios - Put - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

    }
}
