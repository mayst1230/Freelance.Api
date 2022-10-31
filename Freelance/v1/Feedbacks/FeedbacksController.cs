using Freelance.Api.Exceptions;
using Freelance.Api.Extensions;
using Freelance.Api.v1.ReferenceItems;
using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Freelance.Api.v1.Feedbacks;

/// <summary>
/// Работа с отзывами пользователей.
/// </summary>
[Route("v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class FeedbacksController : ControllerBase
{
    private readonly DataContext _dataContext;

    public FeedbacksController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    /// <summary>
    /// Список отзывов.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Список отзывов.</returns>
    [HttpGet]
    public async Task<FeedbackListResponse> ListAsync([FromQuery] FeedbackListRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var result = new FeedbackListResponse();

        var feedbacks = _dataContext.Feedbacks.Where(i => !i.IsDeleted)
                                              .OrderBy(i => i.Created)
                                              .AsQueryable();

        if (request.UserUniqueIdentifier.HasValue)
        {
            var userId = await _dataContext.Users.Where(i => !i.IsDeleted
                                                             && !i.IsBanned
                                                             && i.UniqueIdentifier == request.UserUniqueIdentifier.Value)
                                                 .Select(i => i.Id)
                                                 .FirstOrDefaultAsync();

            feedbacks = feedbacks.Where(i => i.UserId == userId);
        }
        else
        {
            feedbacks = feedbacks.Where(i => i.UserId == User.GetUserId());
        }

        result.TotalCount = await feedbacks.CountAsync();
        result.Items = await feedbacks.LimitOffset(request.Limit, request.Offset).Select(i => new FeedbackItem()
        {
            UniqueIdentifier = i.UniqueIdentifier,
            UserFeedback = new UserReferenceItem()
            {
                UniqueIdentifier = i.User.UniqueIdentifier,
                UserName = i.User.UserName,
                Role = i.User.Role,
                Rating = i.User.Rating,
                PhotoFile = i.User.PhotoFile != default ? new FileReferenceItem()
                {
                    UniqueIdentifier = i.User.PhotoFile.UniqueIdentifier,
                    FileName = i.User.PhotoFile.FileName,
                    DisplayName = i.User.PhotoFile.DisplayName,
                    GroupType = i.User.PhotoFile.GroupType,
                    MimeType = i.User.PhotoFile.MimeType,
                    Size = i.User.PhotoFile.Size,
                    Url = Url.Action("Download", "Files", new { fileUuid = i.User.PhotoFile.UniqueIdentifier })!
                } : default,
            },
            CreatedBy = new UserReferenceItem()
            {
                UniqueIdentifier = i.CreatedByUser.UniqueIdentifier,
                UserName = i.CreatedByUser.UserName,
                Role = i.CreatedByUser.Role,
                Rating = i.CreatedByUser.Rating,
                PhotoFile = i.CreatedByUser.PhotoFile != default ? new FileReferenceItem()
                {
                    UniqueIdentifier = i.CreatedByUser.PhotoFile.UniqueIdentifier,
                    FileName = i.CreatedByUser.PhotoFile.FileName,
                    DisplayName = i.CreatedByUser.PhotoFile.DisplayName,
                    GroupType = i.CreatedByUser.PhotoFile.GroupType,
                    MimeType = i.CreatedByUser.PhotoFile.MimeType,
                    Size = i.CreatedByUser.PhotoFile.Size,
                    Url = Url.Action("Download", "Files", new { fileUuid = i.CreatedByUser.PhotoFile.UniqueIdentifier })!
                } : default,
            },
            Text = i.Text,
            Created = i.Created,
            Updated = i.Updated,
            UserRating = i.UserRating,
        }).ToArrayAsync();

        return result;
    }

    /// <summary>
    /// Создание отзыва.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения о созданном отзыве.</returns>
    [HttpPost("create")]
    public async Task<FeedbackItem> CreateAsync(FeedbackCreateRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var feedbackUser = await _dataContext.Users.Where(i => i.UniqueIdentifier == request.UserUniqueIdentifier).FirstOrDefaultAsync()
            ?? throw new ApiException("Пользователь не найден.");

        if (await _dataContext.Feedbacks.Where(i => i.CreatedBy == userId && i.UserId == feedbackUser.Id).AnyAsync())
            throw new ApiException("Отзыв об этом пользователе уже оставлен.");

        var feedback = new Feedback()
        {
            UserId = feedbackUser.Id,
            Text = request.Text,
            UserRating = request.UserRating,
            CreatedBy = userId,
            Created = DateTimeOffset.UtcNow,
            Updated = DateTimeOffset.UtcNow,
        };

        await _dataContext.Feedbacks.AddAsync(feedback);
        await _dataContext.SaveChangesAsync();

        feedbackUser.Rating = await CalculateRatingSum(feedbackUser.Id);
        feedbackUser.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        return await GetDbItemAsync(feedback);
    }

    /// <summary>
    /// Изменение отзыва.
    /// </summary>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения об измененном отзыве.</returns>
    [HttpPut("edit")]
    public async Task<FeedbackItem> EditAsync(FeedbackEditRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var feedback = await _dataContext.Feedbacks.Where(i => i.UniqueIdentifier == request.FeedbackUuid).FirstOrDefaultAsync()
            ?? throw new ApiException("Отзыв не найден.");

        if (feedback.CreatedBy != userId)
            throw new ApiForbidException("Редактировать можно только собственные отзывы.");

        feedback.UserRating = request.UserRating;
        feedback.Text = request.Text;
        feedback.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        var feedbackUser = await _dataContext.Users.Where(i => i.Id == feedback.UserId).FirstOrDefaultAsync()
            ?? throw new ApiException("Пользователь не найден.");

        feedbackUser.Rating = await CalculateRatingSum(feedbackUser.Id);
        feedbackUser.Updated = DateTimeOffset.UtcNow;

        await _dataContext.SaveChangesAsync();

        return await GetDbItemAsync(feedback);
    }

    /// <summary>
    /// Получение сведений об отзыве.
    /// </summary>
    /// <param name="uuid">УИД отзыва.</param>
    /// <returns>Сведения об отзыве.</returns>
    [HttpGet("{uuid}")]
    public async Task<FeedbackItem> DetailsAsync(Guid uuid)
    {
        var feedback = await _dataContext.Feedbacks.Where(i => i.UniqueIdentifier == uuid).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Отзыв не найден.");

        return await GetDbItemAsync(feedback);
    }

    /// <summary>
    /// Сведения об отзыве.
    /// </summary>
    /// <param name="item">Отзыв.</param>
    /// <returns>Сведения об отзыве.</returns>
    private async Task<FeedbackItem> GetDbItemAsync(Feedback item)
    {
        var userFeedback = await _dataContext.Users.Where(i => i.Id == item.UserId).Include(i => i.PhotoFile).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Пользователь, к которому относится отзыв не найден.");

        var createdByUser = await _dataContext.Users.Where(i => i.Id == item.CreatedBy).Include(i => i.PhotoFile).FirstOrDefaultAsync()
            ?? throw new ApiNotFoundException("Пользователь, создавший отзыв не найден.");

        return new FeedbackItem()
        {
            UniqueIdentifier = item.UniqueIdentifier,
            UserFeedback = new UserReferenceItem()
            {
                UniqueIdentifier = userFeedback.UniqueIdentifier,
                UserName = userFeedback.UserName,
                Rating = userFeedback.Rating,
                PhotoFile = userFeedback.PhotoFile != null ? new FileReferenceItem()
                {
                    UniqueIdentifier = userFeedback.PhotoFile.UniqueIdentifier,
                    FileName = userFeedback.PhotoFile.FileName,
                    DisplayName = userFeedback.PhotoFile.DisplayName,
                    GroupType = userFeedback.PhotoFile.GroupType,
                    MimeType = userFeedback.PhotoFile.MimeType,
                    Size = userFeedback.PhotoFile.Size,
                    Url = Url.Action("Download", "Files", new { fileUuid = userFeedback.PhotoFile.UniqueIdentifier })!
                } : default,
            },
            Text = item.Text,
            UserRating = item.UserRating,
            CreatedBy = new UserReferenceItem()
            {
                UniqueIdentifier = createdByUser.UniqueIdentifier,
                UserName = createdByUser.UserName,
                Rating = createdByUser.Rating,
                PhotoFile = createdByUser.PhotoFile != null ? new FileReferenceItem()
                {
                    UniqueIdentifier = createdByUser.PhotoFile.UniqueIdentifier,
                    FileName = createdByUser.PhotoFile.FileName,
                    DisplayName = createdByUser.PhotoFile.DisplayName,
                    GroupType = createdByUser.PhotoFile.GroupType,
                    MimeType = createdByUser.PhotoFile.MimeType,
                    Size = createdByUser.PhotoFile.Size,
                    Url = Url.Action("Download", "Files", new { fileUuid = createdByUser.PhotoFile.UniqueIdentifier })!
                } : default,
            },
            Created = item.Created,
            Updated = item.Updated,
        };
    }

    /// <summary>
    /// Получение общего рейтинга пользователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <returns>Общий рейтинг пользователя.</returns>
    private async Task<decimal> CalculateRatingSum(int userId)
    {
        var userRatingSum = await _dataContext.Feedbacks.Where(i => i.UserId == userId).Select(i => i.UserRating).SumAsync();
        var userRatingCount = await _dataContext.Feedbacks.Where(i => i.UserId == userId).Select(i => i.UserRating).CountAsync();
        return userRatingSum / userRatingCount;
    }
}
