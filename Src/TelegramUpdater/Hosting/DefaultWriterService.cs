using TelegramUpdater.UpdateWriters;

namespace TelegramUpdater.Hosting;

internal class DefaultWriterService()
    : UpdateWriterService<DefaultUpdateWriter>(
        new DefaultUpdateWriter())
{
   
}
