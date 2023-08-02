using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Semantic.Consts.System;
using Semantic.Extensions.Cognitive;
using Semantic.Extensions.Logging;
using Semantic.Extensions.String;
using Semantic.Models.System;
using Semantic.Skills;
using Semantic.Skills.Core;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

// Figure out why updates isn't working in group chats.
// Always reply on the same channel as received when in chats and tag the person that asked the question, in the response.

var botName = "@frostaura_zeus_bot";
var IS_IN_GROUP_MODE = false;
var currentMessageId = -1;
long chatId = -1;
using var cts = new CancellationTokenSource();

Task PollingErrorHandler(ITelegramBotClient bot, Exception ex, CancellationToken ct)
{
    Console.WriteLine($"Exception while polling for updates: {ex}");
    return Task.CompletedTask;
}

InlineKeyboardMarkup GetMainMenu()
{
    var menu = new List<InlineKeyboardButton[]>
    {
        new[] { InlineKeyboardButton.WithCallbackData("Stop", "/stop") }
    };

    return new InlineKeyboardMarkup(menu);
}

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct, BaseSkill semanticSkill)
{
    try
    {
        if (IS_IN_GROUP_MODE && (update.Message?.Text == default || !update.Message.Text.StartsWith(botName))) return;

        if (update.Message?.Text == "/start")
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, "Welcome to the bot!", replyMarkup: GetMainMenu());
        }
        else if(update.Message.Text == "/stop")
        {
            cts.Cancel();
        }
        else if (update.Message != default)
        {
            if (string.IsNullOrWhiteSpace(update.Message.Text)) return;

            var initialMessage = $"Ask: {update.Message.Text}{Environment.NewLine}";
            var message = await bot.SendTextMessageAsync(update.Message.Chat.Id, initialMessage, replyMarkup: GetMainMenu());

            chatId = message.Chat.Id;
            currentMessageId = message.MessageId;

            // Clear the logs.
            var context = new Dictionary<string, string>();
            Logging.CurrentIndentation = 0;
            Logging.Logs.Value = new List<IndentedLog>
            {
                new IndentedLog { Indentation = Logging.CurrentIndentation, Text = initialMessage }
            };
            Logging.CurrentIndentation = 1;
            var response = await semanticSkill.RunAsync(update.Message.Text, context);

            await bot.SendTextMessageAsync(update.Message.Chat.Id, response.MarkdownV2Escape(), parseMode: ParseMode.MarkdownV2);
        }
        else if(update.CallbackQuery?.Data == "/stop")
        {
            cts.Cancel();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while handling {update.Type}: {ex}");
    }
}

var token = Environment.GetEnvironmentVariable("TOKEN")! ?? "5974796821:AAGGt6fyfMX3RuZUQY9RzNfhKS-8GoLYkok";
var bot = new TelegramBotClient(token);
var me = await bot.GetMeAsync();
var logger = ILoggerExtensions.CreateDefaultLogger(typeof(PlannerSkill));
var skills = typeof(BaseSkill)
    .Assembly
    .GetAllSkillTypesInAssembly()
    .CreateInstances()
    .ToList();
var plannerSkill = new PlannerSkill(skills, logger);

Logging.Logs.Subscribe(async updatedLogs =>
{
    var update = string.Empty;

    foreach (var log in updatedLogs)
    {
        var padding = string.Empty;

        if(log.Indentation > 0)
            padding = Enumerable
                .Range(0, log.Indentation)
                .Select(r => "-")
                .Aggregate((l, r) => l + r);

        update += $"{padding}{log.Text}{Environment.NewLine}";
    }

    if(currentMessageId > -1 && chatId > -1)
    {
        try
        {
            await bot.EditMessageTextAsync(chatId, currentMessageId, update);
        }
        catch (Exception ex)
        { }
    }
});

var options = new Telegram.Bot.Polling.ReceiverOptions
{
    ThrowPendingUpdates = true,
    AllowedUpdates = new []
    {
        UpdateType.CallbackQuery,
        UpdateType.ChannelPost,
        UpdateType.ChatJoinRequest,
        UpdateType.ChatMember,
        UpdateType.ChosenInlineResult,
        UpdateType.EditedChannelPost,
        UpdateType.EditedMessage,
        UpdateType.InlineQuery,
        UpdateType.Message,
        UpdateType.MyChatMember,
        UpdateType.Poll,
        UpdateType.PollAnswer,
        UpdateType.PreCheckoutQuery,
        UpdateType.ShippingQuery,
        UpdateType.Unknown
    } 
};
bot.StartReceiving((bot, update, token) => HandleUpdateAsync(bot, update, token, plannerSkill), PollingErrorHandler, options, cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// stop the bot
cts.Cancel();