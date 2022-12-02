using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CodigoEan { get; set; }
        public decimal Preco { get; set; }
        public string Fabricante { get; set; }
        public int CategoriaId { get; set; }
        public int Quantia { get; set; }
    }
}
