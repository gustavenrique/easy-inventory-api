using Dapper;
using Domain.Dtos;
using Infrastructure.Interfaces;
using System.Data;

namespace Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository, IProdutoRepository
    {
        public ProdutoRepository(string connectionString) : base(connectionString) { }

        public async Task<ProdutoSimplificadoDto> BuscarProdutosCompletos()
        {
            var query = @"SELECT Id, Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque AS Quantia FROM Produto WITH(NOLOCK)
                          SELECT Id, Nome FROM Categoria WITH(NOLOCK)
                          SELECT Id, Nome, Email, Telefone FROM Fornecedor WITH(NOLOCK)
                          SELECT Id, ProdutoId, FornecedorId FROM Produto_Fornecedor WITH(NOLOCK)";

            return await MultipleQueryAsync<ProdutoSimplificadoDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var retorno = new ProdutoSimplificadoDto();

                    retorno.Produtos = (await gridRetornado.ReadAsync<ProdutoDto>()).ToList();
                    retorno.Categorias = (await gridRetornado.ReadAsync<CategoriaDto>()).ToList();
                    retorno.Fornecedores = (await gridRetornado.ReadAsync<FornecedorDto>()).ToList();
                    var produtoFornecedores = (await gridRetornado.ReadAsync<ProdutoFornecedorDto>()).ToList();
                    
                    retorno.Produtos.ForEach(p =>
                    {
                        p.Fornecedores = new List<int>();
                        produtoFornecedores.ForEach(pf =>
                        {
                            if (pf.ProdutoId == p.Id) p.Fornecedores.Add(pf.FornecedorId);
                        });
                    });

                    return retorno;
                });
        }

        public async Task<ProdutoDto> BuscarProduto(int produtoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ProdutoId", produtoId, DbType.Int32);

            var query = @"SELECT Id, Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque AS Quantia FROM Produto WITH(NOLOCK) WHERE ID = @ProdutoId
                          SELECT FornecedorId FROM Produto_Fornecedor WITH(NOLOCK) WHERE ProdutoId = @ProdutoId";

            return await MultipleQueryAsync<ProdutoDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var produto = await gridRetornado.ReadFirstOrDefaultAsync<ProdutoDto>();
                    produto.Fornecedores = (await gridRetornado.ReadAsync<int>()).ToList();

                    return produto;
                },
                param: parameters);
        }

        public async Task<ProdutoDto> BuscarPorCodigoEan(string codigoEan)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CodigoEan", codigoEan, DbType.Int32);

            var query = @"SELECT Id, Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque AS Quantia FROM Produto WITH(NOLOCK) WHERE CodigoEan = @CodigoEan";

            return await QueryFirstOrDefaultAsync<ProdutoDto>(query, param: parameters, commandType: CommandType.Text);
        }

        public async Task<int> CriarProduto(ProdutoDto produto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Nome", produto.Nome, DbType.String);
            parameters.Add("@CodigoEan", produto.CodigoEan, DbType.String);
            parameters.Add("@Preco", produto.Preco, DbType.Decimal);
            parameters.Add("@Fabricante", produto.Fabricante, DbType.String);
            parameters.Add("@CategoriaId", produto.CategoriaId, DbType.Int32);
            parameters.Add("@EmEstoque", produto.Quantia, DbType.Int32);

            var query = @"INSERT INTO Produto (Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque)
                          OUTPUT INSERTED.Id
                          VALUES (@Nome, @CodigoEan, @Preco, @Fabricante, @CategoriaId, @EmEstoque)";

            return await QueryFirstOrDefaultAsync<int>(query, commandType: CommandType.Text, param: parameters);
        }

        public async Task<bool> AtualizarProduto(ProdutoDto produto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", produto.Id, DbType.Int64);
            parameters.Add("@Nome", produto.Nome, DbType.String);
            parameters.Add("@CodigoEan", produto.CodigoEan, DbType.String);
            parameters.Add("@Preco", produto.Preco, DbType.Decimal);
            parameters.Add("@Fabricante", produto.Fabricante, DbType.String);
            parameters.Add("@CategoriaId", produto.CategoriaId, DbType.Int32);
            parameters.Add("@EmEstoque", produto.Quantia, DbType.Int32);

            var query = @"UPDATE Produto SET Nome = @Nome, CodigoEan = @CodigoEan, Preco = @Preco, Fabricante = @Fabricante, CategoriaId = @CategoriaId, EmEstoque = @EmEstoque WHERE Id = @Id";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }

        public async Task<bool> DeletarProduto(int produtoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ProdutoId", produtoId, DbType.Int64);

            var query = "DELETE FROM Produto WHERE Id = @ProdutoId";

            return await ExecuteAsync(query, param: parameters, commandType: CommandType.Text) > 0;
        }

        #region ProdutoFornecedor
        public async Task<bool> CriarProdutoFornecedor(ProdutoDto produto)
        {
            var parameters = new List<dynamic>();
            produto.Fornecedores.ForEach(f => parameters.Add(new { ProdutoId = produto.Id, FornecedorId = f }));

            var query = @"INSERT INTO Produto_Fornecedor (ProdutoId, FornecedorId) VALUES (@ProdutoId, @FornecedorId)";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }

        public async Task<bool> DeletarProdutoFornecedor(int produtoId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ProdutoId", produtoId, DbType.Int64);

            var query = @"DELETE FROM Produto_Fornecedor WHERE ProdutoId = @ProdutoId";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }
        #endregion
    }
}
