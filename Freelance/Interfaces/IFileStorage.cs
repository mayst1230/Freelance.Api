using Freelance.Api.v1.Files;
using Freelance.Core.Models.Storage;

namespace Freelance.Api.Interfaces;

/// <summary>
/// Хранилище файлов.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Получение содержимого файла.
    /// </summary>
    /// <param name="fileUuid">Уникальный ИД файла.</param>
    /// <returns>Данные и mime-тип.</returns>
    Task<(Stream data, string mime)> GetContentAsync(Guid fileUuid);

    /// <summary>
    /// Получение информации о файле.
    /// </summary>
    /// <param name="fileUuid">Уникальный ИД файла.</param>
    /// <returns>Информацию о файле.</returns>
    Task<FileInfoResponse> GetDetailsAsync(Guid fileUuid);

    /// <summary>
    /// Загрузка файла на сервер.
    /// </summary>
    /// <param name="data">Поток данных.</param>
    /// <param name="contentType">Тип файла.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="fileGroup">Группа файлов.</param>
    /// <param name="displayName">Описание файла.</param>
    /// <param name="createdBy">Кем создан файл.</param>
    /// <returns>УИД загруженного файла.</returns>
    Task<int> UploadAsync(MemoryStream data, string contentType, string fileName, FileGroupType fileGroup, string? displayName = default, int? createdBy = default);
}
