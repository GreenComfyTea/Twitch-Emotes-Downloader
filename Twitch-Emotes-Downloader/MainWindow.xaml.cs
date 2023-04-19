using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Ookii.Dialogs.Wpf;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
namespace Twitch_Emotes_Downloader
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Config config = null;

		private bool IsTwitchApiBusy { get; set; }
		private bool IsBttvApiBusy { get; set; }
		private bool IsFfzApiBusy { get; set; }

		public static MainWindow MainWndw { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			MainWndw = this;

			saveToTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			LoadConfig();
		}

		private async void LoadConfig()
		{
			config = await ConfigHelper.LoadConfigAsync();

			if (config != null)
			{
				clientIdTextBox.Text = config.ClientID;
				oauthTokenTextBox.Text = config.OAuthToken;
			}
			else
			{
				config = new Config();
			}

			UpdateUI();

		}

		private void UpdateUI()
		{
			if (clientIdTextBox.Text != string.Empty && oauthTokenTextBox.Text != string.Empty  && saveToTextBox.Text != string.Empty)
			{
				downloadGlobalTwitchEmotesButton.IsEnabled = !IsTwitchApiBusy;
				downloadGlobalBttvEmotesButton.IsEnabled = !IsBttvApiBusy;
				downloadSpecialTwitchEmotesButton.IsEnabled = !IsTwitchApiBusy;

				downloadGlobalTwitchBadgesButton.IsEnabled = !IsTwitchApiBusy;
				downloadGlobalBttvBadgesButton.IsEnabled = !IsBttvApiBusy;

				downloadAllGlobalEmotesButton.IsEnabled = !(IsTwitchApiBusy || IsBttvApiBusy);
				downloadAllGlobalBadgesButton.IsEnabled = !(IsTwitchApiBusy || IsBttvApiBusy);
				downloadAllGlobalEmotesAndBadgesButton.IsEnabled = !(IsTwitchApiBusy || IsBttvApiBusy);

				if (channelNameTextBox.Text != string.Empty)
				{
					downloadChannelTwitchEmotesButton.IsEnabled = !IsTwitchApiBusy;
					downloadChannelBttvEmotesButton.IsEnabled = !IsBttvApiBusy;
					downloadChannelFfzEmotesButton.IsEnabled = !IsFfzApiBusy;

					downloadChannelTwitchBadgesButton.IsEnabled = !IsTwitchApiBusy;
					downloadChannelFfzBadgesButton.IsEnabled = !IsFfzApiBusy;

					downloadAllChannelEmotesButton.IsEnabled = !(IsTwitchApiBusy || IsBttvApiBusy || IsFfzApiBusy);
					downloadAllChannelBadgesButton.IsEnabled = !(IsTwitchApiBusy || IsFfzApiBusy);
					downloadAllChannelEmotesAndBadgesButton.IsEnabled = !(IsTwitchApiBusy || IsBttvApiBusy || IsFfzApiBusy);
				}
				else
				{
					downloadChannelTwitchEmotesButton.IsEnabled = false;
					downloadChannelBttvEmotesButton.IsEnabled = false;
					downloadChannelFfzEmotesButton.IsEnabled = false;

					downloadChannelTwitchBadgesButton.IsEnabled = false;
					downloadChannelFfzBadgesButton.IsEnabled = false;

					downloadAllChannelEmotesButton.IsEnabled = false;
					downloadAllChannelBadgesButton.IsEnabled = false;
					downloadAllChannelEmotesAndBadgesButton.IsEnabled = false;
				}
			}
			else
			{
				downloadChannelTwitchEmotesButton.IsEnabled = false;
				downloadGlobalTwitchEmotesButton.IsEnabled = false;
				downloadSpecialTwitchEmotesButton.IsEnabled = false;

				downloadChannelBttvEmotesButton.IsEnabled = false;
				downloadChannelFfzEmotesButton.IsEnabled = false;

				downloadGlobalBttvEmotesButton.IsEnabled = false;
				downloadGlobalBttvBadgesButton.IsEnabled = false;

				downloadChannelTwitchBadgesButton.IsEnabled = false;
				downloadGlobalTwitchBadgesButton.IsEnabled = false;
				downloadChannelFfzBadgesButton.IsEnabled = false;

				downloadAllChannelEmotesButton.IsEnabled = false;
				downloadAllChannelBadgesButton.IsEnabled = false;
				downloadAllChannelEmotesAndBadgesButton.IsEnabled = false;

				downloadAllGlobalEmotesButton.IsEnabled = false;
				downloadAllGlobalBadgesButton.IsEnabled = false;
				downloadAllGlobalEmotesAndBadgesButton.IsEnabled = false;
			}
		}

		public void ConsoleWriteLine(string line)
		{
			Dispatcher.Invoke(() =>
			{
				if(consoleTextBox.Text == string.Empty)
				{
					consoleTextBox.Text += line;
				}
				else
				{
					consoleTextBox.Text += "\n" + line;
				}

				consoleScrollViewer.ScrollToEnd();
			});
		}
		private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = e.Uri.ToString(),
				UseShellExecute = true
			};
			Process.Start(processStartInfo);
		}

		private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newHeight = outerScrollViewer.ActualHeight - row1.ActualHeight - row2.ActualHeight - row3.ActualHeight - consoleScrollViewer.Margin.Top - consoleScrollViewer.Margin.Bottom;

			if(newHeight > consoleScrollViewer.MinHeight)
			{
				consoleScrollViewer.MaxHeight = newHeight;
			}
			else
			{
				consoleScrollViewer.MaxHeight = consoleScrollViewer.MinHeight;
			}
		}

		private void ClientIdChanged(object sender, RoutedEventArgs e)
		{
			if (clientIdTextBox.IsLoaded && config != null)
			{
				config.ClientID = clientIdTextBox.Text;

				UpdateUI();
			}
		}

		private void OAuthTokenChanged(object sender, RoutedEventArgs e)
		{
			if (oauthTokenTextBox.IsLoaded && config != null)
			{
				config.OAuthToken = oauthTokenTextBox.Text;

				UpdateUI();
			}
		}

		private void SaveToChanged(object sender, TextChangedEventArgs e)
		{
			if (saveToTextBox.IsLoaded)
			{
				UpdateUI();
			}
		}

		private void SaveToBrowseClick(object sender, RoutedEventArgs e)
		{
			var dialog = new VistaFolderBrowserDialog();
			if (dialog.ShowDialog(this).GetValueOrDefault())
			{
				if (dialog.SelectedPath.Length > 0)
				{
					saveToTextBox.Text = dialog.SelectedPath;
				}
			}
		}

		private void ChannelNameChanged(object sender, TextChangedEventArgs e)
		{
			if (channelNameTextBox.IsLoaded && config != null)
			{
				config.SavePath = saveToTextBox.Text;

				UpdateUI();
			}
		}

		private async void DownloadChannelTwitchEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadChannelTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.TWITCH), channelName));

			IsTwitchApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadChannelTwitchEmotesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await TwitchApiHandler.GetChannelEmotesAsync(channelName);

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Channel Twitch Emotes...");
				
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadGlobalTwitchEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadGlobalTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.TWITCH)));

			IsTwitchApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadGlobalTwitchEmotesAsync(string clientID, string oauthToken, string savePath)
		{

			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await TwitchApiHandler.GetGlobalEmotesAsync();

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Global Twitch Emotes...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadSpecialTwitchEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			UpdateUI();
			
			await Task.Run(() => DownloadSpecialTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.SPECIAL_EMOTE_FOLDER_NAME)));

			IsTwitchApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadSpecialTwitchEmotesAsync(string clientID, string oauthToken, string savePath)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await TwitchApiHandler.GetSpecialEmotesAsync();

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Special Twitch Emotes...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadChannelBttvEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadChannelBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.BTTV), channelName));

			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadChannelBttvEmotesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await BttvApiHandler.GetChannelBttvEmotesAsync(channelName);

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Channel BTTV Emotes...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadGlobalBttvEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadGlobalBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.BTTV)));

			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadGlobalBttvEmotesAsync(string clientID, string oauthToken, string savePath)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await BttvApiHandler.GetGlobalBttvEmotesAsync();

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Global BTTV Emotes...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadChannelFfzEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsFfzApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadChannelFfzEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.FFZ), channelName));

			IsFfzApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadChannelFfzEmotesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await FfzApiHandler.GetChannelFfzEmotesAsync(channelName);
			//var emotes = await BttvApiHandler.GetChannelFfzEmotesAsync(channelName);

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Channel FFZ Emotes...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadChannelTwitchBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadChannelTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.TWITCH), channelName));

			IsTwitchApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadChannelTwitchBadgesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var badges = await TwitchApiHandler.GetChannelBadgesAsync(channelName);

			if (badges != null)
			{
				ConsoleWriteLine("Downloading Channel Twitch Badges...");
				ImageDownloader.DownloadEmotes(badges, savePath);
			}
		}

		private async void DownloadGlobalTwitchBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadGlobalTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.TWITCH)));

			IsTwitchApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadGlobalTwitchBadgesAsync(string clientID, string oauthToken, string savePath)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await TwitchApiHandler.GetGlobalBadgesAsync();

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Global Twitch Badges...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadGlobalBttvBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadGlobalBttvBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.BTTV)));
			
			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadGlobalBttvBadgesAsync(string clientID, string oauthToken, string savePath)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var emotes = await BttvApiHandler.GetGlobalBttvBadgesAsync();

			if (emotes != null)
			{
				ConsoleWriteLine("Downloading Global BTTV badges...");
				ImageDownloader.DownloadEmotes(emotes, savePath);
			}
		}

		private async void DownloadChannelFfzBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsFfzApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadChannelFfzBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.FFZ), channelName));

			IsFfzApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadChannelFfzBadgesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			TwitchApiHandler.UpdateAuthInfo(clientID, oauthToken);
			var badges = await FfzApiHandler.GetChannelFfzBadgesAsync(channelName);

			if (badges != null)
			{
				ConsoleWriteLine("Downloading Channel FFZ Badges...");
				ImageDownloader.DownloadEmotes(badges, savePath);
			}
		}

		private async void DownloadAllChannelEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsBttvApiBusy = true;
			IsFfzApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllChannelEmotesAsync(clientID, oauthToken, savePath, channelName));

			IsTwitchApiBusy = false;
			IsBttvApiBusy = false;
			IsFfzApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllChannelEmotesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			Task channelTwitchEmotes = Task.Run(() => DownloadChannelTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.TWITCH), channelName));
			Task channelBttvEmotes = Task.Run(() => DownloadChannelBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.BTTV), channelName));
			Task channelFfzEmotes = Task.Run(() => DownloadChannelFfzEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.FFZ), channelName));

			await Task.WhenAll(channelTwitchEmotes, channelBttvEmotes, channelFfzEmotes);
		}

		private async void DownloadAllGlobalEmotesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllGlobalEmotes(clientID, oauthToken, savePath));

			IsTwitchApiBusy = false;
			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllGlobalEmotes(string clientID, string oauthToken, string savePath)
		{
			Task globalTwitchEmotes = Task.Run(() => DownloadGlobalTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.TWITCH)));
			Task globalBttvEmotes = Task.Run(() => DownloadGlobalBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.BTTV)));

			await Task.WhenAll(globalTwitchEmotes, globalBttvEmotes);
		}

		private async void DownloadAllChannelBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsFfzApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllChannelBadgesAsync(clientID, oauthToken, savePath, channelName));

			IsTwitchApiBusy = false;
			IsFfzApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllChannelBadgesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			Task channelTwitchBadges = Task.Run(() => DownloadChannelTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.TWITCH), channelName));
			Task channelFfzBadges = Task.Run(() => DownloadChannelFfzBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.FFZ), channelName));

			await Task.WhenAll(channelTwitchBadges, channelFfzBadges);
		}

		private async void DownloadAllGlobalBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllGlobalBadgesAsync(clientID, oauthToken, savePath));

			IsTwitchApiBusy = false;
			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllGlobalBadgesAsync(string clientID, string oauthToken, string savePath)
		{
			Task globalTwitchBadges = Task.Run(() => DownloadGlobalTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.TWITCH)));
			Task globalBttvBadges = Task.Run(() => DownloadGlobalBttvBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.BTTV)));

			await Task.WhenAll(globalTwitchBadges, globalBttvBadges);
		}

		private async void DownloadAllChannelEmotesAndBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsBttvApiBusy = true;
			IsFfzApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllChannelEmotesAndBadgesAsync(clientID, oauthToken, savePath, channelName));

			IsTwitchApiBusy = false;
			IsBttvApiBusy = false;
			IsFfzApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllChannelEmotesAndBadgesAsync(string clientID, string oauthToken, string savePath, string channelName)
		{
			Task channelTwitch = Task.Run(async () =>
			{
				await DownloadChannelTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.TWITCH), channelName);
				await DownloadChannelTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.TWITCH), channelName);
			});

			Task channelBttv = Task.Run(() => DownloadChannelBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.BTTV), channelName));

			Task channelFfz = Task.Run(async () =>
			{
				await DownloadChannelFfzEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, channelName, EmoteSource.FFZ), channelName);
				await DownloadChannelFfzBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, channelName, EmoteSource.FFZ), channelName);
			});

			await Task.WhenAll(channelTwitch, channelBttv, channelFfz);
		}

		private async void DownloadAllGlobalEmotesAndBadgesAsyncClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(saveToTextBox.Text))
			{
				ConsoleWriteLine("Invalid Save Path. Process Aborted.");
				return;
			}

			string clientID = clientIdTextBox.Text;
			string oauthToken = oauthTokenTextBox.Text;
			string savePath = saveToTextBox.Text;
			string channelName = channelNameTextBox.Text;

			ConfigHelper.SaveConfigAsync(config);

			IsTwitchApiBusy = true;
			IsBttvApiBusy = true;
			UpdateUI();

			await Task.Run(() => DownloadAllGlobalEmotesAndBadgesAsync(clientID, oauthToken, savePath));

			IsTwitchApiBusy = false;
			IsBttvApiBusy = false;
			UpdateUI();

			ConsoleWriteLine("All Done!");
		}

		private async Task DownloadAllGlobalEmotesAndBadgesAsync(string clientID, string oauthToken, string savePath)
		{
			Task globalTwitch = Task.Run(async () =>
			{
				await DownloadGlobalTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.TWITCH));
				await DownloadSpecialTwitchEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.SPECIAL_EMOTE_FOLDER_NAME, EmoteSource.TWITCH));
				await DownloadGlobalTwitchBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.TWITCH));
			});

			Task globalBttv = Task.Run(async () =>
			{
				await DownloadGlobalBttvEmotesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.EMOTES, ChannelName.GLOBAL, EmoteSource.BTTV));
				await DownloadGlobalBttvBadgesAsync(clientID, oauthToken, Path.Combine(savePath, EmoteType.BADGES, ChannelName.GLOBAL, EmoteSource.BTTV));
			});

			await Task.WhenAll(globalTwitch, globalBttv);
		}
	}
}
