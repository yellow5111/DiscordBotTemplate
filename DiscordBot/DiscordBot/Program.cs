using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiscordBot
{
    class Program
    {
        private DiscordSocketClient _client;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            string token = GetTokenFromXml("Token.xml");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.MessageReceived += MessageReceivedAsync;

            await Task.Delay(-1);
        }

        private string GetTokenFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);
                // <Token>
                //   <BotToken>YOUR_BOT_TOKEN</BotToken>
                // </Token>
                return doc.Root.Element("DiscordBotToken").Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading token from XML: {ex.Message}");
                throw;
            }
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Console.WriteLine($"Received message: {message.Content} from {message.Author.Username}");

            if (message.Author.IsBot)
                return;

            string cleanedContent = message.Content;

            foreach (var user in message.MentionedUsers)
            {
                cleanedContent = cleanedContent.Replace($"<@{user.Id}>", "")
                                               .Replace($"<@!{user.Id}>", "");
            }

            foreach (var role in message.MentionedRoles)
            {
                cleanedContent = cleanedContent.Replace($"<@&{role.Id}>", "");
            }

            cleanedContent = cleanedContent.Trim();

            if (cleanedContent.Equals("/gp", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("'/gp' command recognized.");
                if (message.Channel is SocketGuildChannel)
                {
                    try
                    {
                        await message.Channel.SendMessageAsync("I can't tell you what's coming yet, but it's exciting, so you should stay tuned...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message: {ex.Message}");
                    }
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
