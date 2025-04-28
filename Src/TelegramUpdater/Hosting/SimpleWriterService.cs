using TelegramUpdater.UpdateWriters;

namespace TelegramUpdater.Hosting;

internal class SimpleWriterService()
    : UpdateWriterService<SimpleUpdateWriter>(
        new SimpleUpdateWriter())
{
   
}
