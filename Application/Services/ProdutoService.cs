using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;
        private readonly ILogger<ProdutoService> _logger;
        public ProdutoService(IProdutoRepository repository, ILogger<ProdutoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<MensagemBase<ProdutosCategoriasDto>> BuscarTodos()
        {
            try
            {
                var consultaSimplificada = await _repository.BuscarProdutoCategoria();
                
                if (consultaSimplificada.Produtos?.Any() == false)
                    return new MensagemBase<ProdutosCategoriasDto>(StatusCodes.Status404NotFound, "Não há produtos registrados.");

                return new MensagemBase<ProdutosCategoriasDto>(StatusCodes.Status200OK, "Produtos buscados com sucesso!", consultaSimplificada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - BuscarTodos - Erro ao tentar buscar os produtos. Exception: {ex}");
                return new MensagemBase<ProdutosCategoriasDto>(StatusCodes.Status500InternalServerError, "Erro ao buscar os produtos.");
            }
        }

        public async Task<MensagemBase<bool>> CriarProduto(ProdutoDto produto)
        {
            try
            {
                if (string.IsNullOrEmpty(produto.Nome) || string.IsNullOrEmpty(produto.CodigoEan) || produto.Preco <= 0)
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Há campos a serem preenchidos.");

                var resultado = await _repository.CriarProduto(produto);

                if (!resultado)
                {
                    _logger.LogError($"Produto - Post - Erro ao tentar criar produto. Produto: {produto}");
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Ocorreu um erro ao tentar registrar o usuário.");
                }

                return new MensagemBase<bool>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Produto criado com sucesso!",
                    Object = resultado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao criar o produto. Produto: {produto}", false);
            }
        }
        public async Task<MensagemBase<bool>> AtualizarProduto(ProdutoDto produto)
        {
            throw new NotImplementedException();
        }

        public async Task<MensagemBase<ProdutoDto>> BuscarProduto(int produtoId)
        {
            throw new NotImplementedException();
        }


        public async Task<MensagemBase<bool>> DeletarProduto(int produtoId)
        {
            throw new NotImplementedException();
        }
    }
}
