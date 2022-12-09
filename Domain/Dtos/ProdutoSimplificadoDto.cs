using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class ProdutoSimplificadoDto
    {
        public List<ProdutoDto> Produtos { get; set; }
        public List<CategoriaDto> Categorias { get; set; }
        public List<FornecedorDto> Fornecedores { get; set; }
    }
}
