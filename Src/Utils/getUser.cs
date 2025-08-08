using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using WTelegram;
using WTelegram.Types;

namespace Src.Utils
{
    public class getUser
    {
        private readonly Bot IrisBot;
        private readonly ConcurrentDictionary<long, (string link, DateTime Time)> _cache = new();

        public getUser(Bot irisbot)
        {
            IrisBot = irisbot;
        }

        public string clearNick(string Text)
        {
            if (string.IsNullOrWhiteSpace(Text)) return "";

            Text = Text.Trim().Normalize(NormalizationForm.FormKC);
            Text = Regex.Replace(Text, @"[^\p{L}\d]+", " ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            Text = Regex.Replace(Text, @"[\u0000-\u0020\u007F-\u009F\u200B-\u200F\u202A-\u202E\u2060-\u206F\uFEFF\u3164\u1160\u180E]+", " ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            Text = Regex.Replace(Text, @"\s+", " ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            return Text.Trim();
        }

        public async Task<string?> getLink(long? userId, bool Error = true)
        {
            if (userId == null || userId == 12) return null;
            if (_cache.TryGetValue(userId ?? 0, out var Cache) && Cache.Time > DateTime.UtcNow)
                return Cache.link;

            try
            {
                var info = await Task.Run(() => IrisBot.User(userId ?? 0));
                if (info == null)
                    return Error ? @$"<a href=""tg://user?id={userId}"">❗️ {userId}</b>" : null;

                string clearName = clearNick($"{info.FirstName ?? ""} {info.LastName ?? ""}");
                string userName = !string.IsNullOrWhiteSpace(clearName)
                    ? clearName
                    : (!string.IsNullOrEmpty(info.Username) ? info.Username.ToLower() : info.Id.ToString());

                string Link = !string.IsNullOrEmpty(info.Username)
                    ? $"https://t.me/{info.Username.ToLower()}"
                    : $"tg://openmessage?user_id={info.Id}";

                string result = @$"<a href=""{Link}"">{userName}</a>";

                _cache[userId ?? 0] = (result, DateTime.UtcNow.AddMinutes(5));
                return result;
            }
            catch
            {
                return Error ? @$"<a href=""tg://user?id={userId}"">❗️ {userId}</b>" : null;
            }
        }
    }
}