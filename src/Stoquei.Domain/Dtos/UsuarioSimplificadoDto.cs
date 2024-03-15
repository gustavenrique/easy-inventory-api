namespace Stoquei.Domain.Dtos
{
    public class UsuarioSimplificadoDto
    {
        public List<UsuarioDto> Usuarios { get; set; }
        public List<AcessoDto> Acessos { get; set; }
    }
}
