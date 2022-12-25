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

        public async Task<FornecedorDto> BuscarFornecedor(int fornecedorId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FornecedorId", fornecedorId, DbType.Int32);

            var query = @"SELECT Id, Nome, Email, Telefone FROM Fornecedor WITH(NOLOCK) WHERE ID = @FornecedorId";

            return await QueryFirstOrDefaultAsync<FornecedorDto>(query, parameters, commandType: CommandType.Text);
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

        public async Task<bool> DeletarFornecedor(int fornecedorId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FornecedorId", fornecedorId, DbType.Int64);

            var query = "DELETE FROM Fornecedor WHERE Id = @FornecedorId";

            return await ExecuteAsync(query, param: parameters, commandType: CommandType.Text) > 0;
        }

        public async Task<bool> AtualizarFornecedor(FornecedorDto fornecedor)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", fornecedor.Id, DbType.Int32);
            parameters.Add("@Nome", fornecedor.Nome, DbType.String);
            parameters.Add("@Email", fornecedor.Email, DbType.String);
            parameters.Add("@Telefone", fornecedor.Telefone, DbType.String);

            var query = @"UPDATE Fornecedor SET Nome = @Nome, Email = @Email, Telefone = @Telefone WHERE Id = @Id";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }
    }
}
