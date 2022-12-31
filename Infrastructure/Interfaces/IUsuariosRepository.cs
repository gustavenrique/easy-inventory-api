using Domain.Dtos;

namespace Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioDto> BuscarUsuario(string username, int usuarioId);
        Task<UsuarioSimplificadoDto> BuscaSimplificada();
        Task<int> CriarUsuario(UsuarioDto usuario);
        Task<List<AcessoDto>> BuscarAcessos();  
        Task<bool> CriarUsuarioAcesso(UsuarioDto usuario);
        Task<bool> DeletarUsuarioAcesso(int usuarioId);
    }
}
