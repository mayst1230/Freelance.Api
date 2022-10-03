using Freelance.Core.Models.Storage;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Files;

/// <summary>
/// Информация о файле.
/// </summary>
public class FileInfoResponse
{
    /// <summary>
    /// Имя файла.
    /// </summary>
    [Required]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Описание.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// MIME-тип.
    /// </summary>
    [Required]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла.
    /// </summary>
    [Required]
    public long Size { get; set; }

    /// <summary>
    /// Группа файлов.
    /// </summary>
    [Required]
    public FileGroupType GroupType { get; set; }

    /// <summary>
    /// Ссылка на скачивание файла.
    /// </summary>
    [Required]
    public string Url { get; set; } = string.Empty;
}
