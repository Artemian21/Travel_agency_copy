using Travel_agency.BLL.Abstractions;

namespace Travel_agency.BLL.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateHash(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool VerifyHash(string password, string hash) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}
