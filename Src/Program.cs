using Microsoft.Data.Sqlite;
using Src;
using WTelegram;
using WTelegram.Types;
using System.Threading.Channels;
using Telegram.Bot.Types.Enums;
using IrisClient.Services;

var (apiId, apiHash, botToken, PathSession, Commands) = IrisConfig.getSetting();

using var Connection = new SqliteConnection($"Data Source={PathSession}");
Connection.Open();

using (var cmd = Connection.CreateCommand())
{
    cmd.CommandText = Commands;
    cmd.ExecuteNonQuery();
}

using var IrisBot = new Bot(botToken, apiId, apiHash, Connection);
var IrisApi = IrisApiClient.Create();

var Me = await IrisBot.GetMe();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"[LOG MESSAGE] 🟢 {Me.FirstName} run in Telegram: @{Me.Username}, {Me.Id}");
Console.ResetColor();

ModuleRegister.RegisterAll();

var MessageChannel = Channel.CreateUnbounded<Message>();
var UpdateChannel = Channel.CreateUnbounded<Update>();

IrisBot.OnMessage += async (Msg, _) => await MessageChannel.Writer.WriteAsync(Msg);

_ = Task.Run(async () =>
{
    await foreach (var Msg in MessageChannel.Reader.ReadAllAsync())
        _ = Task.Run(() => HandleMessage(Msg));
});

await Task.Delay(-1);

void HandleMessage(Message Msg)
{

    if (Msg.ForwardFrom == null && Msg.Chat.Type != ChatType.Channel)
        _ = Task.Run(() => Router.Route(Msg, IrisBot, IrisApi));
}