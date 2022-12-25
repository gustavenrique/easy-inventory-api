using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Transactions;

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

        public async Task<MensagemBase<ProdutoSimplificadoDto>> BuscarTodos()
        {
            try
            {
                var consultaSimplificada = await _repository.BuscarProdutosCompletos();
                
                if (consultaSimplificada.Produtos?.Any() == false)
                    return new MensagemBase<ProdutoSimplificadoDto>(StatusCodes.Status404NotFound, "Não há produtos registrados.");

                return new MensagemBase<ProdutoSimplificadoDto>(StatusCodes.Status200OK, "Produtos buscados com sucesso!", consultaSimplificada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - BuscarTodos - Erro ao tentar buscar os produtos. Exception: {ex}");
                return new MensagemBase<ProdutoSimplificadoDto>(StatusCodes.Status500InternalServerError, "Erro ao buscar os produtos.");
            }
        }

        public async Task<MensagemBase<int>> CriarProduto(ProdutoDto produto)
        {
            try
            {
                if (string.IsNullOrEmpty(produto.Nome) || string.IsNullOrEmpty(produto.CodigoEan) || produto.Preco <= 0)
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Há campos a serem preenchidos.");

                var produtoExistente = await _repository.BuscarPorCodigoEan(produto.CodigoEan);
                if (produtoExistente != null)
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, $"O código EAN '{produto.CodigoEan}' já está em uso");

                bool resultado;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var produtoCriadoId = await _repository.CriarProduto(produto);

                    produto.Id = produtoCriadoId;
                    var resultProdutoFornecedor = await _repository.CriarProdutoFornecedor(produto);

                    resultado = produtoCriadoId > 0 && resultProdutoFornecedor;

                    transaction.Complete();
                }

                if (!resultado)
                {
                    _logger.LogError($"Produto - Post - Erro ao tentar criar produto. Produto: {produto}");
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Ocorreu um erro ao tentar registrar o produto.");
                }

                return new MensagemBase<int>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Produto criado com sucesso!",
                    Object = produto.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - Post - Erro ao tentar buscar os produtos. Exception: {ex}");
                return new MensagemBase<int>(StatusCodes.Status500InternalServerError, $"Erro ao criar o produto. Produto: {produto}");
            }
        }
        
        public async Task<MensagemBase<bool>> AtualizarProduto(ProdutoDto produto)
        {
            try
            {
                var produtoBanco = await _repository.BuscarProduto(produto.Id);

                if (produtoBanco == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível alterar um produto inexistente.", false);

                var houveAlteracaoSimples = produto.Nome != produtoBanco.Nome || produto.Preco != produtoBanco.Preco || produto.CodigoEan != produtoBanco.CodigoEan || produto.CategoriaId != produtoBanco.CategoriaId || produto.Fabricante != produtoBanco.Fabricante || produto.Quantia != produtoBanco.Quantia;
                var houveAlteracaoComplexa = !areListsEqual(produtoBanco.Fornecedores, produto.Fornecedores);

                if (!houveAlteracaoComplexa && !houveAlteracaoSimples)
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Nenhuma propriedade foi alterada.", false);

                var sucesso = false;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    if (houveAlteracaoSimples) sucesso = await _repository.AtualizarProduto(produto);
         
                    if (houveAlteracaoComplexa) sucesso = sucesso && await _repository.DeletarProdutoFornecedor(produto.Id) && await _repository.CriarProdutoFornecedor(produto);

                    transaction.Complete();
                }

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Produto atualizado com sucesso!", sucesso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - AtualizarProduto - Erro ao tentar atualizar o produto. Produto: {produto}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Ocorreu um erro inesperado ao atualizar o produto.", false);
            }
        }

        private bool areListsEqual(List<int> list1, List<int> list2)
        {
            var firstNotSecond = list1.Except(list2).ToList();
            var secondNotFirst = list2.Except(list1).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }

        public async Task<MensagemBase<bool>> DeletarProduto(int produtoId)
        {
            try
            {
                var produtoExiste = await _repository.BuscarProduto(produtoId);

                if (produtoExiste == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível deletar um produto inexistente.", false);

                var resultado = await _repository.DeletarProduto(produtoId);

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Produto deletado com sucesso!", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Produto - BuscarTodos - Erro ao tentar deletar o produto com Id: {produtoId}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao deletar o produto.");
            }
        }
    }
}
