using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using EventNotifier.Properties;

namespace EventNotifier
{
	public partial class MainWindow
	{
		private readonly ICollection<Event> _events = new Collection<Event>();
		private Point? _startMousePosition;
		private Point? _startWindowPosition;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += (sender, e) =>
			{
				var period = TimeSpan.FromMilliseconds(TimeSpan.FromMinutes(1).TotalMilliseconds / (2 * Settings.Default.GameTimeRatio));
				new Timer(obj =>
				{
					if (Dispatcher.CheckAccess())
						OnTimer();
					else
						Dispatcher.Invoke(OnTimer);
				}, null, TimeSpan.Zero, period);

				_events.Add(new Event
				{
					Name = "Ущелье Кровавой росы",
					StartTime = new DateTime(2015, 07, 28, 20, 0, 0),
					Period = TimeSpan.FromHours(24),
					RealTime = true
				});
				_events.Add(new Event
				{
					Name = "Ущелье Кровавой росы",
					StartTime = new DateTime(2015, 07, 28, 16, 0, 0),
					Period = TimeSpan.FromHours(24),
					RealTime = true
				});
				_events.Add(new Event
				{
					Name = "Кровавый разлом",
					StartTime = new DateTime(2015, 01, 1, 12, 0, 0),
					Period = TimeSpan.FromHours(24),
					RealTime = false
				});
				_events.Add(new Event
				{
					Name = "Призрачный разлом",
					StartTime = new DateTime(2015, 02, 2, 0, 0, 0),
					Period = TimeSpan.FromHours(24),
					RealTime = false
				});
				_events.Add(new Event
				{
					Name = "Око бури",
					StartTime = new DateTime(2015, 02, 2, 21, 0, 0),
					Period = TimeSpan.FromHours(24),
					RealTime = true
				});
			};
		}

		private void OnTimer()
		{
			_mskTime.Text = DateTime.Now.ToShortTimeString();
			_gameTime.Text = GameTime.Now.ToShortTimeString();

			Title = GameTime.Now.ToShortTimeString();

			RefreshItems();

			var nearEvents = _events.Where(e => e.RemainReal < Settings.Default.NotifyBefore);
			if (nearEvents.Any())
				new NotifyWindow(nearEvents).ShowDialog();
		}

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			var newEvent = new Event();
			if (new EventWindow(newEvent) { Owner = this }.ShowDialog() == true)
			{
				_events.Add(newEvent);
				RefreshItems();
			}
		}

		private void RefreshItems()
		{
			_eventItemsControl.ItemsSource = null;
			_eventItemsControl.ItemsSource = _events.OrderBy(ev => ev.RemainReal);
		}

		private void OnEventMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			if (e.ClickCount == 2)
			{
				var gameEvent = (Event) ((FrameworkElement) sender).DataContext;
				if (new EventWindow(gameEvent) { Owner = this }.ShowDialog() == true)
					RefreshItems();
				e.Handled = true;
			}
		}

		private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			if (_startMousePosition == null)
			{
				_startMousePosition = PointToScreen(e.GetPosition(this));
				_startWindowPosition = new Point(Left, Top);
				CaptureMouse();
				e.Handled = true;
			}
		}

		private void OnWindowMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Handled)
				return;

			if (_startMousePosition != null)
			{
				var mousePos = PointToScreen(e.GetPosition(this));
				var dx = mousePos.X - _startMousePosition.Value.X;
				var dy = mousePos.Y - _startMousePosition.Value.Y;
				Left = _startWindowPosition.Value.X + dx;
				Top = _startWindowPosition.Value.Y + dy;
				e.Handled = true;
			}
		}

		private void OnWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			if (_startMousePosition != null)
			{
				_startMousePosition = null;
				ReleaseMouseCapture();
				e.Handled = true;
			}
		}

		private void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			var gameEvent = (Event)((FrameworkElement)sender).DataContext;
			_events.Remove(gameEvent);
			RefreshItems();
		}
	}

	public class SuperConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType == typeof (string))
			{
				if (value is TimeSpan)
				{
					var timeSpan = (TimeSpan) value;
					var m = (int)timeSpan.TotalMinutes;

					if (timeSpan.TotalHours < 1)
						return m + " мин.";

					var mm = (m % 60).ToString();
					if (mm.Length < 2)
						mm = "0" + mm;
					return (m / 60).ToString("#") + " ч. " + mm + " мин.";
				}
			}

			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
