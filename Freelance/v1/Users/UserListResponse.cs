using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Users;

/// <summary>
/// Ответ на запрос списка пользователей.
/// </summary>
public class UserListResponse
{
    /// <summary>
    /// Общее количество элементов.
    /// </summary>
    [Required]
    public int TotalCount { get; set; }

    /// <summary>
    /// Элементы ответа.
    /// </summary>
    [Required]
    public UserItem[] Items { get; set; } = Array.Empty<UserItem>();
}
