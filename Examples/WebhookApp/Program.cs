using TelegramUpdater.Asp;
using WebhookApp.Services;
using WebhookApp.UpdateHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var updaterConfigs = builder.Configuration.GetUpdaterConfigs();

builder.Services.AddTelegramUpdater(
    updaterConfigs, (builder) =>
        builder.AddMessageHandler<SimpleMessageHandler>()
);

builder.Services.AddWebhookConfigs<WebhookConfigs>();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapWebhook("tgbotwebhook", pattern: @$"updates/{updaterConfigs.BotToken}");
    endpoints.MapControllers();
});


app.Run();
