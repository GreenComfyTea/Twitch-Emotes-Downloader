using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twitch_Emotes_Downloader
{
	public class ImageDownloader
	{
		public static  void DownloadEmotes(ConcurrentBag<Emote> emotes, string savePath)
		{
			WebClient webClient = new();

			MainWindow mainWindow = MainWindow.MainWndw;

			Directory.CreateDirectory(savePath);

			foreach (var emote in emotes)
			{
				//Forbidden name symbols in Windows: / \ : * ? " < > |
				string emoteFileName = emote.Name.Replace("/", "∕").Replace("\\", "⧵").Replace(":", "∶").Replace("*", "∗")
					.Replace("?", "？").Replace("\"", "″").Replace("<", "‹").Replace(">", "›").Replace("|", "ǀ");
				string emoteFileFullName = Path.Combine(savePath, emoteFileName);

				// Downloading
				try
				{
					webClient.DownloadFile(emote.Link, emoteFileFullName);
				}
				catch (Exception ex)
				{
					while (ex != null)
					{
						Debug.WriteLine(emote.Name + ": " + ex.Message);
						mainWindow.ConsoleWriteLine(emote.Name + ": " + ex.Message);
						ex = ex.InnerException;
					}
					
					continue;
				}

				if (!emote.Name.EndsWith(".png") && !emote.Name.EndsWith(".gif"))
				{
					if (ImageFormatHandler.IsPng(emoteFileFullName, mainWindow))
					{
						string newEmoteFileFullName = Path.ChangeExtension(emoteFileFullName, ".png");
						if (File.Exists(newEmoteFileFullName))
						{
							File.Delete(newEmoteFileFullName);
						}

						File.Move(emoteFileFullName, newEmoteFileFullName);
						emoteFileFullName = newEmoteFileFullName;
						emoteFileName = Path.ChangeExtension(emoteFileName, ".png");
					}
					else if (ImageFormatHandler.IsGif(emoteFileFullName, mainWindow))
					{
						string newEmoteFileFullName = Path.ChangeExtension(emoteFileFullName, ".gif");
						if (File.Exists(newEmoteFileFullName))
						{
							File.Delete(newEmoteFileFullName);
						}

						File.Move(emoteFileFullName, newEmoteFileFullName);
						emoteFileFullName = newEmoteFileFullName;
						emoteFileName = Path.ChangeExtension(emoteFileName, ".gif");
					}
				}

				// Was the download successful?
				if (File.Exists(emoteFileFullName))
				{
					mainWindow.ConsoleWriteLine(emoteFileName + " downloaded!");
				}
				else
				{
					mainWindow.ConsoleWriteLine(emoteFileName + " download failed!");
				}
			}
		}
	}
}
