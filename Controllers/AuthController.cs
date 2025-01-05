using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api_Tutorial.Data;
using Api_Tutorial.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Api_Tutorial.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DapperDataContext _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DapperDataContext(config);
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(RegistrationDto registrationDto)
        {
            if (registrationDto.Password == registrationDto.PasswordConfirmation)
            {

                string sqlCheckUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + registrationDto.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExist);

                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(registrationDto.Password, passwordSalt);

                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES ('" + registrationDto.Email + "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {

                        string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES(
                                '" + registrationDto.FirstName +
                                "', '" + registrationDto.LastName +
                                "', '" + registrationDto.Email +
                                "', '" + registrationDto.Gender +
                                "', 1)";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok(new { message = "User created successfully" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Failed to add user" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Failed to register user" });
                    }
                }

                return BadRequest(new { message = "Email already exists" });
            }
            else
            {
                return BadRequest(new { message = "Passwords do not match" });
            }

        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            string sqlForHashAndSalt = @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + loginDto.Email + "'";

            LoginConfirmationDto loginConfirmationDto = _dapper.LoadSingleData<LoginConfirmationDto>(sqlForHashAndSalt);

            if (loginConfirmationDto != null)
            {
                byte[] passwordHash = GetPasswordHash(loginDto.Password, loginConfirmationDto.PasswordSalt);

                // if (loginConfirmationDto.PasswordHash.SequenceEqual(passwordHash))

                for (int index = 0; index < passwordHash.Length; index++)
                {
                    if (passwordHash[index] != loginConfirmationDto.PasswordHash[index])
                    {
                        return BadRequest(new { message = "Login failed" });
                    }
                }

                string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + loginDto.Email + "'";

                int userId = _dapper.LoadSingleData<int>(userIdSql);

                return Ok(new { message = "Login successful", token = CreateToken(userId) });
            }

            return BadRequest(new { message = "Login failed" });
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {

            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }

        private string CreateToken(int userId)
        {
            Claim[] claims = [
                new Claim("userId", userId.ToString())
            ];

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials signingCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signingCredentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
