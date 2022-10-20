using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api;

public sealed class BackgroundServiceQueue : Queue<Action>
{
}