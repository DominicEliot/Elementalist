using Discord;

namespace Elementalist.Shared;
public class Result<TResult>
{
    private readonly TResult? _value;

    public TResult Value => _value ?? throw new InvalidOperationException($"Make sure to check if there's an error before accessing the {nameof(Value)} property");
    public Error? Error { get; }

    public bool IsError() => Error != null;
    public bool IsValid() => Value != null;

    public Result(Error error)
    {
        Error = error;
    }

    public Result(TResult value)
    {
        _value = value;
    }
}

public class Error
{
    public Error() { Message = "An error occoured"; }
    public Error(string message) { Message = message; }
    public string Message { get; }
}

public class NotFoundError() : Error("The requested record could not be found")
{
}
