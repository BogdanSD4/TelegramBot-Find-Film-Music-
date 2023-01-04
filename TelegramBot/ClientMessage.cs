using System;
using System.Collections.Generic;
using System.Text;
using TelegramBot;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    class ClientMessage
    {
        public static string ClientCallBack(ITelegramBotClient client, Message message, UrlSettings urlSettings, string messageText = null)
        {
            
            if (messageText == null)
            {
                urlSettings.request = message.Text;
            }
            else
            {
                urlSettings.request = messageText;
            }

            urlSettings.clientId = message.Chat.Id.ToString();

            var saveSettings = urlSettings.saveSettings;
            var requestAnswer = new List<Link>();

            if (saveSettings.needLoad && FileManager.FileExists(urlSettings.saveSettings.fileName, saveSettings.filePath))
            {
                var file = FileManager.LoadData<string>(urlSettings.saveSettings.fileName, saveSettings.filePath);
                var massife = FileManager.PackOutString(file);
                var counter = new int[] { };
                
                if (urlSettings.resultWriter.resultCount != 0)
                {
                    counter = BotInstruments.RandomRange(urlSettings.resultWriter.resultCount, massife.Length);

                    for (int i = 0; i < counter.Length; i++) requestAnswer.Add(new Link { linkBody = massife[counter[i]] });
                }
                else
                {
                    for (int i = 0; i < massife.Length; i++) requestAnswer.Add(new Link { linkBody = massife[i] });
                }   
            }
            else
            {

                var link = WebReq.CreateLinkRequest(urlSettings);

                for (int i = 0; i < link.Length; i++)
                {
                    if (i == link.Length / 2) ClientExtraMessage(client, message, "Almost done");
                    var value = WebReq.GetRequestFilling(link[i], urlSettings);
                    for (int j = 0; j < value.Length; j++)
                    {
                        requestAnswer.Add(value[j]);
                    }
                }

                if (saveSettings.needSave)
                {
                    var saveData = new List<string>();
                    for (int i = 0; i < requestAnswer.Count; i++) saveData.Add(requestAnswer[i].linkBody);
                    FileManager.SaveData(saveData.ToArray(), urlSettings.saveSettings.fileName, saveSettings.filePath);
                }
            }

            return urlSettings.resultWriter.writer.Invoke(requestAnswer, urlSettings);
        }

        public static async void ClientExtraMessage(ITelegramBotClient client, Message message, string messageText)
        {
            await client.SendTextMessageAsync(message.Chat.Id, messageText);
        }
    }

    
}
