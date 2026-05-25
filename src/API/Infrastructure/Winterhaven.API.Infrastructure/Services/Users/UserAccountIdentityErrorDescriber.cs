namespace Winterhaven.API.Infrastructure.Services.Users;

using Microsoft.AspNetCore.Identity;

internal sealed class UserAccountIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
    {
        return new()
        {
            Code = "Error",
            Description = "Something went wrong on our end. Please try again in a moment."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new()
        {
            Code = "Email Address",
            Description = $"'{email}' is already registered to an account."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new()
        {
            Code = "Username",
            Description = $"'{userName}' is already taken. Please choose another."
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new()
        {
            Code = "Email Address",
            Description = $"'{email}' is not a valid email address."
        };
    }

    public override IdentityError InvalidUserName(string? userName)
    {
        return new()
        {
            Code = "Username",
            Description = $"'{userName}' is not a valid username.",
        };
    }

    public override IdentityError PasswordMismatch()
    {
        return new()
        {
            Code = "Password",
            Description = "The password you entered is incorrect."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new()
        {
            Code = "Password",
            Description = "Your password must contain at least one number (0–9)."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new()
        {
            Code = "Password",
            Description = "Your password must contain at least one lowercase letter (a–z)."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new()
        {
            Code = "Password",
            Description = "Your password must contain at least one special character (e.g. !, @, #, $)."
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new()
        {
            Code = "Password",
            Description = $"Your password must contain at least {uniqueChars} unique character{(uniqueChars == 1 ? "" : "s")}."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new()
        {
            Code = "Password",
            Description = "Your password must contain at least one uppercase letter (A–Z)."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new()
        {
            Code = "Password",
            Description = $"Your password must be at least {length} characters long."
        };
    }
}