namespace MailButler.Dtos;

public abstract class BaseResponse<T>
{
	public T Result { get; init; } = default!;
	public string Message { get; init; } = String.Empty;
	public Status Status { get; init; } = Status.Success;
}