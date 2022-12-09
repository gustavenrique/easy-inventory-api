using Domain.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<ProdutoDto>> BuscarProdutos();
        Task<ProdutoSimplificadoDto> BuscarProdutosCompletos();
        Task<ProdutoDto> BuscarProduto(int produtoId);
        Task<int> CriarProduto(ProdutoDto produto);
        Task<bool> DeletarProduto(int produtoId);
        Task<bool> AtualizarProduto(ProdutoDto produto);
        Task<bool> CriarProdutoFornecedor(ProdutoDto produto);
        Task<bool> DeletarProdutoFornecedor(int produtoId);

    }
}
