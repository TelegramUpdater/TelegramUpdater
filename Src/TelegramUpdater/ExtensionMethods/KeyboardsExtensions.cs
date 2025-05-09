using TelegramUpdater.Helpers;

namespace Telegram.Bot.Types.ReplyMarkups;

/// <summary>
/// Extension methods for telegram bot keyboards.
/// </summary>
public static class MarkupExtensions
{
    /// <summary>
    /// Build an <see cref="InlineKeyboardMarkup"/>.
    /// </summary>
    /// <param name="buildButtonsGrid">An action to build your inline markup.</param>
    /// <param name="rowCapacity">Maximum buttons in a row.</param>
    /// <returns></returns>
    public static InlineKeyboardMarkup BuildInlineKeyboards(
        Action<GridCollection<InlineKeyboardButton>> buildButtonsGrid,
        int? rowCapacity = default)
    {
        var grid = new GridCollection<InlineKeyboardButton>(rowCapacity);
        buildButtonsGrid(grid);
        return new(grid.Items);
    }

    /// <summary>
    /// Build a <see cref="ReplyKeyboardMarkup"/>.
    /// </summary>
    /// <param name="buildButtonsGrid">An action to build your reply markup.</param>
    /// <param name="rowCapacity">Maximum buttons in a row.</param>
    public static ReplyKeyboardMarkup BuildReplyKeyboards(
        Action<GridCollection<KeyboardButton>> buildButtonsGrid,
        int? rowCapacity = default)
    {
        var grid = new GridCollection<KeyboardButton>(rowCapacity);
        buildButtonsGrid(grid);
        return new(grid.Items);
    }

    #region InlineKeyboardButtons
    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackData(string, string)"/>
    public static GridCollection<InlineKeyboardButton> AddCallbackQuery(
        this GridCollection<InlineKeyboardButton> buttonsGrid,
        string text,
        string? callbackData = default)
    {
        return buttonsGrid.AddItem(callbackData is null ?
            InlineKeyboardButton.WithCallbackData(text) :
            InlineKeyboardButton.WithCallbackData(text, callbackData));
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithUrl(string, string)"/>
    public static GridCollection<InlineKeyboardButton> AddUrl(
        this GridCollection<InlineKeyboardButton> buttonsGrid,
        string text,
        string url)
    {
        return buttonsGrid.AddItem(InlineKeyboardButton.WithUrl(text, url));
    }
    #endregion
}
