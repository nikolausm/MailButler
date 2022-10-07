using MailButler.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MailButler.UseCases.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUseCases(this IServiceCollection services)
	{
		services.TryAddSingleton<IImapClientFactory, ImapClientFactory>();
		services.TryAddSingleton<ISmtpClientFactory, SmtpClientFactory>();
		services.AddMediatR(typeof(AssemblyMarker));
		
		return services;
	}
}