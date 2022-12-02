using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Stoquei.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MensagemBase<ProdutosCategoriasDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(MensagemBase<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Produto - Get All - Início");

            var retorno = await _service.BuscarTodos();

            _logger.LogInformation($"Produto - Get All - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MensagemBase<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] ProdutoDto produto)
        {
            _logger.LogInformation($"Produto - Post - Início");

            var retorno = await _service.CriarProduto(produto);

            _logger.LogInformation($"Produto - Post - Fim - Retorno: {JsonConvert.SerializeObject(retorno)}");

            return StatusCode(retorno.StatusCode, retorno);
        }
    }
}
