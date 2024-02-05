using MailButler.Api.BackgroundService;
using MailButler.Dtos;
using Microsoft.AspNetCore.Mvc;
using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ActionsController : ControllerBase
{
	private readonly BackgroundServiceQueue _backgroundServiceQueue;
	private readonly BackgroundServiceWorker _backgroundServiceWorker;
	private readonly ILogger<ActionsController> _logger;

	public ActionsController(
		ILogger<ActionsController> logger,
		BackgroundServiceQueue backgroundServiceQueue,
		BackgroundServiceWorker backgroundServiceWorker
	)
	{
		_logger = logger;
		_backgroundServiceQueue = backgroundServiceQueue;
		_backgroundServiceWorker = backgroundServiceWorker;
	}


	[HttpGet]
	[ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult GetAsync(Action action, CancellationToken cancellationToken)
	{
		using (var _ = _logger.BeginScope($"{action.ToString()} =>"))
		{
			IActionResult result;
			switch (action)
			{
				case Action.All:
					Enum.GetValues<Action>()
						.Where(item =>
							item != Action.Unknown
							&& item != Action.CurrentAction
							&& item != Action.All
						)
						.ToList().ForEach(item =>
						{
							_logger.LogInformation("Adding {Action} to queue", item);
							_backgroundServiceQueue.Enqueue(item);
						});
					result = Ok("Started");
					break;
				case Action.CurrentAction:
					return Ok(_backgroundServiceWorker.Started == DateTime.MinValue
						? "No Job running"
						: $"Currently running: {_backgroundServiceWorker.Action} {(DateTime.Now - _backgroundServiceWorker.Started).TotalSeconds} seconds");

				case Action.CheckAccounts:
				case Action.AmazonOrderSummary:
				case Action.DeleteFromKnownSender:
				case Action.MarkOldEmailAsRead:
				case Action.ForwardInvoicesToGetMyInvoices:
					_logger.LogInformation("Starting");
					_backgroundServiceQueue.Enqueue(action);
					result = Ok("Started");
					break;
				case Action.Unknown:
				default:
					result = BadRequest($"Unknown action {action}");
					break;
			}

			_logger.LogInformation("Finished");
			return result;
		}
	}
}