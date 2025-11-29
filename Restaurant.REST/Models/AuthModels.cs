using System.ComponentModel.DataAnnotations;

namespace Restaurant.REST.Models
{
    /// <summary>
    /// DTO для реєстрації нового користувача
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Ім'я користувача (логін)
        /// </summary>
        [Required(ErrorMessage = "Ім'я користувача обов'язкове")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я користувача має бути від 3 до 50 символів")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email адреса
        /// </summary>
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обов'язковий")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути від 6 до 100 символів")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я користувача
        /// </summary>
        [StringLength(100, ErrorMessage = "Повне ім'я не може перевищувати 100 символів")]
        public string? FullName { get; set; }

        /// <summary>
        /// Номер телефону
        /// </summary>
        [Phone(ErrorMessage = "Невірний формат телефону")]
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO для входу в систему
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email або ім'я користувача
        /// </summary>
        [Required(ErrorMessage = "Email або ім'я користувача обов'язкове")]
        public string EmailOrUserName { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO відповіді при успішній аутентифікації
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT токен доступу
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Тип токену (Bearer)
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Час життя токену в секундах
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Email користувача
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Ім'я користувача
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Ролі користувача
        /// </summary>
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// DTO відповіді при успішній реєстрації
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// ID створеного користувача
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Email користувача
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Ім'я користувача
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Повідомлення про успішну реєстрацію
        /// </summary>
        public string Message { get; set; } = "Користувач успішно зареєстрований";
    }

    /// <summary>
    /// DTO для зміни пароля
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Поточний пароль
        /// </summary>
        [Required(ErrorMessage = "Поточний пароль обов'язковий")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Новий пароль
        /// </summary>
        [Required(ErrorMessage = "Новий пароль обов'язковий")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути від 6 до 100 символів")]
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO для відповіді про помилку
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Повідомлення про помилку
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Деталі помилки
        /// </summary>
        public List<string>? Errors { get; set; }
    }
}

