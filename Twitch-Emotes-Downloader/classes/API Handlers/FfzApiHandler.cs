using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Twitch_Emotes_Downloader
{
	public class FfzApiHandler
	{
		public static async Task<ConcurrentBag<Emote>> GetChannelFfzEmotesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Channel FFZ Emote Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.frankerfacez.com/v1/room/id", channelName);

				ConcurrentBag<Emote> emotes = new();

				var emoteSet = $"{responseJson["room"]["set"]}";
				int[] indices = Enumerable.Range(0, (int)responseJson["sets"][emoteSet]["emoticons"].Count).ToArray();

				Parallel.ForEach(indices, index =>
				{
					var emote = responseJson["sets"][emoteSet]["emoticons"][index];

					if(DynamicHelper.HasProperty(emote["urls"], "4"))
					{
						emotes.Add(new Emote($"{emote["name"]}.png", $"https:{emote["urls"]["4"]}"));
					}
					else if (DynamicHelper.HasProperty(emote["urls"], "2"))
					{
						emotes.Add(new Emote($"{emote["name"]}.png", $"https:{emote["urls"]["2"]}"));
					}
					else if (DynamicHelper.HasProperty(emote["urls"], "1"))
					{
						emotes.Add(new Emote($"{emote["name"]}.png", $"https:{emote["urls"]["1"]}"));
					}
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Emotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Channel FFZ Emote Links... Done!");

				return emotes;
			}
			catch (TwitchLib.Api.Core.Exceptions.BadScopeException badScopeEx)
			{
				mainWindow.ConsoleWriteLine("Authentication Failure... Make sure that Client ID and OAuth Token are correct.");
				Debug.WriteLine(badScopeEx.Message);
				return null;
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mainWindow.ConsoleWriteLine(ex.Message);
					Debug.WriteLine(ex.Message);
					ex = ex.InnerException;
				}
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetChannelFfzBadgesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Channel FFZ Badge Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.frankerfacez.com/v1/room/id", channelName);

				ConcurrentBag<Emote> badges = new();

				if (DynamicHelper.HasProperty(responseJson["room"]["mod_urls"], "4"))
				{
					badges.Add(new Emote("moderator_badge.png", $"https:{responseJson["room"]["mod_urls"]["4"]}"));
				}
				else if (DynamicHelper.HasProperty(responseJson["room"]["mod_urls"], "2"))
				{
					badges.Add(new Emote("moderator_badge.png", $"https:{responseJson["room"]["mod_urls"]["2"]}"));
				}
				else if (DynamicHelper.HasProperty(responseJson["room"]["mod_urls"], "1"))
				{
					badges.Add(new Emote("moderator_badge.png", $"https:{responseJson["room"]["mod_urls"]["1"]}"));
				}

				if(DynamicHelper.HasProperty(responseJson["room"]["vip_badge"], "4"))
				{
					badges.Add(new Emote("vip_badge.png", $"https:{responseJson["room"]["vip_badge"]["4"]}"));
				}
				else if (DynamicHelper.HasProperty(responseJson["room"]["vip_badge"], "2"))
				{
					badges.Add(new Emote("vip_badge.png", $"https:{responseJson["room"]["vip_badge"]["2"]}"));
				}
				else if (DynamicHelper.HasProperty(responseJson["room"]["vip_badge"], "1"))
				{
					badges.Add(new Emote("vip_badge.png", $"https:{responseJson["room"]["vip_badge"]["1"]}"));
				}

				if (badges.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Badges found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Channel FFZ Badge Links... Done!");

				return badges;
			}
			catch (TwitchLib.Api.Core.Exceptions.BadScopeException badScopeEx)
			{
				mainWindow.ConsoleWriteLine("Authentication Failure... Make sure that Client ID and OAuth Token are correct.");
				Debug.WriteLine(badScopeEx.Message);
				return null;
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					mainWindow.ConsoleWriteLine(ex.Message);
					Debug.WriteLine(ex.Message);
					ex = ex.InnerException;
				}
				return null;
			}
		}
	}
}
