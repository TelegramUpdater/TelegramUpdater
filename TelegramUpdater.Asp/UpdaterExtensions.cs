using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.Asp.Services;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater.Asp
{
    public static class UpdaterExtensions
    {
        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="configs">
        /// You can use <see cref="AspExtensions.ReadUpdaterConfigs(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
        /// to read configs from appsettings.json
        /// </param>
        public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                              UpdaterConfigs configs,
                                              Action<UpdaterServiceBuilder> builder)
        {
            serviceDescriptors.AddTelegramBotClient(configs);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var botClient = services.GetRequiredService<ITelegramBotClient>();
                    var updater = new Updater(botClient, new UpdaterOptions(
                        configs.MaxDegreeOfParallelism,
                        configs.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>()
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<UpdaterService>();
        }

        public static Updater RegisterUpdateHandler<THandler, TUpdate>(
            this Updater updater,
            Filter<TUpdate>? filter = default)
            where THandler : class, IScopedUpdateHandler where TUpdate : class
        {
            updater.AddScopedHandler<THandler, TUpdate>(filter);
            return updater;
        }

        public static async Task WriteUpdateAsync(this Updater updater,
                                                  Update update)
            => await updater.ChannelWriter.WriteAsync(update);
    }
}
