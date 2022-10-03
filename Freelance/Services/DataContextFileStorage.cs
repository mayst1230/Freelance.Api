using Freelance.Api.Exceptions;
using Freelance.Api.Interfaces;
using Freelance.Api.v1.Files;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace Freelance.Api.Services;

/// <summary>
/// Хранилище файлов в БД.
/// </summary>
public class DataContextFileStorage : IFileStorage
{
    private readonly DataContext _dataContext;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor;

    public DataContextFileStorage(
        DataContext dataContext,
        IUrlHelperFactory urlHelperFactory,
        IActionContextAccessor actionContextAccessor)
    {
        _dataContext = dataContext;
        _urlHelperFactory = urlHelperFactory;
        _actionContextAccessor = actionContextAccessor;
    }

    /// <summary>
    /// Получение содержимого файла.
    /// </summary>
    /// <param name="fileUuid">Уникальный ИД файла.</param>
    /// <returns>Содержимое файла.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<(Stream data, string mime)> GetContentAsync(Guid fileUuid)
    {
        var file = await _dataContext.Files.FirstOrDefaultAsync(i => i.UniqueIdentifier == fileUuid);

        if (file == null)
        {
            throw new FileNotFoundException();
        }

        Stream data = new MemoryStream(file.Data);
        return (data, file.MimeType);
    }

    /// <summary>
    /// Получение информации о файле.
    /// </summary>
    /// <param name="fileUuid">Уникальный ИД файла.</param>
    /// <returns>Информация о файле.</returns>
    public async Task<FileInfoResponse> GetDetailsAsync(Guid fileUuid)
    {
        var file = await _dataContext.Files.FirstOrDefaultAsync(i => i.UniqueIdentifier == fileUuid);

        if (file == null)
        {
            throw new FileNotFoundException();
        }

        return new FileInfoResponse()
        {
            FileName = file.FileName,
            DisplayName = file.DisplayName,
            MimeType = file.MimeType,
            Size = file.Size,
            GroupType = file.GroupType,
            Url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext!).Action("Download", "Files", new { fileUuid = file.UniqueIdentifier }) ?? throw new ApiException("Не удалось сформировать ссылку на файл.")
        };
    }

    /// <summary>
    /// Загрузка файла на сервер.
    /// </summary>
    /// <param name="data">Данные файла.</param>
    /// <param name="contentType">MIME-тип</param>
    /// <param name="fileName">Название файла.</param>
    /// <param name="fileGroup">Группа файла.</param>
    /// <param name="displayName">Описание файла.</param>
    /// <param name="createdBy">Кем создан файл.</param>
    /// <returns>УИД загруженного файла.</returns>
    public async Task<int> UploadAsync(MemoryStream data, string contentType, string fileName, FileGroupType fileGroup, string? displayName = default, int? createdBy = default)
    {
        var dbFile = new Core.Models.Storage.File()
        {
            FileName = fileName,
            DisplayName = displayName,
            MimeType = contentType,
            Created = DateTimeOffset.Now.ToUniversalTime(),
            Size = data.Length,
            Data = data.ToArray(),
            GroupType = fileGroup,
            CreatedBy = createdBy,
        };

        await _dataContext.Files.AddAsync(dbFile);
        await _dataContext.SaveChangesAsync();

        return dbFile.Id;
    }
}
