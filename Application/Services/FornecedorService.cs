using Application.Interfaces;
using Domain.Dtos;
using Domain.ViewModels;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Application.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly ILogger<FornecedorService> _logger;
        private readonly IFornecedorRepository _repository;

        public FornecedorService(ILogger<FornecedorService> logger, IFornecedorRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<MensagemBase<List<FornecedorDto>>> BuscarTodos()
        {
            try
            {
                var fornecedores = await _repository.BuscarFornecedores();

                if (fornecedores?.Any() == false)
                    return new MensagemBase<List<FornecedorDto>>(StatusCodes.Status404NotFound, "Não há fornecedores registrados.");

                return new MensagemBase<List<FornecedorDto>>(StatusCodes.Status200OK, "Fornecedores buscados com sucesso!", fornecedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fornecedor - BuscarTodos - Erro ao tentar buscar os fornecedores. Exception: {ex}");
                return new MensagemBase<List<FornecedorDto>>(StatusCodes.Status500InternalServerError, "Erro ao buscar os fornecedores.");
            }
        }

        public async Task<MensagemBase<int>> CriarFornecedor(FornecedorDto fornecedor)
        {
            try
            {
                if (string.IsNullOrEmpty(fornecedor.Nome) || string.IsNullOrEmpty(fornecedor.Email) || string.IsNullOrEmpty(fornecedor.Telefone))
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Há campos a serem preenchidos.");

                var validacaoTelefone = Regex.IsMatch(fornecedor.Telefone, @"^\+[0-9]{2} \([1-9]{2}\) (?:[2-8]|9[1-9])[0-9]{3}\-[0-9]{4}$");

                if (!validacaoTelefone)
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Número de telefone fora do padrão aceito.");

                var fornecedorCriadoId = await _repository.CriarFornecedor(fornecedor);

                if (fornecedorCriadoId <= 0)
                {
                    _logger.LogError($"Fornecedor - Post - Erro ao tentar criar fornecedor. Fornecedor: {fornecedor}");
                    return new MensagemBase<int>(StatusCodes.Status400BadRequest, "Ocorreu um erro ao tentar registrar o fornecedor.");
                }

                return new MensagemBase<int>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Fornecedor criado com sucesso!",
                    Object = fornecedorCriadoId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fornecedor - Post - Erro ao tentar buscar os fornecedor. Exception: {ex}");
                return new MensagemBase<int>(StatusCodes.Status500InternalServerError, $"Erro ao criar o fornecedor. Fornecedor: {fornecedor}");
            }
        }

        public async Task<MensagemBase<bool>> DeletarFornecedor(int fornecedorId)
        {
            try
            {
                var fornecedor = await _repository.BuscarFornecedor(fornecedorId);

                if (fornecedor == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível deletar um fornecedor inexistente.", false);

                var resultado = await _repository.DeletarFornecedor(fornecedorId);

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Fornecedor deletado com sucesso!", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fornecedor - DeletarFornecedor - Erro ao tentar deletar o fornecedor com Id: {fornecedorId}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Erro ao deletar o fornecedor.");
            }
        }

        public async Task<MensagemBase<bool>> AtualizarFornecedor(FornecedorDto fornecedor)
        {
            try
            {
                var fornecedorDto = await _repository.BuscarFornecedor(fornecedor.Id);

                if (fornecedorDto == null)
                    return new MensagemBase<bool>(StatusCodes.Status404NotFound, "Não é possível alterar um fornecedor inexistente.", false);

                var houveAlteracao = fornecedor.Nome != fornecedorDto.Nome || fornecedor.Email != fornecedorDto.Email || fornecedor.Telefone != fornecedorDto.Telefone;

                if (!houveAlteracao)
                    return new MensagemBase<bool>(StatusCodes.Status400BadRequest, "Nenhuma propriedade foi alterada.", false);

                var sucesso = await _repository.AtualizarFornecedor(fornecedor);

                return new MensagemBase<bool>(StatusCodes.Status204NoContent, "Fornecedor atualizado com sucesso!", sucesso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fornecedor - AtualizarFornecedor - Erro ao tentar atualizar o fornecedor. Fornecedor: {fornecedor}. Exception: {ex}");
                return new MensagemBase<bool>(StatusCodes.Status500InternalServerError, $"Ocorreu um erro inesperado ao atualizar o fornecedor.", false);
            }
        }
    }
}
