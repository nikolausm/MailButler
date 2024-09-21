using System.Reflection;
using System.Text.Json.Serialization;
using MailButler.Api.BackgroundService;
using MailButler.UseCases.Components;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using MailButler.UseCases.Solutions.ForwardToGetMyInvoices;
using MailButler.UseCases.Solutions.MarkOldEmailsAsRead;
using MailButler.UseCases.Solutions.Spamfilter;
using Microsoft.OpenApi.Models;
using MailButlerApiOptions = MailButler.Api.MailButlerApiOptions;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile(
		"appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
		false
	)
	.AddEnvironmentVariables()
	.AddUserSecrets<Program>()
	.Build();


// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddUseCases();
builder.Services.AddLogging(configurationBuilder =>
{
	configurationBuilder.AddConfiguration(configuration.GetSection("Logging"));
	configurationBuilder.AddSimpleConsole(options =>
	{
		options.IncludeScopes = true;
		options.SingleLine = true;
		options.TimestampFormat = "hh:mm:ss ";
	});
});

builder.Services.AddSwaggerGen(options =>
{
	var assemblyName = Assembly.GetEntryAssembly()!.GetName();

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = assemblyName.Version!.ToString(3),
		Title = assemblyName.Name
	});
});
builder.Services.Configure<MailButlerApiOptions>(configuration.GetSection("MailButler"));
builder.Services.AddTransient<AmazonOrderSummaryAction>();
builder.Services.AddTransient<ForwardToGetMyInvoicesAction>();
builder.Services.AddTransient<DeleteFromKnownSenderAction>();
builder.Services.AddSingleton<BackgroundServiceQueue>();
builder.Services.AddSingleton<BackgroundServiceWorker>();
builder.Services.AddTransient<EmailBodyParts>();
builder.Services.AddTransient<MarkOldEmailsAsReadAction>();

builder.Services.AddHostedService(e => e.GetRequiredService<BackgroundServiceWorker>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();

app.MapControllers();

app.Run();