using Domain.Dtos;
using Domain.ViewModels;

namespace Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioDto> BuscarUsuario(string username, int usuarioId);
        Task<UsuarioDto> BuscarUsuarioCompleto(int usuarioId);
        Task<UsuarioSimplificadoDto> BuscaSimplificada();
        Task<int> CriarUsuario(UsuarioDto usuario);
        Task<List<AcessoDto>> BuscarAcessos();  
        Task<bool> CriarUsuarioAcesso(UsuarioDto usuario);
        Task<bool> DeletarUsuarioAcesso(int usuarioId);
        Task<bool> AlterarSenha(AlteracaoSenhaViewModel usuario);
        Task<bool> DeletarUsuario(int usuarioId);
        Task<bool> AtualizarUsuario(UsuarioDto usuario);
    }
}
