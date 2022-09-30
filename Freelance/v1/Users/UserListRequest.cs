using Freelance.Core.Models.Storage;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Запрос на список пользователей.
/// </summary>
public class UserListRequest
{
    /// <summary>
    /// Количество записей.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Отступ от начала списка.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// УИД пользователя.
    /// </summary>
    public Guid? UserUniqueIdentifier { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Почта.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Рейтинг.
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// Роль.
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Пользователь удален.
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Пользователь заблокирован.
    /// </summary>
    public bool? IsBanned { get; set; }
}
