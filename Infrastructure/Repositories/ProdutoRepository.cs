using Dapper;
using Domain.Dtos;
using Infrastructure.Interfaces;
using System.Data;

namespace Infrastructure.Repositories
{
    public class ProdutoRepository : BaseRepository, IProdutoRepository
    {
        public ProdutoRepository(string connectionString) : base(connectionString) { }

        public async Task<List<ProdutoDto>> BuscarProdutos()
        {
            var query = @"SELECT Id, Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque AS Quantia FROM Produto WITH(NOLOCK)";

            return await QueryAsync<ProdutoDto>(query, commandType: CommandType.Text);
        }

        public async Task<ProdutosCategoriasDto> BuscarProdutoCategoria()
        {
            var query = @"SELECT Id, Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque AS Quantia FROM Produto WITH(NOLOCK)
                          SELECT Id, Nome FROM Categoria WITH(NOLOCK)";

            return await MultipleQueryAsync<ProdutosCategoriasDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var retorno = new ProdutosCategoriasDto();

                    retorno.Produtos = (await gridRetornado.ReadAsync<ProdutoDto>()).ToList();
                    retorno.Categorias = (await gridRetornado.ReadAsync<CategoriaDto>()).ToList();

                    return retorno;
                });
        }

        public async Task<ProdutoDto> BuscarProduto(int produtoId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CriarProduto(ProdutoDto produto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Nome", produto.Nome, DbType.String);
            parameters.Add("@CodigoEan", produto.CodigoEan, DbType.String);
            parameters.Add("@Preco", produto.Preco, DbType.Decimal);
            parameters.Add("@Fabricante", produto.Fabricante, DbType.String);
            parameters.Add("@CategoriaId", produto.CategoriaId, DbType.Int32);
            parameters.Add("@EmEstoque", produto.Quantia, DbType.Int32);

            var query = @"INSERT INTO Produto (Nome, CodigoEan, Preco, Fabricante, CategoriaId, EmEstoque) 
                          VALUES (@Nome, @CodigoEan, @Preco, @Fabricante, @CategoriaId, @EmEstoque)";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }

        public async Task<List<CategoriaDto>> BuscarCategorias()
        {
            var query = "SELECT Id, Nome FROM Categoria WITH(NOLOCK)";

            return await QueryAsync<CategoriaDto>(query, commandType: CommandType.Text);
        }
    }
}
