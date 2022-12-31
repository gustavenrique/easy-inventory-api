using Domain.Dtos;

namespace Domain.ViewModels
{
    public class UsuarioCriacaoViewModel
    {
        public string Usuario { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Admin { get; set; }
        public List<int> Acessos { get; set; }

        public UsuarioDto ParaDto()
        {
            return new UsuarioDto(Usuario, Email, Ativo, Admin, Acessos);
        }
    }
}
