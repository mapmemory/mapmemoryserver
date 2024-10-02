using System.Security.Claims;
using System.Text;
using Models;
using Microsoft.IdentityModel.Tokens;
using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using Isopoh.Cryptography.SecureArray;
using System.Text.Json;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Services;

public class AuthService
{
    private static SymmetricSecurityKey? securityKey;
    private static readonly string secret = "oepsko34okfso23ooj3pqo3pj21321";
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public AuthService()
    {
        securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    internal static bool ValidateToken(string token, ApplicationContext contextData)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token).ToString();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    {
                        if (expires != null)
                        {
                            return expires > DateTime.UtcNow;
                        }
                        return false;
                    }
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (principal.Identity!=null && principal.Identity.IsAuthenticated)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating token: {ex.Message}");
                // do nothing
            }

            Console.WriteLine("Failed to validate token");
            return false;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[16];
            Rng.GetBytes(salt);

            var config = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 10,
                MemoryCost = 32768,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
                Password = passwordBytes,
                Salt = salt,
                Secret = Encoding.UTF8.GetBytes(secret),
                HashLength = 20 
            };
            var argon2A = new Argon2(config);
            string hashString;
            using(SecureArray<byte> hashA = argon2A.Hash())
            {
                hashString = config.EncodeString(hashA.Buffer);
            }
            return hashString;
        }

        public static bool VerifyPassword(string password, string hash)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            return Argon2.Verify(hash, passwordBytes, Encoding.UTF8.GetBytes(secret), Environment.ProcessorCount);
        }
}