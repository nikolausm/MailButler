namespace MailButler.Dtos;

public abstract class BaseResponse<T>
{
	public T Result { get; init; } = default!;
	public string Message { get; init; } = "OK";
	public Status Status { get; init; } = Status.Success;
}