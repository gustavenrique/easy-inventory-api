using Stoquei.Application.Interfaces;
using Stoquei.Domain.Dtos;
using Stoquei.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Stoquei.Api.Controllers
{
    [ApiController]
    [Route("produtos")]
    public class ProdutoController : ControllerBase
    {
        private readonly ILogger<ProdutoController> _logger;
        private readonly IProdutoService _service;
        public ProdutoController(ILogger<ProdutoController> logger, IProdutoService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<ProdutoSimplificadoDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Produto - Get All - Início");

            var retorno = await _service.BuscarTodos();

            _logger.LogInformation("Produto - Get All - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<int>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] ProdutoDto produto)
        {
            _logger.LogInformation($"Produto - Post - Início");

            var retorno = await _service.CriarProduto(produto);

            _logger.LogInformation("Produto - Post - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [Route("{produtoId:int}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int produtoId)
        {
            _logger.LogInformation($"Produto - Delete - Início");

            var retorno = await _service.DeletarProduto(produtoId);

            _logger.LogInformation("Produto - Delete - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] ProdutoDto produto)
        {
            _logger.LogInformation($"Produto - Put - Início");

            var retorno = await _service.AtualizarProduto(produto);

            _logger.LogInformation("Produto - Put - Fim - Retorno: {@Retorno}", retorno);

            return StatusCode(retorno.StatusCode, retorno);
        }
    }
}
