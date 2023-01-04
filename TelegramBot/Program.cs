using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;

namespace TelegramBot
{
    class Program
    {

        static string apiKey = "5922664857:AAFHihEbkwxs1m_JQIab557RpPtuwIM2iSo";

        static void Main(string[] args)
        {
            var client = new TelegramBotClient(apiKey);

            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            var callBackQuery = update.CallbackQuery;

            await CreateBotCommandsMenu(client);


            if (update.Type == UpdateType.Message && message != null)
            {
                await ClientMessageCallBack(client, message);
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await ClientQueryCallBack(client, callBackQuery);
            }
        }

        private static async Task CreateBotCommandsMenu(ITelegramBotClient client)
        {
            var commands = new BotCommand[]
            {
                new BotCommand{Command = "getfilm", Description = "Find Film" },
                new BotCommand{Command = "getmusic", Description = "Find Music" },
                new BotCommand{Command = "setevent", Description = "Add Event" },
                new BotCommand{Command = "start", Description = "Start" },
                new BotCommand{Command = "showcommands", Description = "Show all text commands" },
            };

            await client.SetMyCommandsAsync(commands);
            await client.GetMyCommandsAsync();
        }
        private static async Task ClientQueryCallBack(ITelegramBotClient client, CallbackQuery callBackQuery)
        {
            if (callBackQuery.Data.Contains("Movie"))
            {
                return;
            }

            if (callBackQuery.Data.Contains("Music"))
            {
                return;
            }
        }
        private static async Task ClientMessageCallBack(ITelegramBotClient client, Message message)
        {
            //return;
            var messageText = message.Text;
            var clientID = message.Chat.Id.ToString();

            if (CheckCLientMessage(messageText, client, message))
            {
                var clientData = ClientIdentificator.GetClienData(clientID);
                if (clientData.findByName.Item2)
                {
                    var requets = message.Text;
                    var count = 0;
                    if (Int32.TryParse(messageText, out count))
                    {
                        var data = ClientIdentificator.GetClienData(clientID).topValue;
                        requets = data.Item2[count-1].linkBody;
                        ClientIdentificator.ChangeClientData(clientID, new ClientDatas { topValue = (true, new List<Link>()) });
                    }
                    else
                    {
                        var data = ClientIdentificator.GetClienData(clientID).topValue;
                        if (data.Item2.Count == 0)
                        {
                            await client.SendTextMessageAsync(clientID, 
                                "Sorry, your number does not match any movie number" +
                                "or it's not a number at all");
                            return;
                        }
                    }

                    var answer = ClientMessage.ClientCallBack(client, message, SearchSettings.GetUrlSettings(LinkName.GoogleSearch), requets);

                    await client.SendTextMessageAsync(clientID, answer);

                    ClientIdentificator.ChangeClientData(clientID, new ClientDatas { findByName = (true, false) });
                    return;
                }

                if (clientData.findByGener.Item2)
                {
                    var count = 0;
                    if (Int32.TryParse(messageText, out count))
                    {
                        await client.SendTextMessageAsync(clientID, "OK, I'm connecting the databases");

                        var url = SearchSettings.GetUrlSettings(LinkName.Kinoafisha);
                        url.SetResultCount(count);
                        

                        var text = ClientIdentificator.GetClienData(clientID).request.Item2;
                        url.saveSettings.fileName = text.Remove(0, 1);
                        var answer = ClientMessage.ClientCallBack(client, message, url, text.Remove(0, 1));

                        await client.SendTextMessageAsync(clientID, "Well, not even tired\n"+answer);

                        await client.SendTextMessageAsync(clientID, 
                            $"If you want find one of them, send me movie number");

                        ClientIdentificator.ChangeClientData(clientID, new ClientDatas 
                        {
                            findByName = (true, true),
                        });
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id,
                            "...Someone is not reading my requirements");
                    }

                    ClientIdentificator.ChangeClientData(clientID, new ClientDatas { findByGener = (true, false) });
                }
            }
            else
            {
                ClientIdentificator.ChangeClientData(clientID, ClientDatas.reset);
            }

