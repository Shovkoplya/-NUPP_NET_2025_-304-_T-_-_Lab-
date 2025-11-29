using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Infrastructure.Models;
using Restaurant.REST.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurant.REST.Controllers
{
    /// <summary>
    /// Контролер для управління аутентифікацією та авторизацією користувачів
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        /// <param name="request">Дані для реєстрації</param>
        /// <returns>Інформація про створеного користувача</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Перевірка чи існує користувач з таким email
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Користувач з таким email вже існує"
                    });
                }

                // Перевірка чи існує користувач з таким ім'ям
                var existingUserByName = await _userManager.FindByNameAsync(request.UserName);
                if (existingUserByName != null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Користувач з таким ім'ям вже існує"
                    });
                }

                // Створення нового користувача
                var user = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    RegistrationDate = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Помилка при створенні користувача",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Призначення ролі Customer за замовчуванням
                await _userManager.AddToRoleAsync(user, "Customer");

                _logger.LogInformation("Новий користувач зареєстрований: {Email} з роллю Customer", user.Email);

                return Ok(new RegisterResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Message = "Користувач успішно зареєстрований"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при реєстрації користувача");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Внутрішня помилка сервера"
                });
            }
        }

        /// <summary>
        /// Вхід в систему
        /// </summary>
        /// <param name="request">Дані для входу</param>
        /// <returns>JWT токен при успішному вході</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Спроба знайти користувача за email або username
                var user = await _userManager.FindByEmailAsync(request.EmailOrUserName)
                    ?? await _userManager.FindByNameAsync(request.EmailOrUserName);

                if (user == null)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Невірний email/ім'я користувача або пароль"
                    });
                }

                // Перевірка чи активний користувач
                if (!user.IsActive)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Обліковий запис деактивовано"
                    });
                }

                // Перевірка пароля
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (!result.Succeeded)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Невірний email/ім'я користувача або пароль"
                    });
                }

                // Оновлення дати останнього входу
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Генерація JWT токену
                var token = await GenerateJwtToken(user);

                _logger.LogInformation("Користувач {Email} увійшов в систему", user.Email);

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при вході користувача");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Внутрішня помилка сервера"
                });
            }
        }

        /// <summary>
        /// Отримання інформації про поточного користувача
        /// </summary>
        /// <returns>Інформація про користувача</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Користувача не знайдено"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                user.RegistrationDate,
                user.LastLoginDate,
                user.IsActive,
                Roles = roles
            });
        }

        /// <summary>
        /// Зміна пароля
        /// </summary>
        /// <param name="request">Дані для зміни пароля</param>
        /// <returns>Результат операції</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Користувача не знайдено"
                    });
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Помилка при зміні пароля",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("Користувач {Email} змінив пароль", user.Email);

                return Ok(new { Message = "Пароль успішно змінено" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при зміні пароля");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Внутрішня помилка сервера"
                });
            }
        }

        /// <summary>
        /// Генерація JWT токену для користувача
        /// </summary>
        private async Task<LoginResponse> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Додавання ролей до claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Додавання додаткових даних
            if (!string.IsNullOrEmpty(user.FullName))
            {
                claims.Add(new Claim("FullName", user.FullName));
            }

            var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345678901234567890";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "RestaurantAPI",
                audience: _configuration["Jwt:Audience"] ?? "RestaurantAPIUsers",
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                AccessToken = tokenString,
                TokenType = "Bearer",
                ExpiresIn = expiresInMinutes * 60,
                Email = user.Email!,
                UserName = user.UserName!,
                Roles = roles.ToList()
            };
        }
    }
}

