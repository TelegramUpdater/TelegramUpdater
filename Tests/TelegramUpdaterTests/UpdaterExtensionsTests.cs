using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using Xunit;

namespace TelegramUpdaterTests;

public class UpdaterExtensionsTests
{
    [Fact]
    public void GetInnerUpdateTests()
    {
        foreach (var updateType in Update.AllTypes)
        {
            var update = CreateUpdateForType(updateType);

            Assert.IsType(updateType.ToObjectType(), update.GetInnerUpdate());
        }
    }

    static Update CreateUpdateForType(UpdateType updateType)
    {
        var update = new Update();

        typeof(Update).GetProperty(updateType.ToString())?
            .SetValue(update, Activator.CreateInstance(updateType.ToObjectType()!));

        return update;
    }
}
