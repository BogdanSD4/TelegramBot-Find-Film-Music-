using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot
{
    public interface ISource
    {
        public string name { get; }
        public string programName { get; }
        public string additonalText { get; }
        public Enum type { get; }
    }
}
