using Domain.Enums;

namespace Domain.ViewModels
{
    public class LoginResponseViewModel
    {
        public int UsuarioId { get; set; }
        public List<Acesso> UsuarioAcessos { get; set; }
    }
}
