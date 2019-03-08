using System;
using System.Windows;
using AA.Model;

namespace AA.Windows
{
	public partial class ProgressWindow
	{
		private readonly Progress _progress;

		public ProgressWindow()
		{
			InitializeComponent();
		}

		public ProgressWindow(Progress progress): this()
		{
			_progress = progress;

			progress.Start = DateTime.Now;

			tbStart.Text = progress.Start.ToShortTimeString();
			tbName.Text = progress.Name;
			
			var minutes = (int) progress.Duration.TotalMinutes;
			var days = minutes / (60 * 24);
			var hours = minutes / 60 - days * 24;
			minutes = minutes - days * 24 * 60 - hours * 60;
			tbDurDays.Text = days.ToString();
			tbDurHours.Text = hours.ToString();
			tbDurMinutes.Text = minutes.ToString();

			Loaded += (sender, e) =>
			{
				if (string.IsNullOrWhiteSpace(_progress.Name))
					tbName.Focus();
				else
					tbDurHours.Focus();
			};
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(tbName.Text))
					return;

				_progress.Name = tbName.Text;

				var d = int.Parse(tbDurDays.Text);
				var h = int.Parse(tbDurHours.Text);
				var m = int.Parse(tbDurMinutes.Text);
				_progress.Duration = TimeSpan.FromMinutes(d * 24 * 60 + h * 60 + m);

				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowWarning(error.Message);
			}
		}

		private void OnNowClick(object sender, RoutedEventArgs e)
		{
			_progress.Start = DateTime.Now;
			tbStart.Text = _progress.Start.ToShortTimeString();
		}
	}
}
