using MailButler.Dtos;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.AspNetCore.Mvc;

namespace MailButler.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ActionsController : ControllerBase
{
	private readonly ILogger<ActionsController> _logger;
	private readonly IServiceScope _scope;

	public ActionsController(ILogger<ActionsController> logger, IServiceScopeFactory scope)
	{
		_logger = logger;
		_scope = scope.CreateScope();
	}

	[HttpGet]
	[ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetAsync(Actions action, CancellationToken cancellationToken)
	{
		IActionResult result;
		using (var _ = _logger.BeginScope($"{action.ToString()} =>"))
		{
			switch (action)
			{
				case Actions.AmazonOderSummary:
					var smtpAccount = _scope.ServiceProvider.GetRequiredService<IList<Account>>()
						.First(r => r.Name.Contains("iCloud", StringComparison.InvariantCultureIgnoreCase));
					_logger.LogInformation("Starting");

					await _scope.ServiceProvider.GetRequiredService<AmazonOrderSummaryAction>()
						.ExecuteAsync(
							new AmazonOrderSummaryRequest
							{
								SmtpAccount = smtpAccount
							}, cancellationToken
						);

					result = Ok();
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