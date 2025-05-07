using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramUpdater.Helpers;
using Xunit;
using static TelegramUpdater.UpdaterExtensions;

namespace TelegramUpdaterTests;

public class CompositeKeyTests
{
    [Fact]
    public void FirstTest()
    {
        var scopeId = new HandlingStoragesKeys.ScopeId(Guid.NewGuid());

        var key1 = new CompositeKey<HandlingStoragesKeys.ScopeId, CompositeKey<long, string>>(
            scopeId, new CompositeKey<long, string>(12345, "test"));

        var key2 = new CompositeKey<HandlingStoragesKeys.ScopeId, CompositeKey<long, string>>(
            scopeId, new CompositeKey<long, string>(12345, "test"));

        Assert.Equal(key1, key2);
    }
}
