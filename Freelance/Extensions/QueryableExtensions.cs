namespace Freelance.Api.Extensions;

/// <summary>
/// Методы расширения для Queryable.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Пагинация списка с проверками limit и offset.
    /// </summary>
    /// <typeparam name="T">Тип элементов списка.</typeparam>
    /// <param name="list">Список, для которого требуется пагинация.</param>
    /// <param name="limit">Количество записей, возвращаемое в запросе.</param>
    /// <param name="offset">Количество записей для пропуска.</param>
    /// <returns>Пагигинированный список.</returns>
    public static IQueryable<T> LimitOffset<T>(this IQueryable<T> list, int? limit, int? offset)
    {
        if (!limit.HasValue)
            limit = 10;
        if (limit < 5)
            limit = 5;
        if (limit > 500)
            limit = 1000;
        if (!offset.HasValue || offset < 0)
            offset = 0;

        return list.Skip(offset.Value).Take(limit.Value);
    }
}
