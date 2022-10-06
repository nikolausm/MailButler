namespace MailButler.Dtos;

public abstract class BaseResponse<T>
{
	public T Result { get; init; } = default!;
	public string Message { get; init; } = string.Empty;
	public Status Status { get; init; } = Status.Success;
}