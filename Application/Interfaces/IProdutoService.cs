using Domain.Dtos;
using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IProdutoService
    {
        Task<MensagemBase<ProdutosCategoriasDto>> BuscarTodos();
        Task<MensagemBase<ProdutoDto>> BuscarProduto(int produtoId);
        Task<MensagemBase<bool>> CriarProduto(ProdutoDto produto);
        Task<MensagemBase<bool>> AtualizarProduto(ProdutoDto produto);
        Task<MensagemBase<bool>> DeletarProduto(int produtoId);
    }
}
