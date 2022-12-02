using Dapper;
using Domain.Dtos;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(string connectionString) : base(connectionString) { }

        public async Task<List<UsuarioDto>> BuscarUsuarios()
        {
            var query = @"SELECT ID, Usuario, Senha, Ativo, Admin, Acessos FROM Usuario WITH(NOLOCK)";

            return await QueryAsync<UsuarioDto>(query, commandType: CommandType.Text);
        }

        public async Task<UsuarioDto> BuscarUsuario(string username, int usuarioId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Usuario", username, DbType.String);
            parameters.Add("@UsuarioID", usuarioId, DbType.Int32);

            var query = @"SELECT ID, Usuario, Senha, Ativo, Admin, Acessos 
                          FROM Usuario WITH(NOLOCK)
                          WHERE Usuario = @Usuario OR ID = @UsuarioID";

            return await QueryFirstOrDefaultAsync<UsuarioDto>(query, parameters, CommandType.Text);
        }


        public async Task<bool> CriarUsuario(UsuarioDto usuario)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Usuario", usuario.Usuario, DbType.String);
            parameters.Add("@Senha", usuario.Senha, DbType.String);
            parameters.Add("@Ativo", usuario.Ativo, DbType.Boolean);
            parameters.Add("@Admin", usuario.Admin, DbType.Boolean);
            parameters.Add("@Acessos", usuario.Acessos, DbType.String);

            var query = @"INSERT INTO Usuario (Usuario, Senha, Ativo, Admin, Acessos)
                          VALUES (@Usuario, @Senha, @Ativo, @Admin, @Acessos)";

            return (await ExecuteAsync(query, parameters, CommandType.Text) > 0);
        }
    }
}
