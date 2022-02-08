using System;
using TelegramUpdater.Hosting;

namespace TelegramUpdater.Asp.Services
{
    public class ManualWritingUpdaterService : UpdaterService
    {
        public ManualWritingUpdaterService(IServiceProvider services) : base(services)
        {
        }

        protected override bool ManualWriting => true;
    }
}
