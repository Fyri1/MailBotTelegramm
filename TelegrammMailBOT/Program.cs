using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using TelegrammMailBOT;
using Microsoft.VisualBasic;
class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;
    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;
    // Словарь для отслеживания состояния пользователей
    private static ConcurrentDictionary<long, string> userStates = new ConcurrentDictionary<long, string>();


    static async Task Main()
    {

        _botClient = new TelegramBotClient("6477973369:AAGKRQUWYEHv-wZmYAktwgLNIKk2WtjRJf0"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;

                        // From - это от кого пришло сообщение
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
                        var chat = message.Chat;

                        // Проверяем состояние пользователя
                        if (userStates.TryGetValue(chat.Id, out string state) && state == "awaiting_email")
                        {
                            // Обрабатываем ввод email
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                ChangeMail.AddMail(message.Text),
                                replyToMessageId: message.MessageId);

                            // Сбрасываем состояние
                            userStates.TryRemove(chat.Id, out _);
                            return;
                        }

                        // Добавляем проверку на тип Message
                        switch (message.Type)
                        {
                            // Тут понятно, текстовый тип
                            case MessageType.Text:
                                {
                                    // тут обрабатываем команду /start, остальные аналогично
                                    if (message.Text == "/start")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Выбери клавиатуру:\n" +
                                            "/send_mail\n" +
                                            "/change_mail\n");
                                        return;
                                    }

                                    if (message.Text == "/send_mail")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("📩 ДАЙ ЕЩЕ 📩"),
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("/change_mail"),

                                                },
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Это reply клавиатура!",
                                            replyMarkup: replyKeyboard);

                                        return;
                                    }

                                    if (message.Text == "/change_mail")
                                    {
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("Добавить почты"),
                                                    new KeyboardButton("Удалить почты"),
                                                    new KeyboardButton("Файл с почтами"),
                                                },
                                                new KeyboardButton[]
                                                {
                                                    new KeyboardButton("/send_mail"),

                                                },
                                            })
                                        {
                                            ResizeKeyboard = true,
                                        };

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Это change_mail клавиатура!",
                                            replyMarkup: replyKeyboard);

                                        return;
                                    }

                                    if (message.Text == "📩 ДАЙ ЕЩЕ 📩")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            SendMail.ReadMail(),
                                            replyToMessageId: message.MessageId);

                                        return;
                                    }

                                    if (message.Text == "Файл с почтами")
                                    {
                                        string filePath = "Mail.txt"; // Укажите здесь путь к вашему файлу
                                        await SendMailFile(botClient, chat.Id, filePath);
                                        return;
                                    }

                                    if (message.Text == "Удалить почты")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            ChangeMail.DeliteMail(),
                                            replyToMessageId: message.MessageId);

                                        return;
                                    }
                                    if (message.Text == "Добавить почты")
                                    {
                                        // Устанавливаем состояние пользователя в "ожидание ввода почт"
                                        userStates[chat.Id] = "awaiting_email";

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Введите почту для добавления:",
                                            replyToMessageId: message.MessageId);

                                        return;
                                    }

                                    return;
                                }

                            default:
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Используй только текст!");
                                    return;
                                }
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }




    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    private static async Task SendMailFile(ITelegramBotClient botClient, long chatId, string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            await botClient.SendDocumentAsync(
                chatId: chatId,
                document: new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream, "mail_list.txt"),
                caption: "Вот ваш файл с почтами"
            );
        }
    }




}