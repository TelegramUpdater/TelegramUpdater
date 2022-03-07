using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;
using Xunit;

namespace TelegramUpdaterTests
{
    public class SingletonUpdateHandlerAttributeTests
    {
        [SingletonHandlerCallback(UpdateType.Message)]
        public static Task MyHandlerCallback(IContainer<Message> container)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Test_1()
        {
            
        }
    }
}
