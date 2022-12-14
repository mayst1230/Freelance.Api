using Freelance.Api.Exceptions;
using Freelance.Api.Extensions;
using Freelance.Api.Interfaces;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Files
{
    /// <summary>
    /// Работа с файлами.
    /// </summary>
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IFileStorage _fileStorage;

        public FilesController(
            DataContext dataContext,
            IFileStorage fileStorage)
        {
            _dataContext = dataContext;
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Cкачивание файла по его уникальному ИД.
        /// </summary>
        /// <param name="fileUuid">Уникальный ИД файла.</param>
        [HttpGet("{fileUuid}")]
        public async Task<ActionResult> DownloadAsync([FromRoute] Guid fileUuid)
        {
            try
            {
                var info = await _fileStorage.GetDetailsAsync(fileUuid);
                (Stream data, string mime) = await _fileStorage.GetContentAsync(fileUuid);
                return File(data, mime, info.FileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Удаление файла по его УИД.
        /// </summary>
        /// <param name="fileUuid">УИД файла.</param>
        /// <returns></returns>
        [HttpDelete("{fileUuid}")]
        public async Task DeleteAsync([FromRoute] Guid fileUuid)
        {
            var file = await _dataContext.Files.Where(i => i.UniqueIdentifier == fileUuid).FirstOrDefaultAsync() 
                ?? throw new FileNotFoundException();

            file.IsDeleted = true;
            await _dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Загрузка файла.
        /// </summary>
        /// <param name="formFile">Файл для загрузки.</param>
        /// <param name="displayName">Описание файла.</param>
        /// <param name="fileGroup">Группа файлов.</param>
        /// <returns>ИД загруженного файла.</returns>
        [HttpPost("{fileGroup}")]
        [DisableRequestSizeLimit]
        public async Task<int> UploadAsync([Required] IFormFile formFile, [Required][FromRoute] FileGroupType fileGroup, string? displayName = default)
        {
            try
            {
                var userId = User.GetUserId();
                using var fileStream = new MemoryStream();
                formFile.CopyTo(fileStream);

                return await _fileStorage.UploadAsync(fileStream, formFile.ContentType, formFile.FileName, fileGroup, displayName, userId);
            }
            catch (FileLoadException)
            {
                throw new ApiException("Ошибка при загрузке файла.");
            }
        }

        /// <summary>
        /// Получение информации о файле.
        /// </summary>
        /// <param name="fileUuid">Уникальный ИД файла.</param>
        /// <returns></returns>
        [HttpGet("{fileUuid}/details")]
        public async Task<ActionResult<FileInfoResponse>> DetailsAsync([FromRoute] Guid fileUuid)
        {
            try
            {
                return await _fileStorage.GetDetailsAsync(fileUuid);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
