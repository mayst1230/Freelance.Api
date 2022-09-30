using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance.Core.Models.Storage;

/// <summary>
/// Представление файла.
/// </summary>
public class File
{
    /// <summary>
    /// ИД.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Уникальный ИД.
    /// </summary>
    [Required]
    public Guid UniqueIdentifier { get; set; }

    /// <summary>
    /// Имя файла.
    /// </summary>
    [MaxLength(256)]
    [Required]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Описание.
    /// </summary>
    [MaxLength(256)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// MIME-тип.
    /// </summary>
    [MaxLength(256)]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Группа файлов.
    /// </summary>
    [Required]
    public FileGroupType GroupType { get; set; }

    /// <summary>
    /// Контент файла.
    /// </summary>
    [Required]
    public byte[] Data { get; set; } = null!;

    /// <summary>
    /// Кем создан файл.
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Дата и время загрузки файла.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Файл удален.
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Группа файлов.
/// </summary>
public enum FileGroupType
{
    /// <summary>
    /// Файл фото профиля.
    /// </summary>
    PhotoFile = 1,

    /// <summary>
    /// Файл исполнителя.
    /// </summary>
    ContractorFile = 2,

    /// <summary>
    /// Файл заказчика.
    /// </summary>
    CustomerFile = 3,
}
