using System.Text.RegularExpressions;
using IrisClient.Services;
using Src.Utils;
using WTelegram;
using WTelegram.Types;

namespace Src.Mosules
{
    public class IrisCommands
    {
        public void Register()
        {
            Router.addHandler(
                new[] { "мешок", "bag", "баланс" },
                new Regex(@"^[./! ]?[ ]?(мешок бот(?:a)?|bag bot|баланс бот(?:a)?)[ ]*(?:\n+.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
                async (Message Msg, Bot IrisBot, Match _Match, IrisApiClient IrisApi) =>
                {
                    if (Msg.From == null) return;

                    var Balance = await IrisApi.GetBalance();

                    if (Balance.sweets == 0.0 || Balance.gold == 0.0 || Balance.donate_score == 0)
                    {
                        await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, "📝 Мешок бота полностью пуст", true);
                        return;
                    }
                    string resultMessage = "💰 В мешке бота:\n" +
                                        $"🍬 {Balance.sweets} ирисок\n" +
                                        $"🌕 {Balance.gold} ирис-голд\n" +
                                        $"🥯 {Balance.donate_score} очков доната\n\n" +
                                        "💬 Запасы бота можно пополнить, введя команду \"Пополнить бота (число) @id\"";

                    await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, resultMessage, true);
                }
            );

            Router.addHandler(
                new[] { "+мешок", "-мешок", "запретить", "разрешить" },
                new Regex(@"^[./! ]?[ ]?(-[ ]?|/+[ ]?|запретить |разрешить )мешок[ ]*(?:\n+.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
                async (Message Msg, Bot IrisBot, Match Match, IrisApiClient IrisApi) =>
                {
                    if (Msg.From == null) return;

                    string value = Match.Groups[1].Value.ToLower();
                    bool Status = value == "+" || value == "разрешить";

                    var result = await IrisApi.OpenBag(Status);

                    if (result.response == "true")
                    {
                        await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, $"📝 Вы {(Status == true ? "открыли" : "закрыли")} мешок для других глаз");
                        return;
                    }
                    else
                    {
                        await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, $"📝 Статус мешка и так: {(Status == true ? "открыт" : "закрыт")}");
                        return;
                    }
                }
            );

            Router.addHandler(
                new[] { "запретить", "разрешить", "+переводы", "-переводы" },
                new Regex(@"^[./! ]?[ ]?(-[ ]?|/+[ ]?|запретить |разрешить )переводы (для всех|(.+))[ ]*(?:\n+.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline),
                async (Message Msg, Bot IrisBot, Match Match, IrisApiClient IrisApi) =>
                {
                    string statusString = Match.Groups[1].Value.ToLower();
                    string Entity = Match.Groups[2].Value.ToLower();
                    bool Status = statusString == "+" || statusString == "разрешить";


                    if (Entity == "для всех")
                    {
                        var result = await IrisApi.AllowAll(Status);
                        if (result.response == "true")
                        {
                            await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, $"📝 Вы {(Status == true ? "разрешили" : "запретили")} переводы для других");
                            return;
                        }
                        else
                        {
                            await Router.MessageSend(Msg, Msg.Chat.Id, IrisBot, $"📝 Статус переводов для всех и так: {(Status == true ? "разрешен" : "запрещён")}");
                            return;
                        }
                    }
                    else
                    {
                        // Пока не готово и с костылями
                        return;
                        
                    }
                }
            );
        }
    }
}