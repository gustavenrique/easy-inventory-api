using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class ProdutosCategoriasDto
    {
        public List<ProdutoDto> Produtos { get; set; }
        public List<CategoriaDto> Categorias { get; set; }
    }
}
