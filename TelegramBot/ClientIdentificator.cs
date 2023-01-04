using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot
{
    public class ClientIdentificator
    {
        public static void ChangeClientData(string clientID, ClientDatas data)
        {
            if (FileManager.FileExists(clientID, FileManager.CLIENT_PATH))
            {
                var target = FileManager.LoadData<ClientDatas>(clientID, FileManager.CLIENT_PATH);
                FileManager.SaveData(ClientDatas.SetData(target, data), clientID, FileManager.CLIENT_PATH);
            }
            else
            {
                FileManager.SaveData(data, clientID, FileManager.CLIENT_PATH);
            }
        }

        public static ClientDatas GetClienData(string clientID)
        {
            return FileManager.LoadData<ClientDatas>(clientID, FileManager.CLIENT_PATH);
        }
    }
    public struct ClientDatas
    {
        public (bool, bool) findByName;
        public (bool, bool) findByGener;
        public (bool, string) request;
        public (bool, List<Link>) topValue;

        public static ClientDatas SetData(ClientDatas target, ClientDatas data)
        {
            target.findByName = data.findByName.Item1 == true ? (false, data.findByName.Item2) : target.findByName;
            target.findByGener = data.findByGener.Item1 == true ? (false, data.findByGener.Item2) : target.findByGener;
            target.topValue = data.topValue.Item1 == true ? (false, data.topValue.Item2) : target.topValue;
            target.request = data.request.Item1 == true ? (false, data.request.Item2) : target.request;

            return target;
        }

        public static ClientDatas reset { get; } = new ClientDatas 
        {
            findByGener = (true, false),
            findByName = (true, false),
            topValue = (true, new List<Link>()),
            request = (true, null),
        };
    };
}
