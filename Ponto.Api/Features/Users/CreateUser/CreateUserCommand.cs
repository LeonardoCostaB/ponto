

using MediatR;
using Ponto.Api.Infra.Results;

public class CreateUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}
public class CreateUserCommand : IRequest<Result<CreateUserResponse>>
{
    public string Email { get; set; }
    public string EmployeeId { get; set; }
    public string CompanyId { get; set; }
    public string Pin { get; set; }
}