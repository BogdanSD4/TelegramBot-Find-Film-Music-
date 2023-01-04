using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TelegramBot;

namespace TelegramBot
{
    class WebReq
    {
        private const string CHROME_EXE = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";

        public static Link[] GetRequestFilling(Link link, UrlSettings urlSettings)
        {
            var result = new List<Link>();
            urlSettings.streamReader = CreateWebRequest(link.linkBody);

            var html = HtmlReader(urlSettings, link.linkType);
            //Console.WriteLine(html.Length);

            for (int i = 0; i < html.Length; i++)
            {
                //Console.WriteLine(html[i].linkBody);
                result.Add(new Link { linkBody = html[i].linkBody, linkType = html[i].linkType});
            }

            return result.ToArray();
        }

        public static StreamReader CreateWebRequest(string linkRequest)
        {
            WebRequest web = WebRequest.Create(linkRequest);
            web.Proxy = new WebProxy();
            Stream objStream = web.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            return objReader;
        }
        public static Link[] CreateLinkRequest(UrlSettings urlSettings)
        {
            var linkList = new List<Link>();
            var result = new List<Link>();
            var pageSettings = urlSettings.pageSettings;

            if (pageSettings.needFindAllLinks)
            {
                result.AddRange(pageSettings.pageFounder.Invoke(urlSettings.mainLink+urlSettings.additionalPrefix, urlSettings, 0));
            }
            else
            {
                for (int i = 0; i < urlSettings.whatLook.Length; i++)
                {
                    for (int j = 0; j < urlSettings.whereLook.Length; j++)
                    {
                        var body = "";
                        switch (urlSettings.linkType)
                        { 
                            case LinkType.Data:
                                body = Uri.EscapeUriString(
                                    $"{urlSettings.mainLink} " +
                                    $"{urlSettings.wordForWebSearch} " +
                                    $"{urlSettings.request} " +
                                    $"{urlSettings.whatLook[i].programName}" +
                                    $"{urlSettings.whereLook[j].programName} " +
                                    $"{urlSettings.endOfLink} " +
                                    $"{urlSettings.additionalPrefix}");
                                break;
                            case LinkType.Url:
                                body = Uri.EscapeUriString(
                                    $"{urlSettings.mainLink}" +
                                    $"{urlSettings.wordForWebSearch}" +
                                    $"{urlSettings.request}" +
                                    $"{urlSettings.whatLook[i].programName}" +
                                    $"{urlSettings.whereLook[j].programName}" +
                                    $"{urlSettings.endOfLink}" +
                                    $"{urlSettings.additionalPrefix}");
                                break;
                            default:
                                throw new Exception("WebReq: line 104");
                        }
                        linkList.Add(new Link { linkBody = body, linkType = urlSettings.whereLook[j] });
                    }
                }

                for (int i = 0; i < linkList.Count; i++)
                {
                    var pageCount = 0;
                    if (pageSettings.needFound)
                    {
                        pageCount = GetPageCount(linkList[i].linkBody, pageSettings);

                        for (int j = 0; j < pageCount; j++)
                        {
                            result.Add(new Link
                            {
                                linkBody = linkList[i].linkBody + urlSettings.pageSettings.pageNameLink + j,
                                linkType = linkList[i].linkType,
                            });
                            continue;
                        }
                        if (pageCount == 0)
                        {
                            result.Add(new Link
                            {
                                linkBody = linkList[i].linkBody,
                                linkType = linkList[i].linkType,
                            });
                        }
                    }
                    else
                    {
                        result.Add(linkList[i]);
                    }
                }
            }
 
            return result.ToArray();
        }
        private static Link[] HtmlReader(UrlSettings urlSettings, ISource source)
        {
            var result = new List<Link>();
            var sLine = urlSettings.streamReader.ReadToEnd();
            var keyWord = urlSettings.keyWord;

            var dellInFinalLink = urlSettings.correction.dellInFinalLink;
            var stopSymbol = urlSettings.correction.stopSymbol;
            var symbolRepeat = urlSettings.correction.symbolRepeatCount;

            var link = new Link() { linkType = source};
            var findWord = false;
            var findstart = urlSettings.startAt == null ? true : false;
            var findfinish = urlSettings.finishAt == null ? true : false;

            for (int i = 0; i < sLine.Length; i++)
            {
                if (!findstart)
                {
                    var correct = 0;
                    for (int j = 0; j < urlSettings.startAt.Length; j++)
                    {
                        if (sLine[i + j] == urlSettings.startAt[j])
                        {
                            correct++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (correct == urlSettings.startAt.Length)
                    {
                        findstart = true;
                    }
                    continue;
                }
                if (!findfinish)
                {
                    var correct = 0;
                    for (int j = 0; j < urlSettings.startAt.Length; j++)
                    {
                        if (sLine[i + j] == urlSettings.startAt[j])
                        {
                            correct++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (correct == urlSettings.startAt.Length)
                    {
                        break;
                    }
                }

                link.linkBody += sLine[i];
                if (!findWord)
                {
                    var correct = 0;
                    for (int j = 0; j < keyWord.Length; j++)
                    {
                        if (sLine[i + j] == keyWord[j])
                        {
                            correct++;
                        }
                        else
                        {
                            link.linkBody = "";
                            break;
                        }
                    }
                    if (correct == keyWord.Length)
                    {
                        findWord = true;
                    }
                }
                else
                {
                    if (i != sLine.Length - 1)
                    {
                        if (sLine[i + 1] == stopSymbol)
                        {
                            if(symbolRepeat != 0)
                            {
                                symbolRepeat--;
                                continue;
                            }

                            var correct = true;

                            if (correct)
                            {
                                link.linkBody = link.linkBody.Remove(0, dellInFinalLink.Length);

                                var cancelPack = urlSettings.cancelPack;
                                var removeCount = i - link.linkBody.Length - dellInFinalLink.Length;
                                cancelPack.text = sLine.Remove(0, removeCount);

                                if (CheckUrlBeforReturnClient(link, cancelPack))
                                {
                                    result.Add(link);
                                }
                                symbolRepeat = urlSettings.correction.symbolRepeatCount;
                            }

                            link.linkBody = "";
                            findWord = false;
                            continue;
                        }
                    }
                }
            }
            
            return result.ToArray();
        }       
        private static bool CheckUrlBeforReturnClient(Link result, NamePack namePack)
        {
            var value = false;
            var namesLink = namePack.packLink.cancelNames;
            var namesHtml = namePack.packHtml;

            if (namePack.searchProgramNameInLink)
            {
                BotInstruments.TextConformity(out value, result.linkBody, result.linkType.programName);
                if (!value) return false;
            }
            for (int i = 0; i < namesLink?.Length; i++) 
            {
                BotInstruments.TextConformity(out value, result.linkBody, namesLink[i]);
                //Console.WriteLine(result.linkBody + " | " + namesLink[i] +" | "+ value);
                if (value) return false;
            }
            for(int i = 0; i < namesHtml?.Length; i++)
            {
                for(int j = 0; j < namesHtml[i].cancelNames?.Length; j++)
                {

                    BotInstruments.TextConformity(out value, namePack.text, namesHtml[i].cancelNames[j], namesHtml[i].stopSymbol);
                    //Console.WriteLine(result.linkBody +" | "+ namesHtml[i].cancelNames[j] +" | "+ value);
                    if (value) return false;
                }
            }
 
            return true;
        }
        public static int GetPageCount(string linlRequest, PageSettings settings)
        {
            var htmlFormatSite = CreateWebRequest(linlRequest).ReadToEnd();
            var count = 0;
            BotInstruments.TextConformity(out count, htmlFormatSite, settings.pageNameHtml);
            return count;
        }
        private static string GetSiteName(string result)
        {
            var newResult = "";
            var isFind = false;

            for(int i = 0; i < result.Length; i++)
            {
                if (!isFind) 
                {
                    if (result[i] == '/' && result[i + 1] == '/')
                    {
                        i += 2;
                        isFind = true;
                    }
                    else continue;
                }

                newResult += result[i];
                if(i != result.Length - 1)
                {
                    if(result[i+1] == '/')
                    {
                        return newResult;
                    }
                }
            }

            return "----";
        }
    }
}



