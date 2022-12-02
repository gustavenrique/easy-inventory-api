using Domain.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<UsuarioDto>> BuscarUsuarios();
        Task<UsuarioDto> BuscarUsuario(string username, int usuarioId);
        Task<bool> CriarUsuario(UsuarioDto usuario);
    }
}
