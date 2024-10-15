namespace Webql.Tests.Models;

public class Email
{
    public string Username { get; }
    public string Domain { get; }
    public string Extension { get; }

    public Email(string username, string domain, string extension)
    {
        Username = username;
        Domain = domain;
        Extension = extension;
    }

    public Email(string email)
    {
        var parts = email.Split('@');

        if (parts.Length != 2)
            throw new ArgumentException("Invalid email address");

        var domainParts = parts[1].Split('.');

        if (domainParts.Length != 2)
            throw new ArgumentException("Invalid email address");

        Username = parts[0];
        Domain = domainParts[0];
        Extension = domainParts[1];
    }

    public override string ToString()
    {
        return $"{Username}@{Domain}.{Extension}";
    }
}