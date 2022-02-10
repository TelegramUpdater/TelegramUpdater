using System;

namespace TelegramUpdater.Hosting
{
    public sealed class ManualWritingUpdaterService : UpdaterService
    {
        public ManualWritingUpdaterService(IServiceProvider services) : base(services)
        {
        }

        protected override bool ManualWriting => true;
    }
}
