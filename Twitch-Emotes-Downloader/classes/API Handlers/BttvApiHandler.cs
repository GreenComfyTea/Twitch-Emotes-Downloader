
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
	public class BttvApiHandler
	{
		public static async Task<ConcurrentBag<Emote>> GetChannelBttvEmotesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting BTTV Channel Emote Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.betterttv.net/3/cached/users/twitch", channelName);

				ConcurrentBag<Emote> emotes = new();

				int[] indices = Enumerable.Range(0, (int)responseJson["channelEmotes"].Count).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = responseJson["channelEmotes"][index];

					emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", $"https://cdn.betterttv.net/emote/{emote["id"]}/3x"));
				});

				indices = Enumerable.Range(0, (int)responseJson["sharedEmotes"].Count).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = responseJson["sharedEmotes"][index];

					emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", $"https://cdn.betterttv.net/emote/{emote["id"]}/3x"));
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Enotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting BTTV Channel Emote Links... Done!");

				return emotes;
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

		public static async Task<ConcurrentBag<Emote>> GetChannelFfzEmotesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting FFZ Channel Emote Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.betterttv.net/3/cached/frankerfacez/users/twitch", channelName);

				ConcurrentBag <Emote> emotes = new();

				int[] indices = Enumerable.Range(0, (int) responseJson.Count).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = responseJson[index];
					if(DynamicHelper.HasProperty(emote["image"], "4x"))
					{
						emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", (string)emote["images"]["4x"]));
					}
					else if (DynamicHelper.HasProperty(emote["image"], "2x"))
					{
						emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", (string)emote["images"]["2x"]));
					}
					else if (DynamicHelper.HasProperty(emote["image"], "1x"))
					{
						emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", (string)emote["images"]["1x"]));
					}
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Enotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting FFZ Channel Emote Links... Done!");

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
				while(ex != null)
				{
					mainWindow.ConsoleWriteLine(ex.Message);
					Debug.WriteLine(ex.Message);
					ex = ex.InnerException;
				}
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetGlobalBttvEmotesAsync()
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Global BTTV Emote Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.betterttv.net/3/cached/emotes/global");

				ConcurrentBag<Emote> emotes = new();

				int[] indices = Enumerable.Range(0, (int)responseJson.Count).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = responseJson[index];
					emotes.Add(new Emote($"{emote["code"]}.{emote["imageType"]}", $"https://cdn.betterttv.net/emote/{emote["id"]}/3x"));
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Enotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Global BTTV Emote Links... Done!");

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

		public static async Task<ConcurrentBag<Emote>> GetGlobalBttvBadgesAsync()
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Global BTTV Badge Links...");

				var responseJson = await WebClientHelper.GetJsonAsync("https://api.betterttv.net/3/cached/badges");

				ConcurrentDictionary<string, Emote> badgesDictionary = new();

				int[] indices = Enumerable.Range(0, (int)responseJson.Count).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var badge = responseJson[index]["badge"];
					string name = $"{badge["description"]}.svg";
					badgesDictionary.TryAdd(name, new Emote(name, badge["svg"]));
				});

				ConcurrentBag<Emote> badges = new();

				foreach(var badge in badgesDictionary)
				{
					badges.Add(badge.Value);
				}

				if (badges.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Badges found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Global BTTV Badge Links... Done!");

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
