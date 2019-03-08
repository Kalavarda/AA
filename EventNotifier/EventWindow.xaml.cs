using System;

namespace EventNotifier
{
	public partial class EventWindow
	{
		private readonly Event _gameEvent;

		public EventWindow()
		{
			InitializeComponent();
		}

		public EventWindow(Event gameEvent): this()
		{
			_gameEvent = gameEvent;

			_name.Text = gameEvent.Name;
			_realTime.IsChecked = gameEvent.RealTime;
			_gameTime.IsChecked = !gameEvent.RealTime;

			_startTimeH.Text = gameEvent.StartTime.Hour.ToString();
			_startTimeM.Text = gameEvent.StartTime.Minute.ToString();

			_periodH.Text = ((int)gameEvent.Period.TotalMinutes / 60).ToString();
			_periodM.Text = (gameEvent.Period.TotalMinutes % 60).ToString();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(_name.Text))
					throw new Exception("Название не должно быть пустым");

				_gameEvent.Name = _name.Text;
				_gameEvent.RealTime = _realTime.IsChecked == true;

				_gameEvent.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(_startTimeH.Text), int.Parse(_startTimeM.Text), 0);
				_gameEvent.Period = TimeSpan.FromMinutes(int.Parse(_periodH.Text) * 60 + int.Parse(_periodM.Text));

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowError(error);
			}
		}
	}
}
