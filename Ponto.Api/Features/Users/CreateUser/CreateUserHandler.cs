using MediatR;
using Ponto.Api.Infra.Database;
using Ponto.Api.Infra.Results;

namespace Ponto.Api.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly AppDbContext _dbContext;

    public CreateUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userFound = _dbContext.Users.FirstOrDefault(x => x.Email == request.Email);

        if (userFound is not null)
        {
            return Result<CreateUserResponse>.Fail(new ResultError("user_already_exists", "User already exists"));
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            EmployeeId = request.EmployeeId,
            companyId = request.CompanyId,
            pin = request.Pin
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CreateUserResponse>.Ok(new CreateUserResponse
        {
            Id = user.Id,
            Email = user.Email
        });
    }
}