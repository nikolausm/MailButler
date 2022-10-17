using MailButler.Api.Dtos;
using MailButler.Dtos;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.AspNetCore.Mvc;
using Action = MailButler.Api.Dtos.Action;

namespace MailButler.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ActionsController : ControllerBase
{
	private readonly ILogger<ActionsController> _logger;
	private readonly BackgroundServiceQueue _backgroundServiceQueue;

	public ActionsController(
		ILogger<ActionsController> logger,
		BackgroundServiceQueue backgroundServiceQueue
	)
	{
		_logger = logger;
		_backgroundServiceQueue = backgroundServiceQueue;
	}

	[HttpGet]
	[ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult GetAsync(Action action, CancellationToken cancellationToken)
	{
		IActionResult result;
		using (var _ = _logger.BeginScope($"{action.ToString()} =>"))
		{
			switch (action)
			{
				case Action.AmazonOrderSummary:
					_logger.LogInformation("Starting");
					_backgroundServiceQueue.Enqueue(action);
					result = Ok("Started");
					break;
				default:
					result = BadRequest($"Unknown action {action}");
					break;
			}

			_logger.LogInformation("Finished");
			return result;
		}
	}
}