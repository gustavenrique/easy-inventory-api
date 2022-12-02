using Domain.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<ProdutoDto>> BuscarProdutos();
        Task<ProdutosCategoriasDto> BuscarProdutoCategoria();
        Task<ProdutoDto> BuscarProduto(int produtoId);
        Task<bool> CriarProduto(ProdutoDto produto);
    }
}
