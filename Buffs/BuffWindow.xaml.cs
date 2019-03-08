using System;
using System.Windows;

namespace Buffs
{
	public partial class BuffWindow
	{
		private readonly Buff _buff;

		public BuffWindow()
		{
			InitializeComponent();

			Loaded += (sender, e) =>
			{
				_remainM.Focus();
				_remainM.Select(0, _remainM.Text.Length);
			};
		}

		public BuffWindow(Buff buff): this()
		{
			_buff = buff;
			if (buff == null) throw new ArgumentNullException("buff");

			if (buff.Remain != null)
			{
				var hours = (int)buff.Remain.Value.TotalHours;
				var minutes = (int) buff.Remain.Value.TotalMinutes - 60 * hours;
				_remainH.Text = hours.ToString();
				_remainM.Text = minutes.ToString();
			}
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			if (_buff.StartTime != null)
			{
				var h = int.Parse(_remainH.Text);
				var m = int.Parse(_remainM.Text);
				var ts = TimeSpan.FromMinutes(h * 60 + m);

				var finish = DateTime.Now + ts;
				_buff.Duration = finish - _buff.StartTime.Value;

				DialogResult = true;
			}
		}
	}
}
