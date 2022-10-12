using System.ComponentModel.DataAnnotations;

namespace MailButler.Api.Controllers;

public enum Actions
{
	[Display(Name = "No Action")] Unknown,

	[Display(Name = "Amazon Order Summary")]
	AmazonOrderSummary
}