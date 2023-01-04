using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TelegramBot
{

    public class SearchSettings
    {
        public static ISource[] filmSitesList = new ISource[]
        {
            new FilmSource{name = "megogo", type = FilmSourceType.Megogo},
            new FilmSource{name = "kinogo", type = FilmSourceType.Kinogo},
            new FilmSource{name = "gidonline", additionalText = " (Have a Chromecast)", type = FilmSourceType.Gidonline},
            new FilmSource{name = "rezka", type = FilmSourceType.Rezka},
            new FilmSource{name = "hdrezka", type = FilmSourceType.Hdrezka},
            new FilmSource{name = "uakino", type = FilmSourceType.Uakino},
            new FilmSource{name = "eneyida", type = FilmSourceType.EneyidaTV},
        };
        public static ISource[] filmGenerList = new ISource[]
        {
            new FilmGener{ name = "animation"},
            new FilmGener{ name = "anime"},
            new FilmGener{ name = "action"},
            new FilmGener{ name = "western"},
            new FilmGener{ name = "military"},
            new FilmGener{ name = "detective"},
            new FilmGener{ name = "children"},
            new FilmGener{ name = "drama"},
            new FilmGener{ name = "comedy"},
            new FilmGener{ name = "criminal"},
            new FilmGener{ name = "romantic"},
            new FilmGener{ name = "adventure"},
            new FilmGener{ name = "family"},
            new FilmGener{ name = "thriller"},
            new FilmGener{ name = "horror"},
            new FilmGener{ name = "scifi", programName = "sci-fi"},
            new FilmGener{ name = "fantasy"},
        };

        public static UrlSettings[] urlSettings = new UrlSettings[]
        {
            new UrlSettings
            {
                linkName = LinkName.GoogleSearch,

                linkType = LinkType.Data,
                mainLink = "https://www.google.com/search?q=",
                wordForWebSearch = "смотреть онлайн",
                endOfLink = "",
                additionalPrefix = "",
                correction = new CorrectionSettings
                {
                    dellInFinalLink = "href=\"/url?q=",
                    stopSymbol = '&',
                    symbolRepeatCount = 0,
                },
                cancelPack = new NamePack
                {
                    packLink = new NamePack.DelInFinalLink
                    {
                        cancelNames = new string[]
                        {
                            "accounts",
                            "/ru/",
                        }
                    },
                    searchProgramNameInLink = true,
                },
                pageSettings = new PageSettings
                {

                },
                resultWriter = new ResultWriter
                {
                    list = filmSitesList,
                    writer = (request, url) =>
                    {
                        var answer = "";
                        var writer = url.resultWriter;
                        for(int i = 0; i < writer.list.Length; i++)
                        {
                            var line = writer.list[i];
                            var counter = 1;
                            var returnCount = url.returnLinkCount;
                            var links = "";

                            foreach(var text in request)
                            {
                                if(text.linkType.type.ToString() == line.type.ToString())
                                {
                                    links +=
                                        counter + ") " + text.linkBody + "\n";
                                    counter++;
                                    if(returnCount-- == 0)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (links != "")
                            {
                                answer +=
                                    line.type.ToString() + line.additonalText + ":\n";
                                answer += links;
                                answer += "\n";
                            }
                        }
                        return answer;
                    },
                },
                saveSettings = new SaveSettings
                {
                    needSave = false,
                },

                whereLook = filmSitesList,

                keyWord = "href=\"/url?q=https://",
                returnLinkCount = 1,
            },
            new UrlSettings
            {
                linkName = LinkName.Kinoafisha,

                linkType = LinkType.Url,
                mainLink = "https://www.kinoafisha.info/rating/movies",
                wordForWebSearch = "",
                endOfLink = "/",
                additionalPrefix = "",
                correction = new CorrectionSettings
                {
                    dellInFinalLink = "title=\"",
                    stopSymbol = '\"',
                    symbolRepeatCount = 1,
                },
                cancelPack = new NamePack
                {
                    packHtml = new NamePack.DelInHtmlFile[]
                    {
                        new NamePack.DelInHtmlFile
                        {
                            cancelNames = new string[]
                            {
                                "россия",
                                "загрузить",
                            },
                            stopSymbol = "/div",
                        }
                    }
                },
                pageSettings = new PageSettings
                {
                    needFound = true,
                    pageNameLink = "?page=",
                    pageNameHtml = "?page=",
                },
                resultWriter = new ResultWriter
                {
                    writer = (request, url) =>
                    {
                        var answer = "";
                        var counter = BotInstruments.RandomRange(url.resultWriter.resultCount, request.Count);
                        var topCreate = new List<Link>();

                        for(int i = 0; i < counter.Length; i++)
                        {
                            topCreate.Add(request[counter[i]]);
                            answer += $"{i+1}) {request[counter[i]].linkBody}\n";
                        };

                        ClientIdentificator.ChangeClientData(url.clientId, new ClientDatas{ topValue = (true, topCreate)});

                        return answer;
                    },
                },
                saveSettings = new SaveSettings
                {
                    needSave = true,
                    needLoad = true,
                    filePath = FileManager.GENER_PATH,
                },

                keyWord = "title=\"",
                returnLinkCount = -1,
            },
            new UrlSettings
            {
                linkName = LinkName.CreateFilmLibrary,

                linkType = LinkType.Url,
                mainLink = "https://ru.wikipedia.org",
                wordForWebSearch = "",
                endOfLink = "",
                additionalPrefix = "/wiki/Категория:Фильмы_по_алфавиту",
                correction = new CorrectionSettings
                {
                    dellInFinalLink = "title=\"",
                    stopSymbol = '\"',
                    symbolRepeatCount = 1,
                },
                pageSettings = new PageSettings
                {
                    pageFounder = (link, url, age) =>
                    {
                        //Console.WriteLine(Uri.UnescapeDataString(link)+"\n\n");
                        //Console.WriteLine("page "+age+"\n");
                        var result = new List<Link>();
                        var html = WebReq.CreateWebRequest(link).ReadToEnd();

                        var newLink = "";
                        BotInstruments.TextConformity(out newLink, html, url.pageSettings.findWord, url.pageSettings.startFoundWord, '\"', 1);
                        newLink = newLink.Remove(0, url.pageSettings.findWord.Length);
                        newLink = BotInstruments.EditText(newLink, "amp;");

                        if(age != 167)
                        {
                            result.AddRange(url.pageSettings.pageFounder(url.mainLink + newLink, url, ++age));
                        }
                        result.Add(new Link{ linkBody = link });

                        return result.ToArray();
                    },
                    findWord = "href=\"",
                    startFoundWord = "Предыдущая",
                    needFindAllLinks = true,
                },
                saveSettings = new SaveSettings
                {
                    needSave = true,
                    needLoad = true,
                    filePath = FileManager.GENER_PATH,
                    fileName = "WikiLibrary",
                },
                resultWriter = new ResultWriter
                {
                    writer = (links, url) =>
                    {
                        var random = BotInstruments.RandomNum(links.Count);

                        return links[random].linkBody;
                    }
                },
                cancelPack = new NamePack
                {
                    packLink = new NamePack.DelInFinalLink
                    {
                        cancelNames = new string[]
                        {
                            "Категория:Фильмы",
                            "201",
                            "? (фильм)",
                            "...а пятый всадник - Страх",
                            ":и передайте привет ласточкам",
                            "<Чудотворец> из Бирюлёва",
                            "0-41*",
                        }
                    }
                },

                keyWord = "title=\"",
                startAt = "Следующая",
                finishAt = "noscript",
                returnLinkCount = -1,
            },
           
        };
        public static UrlSettings GetUrlSettings(LinkName linkName)
        {
            foreach(var url in urlSettings)
            {
                if (url.linkName == linkName) return url;
            }

            Console.WriteLine("**** Error" + " SearchSettings: line 53");
            return null;
        }
    }

    public class UrlSettings
    {
        public LinkName linkName { get; set; }
        public StreamReader streamReader { get; set; }
        public string clientId { get; set; }

        public LinkType linkType;
        public string mainLink;
        public string wordForWebSearch;
        public string request;
        public string endOfLink;
        public string additionalPrefix;
        public CorrectionSettings correction = CorrectionSettings.empty;
        public NamePack cancelPack = NamePack.empty;
        public PageSettings pageSettings;
        public ResultWriter resultWriter;
        public SaveSettings saveSettings;

        public ISource[] whereLook = new ISource[1] { new FilmGener { name = "", programName = "" } };
        public ISource[] whatLook = new ISource[1] { new FilmGener { name = "", programName = "" } };

        public string keyWord;
        public string startAt;
        public string finishAt;
        public int returnLinkCount = 0;

        public void SetResultCount(int num) => resultWriter.resultCount = num;
    }

    public enum LinkName
    {
        GoogleSearch,
        Kinoafisha,
        CreateFilmLibrary,
    }
    public struct Link
    {
        public string linkBody;
        public ISource linkType;
    }
    public enum LinkType
    {
        Data,
        Url,
    }

    public struct PageSettings
    {
        public bool needFound;
        public string pageNameLink;
        public string pageNameHtml;

        public bool needFindAllLinks;
        public string startFoundWord;
        public string findWord;
        public delegate Link[] NewPageFounder(string htmlPage, UrlSettings urlSettings, int age);
        public NewPageFounder pageFounder;

        public static PageSettings empty { get; } = new PageSettings() { };
    }
    public struct CorrectionSettings
    {
        public string dellInFinalLink;
        public char stopSymbol;
        public int symbolRepeatCount;

        public static CorrectionSettings empty { get; } = new CorrectionSettings() { };
    }
    public struct NamePack
    {
        public bool searchProgramNameInLink;
        public DelInHtmlFile[] packHtml;
        public DelInFinalLink packLink;
        public string text;
        public int startIndex;


        public static NamePack empty = new NamePack();
        public static bool Compare(NamePack target, NamePack date = new NamePack())
        {
            if (target.packHtml != date.packHtml) return false;
            else if (target.packLink.cancelNames != date.packLink.cancelNames) return false;
            else if (target.text != date.text) return false;
            else if (target.startIndex != date.startIndex) return false;

            return true;
        }

        public struct DelInHtmlFile
        {
            public string[] cancelNames;
            public string stopSymbol;
        }
        public struct DelInFinalLink
        {
            public string[] cancelNames;
        }
    }
    public struct ResultWriter
    {
        public ISource[] list;
        public delegate string Writer(List<Link> links, UrlSettings urlSettings);
        public Writer writer;
        public int resultCount;
    }
    public struct SaveSettings
    {
        public bool needSave;
        public bool needLoad;
        public string filePath;
        public string fileName;
    }

    public struct FilmGener : ISource
    {
        public string name;
        public string programName;

        Enum ISource.type => throw new NotImplementedException();

        string ISource.name => name == null ? "" : name;

        string ISource.programName => programName == null ? name : programName;

        string ISource.additonalText => throw new NotImplementedException();
    }
    public struct FilmSource : ISource
    {
        public string name;
        public string programName;
        public string additionalText;
        public FilmSourceType type;

        string ISource.additonalText => additionalText;

        string ISource.programName => programName == null ? name : programName;

        string ISource.name => name;

        Enum ISource.type => type;
    }
    public enum FilmSourceType
    {
        Megogo,
        Kinogo,
        Gidonline,
        Rezka,
        Hdrezka,
        Uakino,
        EneyidaTV,
    }
}
