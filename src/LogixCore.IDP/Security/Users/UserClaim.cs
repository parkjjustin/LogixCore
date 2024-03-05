namespace Core.Users;

public sealed class UserClaim
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; } = default!;
    public string Type { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public byte[] RowVersion { get; private set; } = default!;
}