using Dapper;
using Domain.Dtos;
using Infrastructure.Interfaces;
using System.Data;

namespace Infrastructure.Repositories
{
    public class FornecedorRepository : BaseRepository, IFornecedorRepository
    {
        public FornecedorRepository(string connectionString) : base(connectionString) { }

        public async Task<List<FornecedorDto>> BuscarFornecedores()
        {
            var query = @"SELECT Id, Nome, Email, Telefone FROM Fornecedor WITH(NOLOCK)";

            return await QueryAsync<FornecedorDto>(query, commandType: CommandType.Text);
        }

        public async Task<int> CriarFornecedor(FornecedorDto fornecedor)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Nome", fornecedor.Nome, DbType.String);
            parameters.Add("@Email", fornecedor.Email, DbType.String);
            parameters.Add("@Telefone", fornecedor.Telefone, DbType.String);

            var query = @"INSERT INTO Fornecedor (Nome, Email, Telefone)
                          OUTPUT INSERTED.Id
                          VALUES (@Nome, @Email, @Telefone)";

            return await QueryFirstOrDefaultAsync<int>(query, commandType: CommandType.Text, param: parameters);
        }
    }
}
