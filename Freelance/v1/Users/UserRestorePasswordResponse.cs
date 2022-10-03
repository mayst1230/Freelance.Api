using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users
{
    /// <summary>
    /// Ответ на запрос для восстановления пароля.
    /// </summary>
    public class UserRestorePasswordResponse
    {
        /// <summary>
        /// УИД пользователя.
        /// </summary>
        [Required]
        public Guid UserUniqueIdentifier { get; set; }

        /// <summary>
        /// Успешное выполнение запроса.
        /// </summary>
        [Required]
        public bool IsSuccess { get; set; }
    }
}
