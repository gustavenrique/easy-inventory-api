using Domain.Dtos;
using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IUsuariosService
    {
        Task<MensagemBase<List<UsuarioDto>>> BuscarTodos();
        Task<MensagemBase<UsuarioDto>> BuscarUsuario(string username, int usuarioId = 0);

        Task<MensagemBase<bool>> CriarUsuario(UsuarioDto usuario);
        Task<MensagemBase<bool>> LogarUsuario(LoginViewModel usuario);
    }
}
