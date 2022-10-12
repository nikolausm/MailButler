using System.Text.Json.Serialization;
using MailButler.Api;
using MailButler.Dtos;
using MailButler.UseCases.Components.Extensions.DependencyInjection;
using MailButler.UseCases.Solutions.Amazon.AmazonOrderSummary;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
		true)
	.AddUserSecrets<MailButlerOptions>()
	.AddEnvironmentVariables()
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

builder.Services.AddSwaggerGen();
builder.Services.Configure<MailButlerOptions>(configuration.GetSection("MailButler"));
builder.Services.AddTransient<IList<Account>>(
	sp => sp.GetRequiredService<IOptions<MailButlerOptions>>().Value.Accounts
);
builder.Services.AddTransient<AmazonOrderSummaryAction>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();

app.MapControllers();

app.Run();