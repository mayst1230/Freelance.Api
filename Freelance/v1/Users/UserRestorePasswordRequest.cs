using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users
{
    /// <summary>
    /// Запрос на восстановление пароля.
    /// </summary>
    public class UserRestorePasswordRequest
    {
        /// <summary>
        /// УИД пользователя.
        /// </summary>
        [Required]
        public Guid UserUniqueIdentifier { get; set; }

        /// <summary>
        /// Код с почты.
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Новый пароль.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
