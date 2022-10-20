using System.ComponentModel.DataAnnotations;

namespace MailButler.Api.Dtos;

public enum Action
{
	[Display(Name = "No Action")] Unknown,

	[Display(Name = "Amazon Order Summary")]
	AmazonOrderSummary,

	[Display(Name = "Current Action")] CurrentAction
}