using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot
{
    class BotInstruments
    {
        public static int RandomNum(int max, int min = 0)
        {
            Random random = new Random();

            return random.Next(min, max);
        }
        public static int[] RandomRange(int count, int max, int min = 0)
        {
            var range = new List<int>();
            Random random = new Random();

            for (int i = 0; i < count;)
            {
                var num = random.Next(min, max);
                if (!range.Contains(num))
                {
                    range.Add(num);
                    i++;
                }
            }

            return range.ToArray();
        }
        
        public static void TextConformity(out string result, string target, string findWord, 
            string startSymbol = null ,char stopSymbol = ' ', int stopSymbolrepeat = 0)
        {
            var res = "";
            var wordIsFind = false;
            var startSymbolFind = startSymbol == null? true : false;
            var repeatCount = stopSymbolrepeat;

            for (int i = 0; i < target.Length; i++)
            {
                res += target[i];
                if (!startSymbolFind)
                {
                    var correct = 0;
                    for (int j = 0; j < startSymbol.Length; j++)
                    {
                        if (target[i + j] != startSymbol[j])
                        {
                            res = "";
                            break;
                        }
                        else correct++;
                    }
                    if (correct == startSymbol.Length)
                    {
                        startSymbolFind = true;
                    }
                    res = "";
                    continue;
                }
                if (!wordIsFind)
                {
                    var correct = 0;
                    for (int j = 0; j < findWord.Length; j++)
                    {
                        if (target[i + j] != findWord[j])
                        {
                            res = "";
                            break;
                        }
                        else correct++;
                    }
                    if (correct == findWord.Length)
                    {
                        wordIsFind = true;
                    }
                }
                else
                {
                    if (i != target.Length - 1)
                    {
                        if (target[i + 1] == stopSymbol)
                        {
                            if (repeatCount-- == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            result = res;
        }
        public static void TextConformity(out bool availability, string target, string findWord, string stopSymbol = null)
        {
            var res = "";
            var wordIsFind = false;

            for (int i = 0; i < target.Length; i++)
            {
                res += target[i];

                if (stopSymbol != null)
                {
                    var stop = 0;
                    for (int j = 0; j < stopSymbol.Length; j++)
                    {
                        try
                        {
                            if (target[i + j] == stopSymbol[j])
                            {
                                stop++;
                            }
                        }
                        catch (Exception) { continue; }
                    }
                    if(stop == stopSymbol.Length)
                    {
                        availability = false;
                        return;
                    }
                }

                if (!wordIsFind)
                {
                    var correct = 0;
                    for (int j = 0; j < findWord.Length; j++)
                    {
                        try
                        {
                            if (target[i + j] != findWord[j])
                            {
                                res = "";
                                break;
                            }
                            else
                            {
                                correct++;
                            }
                        }
                        catch (Exception) { continue; }
                    }
                    if (correct == findWord.Length)
                    {
                        availability = true;
                        return;
                    }
                }
            }
            availability = false;
        }
        public static void TextConformity(out int repeatCount, string target, string findWord)
        {
            var res = "";
            var repeat = 0;

            for (int i = 0; i < target.Length; i++)
            {
                res += target[i];

                var correct = 0;
                for (int j = 0; j < findWord.Length; j++)
                {
                    
                    if (target[i + j] != findWord[j])
                    {
                        res = "";
                        break;
                    }
                    else
                    {
                        correct++;
                    }
                }
                if (correct == findWord.Length)
                {
                    repeat++;
                    i += findWord.Length;
                }
            }

            repeatCount = repeat;
        }
        public static string EditText(string text, string dellText)
        {
            var result = "";
            var findFlaw = false;

            for (int i = 0; i < text.Length; i++)
            {
                var access = 0;
                if (!findFlaw)
                {
                    if (i <= text.Length - dellText.Length)
                    {
                        for (int j = 0; j < dellText.Length; j++)
                        {
                            if (text[i + j] == dellText[j])
                            {
                                access++;
                            }
                            if (access == dellText.Length)
                            {
                                i += dellText.Length;
                                findFlaw = true;
                            }
                        }
                    }
                }
                result += text[i];
            }
            return result;
        }
    }
}
