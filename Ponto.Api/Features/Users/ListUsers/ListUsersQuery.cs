using MediatR;
using Ponto.Infra.Results;

namespace Ponto.Api.Features.Users.ListUsers;

public class ListUsersResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string EmployeeId { get; set; }
    public required string CompanyId { get; set; }
}

public record ListUsersQuery : IRequest<Result<List<ListUsersResponse>>>;
