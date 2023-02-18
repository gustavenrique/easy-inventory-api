using Application.Interfaces;
using Domain.Dtos;
using Domain.Enums;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ILogger<UsuariosService> _logger;
        private readonly IConfiguration _configuration;

        public UsuariosService(IUsuarioRepository repository, ILogger<UsuariosService> logger, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<MensagemBase<UsuarioSimplificadoDto>> BuscarTodos()
        {
            try
            {
                var buscaSimplificada = await _repository.BuscaSimplificada();

                if (buscaSimplificada.Usuarios?.Any() != true)
                    return new MensagemBase<UsuarioSimplificadoDto>(StatusCodes.Status404NotFound, "Não há usuários registrados.", buscaSimplificada);

                return new MensagemBase<UsuarioSimplificadoDto>(StatusCodes.Status200OK, "Usuários buscados com sucesso!", buscaSimplificada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<UsuarioSimplificadoDto>(StatusCodes.Status500InternalServerError, "Erro ao buscar os usuários.");
            }
        }
 
        public async Task<MensagemBase<UsuarioDto>> BuscarUsuario(string username, int usuarioId = 0)
        {
            try
            {
                if (usuarioId < 1 && string.IsNullOrEmpty(username)) return new MensagemBase<UsuarioDto>(StatusCodes.Status404NotFound, "Usuário inexistente.");

                var usuario = await _repository.BuscarUsuario(username, usuarioId);

                if (usuario == null) return new MensagemBase<UsuarioDto>(StatusCodes.Status404NotFound, "Usuário inexistente.");

                return new MensagemBase<UsuarioDto>(StatusCodes.Status200OK, "Usuário buscado com sucesso!", usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarUsuario - Erro ao tentar buscar o usuário. Exception: {ex}");
                return new MensagemBase<UsuarioDto>(StatusCodes.Status500InternalServerError, $"Erro ao buscar o usuário. UsuarioID: {usuarioId}");
            }
        }

        public async Task<MensagemBase<int>> CriarUsuario(UsuarioCriacaoViewModel usuarioRequest)
        {
            try
            {
                var usuarioDto = usuarioRequest.ParaDto();

                if (string.IsNullOrEmpty(usuarioDto.Usuario) || string.IsNullOrEmpty(usuarioDto.Email))
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Há campos a serem preenchidos.");

                var buscaSimplificada = await _repository.BuscaSimplificada();

                if (buscaSimplificada.Usuarios.Any(u => u.Usuario == usuarioRequest.Usuario))
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, $"O username {usuarioRequest.Usuario} já está em uso");

                if (buscaSimplificada.Usuarios.Any(u => u.Email == usuarioRequest.Email))
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, $"O email {usuarioRequest.Email} já está em uso");

                bool resultado = false;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var usuarioCriadoId = usuarioDto.Id = await _repository.CriarUsuario(usuarioDto);

                    if (usuarioDto.Admin)
                        usuarioDto.Acessos = Enum.GetValues(typeof(Acesso)).Cast<Acesso>().ToList();

                    var usuarioAcessoSucesso = usuarioDto.Acessos.Any() ? await _repository.CriarUsuarioAcesso(usuarioDto) : true;

                    resultado = usuarioCriadoId > 0 && usuarioAcessoSucesso;

                    transaction.Complete();
                }

                if (!resultado)
                {
                    _logger.LogError($"Usuarios - Post - Erro ao tentar criar usuário. Usuario: {usuarioRequest}");
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Ocorreu um erro ao tentar registrar o usuário.");
                }

                usuarioDto.EnviarEmailComSenha(_configuration["Email:SenderAddress"], _configuration["Email:SenderPassword"]);

                return new MensagemBase<int>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Usuário criado com sucesso!",
                    Object = usuarioDto.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<int>(StatusCodes.Status500InternalServerError, $"Erro ao criar o usuário. Usuario: {usuarioRequest}", 0);
            }
        }

        public async Task<MensagemBase<LoginResponseViewModel>> LogarUsuario(LoginViewModel usuarioRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(usuarioRequest.Usuario) || string.IsNullOrEmpty(usuarioRequest.Senha))
                    return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status400BadRequest, "Usuário ou senha não foram preenchidos.");

                var usuarioBanco = await _repository.BuscarUsuarioCompletoPorUsername(usuarioRequest.Usuario);

                var validacaoUsuario = ValidacaoUsuario(usuarioBanco, usuarioRequest.Senha);

                if (validacaoUsuario.Object == null) return validacaoUsuario;

                if (usuarioBanco.AlterarSenha)
                    return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status412PreconditionFailed, "Senha deve ser alterada.", new LoginResponseViewModel() { UsuarioId = usuarioBanco.Id });

                return new MensagemBase<LoginResponseViewModel>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login realizado com sucesso!",
                    Object = new LoginResponseViewModel()
                    {
                        UsuarioId = usuarioBanco.Id,
                        UsuarioAcessos = usuarioBanco.Acessos
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - LogarUsuario - Erro ao tentar logar o usuário. Exception: {ex}");
                return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status500InternalServerError, $"Erro ao logar o usuário. Request obj: {usuarioRequest}");
            }
        }

        private MensagemBase<LoginResponseViewModel> ValidacaoUsuario(UsuarioDto usuarioBanco, string usuarioRequestSenha)
        {
            if (usuarioBanco == null)
                return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status404NotFound, "Usuário inexistente.");

            if (usuarioBanco.Senha != usuarioRequestSenha)
                return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status400BadRequest, "Senha incorreta.");

            if (!usuarioBanco.Ativo)
                return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status400BadRequest, "Usuário inativo.");

            return new MensagemBase<LoginResponseViewModel> { Object =  new LoginResponseViewModel()};
        }

        public async Task<MensagemBase<LoginResponseViewModel>> AlterarSenha(AlteracaoSenhaViewModel model)
        {
            try
            {
                var usuarioBanco = await _repository.BuscarUsuarioCompleto(model.UsuarioId);

                var validacaoUsuario = ValidacaoUsuario(usuarioBanco, model.SenhaAtual);
                
                if (validacaoUsuario.Object == null) return validacaoUsuario;

                var sucesso = await _repository.AlterarSenha(model);

                if (!sucesso)
                    return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status500InternalServerError, "Ocorreu um erro na alteração de senha. Por favor, tente novamente.");

                return new MensagemBase<LoginResponseViewModel>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Senha alterada com sucesso!",
                    Object = new LoginResponseViewModel()
                    {
                        UsuarioId = usuarioBanco.Id,
                        UsuarioAcessos = usuarioBanco.Acessos
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - AlterarSenha - Erro ao tentar alterar senha. Exception: {ex}");
                return new MensagemBase<LoginResponseViewModel>(StatusCodes.Status500InternalServerError, $"Erro ao alterar a senha. Model: {model}");
            }
        }

        public async Task<MensagemBase<bool>> DeletarUsuario(int usuarioId)
        {
            try
            {
                var usuario = await _repository.BuscarUsuario("", usuarioId);

                if (usuario == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível deletar um usuário inexistente.", false);

                var resultado = await _repository.DeletarUsuario(usuarioId);

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Usuário deletado com sucesso!", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuario - DeletarUsuario - Erro ao tentar deletar o usuário com Id: {usuarioId}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao deletar o usuário.");
            }
        }

        public async Task<MensagemBase<bool>> AtualizarUsuario(UsuarioAlteracaoViewModel usuario)
        {
            try
            {
                var usuarioBanco = await _repository.BuscarUsuarioCompleto(usuario.Id);

                if (usuarioBanco == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível alterar um usuário inexistente.", false);

                var houveAlteracaoSimples = usuario.Usuario != usuarioBanco.Usuario || usuario.Email != usuarioBanco.Email || usuario.Ativo != usuarioBanco.Ativo || usuario.Admin != usuarioBanco.Admin;
                bool houveAlteracaoComplexa = !listaAcessoIgual(usuario.Acessos, usuarioBanco.Acessos);

                if (!houveAlteracaoComplexa && !houveAlteracaoSimples)
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Nenhuma propriedade foi alterada.", false);

                var sucesso = false;
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var usuarioDto = usuario.ParaDto();

                    if (houveAlteracaoSimples) sucesso = await _repository.AtualizarUsuario(usuarioDto);

                    if (houveAlteracaoComplexa) sucesso = sucesso && await _repository.DeletarUsuarioAcesso(usuarioDto.Id) && await _repository.CriarUsuarioAcesso(usuarioDto);

                    transaction.Complete();
                }

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Usuário atualizado com sucesso!", sucesso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuario - AtualizarUsuario - Erro ao tentar atualizar o usuário. Usuario: {usuario}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Ocorreu um erro inesperado ao atualizar o usuário.", false);
            }
        }

        private bool listaAcessoIgual(List<Acesso> list1, List<Acesso> list2)
        {
            var firstNotSecond = list1.Except(list2).ToList();
            var secondNotFirst = list2.Except(list1).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}
