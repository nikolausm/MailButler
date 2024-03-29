using MailButler.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MailButler.UseCases.Components.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUseCases(this IServiceCollection services)
	{
		services.TryAddSingleton<EmailBodyParts>();
		services.TryAddSingleton<IImapClientFactory, ImapClientFactory>();
		services.TryAddSingleton<ISmtpClientFactory, SmtpClientFactory>();
		services.AddMediator();

		return services;
	}
}