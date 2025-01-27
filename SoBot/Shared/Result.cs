using Discord;

namespace SorceryBot.Shared;
public class Result<TResult>
{
    public TResult? Value { get; }
    public Error? Error { get; }

    public bool IsError() => Error != null;
    public bool IsValid() => Value != null;
}

public class Error
{
}
