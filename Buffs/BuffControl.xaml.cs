using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Buffs.Properties;

namespace Buffs
{
	public partial class BuffControl
	{
		Timer _timer;
		private int counter = 0;

		private readonly SolidColorBrush BuffOnBrush;
		private readonly SolidColorBrush TransparentBrush;
		private readonly SolidColorBrush RedBrush;

		public Buff Buff { get; private set; }

		public BuffControl()
		{
			InitializeComponent();

			TransparentBrush = new SolidColorBrush(Colors.Transparent);
			BuffOnBrush = new SolidColorBrush(Colors.Green);
			RedBrush = new SolidColorBrush(Colors.Red);
		}

		public BuffControl(ImageSource imageSource, Buff buff): this()
		{
			Buff = buff;
			_image.Source = imageSource;

			Tune();

			_timer = new Timer(state =>
			{
				if (Dispatcher.CheckAccess())
					Tune();
				else
					Dispatcher.Invoke(OnTimer);
			}, null, TimeSpan.FromSeconds(1), Settings.Default.TimerInterval);
		}

		private void OnTimer()
		{
			if (Dispatcher.CheckAccess())
			{
				Tune();
				counter++;
			}
			else
				Dispatcher.Invoke(OnTimer);
		}

		private void Tune()
		{
			if (Buff.IsOn)
			{
				_border.BorderBrush = BuffOnBrush;
				if (Buff.Remain < Settings.Default.FlashBefore)
				{
					_warning.Visibility = Visibility.Visible;
					_warning.Fill = counter % 2 == 0 ? RedBrush : TransparentBrush;
				}
				else
					_warning.Visibility = Visibility.Collapsed;
				_durationBtn.Text = ToString(Buff.Remain.Value);
				_durationMI.Visibility = Visibility.Visible;
			}
			else
			{
				_border.BorderBrush = TransparentBrush;
				_warning.Visibility = Visibility.Collapsed;
				_durationBtn.Text = string.Empty;
				_durationMI.Visibility = Visibility.Collapsed;
			}
		}

		private void OnBuffClick(object sender, RoutedEventArgs e)
		{
			Buff.On();
			Tune();
		}

		private static string ToString(TimeSpan value)
		{
			if (value.TotalHours > 1)
				return (int)value.TotalHours + " ч.";

			if (value.TotalMinutes > 1)
				return (int)value.TotalMinutes + " м.";

			if (value.TotalSeconds < 0)
				return "-";

			return (int)value.TotalSeconds + " с.";
		}

		private void OnOffBuffClick(object sender, RoutedEventArgs e)
		{
			Buff.Off();
			Tune();
		}

		private void OnDurationClick(object sender, RoutedEventArgs e)
		{
			var c = (FrameworkElement)this;
			Window w;
			do
			{
				c = (FrameworkElement)c.Parent;
				w = c as Window;
			} while (w == null);

			new BuffWindow(Buff) { Owner = w }.ShowDialog();
		}
	}
}
