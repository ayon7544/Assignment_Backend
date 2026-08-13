namespace AssignmentSystem.API.DTOs.Responses;

public record PagedResponse<T>(
    List<T> Data,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);        