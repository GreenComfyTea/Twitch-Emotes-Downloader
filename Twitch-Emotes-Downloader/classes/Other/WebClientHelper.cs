using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Twitch_Emotes_Downloader
{
	public class WebClientHelper
	{
		public static async Task<dynamic> GetJsonAsync(string endpoint, string channelName = null)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			string channelID = "";
			if (channelName != null)
			{
				channelID = await TwitchApiHandler.NicknameToIdAsync(channelName);

				if (channelID == TwitchApiHandler.failure)
				{
					mainWindow.ConsoleWriteLine("Failure... Make sure that Channel Name is correct.");
					return null;
				}
			}

			try
			{
				WebClient web = new();
				string url = channelName == null ? endpoint : $"{endpoint}/{channelID}";
				string responseString = web.DownloadString(url);

				return JsonSerializer.Deserialize<dynamic>(responseString);
			}
			catch (TwitchLib.Api.Core.Exceptions.BadScopeException badScopeEx)
			{
				mainWindow.ConsoleWriteLine("Authentication Failure... Make sure that Client ID and OAuth Token are correct.");
				Debug.WriteLine(badScopeEx.Message);
				return null;
			}
		}
	}
}
