using Aidan.Core.Patterns;

namespace Webql.Tests.Models;

public class Account
{
    public Guid Id { get; }
    public string Nickname { get; private set; }
    public Email Email { get; private set; }

    public Account(
        Guid id, 
        string nickname, 
        Email email)
    {
        Id = id;
        Nickname = nickname;
        Email = email;
    }

    public static AccountBuilder Create()
    {
        return new AccountBuilder();
    }

    public override string ToString()
    {
        return $"Id: {Id}, Nickname: {Nickname}, Email: {Email}";
    }

    public void ChangeNickname(string nickname)
    {
        Nickname = nickname;
    }

    public void ChangeEmail(Email email)
    {
        Email = email;
    }
}


public class AccountBuilder : IBuilder<Account>
{
    private Guid? Id { get; set; } = Guid.NewGuid();
    private string? Nickname { get; set; }
    private Email? Email { get; set; }

    public AccountBuilder WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public AccountBuilder WithNickname(string nickname)
    {
        Nickname = nickname;
        return this;
    }

    public AccountBuilder WithEmail(Email email)
    {
        Email = email;
        return this;
    }

    public Account Build()
    {
        if (Id == null)
        {
            throw new InvalidOperationException("Id is required.");
        }

        if (Nickname == null)
        {
            throw new InvalidOperationException("Nickname is required.");
        }

        if (Email == null)
        {
            throw new InvalidOperationException("Email is required.");
        }

        return new Account(Id.Value, Nickname, Email);
    }

}