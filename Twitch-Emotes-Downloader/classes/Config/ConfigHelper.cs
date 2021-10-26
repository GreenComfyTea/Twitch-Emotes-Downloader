using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Twitch_Emotes_Downloader
{
	public class ConfigHelper
	{
		private static string ConfigFileName { get; } = "config.json";

		public static async Task<Config> LoadConfigAsync()
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			if (File.Exists(ConfigFileName))
			{
				using FileStream configFileStream = File.OpenRead(ConfigFileName);
				Config config = await JsonSerializer.DeserializeAsync<Config>(configFileStream);
				mainWindow.ConsoleWriteLine("Config File Loaded.");
				return config;
			}
			else
			{
				mainWindow.ConsoleWriteLine("No Config File Found.");
				return null;
			}
		}

		public static async Task SaveConfigAsync(Config config)
		{
			MainWindow mainWindow = MainWindow.MainWndw;

			using FileStream configFileStream = File.Create(ConfigFileName);
			await JsonSerializer.SerializeAsync(configFileStream, config);
			await configFileStream.DisposeAsync();
			mainWindow.ConsoleWriteLine("Config File Saved.");
		}
	}
}
