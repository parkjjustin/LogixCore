namespace LogixCore.IDP.Security;

public interface IRegisterUserModel
{
    string UserName { get; init; }
    string Email { get; init; }
    string Password { get; init; }
}

public record RegisterUserModel : IRegisterUserModel
{
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string ConfirmPassword { get; init; } = default!;
}
