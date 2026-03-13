using Dapper;
using HabitTracker.API.Application.Common;
using HabitTracker.API.Application.Common.Interfaces;
using MediatR;

namespace HabitTracker.API.Application.Habits.Queries;

public sealed record GetHabitByIdQuery(Guid Id) : IRequest<Result<HabitResponse>>;

public sealed record GetAllHabitsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsActive = null,
    string? SearchTerm = null) : IRequest<Result<PagedResponse<HabitSummaryResponse>>>;

public sealed class GetAllHabitsQueryHandler(
    IDapperContext dapperContext)
    : IRequestHandler<GetAllHabitsQuery, Result<PagedResponse<HabitSummaryResponse>>>
{
    public async Task<Result<PagedResponse<HabitSummaryResponse>>> Handle(
        GetAllHabitsQuery request,
        CancellationToken cancellationToken)
    {
        // Dynamic WHERE clause
        var conditions = new List<string>();

        if (request.IsActive.HasValue)
        {
            conditions.Add("h.IsActive = @IsActive");
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            conditions.Add("h.Name LIKE @SearchTerm");
        }

        var where = conditions.Count > 0
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : string.Empty;

        // Count query — total for pagination
        var countSql = $"""
            SELECT COUNT(1)
            FROM Habits h
            {where}
            """;

        // Data query — paginated with TotalCompletions
        var dataSql = $"""
            SELECT
                h.Id              AS {nameof(HabitSummaryResponse.Id)},
                h.Name            AS {nameof(HabitSummaryResponse.Name)},
                h.FrequencyDays   AS {nameof(HabitSummaryResponse.Frequency)},
                h.FrequencyTypeId AS {nameof(HabitSummaryResponse.FrequencyType)},
                h.IsActive        AS {nameof(HabitSummaryResponse.IsActive)},
                h.CreatedAt       AS {nameof(HabitSummaryResponse.CreatedAt)},
                COUNT(hl.Id)      AS {nameof(HabitSummaryResponse.TotalCompletions)}
            FROM Habits h
            LEFT JOIN HabitLogs hl ON hl.HabitId = h.Id
            {where}
            GROUP BY
                h.Id,
                h.Name,
                h.Frequency,
                h.FrequencyTypeId,
                h.IsActive,
                h.CreatedAt
            ORDER BY h.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

        var parameters = new
        {
            request.IsActive,
            SearchTerm = $"%{request.SearchTerm}%",
            Offset = (request.PageNumber - 1) * request.PageSize,
            request.PageSize
        };

        using var connection = dapperContext.CreateConnection();

        var totalCount = await connection.ExecuteScalarAsync<int>(
            countSql,
            parameters);

        var items = await connection.QueryAsync<HabitSummaryResponse>(
            dataSql,
            parameters);

        var paged = new PagedResponse<HabitSummaryResponse>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResponse<HabitSummaryResponse>>.Success(paged);
    }
}