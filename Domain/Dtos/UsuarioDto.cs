namespace Domain.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public bool Admin { get; set; }
        public bool AlterarSenha { get; set; }
        public DateTime? UltimaAlteracaoSenha { get; set; }
        public List<int> Acessos { get; set; }
        public string SenhaCriptografada() => string.IsNullOrEmpty(Senha) ? string.Empty
            : BitConverter
                .ToString(new System.Security.Cryptography.SHA256Managed()
                .ComputeHash(System.Text.Encoding.UTF8.GetBytes(Senha)))
                .Replace("-", string.Empty).ToLower();

        public UsuarioDto(int id, string usuario, string senha, string email, bool ativo, bool admin, List<int> acessos, bool alterarSenha, DateTime ultimaAlteracaoSenha)
        {
            Id = id;
            Usuario = usuario;
            Email = email;
            Senha = senha;
            Ativo = ativo;
            Admin = admin;
            Acessos = acessos;
            AlterarSenha = alterarSenha;
            UltimaAlteracaoSenha = ultimaAlteracaoSenha;
        }

        public UsuarioDto(string usuario, string email, bool ativo, bool admin, List<int> acessos)
        {
            Usuario = usuario;
            Senha = GerarSenhaAleatoria();
            Email = email;
            Ativo = ativo;
            Admin = admin;
            Acessos = acessos;
        }

        public UsuarioDto(int id, string usuario)
        {
            Id = id;
            Usuario = usuario;
        }
        
        public UsuarioDto() { }

        private string GerarSenhaAleatoria()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
