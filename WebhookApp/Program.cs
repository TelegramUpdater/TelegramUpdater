using Telegram.Bot.Types;
using TelegramUpdater.Asp;
using WebhookApp.UpdateHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var botConfigs = builder.Configuration.ReadUpdaterConfigs();

builder.Services.AddTelegramUpdater(
    botConfigs, (builder) =>
        builder.AddHandler<SimpleMessageHandler, Message>()
);


builder.Services.AddWebhookConfigs();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapWebhook("tgbotwebhook", botConfigs);
    endpoints.MapControllers();
});


app.Run();
