namespace Stoquei.Domain.Dtos
{
    public class ProdutoSimplificadoDto
    {
        public List<ProdutoDto> Produtos { get; set; }
        public List<CategoriaDto> Categorias { get; set; }
        public List<FornecedorDto> Fornecedores { get; set; }
    }
}
