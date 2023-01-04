using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace TelegramBot
{
    class FileManager
    {
        public const string GENER_PATH = "C:\\Users\\dokto\\OneDrive\\Рабочий стол\\Project\\TelegramBot\\Data\\Geners\\";
        public const string CLIENT_PATH = "C:\\Users\\dokto\\OneDrive\\Рабочий стол\\Project\\TelegramBot\\Data\\Client\\";
        private const string FILE_TYPE = ".txt";
        public static void SaveData(object data, string fileName, string filePath)
        {
            var path = filePath + fileName + FILE_TYPE;

            if (data.GetType() == typeof(string[]))
            {
                data = PackUpString((string[])data);
            }
            else if (data.GetType() != typeof(string))
            {
                data = JsonConvert.SerializeObject(data);
            }

            File.WriteAllText(path, (string)data);
        }
        
        public static T LoadData<T>(string fileName, string filePath)
        {
            if (typeof(T) == typeof(string[]))
            {
                Console.WriteLine("*** Error");
                throw new Exception();
            }

            var result = new List<T>();
            T res;
            var path = filePath + fileName + FILE_TYPE;
            var file = File.ReadAllText(path);

            if (typeof(T) == typeof(string))
            {
                result.Add((T)Convert.ChangeType(file, typeof(T)));
            }
            else 
            { 
                var data = JsonConvert.DeserializeObject<T>(file);
                result.Add((T)data);
            }
                
            res = result[0];
            return res;
        }

        private static string PackUpString(string[] messife)
        {
            var result = "";
            for(int i = 0; i < messife.Length; i++)
            {
                result += messife[i] + ";";
            }
            return result;
        }

        public static string[] PackOutString(string line)
        {
            var result = new List<string>();
            var name = "";

            for(int i = 0; i < line.Length; i++)
            {
                name += line[i];
                if (i != line.Length - 1)
                {
                    if(line[i+1] == ';')
                    {
                        result.Add(name);
                        i += 1;
                        name = "";
                    }
                }
            }

            return result.ToArray();
        }

        public static bool FileExists(string fileName, string filePath)
        {
            var path = filePath + fileName + FILE_TYPE;
            return File.Exists(path);
        }
    }
}
