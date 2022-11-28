namespace Domain.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public int CargoId { get; set; }

        public UsuarioDto(int id, string usuario, string senha, bool ativo, int cargoId)
        {
            Id = id;
            Usuario = usuario;
            Senha = criptografarSenha(senha);
            Ativo = ativo;
            CargoId = cargoId;
        }

        public UsuarioDto(int id, string usuario)
        {
            Id = id;
            Usuario = usuario;
        }

        public UsuarioDto() { }

        public string criptografarSenha(string senha) => string.IsNullOrEmpty(senha) ? string.Empty 
            : BitConverter
                .ToString(new System.Security.Cryptography.SHA256Managed()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha)))
                .Replace("-", string.Empty);
    }
}
