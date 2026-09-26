namespace Ponto.Api.Domain.Entities;

public class ClockEntity
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public required DateTime registeredAt { get; set; }
    public bool manuallyRegistered { get; set; } = false;
}