
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using WTelegram;
using WTelegram.Types;
using Telegram.Bot.Extensions;
using IrisClient.Services;

namespace Src
{
    public static class Router
    {
        private static readonly ConcurrentDictionary<string, List<IrisHandler>> CacheKey = new();


        public static void addHandler(string[] keys, Regex Pattern, Func<Message, Bot, Match,IrisApiClient, Task> Func, bool Html = false)
        {
            if (keys == null || keys.Length == 0)
                throw new ArgumentException("Нужно указать ключи команд");

            var Handler = new IrisHandler(Pattern, Func, Html);

            foreach (var Key in keys)
            {
                var k = Key.ToLowerInvariant();
                if (!CacheKey.TryGetValue(k, out var L))
                {
                    L = new List<IrisHandler>();
                    CacheKey[k] = L;
                }
                L.Add(Handler);
            }
        }

        public static async Task Route(Message Msg, Bot IrisBot, IrisApiClient IrisAPi)
        {
            if (Msg.Text == null) return;

            var Key = MessageKey(Msg.Text);
            if (!CacheKey.TryGetValue(Key, out var Handlers)) return;

            foreach (var H in Handlers)
            {
                string? Text = H.Html ? Msg.ToHtml() : Msg.Text;
                var Match = H.Pattern.Match(Text!);
                if (Match.Success)
                {
                    await H.Func(Msg, IrisBot, Match, IrisAPi);
                    break;
                }
            }
        }

        private static string MessageKey(string Text)
        {
            ReadOnlySpan<char> Trim = Text;

            int i = 0;
            while (i < Trim.Length && (Trim[i] == ' ' || Trim[i] == '.' || Trim[i] == '/' || Trim[i] == '!')) i++;
            Trim = Trim.Slice(i);

            int SInd = Trim.IndexOf(' ');
            if (SInd >= 0)
                Trim = Trim.Slice(0, SInd);

            return Trim.ToString().ToLowerInvariant();
        }

        public static async Task<string> MessageSend(Message Msg,long chatId, Bot IrisBot, string Text, bool Link = false, InlineKeyboardMarkup? K = null, bool Html = true)
        {
            var linkPreviewOption = Link ? null : new Telegram.Bot.Types.LinkPreviewOptions { IsDisabled = true };
            try
            {
                await IrisBot.SendMessage(chatId, Text, Html ? Telegram.Bot.Types.Enums.ParseMode.Html : Telegram.Bot.Types.Enums.ParseMode.None, replyParameters: Msg.IsTopicMessage ? Msg.MessageId : null, linkPreviewOptions: linkPreviewOption, replyMarkup: K);
            }
            catch
            {
                return "Error";
            }
                
            return "Done";
        }
    }
}