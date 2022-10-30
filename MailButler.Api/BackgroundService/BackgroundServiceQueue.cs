using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api.BackgroundService;

public sealed class BackgroundServiceQueue : Queue<Action>
{
}