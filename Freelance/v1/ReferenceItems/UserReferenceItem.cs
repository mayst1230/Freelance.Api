using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.ReferenceItems;

/// <summary>
/// Элемент ссылки на пользователя.
/// </summary>
public class UserReferenceItem
{
    /// <summary>
    /// УИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    
    [Required]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Роль.
    /// </summary>
    [Required]
    public UserRole Role { get; set; }

    /// <summary>
    /// Рейтинг.
    /// </summary>
    [Required]
    public decimal Rating { get; set; }

    /// <summary>
    /// Файл с фото профиля.
    /// </summary>
    public FileReferenceItem? PhotoFile { get; set; }
}
