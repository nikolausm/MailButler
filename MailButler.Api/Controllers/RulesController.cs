
using MailButler.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MailButler.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RulesController : ControllerBase
{

	private readonly ILogger<RulesController> _logger;

	public RulesController(ILogger<RulesController> logger)
	{
		_logger = logger;
	}

	
	public IEnumerable<Rule> Get()
	{
		yield return new Rule();
	}
}