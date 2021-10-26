using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix.Models.Chat.Emotes.GetChannelEmotes;

namespace Twitch_Emotes_Downloader
{
	public class TwitchApiHandler
	{
		private static TwitchAPI twitchApi = new TwitchAPI();

		public static readonly string failure = "FAILURE";

		public static void UpdateAuthInfo(string clientID, string oauthToken)
		{
			twitchApi.Settings.ClientId = clientID;
			twitchApi.Settings.AccessToken = oauthToken;
		}

		public static async Task<string> NicknameToIdAsync(string channelName)
		{
			var users = await twitchApi.Helix.Users.GetUsersAsync(logins: new List<string>() { channelName });
			if (users.Users.Length <= 0)
			{
				return failure;
			}

			return users.Users[0].Id;
		}

		public static async Task<ConcurrentBag<Emote>> GetChannelEmotesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Channel Emote Links...");

				string channelID = await NicknameToIdAsync(channelName);

				if (channelID == failure)
				{
					mainWindow.ConsoleWriteLine("Failure... Make sure that Channel Name is correct.");
					return null;
				}

				var channelEmotes = await twitchApi.Helix.Chat.GetChannelEmotesAsync(channelID);

				if(channelEmotes.ChannelEmotes.Length <= 0)
				{
					mainWindow.ConsoleWriteLine("The remote server returned an error: (404) Not Found.");
					return null;
				}

				ConcurrentBag<Emote> emotes = new();

				int[] indices = Enumerable.Range(0, channelEmotes.ChannelEmotes.Length).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = channelEmotes.ChannelEmotes[index];
					emotes.Add(new Emote(emote.Name, $"https://static-cdn.jtvnw.net/emoticons/v2/{emote.Id}/default/dark/3.0"));
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Emotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Channel Emote Links... Done!");

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
				mainWindow.ConsoleWriteLine(ex.Message);
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetGlobalEmotesAsync()
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Global Twitch Emote Links...");

				var globalEmotes = await twitchApi.Helix.Chat.GetGlobalEmotesAsync();

				ConcurrentBag<Emote> emotes = new();

				int[] indices = Enumerable.Range(0, globalEmotes.GlobalEmotes.Length).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = globalEmotes.GlobalEmotes[index];
					emotes.Add(new Emote(emote.Name, $"https://static-cdn.jtvnw.net/emoticons/v2/{emote.Id}/default/dark/3.0"));
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Emotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Global Twitch Emote Links... Done!");

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
				mainWindow.ConsoleWriteLine(ex.Message);
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetSpecialEmotesAsync(string channelName = ChannelName.SPECIAL_EMOTE_CHANNEL)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Special Emote Links...");

				string channelID = await NicknameToIdAsync(channelName);

				if (channelID == failure)
				{
					mainWindow.ConsoleWriteLine("Failure... Make sure that Channel Name is correct.");
					return null;
				}

				var channelEmotes = await twitchApi.Helix.Chat.GetChannelEmotesAsync(channelID);

				if (channelEmotes.ChannelEmotes.Length <= 0)
				{
					mainWindow.ConsoleWriteLine("The remote server returned an error: (404) Not Found.");
					return null;
				}

				ConcurrentBag<Emote> emotes = new();

				int[] indices = Enumerable.Range(0, channelEmotes.ChannelEmotes.Length).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var emote = channelEmotes.ChannelEmotes[index];

					emotes.Add(new Emote($"{emote.Name}", $"https://static-cdn.jtvnw.net/emoticons/v2/{emote.Id}/default/dark/3.0"));
				});

				if (emotes.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Emotes found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Special Emote Links... Done!");

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
				mainWindow.ConsoleWriteLine(ex.Message);
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetChannelBadgesAsync(string channelName)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Channel Badge Links...");

				string channelID = await NicknameToIdAsync(channelName);

				if(channelID == failure)
				{
					mainWindow.ConsoleWriteLine("Failure... Make sure that Channel Name is correct.");
					return null;
				}

				var channelBadges = await twitchApi.Helix.Chat.GetChannelChatBadgesAsync(channelID);

				if (channelBadges.EmoteSet.Length <= 0)
				{
					mainWindow.ConsoleWriteLine("The remote server returned an error: (404) Not Found.");
					return null;
				}

				ConcurrentBag<Emote> badges = new();

				int[] indices = Enumerable.Range(0, channelBadges.EmoteSet.Length).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var badgeSet = channelBadges.EmoteSet[index];
					foreach (var version in badgeSet.Versions)
					{
						badges.Add(new Emote($"{badgeSet.SetId}_${version.Id}", version.ImageUrl4x));
					}
				});

				if (badges.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Badges found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Channel Badge Links... Done!");

				return badges;
			}
			catch(TwitchLib.Api.Core.Exceptions.BadScopeException badScopeEx)
			{
				mainWindow.ConsoleWriteLine("Authentication Failure... Make sure that Client ID and OAuth Token are correct.");
				Debug.WriteLine(badScopeEx.Message);
				return null;
			}
			catch(Exception ex)
			{
				mainWindow.ConsoleWriteLine(ex.Message);
				Debug.WriteLine(ex.Message);
				return null;
			}
		}

		public static async Task<ConcurrentBag<Emote>> GetGlobalBadgesAsync()
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			try
			{
				mainWindow.ConsoleWriteLine("Getting Global Twitch Badge Links...");

				var globalBadges = await twitchApi.Helix.Chat.GetGlobalChatBadgesAsync();

				ConcurrentBag<Emote> badges = new();

				int[] indices = Enumerable.Range(0, globalBadges.EmoteSet.Length).ToArray();
				Parallel.ForEach(indices, index =>
				{
					var badgeSet = globalBadges.EmoteSet[index];
					foreach (var version in badgeSet.Versions)
					{
						badges.Add(new Emote($"{badgeSet.SetId}_${version.Id}", version.ImageUrl4x));
					}
				});

				if (badges.IsEmpty)
				{
					mainWindow.ConsoleWriteLine("No Badges found!");
					return null;
				}

				mainWindow.ConsoleWriteLine("Getting Global Twitch Badge Links... Done!");

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
				mainWindow.ConsoleWriteLine(ex.Message);
				Debug.WriteLine(ex.Message);
				return null;
			}
		}
	}
}
