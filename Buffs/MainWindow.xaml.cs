using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Buffs.Properties;

namespace Buffs
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			Width = Settings.Default.WindowSize.Width;
			Height = Settings.Default.WindowSize.Height;
			Left = Settings.Default.WindowPosition.X;
			Top = Settings.Default.WindowPosition.Y;

			SizeChanged += (sender, e) =>
			{
				Settings.Default.WindowSize = new System.Drawing.Size((int)Width, (int)Height);
				Settings.Default.Save();
			};
			LocationChanged += (sender1, e1) =>
			{
				Settings.Default.WindowPosition = new System.Drawing.Point((int)Left, (int)Top);
				Settings.Default.Save();
			};

			InitBuffs();
		}

		private void InitBuffs()
		{
			_buttons.Children.Clear();
			foreach (var buff in GetBuffs())
			{
				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = new Uri(buff.Key);
				bitmapImage.EndInit();

				var buffControl = new BuffControl(bitmapImage, buff.Value)
				{
					Margin = new Thickness(5)
				};

				_buttons.Children.Add(buffControl);
			}
		}

		private void OnSettingsClick(object sender, RoutedEventArgs e)
		{
			if (new SettingsWindow { Owner = this }.ShowDialog() == true)
				InitBuffs();
		}

		private static IDictionary<string, Buff> GetBuffs()
		{
			return BuffType.Deserialize(Settings.Default.Types).ToDictionary(bt => bt.ImageUri, bt => new Buff(bt.Duration));
		}
	}
}
