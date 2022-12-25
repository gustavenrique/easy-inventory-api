using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IFornecedorRepository
    {
        Task<List<FornecedorDto>> BuscarFornecedores();
        Task<int> CriarFornecedor(FornecedorDto fornecedor);
    }
}
