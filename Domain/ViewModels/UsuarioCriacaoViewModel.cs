using Domain.Dtos;
using Domain.Enums;

namespace Domain.ViewModels
{
    public class UsuarioCriacaoViewModel
    {
        public string Usuario { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Admin { get; set; }
        public List<Acesso> Acessos { get; set; }

        public UsuarioDto ParaDto() => new UsuarioDto(Usuario, Email, Ativo, Admin, Acessos);
    }
}
