using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Emotes_Downloader
{
	public class Config
	{
		public string ClientID { get; set; } = "";
		public string OAuthToken { get; set; } = "";
		public string SavePath { get; set; } = "";

		public Config() { }

		public Config(string twitchNickname, string twitchOAuthToken, string savePath)
		{
			ClientID = twitchNickname;
			OAuthToken = twitchOAuthToken;
			SavePath = savePath;
		}
	}
}
