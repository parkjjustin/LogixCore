﻿using Isopoh.Cryptography.Argon2;

namespace LogixCore.Server.Security.Users;

public interface IRegisterUserModel
{
    string UserName { get; }
    string Email { get; }
    string Password { get; }
}

public interface IUserManager<TUser>
{
    Task RegisterUserAsync(TUser user, CancellationToken ct);
}

public class UserManager<TUser> : IUserManager<TUser> where TUser : IRegisterUserModel
{
    private readonly IUserStore<User> store;

    public UserManager(IUserStore<User> store)
    {
        this.store = store;
    }

    public async Task RegisterUserAsync(TUser user, CancellationToken ct)
    {
        var newUser = new User(user.UserName, user.Email, HashPassword(user.Password));
        await this.store.CreateUserAsync(newUser, ct);
    }

    private static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }
}