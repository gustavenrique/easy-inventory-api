using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Stoquei.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Usuarios - Get All - Início");

            MensagemBase<UsuarioSimplificadoDto> retorno = await _service.BuscarTodos();

            _logger.LogInformation($"Usuarios - Get All - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpGet]
        [Route("{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<UsuarioDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromHeader(Name = "x-username")] string username,
                                             [FromRoute(Name = "usuarioId")] int usuarioId = 0)
        {
            _logger.LogInformation($"Usuarios - Get - Início");

            MensagemBase<UsuarioDto> retorno = await _service.BuscarUsuario(username, usuarioId);

            _logger.LogInformation($"Usuarios - Get - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MensagemBase<List<UsuarioDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(UsuarioCriacaoViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Post - Início");

            MensagemBase<int> retorno = await _service.CriarUsuario(usuario);

            _logger.LogInformation($"Usuarios - Post - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Login - Início");

            MensagemBase<int> retorno = await _service.LogarUsuario(usuario);

            _logger.LogInformation($"Usuarios - Login - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPatch]
        [Route("AlterarSenha")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AlterarSenha([FromBody] AlteracaoSenhaViewModel model)
        {
            _logger.LogInformation($"Usuarios - AlterarSenha - Início");

            MensagemBase<int> retorno = await _service.AlterarSenha(model);

            _logger.LogInformation($"Usuarios - AlterarSenha - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [Route("{usuarioId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int usuarioId)
        {
            _logger.LogInformation($"Usuarios - Delete - Início");

            var retorno = await _service.DeletarUsuario(usuarioId);

            _logger.LogInformation($"Usuarios - Delete - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [Route("")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UsuarioAlteracaoViewModel usuario)
        {
            _logger.LogInformation($"Usuarios - Put - Início");

            var retorno = await _service.AtualizarUsuario(usuario);

            _logger.LogInformation($"Usuarios - Put - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

    }
}
