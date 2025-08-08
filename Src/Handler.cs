using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WTelegram.Types;
using WTelegram;
using IrisClient.Services;

namespace Src
{
    public class IrisHandler
    {
        public Regex Pattern { get; }
        public Func<Message, Bot, Match, IrisApiClient, Task> Func { get; }
        public bool Html { get; }

        public IrisHandler(Regex pattern, Func<Message, Bot, Match, IrisApiClient, Task> func, bool html)
        {
            Pattern = pattern;
            Func = func;
            Html = html;
        }
    }
}