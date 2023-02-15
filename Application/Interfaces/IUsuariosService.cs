using Domain.Dtos;
using Domain.ViewModels;

namespace Application.Interfaces
{
    public interface IUsuariosService
    {
        Task<MensagemBase<UsuarioSimplificadoDto>> BuscarTodos();
        Task<MensagemBase<UsuarioDto>> BuscarUsuario(string username, int usuarioId = 0);
        Task<MensagemBase<int>> CriarUsuario(UsuarioCriacaoViewModel usuarioRequest);
        Task<MensagemBase<LoginResponseViewModel>> LogarUsuario(LoginViewModel usuario);
        Task<MensagemBase<LoginResponseViewModel>> AlterarSenha(AlteracaoSenhaViewModel model);
        Task<MensagemBase<bool>> DeletarUsuario(int usuarioId);
        Task<MensagemBase<bool>> AtualizarUsuario(UsuarioAlteracaoViewModel usuario);
    }
}
