using Stoquei.Application.Interfaces;
using Stoquei.Domain.Dtos;
using Stoquei.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Stoquei.Controllers
{
    [Route("fornecedores")]
    [ApiController]
    public class FornecedorController : ControllerBase
    {
        private readonly ILogger<FornecedorController> _logger;
        private readonly IFornecedorService _service;

        public FornecedorController(ILogger<FornecedorController> logger, IFornecedorService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<List<FornecedorDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Fornecedor - Get All - Início");

            var retorno = await _service.BuscarTodos();

            _logger.LogInformation("Fornecedor - Get All - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] FornecedorDto fornecedor)
        {
            _logger.LogInformation($"Fornecedor - Post - Início");

            var retorno = await _service.CriarFornecedor(fornecedor);

            _logger.LogInformation("Fornecedor - Post - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [Route("{fornecedorId:int}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int fornecedorId)
        {
            _logger.LogInformation($"Fornecedor - Delete - Início");

            var retorno = await _service.DeletarFornecedor(fornecedorId);

            _logger.LogInformation("Fornecedor - Delete - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] FornecedorDto fornecedor)
        {
            _logger.LogInformation($"Fornecedor - Put - Início");

            var retorno = await _service.AtualizarFornecedor(fornecedor);

            _logger.LogInformation("Fornecedor - Put - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }
    }
}
