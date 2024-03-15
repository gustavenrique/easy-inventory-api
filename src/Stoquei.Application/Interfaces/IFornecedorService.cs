using Stoquei.Domain.Dtos;
using Stoquei.Domain.ViewModels;

namespace Stoquei.Application.Interfaces
{
    public interface IFornecedorService
    {
        Task<MensagemBase<List<FornecedorDto>>> BuscarTodos();
        Task<MensagemBase<int>> CriarFornecedor(FornecedorDto fornecedor);
        Task<MensagemBase<bool>> DeletarFornecedor(int fornecedorId);
        Task<MensagemBase<bool>> AtualizarFornecedor(FornecedorDto fornecedor);
    }
}
