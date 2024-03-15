using Stoquei.Domain.Dtos;

namespace Stoquei.Infra.Interfaces
{
    public interface IFornecedorRepository
    {
        Task<List<FornecedorDto>> BuscarFornecedores();
        Task<int> CriarFornecedor(FornecedorDto fornecedor);
        Task<FornecedorDto> BuscarFornecedor(int fornecedorId);
        Task<bool> DeletarFornecedor(int fornecedorId);
        Task<bool> AtualizarFornecedor(FornecedorDto fornecedor);
    }
}
