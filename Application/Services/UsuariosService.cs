using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ILogger<UsuariosService> _logger;

        public UsuariosService(IUsuarioRepository repository, ILogger<UsuariosService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<MensagemBase<List<UsuarioDto>>> BuscarTodos()
        {
            try
            {
                var usuarios = await _repository.BuscarUsuarios();

                if (usuarios?.Any() != true)
                    return new MensagemBase<List<UsuarioDto>>(StatusCodes.Status404NotFound, "Não há usuários registrados.");

                return new MensagemBase<List<UsuarioDto>>(StatusCodes.Status200OK, "Usuários buscados com sucesso!", usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<List<UsuarioDto>>(StatusCodes.Status500InternalServerError, "Erro ao buscar os usuários.");
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
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<UsuarioDto>(StatusCodes.Status500InternalServerError, $"Erro ao buscar o usuário. UsuarioID: {usuarioId}");
            }
        }

        public async Task<MensagemBase<bool>> CriarUsuario(UsuarioDto usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Usuario) || string.IsNullOrEmpty(usuario.Senha))
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Há campos a serem preenchidos.");

                var resultado = await _repository.CriarUsuario(usuario);

                if (!resultado)
                {
                    _logger.LogError($"Usuarios - Post - Erro ao tentar criar usuário. Usuario: {usuario}");
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Ocorreu um erro ao tentar registrar o usuário.");
                }

                return new MensagemBase<bool>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Usuário criado com sucesso!",
                    Object = resultado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao criar o usuário. Usuario: {usuario}", false);
            }
        }

        public async Task<MensagemBase<bool>> LogarUsuario(LoginViewModel usuarioRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(usuarioRequest.Usuario) || string.IsNullOrEmpty(usuarioRequest.Senha))
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Usuário ou senha não foram preenchidos.");

                var usuarioBanco = await _repository.BuscarUsuario(usuarioRequest.Usuario, 0);

                if (usuarioBanco == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Usuário inexistente.");

                if (usuarioBanco.Senha != usuarioRequest.Senha)
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Senha incorreta.");

                return new MensagemBase<bool>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login realizado com sucesso!",
                    Object = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Usuarios - BuscarTodos - Erro ao tentar buscar os usuários. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao criar o usuário. Usuario: {usuarioRequest}", false);
            }
        }
    }
}
