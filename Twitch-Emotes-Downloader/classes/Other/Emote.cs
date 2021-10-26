using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Emotes_Downloader
{
	public class Emote
	{
		public string Name { get; set; } = "";
		public string Link { get; set; } = "";

		public Emote() { }

		public Emote(string name = "", string link = "")
		{
			Name = name;
			Link = link;
		}

		public override string ToString()
		{
			return $"{Name}: {Link}";
		}
	}
}
