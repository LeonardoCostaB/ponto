using System.Drawing;
using Ponto.Api.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string EmployeeId { get; set; }
    public required string companyId { get; set; }
    public required string pin { get; set; }

    public List<ClockEntity> Clocks { get; set; } = [];
}