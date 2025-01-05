using System.Data;
using System.Security.Cryptography;
using System.Text;
using Api_Tutorial.Data;
using Api_Tutorial.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

                    string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: registrationDto.Password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                    );

                    string sqlAddAuth = "INSERT INTO TutorialAppSchema.Auth (Email, PasswordHash, PasswordSalt) VALUES ('" + registrationDto.Email + "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        return Ok(new { message = "User created successfully" });
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
            User user = _dapper.GetUserByEmail(loginDto.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            string token = GenerateJwtToken(user);

            return Ok(new { token });
        }
    }
}
