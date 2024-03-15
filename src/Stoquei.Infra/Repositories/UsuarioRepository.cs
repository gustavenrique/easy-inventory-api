using Dapper;
using Stoquei.Domain.Dtos;
using Stoquei.Domain.Enums;
using Stoquei.Domain.ViewModels;
using Stoquei.Infra.Interfaces;
using System.Data;

namespace Stoquei.Infra.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(string connectionString) : base(connectionString) { }

        public async Task<UsuarioSimplificadoDto> BuscaSimplificada()
        {
            var query = @"SELECT ID, Usuario, Email, Ativo, Admin FROM Usuario WITH(NOLOCK)
                          SELECT Id, Nome FROM Acesso WITH(NOLOCK)
                          SELECT UsuarioId, AcessoId FROM Usuario_Acesso WITH(NOLOCK)";

            return await MultipleQueryAsync<UsuarioSimplificadoDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var retorno = new UsuarioSimplificadoDto();

                    retorno.Usuarios = (await gridRetornado.ReadAsync<UsuarioDto>()).ToList();
                    retorno.Acessos = (await gridRetornado.ReadAsync<AcessoDto>()).ToList();
                    var usuarioAcessos = (await gridRetornado.ReadAsync<UsuarioAcessoDto>()).ToList();

                    retorno.Usuarios.ForEach(async u =>
                    {
                        u.Acessos = new List<Acesso>();
                        usuarioAcessos.ForEach(ua =>
                        {
                            if (ua.UsuarioId == u.Id) u.Acessos.Add((Acesso)ua.AcessoId);
                        });
                    });

                    return retorno;
                });
        }

        public async Task<UsuarioDto> BuscarUsuario(string username, int usuarioId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Usuario", username, DbType.String);
            parameters.Add("@UsuarioID", usuarioId, DbType.Int32);

            var query = @"SELECT ID, Usuario, Email, Senha, Ativo, Admin, AlterarSenha, UltimaAlteracaoSenha
                          FROM Usuario WITH(NOLOCK)
                          WHERE Usuario = @Usuario OR ID = @UsuarioID";

            return await QueryFirstOrDefaultAsync<UsuarioDto>(query, parameters, CommandType.Text);
        }

        public async Task<UsuarioDto> BuscarUsuarioCompleto(int usuarioId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioID", usuarioId, DbType.Int32);

            var query = @"SELECT ID, Usuario, Email, Senha, Ativo, Admin, AlterarSenha, UltimaAlteracaoSenha FROM Usuario WITH(NOLOCK) WHERE ID = @UsuarioID
                          SELECT UsuarioId, AcessoId FROM Usuario_Acesso WITH(NOLOCK) WHERE UsuarioId = @UsuarioID";

            return await MultipleQueryAsync<UsuarioDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var usuario = new UsuarioDto();

                    usuario = (await gridRetornado.ReadAsync<UsuarioDto>()).FirstOrDefault();
                    var usuarioAcessos = (await gridRetornado.ReadAsync<UsuarioAcessoDto>()).ToList();

                    usuario.Acessos = new List<Acesso>();
                    usuarioAcessos.ForEach(ua => usuario.Acessos.Add((Acesso)ua.AcessoId));

                    return usuario;
                }, param: parameters);
        }

        public async Task<UsuarioDto> BuscarUsuarioCompletoPorUsername(string username)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Username", username, DbType.String);

            var query = @"DECLARE @UsuarioID INT;
                          SELECT @UsuarioID = ID FROM Usuario WITH(NOLOCK) WHERE Usuario = @Username

                          SELECT ID, Usuario, Email, Senha, Ativo, Admin, AlterarSenha, UltimaAlteracaoSenha FROM Usuario WITH(NOLOCK) WHERE ID = @UsuarioID
                          SELECT UsuarioId, AcessoId FROM Usuario_Acesso WITH(NOLOCK) WHERE UsuarioId = @UsuarioID";

            return await MultipleQueryAsync<UsuarioDto>(query, commandType: CommandType.Text,
                retornoHandler: async gridRetornado =>
                {
                    var usuario = new UsuarioDto();

                    usuario = (await gridRetornado.ReadAsync<UsuarioDto>()).FirstOrDefault();
                    var usuarioAcessos = (await gridRetornado.ReadAsync<UsuarioAcessoDto>()).ToList();

                    usuario.Acessos = new List<Acesso>();
                    usuarioAcessos.ForEach(ua => usuario.Acessos.Add((Acesso)ua.AcessoId));

                    return usuario;
                }, param: parameters);
        }

        public async Task<int> CriarUsuario(UsuarioDto usuario)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Usuario", usuario.Usuario, DbType.String);
            parameters.Add("@Email", usuario.Email, DbType.String);
            parameters.Add("@Senha", usuario.SenhaCriptografada(), DbType.String);
            parameters.Add("@Ativo", usuario.Ativo, DbType.Boolean);
            parameters.Add("@Admin", usuario.Admin, DbType.Boolean);

            var query = @"INSERT INTO Usuario (Usuario, Email, Senha, Ativo, Admin, AlterarSenha)
                          OUTPUT INSERTED.Id
                          VALUES (@Usuario, @Email, @Senha, @Ativo, @Admin, 1)";

            return await QueryFirstOrDefaultAsync<int>(query, parameters, CommandType.Text);
        }

        public async Task<bool> AlterarSenha(AlteracaoSenhaViewModel usuario)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NovaSenha", usuario.SenhaNova, DbType.String);
            parameters.Add("@UsuarioId", usuario.UsuarioId, DbType.Int32);

            var query = "UPDATE Usuario SET Senha = @NovaSenha, UltimaAlteracaoSenha = GETDATE(), AlterarSenha = 0 WHERE ID = @UsuarioId";

            return await ExecuteAsync(query, parameters, CommandType.Text) > 0;
        }

        public async Task<bool> DeletarUsuario(int usuarioId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", usuarioId, DbType.Int64);

            var query = "DELETE FROM Usuario WHERE Id = @UsuarioId";

            return await ExecuteAsync(query, param: parameters, commandType: CommandType.Text) > 0;
        }

        public async Task<bool> AtualizarUsuario(UsuarioDto usuario)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", usuario.Id, DbType.Int64);
            parameters.Add("@Usuario", usuario.Usuario, DbType.String);
            parameters.Add("@Email", usuario.Email, DbType.String);
            parameters.Add("@Ativo", usuario.Ativo, DbType.Boolean);
            parameters.Add("@Admin", usuario.Admin, DbType.Boolean);

            var query = @"UPDATE Usuario SET Usuario = @Usuario, Email = @Email, Ativo = @Ativo, Admin = @Admin WHERE Id = @Id";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }

        #region Acesso e Usuario_Acesso
        public async Task<List<AcessoDto>> BuscarAcessos()
        {
            var query = @"SELECT Id, Nome FROM Acesso";

            return await QueryAsync<AcessoDto>(query, commandType: CommandType.Text);
        }

        public async Task<bool> CriarUsuarioAcesso(UsuarioDto usuario)
        {
            var parameters = new List<dynamic>();
            usuario.Acessos.ForEach(a => parameters.Add(new { UsuarioId = usuario.Id, AcessoId = a }));

            var query = @"INSERT INTO Usuario_Acesso (UsuarioId, AcessoId) VALUES (@UsuarioId, @AcessoId)";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }

        public async Task<bool> DeletarUsuarioAcesso(int usuarioId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioId", usuarioId, DbType.Int64);

            var query = @"DELETE FROM Usuario_Acesso WHERE UsuarioId = @UsuarioId";

            return (await ExecuteAsync(query, commandType: CommandType.Text, param: parameters)) > 0;
        }
        #endregion
    }
}
