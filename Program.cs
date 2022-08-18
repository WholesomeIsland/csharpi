using System;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Net;
using System.IO;
using System.Text;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Linq;

namespace csharpi
{
    class Program
    {
        string error = "Error: Card Not Found. check your spelling and try again";
        private readonly DiscordSocketClient _client;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync(string[] args)
        {
            
        }

        public Program()
        {
            _client = new DiscordSocketClient();

            //Hook into log event and write it out to the console
            _client.Log += Log;

            //Hook into the client ready event
            _client.Ready += Ready;

            //Hook into the message received event, this is how we handle the hello world example
            _client.MessageReceived += MessageReceivedAsync;

            //Create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");            
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            //This is where we get the Token value from the configuration file
            await _client.LoginAsync(TokenType.Bot, _config["Token"]);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task Ready()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        //I wonder if there's a better way to handle commands (spoiler: there is :))
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //This ensures we don't loop things by responding to ourselves (as the bot)
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith("!spell"))
            {
                try
                {
                    var urlAddOn = message.Content.Split("!spell ")[1];
                    string urlAddress = "https://becomemagi.morallygray.net/card.php?id=" + urlAddOn;
                    var client = new HttpClient();
                     var parser = new HtmlParser();
                    using var doc = parser.ParseDocument(await client.GetStringAsync(new Uri(urlAddress)));
                    string imgUrl1 = doc.QuerySelectorAll("img.front").Select(el => el.GetAttribute("src")).ToArray()[0];
                    foreach (var item in doc.Images)
                    {
                        Console.WriteLine(item.Source);
                    }

                    if (client.Send(new HttpRequestMessage(HttpMethod.Get, imgUrl1)).StatusCode == HttpStatusCode.NotFound)
                    {
                        await message.Channel.SendMessageAsync(error);
                        return;
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("https://becomemagi.morallygray.net/card.php?id=" + urlAddOn);
                    }
                }
                
                catch (Exception)
                {
                    await message.Channel.SendMessageAsync(error);
                    throw;
                }
            }
            else if (message.Content.StartsWith("!spec"))
            {
                try
                {
                    var urlAddOn = message.Content.Split("!spec ")[1];
                    string urlAddress = "https://becomemagi.morallygray.net/spec.php?id=" + urlAddOn;
                    var client = new HttpClient();
                    var parser = new HtmlParser();
                    using var doc = parser.ParseDocument(await client.GetStringAsync(new Uri(urlAddress)));
                    string imgUrl1 = doc.QuerySelectorAll("img.front").Select(el => el.GetAttribute("src")).ToArray()[0];
                    if (client.Send(new HttpRequestMessage(HttpMethod.Get, imgUrl1)).StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        await message.Channel.SendMessageAsync(error);
                        return;
                    }
                    else
                    {

                        await message.Channel.SendMessageAsync("https://becomemagi.morallygray.net/spec.php?id=" + urlAddOn);
                    }
                }
                catch (Exception)
                {
                    await message.Channel.SendMessageAsync(error);
                    throw;
                }
            }
            else if (message.Content.StartsWith("!artifact"))
            {
                try
                {
                    var urlAddOn = message.Content.Split("!artifact ")[1];
                    string urlAddress = "https://becomemagi.morallygray.net/artifact.php?id=" + urlAddOn;
                    var client = new HttpClient();
                    var parser = new HtmlParser();
                    using var doc = parser.ParseDocument(await client.GetStringAsync(new Uri(urlAddress)));
                    string imgUrl1 = doc.QuerySelectorAll("img.front").Select(el => el.GetAttribute("src")).ToArray()[0];
                    foreach (var item in doc.Images)
                    {
                        Console.WriteLine(item.Source);
                    }
                    if (client.Send(new HttpRequestMessage(HttpMethod.Get, imgUrl1)).StatusCode.Equals(HttpStatusCode.NotFound))
                    {
                        await message.Channel.SendMessageAsync(error);
                        return;
                    }
                    else
                    {
                        Console.WriteLine(imgUrl1);
                        await message.Channel.SendMessageAsync("https://becomemagi.morallygray.net/artifact.php?id=" + urlAddOn);
                    }
                }
                catch (Exception)
                {
                    await message.Channel.SendMessageAsync(error);
                    throw;
                }
            }
        }
    }
}