            CheckCLientMessageSecond(messageText, client, message);
           
            if (messageText == "/start")
            {
                await client.SendTextMessageAsync(message.Chat.Id, 
                    "Hello!" + 
                    "\nI'm here to make your life a little easier " +
                    "\n" +
                    "\nThat's what I can: " +
                    "\n/getfilm - I find film to you" +
                    "\n/getmusic - I find music to you" +
                    "\n/setevent - I will help you not to forget about your plans");
                return;
            }

            if(messageText == "/getfilm")
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "/bygener - Find film by gener" +
                    "\n/byname - Write name of film, what you want find " +
                    "\n/bymind - Write an approximate name of film" +
                    "\n/getrandomfilm - I find to you some film, my answer is newer repeated"
                    );
                return;
            }

            if (messageText == "/bygener")
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Choose something:" +
                    "\n" +
                    "\n/animation" +
                    "\n/anime" +
                    "\n/action" +
                    "\n/western" +
                    "\n/military" +
                    "\n/detective" +
                    "\n/children" +
                    "\n/comedy" +
                    "\n/criminal" +
                    "\n/romantic" +
                    "\n/adventure" +
                    "\n/thriller" +
                    "\n/horror" +
                    "\n/scifi" +
                    "\n/fantasy");
                
                return;
            }

            if (messageText == "/byname")
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Send me film name");
                ClientIdentificator.ChangeClientData(message.Chat.Id.ToString(), new ClientDatas { findByName = (true, true) });

                return;
            }

            if (messageText == "/bymind")
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Oops! It's so hard for me yet\n" +
                    "Try ask in Google:\n" +
                    "https://www.google.com/search?q=");
            }

            if (messageText == "/getrandomfilm")
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "OK. Give me one second..");

                var wiki = ClientMessage.ClientCallBack(client, message, SearchSettings.GetUrlSettings(LinkName.CreateFilmLibrary));

                await client.SendTextMessageAsync(message.Chat.Id,
                    $"It's your film: \"{wiki}\"");

                var url = SearchSettings.GetUrlSettings(LinkName.GoogleSearch);

                var links = ClientMessage.ClientCallBack(client, message, url, wiki);

                await client.SendTextMessageAsync(message.Chat.Id, links);
            }
        }

        private static void CheckCLientMessageSecond(string request, in ITelegramBotClient client, in Message message)
        {
            var generList = new string[]
            {
                "/animation",
                "/anime",
                "/action",
                "/western",
                "/military",
                "/detective",
                "/children",
                "/comedy",
                "/criminal",
                "/romantic",
                "/adventure",
                "/thriller",
                "/horror",
                "/scifi",
                "/fantasy",
            };
            for (int i = 0; i < generList.Length; i++)
            {
                if (request == generList[i])
                {
                    ClientMessage.ClientExtraMessage(client, message,
                        "Ok.Please write number of how much top size you want get" +
                        "\n" +
                        "\nDon't joke with me, number can't be more than 20 " +
                        "and no 0.0001");

                    ClientIdentificator.ChangeClientData(message.Chat.Id.ToString(), new ClientDatas 
                    {
                        findByGener = (true, true),
                        request = (true, message.Text),
                    });
                }
            }
        }

        private static bool CheckCLientMessage(string request, in ITelegramBotClient client, in Message message)
        {
            if (request.StartsWith("/")) return false;

            return true;
        }

        private static void CreateClientMarkup(out ReplyKeyboardMarkup replyKeyboard, out InlineKeyboardMarkup inlineKeyboard)
        {
            replyKeyboard = new ReplyKeyboardMarkup(new[]
                            {
                    new KeyboardButton[]{ "/start"},
                })
            {
                ResizeKeyboard = true
            };
            inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData( "Movie", "Movie"),
                    InlineKeyboardButton.WithCallbackData( "Music", "Music")
                }
            );
        }
        

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine($"Error {arg2}");
            throw new NotImplementedException();
        }
    }
}
