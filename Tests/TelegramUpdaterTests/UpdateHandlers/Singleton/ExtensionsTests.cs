using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;
using Xunit;

namespace TelegramUpdaterTests.UpdateHandlers.Singleton;

public class ExtensionsTests
{
    public static Task HandleMessageUpdate(IContainer<Message> container)
    {
        return Task.CompletedTask;
    }

    public static Task HandleChatBoostUpdate(IContainer<ChatBoostUpdated> container)
    {
        return Task.CompletedTask;
    }

    public static Task HandleWrongChatBoostUpdate(IContainer<ChatBoost> container)
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void GetSingletonUpdateHandlerTests1()
    {
        var method = GetType().GetMethod("HandleMessageUpdate");
        var result = (AnyHandler<Message>?)SingletonAttributesExtensions.GetSingletonUpdateHandler(
            method!, Telegram.Bot.Types.Enums.UpdateType.Message, 0);

        Assert.NotNull(result);

        var actualUpdate = result!.GetActualUpdate(new()
        {
            Message = new()
        });
        
        Assert.NotNull(actualUpdate);
    }

    [Fact]
    public void GetSingletonUpdateHandlerTests2()
    {
        var method = GetType().GetMethod("HandleChatBoostUpdate");
        var result = (AnyHandler<ChatBoostUpdated>?)SingletonAttributesExtensions.GetSingletonUpdateHandler(
            method!, Telegram.Bot.Types.Enums.UpdateType.ChatBoost, 0);

        Assert.NotNull(result);

        var actualUpdate = result!.GetActualUpdate(new()
        {
            ChatBoost = new()
        });

        Assert.NotNull(actualUpdate);
    }

    [Fact]
    public void GetSingletonUpdateHandlerTests3()
    {
        var method = GetType().GetMethod("HandleWrongChatBoostUpdate");
        var result = SingletonAttributesExtensions.GetSingletonUpdateHandler(
            method!, Telegram.Bot.Types.Enums.UpdateType.ChatBoost, 0);

        Assert.Null(result);
    }
}
