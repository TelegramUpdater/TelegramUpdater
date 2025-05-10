using TelegramUpdater.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// this will collect updater options like BotToken, AllowedUpdates and ...
// from configuration section "TelegramUpdater". in this example from appsettings.json
builder.AddTelegramManualUpdater(builder => builder
    .QuickStartCommandReply("Hello there!")
    // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
    .CollectHandlers()
    .AddDefaultExceptionHandler());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
