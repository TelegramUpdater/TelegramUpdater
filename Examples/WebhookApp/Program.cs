using TelegramUpdater.Asp;
using WebhookApp.Services;
using WebhookApp.UpdateHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var updaterConfigs = builder.Configuration.GetUpdaterConfigs();

builder.Services.AddTelegramManualUpdater(
    updaterConfigs,
    (builder) => builder
        .AddMessageHandler<SimpleMessageHandler>()
        .AddExceptionHandler<Exception>(
            (u, e) =>
            {
                u.Logger.LogWarning(exception: e, message: "Error while handlig ...");
                return Task.CompletedTask;
            }, inherit: true)
        );
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
