using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using MailButler.UseCases.Solutions.MarkOldEmailsAsRead;
using Microsoft.Extensions.DependencyInjection;

namespace MailButler.Console.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMailButler(this IServiceCollection services)
	{
		services.AddSingleton<AmazonOrderSummaryAction>();
		services.AddSingleton<MarkOldEmailsAsReadAction>();
		return services;
	}
}