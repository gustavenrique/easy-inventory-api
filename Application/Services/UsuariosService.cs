using Application.Interfaces;
using Domain.Dtos;
using Domain.Enums;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
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
                    return new MensagemBase<UsuarioSimplificadoDto>(StatusCodes.Status404NotFound, "Não há usuários registrados.");

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

        public async Task<MensagemBase<int>> LogarUsuario(LoginViewModel usuarioRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(usuarioRequest.Usuario) || string.IsNullOrEmpty(usuarioRequest.Senha))
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Usuário ou senha não foram preenchidos.");

                var usuarioBanco = await _repository.BuscarUsuario(usuarioRequest.Usuario, 0);

                if (usuarioBanco == null)
                    return new MensagemBase<int>(StatusCodes.Status404NotFound, "Usuário inexistente.");

                if (usuarioBanco.Senha != usuarioRequest.Senha)
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Senha incorreta.");

                if (usuarioBanco.AlterarSenha)
                    return new MensagemBase<int>(StatusCodes.Status412PreconditionFailed, "Senha deve ser alterada.", usuarioBanco.Id);

                return new MensagemBase<int>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login realizado com sucesso!",
                    Object = usuarioBanco.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - LogarUsuario - Erro ao tentar logar o usuário. Exception: {ex}");
                return new MensagemBase<int>(StatusCodes.Status500InternalServerError, $"Erro ao logar o usuário. Request obj: {usuarioRequest}");
            }
        }

        public async Task<MensagemBase<int>> AlterarSenha(AlteracaoSenhaViewModel model)
        {
            try
            {
                var usuarioBanco = await _repository.BuscarUsuario("", model.UsuarioId);

                if (usuarioBanco == null)
                    return new MensagemBase<int>(StatusCodes.Status404NotFound, "Usuário inexistente.");

                if (usuarioBanco.Senha != model.SenhaAtual)
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Senha incorreta.");

                var sucesso = await _repository.AlterarSenha(model);

                if (!sucesso)
                    return new MensagemBase<int>(StatusCodes.Status500InternalServerError, "Ocorreu um erro na alteração de senha. Por favor, tente novamente.");

                return new MensagemBase<int>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Senha alterada com sucesso!",
                    Object = usuarioBanco.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - AlterarSenha - Erro ao tentar alterar senha. Exception: {ex}");
                return new MensagemBase<int>(StatusCodes.Status500InternalServerError, $"Erro ao alterar a senha. Model: {model}");
            }
        }
    }
}
