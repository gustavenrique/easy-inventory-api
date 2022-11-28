using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<List<UsuarioDto>> BuscarUsuarios();
        Task<UsuarioDto> BuscarUsuario(string username, int usuarioId);
        Task<bool> CriarUsuario(UsuarioDto usuario);
    }
}
