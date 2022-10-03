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
    /// <param name="userUuid">УИД пользователя.</param>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения о созданном отзыве.</returns>
    [HttpPost]
    public async Task<FeedbackCreateResponse> CreateAsync([FromQuery][Required] Guid userUuid, [FromBody] FeedbackCreateRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var feedbackUser = await _dataContext.Users.Where(i => i.UniqueIdentifier == userUuid).FirstOrDefaultAsync()
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

        _dataContext.Users.Update(feedbackUser);
        await _dataContext.SaveChangesAsync();

        return new FeedbackCreateResponse()
        {
            UniqueIdentifier = feedback.UniqueIdentifier,
            UserId = feedback.UserId,
            Text = feedback.Text,
            UserRating = feedback.UserRating,
            CreatedBy = feedback.CreatedBy,
            IsDeleted = feedback.IsDeleted,
            Created = feedback.Created,
            Updated = feedback.Updated,
        };
    }

    /// <summary>
    /// Изменение отзыва.
    /// </summary>
    /// <param name="feedbackUuid">УИД отзыва.</param>
    /// <param name="request">Данные запроса.</param>
    /// <returns>Сведения об измененном отзыве.</returns>
    [HttpPut("{uuid}")]
    public async Task<FeedbackEditResponse> EditAsync([FromQuery][Required] Guid feedbackUuid, [FromBody] FeedbackEditRequest request)
    {
        if (!ModelState.IsValid)
            throw new ApiException();

        var userId = User.GetUserId() ?? throw new InvalidOperationException();
        var feedback = await _dataContext.Feedbacks.Where(i => i.UniqueIdentifier == feedbackUuid).FirstOrDefaultAsync()
            ?? throw new ApiException("Отзыв не найден.");

        if (feedback.CreatedBy != userId)
            throw new ApiForbidException("Редактировать можно только собственные отзывы.");

        feedback.UserRating = request.UserRating;
        feedback.Text = request.Text;
        feedback.Updated = DateTimeOffset.UtcNow;

        _dataContext.Feedbacks.Update(feedback);
        await _dataContext.SaveChangesAsync();

        var feedbackUser = await _dataContext.Users.Where(i => i.Id == feedback.UserId).FirstOrDefaultAsync()
            ?? throw new ApiException("Пользователь не найден.");

        feedbackUser.Rating = await CalculateRatingSum(feedbackUser.Id);
        feedbackUser.Updated = DateTimeOffset.UtcNow;

        _dataContext.Users.Update(feedbackUser);
        await _dataContext.SaveChangesAsync();

        return new FeedbackEditResponse()
        {
            UniqueIdentifier = feedback.UniqueIdentifier,
            UserId = feedback.UserId,
            Text = feedback.Text,
            UserRating = feedback.UserRating,
            CreatedBy = feedback.CreatedBy,
            IsDeleted = feedback.IsDeleted,
            Created = feedback.Created,
            Updated = feedback.Updated,
        };
    }

    /// <summary>
    /// Получение общего рейтинга пользователя.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<decimal> CalculateRatingSum(int userId)
    {
        var userRatingSum = await _dataContext.Feedbacks.Where(i => i.UserId == userId).Select(i => i.UserRating).SumAsync();
        var userRatingCount = await _dataContext.Feedbacks.Where(i => i.UserId == userId).Select(i => i.UserRating).CountAsync();
        return userRatingSum / userRatingCount;
    }
}
