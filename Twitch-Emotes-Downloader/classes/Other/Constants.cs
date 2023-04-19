using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Emotes_Downloader
{
	public class EmoteSource
	{
		public const string TWITCH = "twitch";
		public const string BTTV = "bttv";
		public const string FFZ = "ffz";
	}

	public class EmoteType
	{
		public const string EMOTES = "emotes";
		public const string BADGES = "badges";
	}

	public class ChannelName
	{
		public const string GLOBAL = "global";
		public const string SPECIAL_EMOTE_CHANNEL = "qa_TW_Partner";
		public const string SPECIAL_EMOTE_FOLDER_NAME = "special twitch emotes (qa_TW_Partner)";
	}
}
