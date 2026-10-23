using MediatR;
using Microsoft.EntityFrameworkCore;
using Ponto.Infra.Database;
using Ponto.Infra.Results;

namespace Ponto.Api.Features.Users.ListUsers;

public class ListUsersHandler : IRequestHandler<ListUsersQuery, Result<List<ListUsersResponse>>>
{
    private readonly AppDbContext _dbContext;

    public ListUsersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ListUsersResponse>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .Select(user => new ListUsersResponse
            {
                Id = user.Id,
                Email = user.Email,
                EmployeeId = user.EmployeeId,
                CompanyId = user.companyId
            })
            .ToListAsync(cancellationToken);

        return Result<List<ListUsersResponse>>.Ok(users);
    }
}
