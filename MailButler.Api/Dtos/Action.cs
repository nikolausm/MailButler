using System.ComponentModel.DataAnnotations;

namespace MailButler.Api.Dtos;

public enum Action
{
	[Display(Name = "No Action")] Unknown,

	[Display(Name = "Amazon Order Summary")]
	AmazonOrderSummary,

	[Display(Name = "Forward Invoices to GetMyInvoices")]
	ForwardInvoicesToGetMyInvoices,

	[Display(Name = "Current Action")] CurrentAction,

	[Display(Name = "Delete from known sender")]
	DeleteFromKnownSender,
	
	[Display(Name = "Mark old email as read")]
	MarkOldEmailAsRead
}