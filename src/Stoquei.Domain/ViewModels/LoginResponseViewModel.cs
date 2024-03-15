using Stoquei.Domain.Enums;

namespace Stoquei.Domain.ViewModels
{
    public class LoginResponseViewModel
    {
        public int UsuarioId { get; set; }
        public List<Acesso> UsuarioAcessos { get; set; }
    }
}
